namespace DataPodServices.DataControl;

/**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/main/*'/>*/
internal sealed class AzureBlobPublishedTransferSource : IPublishedTransferSource
{
    /**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/field[@name="DataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient DataConfigs;

    /**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/field[@name="RequestContext"]/*'/>*/
    private readonly IPublishedTransferRequestContext RequestContext;

    /**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/constructor[@name="AzureBlobPublishedTransferSource"]/*'/>*/
    public AzureBlobPublishedTransferSource(IDataControlDataConfigsClient dataconfigs , IPublishedTransferRequestContext requestcontext)
    {
        this.DataConfigs = dataconfigs; this.RequestContext = requestcontext;
    }

    /**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/method[@name="TryLoadSnapshot"]/*'/>*/
    public async Task<PublishedTransferSnapshot?> TryLoadSnapshot(Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty || itemInstanceToken == Guid.Empty) { return null; }

        String token = GetToken(); if(String.IsNullOrEmpty(token)) { return null; }

        String id = itemid.ToString();

        StorageSilo? silo = await this.DataConfigs.ResolveStorageAsync(token,CatalogAccess.Read,null,cancel); if(silo is null) { return null; }

        TransferInstanceStoragePath paths = TransferStoragePath.ForInstance(itemInstanceToken);

        BlobClient objectBlob = new(silo.ConnectionString,id,paths.PublishedObjectName);

        BlobClient integrityBlob = new(silo.ConnectionString,id,paths.PublishedIntegrityName);

        BlobClient streamBlob = new(silo.ConnectionString,id,paths.PublishedStreamName);

        if(!await objectBlob.ExistsAsync(cancel)) { return null; }

        if(!await integrityBlob.ExistsAsync(cancel)) { return null; }

        BlobDownloadStreamingResult objectDownload = await objectBlob.DownloadStreamingAsync(cancellationToken:cancel);

        await using ContentLengthStream objectStream = new(objectDownload.Content,objectDownload.Details.ContentLength);

        Byte[] objectPayload = await ObjectPayloadReader.ReadAsync(objectStream,cancel);

        PublishedTransferIntegrity? integrity = JsonUtility.Deserialize<PublishedTransferIntegrity>((await integrityBlob.DownloadContentAsync(cancel)).Value.Content.ToArray());

        if(integrity is null || integrity.ObjectSHA512 is null || integrity.ObjectSHA512.Length != 64 || integrity.Segments is null) { return null; }

        if(integrity.Integrity is not null)
        {
            TransferContentIntegrity calculated;

            try { calculated = TransferContentIntegrityCalculator.Compute(integrity.Segments); }
            catch ( ArgumentException ) { return null; }

            if(!String.Equals(calculated.Algorithm,integrity.Integrity.Algorithm,Ordinal) ||
               calculated.Length != integrity.Integrity.Length ||
               calculated.SegmentCount != integrity.Integrity.SegmentCount ||
               calculated.Digest.AsSpan().SequenceEqual(integrity.Integrity.Digest) is false)
            {
                return null;
            }
        }
        else if(integrity.Segments.Length is not 0) { return null; }

        Byte[] objectHash = objectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(objectPayload,MiB,cancel)
            : SHA512.HashData(objectPayload);

        if(objectHash.AsSpan().SequenceEqual(integrity.ObjectSHA512) is false) { return null; }

        DataItem? item = DataItem.Deserialize(objectPayload); if(item is null) { return null; }

        Descriptor? descriptor = item.GetDescriptor(); if(descriptor is null || descriptor.ID != itemid) { return null; }

        Boolean contentStreamed = descriptor.ContentStreamed is true;

        Response<Boolean> streamBlobExists = await streamBlob.ExistsAsync(cancel);

        if(streamBlobExists.Value != (integrity.Integrity is not null)) { return null; }

        contentStreamed = streamBlobExists.Value;

        TransferContentIntegrity? streamIntegrity = contentStreamed ? integrity.Integrity : null; Int64 streamLength = 0;

        if(contentStreamed)
        {
            if(streamIntegrity is null || !streamIntegrity.Validate()) { return null; }

            BlobProperties properties = await streamBlob.GetPropertiesAsync(cancellationToken:cancel);

            streamLength = properties.ContentLength;

            if(streamIntegrity.Length != streamLength) { return null; }
        }

        return new()
        {
            ItemId = itemid,
            ItemInstanceToken = itemInstanceToken,
            ObjectPayload = objectPayload,
            ObjectSHA512 = integrity.ObjectSHA512,
            Integrity = streamIntegrity,
            IntegritySegments = streamIntegrity is not null ? integrity.Segments : Array.Empty<TransferContentIntegritySegment>(),
            StreamLength = streamLength,
            RealizedRanges = streamLength > 0 ? new[] { new DataItemTransferRange() { Offset = 0 , Length = streamLength } } : Array.Empty<DataItemTransferRange>(),
            ObjectInfo = new DataItemTransferObjectInfo()
            {
                ObjectType = item.GetType().FullName,
                DataType = item.GetDataType(),
                Name = item.GetName(),
                ContentStreamed = contentStreamed,
            },
        };
    }

    /**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/method[@name="OpenReadRange"]/*'/>*/
    public async Task<Stream?> OpenReadRange(PublishedTransferSnapshot snapshot , DataItemTransferRange range , CancellationToken cancel = default)
    {
        if(snapshot is null || snapshot.ItemId == Guid.Empty || snapshot.ItemInstanceToken == Guid.Empty || range.Validate() is false || range.Length <= 0) { return null; }

        String token = GetToken(); if(String.IsNullOrEmpty(token)) { return null; }

        String id = snapshot.ItemId.ToString();

        StorageSilo? silo = await this.DataConfigs.ResolveStorageAsync(token,CatalogAccess.Read,null,cancel); if(silo is null) { return null; }

        String blobName = TransferStoragePath.ForInstance(snapshot.ItemInstanceToken).PublishedStreamName;

        BlobClient streamBlob = new(silo.ConnectionString,id,blobName);

        if(!await streamBlob.ExistsAsync(cancel)) { return null; }

        BlobDownloadStreamingResult downloaded = await streamBlob.DownloadStreamingAsync(new BlobDownloadOptions(){ Range = new(range.Offset,range.Length) },cancel);

        return downloaded.Content;
    }

    /**<include file='AzureBlobPublishedTransferSource.xml' path='AzureBlobPublishedTransferSource/class[@name="AzureBlobPublishedTransferSource"]/method[@name="GetToken"]/*'/>*/
    private String GetToken()
    {
        return this.RequestContext.TryGetBearerToken(out String? token) ? token! : String.Empty;
    }
}
