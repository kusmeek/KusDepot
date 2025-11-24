namespace KusDepot;

/**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KeySet")]
[DataContract(Name = "KeySet" , Namespace = "KusDepot")]
public sealed class KeySet : DataItem , IComparable<KeySet> , IEquatable<KeySet> , IParsable<KeySet>
{
    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/content[@name="AllKeys"]/*'/>*/
    [DataMember(Name = "AllKeys" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private Dictionary<String,SecurityKey>? AllKeys;

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/field[@name="OwnerSecret"]/*'/>*/
    [DataMember(Name = "OwnerSecret" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    private Byte[]? OwnerSecret;

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public KeySet() : this(null) {}

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/constructor[@name="Constructor"]/*'/>*/
    public KeySet(IEnumerable<SecurityKey>? keys = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.Type = DataType.KEYSET;

            if(keys is not null)
            {
                this.AllKeys = new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                foreach(var k in keys)
                {
                    var c = k.Clone(); if(c is null) { continue; } this.AllKeys[c.ID.Value.ToString()] = c;
                }

                if(Equals(this.AllKeys.Count,0)) { this.AllKeys = null; }
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="AddKey"]/*'/>*/
    public Boolean AddKey(SecurityKey? key , String? name = null , ManagementKey? managementkey = null , Boolean sign = true)
    {
        try
        {
            if(key is null || key.ID is null)                               { return false; }

            if(this.Locked && this.CheckManager(managementkey) is false)    { return false; }

            var h = String.IsNullOrWhiteSpace(name) ? key.ID.Value.ToString()! : name!;

            if(Guid.TryParse(h,out var id) && Equals(id,key.ID) is false)   { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || managementkey is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,managementkey); if(ds is null)                                             { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                                                                     { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(set.AllKeys.ContainsKey(h)) { return false; }

                    var kc = set.AllKeys.Values.Where(v => Equals(v.ID,key.ID)).ToList(); if(kc.Any(v => v.Equals(key) is false))        { return false; }

                    set.AllKeys[h] = key.Clone()!;

                    var es = EncryptDataBuffer(set.Serialize(),managementkey); if(es is null)                                            { return false; }

                    this.EncryptedData = es;

                    if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                                                          { return false; }
                    if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey))                                               { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                                                         { return false; }

                    this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                    return true;
                }
                else
                {
                    this.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                    if(this.AllKeys.ContainsKey(h))                                                                     { return false; }

                    var kc = this.AllKeys.Values.Where(v => Equals(v.ID,key.ID)).ToList();

                    if(kc.Any(v => v.Equals(key) is false))                                                             { return false; }

                    this.AllKeys[h] = key.Clone()!; MN();

                    if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))           { return false; }
                    if(sign && managementkey is not null) { if(!UpdateSecureSignature(DataSecurityCNKey,managementkey)) { return false; } }

                    return true;
                }
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="AddKeys"]/*'/>*/
    public Boolean AddKeys(IEnumerable<SecurityKey>? keys , ManagementKey? managementkey = null , Boolean sign = true)
    {
        try
        {
            if(keys is null || this.Locked && (this.CheckManager(managementkey) is false)) { return false; }

            var nk = keys as IList<SecurityKey> ?? keys.ToList(); if(Equals(nk.Count,0))   { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || managementkey is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,managementkey); if(ds is null) { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                         { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                    Boolean mod = false; Boolean all = true;

                    foreach(var k in nk)
                    {
                        var kc = set.AllKeys.Values.Where(v => Equals(v.ID,k.ID)).ToList();

                        if(kc.Any(v => v.Equals(k) is false)) { all = false; continue; }

                        var n = k.ID.Value.ToString()!;

                        if(set.AllKeys.ContainsKey(n)) { all = false; continue; }

                        set.AllKeys[n!] = k.Clone()!; mod = true;
                    }

                    if(mod)
                    {
                        var es = EncryptDataBuffer(set.Serialize(),managementkey); if(es is null) { return false; }

                        this.EncryptedData = es;

                        if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null; MN();

                        if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))            { return false; }
                        if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                        this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);
                    }
                    return all;
                }
                else
                {
                    this.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                    Boolean mod = false; Boolean all = true;

                    foreach(var k in nk)
                    {
                        var kc = this.AllKeys.Values.Where(v => v.ID == k.ID).ToList();

                        if(kc.Any(v => v.Equals(k) is false)) { all = false; continue; }

                        var n = k.ID.Value.ToString()!;

                        if(this.AllKeys.ContainsKey(n)) { all = false; continue; }

                        this.AllKeys[n!] = k.Clone()!; mod = true;
                    }

                    if(mod)
                    {
                        MN();

                        if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))           { return false; }
                        if(sign && managementkey is not null) { if(!UpdateSecureSignature(DataSecurityCNKey,managementkey)) { return false; } }
                    }

                    return all;
                }
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> CheckDataHash(CancellationToken cancel = default)
    {
        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.AllKeys is null) { return false; }

                return await CheckDataHashCore(BitConverter.GetBytes(this.GetHashCode_NoSync()),null,cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CheckOwner"]/*'/>*/
    public Boolean CheckOwner(ManagementKey? managementkey)
    {
        try
        {
            if(this.OwnerSecret is null) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return CheckSecret(managementkey?.Key,this.OwnerSecret); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CheckOwner_NoSync"]/*'/>*/
    private Boolean CheckOwner_NoSync(ManagementKey? managementkey)
    {
        try
        {
            if(this.OwnerSecret is null) { return false; }

            return CheckSecret(managementkey?.Key,this.OwnerSecret);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="ClearKeys"]/*'/>*/
    public Boolean ClearKeys(ManagementKey? managementkey = null)
    {
        try
        {
            if(this.Locked && this.CheckOwner(managementkey) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null) { return true; }

                    ZeroMemory(this.EncryptedData); this.EncryptedData = null;
                    this.DataEncrypted = false; this.DataEncryptInfo = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                    return true;
                }
                else
                {
                    if(this.AllKeys is null) { return true; }

                    foreach(var k in this.AllKeys.Values) { k.ClearKey(); } this.AllKeys = null; MN();

                    if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync()))) { return false; }

                    return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ClearKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is KeySet k ? this.CompareTo(k) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is KeySet k ? this.CompareTo(k) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is KeySet k ? this.CompareTo(k) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is KeySet k ? this.CompareTo(k) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(KeySet? other)
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

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CreateOwnerKey"]/*'/>*/
    public OwnerKey? CreateOwnerKey(String? subject = null)
    {
        try { var _ = new OwnerKey(CreateCertificate(this,subject)!); if(this.TakeOwnership(_)) { return _; } return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateOwnerKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> DecryptData(ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await DecryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!UpdateSecureHash(DataSecurityCNKey,SHA512.HashData(o),true)) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))      { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>()))     { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null; ZeroMemory(o);
                    }

                    return true;
                }
                else
                {
                    MemoryStream? o = null; Byte[]? oa = null;

                    try
                    {
                        var i = this.DataEncrypted && this.EncryptedData is not null ? this.EncryptedData : null;

                        o = (await DecryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream; if(o is null) { return false; }

                        oa = o.ToArray(); var _ = Deserialize(oa) as KeySet; if(_ is null) { return false; }

                        if(_.AllKeys is not null && _.AllKeys.Count > 0)
                        {
                            this.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                            foreach(var k in _.AllKeys) { this.AllKeys[k.Key] = k.Value.Clone()!; }

                            foreach(var k in _.AllKeys.Values) { k.ClearKey(); }
                        }
                        else { this.AllKeys = null; }

                        if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync()))) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))                               { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>()))                              { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null; this.EncryptedData = null;

                        return true;
                    }
                    finally
                    {
                        if(o is not null) { await ZeroMemoryAsync(o,default).ConfigureAwait(false); await o.DisposeAsync().ConfigureAwait(false); } if(oa is not null) { ZeroMemory(oa); }
                    }
                }
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> EncryptData(ManagementKey? managementkey , Boolean sign = true , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await EncryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!UpdateSecureHash(DataSecurityEDKey,SHA512.HashData(o),true))       { return false; }
                        if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                        this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);
                    }

                    return true;
                }
                else
                {
                    Byte[]? i = null;

                    try
                    {
                        i = this.Serialize();

                        using(var o = await EncryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false) as MemoryStream)
                        {
                            if(o is null) { return false; }

                            this.EncryptedData = o.ToArray();

                            if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))            { return false; }
                            if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                            if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                            this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                            if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null;
                        }

                        return true;
                    }
                    finally
                    {
                        if(i is not null) { ZeroMemory(i); }
                    }
                }
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(KeySet? a , KeySet? b) { return a is null ? b is null : a.Equals(b); }
    
    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(KeySet? a , KeySet? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as KeySet); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as KeySet); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as KeySet); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as KeySet); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as KeySet); }

    ///<inheritdoc/>
    public Boolean Equals(KeySet? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            return Guid.Equals(this.GetID(),other.GetID());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
    
    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="FromFile"]/*'/>*/
    public static KeySet? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(KeySet),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return _1.ReadObject(_2) as KeySet;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetAllKeys"]/*'/>*/
    public Dictionary<String,SecurityKey>? GetAllKeys(ManagementKey? managementkey = null)
    {
        try
        {
            if(this.DataMasked && this.DataEncrypted is false)         { return null; }

            if(this.Locked && this.CheckOwner(managementkey) is false) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            var o = new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || managementkey is null) { return null; }

                    ds = DecryptDataBuffer(this.EncryptedData,managementkey); if(ds is null) { return null; }

                    set = Deserialize(ds) as KeySet; if(set?.AllKeys is null) { return null; }                    

                    foreach(var k in set.AllKeys) { o[k.Key] = k.Value.Clone()!; }

                    if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null;

                    return Equals(o.Count,0) is false ? o : null;
                }

                if(this.AllKeys is null) { return null; }

                foreach(var k in this.AllKeys) { o[k.Key] = k.Value.Clone()!; }

                return Equals(o.Count,0) is false ? o : null;
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetAllKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetKey"]/*'/>*/
    public SecurityKey? GetKey(String? name , ManagementKey? managementkey = null)
    {
        try
        {
            if(String.IsNullOrEmpty(name))                               { return null; }
            if(this.DataMasked && this.DataEncrypted is false)           { return null; }
            if(this.Locked && this.CheckManager(managementkey) is false) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || managementkey is null) { return null; }

                    ds = DecryptDataBuffer(this.EncryptedData,managementkey); if(ds is null) { return null; }

                    set = Deserialize(ds) as KeySet; if(set?.AllKeys is null) { return null; }

                    return set.AllKeys.TryGetValue(name,out var k) ? k.Clone() : null;
                }
                else
                {
                    if(this.AllKeys is null) { return null; }

                    return this.AllKeys.TryGetValue(name,out var k) ? k.Clone() : null;
                }
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Descriptor? GetDescriptor()
    {
        try
        {
            return this.GetID() is null ? null :

            new()
            {
                Application        = this.GetApplication(),
                ApplicationVersion = this.GetApplicationVersion()?.ToString(),
                BornOn             = this.GetBornOn()?.ToString("O"),
                DistinguishedName  = this.GetDistinguishedName(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Tags               = this.GetTags(),
                Type               = this.GetType(),
                Version            = this.GetVersion()?.ToString()
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.GetHashCode_NoSync(); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Int32 GetHashCode_NoSync()
    {
        try
        {
            var _ = (this.AllKeys?.Values ?? Enumerable.Empty<SecurityKey>()).OrderBy(v => v.ID)
                .SelectMany(v => BitConverter.GetBytes(v.GetHashCode())).ToArray();

            return BitConverter.ToInt32(SHA512.HashData(this.GetID()!.Value.ToByteArray().Concat(_).ToArray()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static KeySet IParsable<KeySet>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="Parse"]/*'/>*/
    public static new KeySet? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(KeySet)); KeySet? _2 = _1.ReadObject(_0) as KeySet; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="ReleaseOwnership"]/*'/>*/
    public Boolean ReleaseOwnership(ManagementKey? managementkey)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(CheckOwner_NoSync(managementkey) is true) { this.OwnerSecret = null; return true; }

                return false;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReleaseOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="RemoveKey"]/*'/>*/
    public Boolean RemoveKey(String? name , ManagementKey? managementkey = null , Boolean sign = true)
    {
        try
        {
            if(String.IsNullOrEmpty(name) || this.Locked && (this.CheckOwner(managementkey) is false)) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || managementkey is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,managementkey); if(ds is null)              { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                                      { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                    if(set.AllKeys.TryGetValue(name,out var r) is false)                                  { return false; }

                    if(set.AllKeys.Remove(name) is false || r.ClearKey() is false)                        { return false; }

                    if(Equals(set.AllKeys.Count,0)) { set.AllKeys = null; }

                    var es = EncryptDataBuffer(set.Serialize(),managementkey); if(es is null)             { return false; }

                    this.EncryptedData = es;

                    if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                           { return false; }
                    if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey))                { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                          { return false; }

                    this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                    return true;
                }
                else
                {
                    if(this.AllKeys is null) { return false; }

                    if(this.AllKeys.TryGetValue(name,out var r) is false) { return false; }

                    if(this.AllKeys.Remove(name) is false || r.ClearKey() is false) { return false; }

                    if(Equals(this.AllKeys!.Count,0)) { this.AllKeys = null; }

                    MN();

                    if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))           { return false; }
                    if(sign && managementkey is not null) { if(!UpdateSecureSignature(DataSecurityCNKey,managementkey)) { return false; } }

                    return true;
                }
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="RemoveKeys"]/*'/>*/
    public Boolean RemoveKeys(IEnumerable<SecurityKey>? keys , ManagementKey? managementkey = null , Boolean sign = true)
    {
        try
        {
            if(keys is null || this.Locked && (this.CheckOwner(managementkey) is false)) { return false; }

            var rk = keys as IList<SecurityKey> ?? keys.ToList(); if(Equals(rk.Count,0)) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || managementkey is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,managementkey); if(ds is null) { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                         { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                    Boolean mod = false; Boolean all = true;

                    foreach(var k in rk)
                    {
                        var r = set.AllKeys.Where(kv => kv.Value.Equals(k)).Select(kv => kv.Key).ToList();

                        if(Equals(r.Count,0)) { all = false; continue; }

                        foreach(var key in r)
                        {
                            if(set.AllKeys.TryGetValue(key,out var v) is false) { all = false; continue; }

                            if(set.AllKeys.Remove(key) is false || v.ClearKey() is false) { all = false; continue; }
                        }

                        mod = true;
                    }

                    if(mod)
                    {
                        if(Equals(set.AllKeys.Count,0)) { set.AllKeys = null; }

                        var es = EncryptDataBuffer(set.Serialize(),managementkey); if(es is null) { return false; }

                        this.EncryptedData = es;

                        if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null; MN();

                        if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))            { return false; }
                        if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                        this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);
                    }

                    return all;
                }
                else
                {
                    if(this.AllKeys is null) { return false; }

                    Boolean mod = false; Boolean all = true;

                    foreach(var k in rk)
                    {
                        var r = this.AllKeys.Where(kv => kv.Value.Equals(k)).Select(kv => kv.Key).ToList();

                        if(Equals(r.Count,0)) { all = false; continue; }

                        foreach(var key in r)
                        {
                            if(this.AllKeys.TryGetValue(key,out var v) is false) { all = false; continue; }

                            if(this.AllKeys.Remove(key) is false || v.ClearKey() is false) { all = false; continue; }
                        }

                        mod = true;
                    }

                    if(mod)
                    {
                        if(Equals(this.AllKeys!.Count,0)) { this.AllKeys = null; } MN();

                        if(!UpdateSecureHash(DataSecurityCNKey,BitConverter.GetBytes(this.GetHashCode_NoSync())))           { return false; }
                        if(sign && managementkey is not null) { if(!UpdateSecureSignature(DataSecurityCNKey,managementkey)) { return false; } }
                    }

                    return all;
                }
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="TakeOwnership"]/*'/>*/
    public Boolean TakeOwnership(ManagementKey? managementkey = null)
    {
        if( this.Locked || managementkey is null ) { return false; }

        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.OwnerSecret is null || CheckOwner_NoSync(managementkey))
                {
                    this.OwnerSecret = CreateSecret(managementkey.Key); if(this.OwnerSecret is null) { return false; }

                    return CheckManager(managementkey) ? true : RegisterManager_NoSync(managementkey);
                }

                return false;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TakeOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out KeySet key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(KeySet)); KeySet? _2 = _1.ReadObject(_0) as KeySet; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> VerifyData(String? field , ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || String.IsNullOrEmpty(field)) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                switch(field)
                {
                    case DataSecurityCNKey:
                    {
                        if(this.AllKeys is null) { return false; }

                        return await VerifyDataCore(field,managementkey,BitConverter.GetBytes(this.GetHashCode_NoSync()),null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false || this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityHCKey:
                    {
                        return await VerifyDataCore(field,managementkey,null,null,cancel).ConfigureAwait(false);
                    }

                    default: return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> ZeroData(ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        try
        {
            if(this.Locked && this.CheckOwner(managementkey) is false) { return false; }

            await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.AllKeys is not null)
                {
                    foreach(var k in this.AllKeys.Values) { k.ClearKey(); }

                    this.AllKeys.Clear(); this.AllKeys = null;
                }

                if(this.ContentStreamed)
                {
                    var f = this.GetFILE();

                    if(File.Exists(f))
                    {
                        using(var fs = new FileStream(f,FileMode.Open,FileAccess.Write,FileShare.None))
                        {
                            await ZeroMemoryAsync(fs,cancel).ConfigureAwait(false);
                        }
                        File.Delete(f); this.FILE = null;
                    }

                    this.ContentStreamed = false;
                }

                if(this.EncryptedData is not null)
                {
                    ZeroMemory(this.EncryptedData); this.EncryptedData = null;

                    this.DataEncrypted = false; this.DataEncryptInfo = null;
                }

                if(this.SecureHashes is not null)
                {
                    foreach(var _ in this.SecureHashes.Values) { ZeroMemory(_); }

                    this.SecureHashes.Clear(); this.SecureHashes = null;
                }

                if(this.SecureSignatures is not null)
                {
                    foreach(var _ in this.SecureSignatures.Values) { ZeroMemory(_); }

                    this.SecureSignatures.Clear(); this.SecureSignatures = null;
                }

                MN(); return true;
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}