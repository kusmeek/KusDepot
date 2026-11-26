namespace DataPodServices.DataControl;

/**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/main/*'/>*/
internal static class Publisher
{
    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="HasPublishConflictAsync"]/*'/>*/
    internal static async Task<Boolean> HasPublishConflictAsync(IGrainFactory grainfactory , StorageSilo silo , Descriptor descriptor , Guid itemInstanceToken , PublishArtifactExpectation expectation , String id , TraceContext? tracecontext , CancellationToken cancel)
    {
        var catalog = grainfactory.GetGrain<ICatalogDB>(silo.CatalogName);

        TransferInstanceStoragePath paths = TransferStoragePath.ForInstance(itemInstanceToken);
        BlobClient objectBlob = new(silo.ConnectionString,id,paths.PublishedObjectName);
        BlobClient streamBlob = new(silo.ConnectionString,id,paths.PublishedStreamName);
        BlobClient integrityBlob = new(silo.ConnectionString,id,paths.PublishedIntegrityName);

        if(await catalog.Exists(descriptor.ID,tracecontext,cancel) is not false) { return true; }

        Boolean objectExists = (await objectBlob.ExistsAsync(cancel)).Value;
        Boolean streamExists = (await streamBlob.ExistsAsync(cancel)).Value;
        Boolean integrityExists = (await integrityBlob.ExistsAsync(cancel)).Value;

        if(streamExists && expectation.PublishStream is false) { return true; }

        if(objectExists && await VerifyObjectAsync(objectBlob,expectation.ObjectLength,expectation.ObjectSHA512,cancel) is false) { return true; }

        if(streamExists && expectation.StreamIntegrity is null) { return true; }

        if(integrityExists)
        {
            PublishedTransferIntegrity integrity = CreateIntegrityArtifact(expectation.ObjectSHA512,expectation.PublishStream ? expectation.StreamIntegrity : null,expectation.PublishStream ? expectation.StreamSegments : null);

            if(await VerifyIntegrityAsync(integrityBlob,JsonUtility.Serialize(integrity),cancel) is false) { return true; }
        }

        return false;
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="PublishArtifactsAsync"]/*'/>*/
    internal static async Task<Boolean> PublishArtifactsAsync(ITenantTransferStorage storage , TenantTransferBlobReference objectpayload , Byte[] objectSHA512 , TransferBlockPlan? streamplan , Boolean publishstream , Int64 streamlength , TransferContentIntegrity? streamIntegrity , IReadOnlyList<TransferContentIntegritySegment>? streamSegments , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(storage); ArgumentNullException.ThrowIfNull(objectpayload);

        if(objectSHA512 is null || objectSHA512.Length == 0 || streamlength < 0 ||
            publishstream is false && (streamplan is not null || streamlength is not 0 || streamIntegrity is not null) ||
            publishstream && (streamplan is null || streamplan.TotalLength != streamlength || streamIntegrity is null || !streamIntegrity.Validate() || streamSegments is null)) { return false; }

        PublishedTransferIntegrity integrity;

        try { integrity = CreateIntegrityArtifact(objectSHA512,publishstream ? streamIntegrity : null,publishstream ? streamSegments : null); } catch ( ArgumentException ) { return false; }

        PublishedArtifactReference? publishedObject = await storage.PublishObjectAsync(objectpayload,cancel);

        if(publishedObject is null || publishedObject.Length != objectpayload.Length || publishedObject.SHA512.AsSpan().SequenceEqual(objectSHA512) is false) { return false; }

        if(publishstream)
        {
            PublishedArtifactReference? publishedStream = streamlength is 0
                ? await storage.AssembleStreamAsync(new TransferBlockPlan { Blocks = Array.Empty<TransferBlock>() , LogicalRanges = Array.Empty<TransferRangeReference>() , TotalLength = 0 },cancel)
                : await storage.AssembleStreamAsync(streamplan!,cancel);

            if(publishedStream is null || publishedStream.Length != streamlength) { return false; }
        }

        await using MemoryStream integrityPayload = new(JsonUtility.Serialize(integrity),false);

        return await storage.SaveIntegrityAsync(integrityPayload,cancel) is not null;
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="CreateIntegrityArtifact"]/*'/>*/
    internal static PublishedTransferIntegrity CreateIntegrityArtifact(Byte[] objectSHA512 , TransferContentIntegrity? streamIntegrity , IReadOnlyList<TransferContentIntegritySegment>? streamSegments)
    {
        TransferContentIntegritySegment[] segments = streamSegments?.ToArray() ?? Array.Empty<TransferContentIntegritySegment>();

        if(streamIntegrity is null)
        {
            if(segments.Length is not 0) { throw new ArgumentException("Integrity segments require a stream integrity summary.",nameof(streamSegments)); }
        }
        else
        {
            if(!streamIntegrity.Validate()) { throw new ArgumentException("A valid stream integrity summary is required.",nameof(streamIntegrity)); }

            TransferContentIntegrity calculated = TransferContentIntegrityCalculator.Compute(segments);

            if(!String.Equals(calculated.Algorithm,streamIntegrity.Algorithm,Ordinal) ||
               calculated.Length != streamIntegrity.Length ||
               calculated.SegmentCount != streamIntegrity.SegmentCount ||
               calculated.Digest.AsSpan().SequenceEqual(streamIntegrity.Digest) is false)
            {
                throw new ArgumentException("Integrity segments do not match the stream integrity summary.",nameof(streamSegments));
            }
        }

        return new PublishedTransferIntegrity { ObjectSHA512 = objectSHA512.ToArray() , Integrity = streamIntegrity , Segments = segments };
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="VerifyObjectAsync"]/*'/>*/
    private static async Task<Boolean> VerifyObjectAsync(BlobClient blob , Int64 expectedLength , Byte[] expectedHash , CancellationToken cancel)
    {
        BlobProperties properties = await blob.GetPropertiesAsync(cancellationToken:cancel);

        return VerifyMetadata(properties,expectedHash,expectedLength);
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="VerifyIntegrityAsync"]/*'/>*/
    private static async Task<Boolean> VerifyIntegrityAsync(BlobClient blob , Byte[] expectedPayload , CancellationToken cancel)
    {
        Byte[] existing = (await blob.DownloadContentAsync(cancel)).Value.Content.ToArray();

        return existing.AsSpan().SequenceEqual(expectedPayload);
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="VerifyMetadata"]/*'/>*/
    private static Boolean VerifyMetadata(BlobProperties properties , Byte[] expectedHash , Int64 expectedLength)
    {
        return properties.ContentLength == expectedLength &&
               expectedHash is not null &&
               properties.Metadata.TryGetValue(SHA512MetadataKey,out String? actualHash) &&
               properties.Metadata.TryGetValue(LengthMetadataKey,out String? actualLength) &&
               String.Equals(actualHash,Convert.ToHexString(expectedHash),OrdinalIgnoreCase) &&
               String.Equals(actualLength,expectedLength.ToString(CultureInfo.InvariantCulture),Ordinal);
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/method[@name="UpdateCatalogAsync"]/*'/>*/
    internal static Task<Boolean> UpdateCatalogAsync(IGrainFactory grainfactory , StorageSilo silo , Descriptor descriptor , Guid itemInstanceToken , TraceContext? tracecontext , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        return grainfactory.GetGrain<ICatalogDB>(silo.CatalogName).AddUpdate(descriptor,itemInstanceToken,tracecontext,cancel);
    }

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/field[@name="SHA512MetadataKey"]/*'/>*/
    private const String SHA512MetadataKey = "datacontrol_sha512";

    /**<include file='Publisher.xml' path='Publisher/class[@name="Publisher"]/field[@name="LengthMetadataKey"]/*'/>*/
    private const String LengthMetadataKey = "datacontrol_length";
}
