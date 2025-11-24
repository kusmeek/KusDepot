namespace KusDepot.Data;

public sealed partial class DataControl
{
    private void MapGetStream(WebApplication application)
    {
        application.MapGet("GetStream/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => { return GetStream(id,dc,hc); })
                   .WithName("GetStream").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> GetStream(String? id , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            ETW.Log.GetStreamStart(id);

            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString(); FileStream f;

            if(String.IsNullOrEmpty(id)) { Log.Error(BadArgID,id); SetErr(_); ETW.Log.GetStreamError(BadArgID,id); return BadRequest(BadArg); }

            if(String.IsNullOrEmpty(t)) { Log.Error(GetUnAuthID,id); SetErr(_); ETW.Log.GetStreamError(GetUnAuth,id); return Unauthorized(); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(GetUnAuth,id); SetErr(_); ETW.Log.GetStreamError(GetUnAuth,id); return Unauthorized(); }

            BlobClient bl = new(s.ConnectionString,id,id); BlobClient bls = new(s.ConnectionString,id,StreamBlobName); BlobClient blh = new(s.ConnectionString,id,"StreamSHA512");

            String p = Path.Combine(DiskStorePath,id); if(!await bl.ExistsAsync() || !await bls.ExistsAsync() || !await blh.ExistsAsync()) { Log.Error(GetNotFoundID,id); SetErr(_); ETW.Log.GetStreamError(GetNotFound,id); return Results.NotFound(id); }

            using MemoryStream m = new(); await bl.DownloadToAsync(m,hc.RequestAborted); String? it = m.ToArray().ToBase64FromByteArray(); if(String.IsNullOrEmpty(it)) { Log.Error(GetFailID,id); SetErr(_); ETW.Log.GetStreamError(GetFail,id); return InternalError(); }

            if(File.Exists(p) is true)
            {
                f = new(p,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite|FileShare.Delete,StreamBufferSize,FileOptions.Asynchronous|FileOptions.SequentialScan);

                if(String.Equals((await blh.DownloadContentAsync()).Value.Content.ToString(),SHA512.HashData(f).ToBase64FromByteArray(),Ordinal) is not true)
                {
                    File.Delete(p); await bls.DownloadToAsync(p,new(){TransferValidation = new (){ChecksumAlgorithm = StorageChecksumAlgorithm.StorageCrc64}},hc.RequestAborted);
                }

                await f.DisposeAsync();
            }
            else
            {
                await bls.DownloadToAsync(p,new(){TransferValidation = new (){ChecksumAlgorithm = StorageChecksumAlgorithm.StorageCrc64}},hc.RequestAborted);
            }

            if(File.Exists(p) is false) { Log.Error(GetFailID,id); SetErr(_); ETW.Log.GetStreamError(GetFail,id); return InternalError(); }

            f = new(p,FileMode.Open,FileAccess.Read,FileShare.Read,StreamBufferSize,FileOptions.Asynchronous|FileOptions.SequentialScan);

            hc.Response.Headers["DataControlDownload"] = MakeDataControlDownload(it,f)?.ToString();

            Log.Information(GetStreamSuccessID,id); SetOk(_); ETW.Log.GetStreamSuccess(id);

            return Results.Stream(f,"application/octet-stream",id,null,null,true);
        }

        catch ( Exception _ ) { if(String.IsNullOrEmpty(id) is false && File.Exists(Path.Combine(DiskStorePath,id))) { File.Delete(Path.Combine(DiskStorePath,id)); } Log.Error(_,GetFailID,id); ETW.Log.GetError(GetFail,id); return InternalError(); }
    }
}