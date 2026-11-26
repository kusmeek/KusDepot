namespace KusDepot;

/**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.KeySet")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "KeySet" , Namespace = "KusDepot")] [KnownType("GetKnownTypes")]

public sealed partial class KeySet : DataItem , IComparable<KeySet> , IEquatable<KeySet> , IParsable<KeySet>
{
    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/content[@name="AllKeys"]/*'/>*/
    [JsonPropertyName("AllKeys")] [JsonInclude]
    [DataMember(Name = "AllKeys" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal Dictionary<String,SecurityKey>? AllKeys;

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/field[@name="KeyIndex"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    private Dictionary<Guid,String>? KeyIndex;

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/field[@name="OwnerSecret"]/*'/>*/
    [JsonPropertyName("OwnerSecret")] [JsonInclude]
    [DataMember(Name = "OwnerSecret" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    internal Byte[]? OwnerSecret;

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public KeySet() : this(null) {}

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/constructor[@name="Constructor"]/*'/>*/
    public KeySet(IEnumerable<SecurityKey>? keys = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.DataType = DataTypes.KEYSET;

            if(keys is not null)
            {
                this.AllKeys = new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

                foreach(var k in keys)
                {
                    var c = k.Copy(); if(c is null) { continue; } this.AllKeys[c.ID.Value.ToString()] = c;
                }

                if(this.AllKeys.Count == 0) { this.AllKeys = null; }

                else if(!this.RefreshKeyIndex_NoSync()) { throw new InvalidDataException(); }
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="ClearKeyIndex_NoSync"]/*'/>*/
    private void ClearKeyIndex_NoSync()
    {
        this.KeyIndex = null;
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="EnsureKeyIndex_NoSync"]/*'/>*/
    private Boolean EnsureKeyIndex_NoSync()
    {
        return this.KeyIndex is not null || RefreshKeyIndex_NoSync();
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="RefreshKeyIndex_NoSync"]/*'/>*/
    private Boolean RefreshKeyIndex_NoSync()
    {
        if(this.AllKeys is null) { this.KeyIndex = null; return true; }

        if(this.AllKeys.Count == 0) { this.KeyIndex = new(); return true; }

        Dictionary<Guid,String> _ = new(this.AllKeys.Count);

        foreach(var k in this.AllKeys)
        {
            if(k.Value.ID is not Guid id || id == Guid.Empty || _.TryAdd(id,k.Key) is false)
            {
                this.KeyIndex = null; return false;
            }
        }

        this.KeyIndex = _; return true;
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="AddKey"]/*'/>*/
    public Boolean AddKey(SecurityKey? key , String? name = null , DataItemSecurityContext? security = null , Boolean sign = true)
    {
        try
        {
            if(key is null || key.ID is null)                               { return false; }

            if(this.Locked && this.CheckOwner_NoSync(security) is false)    { return false; }

            var h = String.IsNullOrWhiteSpace(name) ? key.ID.Value.ToString()! : name!;

            if(Guid.TryParse(h,out var id) && Equals(id,key.ID) is false)   { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                using DataItemContentMutation mutation = new(this);
                Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null)                                                    { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                                                                       { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!set.EnsureKeyIndex_NoSync()) { return false; }
                    if(set.AllKeys.ContainsKey(h) || set.KeyIndex!.ContainsKey(key.ID.Value))                                              { return false; }

                    set.AllKeys[h] = key.Copy()!; set.KeyIndex[key.ID.Value] = h;

                    var es = EncryptDataBuffer(set.Serialize(),security); if(es is null)                                                   { return false; }

                    this.EncryptedData = es;

                    this.AllKeys = null; this.ClearKeyIndex_NoSync();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))                                                 { return false; }
                    if(sign && !ApplyAssertionState(DataSecurityEDKey,security))                                                        { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))                                                { return false; }

                    this.DataEncrypted = true; this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });
                    ClearKeys(originalKeys);
                    return true;
                }
                else
                {
                    this.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!this.EnsureKeyIndex_NoSync()) { return false; }
                    if(this.AllKeys.ContainsKey(h) || this.KeyIndex!.ContainsKey(key.ID.Value)) { return false; }

                    this.AllKeys[h] = key.Copy()!; this.KeyIndex[key.ID.Value] = h;

                    if(!UpdateContentIntegrityHash()) { return false; }
                    if(sign && !ApplyAssertionState(DataSecurityCNKey,security))                    { return false; }

                    mutation.Commit(() => { MN(); });
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
    public Boolean AddKeys(IEnumerable<SecurityKey>? keys , DataItemSecurityContext? security = null , Boolean sign = true)
    {
        try
        {
            if(keys is null || this.Locked && (this.CheckOwner_NoSync(security) is false)) { return false; }

            var nk = keys as IList<SecurityKey> ?? keys.ToList(); if(nk.Count == 0)   { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                using DataItemContentMutation mutation = new(this);
                Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null) { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                    { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!set.EnsureKeyIndex_NoSync()) { return false; }

                    HashSet<Guid> ids = new(); Boolean mod = false; Boolean all = true;

                    foreach(var k in nk)
                    {
                        if(k.ID is not Guid id || id == Guid.Empty || set.KeyIndex!.ContainsKey(id) || !ids.Add(id)) { all = false; continue; }

                        var n = id.ToString()!;

                        if(set.AllKeys.ContainsKey(n)) { all = false; continue; }

                        set.AllKeys[n!] = k.Copy()!; set.KeyIndex[id] = n; mod = true;
                    }

                    if(mod)
                    {
                        var es = EncryptDataBuffer(set.Serialize(),security); if(es is null) { return false; }

                        this.EncryptedData = es;

                        this.AllKeys = null; this.ClearKeyIndex_NoSync();

                        if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))   { return false; }
                        if(sign && !ApplyAssertionState(DataSecurityEDKey,security))          { return false; }
                        if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))  { return false; }

                        this.DataEncrypted = true; this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                        mutation.Commit(() => { MN(); });
                        ClearKeys(originalKeys);
                    }
                    return all;
                }
                else
                {
                    this.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!this.EnsureKeyIndex_NoSync()) { return false; }

                    HashSet<Guid> ids = new(); Boolean mod = false; Boolean all = true;

                    foreach(var k in nk)
                    {
                        if(k.ID is not Guid id || id == Guid.Empty || this.KeyIndex!.ContainsKey(id) || !ids.Add(id)) { all = false; continue; }

                        var n = id.ToString()!;

                        if(this.AllKeys.ContainsKey(n)) { all = false; continue; }

                        this.AllKeys[n!] = k.Copy()!; this.KeyIndex[id] = n; mod = true;
                    }

                    if(mod)
                    {
                        if(!UpdateContentIntegrityHash()) { return false; }
                        if(sign && !ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                        mutation.Commit(() => { MN(); });
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

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CanChangeProtectionState"]/*'/>*/
    private protected override Boolean CanChangeProtectionState(DataItemSecurityContext security) => !this.Locked || this.CheckOwner_NoSync(security);

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CaptureContentIntegritySnapshot_NoSync"]/*'/>*/
    internal override ContentIntegritySnapshot CaptureContentIntegritySnapshot_NoSync()
    {
        if(this.DataEncrypted)
        {
            ContentIntegrityValue encryptedContent = ContentIntegrityValue.FromBytes(this.EncryptedData);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContent)],[encryptedContent]);
        }

        Dictionary<String,Object?>? keys = null; List<IDisposable>? ownedResources = null;

        if(this.AllKeys is { Count: > 0 })
        {
            keys = new Dictionary<String,Object?>(this.AllKeys.Count,StringComparer.OrdinalIgnoreCase);

            ownedResources = new List<IDisposable>(this.AllKeys.Count);

            foreach(KeyValuePair<String,SecurityKey> entry in this.AllKeys)
            {
                ContentIntegrityValue keyBytes = ContentIntegrityValue.FromBytes(entry.Value.GetKey());

                ownedResources.Add(keyBytes);

                keys.Add(entry.Key,new Object?[] { entry.Value.GetType(),entry.Value.ID,keyBytes });
            }
        }

        return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("AllKeys",keys)],ownedResources);
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CaptureContentMutationMemento_NoSync"]/*'/>*/
    internal override DataItemContentMutationMemento CaptureContentMutationMemento_NoSync()
    {
        Dictionary<String,SecurityKey>? originalKeys = this.AllKeys is null ? null : new(this.AllKeys,this.AllKeys.Comparer);
        Dictionary<Guid,String>? originalKeyIndex = this.KeyIndex is null ? null : new(this.KeyIndex);
        HashSet<SecurityKey> originalKeyReferences = originalKeys is null ? new(ReferenceEqualityComparer.Instance) : new(originalKeys.Values,ReferenceEqualityComparer.Instance);

        return new DataItemContentMutationMemento(item =>
        {
            KeySet keySet = (KeySet)item;

            if(keySet.AllKeys is not null)
            {
                foreach(SecurityKey currentKey in keySet.AllKeys.Values)
                {
                    if(!originalKeyReferences.Contains(currentKey)) { currentKey.ClearKey(); }
                }
            }

            keySet.AllKeys = originalKeys;
            keySet.KeyIndex = originalKeyIndex;
        });
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

                return await this.CheckDataHashCoreHashed(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
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

    private Boolean CheckOwner_NoSync(DataItemSecurityContext? security)
    {
        try
        {
            if(this.OwnerSecret is null) { return false; }

            return CheckSecret(GetAuthorizationCertificate(security),this.OwnerSecret);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="ClearKeys"]/*'/>*/
    public Boolean ClearKeys(DataItemSecurityContext? security = null)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Locked && this.CheckOwner_NoSync(security) is false) { return false; }

                using DataItemContentMutation mutation = new(this);
                Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;
                Byte[]? originalEncryptedData = this.EncryptedData;

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null) { return true; }

                    this.EncryptedData = null;

                    this.DataEncrypted = false; this.DataProtectionInfo = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                    mutation.Commit(() => { MN(); });
                    ZeroMemory(originalEncryptedData);
                    return true;
                }
                else
                {
                    if(this.AllKeys is null) { return true; }

                    this.AllKeys = null; this.ClearKeyIndex_NoSync();

                    if(!UpdateContentIntegrityHash()) { return false; }

                    mutation.Commit(() => { MN(); });
                    ClearKeys(originalKeys);
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

            if(other is null)                            { return this.BornOn is null ? 0 : 1; }

            if(!TryAcquireMetaLocks(other,out LockState s)) { throw SyncException; }

            try
            {
                DateTimeOffset? _ = this.BornOn; DateTimeOffset? o = other.BornOn;

                if(_ is null     && o is null)     { return 0; }
                if(_ is not null && o is null)     { return 1; }
                if(_ is null     && o is not null) { return -1; }

                return _!.Value.CompareTo(o!.Value);
            }
            finally { ReleaseSyncLocks(s); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareToFail,MyTypeName,MyID); throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="CreateOwnerKey"]/*'/>*/
    public OwnerKey? CreateOwnerKey(String? subject = null)
    {
        try { var _ = new OwnerKey(CreateCertificate(this,subject)!); if(this.TakeOwnership(_)) { return _; } return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateOwnerKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="ClearKeysCore"]/*'/>*/
    private static void ClearKeys(Dictionary<String,SecurityKey>? keys)
    {
        if(keys is null) { return; }
        foreach(SecurityKey key in keys.Values) { key.ClearKey(); }
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

            if(!TryAcquireSyncLocks(other,out LockState s)) { return false; }

            try
            {
                if(this.DataEncrypted || other.DataEncrypted) { return false; }

                var a = this.AllKeys; var b = other.AllKeys;

                if(a is null && b is null) { return true; }

                if(a is null || b is null) { return false; }

                if(a.Count != b.Count) { return false; }

                foreach(var kv in a)
                {
                    if(b.TryGetValue(kv.Key,out var otherKey) is false) { return false; }

                    if(kv.Value.Equals(otherKey) is false) { return false; }
                }

                return true;
            }
            finally { ReleaseSyncLocks(s); }
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

            return OrleansUtility.FromFile<KeySet>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetAllKeys"]/*'/>*/
    public Dictionary<String,SecurityKey>? GetAllKeys(DataItemSecurityContext? security = null)
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            var o = new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if(this.Locked && this.CheckOwner_NoSync(security) is false) { return null; }

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return null; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null) { return null; }

                    set = Deserialize(ds) as KeySet; if(set?.AllKeys is null) { return null; }

                    foreach(var k in set.AllKeys) { o[k.Key] = k.Value.Copy()!; }

                    if(this.AllKeys is not null) { foreach(var k in this.AllKeys.Values) { k.ClearKey(); } } this.AllKeys = null;

                    return o.Count != 0 ? o : null;
                }

                if(this.AllKeys is null) { return null; }

                foreach(var k in this.AllKeys) { o[k.Key] = k.Value.Copy()!; }

                return o.Count != 0 ? o : null;
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

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetKeys"]/*'/>*/
    public Dictionary<String,SecurityKey>? GetKeys(IEnumerable<String>? names = null , DataItemSecurityContext? security = null)
    {
        try
        {
            if(names is null) { return this.GetAllKeys(security); }

            if(this.Locked && this.CheckOwner_NoSync(security) is false) { return null; }

            HashSet<String> _ = names as HashSet<String> ?? new(names,StringComparer.OrdinalIgnoreCase); if(_.Count == 0) { return new(StringComparer.OrdinalIgnoreCase); }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            var o = new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return null; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null) { return null; }

                    set = Deserialize(ds) as KeySet; if(set?.AllKeys is null) { return o; }

                    foreach(var name in _)
                    {
                        if(set.AllKeys.TryGetValue(name,out var key) && key.Copy() is {} copy) { o[name] = copy; }
                    }

                    return o;
                }

                if(this.AllKeys is null) { return o; }

                foreach(var name in _)
                {
                    if(this.AllKeys.TryGetValue(name,out var key) && key.Copy() is {} copy) { o[name] = copy; }
                }

                return o;
            }
            finally
            {
                if(ds is not null) { ZeroMemory(ds); }
                if(set is not null && set!.AllKeys?.Count > 0 )
                { foreach(var k in set.AllKeys.Values) { k.ClearKey(); } }
                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Descriptor? GetDescriptor()
    {
        try
        {
            Guid? id = this.GetID(); if(id is null) { return null; }

            Boolean lk = false; this.AcquireLocks(); lk = true;

            try
            {
                if(DataIsolation.IsEnabled())
                {
                    return new()
                    {
                        Application        = this.Application is null ? null : new(this.Application),
                        ApplicationVersion = this.ApplicationVersion?.ToString(),
                        BornOn             = this.BornOn?.ToString("O"),
                        DataType           = this.DataType is null ? null : new(this.DataType),
                        DistinguishedName  = this.DistinguishedName is null ? null : new(this.DistinguishedName),
                        ID                 = id,
                        Modified           = this.Modified?.ToString("O"),
                        Name               = this.Name is null ? null : new(this.Name),
                        Notes              = this.Notes?.Select(n => new String(n)).ToHashSet(),
                        ObjectFILE         = this.ObjectFILE is null ? null : new(this.ObjectFILE),
                        ObjectType         = this.GetCachedInheritanceList(),
                        ServiceVersion     = this.ServiceVersion?.ToString(),
                        Tags               = this.Tags?.Select(t => new String(t)).ToHashSet(),
                        Version            = this.Version?.ToString()
                    };
                }

                return new()
                {
                    Application        = this.Application,
                    ApplicationVersion = this.ApplicationVersion?.ToString(),
                    BornOn             = this.BornOn?.ToString("O"),
                    DataType           = this.DataType,
                    DistinguishedName  = this.DistinguishedName,
                    ID                 = id,
                    Modified           = this.Modified?.ToString("O"),
                    Name               = this.Name,
                    Notes              = this.Notes,
                    ObjectFILE         = this.ObjectFILE,
                    ObjectType         = this.GetCachedInheritanceList(),
                    ServiceVersion     = this.ServiceVersion?.ToString(),
                    Tags               = this.Tags,
                    Version            = this.Version?.ToString()
                };
            }
            finally { if(lk) { this.ReleaseLocks(); } }
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
    public override async Task<Int32> GetHashCodeAsync(CancellationToken cancel = default)
    {
        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try { return await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Int32 GetHashCode_NoSync()
    {
        try
        {
            if(this.DataEncrypted) { return HashCode.Combine(true,this.ID); }

            List<Int32> keyHashes = this.AllKeys is null ? new() : new(this.AllKeys.Count);

            if(this.AllKeys is not null)
            {
                foreach(KeyValuePair<String,SecurityKey> entry in this.AllKeys)
                {
                    keyHashes.Add(HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(entry.Key),entry.Value.GetHashCode()));
                }
            }

            keyHashes.Sort();

            HashCode hash = new(); hash.Add(false); hash.Add(this.AllKeys is not null); hash.Add(keyHashes.Count);

            foreach(Int32 keyHash in keyHashes) { hash.Add(keyHash); }

            return hash.ToHashCode();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            return this.GetHashCode_NoSync();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetKey"]/*'/>*/
    public SecurityKey? GetKey(String? name , DataItemSecurityContext? security = null)
    {
        try
        {
            if(String.IsNullOrEmpty(name))                               { return null; }
            if(this.Locked && this.CheckOwner_NoSync(security) is false) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return null; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null) { return null; }

                    set = Deserialize(ds) as KeySet; if(set?.AllKeys is null) { return null; }

                    return set.AllKeys.TryGetValue(name,out var k) ? k.Copy() : null;
                }
                else
                {
                    if(this.AllKeys is null) { return null; }

                    return this.AllKeys.TryGetValue(name,out var k) ? k.Copy() : null;
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

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    ///<inheritdoc/>
    public override Boolean IsMetadataOnly()
    {
        Boolean lk = false;

        try
        {
            this.AcquireLocks(); lk = true;

            try
            {
                return this.ContentStreamed is false
                    && String.IsNullOrWhiteSpace(this.FILE)
                    && this.AllKeys is null
                    && this.EncryptedData is null;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsMetadataOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        this.GetSyncNode(); this.RefreshSyncNodes(); if(!this.RefreshKeyIndex_NoSync()) { throw new SerializationException(); }
    }

    ///<inheritdoc/>
    static KeySet IParsable<KeySet>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="Parse"]/*'/>*/
    public static new KeySet? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            return OrleansUtility.ParseBase64<KeySet>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareDecryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed) { return null; }

        await using MemoryStream output = CreateBuffer(this.EncryptedData?.Length ?? 0);

        if(!await this.DecryptToStream(this.EncryptedData,output,security,cancel).ConfigureAwait(false)) { return null; }

        KeySet? decrypted = Deserialize<KeySet>(output); if(decrypted is null) { return null; }

        Dictionary<String,SecurityKey>? candidate = null;

        Dictionary<Guid,String>? candidateIndex = null;

        if(decrypted.AllKeys is { Count: > 0 })
        {
            candidate = new Dictionary<String,SecurityKey>(decrypted.AllKeys.Count,StringComparer.OrdinalIgnoreCase);

            foreach(var pair in decrypted.AllKeys)
            {
                SecurityKey? copy = pair.Value.Copy(); if(copy is null) { ClearKeys(candidate); ClearKeys(decrypted.AllKeys); return null; }

                candidate.Add(pair.Key,copy);
            }
        }

        if(decrypted.AllKeys is not null) { foreach(SecurityKey key in decrypted.AllKeys.Values) { key.ClearKey(); } }

        if(candidate is not null)
        {
            candidateIndex = new(candidate.Count);

            foreach(var pair in candidate)
            {
                if(pair.Value.ID is not Guid id || id == Guid.Empty || !candidateIndex.TryAdd(id,pair.Key)) { ClearKeys(candidate); return null; }
            }
        }

        Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;

        Dictionary<Guid,String>? originalIndex = this.KeyIndex;

        return new DataItemProtectionChange(
            () => { this.AllKeys = candidate; this.KeyIndex = candidateIndex; return ValueTask.CompletedTask; },
            () => { this.AllKeys = originalKeys; this.KeyIndex = originalIndex; ClearKeys(candidate); return ValueTask.CompletedTask; });
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareEncryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed) { return null; }

        Byte[]? serialized = this.Serialize_NoSync();

        try
        {
            DataItemSecurityEnvelopeArrayHashResult? encrypted = await this.EncryptDataBufferWithHash(serialized,security,cancel).ConfigureAwait(false);

            if(!encrypted.HasValue) { return null; }

            Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;

            Dictionary<Guid,String>? originalIndex = this.KeyIndex;

            return new DataItemProtectionChange(
                () => { this.AllKeys = null; this.ClearKeyIndex_NoSync(); return ValueTask.CompletedTask; },
                () => { this.AllKeys = originalKeys; this.KeyIndex = originalIndex; return ValueTask.CompletedTask; },
                encrypted.Value.Hash,
                encrypted.Value.Buffer,
                () => { ClearKeys(originalKeys); return ValueTask.CompletedTask; });
        }
        finally { ZeroMemory(serialized); }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> VerifyData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || String.IsNullOrEmpty(field)) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                switch(field)
                {
                    case DataSecurityCNKey:
                    {
                        if(this.AllKeys is null) { return false; }

                        if(this.AllKeys is not { Count: > 0 }) { return false; }

                        return await VerifyDataCoreHashed(field,security,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
                    }

                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false || this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,security,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityHCKey:
                    case DataSecurityMDKey:
                    {
                        return await VerifyDataCore(field,security,null,null,cancel).ConfigureAwait(false);
                    }

                    default: return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> WipeData(DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            return await this.WipeDataCore(security,new HashSet<Object>(ReferenceEqualityComparer.Instance),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
    public Boolean RemoveKey(String? name , DataItemSecurityContext? security = null , Boolean sign = true)
    {
        try
        {
            if(String.IsNullOrEmpty(name)) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.Locked && this.CheckOwner_NoSync(security) is false) { return false; }

                using DataItemContentMutation mutation = new(this);
                Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null)                   { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                                      { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!set.EnsureKeyIndex_NoSync()) { return false; }

                    if(set.AllKeys.TryGetValue(name,out var r) is false)                                  { return false; }

                    if(set.AllKeys.Remove(name) is false || r.ClearKey() is false)                        { return false; }

                    set.KeyIndex!.Remove(r.ID!.Value);

                    if(set.AllKeys.Count == 0) { set.AllKeys = null; set.ClearKeyIndex_NoSync(); }

                    var es = EncryptDataBuffer(set.Serialize(),security); if(es is null)                  { return false; }

                    this.EncryptedData = es;

                    this.AllKeys = null; this.ClearKeyIndex_NoSync();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))                  { return false; }
                    if(sign && !ApplyAssertionState(DataSecurityEDKey,security))                         { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))                 { return false; }

                    this.DataEncrypted = true; this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });
                    ClearKeys(originalKeys);
                    return true;
                }
                else
                {
                    if(this.AllKeys is null) { return false; }

                    if(this.AllKeys.TryGetValue(name,out var r) is false) { return false; }

                    if(this.AllKeys.Remove(name) is false) { return false; }

                    this.KeyIndex?.Remove(r.ID!.Value);

                    if(this.AllKeys!.Count == 0) { this.AllKeys = null; this.ClearKeyIndex_NoSync(); }

                    if(!UpdateContentIntegrityHash()) { return false; }
                    if(sign && !ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                    mutation.Commit(() => { MN(); });
                    return r.ClearKey();
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
    public Boolean RemoveKeys(IEnumerable<SecurityKey>? keys , DataItemSecurityContext? security = null , Boolean sign = true)
    {
        try
        {
            if(keys is null) { return false; }

            var rk = keys as IList<SecurityKey> ?? keys.ToList(); if(rk.Count == 0) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.Locked && this.CheckOwner_NoSync(security) is false) { return false; }

                using DataItemContentMutation mutation = new(this);
                Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;
                List<SecurityKey> removedKeys = new();

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null) { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                         { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!set.EnsureKeyIndex_NoSync()) { return false; }

                    Boolean mod = false; Boolean all = true;

                    foreach(var k in rk)
                    {
                        if(k.ID is not Guid id || !set.KeyIndex!.TryGetValue(id,out var key)) { all = false; continue; }
                        if(set.AllKeys.TryGetValue(key,out var v) is false) { all = false; continue; }

                        if(set.AllKeys.Remove(key) is false || set.KeyIndex.Remove(id) is false) { all = false; continue; }
                        if(!v.ClearKey()) { all = false; }

                        mod = true;
                    }

                    if(mod)
                    {
                        if(set.AllKeys.Count == 0) { set.AllKeys = null; set.ClearKeyIndex_NoSync(); }

                        var es = EncryptDataBuffer(set.Serialize(),security); if(es is null) { return false; }

                        this.EncryptedData = es;

                        this.AllKeys = null; this.ClearKeyIndex_NoSync();

                        if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))   { return false; }
                        if(sign && !ApplyAssertionState(DataSecurityEDKey,security))          { return false; }
                        if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))  { return false; }

                        this.DataEncrypted = true; this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                        mutation.Commit(() => { MN(); });
                        ClearKeys(originalKeys);
                    }

                    return all;
                }
                else
                {
                    if(this.AllKeys is null) { return false; }

                    if(!this.EnsureKeyIndex_NoSync()) { return false; } Boolean mod = false; Boolean all = true;

                    foreach(var k in rk)
                    {
                        if(k.ID is not Guid id || !this.KeyIndex!.TryGetValue(id,out var key)) { all = false; continue; }

                        if(this.AllKeys.TryGetValue(key,out var v) is false) { all = false; continue; }

                        if(this.AllKeys.Remove(key) is false || this.KeyIndex.Remove(id) is false) { all = false; continue; }

                        removedKeys.Add(v);
                        mod = true;
                    }

                    if(mod)
                    {
                        if(this.AllKeys!.Count == 0) { this.AllKeys = null; this.ClearKeyIndex_NoSync(); }

                        if(!UpdateContentIntegrityHash()) { return false; }
                        if(sign && !ApplyAssertionState(DataSecurityCNKey,security))                    { return false; }

                        mutation.Commit(() => { MN(); });
                        foreach(SecurityKey removedKey in removedKeys) { if(!removedKey.ClearKey()) { all = false; } }
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

    /**<include file='KeySet.xml' path='KeySet/class[@name="KeySet"]/method[@name="RemoveKeysName"]/*'/>*/
    public Boolean RemoveKeys(IEnumerable<String>? names , DataItemSecurityContext? security = null , Boolean sign = true)
    {
        try
        {
            if(names is null) { return false; }

            HashSet<String> _ = names as HashSet<String> ?? new(names,StringComparer.OrdinalIgnoreCase); if(_.Count == 0) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? ds = null; KeySet? set = null;

            try
            {
                if(this.Locked && this.CheckOwner_NoSync(security) is false) { return false; }

                using DataItemContentMutation mutation = new(this);
                Dictionary<String,SecurityKey>? originalKeys = this.AllKeys;
                List<SecurityKey> removedKeys = new();

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData is null || security is null) { return false; }

                    ds = DecryptDataBuffer(this.EncryptedData,security); if(ds is null) { return false; }

                    set = Deserialize(ds) as KeySet; if(set is null)                    { return false; }

                    set.AllKeys ??= new Dictionary<String,SecurityKey>(StringComparer.OrdinalIgnoreCase); if(!set.EnsureKeyIndex_NoSync()) { return false; }

                    Boolean mod = false; Boolean all = true;

                    foreach(var name in _)
                    {
                        if(set.AllKeys.TryGetValue(name,out var v) is false) { all = false; continue; }

                        if(set.AllKeys.Remove(name) is false || set.KeyIndex!.Remove(v.ID!.Value) is false) { all = false; continue; }
                        if(!v.ClearKey()) { all = false; }

                        mod = true;
                    }

                    if(mod)
                    {
                        if(set.AllKeys.Count == 0) { set.AllKeys = null; set.ClearKeyIndex_NoSync(); }

                        var es = EncryptDataBuffer(set.Serialize(),security); if(es is null) { return false; }

                        this.EncryptedData = es;

                        this.AllKeys = null; this.ClearKeyIndex_NoSync();

                        if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))   { return false; }
                        if(sign && !ApplyAssertionState(DataSecurityEDKey,security))          { return false; }
                        if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))  { return false; }

                        this.DataEncrypted = true; this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                        mutation.Commit(() => { MN(); });
                        ClearKeys(originalKeys);
                    }

                    return all;
                }
                else
                {
                    if(this.AllKeys is null) { return false; }

                    if(!this.EnsureKeyIndex_NoSync()) { return false; } Boolean mod = false; Boolean all = true;

                    foreach(var name in _)
                    {
                        if(this.AllKeys.TryGetValue(name,out var v) is false) { all = false; continue; }

                        if(this.AllKeys.Remove(name) is false || this.KeyIndex!.Remove(v.ID!.Value) is false) { all = false; continue; }

                        removedKeys.Add(v);
                        mod = true;
                    }

                    if(mod)
                    {
                        if(this.AllKeys!.Count == 0) { this.AllKeys = null; this.ClearKeyIndex_NoSync(); }

                        if(!UpdateContentIntegrityHash()) { return false; }
                        if(sign && !ApplyAssertionState(DataSecurityCNKey,security))                    { return false; }

                        mutation.Commit(() => { MN(); });

                        foreach(SecurityKey removedKey in removedKeys) { if(!removedKey.ClearKey()) { all = false; } }
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

    ///<inheritdoc/>
    public override async Task<String?> SignData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        if(String.Equals(field,DataSecurityCNKey,Ordinal))
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try { if(this.AllKeys is not { Count: > 0 }) { return null; } }

            finally { this.Sync.Data.Release(); }
        }

        return await base.SignData(field,security,cancel).ConfigureAwait(false);
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
            return OrleansUtility.TryParseBase64(input,out key);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataCore(DataItemSecurityContext? security , HashSet<Object> visited , CancellationToken cancel = default)
    {
        if(await base.WipeDataCore(security,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckOwner_NoSync(security) is false) { return false; }

        Boolean lk = false; await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true;

        try
        {
            if(this.AllKeys is not null)
            {
                foreach(var k in this.AllKeys.Values) { k.ClearKey(); }

                this.AllKeys.Clear(); this.AllKeys = null; this.ClearKeyIndex_NoSync();
            }

            if(this.ContentStreamed) {}

            if(this.EncryptedData is not null)
            {
                ZeroMemory(this.EncryptedData); this.EncryptedData = null;

                this.DataEncrypted = false; this.DataProtectionInfo = null;
            }

            if(this.FieldIntegrityHashes is not null)
            {
                foreach(var _ in this.FieldIntegrityHashes.Values) { ZeroMemory(_); }

                this.FieldIntegrityHashes.Clear(); this.FieldIntegrityHashes = null;
            }

            if(this.FieldAssertions is not null)
            {
                foreach(var _ in this.FieldAssertions.Values) { ZeroMemory(_); }

                this.FieldAssertions.Clear(); this.FieldAssertions = null;
            }

            MN(); return true;
        }
        finally { if(lk) { this.ReleaseLocks(); } }
    }

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataInMemoryOnlyCore(DataItemSecurityContext? security , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        Boolean lk = false;
        try
        {
            if(await base.WipeDataInMemoryOnlyCore(security,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckOwner_NoSync(security) is false) { return false; }

            if(requirelocks) { await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true; }

            try
            {
                if(this.AllKeys is not null)
                {
                    foreach(var k in this.AllKeys.Values) { k.ClearKey(); }

                this.AllKeys.Clear(); this.AllKeys = null; this.ClearKeyIndex_NoSync();
                }

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}