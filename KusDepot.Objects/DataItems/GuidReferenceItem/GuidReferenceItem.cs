namespace KusDepot;

/**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.GuidReferenceItem")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "GuidReferenceItem" , Namespace = "KusDepot")]

public sealed partial class GuidReferenceItem : DataItem , IComparable<GuidReferenceItem> , IEquatable<GuidReferenceItem> , IExtensibleDataObject , IParsable<GuidReferenceItem>
{
    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/content[@name="Content"]/*'/>*/
    [JsonPropertyName("Content")] [JsonInclude]
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal Guid? Content;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/field[@name="Next"]/*'/>*/
    [JsonPropertyName("Next")] [JsonInclude]
    [DataMember(Name = "Next" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    internal GuidReferenceItem? Next;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/field[@name="Previous"]/*'/>*/
    [JsonPropertyName("Previous")] [JsonInclude]
    [DataMember(Name = "Previous" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    internal GuidReferenceItem? Previous;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/field[@name="UndirectedLinks"]/*'/>*/
    [JsonPropertyName("UndirectedLinks")] [JsonInclude]
    [DataMember(Name = "UndirectedLinks" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    internal HashSet<GuidReferenceItem>? UndirectedLinks;

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public GuidReferenceItem() : this(null,null,null,null,null,null) {}

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/constructor[@name="Constructor"]/*'/>*/
    public GuidReferenceItem(Guid? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null)
    {
        try
        {
            this.Sync = null!; this.DataType = DataTypes.GUID; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.AddNotes(notes); this.AddTags(tags);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="CaptureContentIntegritySnapshot_NoSync"]/*'/>*/
    internal override ContentIntegritySnapshot CaptureContentIntegritySnapshot_NoSync()
    {
        if(this.DataEncrypted)
        {
            ContentIntegrityValue encryptedContent = ContentIntegrityValue.FromBytes(this.EncryptedData);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContent)],[encryptedContent]);
        }

        HashSet<GuidReferenceItem>? links = this.UndirectedLinks is null ? null : new(this.UndirectedLinks,ReferenceEqualityComparer.Instance);

        return new ContentIntegritySnapshot(
        [
            new KeyValuePair<String,Object?>("Content",this.Content),
            new KeyValuePair<String,Object?>("Next",this.Next),
            new KeyValuePair<String,Object?>("Previous",this.Previous),
            new KeyValuePair<String,Object?>("UndirectedLinks",links)
        ]);
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="CaptureContentMutationMemento_NoSync"]/*'/>*/
    internal override DataItemContentMutationMemento CaptureContentMutationMemento_NoSync()
    {
        Guid? originalContent = this.Content;
        GuidReferenceItem? originalNext = this.Next;
        GuidReferenceItem? originalPrevious = this.Previous;
        HashSet<GuidReferenceItem>? originalUndirectedLinks = this.UndirectedLinks;

        return new DataItemContentMutationMemento(item =>
        {
            GuidReferenceItem reference = (GuidReferenceItem)item;
            reference.Content = originalContent;
            reference.Next = originalNext;
            reference.Previous = originalPrevious;
            reference.UndirectedLinks = originalUndirectedLinks;
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

                if(!this.HasContentIntegrityData_NoSync()) { return false; }

                return await CheckDataHashCoreHashed(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
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

            if(!TryAcquireSyncLocks(other,out LockState s)) { return false; }

            try
            {
                if(this.Content is null && other.Content is null) { return true; }

                return Guid.Equals(this.Content,other.Content);
            }
            finally { ReleaseSyncLocks(s); }
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

            return OrleansUtility.FromFile<GuidReferenceItem>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetContent"]/*'/>*/
    public Guid? GetContent()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.Content; }

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

                if(this.DataEncrypted && this.EncryptedData != null && this.EncryptedData.Length > 0)
                {
                    d = CreateBuffer(this.EncryptedData.Length);

                    if(!await this.DecryptToStream(this.EncryptedData,d,security,cancel).ConfigureAwait(false)) { return null; }

                    if(d is not MemoryStream m) { return null; }
                    GuidReferenceItem? decrypted = Deserialize<GuidReferenceItem>(m); if(decrypted is null) { return null; }

                    return new()
                    {
                        GuidReference = new Dictionary<String,Object?>
                        {
                            {"Content", decrypted.Content},
                            {"Next", decrypted.Next?.Clone() as GuidReferenceItem},
                            {"Previous", decrypted.Previous?.Clone() as GuidReferenceItem},
                            {"UndirectedLinks", decrypted.UndirectedLinks?.Select(l => (l.Clone() as GuidReferenceItem)!).ToHashSet()}
                        }
                    };
                }

                if(!this.HasContentIntegrityData_NoSync()) { return null; }

                return new()
                {
                    GuidReference = new Dictionary<String,Object?>
                    {
                        {"Content", this.Content},
                        {"Next", DataIsolation.IsEnabled() ? this.Next?.Clone() as GuidReferenceItem : this.Next},
                        {"Previous", DataIsolation.IsEnabled() ? this.Previous?.Clone() as GuidReferenceItem : this.Previous},
                        {"UndirectedLinks", DataIsolation.IsEnabled() ? this.UndirectedLinks?.Select(l => (l.Clone() as GuidReferenceItem)!).ToHashSet() : this.UndirectedLinks}
                    }
                };
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
                    FILE               = this.FILE,
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
            if(this.Content is Guid value)
            {
                Span<Byte> h = stackalloc Byte[64]; HashGuid(value,h);

                return ReadHashPrefixInt32(h);
            }

            return ReadHashPrefixInt32(SHA512.HashData(Array.Empty<Byte>()));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            if(this.Content is Guid value)
            {
                Span<Byte> h = stackalloc Byte[64]; HashGuid(value,h,cancel);

                return Task.FromResult(ReadHashPrefixInt32(h));
            }

            return Task.FromResult(ReadHashPrefixInt32(SHA512.HashData(Array.Empty<Byte>())));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetNext"]/*'/>*/
    public GuidReferenceItem? GetNext()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? this.Next?.Clone() as GuidReferenceItem : this.Next; }

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

            try { return DataIsolation.IsEnabled() ? this.Previous?.Clone() as GuidReferenceItem : this.Previous; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetPreviousFail,MyTypeName,MyID); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="GetUndirectedLinks"]/*'/>*/
    public HashSet<GuidReferenceItem>? GetUndirectedLinks()
    {
        try
        {
            if(this.UndirectedLinks is null) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(DataIsolation.IsEnabled())
                {
                    HashSet<GuidReferenceItem> _ = new(this.UndirectedLinks.Select(_=>(_.Clone() as GuidReferenceItem)!));

                    if(!Equals(_.Count,this.UndirectedLinks.Count)) { return null; } return _;
                }

                return this.UndirectedLinks;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetUndirectedLinksFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="HasContentIntegrityData_NoSync"]/*'/>*/
    private Boolean HasContentIntegrityData_NoSync()
    {
        return this.Content is not null || this.Next is not null || this.Previous is not null || this.UndirectedLinks is { Count: > 0 };
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
                    && this.Next is null
                    && this.Previous is null
                    && this.EncryptedData is null;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsMetadataOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
            return OrleansUtility.ParseBase64<GuidReferenceItem>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareDecryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed) { return null; }

        await using MemoryStream output = CreateBuffer(this.EncryptedData?.Length ?? 0);

        if(!await this.DecryptToStream(this.EncryptedData,output,security,cancel).ConfigureAwait(false)) { return null; }

        GuidReferenceItem? decrypted = Deserialize<GuidReferenceItem>(output); if(decrypted is null) { return null; }

        GuidReferenceItem? next = decrypted.Next?.Clone() as GuidReferenceItem;

        GuidReferenceItem? previous = decrypted.Previous?.Clone() as GuidReferenceItem;

        HashSet<GuidReferenceItem>? links = null;

        if(decrypted.UndirectedLinks is not null)
        {
            links = new HashSet<GuidReferenceItem>(decrypted.UndirectedLinks.Count);

            foreach(GuidReferenceItem link in decrypted.UndirectedLinks)
            {
                GuidReferenceItem? clone = link.Clone() as GuidReferenceItem; if(clone is null) { return null; }

                links.Add(clone);
            }
        }

        Guid? originalContent = this.Content;

        GuidReferenceItem? originalNext = this.Next;

        GuidReferenceItem? originalPrevious = this.Previous;

        HashSet<GuidReferenceItem>? originalLinks = this.UndirectedLinks;

        return new DataItemProtectionChange(
            () => { this.Content = decrypted.Content; this.Next = next; this.Previous = previous; this.UndirectedLinks = links; return ValueTask.CompletedTask; },
            () => { this.Content = originalContent; this.Next = originalNext; this.Previous = originalPrevious; this.UndirectedLinks = originalLinks; return ValueTask.CompletedTask; });
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareEncryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed) { return null; }

        Byte[]? serialized = this.Content is not null || this.Next is not null || this.Previous is not null || this.UndirectedLinks is not null ? this.Serialize_NoSync() : null;

        try
        {
            DataItemSecurityEnvelopeArrayHashResult? encrypted = await this.EncryptDataBufferWithHash(serialized,security,cancel).ConfigureAwait(false);

            if(!encrypted.HasValue) { return null; }

            Guid? originalContent = this.Content;

            GuidReferenceItem? originalNext = this.Next;

            GuidReferenceItem? originalPrevious = this.Previous;

            HashSet<GuidReferenceItem>? originalLinks = this.UndirectedLinks;

            GuidReferenceItem wipeTarget = new() { Content = originalContent, Next = originalNext, Previous = originalPrevious, UndirectedLinks = originalLinks };

            return new DataItemProtectionChange(
                () => { this.Content = null; this.Next = null; this.Previous = null; this.UndirectedLinks = null; return ValueTask.CompletedTask; },
                () => { this.Content = originalContent; this.Next = originalNext; this.Previous = originalPrevious; this.UndirectedLinks = originalLinks; return ValueTask.CompletedTask; },
                encrypted.Value.Hash,
                encrypted.Value.Buffer,
                async () =>
                {
                    try
                    {
                        if(!await wipeTarget.WipeDataInMemoryOnly(null,false,cancel).ConfigureAwait(false)) { throw new InvalidOperationException(ProtectionPayloadWipeFailed); }
                    }
                    catch ( Exception error ) { KusDepotLog.Error(error,EncryptDataFail,MyTypeName,MyID); }
                });
        }
        finally { ZeroMemory(serialized); }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="RefreshContentIntegrityState_NoSync"]/*'/>*/
    private Boolean RefreshContentIntegrityState_NoSync(DataItemSecurityContext? security)
    {
        Boolean updated = this.HasContentIntegrityData_NoSync() ? UpdateContentIntegrityHash() : UpdateContentIntegrityHash(true);

        if(updated && ApplyAssertionState(DataSecurityCNKey,security)) { return true; }

        UpdateContentIntegrityHash(true);

        ClearAssertion(DataSecurityCNKey);

        return false;
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="RefreshContentIntegrityStateAsync"]/*'/>*/
    private async Task<Boolean> RefreshContentIntegrityStateAsync(DataItemSecurityContext? security , CancellationToken cancel)
    {
        Boolean updated = this.HasContentIntegrityData_NoSync()
            ? await UpdateContentIntegrityHashAsync(cancel:cancel).ConfigureAwait(false)
            : await UpdateContentIntegrityHashAsync(clear:true,cancel:cancel).ConfigureAwait(false);

        if(updated && ApplyAssertionState(DataSecurityCNKey,security)) { return true; }

        await UpdateContentIntegrityHashAsync(clear:true,cancel:cancel).ConfigureAwait(false);

        ClearAssertion(DataSecurityCNKey);

        return false;
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(Guid? content , DataItemSecurityContext? security = null)
    {
        try
        {
            if(content is null || (this.Locked && this.CheckManager(security) is false)) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                using DataItemContentMutation mutation = new(this);

                if(this.DataEncrypted)
                {
                    if(security is null) { return false; }

                    if(Guid.Equals(content,Guid.Empty)) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData, security); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)      { return false; }

                    di.Content = content;

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; this.UndirectedLinks = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);
                    mutation.Commit(() => { MN(); });
                    return true;
                }
                else
                {
                    this.Content = Guid.Equals(content,Guid.Empty) ? null : content;

                    if(!this.RefreshContentIntegrityState_NoSync(security)) { return false; }
                    mutation.Commit(() => { MN(); });
                    return true;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetNext"]/*'/>*/
    public Boolean SetNext(GuidReferenceItem? next , DataItemSecurityContext? security = null)
    {
        try
        {
            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(security is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)     { return false; }

                    di.Next = next;

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; this.UndirectedLinks = null; MN();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    return true;
                }
                else
                {
                    this.Next = next is null ? null : DataIsolation.IsEnabled() ? next.Clone() as GuidReferenceItem : next; MN();
                    return this.RefreshContentIntegrityState_NoSync(security);
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetNextFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetPrevious"]/*'/>*/
    public Boolean SetPrevious(GuidReferenceItem? previous , DataItemSecurityContext? security = null)
    {
        try
        {
            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(security is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)     { return false; }

                    di.Previous = previous;

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; MN();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    return true;
                }
                else
                {
                    this.Previous = previous is null ? null : DataIsolation.IsEnabled() ? previous.Clone() as GuidReferenceItem : previous; MN();
                    return this.RefreshContentIntegrityState_NoSync(security);
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetPreviousFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="SetUndirectedLinks"]/*'/>*/
    public Boolean SetUndirectedLinks(IEnumerable<GuidReferenceItem>? undirectedlinks , DataItemSecurityContext? security = null)
    {
        try
        {
            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(security is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as GuidReferenceItem; ZeroMemory(db); if(di is null)     { return false; }

                    di.UndirectedLinks = undirectedlinks is null ? null : new(undirectedlinks);

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Next = null; this.Previous = null; this.UndirectedLinks = null; MN();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    return true;
                }
                else
                {
                    HashSet<GuidReferenceItem>? links = undirectedlinks is null
                        ? null
                        : DataIsolation.IsEnabled()
                            ? new(undirectedlinks.Select(_=>(_.Clone() as GuidReferenceItem)!))
                            : new(undirectedlinks);

                    this.UndirectedLinks = links is { Count: > 0 } ? links : null; MN();
                    return this.RefreshContentIntegrityState_NoSync(security);
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetUndirectedLinksFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<String?> SignData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        if(String.Equals(field,DataSecurityCNKey,Ordinal))
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try { if(!this.HasContentIntegrityData_NoSync()) { return null; } }

            finally { this.Sync.Data.Release(); }
        }

        return await base.SignData(field,security,cancel).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out GuidReferenceItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            return OrleansUtility.TryParseBase64(input,out item);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/method[@name="TryPrepareContentIntegrityState"]/*'/>*/
    private protected override Boolean TryPrepareContentIntegrityState(out Byte[]? digest)
    {
        digest = this.HasContentIntegrityData_NoSync() ? this.GetIntegrityHash_NoSync() : null;

        return this.HasContentIntegrityData_NoSync();
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
                    case DataSecurityEDKey:
                    {
                        if(this.DataEncrypted is false || this.EncryptedData is null) { return false; }

                        return await VerifyDataCore(field,security,this.EncryptedData,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(!this.HasContentIntegrityData_NoSync()) { return false; }

                        return await VerifyDataCoreHashed(field,security,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
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
    protected internal override async Task<Boolean> WipeDataCore(DataItemSecurityContext? security, HashSet<Object> visited, CancellationToken cancel = default)
    {
        if(await base.WipeDataCore(security,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(security) is false) { return false; }

        Boolean lk = false; await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true;

        try
        {
            this.Content = null;

            if(this.Next is not null)
            {
                await this.Next.WipeDataCore(security,visited,cancel).ConfigureAwait(false); this.Next = null;
            }

            if(this.Previous is not null)
            {
                await this.Previous.WipeDataCore(security,visited,cancel).ConfigureAwait(false); this.Previous = null;
            }

            if(this.UndirectedLinks is not null)
            {
                foreach(var _ in this.UndirectedLinks) { await _.WipeDataCore(security,visited,cancel).ConfigureAwait(false); }

                this.UndirectedLinks.Clear(); this.UndirectedLinks = null;
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

            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(requirelocks) { await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true; }

            try
            {
                this.Content = null;

                if(this.Next is not null)
                {
                    await this.Next.WipeDataInMemoryOnlyCore(security,visited,true,cancel).ConfigureAwait(false); this.Next = null;
                }

                if(this.Previous is not null)
                {
                    await this.Previous.WipeDataInMemoryOnlyCore(security,visited,true,cancel).ConfigureAwait(false); this.Previous = null;
                }

                if(this.UndirectedLinks is not null)
                {
                    foreach(var _ in this.UndirectedLinks) { await _.WipeDataInMemoryOnlyCore(security,visited,true,cancel).ConfigureAwait(false); }

                    this.UndirectedLinks.Clear(); this.UndirectedLinks = null;
                }

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='GuidReferenceItem.xml' path='GuidReferenceItem/class[@name="GuidReferenceItem"]/property[@name="ExtensionData"]/*'/>*/
    [JsonIgnore] public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}