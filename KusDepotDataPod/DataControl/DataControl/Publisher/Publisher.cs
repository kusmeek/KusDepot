namespace DataPodServices.DataControl;

internal static class Publisher
{
    internal static async Task<Boolean> HasPublishConflict(IGrainFactory grainfactory , StorageSilo silo , Descriptor descriptor , String id , String? traceid , String? spanid , CancellationToken cancel)
    {
        var catalog = grainfactory.GetGrain<ICatalogDB>(silo.CatalogName);

        BlobClient objectBlob = new(silo.ConnectionString,id,id);
        BlobClient streamBlob = new(silo.ConnectionString,id,BlobStrings.StreamBlobName);
        BlobClient integrityBlob = new(silo.ConnectionString,id,BlobStrings.IntegrityBlobName);

        return await catalog.Exists(descriptor,traceid,spanid,cancel) is not false ||
               (await objectBlob.ExistsAsync(cancel)).Value ||
               (await streamBlob.ExistsAsync(cancel)).Value ||
               (await integrityBlob.ExistsAsync(cancel)).Value;
    }

    internal static async Task<Boolean> PublishAsync(IGrainFactory grainfactory , StorageSilo silo , Descriptor descriptor, Byte[] objectpayload , Byte[] objectSHA512 , Stream? streampayload , Byte[]? streamSHA512 , String? traceid , String? spanid , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        String id = descriptor.ID?.ToString() ?? String.Empty; if(String.IsNullOrEmpty(id)) { return false; }

        if(objectSHA512 is null || objectSHA512.Length == 0) { return false; }

        if(streampayload is not null && (streamSHA512 is null || streamSHA512.Length == 0)) { return false; }

        BlobContainerClient container = new(silo.ConnectionString,id);
        BlobClient objectBlob         = new(silo.ConnectionString,id,id);
        BlobClient streamBlob         = new(silo.ConnectionString,id,BlobStrings.StreamBlobName);
        BlobClient integrityBlob      = new(silo.ConnectionString,id,BlobStrings.IntegrityBlobName);
        BlobUploadOptions options     = new() { TransferValidation = new() { ChecksumAlgorithm = StorageChecksumAlgorithm.StorageCrc64 } };
        Boolean objectUploaded = false; Boolean integrityUploaded = false; Boolean streamUploaded = false;

        try
        {
            await container.CreateIfNotExistsAsync(cancellationToken:cancel);

            await objectBlob.UploadAsync(BinaryData.FromBytes(objectpayload),options,cancel);

            objectUploaded = true;

            PublishedTransferIntegrity integrity = new()
            {
                ObjectSHA512 = objectSHA512,

                StreamSHA512 = streamSHA512 ?? Array.Empty<Byte>(),
            };

            Byte[] integrityPayload = JsonUtility.Serialize(integrity);

            await integrityBlob.UploadAsync(BinaryData.FromBytes(integrityPayload),options,cancel);

            integrityUploaded = true;

            if(streampayload is not null)
            {
                if(streampayload.CanSeek) { streampayload.Position = 0; }

                await streamBlob.UploadAsync(streampayload,options,cancel);

                streamUploaded = true;
            }

            var catalog = grainfactory.GetGrain<ICatalogDB>(silo.CatalogName);

            if(await catalog.AddUpdate(descriptor,traceid,spanid,cancel))
            {
                return true;
            }
        }
        catch ( OperationCanceledException ) when(cancel.IsCancellationRequested)
        {
            await CleanupPublishArtifacts(id,container,objectBlob,integrityBlob,streamBlob,objectUploaded,integrityUploaded,streamUploaded);

            throw;
        }
        catch ( Exception _ )
        {
            Log.Error(_,PublishFailID,id);
        }

        await CleanupPublishArtifacts(id,container,objectBlob,integrityBlob,streamBlob,objectUploaded,integrityUploaded,streamUploaded);

        return false;
    }

    private static async Task CleanupPublishArtifacts(String id , BlobContainerClient container , BlobClient objectblob , BlobClient integrityblob , BlobClient streamblob , Boolean objectuploaded , Boolean integrityuploaded , Boolean streamuploaded)
    {
        if(!(objectuploaded || integrityuploaded || streamuploaded)) { return; }

        Log.Warning(PublishCleanupStartID,id);

        Boolean cleanupFailed = false;

        try { if(streamuploaded) { await streamblob.DeleteIfExistsAsync(cancellationToken:CancellationToken.None); } }

        catch ( Exception _ ) { cleanupFailed = true; Log.Warning(_,PublishCleanupFailID,id,BlobStrings.StreamBlobName); }

        try { if(integrityuploaded) { await integrityblob.DeleteIfExistsAsync(cancellationToken:CancellationToken.None); } }

        catch ( Exception _ ) { cleanupFailed = true; Log.Warning(_,PublishCleanupFailID,id,BlobStrings.IntegrityBlobName); }

        try { if(objectuploaded) { await objectblob.DeleteIfExistsAsync(cancellationToken:CancellationToken.None); } }

        catch ( Exception _ ) { cleanupFailed = true; Log.Warning(_,PublishCleanupFailID,id,id); }

        try { await container.DeleteIfExistsAsync(cancellationToken:CancellationToken.None); }

        catch ( Exception _ ) { cleanupFailed = true; Log.Warning(_,PublishCleanupFailID,id,"Blob Container"); }

        if(cleanupFailed is false) { Log.Information(PublishCleanupSuccessID,id); }
    }
}
