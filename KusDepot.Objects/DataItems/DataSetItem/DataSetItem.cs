namespace KusDepot;

/**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.DataSetItem")]
[DataContract(Name = "DataSetItem" , Namespace = "KusDepot")]

public sealed partial class DataSetItem : DataItem , IComparable<DataSetItem> , IEquatable<DataSetItem> , IExtensibleDataObject , IParsable<DataSetItem>
{
    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/content[@name="Content"]/*'/>*/
    [JsonPropertyName("Content")] [JsonInclude]
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal HashSet<DataItem>? Content;

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/field[@name="ContentIndex"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    private Dictionary<Guid,DataItem>? ContentIndex;

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public DataSetItem() : this(null,null,null,null,null,null,null) {}

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/constructor[@name="Constructor"]/*'/>*/
    public DataSetItem(IEnumerable<DataItem>? content = null , String? file = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.AddNotes(notes); this.AddTags(tags); this.SetDataType(type);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="AddDataItems"]/*'/>*/
    public Boolean AddDataItems(IEnumerable<DataItem>? data , DataItemSecurityContext? security = null)
    {
        try
        {
            if(data is null || (this.Locked && this.CheckManager(security) is false)) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                using DataItemContentMutation mutation = new(this);

                if(this.DataEncrypted)
                {
                    if(security is null) { return false; }

                    if(!data.TryDistinctByID(out var nc) || nc.Count == 0) { return nc?.Count == 0; }

                    if(this.ContentStreamed) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null) { return false; }

                    if(di.Content is null) { di.Content = new(new IDEquality()); }

                    var ids = di.GetContentIndex_NoSync();

                    foreach(var i in nc)
                    {
                        Guid? id = i.GetID(); if(id is null || id == Guid.Empty) { return false; }

                        if(ids?.ContainsKey(id.Value) is true) { return false; }
                    }

                    if(di.Content.TryAddByID(nc,out var _) is false) { return false; }

                    di.RefreshContentIndex_NoSync();

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.ContentIndex = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))          { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                         { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))         { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });
                    return true;
                }
                else
                {
                    if(!data.TryDistinctByID(out var nc) || nc.Count == 0) { return nc?.Count == 0; }

                    var ids = this.GetContentIndex_NoSync();

                    foreach(var i in nc)
                    {
                        Guid? id = i.GetID(); if(id is null || id == Guid.Empty) { return false; }

                        if(ids?.ContainsKey(id.Value) is true) { return false; }
                    }

                    if(DataIsolation.IsEnabled())
                    {
                        if(!nc.TryCloneDistinctByID(out var clones)) { return false; }

                        if(clones.Count != nc.Count) { return false; }

                        HashSet<DataItem> candidate = this.Content is null ? new(new IDEquality()) : new(this.Content,new IDEquality());

                        if(clones.Any(candidate.Contains)) { return false; }

                        foreach(DataItem item in clones) { candidate.Add(item); }

                        this.Content = candidate;
                    }
                    else
                    {
                        this.Content ??= new(new IDEquality());

                        foreach(var i in nc) { this.Content.Add(i); }
                    }

                    this.RefreshContentIndex_NoSync();

                    if(UpdateContentIntegrityHash())
                    {
                        if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                        mutation.Commit(() => { MN(); });
                        return true;
                    }
                    else { return false; }
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="BuildContentIndex"]/*'/>*/
    private static Dictionary<Guid,DataItem>? BuildContentIndex(HashSet<DataItem>? content)
    {
        if(content is null || content.Count == 0) { return null; }

        Dictionary<Guid,DataItem> _ = new(content.Count);

        foreach(var i in content)
        {
            if(i.GetID() is Guid id && id != Guid.Empty) { _[id] = i; }
        }

        return _.Count == 0 ? null : _;
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="CaptureContentMutationMemento_NoSync"]/*'/>*/
    internal override DataItemContentMutationMemento CaptureContentMutationMemento_NoSync()
    {
        HashSet<DataItem>? originalContent = this.Content;
        HashSet<DataItem>? originalContentMembers = originalContent is null ? null : new(originalContent,ReferenceEqualityComparer.Instance);
        Dictionary<Guid,DataItem>? originalContentIndex = this.ContentIndex is null ? null : new(this.ContentIndex);

        return new DataItemContentMutationMemento(item =>
        {
            DataSetItem dataSetItem = (DataSetItem)item;

            if(originalContent is not null)
            {
                originalContent.Clear();
                originalContent.UnionWith(originalContentMembers!);
            }

            dataSetItem.Content = originalContent;
            dataSetItem.ContentIndex = originalContentIndex;
        });
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="CaptureContentIntegritySnapshot_NoSync"]/*'/>*/
    internal override ContentIntegritySnapshot CaptureContentIntegritySnapshot_NoSync()
    {
        if(this.DataEncrypted && this.ContentStreamed)
        {
            Stream? encryptedStream = this.GetContentStream_NoSync();

            if(encryptedStream is null) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

            ContentIntegrityValue encryptedContentStream = ContentIntegrityValue.FromStream(encryptedStream);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContentStream)],[encryptedContentStream]);
        }

        if(this.DataEncrypted)
        {
            ContentIntegrityValue encryptedContent = ContentIntegrityValue.FromBytes(this.EncryptedData);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContent)],[encryptedContent]);
        }

        if(this.ContentStreamed)
        {
            Stream? stream = this.GetContentStream_NoSync();

            if(stream is null) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

            ContentIntegrityValue contentStream = ContentIntegrityValue.FromStream(stream);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",contentStream)],[contentStream]);
        }

        HashSet<DataItem>? content = this.Content is null ? null : new(this.Content,ReferenceEqualityComparer.Instance);

        return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",content)]);
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="HasContentIntegrityData_NoSync"]/*'/>*/
    private Boolean HasContentIntegrityData_NoSync() => this.Content is { Count: > 0 } || this.ContentStreamed;

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
                        var encryptedFile = this.GetFILE(); if(File.Exists(encryptedFile) is false) { return false; } using Stream encryptedStream = OpenSequentialRead(encryptedFile);

                        return await CheckDataHashCore(null,encryptedStream,cancel).ConfigureAwait(false);
                    }

                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.ContentStreamed)
                {
                    if(!this.HasContentIntegrityData_NoSync()) { return false; }

                    return await CheckDataHashCoreHashed(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
                }

                if(!this.HasContentIntegrityData_NoSync()) { return false; }

                return await CheckDataHashCoreHashed(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is DataSetItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(DataSetItem? other)
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

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(DataSetItem? a , DataSetItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(DataSetItem? a , DataSetItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as DataSetItem); }

    ///<inheritdoc/>
    public Boolean Equals(DataSetItem? other)
    {
        try
        {
            if(other is null) { return false; } if(ReferenceEquals(this,other)) { return true; }

            if(!TryAcquireSyncLocks(other,out LockState s)) { return false; }

            try
            {
                if(this.Content is null && other.Content is null) { return true; }

                if(this.Content is null && other.Content is not null) { return false; }

                if(this.Content is not null && other.Content is null) { return false; }

                HashSet<Guid?> ids = new();

                foreach(DataItem item in this.Content!) { ids.Add(item.GetID()); }

                HashSet<Guid?> otherIDs = new();

                foreach(DataItem item in other.Content!) { otherIDs.Add(item.GetID()); }

                return ids.SetEquals(otherIDs);
            }
            finally { ReleaseSyncLocks(s); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="FromFile"]/*'/>*/
    public static DataSetItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            return OrleansUtility.FromFile<DataSetItem>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetContent"]/*'/>*/
    public HashSet<DataItem>? GetContent()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Content is null) { return null; }

                if(DataIsolation.IsEnabled())
                {
                    if(!this.Content.TryCloneDistinctByID(out var o)) { return null; }

                    if(Equals(this.Content.Count,o.Count) is false) { return null; } return o;
                }

                return this.Content;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetContentIndex_NoSync"]/*'/>*/
    private Dictionary<Guid,DataItem>? GetContentIndex_NoSync()
    {
        this.ContentIndex ??= BuildContentIndex(this.Content); return this.ContentIndex;
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetDataItem"]/*'/>*/
    public DataItem? GetDataItem(Guid? id , DataItemSecurityContext? security = null)
    {
        try
        {
            if(id is null) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(security is null) { return null; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return null; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null) { return null; }

                    return di.GetContentIndex_NoSync()?.GetValueOrDefault(id.Value);
                }

                if(this.Content is null) { return null; }

                return this.GetContentIndex_NoSync()?.GetValueOrDefault(id.Value) is {} item
                    ? DataIsolation.IsEnabled() ? item.Clone() : item
                    : null;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="GetDataItems"]/*'/>*/
    public HashSet<DataItem>? GetDataItems(IEnumerable<Guid>? ids = null , DataItemSecurityContext? security = null)
    {
        try
        {
            HashSet<Guid>? _ = ids is null ? null : ids as HashSet<Guid> ?? new(ids); if(_?.Count == 0) { return new(new IDEquality()); }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? db = null;
            try
            {
                Boolean iso = DataIsolation.IsEnabled(); HashSet<DataItem>? o = null;

                if(this.DataEncrypted)
                {
                    if(security is null) { return null; }

                    db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return null; }

                    var di = Deserialize(db) as DataSetItem; if(di is null) { return null; }

                    if(_ is null)
                    {
                        if(di.Content is null) { return null; }

                        if(!di.Content.TryCloneDistinctByID(out var cloned)) { return null; }

                        if(Equals(di.Content.Count,cloned.Count) is false) { return null; } return cloned;
                    }

                    var ix = di.GetContentIndex_NoSync(); if(ix is null) { return new(new IDEquality()); }

                    foreach(var id in _)
                    {
                        if(ix.GetValueOrDefault(id) is not {} item) { continue; }

                        if(iso)
                        {
                            var clone = item.Clone(); if(clone is null) { return null; }

                            o ??= new(new IDEquality()); o.Add(clone);
                        }
                        else
                        {
                            o ??= new(new IDEquality()); o.Add(item);
                        }
                    }

                    return o;
                }

                if(_ is null)
                {
                    if(this.Content is null) { return null; }

                    if(DataIsolation.IsEnabled())
                    {
                        if(!this.Content.TryCloneDistinctByID(out var c)) { return null; }

                        if(Equals(this.Content?.Count,c.Count) is false) { return null; } return c;
                    }

                    return this.Content;
                }

                if(this.Content is null) { return new(new IDEquality()); }

                var ix2 = this.GetContentIndex_NoSync(); if(ix2 is null) { return new(new IDEquality()); }

                foreach(var id in _)
                {
                    if(ix2.GetValueOrDefault(id) is not {} item) { continue; }

                    if(iso)
                    {
                        var clone = item.Clone(); if(clone is null) { return null; }

                        o ??= new(new IDEquality()); o.Add(clone);
                    }
                    else
                    {
                        o ??= new(new IDEquality()); o.Add(item);
                    }
                }

                return o;
            }
            finally
            {
                if(db is not null) { ZeroMemory(db); }

                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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
                    DataSetItem? decrypted = Deserialize<DataSetItem>(m); if(decrypted is null) { return null; }

                    return new() { DataSet = decrypted.Content };
                }

                if(this.Content is null) { return null; }
                if(DataIsolation.IsEnabled())
                {
                    if(!this.Content.TryCloneDistinctByID(out HashSet<DataItem>? clone)) { return null; }
                    return new() { DataSet = clone };
                }

                return new() { DataSet = this.Content };
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
                        Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : this.Content?.Count).ToStringInvariant(),
                        Tags               = this.Tags?.Select(t => new String(t)).ToHashSet(),
                        Version            = this.Version?.ToString()
                    };
                }

                return new()
                {
                    Application        = this.Application,
                    ApplicationVersion = this.ApplicationVersion?.ToString(),
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
                    Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : this.Content?.Count).ToStringInvariant(),
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
            if(this.Content is null) { return HashCode.Combine(false,0); }

            HashSet<Guid?> uniqueIDs = new();

            foreach(DataItem item in this.Content) { uniqueIDs.Add(item.GetID()); }

            List<Guid?> contentIDs = new(uniqueIDs); contentIDs.Sort();

            HashCode hash = new(); hash.Add(true); hash.Add(contentIDs.Count);

            foreach(Guid? contentID in contentIDs) { hash.Add(contentID); }

            return hash.ToHashCode();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            return this.GetHashCode_NoSync();
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

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); this.RefreshContentIndex_NoSync(); }

    ///<inheritdoc/>
    static DataSetItem IParsable<DataSetItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="Parse"]/*'/>*/
    public static new DataSetItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            return OrleansUtility.ParseBase64<DataSetItem>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareDecryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed)
        {
            String? file = this.GetFILE(); if(String.IsNullOrEmpty(file) || File.Exists(file) is false) { return null; }

            Int64? originalSize = GetFileSize(file);

            StagedFileTransform? staged = await this.DecryptFileToStagedTransform(file,security,cancel).ConfigureAwait(false); if(staged is null) { return null; }

            return new DataItemProtectionChange(
                () => { staged.Commit(); this.SetCachedFileSize_NoSync(GetFileSize(file)); return ValueTask.CompletedTask; },
                () => { try { staged.Rollback(); } finally { this.SetCachedFileSize_NoSync(originalSize); } return ValueTask.CompletedTask; },
                complete: () => { staged.Complete(); return ValueTask.CompletedTask; },
                cleanupArtifactPath: () => staged.BackupPath);
        }

        await using MemoryStream output = CreateBuffer(this.EncryptedData?.Length ?? 0);

        if(!await this.DecryptToStream(this.EncryptedData,output,security,cancel).ConfigureAwait(false)) { return null; }

        DataSetItem? decrypted = Deserialize<DataSetItem>(output); if(decrypted is null) { return null; }

        HashSet<DataItem>? candidate = null;

        if(decrypted.Content is not null && !decrypted.Content.TryCloneDistinctByID(out candidate)) { return null; }

        HashSet<DataItem>? originalContent = this.Content;

        Dictionary<Guid,DataItem>? originalIndex = this.ContentIndex;

        return new DataItemProtectionChange(
            () => { this.Content = candidate; this.RefreshContentIndex_NoSync(); return ValueTask.CompletedTask; },
            () => { this.Content = originalContent; this.ContentIndex = originalIndex; return ValueTask.CompletedTask; });
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareEncryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed)
        {
            if(this.Content is not null || this.ContentIndex is not null) { return null; }

            String? file = this.GetFILE(); if(String.IsNullOrEmpty(file) || File.Exists(file) is false) { return null; }

            Int64? originalSize = GetFileSize(file);

            StagedFileTransform? staged = await this.EncryptFileToStagedTransform(file,security,cancel).ConfigureAwait(false); if(staged is null) { return null; }

            return new DataItemProtectionChange(
                () => { staged.Commit(); this.SetCachedFileSize_NoSync(GetFileSize(file)); return ValueTask.CompletedTask; },
                () => { try { staged.Rollback(); } finally { this.SetCachedFileSize_NoSync(originalSize); } return ValueTask.CompletedTask; },
                staged.Digest,
                complete: () => { staged.Complete(); return ValueTask.CompletedTask; },
                cleanupArtifactPath: () => staged.BackupPath);
        }

        Byte[]? serialized = this.Content is not null ? this.Serialize_NoSync() : null;

        try
        {
            DataItemSecurityEnvelopeArrayHashResult? encrypted = await this.EncryptDataBufferWithHash(serialized,security,cancel).ConfigureAwait(false);

            if(!encrypted.HasValue) { return null; }

            HashSet<DataItem>? originalContent = this.Content;

            Dictionary<Guid,DataItem>? originalIndex = this.ContentIndex;

            return new DataItemProtectionChange(
                () => { this.Content = null; this.ContentIndex = null; return ValueTask.CompletedTask; },
                () => { this.Content = originalContent; this.ContentIndex = originalIndex; return ValueTask.CompletedTask; },
                encrypted.Value.Hash,
                encrypted.Value.Buffer);
        }
        finally { ZeroMemory(serialized); }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="RefreshContentIndex_NoSync"]/*'/>*/
    private void RefreshContentIndex_NoSync()
    {
        this.ContentIndex = BuildContentIndex(this.Content);
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="RemoveDataItem"]/*'/>*/
    public Boolean RemoveDataItem(Guid? id , DataItemSecurityContext? security = null)
    {
        try
        {
            if(id is null || (this.Locked && this.CheckManager(security) is false)) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                using DataItemContentMutation mutation = new(this);

                if(this.DataEncrypted)
                {
                    if(this.ContentStreamed || security is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null)           { return false; }

                    if(di.Content is null)                                                            { return true; }

                    DataItem? _ = di.GetContentIndex_NoSync()?.GetValueOrDefault(id.Value); if(_ is null) { return true; }

                    if(di.Content.Remove(_))
                    {
                        if(di.Content.Count == 0) { di.Content = null; }

                        di.RefreshContentIndex_NoSync();

                        var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                        this.EncryptedData = eb; this.Content = null; this.ContentIndex = null;

                        if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))          { return false; }
                        if(!ApplyAssertionState(DataSecurityEDKey,security))                         { return false; }
                        if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))         { return false; }

                        this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                        mutation.Commit(() => { MN(); });
                        return true;
                    }
                    return false;
                }
                else
                {
                    if(this.Content is null) { return true; }

                    DataItem? _ = this.GetContentIndex_NoSync()?.GetValueOrDefault(id.Value);

                    if(_ is null) { return true; }

                    if(this.Content.Remove(_))
                    {
                        if(this.Content.Count == 0) { this.Content = null; }

                        this.RefreshContentIndex_NoSync();

                        if(UpdateContentIntegrityHash())
                        {
                            if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                            mutation.Commit(() => { MN(); });
                            return true;
                        }
                        else { return false; }
                    }
                    return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="RemoveDataItems"]/*'/>*/
    public Boolean RemoveDataItems(IEnumerable<Guid>? ids , DataItemSecurityContext? security = null)
    {
        try
        {
            if(ids is null || (this.Locked && this.CheckManager(security) is false)) { return false; }

            HashSet<Guid> _ = ids as HashSet<Guid> ?? new(ids); if(_.Count == 0) { return true; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            Byte[]? db = null;
            try
            {
                using DataItemContentMutation mutation = new(this);

                if(this.DataEncrypted)
                {
                    if(this.ContentStreamed || security is null) { return false; }

                    db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; if(di is null) { return false; }

                    if(di.Content is null) { return true; }

                    var ix = di.GetContentIndex_NoSync(); if(ix is null) { return true; }

                    Boolean mod = false;

                    foreach(var id in _)
                    {
                        if(ix.GetValueOrDefault(id) is DataItem item && di.Content.Remove(item)) { mod = true; }
                    }

                    if(mod is false) { return true; }

                    if(di.Content.Count == 0) { di.Content = null; }

                    di.RefreshContentIndex_NoSync();

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.ContentIndex = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))          { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                         { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))         { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });
                    return true;
                }

                if(this.Content is null) { return true; }

                var ix2 = this.GetContentIndex_NoSync(); if(ix2 is null) { return true; }

                Boolean mod2 = false;

                foreach(var id in _)
                {
                    if(ix2.GetValueOrDefault(id) is DataItem item && this.Content.Remove(item)) { mod2 = true; }
                }

                if(mod2 is false) { return true; }

                if(this.Content.Count == 0) { this.Content = null; }

                this.RefreshContentIndex_NoSync();

                if(UpdateContentIntegrityHash())
                {
                    if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                    mutation.Commit(() => { MN(); });
                    return true;
                }
                else { return false; }
            }
            finally
            {
                if(db is not null) { ZeroMemory(db); }

                this.Sync.Data.Release();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(IEnumerable<DataItem>? content , DataItemSecurityContext? security = null)
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
                    if(this.ContentStreamed || security is null) { return false; }

                    if(!content.TryDistinctByID(out var nc)) { return false; }

                    if(nc.Count == 0) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as DataSetItem; ZeroMemory(db); if(di is null)           { return false; }

                    di.Content = nc;

                    di.RefreshContentIndex_NoSync();

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.ContentIndex = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });
                    return true;
                }
                else
                {
                    if(!content.TryDistinctByID(out var nc) || nc.Any(i => ReferenceEquals(i,this))) { return false; }

                    if(DataIsolation.IsEnabled())
                    {
                        if(!nc.TryCloneDistinctByID(out var __)) { return false; }

                        if(__.Count == 0)
                        {
                            this.Content = null; this.ContentIndex = null;

                            if(UpdateFieldIntegrityHash(DataSecurityCNKey,null))
                            {
                                if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                                mutation.Commit(() => { MN(); });
                                return true;
                            }
                            else { return false; }
                        }

                        this.Content = __; this.RefreshContentIndex_NoSync();
                    }
                    else
                    {
                        if(nc.Count == 0)
                        {
                            this.Content = null; this.ContentIndex = null;

                            if(UpdateFieldIntegrityHash(DataSecurityCNKey,null))
                            {
                                if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                                mutation.Commit(() => { MN(); });
                                return true;
                            }
                            else { return false; }
                        }

                        this.Content = nc; this.RefreshContentIndex_NoSync();
                    }

                    if(UpdateContentIntegrityHash())
                    {
                        if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                        mutation.Commit(() => { MN(); });
                        return true;
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
                if(this.Locked || this.DataEncrypted) { return false; }

				DataItemStateSnapshot original = new(this);
				Boolean committed = false;

				try
				{
					if(!streamed)
					{
						if(this.ContentStreamed is false) { return true; }

						this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false;

                        if(this.Content is null) { this.ClearAssertion(DataSecurityCNKey); }

                        if(await UpdateContentIntegrityHashAsync(clear:this.Content is null,cancel:cancel).ConfigureAwait(false) && ApplyAssertionState(DataSecurityCNKey,security))
						{
							MN(); committed = true; return true;
						}

						return false;
					}

					String? f = this.GetFILE();

					if(String.IsNullOrEmpty(f) || File.Exists(f) is false)
					{
						this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false;

						await RefreshContentHashAsync().ConfigureAwait(false);

						return false;
					}

					await using FileStream s = new(f,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.Asynchronous|FileOptions.SequentialScan});

					this.SetCachedFileSize_NoSync(s.Length); this.ContentStreamed = true;

					if(await UpdateContentIntegrityHashAsync(cancel:cancel).ConfigureAwait(false))
					{
						if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

						MN(); committed = true; return true;
					}

					this.ClearCachedFileSize_NoSync(); this.ContentStreamed = false;

					return await RefreshContentHashAsync().ConfigureAwait(false);
				}
				finally { if(!committed) { original.Restore(this); } }
			}
			finally { this.Sync.Data.Release(); }
		}
		catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

		async Task<Boolean> RefreshContentHashAsync()
		{
			if(await UpdateContentIntegrityHashAsync(cancel:cancel).ConfigureAwait(false)) { return ApplyAssertionState(DataSecurityCNKey,security); }

			return false;
		}
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
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataSetItem item)
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
            if(this.Content is not null)
            {
                foreach(var i in this.Content) { await i.WipeDataCore(security,visited,cancel).ConfigureAwait(false); }

                    this.Content.Clear(); this.Content = null; this.ContentIndex = null;
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
                foreach (var _ in this.FieldIntegrityHashes.Values) { ZeroMemory(_); }

                this.FieldIntegrityHashes.Clear(); this.FieldIntegrityHashes = null;
            }

            if(this.FieldAssertions is not null)
            {
                foreach (var _ in this.FieldAssertions.Values) { ZeroMemory(_); }

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
                    foreach(var i in this.Content)
                    {
                        await i.WipeDataInMemoryOnlyCore(security,visited,true,cancel).ConfigureAwait(false);
                    }

                    this.Content.Clear(); this.Content = null; this.ContentIndex = null;
                }

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataSetItem.xml' path='DataSetItem/class[@name="DataSetItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}