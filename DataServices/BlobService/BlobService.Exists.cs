namespace KusDepot.Data.Services.Blob;

public sealed partial class BlobService
{
    ///<inheritdoc/>
    public async Task<Boolean?> Exists(CancellationToken cancel = default)
    {
        try
        {
            if(this.id is null || this.connection is null) { Log.Error(BlobStrings.BadArg); return null; }

            if(this.blobclient is null) { this.blobclient = new BlobClient(this.connection,this.id,this.id);

            if(this.version is not null) { this.blobclient = this.blobclient.WithVersion(this.version); } }

            Boolean _ = await this.blobclient.ExistsAsync(cancel);

            if(this.version is not null) { Log.Information(BlobStrings.ExistsSuccessIDVersion,this.id,this.version); return _; }

            Log.Information(BlobStrings.ExistsSuccessID,this.id); return _;
        }
        catch ( Exception _ )
        {
            if(this.version is not null) { Log.Error(_,BlobStrings.ExistsFailIDVersion,this.id,this.version); return null; }

            Log.Error(_,BlobStrings.ExistsFailID,this.id); return null;
        }
    }
}