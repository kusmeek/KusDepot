namespace KusDepot;

public abstract partial class DataItem
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CanChangeProtectionState"]/*'/>*/
    private protected virtual Boolean CanChangeProtectionState(DataItemSecurityContext security) => !this.Locked;

    ///<inheritdoc/>
    public async Task<Boolean> DecryptData(DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        if(security is null || this.DataEncrypted is false || !this.CanChangeProtectionState(security)) { return false; }

        Boolean acquired = false;

        try
        {
            await this.AcquireLocksAsync(cancel).ConfigureAwait(false); acquired = true;

            if(this.DataEncrypted is false || !this.CanChangeProtectionState(security)) { return false; }

            await using DataItemProtectionTransaction transaction = new(nameof(DecryptData));

            DataItemStateSnapshot original = new(this);

            DataItemProtectionChange? change = await this.PrepareDecryptionAsync(security,cancel).ConfigureAwait(false);

            if(change is null) { return false; }

            transaction.Enlist(
                async () =>
                {
                    await change.CommitAsync().ConfigureAwait(false);

                    this.DataEncrypted = false; this.EncryptedData = null;

                    Boolean hasContent = this.TryPrepareContentIntegrityState(out Byte[]? contentHash);

                    if(hasContent)
                    {
                        if(contentHash is null || !await this.UpdateFieldIntegrityHashAsync(DataSecurityCNKey,contentHash,true,cancel).ConfigureAwait(false)) { throw new DataProtectionOperationRejectedException(); }
                    }
                    else
                    {
                        if(!await this.UpdateContentIntegrityHashAsync(clear:true,cancel:cancel).ConfigureAwait(false)) { throw new DataProtectionOperationRejectedException(); }

                        this.ClearAssertion(DataSecurityCNKey);
                    }

                    if(!this.ApplyAssertionState(DataSecurityCNKey,security)) { throw new DataProtectionOperationRejectedException(); }

                    if(!await this.UpdateFieldIntegrityHashAsync(DataSecurityEDKey,Array.Empty<Byte>(),cancel:cancel).ConfigureAwait(false)) { throw new DataProtectionOperationRejectedException(); }

                    this.ClearAssertion(DataSecurityEDKey);

                    this.DataProtectionInfo = null;
                },
                async () =>
                {
                    try { await change.RollbackAsync().ConfigureAwait(false); }

                    finally { original.Restore(this); }
                },
                () => change.CompleteAsync());

            await transaction.CommitAsync().ConfigureAwait(false);

            try { await transaction.CompleteAsync().ConfigureAwait(false); }

            catch ( Exception cleanupError )
            {
                try { KusDepotLog.Error(cleanupError,DecryptDataFail,MyTypeName,MyID); } catch {}

                throw new DataProtectionCleanupFailedException(nameof(DecryptData),change.CleanupArtifactPath,cleanupError);
            }

            return true;
        }
        catch ( DataProtectionOperationRejectedException ) { return false; }

        catch ( DataProtectionCleanupFailedException ) { throw; }

        catch ( DataProtectionRollbackFailedException ) { throw; }

        catch ( Exception error )
        {
            KusDepotLog.Error(error,DecryptDataFail,MyTypeName,MyID);

            if(NoExceptions||MyNoExceptions) { return false; }

            throw;
        }
        finally { if(acquired) { this.ReleaseLocks(); } }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="BuildCanonicalEnvelopeRootContext"]/*'/>*/
    private Byte[]? BuildCanonicalEnvelopeRootContext(DataFieldState fieldstate)
    {
        Guid? id = this.GetID(); if(id is null || id == Guid.Empty) { return null; }

        Byte[] context = new Byte[16 + sizeof(Int32)];

        Span<Byte> buffer = context;

        Int32 offset = 0;

        WriteGuidBytes(id.Value,buffer.Slice(offset,16)); offset += 16;

        WriteInt32BigEndian(buffer.Slice(offset,sizeof(Int32)),(Int32)fieldstate); offset += sizeof(Int32);

        return context;
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CreateReplacementFile"]/*'/>*/
    private static String CreateReplacementFile(String file)
    {
        String? directory = Path.GetDirectoryName(Path.GetFullPath(file));

        if(String.IsNullOrEmpty(directory)) { throw new IOException(); }

        while(true)
        {
            String path = Path.Combine(directory,$".kusdepot-{Guid.NewGuid():N}.tmp");

            try
            {
                using FileStream _ = new(path,new FileStreamOptions{Mode = FileMode.CreateNew , Access = FileAccess.Write , Share = FileShare.None , Options = FileOptions.Asynchronous});

                return path;
            }
            catch ( IOException ) when(File.Exists(path)) {}
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DeleteReplacementFile"]/*'/>*/
    private static void DeleteReplacementFile(String? file)
    {
        if(String.IsNullOrEmpty(file)) { return; }

        try { File.Delete(file); } catch {}
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptDataBuffer"]/*'/>*/
    protected Byte[]? DecryptDataBuffer(Byte[]? data , DataItemSecurityContext? security)
    {
        try
        {
            if(security is null || data is null || data.Length == 0) { return null; }

            var c = security.LocalObject.Certificate; Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent);

            if(c is null || rootcontext is null) { return null; }

            return DataItemSecurityEnvelope.DecryptArray(data,c,rootcontext);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptDataBufferAsync"]/*'/>*/
    protected async Task<Byte[]?> DecryptDataBuffer(Byte[]? data , DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || data is null || data.Length == 0) { return null; }

            var c = security.LocalObject.Certificate; Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent);

            if(c is null || rootcontext is null) { return null; }

            return await DataItemSecurityEnvelope.DecryptArrayAsync(data,c,rootcontext,cancel:cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptFileToStagedTransform"]/*'/>*/
    private protected async Task<StagedFileTransform?> DecryptFileToStagedTransform(String? file , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        String? t = null;

        try
        {
            if(security is null || String.IsNullOrEmpty(file) || File.Exists(file) is false) { return null; }

            t = CreateReplacementFile(file);

            Stream i = OpenSequentialRead(file);

            await using (i.ConfigureAwait(false))
            {
                await using Stream o = OpenExclusiveWrite(t);

                if(!await DecryptToStream(i,o,security,cancel).ConfigureAwait(false)) { return null; }

                await o.FlushAsync(cancel).ConfigureAwait(false);
            }

            StagedFileTransform staged = new(file,t); t = null;

            return staged;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptFileToStagedTransformFail,MyTypeName,MyID); return null; }

        finally { DeleteReplacementFile(t); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptToStream"]/*'/>*/
    protected async Task<Boolean> DecryptToStream(Byte[]? data , Stream destination , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || data is null || data.Length == 0 || destination is null || destination.CanWrite is false) { return false; }

            var c = security.LocalObject.Certificate; Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent);

            if(c is null || rootcontext is null || DataEncrypted is false) { return false; }

            if(destination.CanSeek)
            {
                destination.Position = 0; destination.SetLength(0);
            }

            using var i = new MemoryStream(data,false);

            return await DataItemSecurityEnvelope.DecryptStreamAsync(i,destination,c,rootcontext,cancel:cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptToStreamFail,MyTypeName,MyID); return false; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DecryptToStreamStream"]/*'/>*/
    protected async Task<Boolean> DecryptToStream(Stream? data , Stream destination , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || data is null || data.CanRead is false || destination is null || destination.CanWrite is false) { return false; }

            var c = security.LocalObject.Certificate; Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent);

            if(c is null || rootcontext is null || DataEncrypted is false) { return false; }

            if(destination.CanSeek)
            {
                destination.Position = 0; destination.SetLength(0);
            }

            return await DataItemSecurityEnvelope.DecryptStreamAsync(data,destination,c,rootcontext,cancel:cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptToStreamFail,MyTypeName,MyID); return false; }
    }

    ///<inheritdoc/>
    public async Task<Boolean> EncryptData(DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        if(security is null || this.DataEncrypted || !this.CanChangeProtectionState(security)) { return false; }

        Boolean acquired = false;

        try
        {
            await this.AcquireLocksAsync(cancel).ConfigureAwait(false); acquired = true;

            if(this.DataEncrypted || !this.CanChangeProtectionState(security)) { return false; }

            await using DataItemProtectionTransaction transaction = new(nameof(EncryptData));

            DataItemStateSnapshot original = new(this);

            DataItemProtectionChange? change = await this.PrepareEncryptionAsync(security,cancel).ConfigureAwait(false);

            if(change is null) { return false; }

            transaction.Enlist(
                async () =>
                {
                    await change.CommitAsync().ConfigureAwait(false);

                    this.DataEncrypted = true;

                    if(change.EncryptedData is not null) { this.EncryptedData = change.EncryptedData; }

                    Byte[]? digest = change.FieldDigest;

                    if(digest is not null)
                    {
                        if(!await this.UpdateFieldIntegrityHashAsync(DataSecurityEDKey,digest,true,cancel).ConfigureAwait(false)) { throw new DataProtectionOperationRejectedException(); }
                    }
                    else if(!await this.UpdateFieldIntegrityHashAsync(DataSecurityEDKey,Array.Empty<Byte>(),cancel:cancel).ConfigureAwait(false)) { throw new DataProtectionOperationRejectedException(); }

                    if(!this.ApplyAssertionState(DataSecurityEDKey,security)) { throw new DataProtectionOperationRejectedException(); }

                    if(!await this.UpdateContentIntegrityHashAsync(clear:true,cancel:cancel).ConfigureAwait(false)) { throw new DataProtectionOperationRejectedException(); }

                    this.ClearAssertion(DataSecurityCNKey);

                    this.DataProtectionInfo = this.BuildProtectionInfo(security,DataFieldState.EncryptedContent);
                },
                async () =>
                {
                    try { await change.RollbackAsync().ConfigureAwait(false); }

                    finally { original.Restore(this); }
                },
                () => change.CompleteAsync());

            await transaction.CommitAsync().ConfigureAwait(false);

            try { await transaction.CompleteAsync().ConfigureAwait(false); }

            catch ( Exception cleanupError )
            {
                try { KusDepotLog.Error(cleanupError,EncryptDataFail,MyTypeName,MyID); } catch {}

                throw new DataProtectionCleanupFailedException(nameof(EncryptData),change.CleanupArtifactPath,cleanupError);
            }

            return true;
        }
        catch ( DataProtectionOperationRejectedException ) { return false; }

        catch ( DataProtectionCleanupFailedException ) { throw; }

        catch ( DataProtectionRollbackFailedException ) { throw; }

        catch ( Exception error )
        {
            KusDepotLog.Error(error,EncryptDataFail,MyTypeName,MyID);

            if(NoExceptions||MyNoExceptions) { return false; }

            throw;
        }
        finally { if(acquired) { this.ReleaseLocks(); } }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBuffer"]/*'/>*/
    protected Byte[]? EncryptDataBuffer(Byte[]? data , DataItemSecurityContext? security)
    {
        try
        {
            if(security is null || data is null || data.Length == 0) { return null; }

            Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent); ImmutableArray<DataSecurityRecipient>? r = ResolveRecipients(security,default).GetAwaiter().GetResult();

            if(rootcontext is null || !r.HasValue) { return null; }

            return DataItemSecurityEnvelope.EncryptArray(data,r.Value,security.LocalObject,rootcontext);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBufferWithHash"]/*'/>*/
    internal async Task<DataItemSecurityEnvelopeArrayHashResult?> EncryptDataBufferWithHash(Byte[]? data , DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || data is null || data.Length == 0) { return null; }

            Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent); ImmutableArray<DataSecurityRecipient>? r = await ResolveRecipients(security,cancel).ConfigureAwait(false);

            if(rootcontext is null || !r.HasValue) { return null; }

            return await DataItemSecurityEnvelope.EncryptArrayWithHashAsync(data,r.Value,security.LocalObject,rootcontext,DefaultCryptoChunkPower,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBufferAsync"]/*'/>*/
    protected async Task<Byte[]?> EncryptDataBuffer(Byte[]? data , DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || data is null || data.Length == 0) { return null; }

            Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent); ImmutableArray<DataSecurityRecipient>? r = await ResolveRecipients(security,cancel).ConfigureAwait(false);

            if(rootcontext is null || !r.HasValue) { return null; }

            return await DataItemSecurityEnvelope.EncryptArrayAsync(data,r.Value,security.LocalObject,rootcontext,DefaultCryptoChunkPower,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferFail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBufferUtf16"]/*'/>*/
    protected Byte[]? EncryptDataBufferUtf16(String? data , DataItemSecurityContext? security)
    {
        try
        {
            if(String.IsNullOrEmpty(data) || security is null) { return null; }

            Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent); ImmutableArray<DataSecurityRecipient>? r = ResolveRecipients(security,default).GetAwaiter().GetResult();

            if(rootcontext is null || !r.HasValue) { return null; }

            Byte[] plain = Encoding.Unicode.GetBytes(data);
            Int32? capacity = DataItemSecurityEnvelope.GetEncryptedCapacity(r.Value,plain.LongLength,chunksizepower:DefaultCryptoChunkPower);

            if(capacity.HasValue)
            {
                using var o = new MemoryStream(capacity.Value);
                using var i = new MemoryStream(plain,false);

                if(DataItemSecurityEnvelope.EncryptStream(i,o,r.Value,security.LocalObject,rootcontext,DefaultCryptoChunkPower) is false) { return null; }

                return GetMemoryStreamBuffer(o);
            }

            using(var o = new MemoryStream())
            {
                using var i = new MemoryStream(plain,false);

                if(DataItemSecurityEnvelope.EncryptStream(i,o,r.Value,security.LocalObject,rootcontext,DefaultCryptoChunkPower) is false) { return null; }

                return o.ToArray();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferUtf16Fail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptDataBufferUtf16WithHash"]/*'/>*/
    internal async Task<DataItemSecurityEnvelopeArrayHashResult?> EncryptDataBufferUtf16WithHash(String? data , DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(data) || security is null) { return null; }

            Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent);

            ImmutableArray<DataSecurityRecipient>? r = await ResolveRecipients(security,cancel).ConfigureAwait(false);

            if(rootcontext is null || !r.HasValue) { return null; }

            Byte[] plain = Encoding.Unicode.GetBytes(data);

            return await DataItemSecurityEnvelope.EncryptArrayWithHashAsync(plain,r.Value,security.LocalObject,rootcontext,DefaultCryptoChunkPower,cancel).ConfigureAwait(false);

        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptDataBufferUtf16Fail,MyTypeName,MyID); return null; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptFileToStagedTransform"]/*'/>*/
    private protected async Task<StagedFileTransform?> EncryptFileToStagedTransform(String? file , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        String? t = null;

        try
        {
            if(security is null || String.IsNullOrEmpty(file) || File.Exists(file) is false) { return null; }

            t = CreateReplacementFile(file);

            Byte[] h;

            Stream i = OpenSequentialRead(file);

            await using (i.ConfigureAwait(false))
            {
                HashingWriteStream o = new(OpenExclusiveWrite(t));

                await using (o.ConfigureAwait(false))
                {
                    if(!await EncryptToStream(i,o,security,cancel).ConfigureAwait(false)) { return null; }

                    await o.FlushAsync(cancel).ConfigureAwait(false);

                    h = o.GetHashAndReset();
                }
            }

            StagedFileTransform staged = new(file,t,h); t = null;

            return staged;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptFileToStagedTransformFail,MyTypeName,MyID); return null; }

        finally { DeleteReplacementFile(t); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="EncryptToStream"]/*'/>*/
    protected async Task<Boolean> EncryptToStream(Stream? data , Stream destination , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        try
        {
            if(security is null || data is null || data.CanRead is false || destination is null || destination.CanWrite is false) { return false; }

            Byte[]? rootcontext = BuildCanonicalEnvelopeRootContext(DataFieldState.EncryptedContent); ImmutableArray<DataSecurityRecipient>? r = await ResolveRecipients(security,cancel).ConfigureAwait(false);

            if(rootcontext is null || !r.HasValue || DataEncrypted is true) { return false; }

            if(destination.CanSeek)
            {
                destination.Position = 0; destination.SetLength(0);
            }

            return await DataItemSecurityEnvelope.EncryptStreamAsync(data,destination,r.Value,security.LocalObject,rootcontext,DefaultCryptoChunkPower,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptToStreamFail,MyTypeName,MyID); return false; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="PrepareDecryptionAsync"]/*'/>*/
    private protected virtual Task<DataItemProtectionChange?> PrepareDecryptionAsync(DataItemSecurityContext security,CancellationToken cancel) => Task.FromResult<DataItemProtectionChange?>(null);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="PrepareEncryptionAsync"]/*'/>*/
    private protected virtual Task<DataItemProtectionChange?> PrepareEncryptionAsync(DataItemSecurityContext security,CancellationToken cancel) => Task.FromResult<DataItemProtectionChange?>(null);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ResolveRecipient"]/*'/>*/
    private static async ValueTask<DataSecurityRecipient?> ResolveRecipient(DataSecurityRecipient recipient , Dictionary<Guid,DataSecurityObject> knownobjects , ISecurityIdentityDirectory? directory , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(recipient.Certificate is not null)
        {
            return recipient.ObjectId.HasValue ? new DataSecurityRecipient(recipient.Certificate,recipient.ObjectId) : new DataSecurityRecipient(recipient.Certificate);
        }

        if(recipient.ObjectId.HasValue)
        {
            if(knownobjects.TryGetValue(recipient.ObjectId.Value,out DataSecurityObject? known))
            {
                return new DataSecurityRecipient(known.Certificate,known.ObjectId);
            }

            if(directory is not null)
            {
                DataSecurityObject? resolvedobject = await directory.ResolveIdentity(recipient.ObjectId.Value,cancel).ConfigureAwait(false);

                if(resolvedobject is not null)
                {
                    return new DataSecurityRecipient(resolvedobject.Certificate,resolvedobject.ObjectId);
                }
            }
        }

        if(!String.IsNullOrWhiteSpace(recipient.Thumbprint) && directory is not null)
        {
            DataSecurityObject? thumbprintobject = await directory.ResolveIdentity(recipient.Thumbprint,cancel).ConfigureAwait(false);

            if(thumbprintobject is not null)
            {
                return new DataSecurityRecipient(thumbprintobject.Certificate,thumbprintobject.ObjectId);
            }
        }

        return null;
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ResolveRecipientsAsync"]/*'/>*/
    private static async Task<ImmutableArray<DataSecurityRecipient>?> ResolveRecipients(DataItemSecurityContext security , CancellationToken cancel)
    {
        Dictionary<Guid,DataSecurityObject> knownobjects = security.KnownObjects.IsDefaultOrEmpty
            ? new Dictionary<Guid,DataSecurityObject> { [security.LocalObject.ObjectId] = security.LocalObject }
            : security.KnownObjects.ToDictionary(_=>_.ObjectId,_=>_);

        if(!knownobjects.ContainsKey(security.LocalObject.ObjectId)) { knownobjects[security.LocalObject.ObjectId] = security.LocalObject; }

        ImmutableArray<DataSecurityRecipient> source = security.Recipients.IsDefaultOrEmpty && security.IncludeSelfRecipient
            ? [new DataSecurityRecipient(security.LocalObject.Certificate,security.LocalObject.ObjectId)]
            : security.Recipients;

        if(source.IsDefaultOrEmpty) { return null; }

        ImmutableArray<DataSecurityRecipient>.Builder builder = ImmutableArray.CreateBuilder<DataSecurityRecipient>(source.Length + 1);

        foreach(var recipient in source)
        {
            DataSecurityRecipient? resolved = await ResolveRecipient(recipient,knownobjects,security.Directory,cancel).ConfigureAwait(false);

            if(resolved is null) { return null; }

            builder.Add(resolved);
        }

        if(security.IncludeSelfRecipient && !builder.Any(_=>_.ObjectId == security.LocalObject.ObjectId))
        {
            builder.Add(new DataSecurityRecipient(security.LocalObject.Certificate,security.LocalObject.ObjectId));
        }

        return builder.ToImmutable();
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryPrepareContentIntegrityState"]/*'/>*/
    private protected virtual Boolean TryPrepareContentIntegrityState(out Byte[]? digest)
    {
        digest = this.GetIntegrityHash_NoSync();

        return digest is not null && digest.Length > 0;
    }
}