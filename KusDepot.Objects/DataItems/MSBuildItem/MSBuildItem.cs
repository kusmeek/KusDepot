namespace KusDepot;

/**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.MSBuildItem")]
[DataContract(Name = "MSBuildItem" , Namespace = "KusDepot")]

public sealed class MSBuildItem : DataItem , IComparable<MSBuildItem> , IEquatable<MSBuildItem> , IExtensibleDataObject , IParsable<MSBuildItem>
{
    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/content[@name="Content"]/*'/>*/
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private String? Content;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Requirements"]/*'/>*/
    [DataMember(Name = "Requirements" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    private HashSet<DataItem>? Requirements;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Sequence"]/*'/>*/
    [DataMember(Name = "Sequence" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    private SortedList<Int32,MSBuildItem>? Sequence;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public MSBuildItem() : this(null,null,null,null,null,null) {}

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/constructor[@name="Constructor"]/*'/>*/
    public MSBuildItem(String? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try
        {
            this.Sync = null!; this.DataType = DataTypes.MSB; this.Initialize();

            this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.AddNotes(notes); this.AddTags(tags);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
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
                    if(this.ContentStreamed)
                    {
                        var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = File.OpenRead(f);

                        return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                    }

                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = File.OpenRead(f);

                    return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                }

                if(this.Content is null) { return false; }

                return await CheckDataHashCore(this.Content.ToByteArrayFromUTF16String(),null,cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is MSBuildItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is MSBuildItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is MSBuildItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is MSBuildItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(MSBuildItem? other)
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
    public override async Task<Boolean> DecryptData(ManagementKey? managementkey , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || this.Locked) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.AcquireLocksAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await DecryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!await UpdateSecureHashAsync(DataSecurityCNKey,o,cancel).ConfigureAwait(false)) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))  { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null;
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

                        oa = o.ToArray(); var _ = Deserialize(oa) as MSBuildItem; if(_ is null) { return false; }

                        this.Content = _.Content is null ? null : new String(_.Content);

                        if(_.Requirements is null) { this.Requirements = null; }
                        else
                        {
                            if(!_.Requirements.TryCloneDistinctByID(out var requirements)) { return false; }

                            this.Requirements = requirements;
                        }

                        this.Sequence     = _.Sequence is null ? null :
                            new (_.Sequence.ToDictionary(s=>s.Key,s=>(s.Value.Clone() as MSBuildItem)!));

                        await _.WipeDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);

                        if(!await UpdateSecureHashAsync(DataSecurityCNKey,this.Content.ToByteArrayFromUTF16String(),false,cancel).ConfigureAwait(false)) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))  { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null; this.EncryptedData = null;

                        return true;
                    }
                    finally
                    {
                        if(o is not null) { await ZeroMemoryAsync(o,default).ConfigureAwait(false); await o.DisposeAsync().ConfigureAwait(false); } if(oa is not null) { ZeroMemory(oa); }
                    }
                }
            }
            finally { this.ReleaseLocks(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> EncryptData(ManagementKey? managementkey , Boolean sign = true , CancellationToken cancel = default)
    {
        try
        {
            if(managementkey is null || this.Locked) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

            await this.AcquireLocksAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    using(var o = await EncryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false))
                    {
                        if(o is null) { return false; }

                        if(!await UpdateSecureHashAsync(DataSecurityEDKey,o,cancel).ConfigureAwait(false)) { return false; }
                        if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                        if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                        this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);
                    }

                    return true;
                }
                else
                {
                    Byte[]? i = null;

                    try
                    {
                        i = this.Content is not null || this.Requirements is not null || this.Sequence is not null ? this.Serialize() : null;

                        using(var o = (await EncryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream)
                        {
                            if(o is null) { return false; }

                            this.EncryptedData = o.ToArray();

                            if(!await UpdateSecureHashAsync(DataSecurityEDKey,this.EncryptedData, false, cancel).ConfigureAwait(false)) { return false; }
                            if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                            if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                            this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                            await this.WipeDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);
                        }

                        return true;
                    }
                    finally
                    {
                        if(i is not null) { ZeroMemory(i); }
                    }
                }
            }
            finally { this.ReleaseLocks(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(MSBuildItem? a , MSBuildItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(MSBuildItem? a , MSBuildItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as MSBuildItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as MSBuildItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as MSBuildItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as MSBuildItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as MSBuildItem); }

    ///<inheritdoc/>
    public Boolean Equals(MSBuildItem? other)
    {
        try
        {
            if(other is null) { return false; } if(ReferenceEquals(this, other)) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                var _req = this.Requirements; var __req = other.GetRequirements();
                String? _con = this.Content; String? __con = other.GetContent();
                var _seq = this.Sequence; var __seq = other.GetSequence();

                Boolean _ce = _con is not null; Boolean __ce = __con is not null; if(_ce != __ce) { return false; }
                Boolean _re = _req is not null; Boolean __re = __req is not null; if(_re != __re) { return false; }
                Boolean _se = _seq is not null; Boolean __se = __seq is not null; if(_se != __se) { return false; }

                if(_ce is false && _re is false && _se is false) { return true; }

                if(_ce && _con.AsSpan().SequenceEqual(__con.AsSpan()) is false) { return false; }

                if(_se && _seq!.SequenceEqual(__seq!) is false) { return false; }

                if(_re && _req!.SetEquals(__req!) is false) { return false; }

                return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="FromFile"]/*'/>*/
    public static MSBuildItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            return DataContractUtility.FromFile<MSBuildItem>(path,SerializationData.ForType(typeof(MSBuildItem)));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetContent"]/*'/>*/
    public String? GetContent()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.Content is null ? null : new(this.Content); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<DataContent?> GetDataContent(ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        Stream? d = null;

        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted && (managementkey is null || CheckSecrets(managementkey.Key,this.Secrets) is false)) { return null; }

                if(this.DataEncrypted)
                {
                    if(this.ContentStreamed)
                    {
                        var f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return null; }

                        d = await this.DecryptDataCore(null,f,managementkey,cancel).ConfigureAwait(false);
                    }
                    else if(Array.Empty<Byte>().AsSpan().SequenceEqual(this.EncryptedData.AsSpan()) is false)
                    {
                        d = await this.DecryptDataCore(this.EncryptedData,null,managementkey,cancel).ConfigureAwait(false);
                    }

                    if(d is not MemoryStream m) { return null; } var _ = Deserialize(m.ToArray()) as MSBuildItem; if(_ is null) { return null; }

                    try
                    {
                        HashSet<DataItem>? er = null;

                        if(_.Requirements is not null) { if(!_.Requirements.TryCloneDistinctByID(out er)) { return null; } }

                        return new()
                        {
                            MSBuild = new Dictionary<String,Object?>()
                            {
                                {"Content", _.Content is null ? null : new String(_.Content)},
                                {"Requirements", er},
                                {"Sequence", _.Sequence is null ? null : new SortedList<Int32,MSBuildItem>(_.Sequence.ToDictionary(s => s.Key, s => (s.Value.Clone() as MSBuildItem)!))}
                            }
                        };
                    }
                    finally { await _.WipeDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false); _ = null; }
                }

                if(this.Content is null && this.Requirements is null && this.Sequence is null) { return null; }

                HashSet<DataItem>? r = null;

                if(this.Requirements is not null) { if(!this.Requirements.TryCloneDistinctByID(out r)) { return null; } }

                return new()
                {
                    MSBuild = new Dictionary<String,Object?>()
                    {
                        {"Content", this.Content is null ? null : new String(this.Content)},
                        {"Requirements", r},
                        {"Sequence", this.Sequence is null ? null : new SortedList<Int32,MSBuildItem>(this.Sequence.ToDictionary(s => s.Key, s => (s.Value.Clone() as MSBuildItem)!))}
                    }
                };
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(d is not null) { await ZeroMemoryAsync(d,default).ConfigureAwait(false); await d.DisposeAsync().ConfigureAwait(false); } }
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
                ContentStreamed    = this.GetContentStreamed(),
                DataType           = this.GetDataType(),
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Size               = (this.ContentStreamed ? GetFileSize(this.FILE) : (this.Content?.Length * 2)).ToStringInvariant(),
                Tags               = this.GetTags(),
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
            var c = this.Content.ToByteArrayFromUTF16String() ?? Array.Empty<Byte>();
            var r = this.Requirements?.Order()
                .SelectMany(r => BitConverter.GetBytes(r.GetHashCode())).ToArray() ?? Array.Empty<Byte>();
            var s = this.Sequence?
                .SelectMany(entry => BitConverter.GetBytes(entry.Key)
                .Concat(BitConverter.GetBytes(entry.Value?.GetHashCode() ?? 0)))
                .ToArray() ?? Array.Empty<Byte>();

            return BitConverter.ToInt32(SHA512.HashData(c.Concat(r).Concat(s).ToArray()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            var requirements = this.Requirements?.Order()?.ToArray();
            var content = this.Content.ToByteArrayFromUTF16String() ?? Array.Empty<Byte>();

            var capacity = content.Length
                           + (requirements?.Length ?? 0) * sizeof(Int32)
                           + (this.Sequence?.Count ?? 0) * sizeof(Int32) * 2;

            using var buffer = new MemoryStream(capacity);

            buffer.Write(content.AsSpan());

            if(requirements is not null)
            {
                foreach(var r in requirements)
                {
                    cancel.ThrowIfCancellationRequested();

                    buffer.Write(BitConverter.GetBytes(await r.GetHashCodeAsync(cancel).ConfigureAwait(false)).AsSpan());
                }
            }

            if(this.Sequence is not null)
            {
                foreach(var s in this.Sequence)
                {
                    cancel.ThrowIfCancellationRequested();

                    buffer.Write(BitConverter.GetBytes(s.Key).AsSpan());

                    if(s.Value is null) { continue; }

                    buffer.Write(BitConverter.GetBytes(await s.Value.GetHashCodeAsync(cancel).ConfigureAwait(false)).AsSpan());
                }
            }

            buffer.Position = 0;

            return BitConverter.ToInt32(await SHA512.HashDataAsync(buffer,cancel).ConfigureAwait(false),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetRequirements"]/*'/>*/
    public HashSet<DataItem>? GetRequirements()
    {
        try
        {
            if(this.Requirements is null) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(!this.Requirements.TryCloneDistinctByID(out var o)) { return null; }

                if(Equals(o.Count,this.Requirements.Count) is false) { return null; } return o;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetRequirementsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetSequence"]/*'/>*/
    public SortedList<Int32,MSBuildItem>? GetSequence()
    {
        try
        {
            if(this.Sequence is null) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                SortedList<Int32,MSBuildItem> __ = new(this.Sequence.ToDictionary(_=>_.Key,_=>(_.Value.Clone() as MSBuildItem)!));

                if(!Equals(__.Count,this.Sequence.Count)) { return null; } return __;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetSequenceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static MSBuildItem IParsable<MSBuildItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="Parse"]/*'/>*/
    public static new MSBuildItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            return DataContractUtility.ParseBase64<MSBuildItem>(input,SerializationData.ForType(typeof(MSBuildItem)));
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(String? content , ManagementKey? managementkey = null)
    {
        try
        {
            if( content is null || (this.Locked && this.CheckManager(managementkey) is false )) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    if(String.IsNullOrEmpty(content))
                    {
                        this.EncryptedData = null; this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityEDKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityEDKey,managementkey);
                        }
                        else { return false; }
                    }

                    var db = this.DecryptDataBuffer(this.EncryptedData, managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as MSBuildItem; ZeroMemory(db); if(di is null)            { return false; }

                    di.Content = content;

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)           { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Requirements = null; this.Sequence = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if(String.IsNullOrEmpty(content))
                    {
                        this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityCNKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                        }
                        else { return false; }
                    }

                    this.Content = new(content); MN();

                    if(UpdateSecureHash(DataSecurityCNKey,this.Content.ToByteArrayFromUTF16String()))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> SetContentStreamed(Boolean streamed , ManagementKey? managementkey = null , CancellationToken cancel = default)
    {
        Byte[] GetContentBytes() => this.Content.ToByteArrayFromUTF16String();

        try
        {
            if(this.Locked || this.DataEncrypted) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(streamed is false)
                {
                    if(this.ContentStreamed is false) { return true; }

                    this.ContentStreamed = false; MN();

                    if(await UpdateSecureHashAsync(DataSecurityCNKey,GetContentBytes(),cancel:cancel).ConfigureAwait(false))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }

                var f = this.GetFILE();

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false)
                {
                    this.ContentStreamed = false; MN(); return await RefreshContentHashAsync().ConfigureAwait(false);
                }

                await using FileStream s = new(f,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.Asynchronous|FileOptions.SequentialScan});

                var h = await SHA512.HashDataAsync(s,cancel).ConfigureAwait(false);

                if(await UpdateSecureHashAsync(DataSecurityCNKey,h,true,cancel).ConfigureAwait(false))
                {
                    this.ContentStreamed = true; MN(); return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                }

                this.ContentStreamed = false; MN(); return await RefreshContentHashAsync().ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }

        async Task<Boolean> RefreshContentHashAsync()
        {
            if(await UpdateSecureHashAsync(DataSecurityCNKey,GetContentBytes(),cancel:cancel).ConfigureAwait(false)) { UpdateSecureSignature(DataSecurityCNKey,managementkey); }

            return false;
        }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetRequirements"]/*'/>*/
    public Boolean SetRequirements(IEnumerable<DataItem>? requirements , ManagementKey? managementkey = null)
    {
        try
        {
            if( (this.Locked && this.CheckManager(managementkey) is false )) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    HashSet<DataItem>? nc = null;

                    if(requirements is not null)
                    {
                        if(!requirements.TryDistinctByID(out nc)) { return false; } if(Equals(nc.Count,0)) { nc = null; }
                    }

                    var db = this.DecryptDataBuffer(this.EncryptedData, managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as MSBuildItem; ZeroMemory(db); if(di is null)            { return false; }

                    di.Requirements = nc;

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)           { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Requirements = null; this.Sequence = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if( requirements is null ) { return false; }

                    if(!requirements.TryDistinctByID(out var nc) || nc.Any(_ => ReferenceEquals(_,this))) { return false; }

                    if(!nc.TryCloneDistinctByID(out var __)) { return false; }

                    if(Equals(__.Count,0)) { this.Requirements = null; MN(); return true; }

                    this.Requirements = __; MN(); return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetRequirementsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetSequence"]/*'/>*/
    public Boolean SetSequence(IDictionary<Int32,MSBuildItem>? sequence , ManagementKey? managementkey = null)
    {
        try
        {
            if( (this.Locked && this.CheckManager(managementkey) is false )) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as MSBuildItem; ZeroMemory(db); if(di is null)           { return false; }

                    di.Sequence = sequence is null ? null : new SortedList<Int32,MSBuildItem>(sequence);

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)          { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Requirements = null; this.Sequence = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if( sequence is null ) { return false; }

                    SortedList<Int32,MSBuildItem> _ = new(sequence); if(_.Any(_ => ReferenceEquals(_.Value,this))) { return false; }

                    SortedList<Int32,MSBuildItem> __ = new(_.ToDictionary(_=>_.Key,_=>(_.Value.Clone() as MSBuildItem)!,null));

                    if(Equals(__.Count,0)) { this.Sequence = null; MN(); return true; }

                    if(Equals(_.Count,__.Count) is false) { return false; }

                    this.Sequence = __; MN(); return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetSequenceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out MSBuildItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(MSBuildItem)),out item);
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
                using var s = this.GetContentStream_NoSync();

                switch(field)
                {
                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false) { return false; }

                        if(this.ContentStreamed)
                        {
                            return await VerifyDataCore(field,managementkey,null,s,cancel).ConfigureAwait(false);
                        }

                        if(this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.Content is null && this.ContentStreamed is false) { return false; }

                        return this.ContentStreamed ?
                            await VerifyDataCore(field,managementkey,null,s,cancel).ConfigureAwait(false) :
                            await VerifyDataCore(field,managementkey,this.Content.ToByteArrayFromUTF16String(),null,cancel).ConfigureAwait(false);
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
    public override async Task<Boolean> WipeData(ManagementKey? managementkey = null, CancellationToken cancel = default)
    {
        try
        {
            return await this.WipeDataCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataCore(ManagementKey? managementkey, HashSet<Object> visited, CancellationToken cancel)
    {
        if(await base.WipeDataCore(managementkey,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

        await this.AcquireLocksAsync(cancel).ConfigureAwait(false);

        try
        {
            ZeroMemory(this.Content); this.Content = null;

            if(this.Requirements is not null)
            {
                foreach(var item in this.Requirements) { await item.WipeDataCore(managementkey, visited, cancel).ConfigureAwait(false); }

                this.Requirements.Clear(); this.Requirements = null;
            }

            if(this.Sequence is not null)
            {
                foreach(var item in this.Sequence.Values) { await item.WipeDataCore(managementkey, visited, cancel).ConfigureAwait(false); }

                this.Sequence.Clear(); this.Sequence = null;
            }

            if(this.ContentStreamed)
            {
                var f = this.GetFILE();

                if(File.Exists(f))
                {
                    using(var fs = new FileStream(f,new FileStreamOptions{Mode = FileMode.Open , Access = FileAccess.Write , Share = FileShare.None , Options = FileOptions.Asynchronous}))
                    {
                        await WipeMemoryAsync(fs,cancel).ConfigureAwait(false);
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
        finally { this.ReleaseLocks(); }
    }

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel)
    {
        try
        {
            if(await base.WipeDataInMemoryOnlyCore(managementkey,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            if(requirelocks) { await this.AcquireLocksAsync(cancel).ConfigureAwait(false); }

            try
            {
                ZeroMemory(this.Content); this.Content = null;

                if(this.Requirements is not null)
                {
                    foreach(var _ in this.Requirements) { await _.WipeDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); }

                    this.Requirements.Clear(); this.Requirements = null;
                }

                if(this.Sequence is not null)
                {
                    foreach(var _ in this.Sequence.Values) { await _.WipeDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); }

                    this.Sequence.Clear(); this.Sequence = null;
                }

                return true;
            }
            finally { if(requirelocks) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}