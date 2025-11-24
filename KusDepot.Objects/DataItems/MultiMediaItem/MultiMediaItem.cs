namespace KusDepot;

/**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.MultiMediaItem")]
[DataContract(Name = "MultiMediaItem" , Namespace = "KusDepot")]
public sealed class MultiMediaItem : DataItem , IComparable<MultiMediaItem> , IEquatable<MultiMediaItem> , IExtensibleDataObject , IParsable<MultiMediaItem>
{
    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Artists"]/*'/>*/
    [DataMember(Name = "Artists" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private HashSet<String>? Artists;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Bitrate"]/*'/>*/
    [DataMember(Name = "Bitrate" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    private Double? Bitrate;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/content[@name="Content"]/*'/>*/
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    private Byte[]? Content;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Duration"]/*'/>*/
    [DataMember(Name = "Duration" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    private Decimal? Duration;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Framerate"]/*'/>*/
    [DataMember(Name = "Framerate" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    private Single? Framerate;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Language"]/*'/>*/
    [DataMember(Name = "Language" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    private String? Language;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Source"]/*'/>*/
    [NonSerialized]
    [IgnoreDataMember]
    private Stream? Source;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Title"]/*'/>*/
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    private String? Title;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="UseSourceStream"]/*'/>*/
    [DataMember(Name = "UseSourceStream" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    private Boolean UseSourceStream;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Year"]/*'/>*/
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    private Int32? Year;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public MultiMediaItem() : this(null,null,null,null,null,null,null,null,null,null) {}

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/constructor[@name="Constructor"]/*'/>*/
    public MultiMediaItem(Byte[]? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? artists = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? title = null , String? type = null , Int32? year = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetArtists(artists); this.SetTitle(title); this.SetType(type); this.SetYear(year); this.AddNotes(notes); this.AddTags(tags);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> CheckDataHash(CancellationToken cancel = default)
    {
        try
        {
            if(this.UseSourceStream) { return true; }

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
    public override Int32 CompareTo(IDataItem? other) { return other is MultiMediaItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is MultiMediaItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is MultiMediaItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is MultiMediaItem m ? this.CompareTo(m) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(MultiMediaItem? other)
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

                        o = (await DecryptDataCore(i,null,managementkey,cancel).ConfigureAwait(false)) as MemoryStream; if(o is null) { return false; }

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

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(MultiMediaItem? a , MultiMediaItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(MultiMediaItem? a , MultiMediaItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as MultiMediaItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as MultiMediaItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as MultiMediaItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as MultiMediaItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as MultiMediaItem); }

    ///<inheritdoc/>
    public Boolean Equals(MultiMediaItem? other)
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

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="FromFile"]/*'/>*/
    public static MultiMediaItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            using FileStream _0 = new(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read});

            DataContractSerializer _1 = new(typeof(MultiMediaItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            using XmlDictionaryReader _2 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max);

            return _1.ReadObject(_2) as MultiMediaItem;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetArtists"]/*'/>*/
    public HashSet<String>? GetArtists()
    {
        try
        {
            if(this.Artists is null) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                HashSet<String> _ = new(); _.UnionWith(this.Artists.Select(_=>new String(_))); return _;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetArtistsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetBitrate"]/*'/>*/
    public Double? GetBitrate()
    {
        try { return this.Bitrate; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetBitrateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetContent"]/*'/>*/
    public Byte[]? GetContent()
    {
        try
        {
            if(this.Content is null || this.DataMasked is true) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                return this.Content.Clone() as Byte[];
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Stream? GetContentStream()
    {
        try
        {
            if(this.DataMasked is true) { return null; }

            if(this.UseSourceStream is true) { return this.Source; }

            if(this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE)) { return null; }

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

    ///<inheritdoc/>
    protected override Stream? GetContentStream_NoSync()
    {
        try
        {
            if(this.DataMasked is true) { return null; }

            if(this.UseSourceStream is true) { return this.Source; }

            if(this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE)) { return null; }

            if(File.Exists(this.FILE)) { try { return File.OpenRead(this.FILE); } catch {} }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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

                    if(d is not MemoryStream m) { return null; }

                    return new() { MultiMedia = new Dictionary<String,Object?>(){{"Content",m.ToArray()},{"Source",this.Source}} };
                }

                return this.Content is not null || this.Source is not null ?
                    new() { MultiMedia = new Dictionary<String,Object?>(){{"Content",this.Content?.Clone() as Byte[]},{"Source",this.Source}} } : null;
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
                Artist             = this.GetArtists()?.FirstOrDefault(),
                BornOn             = this.GetBornOn()?.ToString("O"),
                ContentStreamed    = this.GetContentStreamed(),
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                LiveStream         = this.GetUseSourceStream(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Size               = this.Content?.LongLength.ToStringInvariant(),
                Tags               = this.GetTags(),
                Title              = this.GetTitle(),
                Type               = this.GetType(),
                Version            = this.GetVersion()?.ToString(),
                Year               = this.GetYear()?.ToStringInvariant()
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetDuration"]/*'/>*/
    public Decimal? GetDuration()
    {
        try { return this.Duration; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetDurationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetFramerate"]/*'/>*/
    public Single? GetFramerate()
    {
        try { return this.Framerate; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetFramerateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetLanguage"]/*'/>*/
    public String? GetLanguage()
    {
        try { return this.Language is null ? null : new(this.Language); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetLanguageFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetSource"]/*'/>*/
    public Stream? GetSource()
    {
        try { return this.DataMasked is true ? null : this.Source; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetSourceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetTitle"]/*'/>*/
    public String? GetTitle()
    {
        try { return this.Title is null ? null : new(this.Title); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetTitleFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetUseSourceStream"]/*'/>*/
    public Boolean GetUseSourceStream()
    {
        try { return this.UseSourceStream; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetUseSourceStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetYear"]/*'/>*/
    public Int32? GetYear()
    {
        try { return this.Year; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetYearFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static MultiMediaItem IParsable<MultiMediaItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="Parse"]/*'/>*/
    public static new MultiMediaItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(MultiMediaItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            MultiMediaItem? _2 = _1.ReadObject(_0) as MultiMediaItem; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetArtists"]/*'/>*/
    public Boolean SetArtists(IEnumerable<String>? artists)
    {
        try
        {
            if( artists is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                HashSet<String> _ = new(artists.Select(_=>new String(_)));

                if(Equals(_.Count,0)) { this.Artists = null; MN(); return true; }

                this.Artists = _; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetArtistsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetBitrate"]/*'/>*/
    public Boolean SetBitrate(Double? bitrate)
    {
        try
        {
            if( bitrate is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(Double.Equals(bitrate,0)) { this.Bitrate = null; MN(); return true; }

                this.Bitrate = bitrate; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetBitrateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetContent"]/*'/>*/
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
                        this.EncryptedData = null; this.Content = null; MN();

                        if(UpdateSecureHash(DataSecurityEDKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityEDKey,managementkey);
                        }
                        else { return false; }
                    }

                    var eb = EncryptDataBuffer(content,managementkey); if(eb is null)                       { return false; }

                    this.EncryptedData = eb; this.Content = null; MN();

                    if(!UpdateSecureHash(DataSecurityEDKey,this.EncryptedData))                             { return false; }
                    if(managementkey.CanSign) { if(!UpdateSecureSignature(DataSecurityEDKey,managementkey)) { return false; } }
                    if(!UpdateSecureHash(DataSecurityCNKey,Array.Empty<Byte>()))                            { return false; }

                    this.DataEncryptInfo = new(managementkey); return true;
                }
                else
                {
                    if(Equals(content.Length,0))
                    {
                        this.Content = null;

                        if(UpdateSecureHash(DataSecurityCNKey,null))
                        {
                            return managementkey is null ? true : UpdateSecureSignature(DataSecurityCNKey,managementkey);
                        }
                        else { return false; }
                    }

                    this.Content = content.Clone() as Byte[];

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

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetDuration"]/*'/>*/
    public Boolean SetDuration(Decimal? duration)
    {
        try
        {
            if( duration is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(Decimal.Equals(duration,0)) { this.Duration = null; MN(); return true; }

                this.Duration = duration; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetDurationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetFramerate"]/*'/>*/
    public Boolean SetFramerate(Single? framerate)
    {
        try
        {
            if( framerate is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(Single.Equals(framerate,0)) { this.Framerate = null; MN(); return true; }

                this.Framerate = framerate; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetFramerateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetLanguage"]/*'/>*/
    public Boolean SetLanguage(String? language)
    {
        try
        {
            if( language is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(String.IsNullOrEmpty(language)) { this.Language = null; MN(); return true; }

                this.Language = new(language); MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetLanguageFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetSource"]/*'/>*/
    public Boolean SetSource(Stream? source)
    {
        try
        {
            if(this.Locked) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                this.Source = source; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetSourceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetTitle"]/*'/>*/
    public Boolean SetTitle(String? title)
    {
        try
        {
            if( title is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(String.IsNullOrEmpty(title)) { this.Title = null; MN(); return true; }

                this.Title = new(title); MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTitleFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetUseSourceStream"]/*'/>*/
    public Boolean SetUseSourceStream(Boolean use)
    {
        try
        {
            if(this.Locked) { return false; }

            this.UseSourceStream = use; MN(); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetUseSourceStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetYear"]/*'/>*/
    public Boolean SetYear(Int32? year)
    {
        try
        {
            if( year is null || this.Locked ) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(Equals(year,0)) { this.Year = null; MN(); return true; }

                this.Year = year; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetYearFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out MultiMediaItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(MultiMediaItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            MultiMediaItem? _2 = _1.ReadObject(_0) as MultiMediaItem; if( _2 is not null ) { item = _2; return true; }

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

                MN(); return true;
            }
            finally { this.Sync.Data.Release(); this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}