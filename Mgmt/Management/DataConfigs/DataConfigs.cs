namespace KusDepot.Data;

internal partial class Management : Actor , IManagement
{
    public Task<Boolean> BackupDataConfigs(String connection , String certificate , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("BackupDataConfigs",traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(new []{connection,certificate,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); } Log.Information(BackupDCAttempt);

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,dt,ds).Result) { Log.Error(BackupDCAuthFail); SetErr(_); return Task.FromResult(false); }

            HashSet<StorageSilo>? s = ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService).GetStorageSilos(token,dt,ds).Result;

            if(s is null || Equals(s.Count,0)) { Log.Error(BackupDCEmpty); SetErr(_); return Task.FromResult(false); } MemoryStream m = new MemoryStream(); 

            XmlDictionaryWriter x = XmlDictionaryWriter.CreateBinaryWriter(m); DataContractSerializer c = new DataContractSerializer(typeof(HashSet<StorageSilo>));

            c.WriteObject(x,s); x.Flush(); m.Seek(0,SeekOrigin.Begin); IBlob b = ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService);

            if(b.Store(connection,String.Format(InvariantCulture,"{0}-{1}",BackupPrefixDC,DateTimeOffset.Now.Ticks),m.ToArray().Encrypt(Utility.DeserializeCertificate(certificate)),dt,ds).Result)
            {
                Log.Information(BackupDCSuccess); x.Dispose(); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(BackupDCFail); x.Dispose(); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,BackupDCFail); return Task.FromResult(false); }
    }

    public Task<Boolean> RestoreDataConfigs(String connection , String certificate , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("RestoreDataConfigs",traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(new[]{connection,certificate,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); } Log.Information(RestoreDCAttempt);

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,dt,ds).Result) { Log.Error(RestoreDCAuthFail); SetErr(__); return Task.FromResult(false); }

            String container = String.Empty; Int64 newest = 0;

            foreach(BlobContainerItem _ in new BlobServiceClient(connection).GetBlobContainers(prefix: BackupPrefixDC))
            {
                if(Int64.TryParse(_.Name.Split('-')[2], out Int64 tcs))
                {
                    if(tcs > newest) { newest = tcs; container = _.Name; }
                }
            }

            if(String.IsNullOrEmpty(container)) { Log.Error(RestoreDCFoundNone); SetErr(__); return Task.FromResult(false); }

            Log.Information(RestoreDCFoundNewest,new DateTimeOffset(newest,TimeSpan.Zero));

            MemoryStream m = new MemoryStream(); new BlobClient(connection,container,container).DownloadTo(m);

            m = new MemoryStream(m.ToArray().Decrypt(Utility.DeserializeCertificate(certificate))!);

            XmlDictionaryReader x = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

            HashSet<StorageSilo>? s = new DataContractSerializer(typeof(HashSet<StorageSilo>)).ReadObject(x) as HashSet<StorageSilo>; x.Dispose();

            if(s is null || Equals(s.Count,0)) { Log.Error(RestoreDCFail); SetErr(__); return Task.FromResult(false); }

            if(ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService).SetStorageSilos(s,token,dt,ds).Result)
            {
                Log.Information(RestoreDCSuccess,new DateTimeOffset(newest,TimeSpan.Zero)); SetOk(__); return Task.FromResult(true);
            }
            Log.Error(RestoreDCFail); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,RestoreDCFail); return Task.FromResult(false); }
    }
}