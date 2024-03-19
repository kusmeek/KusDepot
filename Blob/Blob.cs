namespace KusDepot.Data;

[StatePersistence(StatePersistence.None)]
internal sealed partial class Blob : Actor , IBlob
{
    public Blob(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    public Task<Boolean> Delete(String? connection , String? id , String? version = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? ___ = StartDiagnostic("Delete",traceid,spanid)?.AddTag("id",id);

            if(id is null || connection is null) { Log.Error(BadArg); SetErr(___);  return Task.FromResult(false); }

            BlobClient _ = new BlobClient(connection,id,id);

            if(version is not null) { _ = _.WithVersion(version); }

            if(String.Equals(_.Name,id,StringComparison.Ordinal))
            {
                if(_.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots).Value)
                {
                    BlobContainerClient __ = new BlobContainerClient(connection,id);

                    if(Equals(__.GetBlobs().ToArray().Length,0))
                    {
                        if(__.DeleteIfExists().Value) { Log.Information(DeleteContainerSuccessID,id); SetOk(___); return Task.FromResult(true); }
                    }
                    if(version is not null) { Log.Information(DeleteSuccessIDVersion,id,version); SetOk(___); return Task.FromResult(true); }

                    Log.Information(DeleteSuccessID,id); SetOk(___); return Task.FromResult(true);
                }
                if(version is not null) { Log.Error(DeleteFailIDVersion,id,version); SetErr(___); return Task.FromResult(false); }

                Log.Error(DeleteFailID,id); SetErr(___); return Task.FromResult(false);
            }
            if(version is not null) { Log.Error(DeleteFailIDVersion,id,version); SetErr(___); return Task.FromResult(false); }

            Log.Error(DeleteFailID,id); SetErr(___); return Task.FromResult(false);
        }
        catch ( Exception _ )
        {
            if(version is not null) { Log.Error(_,DeleteFailIDVersion,id,version); return Task.FromResult(false); }

            Log.Error(_,DeleteFailID,id); return Task.FromResult(false);
        }
    }

    public Task<Boolean?> Exists(String? connection , String? id , String? version = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("Exists",traceid,spanid)?.AddTag("id",id);

            if(id is null || connection is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult<Boolean?>(null); }

            BlobClient _ = new BlobClient(connection,id,id);

            if(version is not null) { _ = _.WithVersion(version); }

            Boolean r = _.Exists().Value; SetOk(__); return Task.FromResult<Boolean?>(r);
        }
        catch ( Exception _ )
        {
            if(version is not null) { Log.Error(_,ExistsFailIDVersion,id,version); return Task.FromResult<Boolean?>(null); }

            Log.Error(_,ExistsFailID,id); return Task.FromResult<Boolean?>(null);
        }
    }

    public Task<Byte[]?> Get(String? connection , String? id , String? version = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? ___ = StartDiagnostic("Get",traceid,spanid)?.AddTag("id",id);

            if(id is null || connection is null) { Log.Error(BadArg); SetErr(___); return Task.FromResult<Byte[]?>(null); }

            BlobClient _ = new BlobClient(connection,id,id);

            if(version is not null) { _ = _.WithVersion(version); }

            if(String.Equals(_.Name,id,StringComparison.Ordinal))
            {
                MemoryStream __ = new MemoryStream(); _.DownloadTo(__);

                if(version is not null) { Log.Information(GetSuccessIDVersion,id,version); SetOk(___); return Task.FromResult<Byte[]?>(__.ToArray()); }

                Log.Information(GetSuccessID,id); SetOk(___); return Task.FromResult<Byte[]?>(__.ToArray());
            }
            if(version is not null) { Log.Error(GetFailIDVersion,id,version); SetErr(___); return Task.FromResult<Byte[]?>(null);; }

            Log.Error(GetFailID,id); SetErr(___); return Task.FromResult<Byte[]?>(null);;
        }
        catch ( Exception _ )
        {
            if(version is not null) { Log.Error(_,GetFailIDVersion,id,version); return Task.FromResult<Byte[]?>(null); }

            Log.Error(_,GetFailID,id); return Task.FromResult<Byte[]?>(null);
        }
    }

    public Task<Boolean> Store(String? connection , String? id , Byte[]? it , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Store",traceid,spanid)?.AddTag("id",id);

            if(id is null || it is null || connection is null) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.Exists(connection,id).Result is true) { Log.Error(StoreConflictID,id); SetErr(_); return Task.FromResult(false); }

            BlobContainerClient _0 = new BlobContainerClient(connection,id); _0.CreateIfNotExists(); BlobClient _1 = _0.GetBlobClient(id);

            BlobUploadOptions _2 = new BlobUploadOptions() { HttpHeaders = new BlobHttpHeaders(){ ContentHash = MD5.HashData(it) } };

            _1.Upload(BinaryData.FromBytes(it),_2); Log.Information(StoreSuccessID,id); SetOk(_); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,StoreFailID,id); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync() { Log.Information(Activated); return Task.FromResult(true); }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }
}