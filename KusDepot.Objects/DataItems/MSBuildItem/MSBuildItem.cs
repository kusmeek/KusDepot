namespace KusDepot;

/**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.MSBuildItem")]
[DataContract(Name = "MSBuildItem" , Namespace = "KusDepot")]

public sealed partial class MSBuildItem : DataItem , IComparable<MSBuildItem> , IEquatable<MSBuildItem> , IExtensibleDataObject , IParsable<MSBuildItem>
{
    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/content[@name="Content"]/*'/>*/
    [JsonPropertyName("Content")] [JsonInclude]
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal String? Content;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Requirements"]/*'/>*/
    [JsonPropertyName("Requirements")] [JsonInclude]
    [DataMember(Name = "Requirements" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    internal HashSet<DataItem>? Requirements;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Sequence"]/*'/>*/
    [JsonPropertyName("Sequence")] [JsonInclude]
    [DataMember(Name = "Sequence" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    internal SortedList<Int32,MSBuildItem>? Sequence;

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

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="CaptureContentIntegritySnapshot_NoSync"]/*'/>*/
    internal override ContentIntegritySnapshot CaptureContentIntegritySnapshot_NoSync()
    {
        if(this.DataEncrypted)
        {
            if(this.ContentStreamed)
            {
                Stream? encryptedStream = this.GetContentStream_NoSync();

                if(encryptedStream is null) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

                ContentIntegrityValue encryptedContentStream = ContentIntegrityValue.FromStream(encryptedStream);

                return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContentStream)],[encryptedContentStream]);
            }

            ContentIntegrityValue encryptedContent = ContentIntegrityValue.FromBytes(this.EncryptedData);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContent)],[encryptedContent]);
        }

        Object? content = this.Content;

        KeyValuePair<Int32,MSBuildItem>[]? sequence = this.Sequence?.Select(entry => new KeyValuePair<Int32,MSBuildItem>(entry.Key,entry.Value)).ToArray();

        if(this.ContentStreamed)
        {
            Stream? contentStream = this.GetContentStream_NoSync();

            if(contentStream is null) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

            content = ContentIntegrityValue.FromStream(contentStream);

            return new ContentIntegritySnapshot(
                [
                    new KeyValuePair<String,Object?>("Content",content),
                    new KeyValuePair<String,Object?>("Requirements",this.Requirements),
                    new KeyValuePair<String,Object?>("Sequence",sequence)
                ],
                [(IDisposable)content]);
        }

        return new ContentIntegritySnapshot(
            [
                new KeyValuePair<String,Object?>("Content",content),
                new KeyValuePair<String,Object?>("Requirements",this.Requirements),
                new KeyValuePair<String,Object?>("Sequence",sequence)
            ]);
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="CaptureContentMutationMemento_NoSync"]/*'/>*/
    internal override DataItemContentMutationMemento CaptureContentMutationMemento_NoSync()
    {
        String? originalContent = this.Content;
        HashSet<DataItem>? originalRequirements = this.Requirements;
        SortedList<Int32,MSBuildItem>? originalSequence = this.Sequence;

        return new DataItemContentMutationMemento(item =>
        {
            MSBuildItem msbuild = (MSBuildItem)item;
            msbuild.Content = originalContent;
            msbuild.Requirements = originalRequirements;
            msbuild.Sequence = originalSequence;
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
                    if(this.ContentStreamed)
                    {
                        var f = this.GetFILE(); if(File.Exists(f) is false) { return false; } using Stream s = OpenSequentialRead(f);

                        return await CheckDataHashCore(null,s,cancel).ConfigureAwait(false);
                    }

                    if(this.EncryptedData is null) { return false; }

                    return await CheckDataHashCore(this.EncryptedData,null,cancel).ConfigureAwait(false);
                }

                if(this.Content is null && this.Requirements is null && this.Sequence is null && !this.ContentStreamed) { return false; }

                return await CheckDataHashCoreHashed(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
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

            if(!TryAcquireSyncLocks(other,out LockState s)) { return false; }

            try
            {
                var _req = this.Requirements; var __req = other.Requirements;
                String? _con = this.Content; String? __con = other.Content;
                var _seq = this.Sequence; var __seq = other.Sequence;

                Boolean _ce = _con is not null; Boolean __ce = __con is not null; if(_ce != __ce) { return false; }
                Boolean _re = _req is not null; Boolean __re = __req is not null; if(_re != __re) { return false; }
                Boolean _se = _seq is not null; Boolean __se = __seq is not null; if(_se != __se) { return false; }

                if(_ce is false && _re is false && _se is false) { return true; }

                if(_ce && _con.AsSpan().SequenceEqual(__con.AsSpan()) is false) { return false; }

                if(_se && _seq!.SequenceEqual(__seq!) is false) { return false; }

                if(_re && _req!.SetEquals(__req!) is false) { return false; }

                return true;
            }
            finally { ReleaseSyncLocks(s); }
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

            return OrleansUtility.FromFile<MSBuildItem>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetContent"]/*'/>*/
    public String? GetContent()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.Content is null ? null : DataIsolation.IsEnabled() ? new(this.Content) : this.Content; }

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
                    MSBuildItem? decrypted = Deserialize<MSBuildItem>(m); if(decrypted is null) { return null; }

                    return new()
                    {
                        MSBuild = new Dictionary<String,Object?>
                        {
                            {"Content", decrypted.Content},
                            {"Requirements", decrypted.Requirements},
                            {"Sequence", decrypted.Sequence}
                        }
                    };
                }

                if(this.Content is null && this.Requirements is null && this.Sequence is null) { return null; }

                if(DataIsolation.IsEnabled())
                {
                    HashSet<DataItem>? requirements = await CloneRequirementsAsync(this.Requirements,cancel).ConfigureAwait(false);
                    SortedList<Int32,MSBuildItem>? sequence = await CloneSequenceAsync(this.Sequence,cancel).ConfigureAwait(false);

                    return new()
                    {
                        MSBuild = new Dictionary<String,Object?>
                        {
                            {"Content", this.Content is null ? null : new String(this.Content)},
                            {"Requirements", requirements},
                            {"Sequence", sequence}
                        }
                    };
                }

                return new()
                {
                    MSBuild = new Dictionary<String,Object?>
                    {
                        {"Content", this.Content},
                        {"Requirements", this.Requirements},
                        {"Sequence", this.Sequence}
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
                        Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : (this.Content?.Length * 2)).ToStringInvariant(),
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
                    Size               = (this.ContentStreamed ? this.ResolveCachedFileSize_NoSync() : (this.Content?.Length * 2)).ToStringInvariant(),
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
            using IncrementalHash h = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

            Byte[] b = ArrayPool<Byte>.Shared.Rent(DataHashingBufferSize * sizeof(Char));

            try
            {
                ReadOnlySpan<Char> c = this.Content.AsSpan();

                while(!c.IsEmpty)
                {
                    Int32 n = Math.Min(DataHashingBufferSize,c.Length); Int32 e = Encoding.Unicode.GetBytes(c[..n],b);

                    h.AppendData(b.AsSpan(0,e)); c = c[n..];
                }

                var r = this.Requirements?.Order();

                if(r is not null)
                {
                    foreach(var _ in r) { h.AppendData(BitConverter.GetBytes(_.GetHashCode())); }
                }

                if(this.Sequence is not null)
                {
                    foreach(var s in this.Sequence)
                    {
                        h.AppendData(BitConverter.GetBytes(s.Key)); h.AppendData(BitConverter.GetBytes(s.Value?.GetHashCode() ?? 0));
                    }
                }

                return BitConverter.ToInt32(h.GetHashAndReset(),0);
            }
            finally { ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override async Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            var requirements = this.Requirements?.Order()?.ToArray();

            using IncrementalHash h = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

            Byte[] b = ArrayPool<Byte>.Shared.Rent(DataHashingBufferSize * sizeof(Char));

            try
            {
                ReadOnlySpan<Char> c = this.Content.AsSpan();

                while(!c.IsEmpty)
                {
                    cancel.ThrowIfCancellationRequested();

                    Int32 n = Math.Min(DataHashingBufferSize,c.Length); Int32 e = Encoding.Unicode.GetBytes(c[..n],b);

                    h.AppendData(b.AsSpan(0,e)); c = c[n..];
                }

                if(requirements is not null)
                {
                    foreach(var r in requirements)
                    {
                        cancel.ThrowIfCancellationRequested();

                        h.AppendData(BitConverter.GetBytes(await r.GetHashCodeAsync(cancel).ConfigureAwait(false)));
                    }
                }

                if(this.Sequence is not null)
                {
                    foreach(var s in this.Sequence)
                    {
                        cancel.ThrowIfCancellationRequested();

                        h.AppendData(BitConverter.GetBytes(s.Key));

                        if(s.Value is null) { continue; }

                        h.AppendData(BitConverter.GetBytes(await s.Value.GetHashCodeAsync(cancel).ConfigureAwait(false)));
                    }
                }

                return BitConverter.ToInt32(h.GetHashAndReset(),0);
            }
            finally { ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }
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
                if(DataIsolation.IsEnabled())
                {
                    if(!this.Requirements.TryCloneDistinctByID(out var o)) { return null; }

                    if(o.Count != this.Requirements.Count) { return null; } return o;
                }

                return this.Requirements;
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
                if(DataIsolation.IsEnabled())
                {
                    SortedList<Int32,MSBuildItem> __ = new(this.Sequence.ToDictionary(_=>_.Key,_=>(_.Value.Clone() as MSBuildItem)!));

                    if(__.Count != this.Sequence.Count) { return null; } return __;
                }

                return this.Sequence;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetSequenceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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
                    && this.Requirements is null
                    && this.Sequence is null
                    && this.EncryptedData is null;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsMetadataOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
            return OrleansUtility.ParseBase64<MSBuildItem>(input);
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

        MSBuildItem? decrypted = Deserialize<MSBuildItem>(output); if(decrypted is null) { return null; }

        String? content = decrypted.Content is null ? null : new String(decrypted.Content);

        HashSet<DataItem>? requirements = await CloneRequirementsAsync(decrypted.Requirements,cancel).ConfigureAwait(false);

        if(decrypted.Requirements is not null && requirements is null) { return null; }

        SortedList<Int32,MSBuildItem>? sequence = await CloneSequenceAsync(decrypted.Sequence,cancel).ConfigureAwait(false);

        if(decrypted.Sequence is not null && sequence is null) { return null; }

        String? originalContent = this.Content;

        HashSet<DataItem>? originalRequirements = this.Requirements;

        SortedList<Int32,MSBuildItem>? originalSequence = this.Sequence;

        return new DataItemProtectionChange(
            () => { this.Content = content; this.Requirements = requirements; this.Sequence = sequence; return ValueTask.CompletedTask; },
            () => { this.Content = originalContent; this.Requirements = originalRequirements; this.Sequence = originalSequence; return ValueTask.CompletedTask; });
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareEncryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed)
        {
            if(this.Content is not null || this.Requirements is not null || this.Sequence is not null) { return null; }

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

        Byte[]? serialized = this.Content is not null || this.Requirements is not null || this.Sequence is not null ? this.Serialize_NoSync() : null;

        try
        {
            DataItemSecurityEnvelopeArrayHashResult? encrypted = await this.EncryptDataBufferWithHash(serialized,security,cancel).ConfigureAwait(false);

            if(!encrypted.HasValue) { return null; }

            String? originalContent = this.Content;

            HashSet<DataItem>? originalRequirements = this.Requirements;

            SortedList<Int32,MSBuildItem>? originalSequence = this.Sequence;

            MSBuildItem wipeTarget = new() { Content = originalContent, Requirements = originalRequirements, Sequence = originalSequence };

            return new DataItemProtectionChange(
                () => { this.Content = null; this.Requirements = null; this.Sequence = null; return ValueTask.CompletedTask; },
                () => { this.Content = originalContent; this.Requirements = originalRequirements; this.Sequence = originalSequence; return ValueTask.CompletedTask; },
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

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="RefreshContentIntegrityState_NoSync"]/*'/>*/
    private Boolean RefreshContentIntegrityState_NoSync(DataItemSecurityContext? security)
    {
        Boolean hasContent = this.ContentStreamed || this.Content is not null || this.Requirements is { Count: > 0 } || this.Sequence is { Count: > 0 };

        Boolean updated = hasContent ? UpdateContentIntegrityHash() : UpdateContentIntegrityHash(true);

        if(updated && ApplyAssertionState(DataSecurityCNKey,security)) { return true; }

        UpdateContentIntegrityHash(true);

        ClearAssertion(DataSecurityCNKey);

        return false;
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="RefreshContentIntegrityStateAsync"]/*'/>*/
    private async Task<Boolean> RefreshContentIntegrityStateAsync(DataItemSecurityContext? security , CancellationToken cancel)
    {
        Boolean hasContent = this.ContentStreamed || this.Content is not null || this.Requirements is { Count: > 0 } || this.Sequence is { Count: > 0 };

        Boolean updated = hasContent ? await UpdateContentIntegrityHashAsync(cancel:cancel).ConfigureAwait(false) : await UpdateContentIntegrityHashAsync(clear:true,cancel:cancel).ConfigureAwait(false);

        if(updated && ApplyAssertionState(DataSecurityCNKey,security)) { return true; }

        await UpdateContentIntegrityHashAsync(clear:true,cancel:cancel).ConfigureAwait(false);

        ClearAssertion(DataSecurityCNKey);

        return false;
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(String? content , DataItemSecurityContext? security = null)
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

                    if(String.IsNullOrEmpty(content)) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData, security); if(db is null) { return false; }

                    var di = Deserialize(db) as MSBuildItem; ZeroMemory(db); if(di is null)            { return false; }

                    di.Content = content;

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Requirements = null; this.Sequence = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateContentIntegrityHash(true))                                        { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });
                    return true;
                }
                else
                {
                    if(String.IsNullOrEmpty(content))
                    {
                        this.Content = null;

                        if(RefreshContentIntegrityState_NoSync(security))
                        {
                            if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }
                            mutation.Commit(() => { MN(); });
                            return true;
                        }
                        else { return false; }
                    }

                    this.Content = DataIsolation.IsEnabled() ? new(content) : content;

                    if(RefreshContentIntegrityState_NoSync(security))
                    {
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

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetRequirements"]/*'/>*/
    public Boolean SetRequirements(IEnumerable<DataItem>? requirements , DataItemSecurityContext? security = null)
    {
        try
        {
            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.ContentStreamed || security is null) { return false; }

                    HashSet<DataItem>? nc = null;

                    if(requirements is not null)
                    {
                        if(!requirements.TryDistinctByID(out nc)) { return false; } if(nc.Count == 0) { nc = null; }
                    }

                    var db = this.DecryptDataBuffer(this.EncryptedData, security); if(db is null) { return false; }

                    var di = Deserialize(db) as MSBuildItem; ZeroMemory(db); if(di is null)       { return false; }

                    di.Requirements = nc;

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Requirements = null; this.Sequence = null; MN();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateContentIntegrityHash(true))                                        { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    return true;
                }
                else
                {
                    if( requirements is null ) { return false; }

                    if(!requirements.TryDistinctByID(out var nc) || nc.Any(_ => ReferenceEquals(_,this))) { return false; }

                    if(DataIsolation.IsEnabled())
                    {
                        if(!nc.TryCloneDistinctByID(out var __)) { return false; }

                        if(__.Count == 0) { this.Requirements = null; MN(); return RefreshContentIntegrityState_NoSync(security); }

                        this.Requirements = __; MN(); return RefreshContentIntegrityState_NoSync(security);
                    }

                    if(nc.Count == 0) { this.Requirements = null; MN(); return RefreshContentIntegrityState_NoSync(security); }

                    this.Requirements = nc; MN(); return RefreshContentIntegrityState_NoSync(security);
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetRequirementsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetSequence"]/*'/>*/
    public Boolean SetSequence(IDictionary<Int32,MSBuildItem>? sequence , DataItemSecurityContext? security = null)
    {
        try
        {
            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.DataEncrypted)
                {
                    if(this.ContentStreamed || security is null) { return false; }

                    var db = this.DecryptDataBuffer(this.EncryptedData,security); if(db is null) { return false; }

                    var di = Deserialize(db) as MSBuildItem; ZeroMemory(db); if(di is null)           { return false; }

                    di.Sequence = sequence is null ? null : new SortedList<Int32,MSBuildItem>(sequence);

                    var eb = EncryptDataBuffer(di.Serialize(),security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null; this.Requirements = null; this.Sequence = null; MN();

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateContentIntegrityHash(true))                                        { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);
                    return true;
                }
                else
                {
                    if( sequence is null ) { return false; }

                    SortedList<Int32,MSBuildItem> _ = new(sequence); if(_.Any(_ => ReferenceEquals(_.Value,this))) { return false; }

                    if(DataIsolation.IsEnabled())
                    {
                        SortedList<Int32,MSBuildItem> __ = new(_.ToDictionary(_=>_.Key,_=>(_.Value.Clone() as MSBuildItem)!,null));

                        if(__.Count == 0) { this.Sequence = null; MN(); return RefreshContentIntegrityState_NoSync(security); }

                        if(_.Count != __.Count) { return false; }

                        this.Sequence = __; MN(); return RefreshContentIntegrityState_NoSync(security);
                    }

                    if(_.Count == 0) { this.Sequence = null; MN(); return RefreshContentIntegrityState_NoSync(security); }

                    this.Sequence = _; MN(); return RefreshContentIntegrityState_NoSync(security);
                }
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetSequenceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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

                        if(this.Content is null && this.Requirements is not { Count: > 0 } && this.Sequence is not { Count: > 0 }) { this.ClearAssertion(DataSecurityCNKey); }

						Boolean success = await RefreshContentHashAsync().ConfigureAwait(false);
						if(success) { MN(); committed = true; }

						return success;
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

					if(await RefreshContentIntegrityStateAsync(security,cancel).ConfigureAwait(false))
					{
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
			return await RefreshContentIntegrityStateAsync(security,cancel).ConfigureAwait(false);
		}
	}

    ///<inheritdoc/>
    public override async Task<String?> SignData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        if(String.Equals(field,DataSecurityCNKey,Ordinal))
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(this.Content is null && this.ContentStreamed is false && this.Requirements is not { Count: > 0 } && this.Sequence is not { Count: > 0 }) { return null; }
            }
            finally { this.Sync.Data.Release(); }
        }

        return await base.SignData(field,security,cancel).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out MSBuildItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            return OrleansUtility.TryParseBase64(input,out item);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="TryPrepareContentIntegrityState"]/*'/>*/
    private protected override Boolean TryPrepareContentIntegrityState(out Byte[]? digest)
    {
        Boolean hasContent = this.ContentStreamed || this.Content is not null || this.Requirements is { Count: > 0 } || this.Sequence is { Count: > 0 };

        digest = hasContent ? this.GetIntegrityHash_NoSync() : null;

        return hasContent;
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
                        if(this.Content is null && this.Requirements is null && this.Sequence is null && !this.ContentStreamed) { return false; }

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
    protected internal override async Task<Boolean> WipeDataCore(DataItemSecurityContext? security, HashSet<Object> visited, CancellationToken cancel)
    {
        if(await base.WipeDataCore(security,visited,cancel).ConfigureAwait(false)) { return true; }

        if(this.Locked && this.CheckManager(security) is false) { return false; }

        Boolean lk = false; await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true;

        try
        {
            ZeroMemory(this.Content); this.Content = null;

            if(this.Requirements is not null)
            {
                foreach(var item in this.Requirements) { await item.WipeDataCore(security, visited, cancel).ConfigureAwait(false); }

                this.Requirements.Clear(); this.Requirements = null;
            }

            if(this.Sequence is not null)
            {
                foreach(var item in this.Sequence.Values) { await item.WipeDataCore(security, visited, cancel).ConfigureAwait(false); }

                this.Sequence.Clear(); this.Sequence = null;
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
    protected internal override async Task<Boolean> WipeDataInMemoryOnlyCore(DataItemSecurityContext? security , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel)
    {
        Boolean lk = false;
        try
        {
            if(await base.WipeDataInMemoryOnlyCore(security,visited,requirelocks,cancel).ConfigureAwait(false)) { return true; }

            if(this.Locked && this.CheckManager(security) is false) { return false; }

            if(requirelocks) { await this.AcquireLocksAsync(cancel).ConfigureAwait(false); lk = true; }

            try
            {
                ZeroMemory(this.Content); this.Content = null;

                if(this.Requirements is not null)
                {
                    foreach(var _ in this.Requirements) { await _.WipeDataInMemoryOnlyCore(security,visited,true,cancel).ConfigureAwait(false); }

                    this.Requirements.Clear(); this.Requirements = null;
                }

                if(this.Sequence is not null)
                {
                    foreach(var _ in this.Sequence.Values) { await _.WipeDataInMemoryOnlyCore(security,visited,true,cancel).ConfigureAwait(false); }

                    this.Sequence.Clear(); this.Sequence = null;
                }

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="CloneRequirementsAsync"]/*'/>*/
    private static async Task<HashSet<DataItem>?> CloneRequirementsAsync(HashSet<DataItem>? requirements , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(requirements is null) { return null; }

        if(!requirements.TryCloneDistinctByID(out HashSet<DataItem>? cloned)) { return null; }

        return cloned;
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="CloneSequenceAsync"]/*'/>*/
    private static async Task<SortedList<Int32,MSBuildItem>?> CloneSequenceAsync(SortedList<Int32,MSBuildItem>? sequence , CancellationToken cancel)
    {
        if(sequence is null) { return null; }

        SortedList<Int32,MSBuildItem> cloned = new(sequence.Count);

        foreach(KeyValuePair<Int32,MSBuildItem> step in sequence)
        {
            cancel.ThrowIfCancellationRequested();
            MSBuildItem? clone = step.Value.Clone() as MSBuildItem; if(clone is null) { return null; }
            cloned.Add(step.Key,clone);
        }

        await Task.CompletedTask.ConfigureAwait(false);

        return cloned;
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}