namespace KusDepot;

/**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.DataItem")]
[DataContract(Name = "DataItem" , Namespace = "KusDepot")]
public abstract class DataItem : MetaBase , ICloneable , IComparable<DataItem> , IDataItem , IEquatable<DataItem>
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

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataMasked"]/*'/>*/
    [DataMember(Name = "DataMasked" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    protected Boolean DataMasked = false;

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

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    protected String? Type;

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

    ///<inheritdoc/>
    public virtual Boolean ClearEncryptedData(ManagementKey? managementkey = null)
    {
        try
        {
            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            if(this.DataEncrypted is false || this.EncryptedData is null) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                ZeroMemory(this.EncryptedData);
                this.EncryptedData = null;
                this.DataEncrypted = false;
                this.DataEncryptInfo = null;
                MN();

                if(!UpdateSecureHash(DataSecurityEDKey, Array.Empty<Byte>())) { return false; }

                return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ClearEncryptedDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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

            if(c is null || a is null || Array.Empty<Byte>().SequenceEqual(a)) { return null; }

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

            if(c is null || a is null || Array.Empty<Byte>().SequenceEqual(a)) { return null; }

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

            if(c is null || a is null || Array.Empty<Byte>().SequenceEqual(a)) { return null; }

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

            if(c is null || a is null || Array.Empty<Byte>().SequenceEqual(a)) { return null; }

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

            if(c is null || a is null || Array.Empty<Byte>().SequenceEqual(a)) { return null; }

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

            if(c is null || a is null || Array.Empty<Byte>().SequenceEqual(a)) { return null; }

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
    public static TResult? FromFile<TResult>(String path)
    {
        try
        {
            if(path is null) { return default; }

            if(!File.Exists(path)) { return default; }

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return (TResult?)_1.ReadObject(_2);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return default; } throw; }
    }

    ///<inheritdoc/>
    public virtual Stream? GetContentStream()
    {
        try
        {
            if( this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE) || this.DataMasked is true ) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(File.Exists(this.FILE)) { try { return File.OpenRead(this.FILE); } catch {} }
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
            if( this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE) || this.DataMasked is true ) { return null; }

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
    public virtual Boolean GetDataMasked()
    {
        try { return this.DataMasked; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataMaskedFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
    public override String? GetFILE() { return this.DataMasked ? null : base.GetFILE(); }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try { return this.GetHashCode_NoSync(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCode_NoSync"]/*'/>*/
    protected virtual Int32 GetHashCode_NoSync()
    {
        try { return BitConverter.ToInt32(SHA512.HashData(this.ID!.Value.ToByteArray())); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetDataItemKnownTypes();

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
    public new virtual String? GetType()
    {
        if(this.Type is null) { return null; }

        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return new(this.Type); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTypeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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

    ///<inheritdoc/>
    public virtual Boolean MaskData(Boolean mask = true , ManagementKey? managementkey = null)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { if(CheckSecrets(managementkey?.Key,this.Secrets)) { this.DataMasked = mask; return Equals(this.DataMasked,mask); } return false; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,mask?MaskDataFail:UnMaskDataFail,MyTypeName,MyID); return false; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ParseAny"]/*'/>*/
    public static DataItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; }

            var kt = GetKnownTypes()?.Where(t => typeof(DataItem).IsAssignableFrom(t));

            if(kt is null) { return null; }

            var dcs = new DataContractSerializerSettings(){ MaxItemsInObjectGraph = Int32.MaxValue };

            foreach(var t in kt)
            {
                try
                {
                    using var ms = new MemoryStream(input.ToByteArrayFromBase64());

                    using XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(ms,XmlDictionaryReaderQuotas.Max);

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
            if(Array.Empty<Byte>().SequenceEqual(input)) { return null; }

            var kt = GetKnownTypes()?.Where(t => typeof(DataItem).IsAssignableFrom(t));

            if(kt is null) { return null; }

            var dcs = new DataContractSerializerSettings(){ MaxItemsInObjectGraph = Int32.MaxValue };

            foreach(var t in kt)
            {
                try
                {
                    using var ms = new MemoryStream(input);

                    using XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(ms,XmlDictionaryReaderQuotas.Max);

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
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Deserialize"]/*'/>*/
    public static TResult? Deserialize<TResult>(Byte[] input , IFormatProvider? format = null) where TResult : DataItem
    {
        try
        {
            if(Array.Empty<Byte>().SequenceEqual(input)) { return null; }

            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail); if(NoExceptions) { return null; } throw; }
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
    public virtual Boolean SetContentStreamed(Boolean streamed , ManagementKey? managementkey = null)
    {
        try { return streamed is false ? true : false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
    public virtual Boolean SetType(String? type)
    {
        try
        {
            if( type is null || this.Locked ) { return false; }

            if(DataValidation.IsDataTypeChangeAllowed(this) is false) { return false; }

            if(DataValidation.IsValid(this,type) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(String.IsNullOrEmpty(type)) { this.Type = null; MN(); return true; }

                this.Type = new(type); MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTypeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? SignData(String? field , ManagementKey? managementkey)
    {
        if(managementkey is null || String.IsNullOrEmpty(field)) { return null; }

        var i = GenerateSignatureIndex(managementkey); if(i is null) { return null; } Byte[] hash;

        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.SecureSignatures != null)
                {
                    var r = this.SecureSignatures.Keys.Where(k => k.StartsWith($"{i.Value.Item1}!{field}!",StringComparison.Ordinal)).ToList();

                    foreach (var k in r) this.SecureSignatures.Remove(k);
                }

                if(String.Equals(field,DataSecurityHCKey)) { hash = SHA512.HashData(BitConverter.GetBytes(this.GetHashCode_NoSync())); }

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
        if(this.DataMasked) { return false; }

        try
        {
            if(File.Exists(path)) { return false; } this.AcquireLocks();

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Share = FileShare.None});

            DataContractSerializer _1 = new(((Object)this).GetType(),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryWriter _2 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            _1.WriteObject(_2,this); _2.Flush(); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToFileFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { this.ReleaseLocks(); }
    }

    ///<inheritdoc/>
    public override String ToString() { return this.DataMasked ? String.Empty : this.ToStringCore(); }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ToStringCore"]/*'/>*/
    private String ToStringCore()
    {
        try
        {
            this.AcquireLocks();

            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(((Object)this).GetType(),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return String.Empty; } throw; }

        finally { this.ReleaseLocks(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Serialize"]/*'/>*/
    protected Byte[] Serialize()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(((Object)this).GetType(),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : DataItem
    {
        result = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { result = _2; return true; }

            return false;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryDeserialize"]/*'/>*/
    public static Boolean TryDeserialize<TResult>(Byte[]? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : DataItem
    {
        result = null; if(input is null || Array.Empty<Byte>().SequenceEqual(input)) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input,XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { result = _2; return true; }

            return false;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
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

            if(data is null || Array.Empty<Byte>().SequenceEqual(data))
            {
                if(this.SecureHashes is null || this.SecureHashes.ContainsKey(key) is false) { return true; }

                if(this.SecureHashes.Remove(key)) { if(Equals(this.SecureHashes.Count,0)) { this.SecureHashes = null; } return true; } return false;
            }

            this.SecureHashes ??= new(); this.SecureHashes[key] = store ? data : SHA512.HashData(data); return true;
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

            if(h is null || Array.Empty<Byte>().SequenceEqual(h))
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

            if(String.Equals(fn,DataSecurityHCKey,Ordinal)) { h = SHA512.HashData(BitConverter.GetBytes(this.GetHashCode_NoSync())); }

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

            if(s is null || Array.Empty<Byte>().SequenceEqual(s)) { return false; }

            var h = this.GetSecureHashes()?.GetValueOrDefault(key);

            if(h is null || Array.Empty<Byte>().SequenceEqual(h)) { return false; }

            return VerifyHash(managementKey,h,s);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifySecureSignatureFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> ZeroData(ManagementKey? managementkey = null , CancellationToken cancel = default) => Task.FromResult(false);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ZeroDataCore"]/*'/>*/
    internal virtual Task<Boolean> ZeroDataCore(ManagementKey? managementkey , HashSet<Object> visited , CancellationToken cancel = default)
    {
        if(visited.Contains(this)) { return Task.FromResult(true); } visited.Add(this); return Task.FromResult(false);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ZeroDataInMemoryOnlyCore"]/*'/>*/
    internal virtual Task<Boolean> ZeroDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        if(visited.Contains(this)) { return Task.FromResult(true); } visited.Add(this); return Task.FromResult(false);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="AcquireLocks"]/*'/>*/
    protected virtual void AcquireLocks()
    {
        if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

        if(!this.Sync.Data.Wait(SyncTime)) { this.Sync.Meta.Release(); throw SyncException; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ReleaseLocks"]/*'/>*/
    protected virtual void ReleaseLocks()
    {
        this.Sync.Data.Release(); this.Sync.Meta.Release();
    }
}