namespace KusDepot;

/**<include file='Common.xml' path='Common/class[@name="Common"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Common")]
[DataContract(Name = "Common" , Namespace = "KusDepot")]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract class Common : ICommon , IComparable<Common> , IEquatable<Common>
{
    /**<include file='Common.xml' path='Common/class[@name="Common"]/property[@name="ExceptionsEnabled"]/*'/>*/
    public Boolean ExceptionsEnabled { get { return !this.MyNoExceptions; } }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    protected Guid? ID;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Locked"]/*'/>*/
    [DataMember(Name = "Locked" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    protected Boolean Locked;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="MyNoExceptions"]/*'/>*/
    [DataMember(Name = "MyNoExceptions" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    protected Boolean MyNoExceptions = true;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/property[@name="MyID"]/*'/>*/
    public String? MyID { get { return this.GetID()?.ToString(); } protected set {} }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/property[@name="MyTypeName"]/*'/>*/
    protected String? MyTypeName { get { return this.GetType()!.Name; } set {} }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Secrets"]/*'/>*/
    [DataMember(Name = "Secrets" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    protected List<Byte[]>? Secrets;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Sync"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    protected SyncNode Sync;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    protected Common() { this.Sync = null!; }

    ///<inheritdoc/>
    public virtual Boolean CheckManager(ManagementKey? managementkey)
    {
        try
        {
            if( this.Secrets is null || Equals(this.Secrets.Count,0) ) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            return this.Secrets.Any(_ => CheckSecret(managementkey?.Key,_.AsSpan()));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="CheckRemoveSecret"]/*'/>*/
    protected virtual Boolean CheckRemoveSecret(Byte[]? certificate , ICollection<Byte[]>? secrets)
    {
        try
        {
            if( secrets is null || Equals(secrets.Count,0) ) { return false; }

            using X509Certificate2? c = DeserializeCertificate(certificate); if(c is null) { return false; }

            ReadOnlySpan<Byte> s = c.SerialNumberBytes.Span; Span<Byte> i = stackalloc Byte[16]; this.GetID()!.Value.TryWriteBytes(i);

            Span<Byte> a = stackalloc Byte[s.Length + i.Length]; s.CopyTo(a[..s.Length]); i.CopyTo(a.Slice(s.Length,i.Length));

            List<Byte[]> r = new(); foreach(var _ in secrets) { if(CheckSecretCore(_.AsSpan(),c,a)) { r.Add(_); } }

            if(Equals(r.Count,0)) { return false; } foreach(var _ in r) { secrets.Remove(_); ZeroMemory(_); }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckRemoveSecretFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="CheckSecret"]/*'/>*/
    protected virtual Boolean CheckSecret(Byte[]? certificate , ReadOnlySpan<Byte> secret)
    {
        try
        {
            using X509Certificate2? c = DeserializeCertificate(certificate); if(c is null) { return false; }

            ReadOnlySpan<Byte> s = c.SerialNumberBytes.Span; Span<Byte> i = stackalloc Byte[16]; this.GetID()!.Value.TryWriteBytes(i);

            Span<Byte> a = stackalloc Byte[s.Length + i.Length]; s.CopyTo(a[..s.Length]); i.CopyTo(a.Slice(s.Length,i.Length));

            return CheckSecretCore(secret,c,a);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckSecretFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="CheckSecrets"]/*'/>*/
    protected virtual Boolean CheckSecrets(Byte[]? certificate , ICollection<Byte[]>? secrets)
    {
        try
        {
            if( secrets is null || Equals(secrets.Count,0) ) { return false; }

            using X509Certificate2? c = DeserializeCertificate(certificate); if(c is null) { return false; }

            ReadOnlySpan<Byte> s = c.SerialNumberBytes.Span; Span<Byte> i = stackalloc Byte[16]; this.GetID()!.Value.TryWriteBytes(i);

            Span<Byte> a = stackalloc Byte[s.Length + i.Length]; s.CopyTo(a[..s.Length]); i.CopyTo(a.Slice(s.Length,i.Length));

            foreach(var _ in secrets) { if(CheckSecretCore(_.AsSpan(),c,a)) { return true; } } return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckSecretsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="CheckSecretCore"]/*'/>*/
    private static Boolean CheckSecretCore(ReadOnlySpan<Byte> secret , X509Certificate2? certificate , ReadOnlySpan<Byte> aad)
    {
        try
        {
            if(certificate is null || aad.IsEmpty || secret.IsEmpty) { return false; }

            return CommonSecret.TryValidate(secret,certificate,new Guid(aad.Slice(certificate.SerialNumberBytes.Length,16)));
        }
        catch { return false; }
    }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(ICommon? other) { return this.CompareTo(other as Common); }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(Common? other)
    {
        try
        {
            if(ReferenceEquals(this,other))          { return 0; }

            Guid? _ = other?.GetID();

            if(this.ID is null     && _ is null)     { return 0; }
            if(this.ID is not null && _ is null)     { return 1; }
            if(this.ID is null     && _ is not null) { return -1; }

            return this.ID!.Value.CompareTo(_!.Value);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareToFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Int32.MinValue; } throw; }
    }

    ///<inheritdoc/>
    public virtual ManagementKey? CreateManagementKey(String? subject)
    {
        if(this.Locked) { return null; }

        try { var _ = new ManagerKey(CreateCertificate(this,subject)!); if(this.RegisterManager(_)) { return _; } return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateManagementKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual ManagementKey? CreateManagementKey(X509Certificate2? certificate)
    {
        if(this.Locked) { return null; }

        try { var _ = new ManagerKey(certificate!); if(this.RegisterManager(_)) { return _; } return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CreateManagementKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="CreateSecret"]/*'/>*/
    protected virtual Byte[]? CreateSecret(Byte[]? certificate)
    {
        try
        {
            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            using X509Certificate2? c = DeserializeCertificate(certificate); if(c is null) { return null; }

            var s = CommonSecret.CreateV1(c,this.GetID()!.Value)?.GetBytes(); if(s is null) { return null; }

            if(this.Secrets is null || Equals(this.Secrets.Count,0)) { return s; }

            foreach(var _ in this.Secrets)
            {
                if(Equals(_.Length,s.Length) && _.AsSpan().SequenceEqual(s)) { ZeroMemory(s); return CreateSecret(certificate); }
            }

            return s;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateSecretFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="DestroySecrets"]/*'/>*/
    protected virtual Boolean DestroySecrets(ManagementKey? managementkey)
    {
        try
        {
            if(this.CheckManager(managementkey) is false) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            if(this.Secrets is null) { return true; }

            foreach(var s in this.Secrets) { ZeroMemory(s); } this.Secrets.Clear();

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DestroySecretsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public virtual Boolean DisableMyExceptions()
    {
        try { this.MyNoExceptions = true; return true; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DisableMyExceptionsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean EnableMyExceptions()
    {
        try { this.MyNoExceptions = false; return true; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EnableMyExceptionsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(Common? a , Common? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(Common? a , Common? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as Common); }

    ///<inheritdoc/>
    public virtual Boolean Equals(ICommon? other) { return this.Equals(other as Common); }

    ///<inheritdoc/>
    public virtual Boolean Equals(Common? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            if(Equals(this.GetType(),other.GetType()) is false) { return false; }

            return Guid.Equals(this.ID,other.GetID());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetDebuggerDisplay"]/*'/>*/
    protected String GetDebuggerDisplay() { return $"{this.GetType().Name} - [{this.GetID()}]"; }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try { return HashCode.Combine(this.ID); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual Guid? GetID()
    {
        try { return this.ID; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetIDFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean GetLocked()
    {
        try { return this.Locked; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetLockedFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetSyncNode"]/*'/>*/
    protected virtual SyncNode GetSyncNode()
    {
        try { this.Sync ??= new(); return this.Sync; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetSyncNodeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean Initialize()
    {
        if(this.Locked) { return false; } this.Sync ??= this.GetSyncNode();

        try
        {
            if(!TryEnter(this.Sync.ID,SyncTime)) { throw SyncException; }

            this.ID = this.ID ?? Guid.NewGuid(); if(this.ID is null) { throw new InitializationException(); }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,InitializeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ID)) { Exit(this.Sync.ID); } }
    }

    ///<inheritdoc/>
    public virtual Boolean Lock(ManagementKey? managementkey)
    {
        Byte[]? s = default;

        try
        {
            if( this.Locked || managementkey is null ) { return false; } s = CreateSecret(managementkey.Key); if(s is null || Equals(s.Length,0)) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw SyncException; } if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            this.Secrets ??= new(); this.Secrets.Add(s); if(this.Secrets.Contains(s)) { this.Locked = true; return true; } return false;
        }
        catch ( Exception _ )
        {
            if(s is not null) { this.Secrets?.Remove(s); }

            this.Locked = false; KusDepotLog.Error(_,LockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw;
        }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    ///<inheritdoc/>
    public virtual Boolean MyExceptionsEnabled() { return this.ExceptionsEnabled; }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="RefreshSyncNodes"]/*'/>*/
    protected virtual void RefreshSyncNodes()
    {
        try { this.Sync = this.GetSyncNode(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,RefreshSyncNodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean RegisterManager(ManagementKey? managementkey)
    {
        try
        {
            if( managementkey is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            this.Secrets ??= new(); Byte[]? _ = CreateSecret(managementkey.Key); if(_ is null || Equals(_.Length,0)) { return false; }

            this.Secrets.Add(_); if(this.Secrets.Contains(_)) { return true; } return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public virtual ManagementKey? RegisterManager(X509Certificate2? certificate)
    {
        try
        {
            if(this.Locked) { return null; }

            var _ = new ManagerKey(certificate!); return this.RegisterManager(_) ? _ : null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetID(Guid? id)
    {
        try
        {
            if( id is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ID,SyncTime)) { throw SyncException; }

            if(Guid.Equals(id,Guid.Empty)) { this.ID = null; return true; }

            this.ID = id; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ID)) { Exit(this.Sync.ID); } }
    }

    ///<inheritdoc/>
    public virtual Boolean UnLock(ManagementKey? managementkey)
    {
        try
        {
            if( this.Secrets is null || Equals(this.Secrets.Count,0) || managementkey is null ) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw SyncException; } if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            if(CheckSecrets(managementkey.Key,this.Secrets)) { this.Locked = false; return true; }

            return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnLockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    ///<inheritdoc/>
    public virtual Boolean UnRegisterManager(ManagementKey? managementkey)
    {
        try
        {
            if( managementkey is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            if(CheckRemoveSecret(managementkey.Key,this.Secrets)) { return true; } return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UnRegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }
}