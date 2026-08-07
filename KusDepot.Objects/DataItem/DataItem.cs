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
[JsonDerivedType(typeof(DataStreamItem),typeDiscriminator:nameof(DataStreamItem))]
[JsonDerivedType(typeof(MultiMediaItem),typeDiscriminator:nameof(MultiMediaItem))]
[JsonDerivedType(typeof(GuidReferenceItem),typeDiscriminator:nameof(GuidReferenceItem))]

public abstract partial class DataItem : MetaBase , ICloneable , IComparable<DataItem> , IDataItem , IEquatable<DataItem>
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonInclude]
    [DataMember(Name = "ContentStreamed" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    protected internal Boolean ContentStreamed;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataEncrypted"]/*'/>*/
    [JsonPropertyName("DataEncrypted")] [JsonInclude]
    [DataMember(Name = "DataEncrypted" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    protected internal Boolean DataEncrypted = false;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataProtectionInfo"]/*'/>*/
    [JsonPropertyName("DataProtectionInfo")] [JsonInclude]
    [DataMember(Name = "DataProtectionInfo" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    protected internal DataProtectionInfo? DataProtectionInfo;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonInclude] [Description(DataTypeDescription)]
    [DataMember(Name = "DataType" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    protected internal String? DataType;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="EncryptedData"]/*'/>*/
    [JsonPropertyName("EncryptedData")] [JsonInclude]
    [DataMember(Name = "EncryptedData" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    protected internal Byte[]? EncryptedData;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="CachedHashCode"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    private Int32 CachedHashCode;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="FieldAssertions"]/*'/>*/
    [JsonPropertyName("FieldAssertions")] [JsonInclude]
    [DataMember(Name = "FieldAssertions" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    protected internal Dictionary<String,Byte[]>? FieldAssertions;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="FieldIntegrityHashes"]/*'/>*/
    [JsonPropertyName("FieldIntegrityHashes")] [JsonInclude]
    [DataMember(Name = "FieldIntegrityHashes" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    protected internal Dictionary<String,Byte[]>? FieldIntegrityHashes;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="HashCodeCached"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    private Boolean HashCodeCached;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="Sync"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    protected new DataSync Sync;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="SyncLockCount"]/*'/>*/
    private static Int64 SyncLockCount;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="SyncLockOrder"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    private Int64 SyncLockOrder;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    protected DataItem()
    {
        this.Sync = (this.GetSyncNode() as DataSync)!; this.SyncLockOrder = Interlocked.Increment(ref SyncLockCount);
    }

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
            if(ReferenceEquals(this,other)) { return 0; }

            if(other is null)               { return this.BornOn is null ? 0 : 1; }

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
    public virtual Boolean Equals(IDataItem? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as DataItem); }

    ///<inheritdoc/>
    public virtual Boolean Equals(DataItem? other) { return ReferenceEquals(this,other); }

    ///<inheritdoc/>
    public virtual Boolean GetContentStreamed() { return this.ContentStreamed; }

    ///<inheritdoc/>
    public virtual Task<DataContent?> GetDataContent(DataItemSecurityContext? security = null , CancellationToken cancel = default) => Task.FromResult<DataContent?>(null);

    ///<inheritdoc/>
    public virtual Boolean GetDataEncrypted() { return this.DataEncrypted; }

    ///<inheritdoc/>
    public virtual DataProtectionInfo? GetDataProtectionInfo()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataProtectionInfo is null) { return null; }

                return new DataProtectionInfo { Version = this.DataProtectionInfo.Version, ProtectedAt = this.DataProtectionInfo.ProtectedAt, ProtectedByObjectId = this.DataProtectionInfo.ProtectedByObjectId, ProtectedByThumbprint = this.DataProtectionInfo.ProtectedByThumbprint, Purpose = this.DataProtectionInfo.Purpose, Recipients = this.DataProtectionInfo.Recipients, Assertions = this.DataProtectionInfo.Assertions, HasMultipleRecipients = this.DataProtectionInfo.HasMultipleRecipients, ProtectionMode = this.DataProtectionInfo.ProtectionMode };
            }

            finally { this.Sync.Data.Release(); }
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataProtectInfoFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetDataType()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.DataType is null ? null : DataIsolation.IsEnabled() ? new(this.DataType) : this.DataType; }

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
    public override Boolean Initialize()
    {
        if(this.Locked) { return false; } this.GetSyncNode(); base.Initialize();

        try
        {
            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                this.BornOn = this.BornOn ?? DateTimeOffset.Now; this.Modified = this.Modified ?? this.BornOn;

                if(this.BornOn is null || this.Modified is null) { throw new InitializationException(); }

                return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InitializeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean IsMetadataOnly()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                return
                    this.ContentStreamed is false
                    && String.IsNullOrWhiteSpace(this.FILE)
                    && this.EncryptedData is null;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsMetadataOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean Lock(ManagementKey? managementkey)
    {
        Boolean ds = false;
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; } ds = true;

            return base.Lock(managementkey);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,LockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ds) { this.Sync.Data.Release(); } }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ModifiedNow"]/*'/>*/
    protected override Boolean MN()
    {
        this.ClearHashCodeCache_NoSync(); return base.MN();
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
    public virtual Task<Boolean> SetContentStreamed(Boolean streamed , DataItemSecurityContext? security = null , CancellationToken cancel = default)
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
                String? _ = String.IsNullOrEmpty(type) ? null : type;

                if(String.Equals(this.DataType,_,StringComparison.Ordinal)) { return true; }

                this.DataType = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
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

            try
            {
                Boolean _ = base.SetID(id);

                if(_) { this.ClearHashCodeCache_NoSync(); }

                return _;
            }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetAuthorizationCertificate"]/*'/>*/
    protected Byte[]? GetAuthorizationCertificate(DataItemSecurityContext? security)
    {
        try
        {
            X509Certificate2? certificate = security?.LocalObject.Certificate;

            return certificate is null ? null : SerializeCertificate(certificate);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CheckManager"]/*'/>*/
    protected Boolean CheckManager(DataItemSecurityContext? security)
    {
        try
        {
            if(this.Secrets is null || this.Secrets.Count == 0) { return false; }

            return this.CheckSecrets(GetAuthorizationCertificate(security),this.Secrets);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}