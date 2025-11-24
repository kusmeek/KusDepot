namespace KusDepot;

/**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.GuidReferenceItem")]
[DataContract(Name = "GuidReferenceItem" , Namespace = "KusDepot")]
public sealed class GuidReferenceItem : DataItem , IComparable<GuidReferenceItem> , IEquatable<GuidReferenceItem> , IExtensibleDataObject , IParsable<GuidReferenceItem>
{
    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/content[@name="Content"]/*'/>*/
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private Guid? Content;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/field[@name="Next"]/*'/>*/
    [DataMember(Name = "Next" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    private GuidReferenceItem? Next;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/field[@name="Previous"]/*'/>*/
    [DataMember(Name = "Previous" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    private GuidReferenceItem? Previous;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/field[@name="UndirectedLinks"]/*'/>*/
    [DataMember(Name = "UndirectedLinks" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    private HashSet<GuidReferenceItem>? UndirectedLinks;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public GuidReferenceItem() : this(null,null,null,null,null,null) {}

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/constructor[@name="Constructor"]/*'/>*/
    public GuidReferenceItem(Guid? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try
        {
            this.Sync = null!; this.Type = DataType.GUID; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.AddNotes(notes); this.AddTags(tags);
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

                if(this.Content is null) { return false; }

                return await CheckDataHashCore(this.Content.Value.ToByteArray(),null,cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is GuidReferenceItem g ? this.CompareTo(g) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is GuidReferenceItem g ? this.CompareTo(g) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is GuidReferenceItem g ? this.CompareTo(g) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is GuidReferenceItem g ? this.CompareTo(g) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(GuidReferenceItem? other)
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

                        oa = o.ToArray(); var _ = Deserialize(oa) as GuidReferenceItem; if(_ is null)         { return false; }

                        this.Content = _.Content.HasValue ? Guid.Parse(_.Content.Value.ToString()) : null;
                        this.Next = _.Next?.Clone() as GuidReferenceItem;
                        this.Previous = _.Previous?.Clone() as GuidReferenceItem;
                        this.UndirectedLinks = _.UndirectedLinks?.Select(l => (l.Clone() as GuidReferenceItem)!).ToHashSet();
                        await _.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false);

                        if(!UpdateSecureHash(DataSecurityCNKey,
                            this.Content.HasValue ? this.Content.Value.ToByteArray() : null )) { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))            { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>()))           { return false; }

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
                        i = this.Content is not null || this.Next is not null ||
                            this.Previous is not null || this.UndirectedLinks is not null ? this.Serialize() : null;

                        using(var o = await EncryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false) as MemoryStream)
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

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(GuidReferenceItem? a , GuidReferenceItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(GuidReferenceItem? a , GuidReferenceItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as GuidReferenceItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as GuidReferenceItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as GuidReferenceItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as GuidReferenceItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as GuidReferenceItem); }

    ///<inheritdoc/>
    public Boolean Equals(GuidReferenceItem? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                var _ = other.GetContent(); if(this.Content is null && _ is null) { return false; } return Guid.Equals(this.Content,_);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="FromFile"]/*'/>*/
    public static GuidReferenceItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(GuidReferenceItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return _1.ReadObject(_2) as GuidReferenceItem;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetContent"]/*'/>*/
    public Guid? GetContent()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.DataMasked is true ? null : this.Content; }

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

                    if(d is not MemoryStream m) { return null; } var _ = Deserialize(m.ToArray()) as GuidReferenceItem; if(_ is null) { return null; }

                    try
                    {
                        return new() { GuidReference = new Dictionary<String,Object?>()
                        {
                            {"Content",         _.Content},
                            {"Next",            _.Next?.Clone() as GuidReferenceItem},
                            {"Previous",        _.Previous?.Clone() as GuidReferenceItem},
                            {"UndirectedLinks", _.UndirectedLinks?.Select(l => (l.Clone() as GuidReferenceItem)!).ToHashSet()}
                        }};
                    }
                    finally { await _.ZeroDataInMemoryOnly(managementkey,false,cancel).ConfigureAwait(false); _ = null; }
                }

                if(this.Content is null && this.Next is null && this.Previous is null && this.UndirectedLinks is null) { return null; }

                return new() { GuidReference = new Dictionary<String,Object?>()
                {
                    {"Content",         this.Content},
                    {"Next",            this.Next?.Clone() as GuidReferenceItem},
                    {"Previous",        this.Previous?.Clone() as GuidReferenceItem},
                    {"UndirectedLinks", this.UndirectedLinks?.Select(l => (l.Clone() as GuidReferenceItem)!).ToHashSet()}
                }};
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
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
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
            var i = this.GetID()!.Value.ToByteArray();
            var c = this.Content?.ToByteArray() ?? Array.Empty<Byte>();
            var n = this.Next?.GetID()!.Value.ToByteArray() ?? Array.Empty<Byte>();
            var p = this.Previous?.GetID()!.Value.ToByteArray() ?? Array.Empty<Byte>();

            return BitConverter.ToInt32(SHA512.HashData(i.Concat(c).Concat(n).Concat(p).ToArray()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetNext"]/*'/>*/
    public GuidReferenceItem? GetNext()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.DataMasked is false ? this.Next?.Clone() as GuidReferenceItem : null; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetNextFail,MyTypeName,MyID); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetPrevious"]/*'/>*/
    public GuidReferenceItem? GetPrevious()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.DataMasked is false ? this.Previous?.Clone() as GuidReferenceItem : null; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetPreviousFail,MyTypeName,MyID); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetUndirectedLinks"]/*'/>*/
    public HashSet<GuidReferenceItem>? GetUndirectedLinks()
    {
        try
        {
            if(this.UndirectedLinks is null || this.DataMasked is true) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                HashSet<GuidReferenceItem> _ = new(this.UndirectedLinks.Select(_=>(_.Clone() as GuidReferenceItem)!));

                if(!Equals(_.Count,this.UndirectedLinks.Count)) { return null; } return _;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetUndirectedLinksFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static GuidReferenceItem IParsable<GuidReferenceItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="Parse"]/*'/>*/
    public static new GuidReferenceItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(GuidReferenceItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            GuidReferenceItem? _2 = _1.ReadObject(_0) as GuidReferenceItem; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(Guid? content , ManagementKey? managementkey = null)
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

                    if(Guid.Equals(content,Guid.Empty))
                    {
                        this.EncryptedData = null; this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityEDKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityEDKey,managementkey);
                        }
                        else { return false; }
                    }

                    var db = this.DecryptDataBuffer(this.EncryptedData, managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)      { return false; }

                    di.Content = content;

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)           { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if(Guid.Equals(content,Guid.Empty))
                    {
                        this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityCNKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                        }
                        else { return false; }
                    }

                    this.Content = content; MN();

                    if(UpdateSecureHash(DataSecurityCNKey,this.Content.Value.ToByteArray()))
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

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetNext"]/*'/>*/
    public Boolean SetNext(GuidReferenceItem? next , ManagementKey? managementkey = null)
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

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)     { return false; }

                    di.Next = next;

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)          { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey);

                    return true;
                }
                else
                {
                    if(next is null) { this.Next = null; MN(); return true; }

                    this.Next = next.Clone() as GuidReferenceItem; MN(); return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetNextFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetPrevious"]/*'/>*/
    public Boolean SetPrevious(GuidReferenceItem? previous , ManagementKey? managementkey = null)
    {
        try
        {
            if( this.Locked && this.CheckManager(managementkey) is false ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)     { return false; }

                    di.Previous = previous;

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)          { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey);

                    return true;
                }
                else
                {
                    if(previous is null) { this.Previous = null; MN(); return true; }

                    this.Previous = previous.Clone() as GuidReferenceItem; MN(); return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetPreviousFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetUndirectedLinks"]/*'/>*/
    public Boolean SetUndirectedLinks(IEnumerable<GuidReferenceItem>? undirectedlinks , ManagementKey? managementkey = null)
    {
        try
        {
            if( this.Locked && this.CheckManager(managementkey) is false ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(managementkey is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,managementkey); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)     { return false; }

                    di.UndirectedLinks = undirectedlinks is null ? null : new(undirectedlinks);

                    var eb = EncryptDataBuffer(di.Serialize(),managementkey); if(eb is null)          { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; this.UndirectedLinks = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey);

                    return true;
                }
                else
                {
                    if(undirectedlinks is null) { this.UndirectedLinks = null; MN(); return true; }

                    HashSet<GuidReferenceItem> _ = new(undirectedlinks.Select(_=>(_.Clone() as GuidReferenceItem)!));

                    if(Equals(_.Count,0)) { this.UndirectedLinks = null; MN(); return true; }

                    this.UndirectedLinks = _; MN(); return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetUndirectedLinksFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out GuidReferenceItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(GuidReferenceItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            GuidReferenceItem? _2 = _1.ReadObject(_0) as GuidReferenceItem; if( _2 is not null ) { item = _2; return true; }

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
                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false || this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.Content is null) { return false; }

                        return await VerifyDataCore(field,managementkey,this.Content.Value.ToByteArray(),null,cancel).ConfigureAwait(false);
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
            return await this.ZeroDataCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    internal override async Task<Boolean> ZeroDataCore(ManagementKey? managementkey, HashSet<Object> visited, CancellationToken cancel = default)
    {
        if(await base.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

        await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            this.Content = null;

            if(this.Next is not null)
            {
                await this.Next.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false); this.Next = null;
            }

            if(this.Previous is not null)
            {
                await this.Previous.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false); this.Previous = null;
            }

            if(this.UndirectedLinks is not null)
            {
                foreach(var _ in this.UndirectedLinks) { await _.ZeroDataCore(managementkey,visited,cancel).ConfigureAwait(false); }

                this.UndirectedLinks.Clear(); this.UndirectedLinks = null;
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

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="ZeroDataInMemoryOnly"]/*'/>*/
    private async Task<Boolean> ZeroDataInMemoryOnly(ManagementKey? managementkey , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            return await this.ZeroDataInMemoryOnlyCore(managementkey,new HashSet<Object>(ReferenceEqualityComparer.Instance),requirelocks,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataInMemoryOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    internal override async Task<Boolean> ZeroDataInMemoryOnlyCore(ManagementKey? managementkey , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            if(await base.ZeroDataInMemoryOnlyCore(managementkey,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            if(requirelocks) { await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false); }

            try
            {
                this.Content = null;

                if(this.Next is not null)
                {
                    await this.Next.ZeroDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); this.Next = null;
                }

                if(this.Previous is not null)
                {
                    await this.Previous.ZeroDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); this.Previous = null;
                }

                if(this.UndirectedLinks is not null)
                {
                    foreach(var _ in this.UndirectedLinks) { await _.ZeroDataInMemoryOnlyCore(managementkey,visited,true,cancel).ConfigureAwait(false); }

                    this.UndirectedLinks.Clear(); this.UndirectedLinks = null;
                }

                return true;
            }
            finally { if(requirelocks) { this.Sync.Data.Release(); this.Sync.Meta.Release(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}