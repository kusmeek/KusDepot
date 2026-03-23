namespace KusDepot;

/**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.DataItem")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[DataContract(Name = "DataItem" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonDerivedType(typeof(KeySet),typeDiscriminator:nameof(KeySet))]
[JsonDerivedType(typeof(TextItem),typeDiscriminator:nameof(TextItem))]
[JsonDerivedType(typeof(CodeItem),typeDiscriminator:nameof(CodeItem))]
[JsonDerivedType(typeof(BinaryItem),typeDiscriminator:nameof(BinaryItem))]
[JsonDerivedType(typeof(DataSetItem),typeDiscriminator:nameof(DataSetItem))]
[JsonDerivedType(typeof(GenericItem),typeDiscriminator:nameof(GenericItem))]
[JsonDerivedType(typeof(MSBuildItem),typeDiscriminator:nameof(MSBuildItem))]
[JsonDerivedType(typeof(MultiMediaItem),typeDiscriminator:nameof(MultiMediaItem))]
[JsonDerivedType(typeof(GuidReferenceItem),typeDiscriminator:nameof(GuidReferenceItem))]

public abstract partial class DataItem : MetaBase , ICloneable , IComparable<DataItem> , IDataItem , IEquatable<DataItem>
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="ContentStreamed"]/*'/>*/
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    protected Boolean ContentStreamed;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataEncrypted"]/*'/>*/
    [DataMember(Name = "DataEncrypted" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    protected Boolean DataEncrypted = false;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataEncryptInfo"]/*'/>*/
    [DataMember(Name = "DataEncryptInfo" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    protected DataEncryptInfo? DataEncryptInfo;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataType"]/*'/>*/
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    protected String? DataType;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/property[@name="MyDataType"]/*'/>*/
    [JsonPropertyName("DataType")] [Description(DataTypeDescription)] [JsonRequired] [IgnoreDataMember]
    public String? MyDataType
    {
        get { try { return this.GetDataType(); } catch { return null; } }

        set { try { this.SetDataType(value); } catch {} }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="EncryptedData"]/*'/>*/
    [DataMember(Name = "EncryptedData" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    protected Byte[]? EncryptedData;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="SecureHashes"]/*'/>*/
    [DataMember(Name = "SecureHashes" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    protected Dictionary<String,Byte[]>? SecureHashes;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="SecureSignatures"]/*'/>*/
    [DataMember(Name = "SecureSignatures" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    protected Dictionary<String,Byte[]>? SecureSignatures;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="Sync"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    protected new DataSync Sync;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    protected DataItem() { this.Sync = (this.GetSyncNode() as DataSync)!; }

    ///<inheritdoc/>
    public virtual Task<Boolean> CheckDataHash(CancellationToken cancel = default) => CheckDataHashCore(null,null,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CheckDataHashCore"]/*'/>*/
    protected async Task<Boolean> CheckDataHashCore(Byte[]? data , Stream? stream , CancellationToken cancel = default)
    {
        try
        {
            String k = DataEncrypted ? DataSecurityEDKey : DataSecurityCNKey; Boolean ok = false;

            if(ContentStreamed is false && data is not null) { ok = await VerifySecureHash(k,data,cancel).ConfigureAwait(false); }

            if(ContentStreamed && stream is not null) { ok = await VerifySecureHash(k,stream,cancel).ConfigureAwait(false); }

            if(ok is false) { return false; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Clone"]/*'/>*/
    public virtual DataItem? Clone() { return Parse(this.ToString()); }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CloneProtected"]/*'/>*/
    protected DataItem? CloneProtected() { return Deserialize(this.Serialize()); }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CloneOpen"]/*'/>*/
    public TResult? Clone<TResult>() where TResult : DataItem
    {
        try { return Parse<TResult>(this.ToString(),null); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CloneFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    Object ICloneable.Clone() { return this.Clone()!; }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is DataItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is DataItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(IDataItem? other) { return this.CompareTo(other as DataItem); }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(DataItem? other)
    {
        try
        {
            if(ReferenceEquals(this,other))              { return 0; }

            DateTimeOffset? _ = other?.GetBornOn();

            if(this.BornOn is null     && _ is null)     { return 0; }
            if(this.BornOn is not null && _ is null)     { return 1; }
            if(this.BornOn is null     && _ is not null) { return -1; }

            return this.BornOn!.Value.CompareTo(_!.Value);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareToFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override ManagementKey? CreateManagementKey(String? subject)
    {
        if(this.Locked) { return null; }

        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { var _ = new ManagerKey(CreateCertificate(this,subject)!); if(this.RegisterManager_NoSync(_)) { return _; } return null; }

            finally { this.Sync.Data.Release(); }
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateManagementKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override ManagementKey? CreateManagementKey(X509Certificate2? certificate)
    {
        if(this.Locked) { return null; }

        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { var _ = new ManagerKey(certificate!); if(this.RegisterManager_NoSync(_)) { return _; } return null; }

            finally { this.Sync.Data.Release(); }
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateManagementKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> DecryptData(ManagementKey? managementkey , CancellationToken cancel = default) => Task.FromResult(false);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptDataBuffer"]/*'/>*/
    protected Byte[]? DecryptDataBuffer(Byte[]? data , ManagementKey? managementkey)
    {
        try
        {
            if(managementkey is null || data is null || Equals(data.Length,0)) { return null; }

            var c = DeserializeCertificate(managementkey.Key); var a = GetID()?.ToByteArray();

            if(c is null || a is null || Array.Empty<Byte>().AsSpan().SequenceEqual(a.AsSpan())) { return null; }

            return data.Decrypt(c,a);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptDataBufferAsync"]/*'/>*/
    protected async Task<Byte[]?> DecryptDataBuffer(Byte[]? data , ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || data is null || Equals(data.Length,0)) { return null; }

            var c = DeserializeCertificate(managementkey.Key); var a = GetID()?.ToByteArray();

            if(c is null || a is null || Array.Empty<Byte>().AsSpan().SequenceEqual(a.AsSpan())) { return null; }

            using var i = new MemoryStream(data); using var o = new MemoryStream();

            if(await DecryptAsync(i,o,c,a,cancel:cancel).ConfigureAwait(false) is false) { return null; }

            return o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptDataCore"]/*'/>*/
    protected async Task<Stream?> DecryptDataCore(Byte[]? data = null , String? file = null , ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || (data is null && String.IsNullOrEmpty(file))) { return null; }

            var c = DeserializeCertificate(managementkey.Key); var a = GetID()?.ToByteArray();

            if(c is null || a is null || Array.Empty<Byte>().AsSpan().SequenceEqual(a.AsSpan())) { return null; }

            if(ContentStreamed && File.Exists(file))
            {
                String t = Path.GetTempFileName(); String b = Path.GetTempFileName();

                try
                {
                    using (var i = File.OpenRead(file)) using (var o = File.OpenWrite(t))
                    {
                        if(DataEncrypted is false) { return null; }

                        if(await DecryptAsync(i,o,c,a,cancel:cancel).ConfigureAwait(false) is false) { return null; }
                    }

                    File.Replace(t,file,b); return File.OpenRead(file);
                }
                catch { if(File.Exists(t)) File.Delete(t); throw; } finally { if(File.Exists(b)) File.Delete(b); }
            }
            else if(data is not null)
            {
                using (var i = new MemoryStream(data))
                {
                    var o = new MemoryStream();

                    if(DataEncrypted is false) { return null; }

                    if(await DecryptAsync(i,o,c,a,cancel:cancel).ConfigureAwait(false) is false) { return null; }

                    o.Seek(0,SeekOrigin.Begin); return new MemoryStream(o.GetBuffer(),0,(Int32)o.Length,false);
                }
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataCoreFail,MyTypeName,MyID); return null; }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> EncryptData(ManagementKey? managementkey = null , Boolean sign = true , CancellationToken cancel = default)  => Task.FromResult(false);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBuffer"]/*'/>*/
    protected Byte[]? EncryptDataBuffer(Byte[]? data , ManagementKey? managementkey)
    {
        try
        {
            if(managementkey is null || data is null || Equals(data.Length,0)) { return null; }

            var c = DeserializeCertificate(managementkey.Key); var a = GetID()?.ToByteArray();

            if(c is null || a is null || Array.Empty<Byte>().AsSpan().SequenceEqual(a.AsSpan())) { return null; }

            return data.Encrypt(c,a);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBufferAsync"]/*'/>*/
    protected async Task<Byte[]?> EncryptDataBuffer(Byte[]? data , ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || data is null || Equals(data.Length,0)) { return null; }

            var c = DeserializeCertificate(managementkey.Key); var a = GetID()?.ToByteArray();

            if(c is null || a is null || Array.Empty<Byte>().AsSpan().SequenceEqual(a.AsSpan())) { return null; }

            using var i = new MemoryStream(data); using var o = new MemoryStream();

            if(await EncryptAsync(i,o,c,a,cancel:cancel).ConfigureAwait(false) is false) { return null; }

            return o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataCore"]/*'/>*/
    protected async Task<Stream?> EncryptDataCore(Byte[]? data = null , String? file = null , ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || (data is null && String.IsNullOrEmpty(file))) { return null; }

            var c = DeserializeCertificate(managementkey.Key); var a = GetID()?.ToByteArray();

            if(c is null || a is null || Array.Empty<Byte>().AsSpan().SequenceEqual(a.AsSpan())) { return null; }

            if(ContentStreamed && File.Exists(file))
            {
                String t = Path.GetTempFileName(); String b = Path.GetTempFileName();

                try
                {
                    using (var i = File.OpenRead(file)) using (var o = File.OpenWrite(t))
                    {
                        if(DataEncrypted is true) { return null; }

                        if(await EncryptAsync(i,o,c,a,includeoriginallength:i.CanSeek,cancel:cancel).ConfigureAwait(false) is false) { return null; }
                    }

                    File.Replace(t,file,b); return File.OpenRead(file);
                }
                catch { if(File.Exists(t)) File.Delete(t); throw; } finally { if(File.Exists(b)) File.Delete(b); }
            }
            else if(data is not null)
            {
                using (var i = new MemoryStream(data))
                {
                    var o = new MemoryStream();

                    if(DataEncrypted is true) { return null; }

                    if(await EncryptAsync(i,o,c,a,includeoriginallength:true,cancel:cancel).ConfigureAwait(false) is false) { return null; }

                    o.Seek(0,SeekOrigin.Begin); return new MemoryStream(o.GetBuffer(),0,(Int32)o.Length,false);
                }
            }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataCoreFail,MyTypeName,MyID); return null; }
    }

    ///<inheritdoc/>
    public virtual Boolean Equals(IDataItem? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public virtual Boolean Equals(DataItem? other) { return ReferenceEquals(this,other); }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="FromFile"]/*'/>*/
    public static TResult? FromFile<TResult>(String path) where TResult : DataItem
    {
        try
        {
            if(path is null) { return default; }

            if(!File.Exists(path)) { return default; }

            return DataContractUtility.FromFile<TResult>(path,SerializationData.ForType(typeof(TResult)));
        }
        catch ( Exception _ )
        {
            KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return default; } throw;
        }
    }

    ///<inheritdoc/>
    public virtual Stream? GetContentStream()
    {
        try
        {
            if( this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE) ) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(File.Exists(this.FILE))
                {
                    try
                    {
                        return new FileStream(this.FILE,new FileStreamOptions{
                            Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.SequentialScan });
                    }
                    catch {}
                }
            }
            finally { this.Sync.Data.Release(); }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetContentStream_NoSync"]/*'/>*/
    protected virtual Stream? GetContentStream_NoSync()
    {
        try
        {
            if( this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE) ) { return null; }

            if(File.Exists(this.FILE)) { try { return File.OpenRead(this.FILE); } catch {} }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean GetContentStreamed()
    {
        try { return this.ContentStreamed; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamedFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<DataContent?> GetDataContent(ManagementKey? managementkey = null , CancellationToken cancel = default) => Task.FromResult<DataContent?>(null);

    ///<inheritdoc/>
    public virtual Boolean GetDataEncrypted()
    {
        try { return this.DataEncrypted; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataEncryptedFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual DataEncryptInfo? GetDataEncryptInfo()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.DataEncryptInfo?.Clone(); }

            finally { this.Sync.Data.Release(); }
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataEncryptInfoFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetDataType()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.DataType is null ? null : new(this.DataType); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataTypeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetEncryptedData"]/*'/>*/
    protected virtual Byte[]? GetEncryptedData()
    {
        try
        {
            if(this.EncryptedData is null) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                return this.EncryptedData.Clone() as Byte[];
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetEncryptedDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override String? GetFILE() { return base.GetFILE(); }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try { return this.GetHashCode_NoSync(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCodeAsync"]/*'/>*/
    public virtual async Task<Int32> GetHashCodeAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

        try { return await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false); }

        finally { this.Sync.Data.Release(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCode_NoSync"]/*'/>*/
    protected virtual Int32 GetHashCode_NoSync()
    {
        try { return BitConverter.ToInt32(SHA512.HashData(this.ID!.Value.ToByteArray())); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCode_NoSyncAsync"]/*'/>*/
    protected virtual async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested(); using var m = new MemoryStream(this.ID!.Value.ToByteArray());

        try { return BitConverter.ToInt32(await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false),0); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetDataItemKnownTypes();

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetIntegrityHash_NoSync"]/*'/>*/
    protected internal virtual Byte[] GetIntegrityHash_NoSync()
    {
        try { return SHA512.HashData(this.ID!.Value.ToByteArray()); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetIntegrityHash_NoSyncAsync"]/*'/>*/
    protected internal virtual async Task<Byte[]> GetIntegrityHash_NoSyncAsync(CancellationToken cancel = default)
    {
        using var m = new MemoryStream(this.ID!.Value.ToByteArray());

        try { return await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetSecureHashes"]/*'/>*/
    protected virtual Dictionary<String,Byte[]>? GetSecureHashes()
    {
        try
        {
            if(this.SecureHashes is null) { return null; }

            return this.SecureHashes.ToDictionary(_=>new String(_.Key),_=>(Byte[])_.Value.Clone());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetSecureHashesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetSecureSignatures"]/*'/>*/
    protected virtual Dictionary<String,Byte[]>? GetSecureSignatures()
    {
        try
        {
            if(this.SecureSignatures is null) { return null; }

            return this.SecureSignatures.ToDictionary(_=>new String(_.Key),_=>(Byte[])_.Value.Clone());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetSecureSignaturesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override SyncNode GetSyncNode()
    {
        try { this.Sync ??= new(); return this.Sync; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetSyncNodeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean Initialize()
    {
        if(this.Locked) { return false; } this.GetSyncNode(); base.Initialize();

        try
        {
            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                this.BornOn = this.BornOn ?? DateTimeOffset.Now; this.Modified = this.Modified ?? this.BornOn;

                if(new Object?[]{this.BornOn,this.Modified}.Any(_=>_ is null)) { throw new InitializationException(); }

                return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InitializeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean Lock(ManagementKey? managementkey)
    {
        if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

        try { return base.Lock(managementkey); }

        finally { this.Sync.Data.Release(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ParseAny"]/*'/>*/
    public static DataItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; }

            var kt = GetKnownTypes()?.Where(t => typeof(DataItem).IsAssignableFrom(t));

            var dcs = new DataContractSerializerSettings(){ MaxItemsInObjectGraph = Int32.MaxValue };

            foreach(var t in kt!)
            {
                try
                {
                    using var m = new MemoryStream(input.ToByteArrayFromBase64());

                    using XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

                    var s = new DataContractSerializer(t,dcs); if(s.ReadObject(r) is DataItem d) { return d; }
                }
                catch {}
            }

            return null;
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DeserializeAny"]/*'/>*/
    public static DataItem? Deserialize(Byte[] input , IFormatProvider? format = null)
    {
        try
        {
            if(input is null || Array.Empty<Byte>().AsSpan().SequenceEqual(input.AsSpan())) { return null; }

            var kt = GetKnownTypes()?.Where(t => typeof(DataItem).IsAssignableFrom(t));

            var dcs = new DataContractSerializerSettings(){ MaxItemsInObjectGraph = Int32.MaxValue };

            foreach(var t in kt!)
            {
                try
                {
                    using var m = new MemoryStream(input);

                    using XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

                    var s = new DataContractSerializer(t,dcs); if(s.ReadObject(r) is DataItem d) { return d; }
                }
                catch {}
            }

            return null;
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,DeserializeFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : DataItem
    {
        return DataContractUtility.ParseBase64<TResult>(input,SerializationData.ForType(typeof(TResult)));
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Deserialize"]/*'/>*/
    public static TResult? Deserialize<TResult>(Byte[] input , IFormatProvider? format = null) where TResult : DataItem
    {
        return DataContractUtility.Deserialize<TResult>(input,SerializationData.ForType(typeof(TResult)));
    }

    ///<inheritdoc/>
    protected override void RefreshSyncNodes()
    {
        try { this.Sync = (this.GetSyncNode() as DataSync)!; base.RefreshSyncNodes(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,RefreshSyncNodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override Boolean RegisterManager(ManagementKey? managementkey)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return base.RegisterManager(managementkey); }

            finally { this.Sync.Data.Release(); }

        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="RegisterManager_NoSync"]/*'/>*/
    protected Boolean RegisterManager_NoSync(ManagementKey? managementkey)
    {
        try
        {
            return base.RegisterManager(managementkey);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override ManagementKey? RegisterManager(X509Certificate2? certificate)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return base.RegisterManager(certificate); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="RegisterManagerCertificate_NoSync"]/*'/>*/
    protected ManagementKey? RegisterManager_NoSync(X509Certificate2? certificate)
    {
        try { return base.RegisterManager(certificate); }

        catch ( Exception _ ) { KusDepotLog.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> SetContentStreamed(Boolean streamed , ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try { return Task.FromResult(streamed is false ? true : false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return Task.FromResult(false); } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetDataType(String? type)
    {
        try
        {
            if( type is null || this.Locked ) { return false; }

            if(DataValidation.IsDataTypeChangeAllowed(this) is false) { return false; }

            if(DataValidation.IsValid(this,type) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(String.IsNullOrEmpty(type)) { this.DataType = null; MN(); return true; }

                this.DataType = new(type); MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetDataTypeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean SetID(Guid? id)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return base.SetID(id); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<String?> SignData(String? field , ManagementKey? managementkey , CancellationToken cancel = default)
    {
        if(managementkey is null || String.IsNullOrEmpty(field)) { return null; }

        var i = GenerateSignatureIndex(managementkey); if(i is null) { return null; } Byte[] hash;

        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.SecureSignatures != null)
                {
                    var r = this.SecureSignatures.Keys.Where(k => k.StartsWith($"{i.Value.Item1}!{field}!",Ordinal)).ToList();

                    foreach (var k in r) this.SecureSignatures.Remove(k);
                }

                if(String.Equals(field,DataSecurityHCKey))
                {
                    using var m = new MemoryStream(BitConverter.GetBytes(await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false)));

                    hash = await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false);
                }

                else
                {
                    var h = GetSecureHashes(); if(h is null || !h.TryGetValue(field,out var hv) || hv is null) { return null; } hash = hv;
                }

                var s = SignHash(managementkey,hash); if(s is null) { return null; }

                this.SecureSignatures ??= new(); this.SecureSignatures[i.Value.Item2!] = s;

                return i.Value.Item2;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SignDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        (String?,String?)? GenerateSignatureIndex(ManagementKey? managementkey)
        {
            var c = DeserializeCertificate(managementkey!.Key); if(c is null ||String.IsNullOrEmpty(c.SerialNumber)) { return null; }

            return (c.SerialNumber,$"{c.SerialNumber}!{field}!{DateTimeOffset.Now:O}" );
        }
    }

    ///<inheritdoc/>
    public virtual Boolean ToFile(String path)
    {
        try
        {
            if(File.Exists(path)) { return false; } this.AcquireLocks();

            return DataContractUtility.ToFile(path,this,SerializationData.ForCommon(this));
        }
        finally { this.ReleaseLocks(); }
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        try
        {
            this.AcquireLocks();

            return DataContractUtility.ToBase64String(this,SerializationData.ForCommon(this));
        }
        finally { this.ReleaseLocks(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Serialize"]/*'/>*/
    protected Byte[] Serialize() { return DataContractUtility.Serialize(this,SerializationData.ForCommon(this)); }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : DataItem
    {
        result = null; if(input is null) { return false; }

        try
        {
            return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(TResult)),out result);
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryDeserialize"]/*'/>*/
    public static Boolean TryDeserialize<TResult>(Byte[]? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : DataItem
    {
        result = null; if(input is null || Array.Empty<Byte>().AsSpan().SequenceEqual(input.AsSpan())) { return false; }

        try
        {
            return DataContractUtility.TryParseBase64(input.ToBase64FromByteArray(),SerializationData.ForType(typeof(TResult)),out result);
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ )
        {
            if(NoExceptions) { return false; } KusDepotLog.Error(_, TryParseFail); throw;
        }
    }

    ///<inheritdoc/>
    public override Boolean UnLock(ManagementKey? managementkey)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return base.UnLock(managementkey); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnLockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean UnRegisterManager(ManagementKey? managementkey)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return base.UnRegisterManager(managementkey); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnRegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateSecureHash"]/*'/>*/
    protected virtual Boolean UpdateSecureHash(String? key , Byte[]? data , Boolean store = false)
    {
        try
        {
            if(String.IsNullOrEmpty(key)) { return false; }

            if(data is null || Array.Empty<Byte>().AsSpan().SequenceEqual(data.AsSpan()))
            {
                if(this.SecureHashes is null || this.SecureHashes.ContainsKey(key) is false) { return true; }

                if(this.SecureHashes.Remove(key)) { if(Equals(this.SecureHashes.Count,0)) { this.SecureHashes = null; } return true; } return false;
            }

            this.SecureHashes ??= new(); this.SecureHashes[key] = store ? data : SHA512.HashData(data); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateSecureHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateSecureHashAsync"]/*'/>*/
    protected virtual async Task<Boolean> UpdateSecureHashAsync(String? key , Byte[]? data , Boolean store = false , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(key)) { return false; }

            if(data is null || Array.Empty<Byte>().AsSpan().SequenceEqual(data.AsSpan()))
            {
                if(this.SecureHashes is null || this.SecureHashes.ContainsKey(key) is false) { return true; }

                if(this.SecureHashes.Remove(key)) { if(Equals(this.SecureHashes.Count,0)) { this.SecureHashes = null; } return true; } return false;
            }

            this.SecureHashes ??= new();

            if(store) { this.SecureHashes[key] = data; return true; } using var m = new MemoryStream(data,false);

            this.SecureHashes[key] = await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateSecureHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateSecureHashAsyncStream"]/*'/>*/
    protected virtual async Task<Boolean> UpdateSecureHashAsync(String? key , Stream? data , CancellationToken cancel = default)
    {
        try
        {
            if(data is null || !data.CanRead || String.IsNullOrEmpty(key)) { return false; }

            var hash = await SHA512.HashDataAsync(data,cancel).ConfigureAwait(false); if(hash is null) { return false; } data.Seek(0,SeekOrigin.Begin);

            if(Array.Empty<Byte>().AsSpan().SequenceEqual(hash.AsSpan()))
            {
                if(this.SecureHashes is null || this.SecureHashes.ContainsKey(key) is false) { return true; }

                if(this.SecureHashes.Remove(key)) { if(Equals(this.SecureHashes.Count,0)) { this.SecureHashes = null; } return true; } return false;
            }

            this.SecureHashes ??= new(); this.SecureHashes[key] = hash;

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateSecureHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateSecureSignature"]/*'/>*/
    protected virtual Boolean UpdateSecureSignature(String? key , ManagementKey? managementkey)
    {
        if(managementkey is null || String.IsNullOrEmpty(key)) { return false; }

        try
        {
            var h = this.GetSecureHashes()?.GetValueOrDefault(key);

            if(h is null || Array.Empty<Byte>().AsSpan().SequenceEqual(h.AsSpan()))
            {
                if(this.SecureSignatures is null || this.SecureSignatures.ContainsKey(key) is false) { return true; }

                if(this.SecureSignatures.Remove(key)) { if(Equals(this.SecureSignatures.Count,0)) { this.SecureSignatures = null; } return true; } return false;
            }

            var s = SignHash(managementkey,h); if(s is null) { return false; }

            this.SecureSignatures ??= new(); this.SecureSignatures[key] = s; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateSecureSignatureFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw;  }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> VerifyData(String? field , ManagementKey? managementkey , CancellationToken cancel = default) => VerifyDataCore(field,managementkey,null,null,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifyDataCore"]/*'/>*/
    protected virtual async Task<Boolean> VerifyDataCore(String? field , ManagementKey? managementkey , Byte[]? currentfielddata , Stream? currentfieldstream = null , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || String.IsNullOrEmpty(field)) { return false; } Boolean ok = false; Byte[]? h = null;

            if(!String.Equals(field,DataSecurityHCKey,Ordinal) && (currentfielddata is null || Equals(currentfielddata.Length,0)) && (currentfieldstream is null || !currentfieldstream.CanRead)) { return false; }

            var sgs = GetSecureSignatures(); var hsh = GetSecureHashes(); if(sgs is null || hsh is null) { return false; }

            var p = GenerateSignaturePrefix(managementkey); if(p is null) { return false; }

            String? mrk = null; DateTimeOffset mrt = DateTimeOffset.MinValue;

            foreach(var k in sgs!.Keys)
            {
                if(k.StartsWith(p,Ordinal))
                {
                    if(DateTimeOffset.TryParse(k.Split('!')[2],out var ts))
                    {
                        if(ts > mrt) { mrt = ts; mrk = k; }
                    }
                }
            }

            if(mrk is null && (String.Equals(field,DataSecurityCNKey,Ordinal) || String.Equals(field,DataSecurityEDKey,Ordinal)))
            {
                if(sgs.ContainsKey(field)) { mrk = field; }
            }

            if(mrk is null) { return false; } var fn = mrk.Contains('!') ? mrk.Split('!')[1] : mrk;

            if(String.Equals(fn,DataSecurityHCKey,Ordinal))
            {
                using var m = new MemoryStream(BitConverter.GetBytes(await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false)));

                h = await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false);
            }

            if(h is null) { hsh!.TryGetValue(fn,out var sh); if(sh is null) { return false; } h = sh; }

            if(currentfieldstream is not null && currentfieldstream.CanRead) { ok = await VerifySecureHash(fn,currentfieldstream,cancel).ConfigureAwait(false); }

            else if(currentfielddata is not null && currentfielddata.Length > 0) { ok = await VerifySecureHash(fn,currentfielddata,cancel).ConfigureAwait(false); }

            else if(String.Equals(field,DataSecurityHCKey,Ordinal)) { ok = true; }

            return ok is false ? false : VerifyHash(managementkey,h,sgs[mrk]);

        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyDataCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        String? GenerateSignaturePrefix(ManagementKey? managementkey)
        {
            var c = DeserializeCertificate(managementkey!.Key); if(c is null ||String.IsNullOrEmpty(c.SerialNumber)) { return null; }

            return $"{c.SerialNumber}!{field}!";
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifySecureHash"]/*'/>*/
    protected virtual async Task<Boolean> VerifySecureHash(String? key , Byte[]? data , CancellationToken cancel = default)
    {
        try
        {
            if( String.IsNullOrEmpty(key) || data is null || Equals(data.Length,0) ) { return false; }

            using var m = new MemoryStream(data); var c = await SHA512.HashDataAsync(m,cancel).ConfigureAwait(false);

            if(this.SecureHashes is null || this.SecureHashes.ContainsKey(key) is false) { return false; }

            return this.SecureHashes[key].AsSpan().SequenceEqual(c);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifySecureHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifySecureHashStream"]/*'/>*/
    protected virtual async Task<Boolean> VerifySecureHash(String? key , Stream? data , CancellationToken cancel = default)
    {
        try
        {
            if( String.IsNullOrEmpty(key) || data is null || data.CanRead is false ) { return false; }

            var c = await SHA512.HashDataAsync(data,cancel).ConfigureAwait(false); data.Seek(0,SeekOrigin.Begin);

            if(this.SecureHashes is null || this.SecureHashes.ContainsKey(key) is false) { return false; }

            return this.SecureHashes[key].AsSpan().SequenceEqual(c);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifySecureHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifySecureSignature"]/*'/>*/
    protected virtual Boolean VerifySecureSignature(String key , ManagementKey? managementKey)
    {
        if(String.IsNullOrEmpty(key) || managementKey is null) { return false; }

        try
        {
            var s = this.GetSecureSignatures()?.GetValueOrDefault(key);

            if(s is null || Array.Empty<Byte>().AsSpan().SequenceEqual(s.AsSpan())) { return false; }

            var h = this.GetSecureHashes()?.GetValueOrDefault(key);

            if(h is null || Array.Empty<Byte>().AsSpan().SequenceEqual(h.AsSpan())) { return false; }

            return VerifyHash(managementKey,h,s);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifySecureSignatureFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> WipeData(ManagementKey? managementkey = null , CancellationToken cancel = default) => Task.FromResult(false);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WipeDataCore"]/*'/>*/
    protected internal virtual Task<Boolean> WipeDataCore(ManagementKey? managementkey , HashSet<Object> visited , CancellationToken cancel = default)
    {
        if(visited.Contains(this)) { return Task.FromResult(true); } visited.Add(this); return Task.FromResult(false);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WipeDataInMemoryOnly"]/*'/>*/
    protected internal virtual async Task<Boolean> WipeDataInMemoryOnly(ManagementKey? managementkey , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            return await this.WipeDataInMemoryOnlyCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),requirelocks,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WipeDataInMemoryOnlyCore"]/*'/>*/
    protected internal virtual Task<Boolean> WipeDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        if(visited.Contains(this)) { return Task.FromResult(true); } visited.Add(this); return Task.FromResult(false);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="AcquireLocks"]/*'/>*/
    protected virtual void AcquireLocks()
    {
        if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

        if(!this.Sync.Data.Wait(SyncTime)) { this.Sync.Meta.Release(); throw SyncException; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="AcquireLocksAsync"]/*'/>*/
    protected virtual async Task AcquireLocksAsync(CancellationToken cancel = default)
    {
        if(!await this.Sync.Meta.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

        if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false))
        {
            this.Sync.Meta.Release(); throw SyncException;
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ReleaseLocks"]/*'/>*/
    protected virtual void ReleaseLocks()
    {
        this.Sync.Data.Release(); this.Sync.Meta.Release();
    }
}