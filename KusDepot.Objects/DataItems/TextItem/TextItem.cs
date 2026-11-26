namespace KusDepot;

/**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.TextItem")]
[DataContract(Name = "TextItem" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed partial class TextItem : DataItem , IComparable<TextItem> , IEquatable<TextItem> , IExtensibleDataObject , IParsable<TextItem>
{
    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/content[@name="Content"]/*'/>*/
    [JsonPropertyName("Content")] [JsonInclude]
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    internal String? Content;

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/field[@name="Language"]/*'/>*/
    [JsonPropertyName("Language")] [JsonInclude]
    [DataMember(Name = "Language" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    internal String? Language;

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public TextItem() : this(null,null,null,null,null,null,null) {}

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/constructor[@name="Constructor"]/*'/>*/
    public TextItem(String? content = null , String? file = null , Guid? id = null , String? name = null , String? language = null, IEnumerable<String>? notes = null , IEnumerable<String>? tags = null , String? type = null)
    {
        try
        {
            this.Sync = null!; this.Initialize(); this.SetContent(content); this.SetFILE(file); this.SetID(id); this.SetName(name); this.SetLanguage(language); this.AddNotes(notes); this.AddTags(tags); this.SetDataType(type);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="CaptureContentIntegritySnapshot_NoSync"]/*'/>*/
    internal override ContentIntegritySnapshot CaptureContentIntegritySnapshot_NoSync()
    {
        if(this.ContentStreamed)
        {
            Stream? stream = this.GetContentStream_NoSync();

            if(stream is null) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

            ContentIntegrityValue contentStream = ContentIntegrityValue.FromStream(stream);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",contentStream)],[contentStream]);
        }

        if(this.DataEncrypted)
        {
            ContentIntegrityValue encryptedContent = ContentIntegrityValue.FromBytes(this.EncryptedData);

            return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",encryptedContent)],[encryptedContent]);
        }

        return new ContentIntegritySnapshot([new KeyValuePair<String,Object?>("Content",this.Content)]);
    }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="CaptureContentMutationMemento_NoSync"]/*'/>*/
    internal override DataItemContentMutationMemento CaptureContentMutationMemento_NoSync()
    {
        String? originalContent = this.Content;

        return new DataItemContentMutationMemento(item => { ((TextItem)item).Content = originalContent; });
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

                if(this.Content is null && this.ContentStreamed is false) { return false; }

                return await CheckDataHashCoreHashed(DataSecurityCNKey,await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false),cancel).ConfigureAwait(false);
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(IDataItem? other) { return other is TextItem t ? this.CompareTo(t) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(DataItem? other) { return other is TextItem t ? this.CompareTo(t) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is TextItem t ? this.CompareTo(t) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is TextItem t ? this.CompareTo(t) : base.CompareTo(other); }

    ///<inheritdoc/>
    public Int32 CompareTo(TextItem? other)
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

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="op_Explicit_Content"]/*'/>*/
    public static explicit operator String?(TextItem? item) => item?.GetContent();

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="op_Explicit"]/*'/>*/
    public static explicit operator TextItem?(String? content) => new TextItem(content);

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(TextItem? a , TextItem? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(TextItem? a , TextItem? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(IDataItem? other) { return this.Equals(other as TextItem); }

    ///<inheritdoc/>
    public override Boolean Equals(DataItem? other) { return this.Equals(other as TextItem); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as TextItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as TextItem); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as TextItem); }

    ///<inheritdoc/>
    public Boolean Equals(TextItem? other)
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

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="FromFile"]/*'/>*/
    public static TextItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            return OrleansUtility.FromFile<TextItem>(path);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="GetContent"]/*'/>*/
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

                    return new() { Text = new(GetUtf16String(m)) };
                }

                return this.Content is not null ? new() { Text = DataIsolation.IsEnabled() ? new(this.Content) : this.Content } : null;
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
                        Language           = this.Language is null ? null : new(this.Language),
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
                    Language           = this.Language,
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
                ReadOnlySpan<Char> s = this.Content.AsSpan();

                while(!s.IsEmpty)
                {
                    Int32 c = Math.Min(DataHashingBufferSize,s.Length); Int32 n = Encoding.Unicode.GetBytes(s[..c],b);

                    h.AppendData(b.AsSpan(0,n)); s = s[c..];
                }

                return BitConverter.ToInt32(h.GetHashAndReset(),0);
            }
            finally { ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    protected override Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        try
        {
            using IncrementalHash h = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

            Byte[] b = ArrayPool<Byte>.Shared.Rent(DataHashingBufferSize * sizeof(Char));

            try
            {
                ReadOnlySpan<Char> s = this.Content.AsSpan();

                while(!s.IsEmpty)
                {
                    cancel.ThrowIfCancellationRequested();

                    Int32 c = Math.Min(DataHashingBufferSize,s.Length); Int32 n = Encoding.Unicode.GetBytes(s[..c],b);

                    h.AppendData(b.AsSpan(0,n)); s = s[c..];
                }

                return Task.FromResult(BitConverter.ToInt32(h.GetHashAndReset(),0));
            }
            finally { ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="GetLanguage"]/*'/>*/
    public String? GetLanguage()
    {
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try { return this.Language is null ? null : DataIsolation.IsEnabled() ? new(this.Language) : this.Language; }

            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetLanguageFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.GetSyncNode(); this.RefreshSyncNodes(); }

    ///<inheritdoc/>
    static TextItem IParsable<TextItem>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="Parse"]/*'/>*/
    public static new TextItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            return OrleansUtility.ParseBase64<TextItem>(input);
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

        String? originalContent = this.Content;

        String candidate = GetUtf16String(output);

        return new DataItemProtectionChange(
            () => { this.Content = candidate; return ValueTask.CompletedTask; },
            () => { this.Content = originalContent; return ValueTask.CompletedTask; });
    }

    ///<inheritdoc/>
    private protected override async Task<DataItemProtectionChange?> PrepareEncryptionAsync(DataItemSecurityContext security , CancellationToken cancel)
    {
        if(this.ContentStreamed)
        {
            if(this.Content is not null) { return null; }

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

        String? originalContent = this.Content;

        DataItemSecurityEnvelopeArrayHashResult? encrypted = await this.EncryptDataBufferUtf16WithHash(originalContent,security,cancel).ConfigureAwait(false);

        if(!encrypted.HasValue) { return null; }

        return new DataItemProtectionChange(
            () => { this.Content = null; return ValueTask.CompletedTask; },
            () => { this.Content = originalContent; return ValueTask.CompletedTask; },encrypted.Value.Hash,encrypted.Value.Buffer,
            () => { ZeroMemory(originalContent); return ValueTask.CompletedTask; });
    }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="SetContent"]/*'/>*/
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

                    var eb = EncryptDataBufferUtf16(content,security); if(eb is null) { return false; }

                    this.EncryptedData = eb; this.Content = null;

                    if(!UpdateFieldIntegrityHash(DataSecurityEDKey,this.EncryptedData))           { return false; }
                    if(!ApplyAssertionState(DataSecurityEDKey,security))                          { return false; }
                    if(!UpdateFieldIntegrityHash(DataSecurityCNKey,Array.Empty<Byte>()))          { return false; }

                    this.DataProtectionInfo = BuildProtectionInfo(security,DataFieldState.EncryptedContent);

                    mutation.Commit(() => { MN(); });

                    return true;
                }
                else
                {
                    if(String.IsNullOrEmpty(content))
                    {
                        this.Content = null;

                        if(UpdateFieldIntegrityHash(DataSecurityCNKey,null))
                        {
                            if(!ApplyAssertionState(DataSecurityCNKey,security)) { return false; }

                            mutation.Commit(() => { MN(); });

                            return true;
                        }
                        else { return false; }
                    }

                    this.Content = DataIsolation.IsEnabled() ? new(content) : content;

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

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/method[@name="SetLanguage"]/*'/>*/
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
			if(await UpdateContentIntegrityHashAsync(clear:this.Content is null,cancel:cancel).ConfigureAwait(false)) { return ApplyAssertionState(DataSecurityCNKey,security); }

			return false;
		}
	}

    ///<inheritdoc/>
    public override async Task<String?> SignData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        if(String.Equals(field,DataSecurityCNKey,Ordinal))
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try { if(this.Content is null && this.ContentStreamed is false) { return null; } }

            finally { this.Sync.Data.Release(); }
        }

        return await base.SignData(field,security,cancel).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TextItem item)
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

                        Byte[] contentIntegrity = await this.GetIntegrityHash_NoSyncAsync(cancel).ConfigureAwait(false);
                        return await VerifyDataCoreHashed(field,security,contentIntegrity,cancel).ConfigureAwait(false);
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

                return true;
            }
            finally { if(lk) { this.ReleaseLocks(); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='TextItem.xml' path='TextItem/class[@name="TextItem"]/property[@name="ExtensionData"]/*'/>*/
    [JsonIgnore] public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}