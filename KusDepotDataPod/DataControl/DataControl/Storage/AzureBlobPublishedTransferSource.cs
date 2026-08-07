namespace DataPodServices.DataControl;

internal sealed class AzureBlobPublishedTransferSource : IPublishedTransferSource
{
    private readonly IGrainFactory GrainFactory;

    private readonly IPublishedTransferRequestContext RequestContext;

    public AzureBlobPublishedTransferSource(IGrainFactory grainfactory , IPublishedTransferRequestContext requestcontext)
    {
        this.GrainFactory = grainfactory; this.RequestContext = requestcontext;
    }

    public async Task<PublishedTransferSnapshot?> TryLoadSnapshot(Guid itemid , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty) { return null; }

        String token = GetToken(); if(String.IsNullOrEmpty(token)) { return null; }

        String id = itemid.ToString();

        var dataConfigs = this.GrainFactory.GetGrain<IDataConfigs>(Guid.NewGuid().ToString());

        StorageSilo? silo = await dataConfigs.GetAuthorizedReadSilo(token,null,null,cancel); if(silo is null) { return null; }

        BlobClient objectBlob = new(silo.ConnectionString,id,id);

        BlobClient integrityBlob = new(silo.ConnectionString,id,BlobStrings.IntegrityBlobName);

        BlobClient streamBlob = new(silo.ConnectionString,id,BlobStrings.StreamBlobName);

        if(!await objectBlob.ExistsAsync(cancel)) { return null; }

        if(!await integrityBlob.ExistsAsync(cancel)) { return null; }

        Byte[] objectPayload = (await objectBlob.DownloadContentAsync(cancel)).Value.Content.ToArray();

        PublishedTransferIntegrity? integrity = JsonUtility.Deserialize<PublishedTransferIntegrity>((await integrityBlob.DownloadContentAsync(cancel)).Value.Content.ToArray());

        if(integrity is null || integrity.ObjectSHA512 is null || integrity.ObjectSHA512.Length != 64) { return null; }

        Byte[] objectHash = objectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(objectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(objectPayload);

        if(objectHash.AsSpan().SequenceEqual(integrity.ObjectSHA512) is false) { return null; }

        DataItem? item = DataItem.Deserialize(objectPayload); if(item is null) { return null; }

        Descriptor? descriptor = item.GetDescriptor(); if(descriptor is null || descriptor.ID != itemid) { return null; }

        Boolean contentStreamed = descriptor.ContentStreamed.HasValue ? descriptor.ContentStreamed.Value : false;

        Response<Boolean> streamBlobExists = await streamBlob.ExistsAsync(cancel);

        Byte[] streamHash = streamBlobExists.Value ? (integrity.StreamSHA512 ?? Array.Empty<Byte>()) : Array.Empty<Byte>(); Int64 streamLength = 0;

        if(streamBlobExists.Value is false && integrity.StreamSHA512 is not null && integrity.StreamSHA512.Length > 0) { return null; }

        if(streamBlobExists.Value)
        {
            if(streamHash.Length is not 0 and not 64) { return null; }

            BlobProperties properties = await streamBlob.GetPropertiesAsync(cancellationToken:cancel);

            streamLength = properties.ContentLength;

            contentStreamed = true;
        }

        return new()
        {
            ItemId = itemid,
            ObjectPayload = objectPayload,
            ObjectSHA512 = integrity.ObjectSHA512,
            StreamSHA512 = streamHash,
            StreamLength = streamLength,
            RealizedRanges = streamLength > 0 ? new[] { new DataItemTransferRange(){ Offset = 0 , Length = streamLength } } : Array.Empty<DataItemTransferRange>(),
            ObjectInfo = new DataItemTransferObjectInfo()
            {
                ObjectType = item.GetType().FullName,
                DataType = item.GetDataType(),
                Name = item.GetName(),
                ContentStreamed = contentStreamed,
            },
        };
    }

    public async Task<Stream?> OpenReadRange(PublishedTransferSnapshot snapshot , DataItemTransferRange range , CancellationToken cancel = default)
    {
        if(snapshot is null || snapshot.ItemId == Guid.Empty || range.Validate() is false || range.Length <= 0) { return null; }

        String token = GetToken(); if(String.IsNullOrEmpty(token)) { return null; }

        String id = snapshot.ItemId.ToString();

        var dataConfigs = this.GrainFactory.GetGrain<IDataConfigs>(Guid.NewGuid().ToString());

        StorageSilo? silo = await dataConfigs.GetAuthorizedReadSilo(token,null,null,cancel); if(silo is null) { return null; }

        BlobClient streamBlob = new(silo.ConnectionString,id,BlobStrings.StreamBlobName);

        if(!await streamBlob.ExistsAsync(cancel)) { return null; }

        BlobDownloadStreamingResult downloaded = await streamBlob.DownloadStreamingAsync(new BlobDownloadOptions(){ Range = new(range.Offset,range.Length) },cancel);

        return downloaded.Content;
    }

    private String GetToken()
    {
        return this.RequestContext.TryGetBearerToken(out String? token) ? token! : String.Empty;
    }
}
