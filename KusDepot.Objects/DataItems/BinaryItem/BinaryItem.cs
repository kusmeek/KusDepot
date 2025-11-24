namespace KusDepot;

/**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.BinaryItem")]
[DataContract(Name = "BinaryItem" , Namespace = "KusDepot")]
public sealed class BinaryItem : DataItem , IComparable<BinaryItem> , IEquatable<BinaryItem> , IExtensibleDataObject , IParsable<BinaryItem>
{
    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/field[@name="Commands"]/*'/>*/
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private HashSet<Tuple<String,String,String>>? Commands;

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/content[@name="Content"]/*'/>*/
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    private Byte[]? Content;

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/field[@name="Services"]/*'/>*/
    [DataMember(Name = "Services" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    private HashSet<Tuple<String,String>>? Services;

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public BinaryItem() : this(null,null,null,null,null,null,null,null) {}

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryItem(Byte[]? content = null , IEnumerable<String>? dllpaths = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetType(type); this.AddNotes(notes); this.AddTags(tags);
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

                return await CheckDataHashCore(this.Content,null,cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is BinaryItem b ? this.CompareTo(b) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is BinaryItem b ? this.CompareTo(b) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is BinaryItem b ? this.CompareTo(b) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is BinaryItem b ? this.CompareTo(b) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(BinaryItem? other)
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
                    MemoryStream? o = null;

                    try
                    {
                        var i = this.DataEncrypted && this.EncryptedData is not null ? this.EncryptedData : null;

                        o = await DecryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false) as MemoryStream; if(o is null) { return false; }

                        this.Content = o.ToArray();

                        if(!UpdateSecureHash(DataSecurityCNKey,this.Content))        { return false; }
                        if(!UpdateSecureSignature(DataSecurityCNKey,managementkey))  { return false; }
                        if(!UpdateSecureHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                        this.DataEncrypted = false; this.DataEncryptInfo = null; this.EncryptedData = null;

                        return true;
                    }
                    finally
                    {
                        if(o is not null) { await ZeroMemoryAsync(o,default).ConfigureAwait(false); await o.DisposeAsync().ConfigureAwait(false); }
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
                        i = this.Content;

                        using(var o = (await EncryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream)
                        {
                            if(o is null) { return false; }

                            this.EncryptedData = o.ToArray();

                            if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))            { return false; }
                            if(sign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                            if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))           { return false; }

                            this.DataEncrypted = true; this.DataEncryptInfo = new(managementkey);

                            ZeroMemory(this.Content); this.Content = null;
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

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(BinaryItem? a , BinaryItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(BinaryItem? a , BinaryItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as BinaryItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as BinaryItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as BinaryItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as BinaryItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as BinaryItem); }

    ///<inheritdoc/>
    public Boolean Equals(BinaryItem? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { return false; }

            try
            {
                Byte[]? _ = other.GetContent();

                if(this.Content is null && _ is null) { return false; }

                if(this.Content is null && _ is not null) { return false; }

                if(this.Content is not null && _ is null) { return false; }

                return this.Content!.AsSpan().SequenceEqual(_.AsSpan());
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="FromFile"]/*'/>*/
    public static BinaryItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return  null; }

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(BinaryItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return _1.ReadObject(_2) as BinaryItem;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="GetContent"]/*'/>*/
    public Byte[]? GetContent()
    {
        try
        {
            if(this.Content is null || this.DataMasked is true) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.Content.Clone() as Byte[]; }

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

                    if(d is not MemoryStream m) { return null; } return new () { Binary = m.ToArray() };
                }

                return this.Content is not null ? new() { Binary = this.Content.Clone() as Byte[] } : null;
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
                Commands           = this.Commands?.ToHashSet(),
                ContentStreamed    = this.GetContentStreamed(),
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                Services           = this.Services?.ToHashSet(),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Size               = this.Content?.LongLength.ToStringInvariant(),
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
            return BitConverter.ToInt32(SHA512.HashData(this.GetID()!.Value.ToByteArray().Concat(this.Content ?? Array.Empty<Byte>()).ToArray()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static BinaryItem IParsable<BinaryItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="Parse"]/*'/>*/
    public static new BinaryItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(BinaryItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            BinaryItem? _2 = _1.ReadObject(_0) as BinaryItem; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(Byte[]? content , ManagementKey? managementkey = null)
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

                    if(Equals(content.Length,0))
                    {
                        this.EncryptedData = null; this.Content = null; this.Commands = null; this.Services = null; MN();

                        if(UpdateSecureHash(DataSecurityEDKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityEDKey,managementkey);
                        }
                        else { return false; }
                    }

                    var eb = EncryptDataBuffer(content,managementkey); if(eb is null)      { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Commands = null; this.Services = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if(Equals(content.Length,0))
                    {
                        this.Content = null; this.Commands = null; this.Services = null; MN();

                        if(UpdateSecureHash(DataSecurityCNKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                        }
                        else { return false; }
                    }

                    this.Content = content.Clone() as Byte[]; this.Commands = GetAssemblyCommands(this.Content); this.Services = GetAssemblyServices(this.Content); MN();

                    if(UpdateSecureHash(DataSecurityCNKey,this.Content))
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

                    if(UpdateSecureHash(DataSecurityCNKey,this.Content))
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
                    if(UpdateSecureHash(DataSecurityCNKey,this.Content)) { UpdateSecureSignature(DataSecurityCNKey,managementkey); }

                    return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out BinaryItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(BinaryItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            BinaryItem? _2 = _1.ReadObject(_0) as BinaryItem; if( _2 is not null ) { item = _2; return true; }

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

                        return await VerifyDataCore(field,managementkey,this.Content,s,cancel).ConfigureAwait(false);
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
            if(this.Locked && this.CheckManager(managementkey) is false) { return false; }

            await this.Sync.Meta.WaitAsync(cancel).ConfigureAwait(false); await this.Sync.Data.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.Content is not null)
                {
                    ZeroMemory(this.Content); this.Content = null;
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

                this.Commands = null; this.Services = null;

                MN(); return true;
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}