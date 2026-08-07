namespace KusDepot;

/**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.MultiMediaItem")]
[DataContract(Name = "MultiMediaItem" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed partial class MultiMediaItem : DataItem , IComparable<MultiMediaItem> , IEquatable<MultiMediaItem> , IExtensibleDataObject , IParsable<MultiMediaItem>
{
    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Artists"]/*'/>*/
    [JsonPropertyName("Artists")] [JsonInclude]
    [DataMember(Name = "Artists" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal HashSet<String>? Artists;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Bitrate"]/*'/>*/
    [JsonPropertyName("Bitrate")] [JsonInclude]
    [DataMember(Name = "Bitrate" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    internal Double? Bitrate;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/content[@name="Content"]/*'/>*/
    [JsonPropertyName("Content")] [JsonInclude] [Description(MultiMediaContentDescription)]
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    internal Byte[]? Content;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Duration"]/*'/>*/
    [JsonPropertyName("Duration")] [JsonInclude]
    [DataMember(Name = "Duration" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    internal Decimal? Duration;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Framerate"]/*'/>*/
    [JsonPropertyName("Framerate")] [JsonInclude]
    [DataMember(Name = "Framerate" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    internal Single? Framerate;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Language"]/*'/>*/
    [JsonPropertyName("Language")] [JsonInclude]
    [DataMember(Name = "Language" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    internal String? Language;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Title"]/*'/>*/
    [JsonPropertyName("Title")] [JsonInclude]
    [DataMember(Name = "Title" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    internal String? Title;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/field[@name="Year"]/*'/>*/
    [JsonPropertyName("Year")] [JsonInclude]
    [DataMember(Name = "Year" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    internal Int32? Year;

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public MultiMediaItem() : this(null,null,null,null,null,null,null,null,null,null) {}

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/constructor[@name="Constructor"]/*'/>*/
    public MultiMediaItem(Byte[]? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? artists = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? title = null , String? type = null , Int32? year = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetArtists(artists); this.SetTitle(title); this.SetDataType(type); this.SetYear(year); this.AddNotes(notes); this.AddTags(tags);
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
                        var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = OpenSequentialRead(f);

                        return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                    }

                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.ContentStreamed)
                {
                    var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = OpenSequentialRead(f);

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

    ///<inheritdoc/>
    public override async Task<Boolean> DecryptData(DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        Boolean lk = false;

        try
        {
            if(security is null || this.DataEncrypted is false || this.Locked) { return false; }

            await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true;

            try
            {
                if(this.ContentStreamed)
                {
                    String? f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    Byte[]? h = await DecryptFileToHash(f,security,cancel).ConfigureAwait(false); if(h is null) { return false; }

                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,h,true)) { return false; }
                    if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                    ClearAssertion(DataSecurityEDKey);
                    this.SetCachedFileSize_NoSync(GetFileSize(f));
                    this.DataEncrypted = false;
                    this.DataProtectionInfo = null;
                    return true;
                }

                Byte[]? encrypted = this.EncryptedData;
                DataItemSecurityEnvelopeArrayHashResult? decrypted = await DecryptDataBufferWithHash(encrypted,security,cancel).ConfigureAwait(false);
                if(!decrypted.HasValue) { return false; }

                this.Content = decrypted.Value.Buffer;

                if(!UpdateFieldIntegrityHash(DataSecurityCNKey,decrypted.Value.Hash,true)) { return false; }
                if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }
                if(!UpdateFieldIntegrityHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                ClearAssertion(DataSecurityEDKey);
                this.DataEncrypted = false;
                this.DataProtectionInfo = null;
                this.EncryptedData = null;
                this.ClearHashCodeCache_NoSync();

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> EncryptData(DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        Boolean lk = false;

        try
        {
            if(security is null || this.DataEncrypted || this.Locked) { return false; }

            await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true;

            try
            {
                if(this.ContentStreamed)
                {
                    String? f = this.GetFILE(); if(String.IsNullOrEmpty(f) || File.Exists(f) is false) { return false; }

                    Byte[]? h = await EncryptFileToHash(f,security,cancel).ConfigureAwait(false); if(h is null) { return false; }

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,h,true)) { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security)) { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                    ClearAssertion(DataSecurityCNKey);
                    this.SetCachedFileSize_NoSync(GetFileSize(f));
                    this.DataEncrypted = true;
                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    return true;
                }

                Byte[]? content = this.Content;

                try
                {
                    DataItemSecurityEnvelopeArrayHashResult? encrypted = await EncryptDataBufferWithHash(content,security,cancel).ConfigureAwait(false);
                    if(!encrypted.HasValue) { return false; }

                    this.EncryptedData = encrypted.Value.Buffer;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,encrypted.Value.Hash,true)) { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security)) { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                    ClearAssertion(DataSecurityCNKey);
                    this.DataEncrypted = true;
                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    ZeroMemory(this.Content); this.Content = null;
                    this.ClearHashCodeCache_NoSync();

                    return true;
                }
                finally
                {
                    if(this.DataEncrypted && content is not null) { ZeroMemory(content); }
                }
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="op_Explicit"]/*'/>*/
    public static explicit operator MultiMediaItem?(Byte[]? content) => new MultiMediaItem(content);

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

            if(!TryAcquireSyncLocks(other,out LockState s)) { return false; }

            try
            {
                if(this.Content is null && other.Content is null) { return true; }

                if(this.Content is null && other.Content is not null) { return false; }

                if(this.Content is not null && other.Content is null) { return false; }

                return this.Content.AsSpan().SequenceEqual(other.Content.AsSpan());
            }
            finally { ReleaseSyncLocks(s); }
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

            return OrleansUtility.FromFile<MultiMediaItem>(path);
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
                if(DataIsolation.IsEnabled())
                {
                    HashSet<String> _ = new(this.Artists.Select(_ => new String(_)));

                    return _;
                }

                return this.Artists;
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
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.Content is null ? null : DataIsolation.IsEnabled() ? this.Content.Clone() as Byte[] : this.Content; }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<DataContent?> GetDataContent(DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        Stream? d = null;

        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted && security is null) { return null; }
                if(this.ContentStreamed) { return null; }

                if(this.DataEncrypted)
                {
                    if(this.EncryptedData != null && this.EncryptedData.Length > 0)
                    {
                        d = CreateBuffer(this.EncryptedData.Length);

                        if(!await this.DecryptToStream(this.EncryptedData,d,security,cancel).ConfigureAwait(false)) { return null; }
                    }

                    if(d is not MemoryStream m) { return null; }

                    return new() { MultiMedia = new Dictionary<String,Object?>(){{"Content",GetMemoryStreamBuffer(m)}} };
                }

                return this.Content is not null
                    ? new() { MultiMedia = new Dictionary<String,Object?>()
                    {
                        {"Content", DataIsolation.IsEnabled() ? this.Content?.Clone() as Byte[] : this.Content}
                    }}
                    : null;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
        finally
        {
            if(d is not null)
            {
                await ZeroMemoryAsync(d,default).ConfigureAwait(false);
                await d.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    ///<inheritdoc/>
    public override Stream? GetContentStream()
    {
        try
        {
            if(this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE)) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(File.Exists(this.FILE)) { try { return OpenSequentialRead(this.FILE); } catch {} }
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
            if(this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE)) { return null; }

            if(File.Exists(this.FILE)) { try { return OpenSequentialRead(this.FILE); } catch {} }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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
                        Artist             = this.Artists?.FirstOrDefault() is {} a ? new(a) : null,
                        BornOn             = this.BornOn?.ToString("O"),
                        ContentStreamed    = this.ContentStreamed,
                        DataType           = this.DataType is null ? null : new(this.DataType),
                        DistinguishedName  = this.DistinguishedName is null ? null : new(this.DistinguishedName),
                        FILE               = this.FILE is null ? null : new(this.FILE),
                        ID                 = id,
                        Modified           = this.Modified?.ToString("O"),
                        Name               = this.Name is null ? null : new(this.Name),
                        Notes              = this.Notes?.Select(n => new String(n)).ToHashSet(),
                        ObjectFILE         = this.ObjectFILE is null ? null : new(this.ObjectFILE),
                        ObjectType         = this.GetCachedInheritanceList(),
                        ServiceVersion     = this.ServiceVersion?.ToString(),
                        Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : this.Content?.LongLength).ToStringInvariant(),
                        Tags               = this.Tags?.Select(t => new String(t)).ToHashSet(),
                        Title              = this.Title is null ? null : new(this.Title),
                        Version            = this.Version?.ToString(),
                        Year               = this.Year?.ToStringInvariant()
                    };
                }

                return new()
                {
                    Application        = this.Application,
                    ApplicationVersion = this.ApplicationVersion?.ToString(),
                    Artist             = this.Artists?.FirstOrDefault(),
                    BornOn             = this.BornOn?.ToString("O"),
                    ContentStreamed    = this.ContentStreamed,
                    DataType           = this.DataType,
                    DistinguishedName  = this.DistinguishedName,
                    FILE               = this.FILE,
                    ID                 = id,
                    Modified           = this.Modified?.ToString("O"),
                    Name               = this.Name,
                    Notes              = this.Notes,
                    ObjectFILE         = this.ObjectFILE,
                    ObjectType         = this.GetCachedInheritanceList(),
                    ServiceVersion     = this.ServiceVersion?.ToString(),
                    Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : this.Content?.LongLength).ToStringInvariant(),
                    Tags               = this.Tags,
                    Title              = this.Title,
                    Version            = this.Version?.ToString(),
                    Year               = this.Year?.ToStringInvariant()
                };
            }
            finally { if(lk) { this.ReleaseLocks(); } }
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
            return BitConverter.ToInt32(SHA512.HashData(this.Content ?? Array.Empty<Byte>()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            var c = this.Content;

            return BitConverter.ToInt32(HashDataChunked(c ?? Array.Empty<Byte>(),cancel),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetLanguage"]/*'/>*/
    public String? GetLanguage()
    {
        try { return this.Language is null ? null : DataIsolation.IsEnabled() ? new(this.Language) : this.Language; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetLanguageFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetTitle"]/*'/>*/
    public String? GetTitle()
    {
        try { return this.Title is null ? null : DataIsolation.IsEnabled() ? new(this.Title) : this.Title; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetTitleFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="GetYear"]/*'/>*/
    public Int32? GetYear()
    {
        try { return this.Year; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetYearFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

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
                    && this.Content is null
                    && this.EncryptedData is null;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch(Exception _) { KusDepotLog.Error(_,IsMetadataOnlyFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
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
            return OrleansUtility.ParseBase64<MultiMediaItem>(input);
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
                HashSet<String> _ = DataIsolation.IsEnabled()
                    ? new(artists.Select(_ => new String(_)))
                    : new(artists);

                if(_.Count == 0) { this.Artists = null; MN(); return true; }

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
                if(bitrate == 0) { this.Bitrate = null; MN(); return true; }

                this.Bitrate = bitrate; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetBitrateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(Byte[]? content , DataItemSecurityContext? security = null)
    {
        try
        {
            if(content is null || (this.Locked && this.CheckManager(security) is false)) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(security is null) { return false; }

                    if(content.Length == 0)
                    {
                        this.EncryptedData = null; this.Content = null; MN();

                        if(UpdateFieldIntegrityHash(DataSecurityEDKey,null))
                        {
                            if(!ApplyAssertionState(DataSecurityEDKey,security))   { return false; }

                            this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                            return true;
                        }
                        else { return false; }
                    }

                    var eb = EncryptDataBuffer(content,security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; MN();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    return true;
                }
                else
                {
                    if(content.Length == 0)
                    {
                        this.Content = null;
                        this.ClearHashCodeCache_NoSync();

                        if(UpdateFieldIntegrityHash(DataSecurityCNKey,null))
                        {
                            return ApplyAssertionState(DataSecurityCNKey,security);
                        }
                        else { return false; }
                    }

                    this.Content = DataIsolation.IsEnabled() ? content.Clone() as Byte[] : content;
                    this.ClearHashCodeCache_NoSync();

                    if(UpdateFieldIntegrityHash(DataSecurityCNKey,this.Content))
                    {
                        return ApplyAssertionState(DataSecurityCNKey,security);
                    }
                    else { return false; }
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> SetContentStreamed(Boolean streamed , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            if(this.Locked || this.DataEncrypted) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(!streamed)
                {
                    if(this.ContentStreamed is false) { return true; }

                    this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false; MN();

                    if(await UpdateFieldIntegrityHashAsync(DataSecurityCNKey,this.Content,cancel:cancel).ConfigureAwait(false))
                    {
                        return ApplyAssertionState(DataSecurityCNKey,security);
                    }

                    return false;
                }

                String? f = this.GetFILE();

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false)
                {
                    this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false; MN();

                    await RefreshContentHashAsync().ConfigureAwait(false);

                    return false;
                }

                await using FileStream s = new(f,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.Asynchronous|FileOptions.SequentialScan});

                Byte[] h = await SHA512.HashDataAsync(s,cancel).ConfigureAwait(false);

                if(await UpdateFieldIntegrityHashAsync(DataSecurityCNKey,h,true,cancel).ConfigureAwait(false))
                {
                    this.SetCachedFileSize_NoSync(s.Length); this.ContentStreamed = true; MN();

                    return ApplyAssertionState(DataSecurityCNKey,security);
                }

                this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false; MN();

                return await RefreshContentHashAsync().ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false; if(NoExceptions||MyNoExceptions) { return false; } throw; }

        async Task<Boolean> RefreshContentHashAsync()
        {
            if(await UpdateFieldIntegrityHashAsync(DataSecurityCNKey,this.Content,cancel:cancel).ConfigureAwait(false)) { return ApplyAssertionState(DataSecurityCNKey,security); }

            return false;
        }
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
                if(duration == 0) { this.Duration = null; MN(); return true; }

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
                if(framerate == 0) { this.Framerate = null; MN(); return true; }

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

                this.Language = DataIsolation.IsEnabled() ? new(language) : language; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetLanguageFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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

                this.Title = DataIsolation.IsEnabled() ? new(title) : title; MN(); return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTitleFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
                if(year == 0) { this.Year = null; MN(); return true; }

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
            return OrleansUtility.TryParseBase64(input,out item);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
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
                using Stream? s = this.GetContentStream_NoSync();

                switch(field)
                {
                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false) { return false; }

                        if(this.ContentStreamed)
                        {
                            return await VerifyDataCore(field,security,null,s,cancel).ConfigureAwait(false);
                        }

                        if(this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,security,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.Content is null && this.ContentStreamed is false) { return false; }

                        return this.ContentStreamed ?
                            await VerifyDataCore(field,security,null,s,cancel).ConfigureAwait(false) :
                            await VerifyDataCore(field,security,this.Content,null,cancel).ConfigureAwait(false);
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

    ///<inheritdoc/>
    protected internal override async Task<Boolean> WipeDataCore(DataItemSecurityContext? security , HashSet<Object> visited , CancellationToken cancel = default)
    {
        if(await base.WipeDataCore(security,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(security) is false) { return false; }

        Boolean lk = false; await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true;

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
                    using(var fs = OpenExclusiveWrite(f))
                    {
                        await WipeMemoryAsync(fs,cancel).ConfigureAwait(false);
                    }
                    File.Delete(f); this.FILE = null; this.ClearCachedFileSize_NoSync();
                }

                this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false;
            }

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

            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(requirelocks) { await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true; }

            try
            {
                if(this.Content is not null)
                {
                    ZeroMemory(this.Content); this.Content = null;
                }

                this.ClearHashCodeCache_NoSync();
                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MultiMediaItem.xml' path='MultiMediaItem/class[@name="MultiMediaItem"]/property[@name="ExtensionData"]/*'/>*/
    [JsonIgnore] public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}