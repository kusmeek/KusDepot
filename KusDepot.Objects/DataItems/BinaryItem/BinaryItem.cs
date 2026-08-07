namespace KusDepot;

/**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.BinaryItem")]
[DataContract(Name = "BinaryItem" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed partial class BinaryItem : DataItem , IComparable<BinaryItem> , IEquatable<BinaryItem> , IExtensibleDataObject , IParsable<BinaryItem>
{
    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/field[@name="Commands"]/*'/>*/
    [JsonPropertyName("Commands")] [JsonInclude]
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal HashSet<CommandDescriptor>? Commands;

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/content[@name="Content"]/*'/>*/
    [JsonPropertyName("Content")] [JsonInclude] [Description(BinaryContentDescription)]
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    internal Byte[]? Content;

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/field[@name="Services"]/*'/>*/
    [JsonPropertyName("Services")] [JsonInclude]
    [DataMember(Name = "Services" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    internal HashSet<ToolDescriptor>? Services;

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public BinaryItem() : this(null,null,null,null,null,null,null,null) {}

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryItem(Byte[]? content = null , IEnumerable<String>? dllpaths = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetDataType(type); this.AddNotes(notes); this.AddTags(tags);
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
                    this.EncryptedData = null;

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

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="op_Explicit_Content"]/*'/>*/
    public static explicit operator Byte[]?(BinaryItem? item) => item?.GetContent();

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="op_Explicit"]/*'/>*/
    public static explicit operator BinaryItem?(Byte[]? content) => new BinaryItem(content);

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

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="FromFile"]/*'/>*/
    public static BinaryItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return  null; }

            return OrleansUtility.FromFile<BinaryItem>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="GetContent"]/*'/>*/
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

                    return new() { Binary = GetMemoryStreamBuffer(m) };
                }

                return this.Content is not null ? new() { Binary = DataIsolation.IsEnabled() ? this.Content.Clone() as Byte[] : this.Content } : null;
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
                        Commands           = this.Commands?.ToHashSet(),
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
                        Services           = this.Services?.ToHashSet(),
                        ServiceVersion     = this.ServiceVersion?.ToString(),
                        Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : this.Content?.LongLength).ToStringInvariant(),
                        Tags               = this.Tags?.Select(t => new String(t)).ToHashSet(),
                        Version            = this.Version?.ToString()
                    };
                }

                return new()
                {
                    Application        = this.Application,
                    ApplicationVersion = this.ApplicationVersion?.ToString(),
                    BornOn             = this.BornOn?.ToString("O"),
                    Commands           = this.Commands,
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
                    Services           = this.Services,
                    ServiceVersion     = this.ServiceVersion?.ToString(),
                    Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : this.Content?.LongLength).ToStringInvariant(),
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
            return BitConverter.ToInt32(SHA512.HashData(this.Content ?? Array.Empty<Byte>()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            var c = this.Content;

            return Task.FromResult(BitConverter.ToInt32(HashDataChunked(c ?? Array.Empty<Byte>(),cancel),0));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
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
        catch ( Exception _ ) { KusDepotLog.Error(_,IsMetadataOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
            return OrleansUtility.ParseBase64<BinaryItem>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/method[@name="SetContent"]/*'/>*/
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
                        this.EncryptedData = null; this.Content = null; this.Commands = null; this.Services = null; MN();

                        if(UpdateFieldIntegrityHash(DataSecurityEDKey,null))
                        {
                            if(!ApplyAssertionState(DataSecurityEDKey,security))   { return false; }

                            this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                            return true;
                        }
                        else { return false; }
                    }

                    var eb = EncryptDataBuffer(content,security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Commands = null; this.Services = null; MN();

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
                        this.Content = null; this.Commands = null; this.Services = null; MN();

                        if(UpdateFieldIntegrityHash(DataSecurityCNKey,null))
                        {
                            return ApplyAssertionState(DataSecurityCNKey,security);
                        }
                        else { return false; }
                    }

                    this.Content = DataIsolation.IsEnabled() ? content.Clone() as Byte[] : content; this.Commands = GetAssemblyCommands(this.Content); this.Services = GetAssemblyServices(this.Content); MN();

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

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out BinaryItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            return OrleansUtility.TryParseBase64(input,out item);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
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

            this.Commands = null; this.Services = null;

            MN(); return true;
        }
        finally { if(lk) { this.ReleaseLocks(); } }
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

                this.Commands = null; this.Services = null;

                this.ClearHashCodeCache_NoSync();

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='BinaryItem.xml' path='BinaryItem/class[@name="BinaryItem"]/property[@name="ExtensionData"]/*'/>*/
    [JsonIgnore] public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}