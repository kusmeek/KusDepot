namespace KusDepot.Data;

public sealed partial class DataControl
{
    private void MapStoreStream(WebApplication application)
    {
        application.MapPost("StoreStream",
                   ([FromBody] Stream str,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   [FromHeader(Name = "DataControlUpload")] DataControlUpload? dcu,
                   HttpContext hc) => { return StoreStream(dcu,str,cdb,dc,hc); })
                   .WithName("StoreStream").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> StoreStream(DataControlUpload? dcu , Stream str , ICatalogDBFactory cdb , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            String? id = dcu?.Descriptor.ID.ToString(); ETW.Log.StoreStreamStart(id);

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",dcu?.Descriptor.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); Descriptor? od = dcu?.Descriptor; String? it = dcu?.Object;

            if(od is null || String.IsNullOrEmpty(id) || str.CanRead is false)
            {
                Log.Error(BadArg); SetErr(_); ETW.Log.StoreStreamError(BadArg,id); return Unprocessable($"{BadArg} {id}");
            }

            if(String.IsNullOrEmpty(t)) { Log.Error(StoreUnAuthID,id); SetErr(_); ETW.Log.StoreStreamError(StoreUnAuth,id); return Unauthorized(); }

            StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds); String p = Path.Combine(DiskStorePath,id); FileStream f;

            if(s is null) { Log.Error(StoreUnAuthID,id); SetErr(_); ETW.Log.StoreStreamError(StoreUnAuth,id); return Unauthorized(); } ICatalogDB c = cdb.Create(s.CatalogName);

            BlobClient bl = new(s.ConnectionString,id,id); BlobClient bls = new(s.ConnectionString,id,StreamBlobName); BlobClient blh = new(s.ConnectionString,id,"StreamSHA512");

            if( await c.Exists(od,dt,ds)              is not false ||
               (await bl.ExistsAsync()).Value         is not false ||
               (await blh.ExistsAsync()).Value        is not false ||
               (await bls.ExistsAsync()).Value        is not false  )
            {
                Log.Error(StoreFailConflictID,id); SetErr(_); ETW.Log.StoreStreamError(StoreFailConflict,id); return Conflict(id);
            }

            if(File.Exists(p))
            {
                f = new(p,FileMode.Open,FileAccess.Read,FileShare.None,StreamBufferSize,FileOptions.Asynchronous|FileOptions.SequentialScan);

                if(od.LiveStream is true || String.Equals(dcu?.StreamSHA512,SHA512.HashData(f).ToBase64FromByteArray(),Ordinal) is not true)
                {
                    await f.DisposeAsync(); File.Delete(p); f = new(p,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None,StreamBufferSize,FileOptions.Asynchronous|FileOptions.SequentialScan);

                    await str.CopyToAsync(f,hc.RequestAborted); f.Seek(0,SeekOrigin.Begin);
                }
            }
            else
            {
                f = new(p,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None,StreamBufferSize,FileOptions.Asynchronous|FileOptions.SequentialScan);

                await str.CopyToAsync(f,hc.RequestAborted); f.Seek(0,SeekOrigin.Begin);
            }

            if(od.LiveStream is not true)
            {
                if(dcu.Verify() is false || String.Equals(dcu?.StreamSHA512,(await SHA512.HashDataAsync(f,hc.RequestAborted)).ToBase64FromByteArray(),Ordinal) is false)
                {
                    await f.DisposeAsync(); File.Delete(p); Log.Error(VerificationFailID,id); SetErr(_); ETW.Log.StoreStreamError(VerificationFail,id); return Unprocessable($"{VerificationFail} {id}");
                }
            }
            else
            {
                if(dcu.Verify() is false)
                {
                    await f.DisposeAsync(); File.Delete(p); Log.Error(VerificationFailID,id); SetErr(_); ETW.Log.StoreStreamError(VerificationFail,id); return Unprocessable($"{VerificationFail} {id}");
                }
            }

            BlobContainerClient bc = new(s.ConnectionString,id); await bc.CreateIfNotExistsAsync(); f.Seek(0,SeekOrigin.Begin);

            BlobUploadOptions uo = new(){TransferValidation = new(){ChecksumAlgorithm = StorageChecksumAlgorithm.StorageCrc64}};

            await bl.UploadAsync(BinaryData.FromBytes(dcu!.Object.ToByteArrayFromBase64()),uo,hc.RequestAborted);

            await blh.UploadAsync(BinaryData.FromBytes(dcu!.StreamSHA512.ToByteArrayFromBase64()),uo,hc.RequestAborted);

            await bls.UploadAsync(f,uo,hc.RequestAborted);

            if(await c.AddUpdate(od,dt,ds) is false) { Log.Error(StoreCatalogFailID,id); ETW.Log.StoreStreamIssue(StoreCatalogFail,id); }

            await f.DisposeAsync(); Log.Information(StoreSuccessID,id); SetOk(_); ETW.Log.StoreStreamSuccess(id); return Results.Ok(id);
        }
        catch ( Exception _ ) { Log.Error(_,StoreFailDescriptor,dcu?.Descriptor); ETW.Log.StoreStreamError(_.Message,dcu?.Descriptor.ID?.ToString()); return InternalError(); }
    }
}