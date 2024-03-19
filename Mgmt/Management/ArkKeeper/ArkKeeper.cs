namespace KusDepot.Data;

internal partial class Management
{
    public Task<Boolean> BackupArkKeeper(String connection , String certificate , String catalog , String token , String? traceid , String? spanid)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("BackupArkKeeper",traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(new[]{connection,certificate,catalog}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); } Log.Information(BackupArkAttemptCatalog,catalog);

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,dt,ds).Result) { Log.Error(BackupArkAuthFail); SetErr(_); return Task.FromResult(false); }

            IArkKeeper a = ActorProxy.Create<IArkKeeper>(new ActorId(catalog),ServiceLocators.ArkKeeperService);

            IBlob b = ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService);

            if(b.Store(connection,String.Format(InvariantCulture,"{0}-{1}-{2}",BackupPrefixArk,catalog,DateTimeOffset.Now.Ticks),a.GetArk(dt,ds).Result.Compress().Encrypt(Utility.DeserializeCertificate(certificate)),dt,ds).Result)
            {
                Log.Information(BackupArkSuccessCatalog,catalog); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(BackupArkFailCatalog,catalog); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,BackupArkFailCatalog,catalog); return Task.FromResult(false); }
    }

    public Task<Boolean> RestoreArkKeeper(String connection , String certificate , String catalog , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("RestoreArkKeeper",traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(new[]{connection,certificate,catalog,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); } Log.Information(RestoreArkAttemptCatalog,catalog);

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,dt,ds).Result) { Log.Error(RestoreArkAuthFail); SetErr(__); return Task.FromResult(false); }

            String container = String.Empty; Int64 newest = 0; String pfx = String.Format(InvariantCulture,"{0}-{1}",BackupPrefixArk,catalog);

            foreach(BlobContainerItem _ in new BlobServiceClient(connection).GetBlobContainers(prefix: pfx))
            {
                if(Int64.TryParse(_.Name.Split('-')[3], out Int64 tcs))
                {
                    if(tcs > newest) { newest = tcs; container = _.Name; }
                }
            }

            if(String.IsNullOrEmpty(container)) { Log.Error(RestoreArkFoundNone,catalog); SetErr(__); return Task.FromResult(false); }

            Log.Information(RestoreArkFoundNewest,new DateTimeOffset(newest,TimeSpan.Zero));

            MemoryStream m = new MemoryStream(); new BlobClient(connection,container,container).DownloadTo(m);

            m = new MemoryStream(m.ToArray().Decrypt(Utility.DeserializeCertificate(certificate)).Decompress()!);

            IArkKeeper a = ActorProxy.Create<IArkKeeper>(new ActorId(catalog),ServiceLocators.ArkKeeperService);

            if(Ark.TryParse(m.ToArray(),out Ark? ark))
            {
                ark.Dispose(); if(a.StoreArk(m.ToArray(),dt,ds).Result) { Log.Information(RestoreArkSuccess,catalog,new DateTimeOffset(newest,TimeSpan.Zero)); SetOk(__); return Task.FromResult(true); }
            }
            Log.Error(RestoreArkFail,catalog); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,RestoreArkFail,catalog); return Task.FromResult(false); }
    }
}