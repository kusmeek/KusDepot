namespace KusDepot.Data;

internal sealed partial class Blob
{
    public async Task<Boolean> Delete(String? connection , String? id , String? version = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.DeleteStart(id,version); using DiagnosticActivity? ___ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null || connection is null) { Log.Error(BadArg); SetErr(___); ETW.Log.DeleteError(BadArg,id,version); return false; }

            BlobClient _ = new BlobClient(connection,id,id); if(version is not null) { _ = _.WithVersion(version); }

            if(String.Equals(_.Name,id,StringComparison.Ordinal))
            {
                if(await _.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots))
                {
                    BlobContainerClient __ = new BlobContainerClient(connection,id);

                    BlobClient _f = __.GetBlobClient("FILE"); if(await _f.ExistsAsync()) { await _f.DeleteIfExistsAsync(); }

                    if(await __.GetBlobsAsync().AnyAsync() is false)
                    {
                        if(await __.DeleteIfExistsAsync()) { Log.Information(DeleteContainerSuccessID,id); SetOk(___); ETW.Log.DeleteContainerSuccess(id); }
                    }
                    if(version is not null) { Log.Information(DeleteSuccessIDVersion,id,version); SetOk(___); ETW.Log.DeleteSuccess(id,version); return true; }

                    Log.Information(DeleteSuccessID,id); SetOk(___); ETW.Log.DeleteSuccess(id); return true;
                }
            }
            if(version is not null) { Log.Error(DeleteFailIDVersion,id,version); SetErr(___); ETW.Log.DeleteError(DeleteFail,id,version); return false; }

            Log.Error(DeleteFailID,id); SetErr(___); ETW.Log.DeleteError(DeleteFail,id); return false;
        }
        catch ( Exception _ )
        {
            if(version is not null) { Log.Error(_,DeleteFailIDVersion,id,version); ETW.Log.DeleteError(_.Message,id,version); return false; }

            Log.Error(_,DeleteFailID,id); ETW.Log.DeleteError(_.Message,id); return false;
        }
    }
}