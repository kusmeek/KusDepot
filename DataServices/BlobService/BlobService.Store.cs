namespace KusDepot.Data.Services.Blob;

public sealed partial class BlobService
{
    ///<inheritdoc/>
    public async Task<Boolean> Store(Byte[]? it , CancellationToken cancel = default)
    {
        try
        {
            if(this.id is null || it is null || this.connection is null) { Log.Error(BlobStrings.BadArg); return false; }

            if(await this.Exists(cancel) is true) { Log.Error(BlobStrings.StoreConflictID,this.id); return false; }

            this.containerclient ??= new BlobContainerClient(this.connection,this.id);

            await this.containerclient.CreateIfNotExistsAsync(cancellationToken:cancel);

            BlobUploadOptions o = new(){TransferValidation = new(){ChecksumAlgorithm = StorageChecksumAlgorithm.StorageCrc64}};

            BlobClient b = this.containerclient.GetBlobClient(this.id); await b.UploadAsync(BinaryData.FromBytes(it),o,cancel);

            Log.Information(BlobStrings.StoreSuccessID,this.id); return true;
        }
        catch ( Exception _ ) { Log.Error(_,BlobStrings.StoreFailID,this.id); return false; }
    }
}