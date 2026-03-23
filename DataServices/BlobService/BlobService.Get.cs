namespace KusDepot.Data.Services.Blob;

public sealed partial class BlobService
{
    ///<inheritdoc/>
    public async Task<Byte[]?> Get(CancellationToken cancel = default)
    {
        try
        {
            if(this.id is null || this.connection is null) { Log.Error(BlobStrings.BadArg); return null; }

            if(this.blobclient is null) { this.blobclient = new BlobClient(this.connection,this.id,this.id);

            if(this.version is not null) { this.blobclient = this.blobclient.WithVersion(this.version); } }

            if(String.Equals(this.blobclient.Name,this.id,StringComparison.Ordinal))
            {
                using MemoryStream m = new(); await this.blobclient.DownloadToAsync(m,cancel);

                if(this.version is not null) { Log.Information(BlobStrings.GetSuccessIDVersion,this.id,this.version); return m.ToArray(); }

                Log.Information(BlobStrings.GetSuccessID,this.id); return m.ToArray();
            }
            if(this.version is not null) { Log.Error(BlobStrings.GetFailIDVersion,this.id,this.version); return null; }

            Log.Error(BlobStrings.GetFailID,this.id); return null;
        }
        catch ( Exception _ )
        {
            if(this.version is not null) { Log.Error(_,BlobStrings.GetFailIDVersion,this.id,this.version); return null; }

            Log.Error(_,BlobStrings.GetFailID,this.id); return null;
        }
    }
}