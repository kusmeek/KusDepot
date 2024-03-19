namespace KusDepot.Data;

internal partial class Management : Actor , IManagement
{
    public Task<Boolean> BackupUniverse(String connection , String certificate , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("BackupUniverse",traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(new[]{connection,certificate,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); } Log.Information(BackupUAttempt);

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,dt,ds).Result) { Log.Error(BackupUAuthFail); SetErr(_); return Task.FromResult(false); }

            HashSet<Guid>? g = ActorProxy.Create<IUniverse>(ActorIds.Universe,ServiceLocators.UniverseService).ListAll(token,dt,ds).Result;

            if(g is null || Equals(g.Count,0)) { Log.Error(BackupUEmpty); SetErr(_); return Task.FromResult(false); } MemoryStream m = new MemoryStream();

            XmlDictionaryWriter x = XmlDictionaryWriter.CreateBinaryWriter(m); DataContractSerializer c = new DataContractSerializer(typeof(HashSet<Guid>));

            c.WriteObject(x,g); x.Flush(); m.Seek(0,SeekOrigin.Begin); IBlob b = ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService);

            if(b.Store(connection,String.Format(InvariantCulture,"{0}-{1}",BackupPrefixU,DateTimeOffset.Now.Ticks),m.ToArray().Compress().Encrypt(Utility.DeserializeCertificate(certificate)),dt,ds).Result)
            {
                Log.Information(BackupUSuccess); x.Dispose(); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(BackupUFail); x.Dispose(); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,BackupUFail); return Task.FromResult(false); }
    }

    public Task<Boolean> RestoreUniverse(String connection , String certificate , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("RestoreUniverse",traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(new[]{connection,certificate,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); } Log.Information(RestoreUAttempt);

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,dt,ds).Result) { Log.Error(RestoreUAuthFail); SetErr(__); return Task.FromResult(false); }

            String container = String.Empty; Int64 newest = 0;

            foreach(BlobContainerItem _ in new BlobServiceClient(connection).GetBlobContainers(prefix: BackupPrefixU))
            {
                if(Int64.TryParse(_.Name.Split('-')[2], out Int64 tcs))
                {
                    if(tcs > newest) { newest = tcs; container = _.Name; }
                }
            }

            if(String.IsNullOrEmpty(container)) { Log.Error(RestoreUFoundNone); SetErr(__); return Task.FromResult(false); }

            Log.Information(RestoreUFoundNewest,new DateTimeOffset(newest,TimeSpan.Zero));

            MemoryStream m = new MemoryStream(); new BlobClient(connection,container,container).DownloadTo(m);

            m = new MemoryStream(m.ToArray().Decrypt(Utility.DeserializeCertificate(certificate))!.Decompress()!);

            XmlDictionaryReader x = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

            HashSet<Guid>? g = new DataContractSerializer(typeof(HashSet<Guid>)).ReadObject(x) as HashSet<Guid>; x.Dispose();

            if(g is null || Equals(g.Count,0)) { Log.Error(RestoreUFail); SetErr(__); return Task.FromResult(false); }

            if(ActorProxy.Create<IUniverse>(ActorIds.Universe,ServiceLocators.UniverseService).Reset(g,token,dt,ds).Result)
            {
                Log.Information(RestoreUSuccess,new DateTimeOffset(newest,TimeSpan.Zero)); SetOk(__); return Task.FromResult(true);
            }
            Log.Error(RestoreUFail); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,RestoreUFail); return Task.FromResult(false); }
    }
}