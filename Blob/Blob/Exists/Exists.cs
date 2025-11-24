namespace KusDepot.Data;

internal sealed partial class Blob
{
    public async Task<Boolean?> Exists(String? connection , String? id , String? version = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.ExistsStart(id,version); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null || connection is null) { Log.Error(BadArg); SetErr(__); ETW.Log.ExistsError(BadArg,id,version); return null; }

            BlobClient _ = new BlobClient(connection,id,id); if(version is not null) { _ = _.WithVersion(version); }

            Boolean r = await _.ExistsAsync();

            if(version is not null) { Log.Information(ExistsSuccessIDVersion,id,version); SetOk(__); ETW.Log.ExistsSuccess(id,version); return r; }

            Log.Information(ExistsSuccessID,id); SetOk(__); ETW.Log.ExistsSuccess(id); return r;
        }
        catch ( Exception _ )
        {
            if(version is not null) { Log.Error(_,ExistsFailIDVersion,id,version); ETW.Log.ExistsError(_.Message,id,version); return null; }

            Log.Error(_,ExistsFailID,id); ETW.Log.ExistsError(_.Message,id); return null;
        }
    }
}