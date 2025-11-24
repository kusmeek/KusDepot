namespace KusDepot.Data;

internal sealed partial class Blob
{
    public async Task<Byte[]?> Get(String? connection , String? id , String? version = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetStart(id,version); using DiagnosticActivity? ___ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null || connection is null) { Log.Error(BadArg); SetErr(___); ETW.Log.GetError(BadArg,id,version); return null; }

            BlobClient _ = new BlobClient(connection,id,id); if(version is not null) { _ = _.WithVersion(version); }

            if(String.Equals(_.Name,id,StringComparison.Ordinal))
            {
                MemoryStream __ = new MemoryStream(); await _.DownloadToAsync(__);

                if(version is not null) { Log.Information(GetSuccessIDVersion,id,version); SetOk(___); ETW.Log.GetSuccess(id,version); return __.ToArray(); }

                Log.Information(GetSuccessID,id); SetOk(___); ETW.Log.GetSuccess(id); return __.ToArray();
            }
            if(version is not null) { Log.Error(GetFailIDVersion,id,version); SetErr(___); ETW.Log.GetError(GetFail,id,version); return null;; }

            Log.Error(GetFailID,id); SetErr(___); ETW.Log.GetError(GetFail,id); return null;;
        }
        catch ( Exception _ )
        {
            if(version is not null) { Log.Error(_,GetFailIDVersion,id,version); ETW.Log.GetError(_.Message,id,version); return null; }

            Log.Error(_,GetFailID,id); ETW.Log.GetError(_.Message,id); return null;
        }
    }
}