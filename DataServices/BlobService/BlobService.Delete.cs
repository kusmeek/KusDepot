namespace KusDepot.Data.Services.Blob;

public sealed partial class BlobService
{
    ///<inheritdoc/>
    public async Task<Boolean> Delete(CancellationToken cancel = default)
    {
        try
        {
            if(this.id is null || this.connection is null) { Log.Error(BlobStrings.BadArg); return false; }

            if(this.blobclient is null) { this.blobclient = new BlobClient(this.connection,this.id,this.id);

            if(this.version is not null) { this.blobclient = this.blobclient.WithVersion(this.version); } }

            if(String.Equals(this.blobclient.Name,this.id,StringComparison.Ordinal))
            {
                if(await this.blobclient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots,cancellationToken:cancel))
                {
                    this.containerclient ??= new BlobContainerClient(this.connection,this.id);

                    BlobClient f = this.containerclient.GetBlobClient("FILE"); if(await f.ExistsAsync(cancel)) { await f.DeleteIfExistsAsync(cancellationToken:cancel); }

                    Boolean any = false; await foreach(var b in this.containerclient.GetBlobsAsync(cancellationToken:cancel)) { any = true; break; }

                    if(any is false)
                    {
                        if(await this.containerclient.DeleteIfExistsAsync(cancellationToken:cancel)) { Log.Information(BlobStrings.DeleteContainerSuccessID,this.id); }
                    }
                    if(this.version is not null) { Log.Information(BlobStrings.DeleteSuccessIDVersion,this.id,this.version); return true; }

                    Log.Information(BlobStrings.DeleteSuccessID,this.id); return true;
                }
            }
            if(this.version is not null) { Log.Error(BlobStrings.DeleteFailIDVersion,this.id,this.version); return false; }

            Log.Error(BlobStrings.DeleteFailID,this.id); return false;
        }
        catch ( Exception _ )
        {
            if(this.version is not null) { Log.Error(_,BlobStrings.DeleteFailIDVersion,this.id,this.version); return false; }

            Log.Error(_,BlobStrings.DeleteFailID,this.id); return false;
        }
    }
}