namespace KusDepot;

/**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.DataStreamItem")]
[DataContract(Name = "DataStreamItem" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed class DataStreamItem : DataItem , IComparable<DataStreamItem> , IEquatable<DataStreamItem> , IExtensibleDataObject , IParsable<DataStreamItem>
{
    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/field[@name="Content"]/*'/>*/
    [NonSerialized]
    [IgnoreDataMember]
    private Stream? Content;

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/field[@name="ContentBindingMode"]/*'/>*/
    [NonSerialized] [IgnoreDataMember]
    private DataStreamItemContentBindingMode ContentBindingMode = DataStreamItemContentBindingMode.Static;

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/field[@name="LiveContentStatus"]/*'/>*/
    [NonSerialized] [IgnoreDataMember]
    private DataStreamItemLiveContentStatus LiveContentStatus = DataStreamItemLiveContentStatus.None;

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/field[@name="LiveContentFaultMessage"]/*'/>*/
    [NonSerialized] [IgnoreDataMember]
    private String? LiveContentFaultMessage;

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public DataStreamItem() : this(null,null,null,null,null) { }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/constructor[@name="Constructor"]/*'/>*/
    public DataStreamItem(Stream? content = null , Guid? id = null , String? name = null , IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetID(id); this.SetName(name); this.AddNotes(notes); this.AddTags(tags); this.SetDataType(type);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="AbortLiveContent"]/*'/>*/
    public Boolean AbortLiveContent() => this.SetLiveContentTerminalState(DataStreamItemLiveContentStatus.Aborted,null);

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="BindLiveContent"]/*'/>*/
    public Boolean BindLiveContent(Stream? content)
    {
        try
        {
            if(this.Locked) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.ContentBindingMode != DataStreamItemContentBindingMode.Static || content is null || content.CanRead is false) { return false; }

                this.Content = content;
                this.ContentStreamed = true;
                this.ClearCachedFileSize_NoSync();
                this.ContentBindingMode = DataStreamItemContentBindingMode.LiveSessionManaged;
                this.LiveContentStatus = DataStreamItemLiveContentStatus.Open;
                this.LiveContentFaultMessage = null;
                MN();

                return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BindLiveContentFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="CompleteLiveContent"]/*'/>*/
    public Boolean CompleteLiveContent() => this.SetLiveContentTerminalState(DataStreamItemLiveContentStatus.Completed,null);

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="FaultLiveContent"]/*'/>*/
    public Boolean FaultLiveContent(String? message = null)
    {
        return this.SetLiveContentTerminalState(DataStreamItemLiveContentStatus.Faulted,message);
    }

    ///<inheritdoc/>
    public override async Task<Boolean> CheckDataHash(CancellationToken cancel = default)
    {
        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged) { return false; }

                String? f = this.FILE;

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false || this.ContentStreamed is false) { return false; }

                using Stream s = OpenSequentialRead(f);

                return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override async Task<String?> SignData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || String.IsNullOrEmpty(field)) { return null; }

            if(String.Equals(field,DataSecurityMDKey,Ordinal)) { return await base.SignData(field,security,cancel).ConfigureAwait(false); }

            if(!String.Equals(field,DataSecurityCNKey,Ordinal)) { return null; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged || this.DataEncrypted) { return null; }

                String? f = this.FILE;

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false || this.ContentStreamed is false) { return null; }

                using Stream s = OpenSequentialRead(f);

                if(!await UpdateFieldIntegrityHashAsync(field,s,cancel).ConfigureAwait(false)) { return null; }

                if(!UpdateFieldAssertion(field,security)) { return null; }

                return field;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SignDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged) { return false; }

                String? f = this.FILE;

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false || this.ContentStreamed is false) { return false; }

                Byte[]? h = await DecryptFileToHash(f,security,cancel).ConfigureAwait(false); if(h is null) { return false; }

                if(!UpdateFieldIntegrityHash(DataSecurityCNKey,h,true)) { return false; }
                if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }
                if(!UpdateFieldIntegrityHash(DataSecurityEDKey,Array.Empty<Byte>())) { return false; }

                ClearAssertion(DataSecurityEDKey);
                this.SetCachedFileSize_NoSync(GetFileSize(f));
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
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged) { return false; }

                String? f = this.FILE;

                if(String.IsNullOrEmpty(f) || File.Exists(f) is false || this.ContentStreamed is false) { return false; }

                Byte[]? h = await EncryptFileToHash(f,security,cancel).ConfigureAwait(false); if(h is null) { return false; }

                if(!UpdateFieldIntegrityHash(DataSecurityEDKey,h,true)) { return false; }
                if(!ApplyAssertionState(DataSecurityEDKey,security)) { return false; }
                if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>())) { return false; }

                ClearAssertion(DataSecurityCNKey);
                this.SetCachedFileSize_NoSync(GetFileSize(f));
                this.DataEncrypted = true;
                this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);
                this.ClearHashCodeCache_NoSync();

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is DataStreamItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is DataStreamItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is DataStreamItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is DataStreamItem d ? this.CompareTo(d) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(DataStreamItem? other)
    {
        try
        {
            if(ReferenceEquals(this,other)) { return 0; }

            if(other is null) { return this.BornOn is null ? 0 : 1; }

            if(!TryAcquireMetaLocks(other,out LockState s)) { throw SyncException; }

            try
            {
                DateTimeOffset? _ = this.BornOn; DateTimeOffset? o = other.BornOn;

                if(_ is null && o is null) { return 0; }
                if(_ is not null && o is null) { return 1; }
                if(_ is null && o is not null) { return -1; }

                return _!.Value.CompareTo(o!.Value);
            }
            finally { ReleaseSyncLocks(s); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareToFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="op_Explicit"]/*'/>*/
    public static explicit operator DataStreamItem?(Stream? content) => new(content);

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(DataStreamItem? a , DataStreamItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(DataStreamItem? a , DataStreamItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as DataStreamItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as DataStreamItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as DataStreamItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as DataStreamItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as DataStreamItem); }

    ///<inheritdoc/>
    public Boolean Equals(DataStreamItem? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            if(!TryAcquireMetaLocks(other,out LockState s)) { return false; }

            try
            {
                return Nullable.Equals(this.ID,other.ID);
            }
            finally { ReleaseSyncLocks(s); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="FromFile"]/*'/>*/
    public static DataStreamItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            return OrleansUtility.FromFile<DataStreamItem>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="GetContent"]/*'/>*/
    public Stream? GetContent() => this.Content;

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="GetContentBindingMode"]/*'/>*/
    public DataStreamItemContentBindingMode GetContentBindingMode()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.ContentBindingMode; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentBindingModeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="GetContentStream"]/*'/>*/
    public override Stream? GetContentStream()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Content?.CanRead is true) { return this.Content; }

                if(this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE)) { return null; }

                if(File.Exists(this.FILE)) { try { return OpenSequentialRead(this.FILE); } catch {} }

                return null;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override Stream? GetContentStream_NoSync()
    {
        try
        {
            if(this.Content?.CanRead is true) { return this.Content; }

            if(this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE)) { return null; }

            if(File.Exists(this.FILE)) { try { return OpenSequentialRead(this.FILE); } catch {} }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return null; } throw; }
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
                String? size = null;

                if(this.ContentBindingMode == DataStreamItemContentBindingMode.LiveSessionManaged && this.LiveContentStatus == DataStreamItemLiveContentStatus.Open)
                {
                    if(this.Content?.CanSeek is true)
                    {
                        try { size = this.Content.Length.ToStringInvariant(); } catch { size = null; }
                    }
                }
                else if(this.ContentStreamed && !String.IsNullOrEmpty(this.FILE))
                {
                    size = this.ResolveCachedFileSize_NoSync().ToStringInvariant();
                }
                else if(this.Content?.CanSeek is true)
                {
                    try { size = this.Content.Length.ToStringInvariant(); } catch { size = null; }
                }

                if(DataIsolation.IsEnabled())
                {
                    return new()
                    {
                        Application = this.Application is null ? null : new(this.Application),
                        ApplicationVersion = this.ApplicationVersion?.ToString(),
                        BornOn = this.BornOn?.ToString("O"),
                        ContentStreamed = this.ContentStreamed,
                        DataType = this.DataType is null ? null : new(this.DataType),
                        DistinguishedName = this.DistinguishedName is null ? null : new(this.DistinguishedName),
                        FILE = this.FILE is null ? null : new(this.FILE),
                        ID = id,
                        Modified = this.Modified?.ToString("O"),
                        Name = this.Name is null ? null : new(this.Name),
                        Notes = this.Notes?.Select(n => new String(n)).ToHashSet(),
                        ObjectFILE = this.ObjectFILE is null ? null : new(this.ObjectFILE),
                        ObjectType = this.GetCachedInheritanceList(),
                        ServiceVersion = this.ServiceVersion?.ToString(),
                        Size = size,
                        Tags = this.Tags?.Select(t => new String(t)).ToHashSet(),
                        Version = this.Version?.ToString()
                    };
                }

                return new()
                {
                    Application = this.Application,
                    ApplicationVersion = this.ApplicationVersion?.ToString(),
                    BornOn = this.BornOn?.ToString("O"),
                    ContentStreamed = this.ContentStreamed,
                    DataType = this.DataType,
                    DistinguishedName = this.DistinguishedName,
                    FILE = this.FILE,
                    ID = id,
                    Modified = this.Modified?.ToString("O"),
                    Name = this.Name,
                    Notes = this.Notes,
                    ObjectFILE = this.ObjectFILE,
                    ObjectType = this.GetCachedInheritanceList(),
                    ServiceVersion = this.ServiceVersion?.ToString(),
                    Size = size,
                    Tags = this.Tags,
                    Version = this.Version?.ToString()
                };
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDescriptorFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return null; } throw; }
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
            return this.ID.HasValue ? this.ID.Value.GetHashCode() : 0;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try { return Task.FromResult(this.GetHashCode_NoSync()); } catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="GetLiveContentStatus"]/*'/>*/
    public DataStreamItemLiveContentStatus GetLiveContentStatus()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.LiveContentStatus; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetLiveContentStatusFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="GetLiveContentFaultMessage"]/*'/>*/
    public String? GetLiveContentFaultMessage()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.LiveContentFaultMessage is null ? null : new(this.LiveContentFaultMessage); }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetLiveContentFaultMessageFail,MyTypeName,MyID); throw; }
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

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static DataStreamItem IParsable<DataStreamItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="Parse"]/*'/>*/
    public static new DataStreamItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            return OrleansUtility.ParseBase64<DataStreamItem>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(Stream? content)
    {
        try
        {
            if(this.Locked) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged) { return false; }

                if(content is null || content.CanRead is false)
                {
                    this.Content = null; this.ContentStreamed = false; this.ClearCachedFileSize_NoSync(); this.LiveContentStatus = DataStreamItemLiveContentStatus.None; this.LiveContentFaultMessage = null; MN();

                    return false;
                }

                this.Content = content; this.ContentStreamed = true; this.ClearCachedFileSize_NoSync(); this.LiveContentStatus = DataStreamItemLiveContentStatus.None; this.LiveContentFaultMessage = null; MN();

                if(content.CanSeek)
                {
                    try { this.SetCachedFileSize_NoSync(content.Length); } catch { this.ClearCachedFileSize_NoSync(); }
                }

                return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="SetContentStreamed"]/*'/>*/
    public override async Task<Boolean> SetContentStreamed(Boolean streamed , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            if(this.Locked) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged) { return false; }

                if(!streamed)
                {
                    if(this.Content?.CanRead is true) { return false; }

                    this.ContentStreamed = false; this.ClearCachedFileSize_NoSync(); this.LiveContentStatus = DataStreamItemLiveContentStatus.None; this.LiveContentFaultMessage = null; MN();

                    return true;
                }

                Stream? content = this.Content;

                if(content?.CanRead is true)
                {
                    this.ContentStreamed = true; this.ClearCachedFileSize_NoSync(); this.LiveContentStatus = DataStreamItemLiveContentStatus.None; this.LiveContentFaultMessage = null; MN();

                    if(content.CanSeek)
                    {
                        try { this.SetCachedFileSize_NoSync(content.Length); } catch { this.ClearCachedFileSize_NoSync(); }
                    }

                    return true;
                }

                String? file = this.FILE;

                if(String.IsNullOrEmpty(file) || File.Exists(file) is false)
                {
                    this.ContentStreamed = false; this.ClearCachedFileSize_NoSync(); this.LiveContentStatus = DataStreamItemLiveContentStatus.None; this.LiveContentFaultMessage = null; MN();

                    return false;
                }

                this.ContentStreamed = true; this.SetCachedFileSize_NoSync(GetFileSize(file)); this.LiveContentStatus = DataStreamItemLiveContentStatus.None; this.LiveContentFaultMessage = null; MN();

                return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetContentStreamedFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean SetFILE(String? file)
    {
        Boolean lk = false;

        try
        {
            if(file is null || this.Locked) { return false; }

            this.AcquireLocks(); lk = true;

            try
            {
                if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged) { return false; }

                String? _ = String.IsNullOrEmpty(file) ? null : file;

                if(String.Equals(this.FILE,_,Ordinal)) { return true; }

                this.FILE = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _;

                this.SetCachedFileSize_NoSync(_ is null ? null : GetFileSize(_));

                this.ContentStreamed = _ is not null || this.Content?.CanRead is true;

                this.LiveContentStatus = DataStreamItemLiveContentStatus.None;

                this.LiveContentFaultMessage = null;

                MN();

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetFILEFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/method[@name="SetLiveContentTerminalState"]/*'/>*/
    private Boolean SetLiveContentTerminalState(DataStreamItemLiveContentStatus status , String? message)
    {
        try
        {
            if(this.Locked) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.ContentBindingMode != DataStreamItemContentBindingMode.LiveSessionManaged || this.LiveContentStatus != DataStreamItemLiveContentStatus.Open) { return false; }

                this.LiveContentStatus = status;
                this.LiveContentFaultMessage = status == DataStreamItemLiveContentStatus.Faulted ? message : null;
                this.ClearCachedFileSize_NoSync();
                MN();

                return true;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetLiveContentTerminalStateFail,MyTypeName,MyID); if(NoExceptions || MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataStreamItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            return OrleansUtility.TryParseBase64(input,out item);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    ///<inheritdoc/>
    public override async Task<Boolean> VerifyData(String? field , DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || String.IsNullOrEmpty(field)) { return false; }

            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                switch(field)
                {
                    case DataSecurityMDKey:
                    {
                        return await VerifyDataCore(field,security,null,null,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityCNKey:
                    {
                        if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged || this.DataEncrypted) { return false; }

                        String? f = this.FILE;

                        if(String.IsNullOrEmpty(f) || File.Exists(f) is false || this.ContentStreamed is false) { return false; }

                        using Stream s = OpenSequentialRead(f);

                        return await VerifyDataCore(field,security,null,s,cancel).ConfigureAwait(false);
                    }

                    case DataSecurityEDKey:
                    {
                        if(this.ContentBindingMode is DataStreamItemContentBindingMode.LiveSessionManaged || this.DataEncrypted is false) { return false; }

                        String? f = this.FILE;

                        if(String.IsNullOrEmpty(f) || File.Exists(f) is false || this.ContentStreamed is false) { return false; }

                        using Stream s = OpenSequentialRead(f);

                        return await VerifyDataCore(field,security,null,s,cancel).ConfigureAwait(false);
                    }

                    default: return false;
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataStreamItem.xml' path='DataStreamItem/class[@name="DataStreamItem"]/property[@name="ExtensionData"]/*'/>*/
    [JsonIgnore] public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}