namespace KusDepot.Data;

internal sealed partial class Management
{
    public async Task<Boolean> BackupDataConfigs(String connection , String certificate , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.BackupDCStart(); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(new[]{connection,certificate,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); ETW.Log.BackupDCError(BadArg); return false; } Log.Information(BackupDCAttempt);

            ISecure se = ActorProxy.Create<ISecure>(new(Guid.NewGuid()),ServiceLocators.SecureService);

            if(await se.IsAdmin(token,dt,ds) is false) { DeleteSecureActor(se.GetActorId()); Log.Error(BackupDCAuthFail); SetErr(_); ETW.Log.BackupDCError(BackupDCAuthFail); return false; } DeleteSecureActor(se.GetActorId());

            HashSet<StorageSilo>? s = await ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService).GetStorageSilos(token,dt,ds);

            if(s is null || Equals(s.Count,0)) { Log.Error(BackupDCEmpty); SetErr(_); ETW.Log.BackupDCError(BackupDCEmpty); return false; } MemoryStream m = new();

            using XmlDictionaryWriter x = XmlDictionaryWriter.CreateBinaryWriter(m); DataContractSerializer c = new(typeof(HashSet<StorageSilo>));

            c.WriteObject(x,s); await x.FlushAsync(); m.Seek(0,SeekOrigin.Begin); IBlob b = ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService);

            if(await b.Store(connection,String.Format(InvariantCulture,"{0}-{1}",BackupPrefixDC,DateTimeOffset.Now.Ticks),await m.ToArray().EncryptAsync(DeserializeCertificate(certificate.ToByteArrayFromBase64())),dt,ds))
            {
                DeleteBlobActor(b.GetActorId()); Log.Information(BackupDCSuccess); SetOk(_); ETW.Log.BackupDCSuccess(); return true;
            }

            DeleteBlobActor(b.GetActorId()); Log.Error(BackupDCFailStore); SetErr(_); ETW.Log.BackupDCError(BackupDCFailStore); return false;
        }
        catch ( Exception _ ) { Log.Error(_,BackupDCFail); ETW.Log.BackupDCError(_.Message); return false; }
    }

    public async Task<Boolean> RestoreDataConfigs(String connection , String certificate , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.RestoreDCStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token)); String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(new[]{connection,certificate,token}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(__); ETW.Log.RestoreDCError(BadArg); return false; } Log.Information(RestoreDCAttempt);

            ISecure se = ActorProxy.Create<ISecure>(new(Guid.NewGuid()),ServiceLocators.SecureService);

            if(await se.IsAdmin(token,dt,ds) is false) { DeleteSecureActor(se.GetActorId()); Log.Error(RestoreDCAuthFail); SetErr(__); ETW.Log.RestoreDCError(RestoreDCAuthFail); return false; } DeleteSecureActor(se.GetActorId());

            String container = String.Empty; Int64 newest = 0;

            await foreach(BlobContainerItem _ in new BlobServiceClient(connection).GetBlobContainersAsync(prefix: BackupPrefixDC))
            {
                if(Int64.TryParse(_.Name.Split('-')[2], out Int64 tcs))
                {
                    if(tcs > newest) { newest = tcs; container = _.Name; }
                }
            }

            if(String.IsNullOrEmpty(container)) { Log.Error(RestoreDCFoundNone); SetErr(__); ETW.Log.RestoreDCError(RestoreDCFoundNone); return false; }

            DateTimeOffset newdto = new(newest,TimeSpan.Zero); Log.Information(RestoreDCFoundNewest,newdto);

            MemoryStream m = new(); await new BlobClient(connection,container,container).DownloadToAsync(m);

            m = new MemoryStream((await m.ToArray().DecryptAsync(DeserializeCertificate(certificate.ToByteArrayFromBase64())))!);

            using XmlDictionaryReader x = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

            HashSet<StorageSilo>? s = new DataContractSerializer(typeof(HashSet<StorageSilo>)).ReadObject(x) as HashSet<StorageSilo>;

            if(s is null || Equals(s.Count,0)) { Log.Error(RestoreDCFailEmpty); SetErr(__); ETW.Log.RestoreDCError(RestoreDCFailEmpty); return false; }

            if(await ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService).SetStorageSilos(s,token,dt,ds))
            {
                Log.Information(RestoreDCSuccess,newdto); SetOk(__); ETW.Log.RestoreDCSuccess(newdto.ToStringInvariant()); return true;
            }

            Log.Error(RestoreDCFailSet); SetErr(__); ETW.Log.RestoreDCError(RestoreDCFailSet); return false;
        }
        catch ( Exception _ ) { Log.Error(_,RestoreDCFail); ETW.Log.RestoreDCError(_.Message); return false; }
    }
}