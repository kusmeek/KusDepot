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
            this.Sync = null!; this.Type = DataType.MSB; this.Initialize();

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

                        oa = o.ToArray(); var _ = Deserialize(oa) as MSBuildItem; if(_ is null)               { return false; }

                        this.Content      = _.Content is null ? null : new String(_.Content);
                        this.Requirements = _.Requirements?.Select(r => r.Clone()!).ToHashSet();
                        this.Sequence     = _.Sequence is null ? null :
                            new (_.Sequence.ToDictionary(s=>s.Key,s=>(s.Value.Clone() as MSBuildItem)!));

                        await _.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);

                        if(!UpdateSecureHash(DataSecurityCNKey,this.Content.ToByteArrayFromUTF16String())) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))                        { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>()))                       { return false; }

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
            if(managementkey is null || this.Locked) { return false; } var c = DeserializeCertificate(managementkey.Key); if(c is null) { return false; }

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
                        i = this.Content is not null || this.Requirements is not null || this.Sequence is not null ? this.Serialize() : null;

                        using(var o = (await EncryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream)
                        {
                            if(o is null) { return false; }

                            this.EncryptedData = o.ToArray();

                            if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))            { return false; }
                            if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                            if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                            this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                            await this.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);
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

                if(_ce is false && _re is false && _se is false) { return false; }

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

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return _1.ReadObject(_2) as MSBuildItem;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetContent"]/*'/>*/
    public String? GetContent()
    {
        try
        {
            if(this.Content is null || this.DataMasked is true) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return new(this.Content); }

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
                if(this.DataMasked is false) { goto ReturnContent; }

                if(managementkey is null || CheckSecrets(managementkey.Key,this.Secrets) is false) { return null; }

                ReturnContent:

                if(this.DataEncrypted && Array.Empty<Byte>().SequenceEqual(this.EncryptedData) is false)
                {
                    d = await this.DecryptDataCore(this.EncryptedData,null,managementkey,cancel).ConfigureAwait(false);

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
                    finally { await _.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false); _ = null; }
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
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Size               = (this.Content?.Length * 2)?.ToStringInvariant(),
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
            var i = this.GetID()!.Value.ToByteArray();
            var c = this.Content.ToByteArrayFromUTF16String();
            var r = this.Requirements?.Order()
                .SelectMany(r => BitConverter.GetBytes(r.GetHashCode())).ToArray() ?? Array.Empty<Byte>();
            var s = this.Sequence?.Order()
                .SelectMany(s => BitConverter.GetBytes(s.Key)
                .Concat(BitConverter.GetBytes(s.Value.GetHashCode()))).ToArray() ?? Array.Empty<Byte>();

            return BitConverter.ToInt32(SHA512.HashData(i.Concat(c).Concat(r).Concat(s).ToArray()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetRequirements"]/*'/>*/
    public HashSet<DataItem>? GetRequirements()
    {
        try
        {
            if(this.Requirements is null || this.DataMasked is true) { return null; }

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
            if(this.Sequence is null || this.DataMasked is true) { return null; }

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
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            MSBuildItem? _2 = _1.ReadObject(_0) as MSBuildItem; if( _2 is not null ) { return _2; }

            throw new FormatException();
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
    public override Boolean SetContentStreamed(Boolean streamed , ManagementKey? managementkey = null)
    {
        try
        {
            if(this.Locked || this.DataEncrypted) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(streamed is false)
                {
                    if(this.ContentStreamed is false) { return true; }

                    this.ContentStreamed = false; MN();

                    if(UpdateSecureHash(DataSecurityCNKey,this.Content.ToByteArrayFromUTF16String()))
                    {
                        return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                    }
                    else { return false; }
                }

                if(String.IsNullOrEmpty(this.GetFILE()) is true || File.Exists(this.GetFILE()) is false)
                {
                    this.ContentStreamed = false; MN(); return RefreshContentHash();
                }

                using Stream s = File.OpenRead(this.GetFILE()!);

                if(UpdateSecureHash(DataSecurityCNKey,SHA512.HashData(s),true))
                {
                    this.ContentStreamed = true; MN(); return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                }

                else { this.ContentStreamed = false; MN(); return RefreshContentHash(); }

                Boolean RefreshContentHash()
                {
                    if(UpdateSecureHash(DataSecurityCNKey,this.Content.ToByteArrayFromUTF16String())) { UpdateSecureSignature(DataSecurityCNKey,managementkey); }

                    return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            MSBuildItem? _2 = _1.ReadObject(_0) as MSBuildItem; if( _2 is not null ) { item = _2; return true; }

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
                using var s = this.GetContentStream_NoSync();

                switch(field)
                {
                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false || this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.Content is null && this.ContentStreamed is false) { return false; }

                        return await VerifyDataCore(field,managementkey,this.Content.ToByteArrayFromUTF16String(),s,cancel).ConfigureAwait(false);
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
    public override async Task<Boolean> ZeroData(ManagementKey? managementkey = null, CancellationToken cancel = default)
    {
        try
        {
            return await this.ZeroDataCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    internal override async Task<Boolean> ZeroDataCore(ManagementKey? managementkey, HashSet<Object> visited, CancellationToken cancel)
    {
        if(await base.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

        await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            this.Content = null;

            if(this.Requirements is not null)
            {
                foreach(var item in this.Requirements) { await item.ZeroDataCore(managementkey, visited, cancel).ConfigureAwait(false); }

                this.Requirements.Clear(); this.Requirements = null;
            }

            if(this.Sequence is not null)
            {
                foreach(var item in this.Sequence.Values) { await item.ZeroDataCore(managementkey, visited, cancel).ConfigureAwait(false); }

                this.Sequence.Clear(); this.Sequence = null;
            }

            if(this.ContentStreamed)
            {
                var f = this.GetFILE();

                if(File.Exists(f))
                {
                    using(var fs = new FileStream(f,FileMode.Open,FileAccess.Write,FileShare.None))
                    {
                        await ZeroMemoryAsync(fs, cancel).ConfigureAwait(false);
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

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="ZeroDataInMemoryOnly"]/*'/>*/
    private async Task<Boolean> ZeroDataInMemoryOnly(ManagementKey? managementkey , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            return await this.ZeroDataInMemoryOnlyCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),requirelocks,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataInMemoryOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    internal override async Task<Boolean> ZeroDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel)
    {
        try
        {
            if(await base.ZeroDataInMemoryOnlyCore(managementkey,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            if(requirelocks) { await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false); }

            try
            {
                this.Content = null;

                if(this.Requirements is not null)
                {
                    foreach(var _ in this.Requirements) { await _.ZeroDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); }

                    this.Requirements.Clear(); this.Requirements = null;
                }

                if(this.Sequence is not null)
                {
                    foreach(var _ in this.Sequence.Values) { await _.ZeroDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); }

                    this.Sequence.Clear(); this.Sequence = null;
                }

                return true;
            }
            finally { if(requirelocks) { this.Sync.Data.Release(); this.Sync.Meta.Release(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}