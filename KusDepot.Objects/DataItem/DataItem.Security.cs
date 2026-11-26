namespace KusDepot;

public abstract partial class DataItem
{
    ///<inheritdoc/>
    public virtual async Task<String?> SignData(String? field , DataItemSecurityContext? security = null , CancellationToken cancel = default)
    {
        if(security is null || String.IsNullOrEmpty(field)) { return null; }

        Byte[] hash;

        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

            try
            {
                if(String.Equals(field,DataSecurityHCKey,Ordinal))
                {
                    Int32 hc = await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false); cancel.ThrowIfCancellationRequested(); Span<Byte> b = stackalloc Byte[sizeof(Int32)];

                    BitConverter.TryWriteBytes(b,hc); hash = HashDataChunked(b,cancel);
                }

                else if(String.Equals(field,DataSecurityMDKey,Ordinal))
                {
                    hash = this.GetMetaBaseIntegrityHash();
                }

                else
                {
                    if(String.Equals(field,DataSecurityCNKey,Ordinal))
                    {
                        hash = this.GetIntegrityHash_NoSync();
                    }
                    else if(this.TryGetFieldIntegrityHash_NoSync(field,out Byte[]? hv) && hv is not null)
                    {
                        hash = hv;
                    }
                    else if(this.TryGetAssertionHash_NoSync(field,out hv) && hv is not null)
                    {
                        hash = hv;
                    }
                    else { return null; }
                }

                Guid? dataitemid = this.GetID(); if(dataitemid is null || dataitemid == Guid.Empty) { return null; }

                DataFieldAssertionData data = DataFieldAssertionData.Create(
                    dataitemid.Value,
                    field,
                    GetAssertionFieldState(field),
                    DataFieldHashAlgorithm.Sha512,
                    hash,
                    security.SigningObject.ObjectId,
                    security.SigningObject.Thumbprint,
                    purpose: security.Purpose,
                    createdat: DateTimeOffset.UtcNow,
                    notbefore: security.NotBefore,
                    expiresat: security.ExpiresAt);

                if(!DataFieldAssertionFactory.TryCreate(data,security.SigningObject.Certificate,out Byte[]? assertion)) { return null; }

                if(this.FieldAssertions is not null && this.FieldAssertions.ContainsKey(field)) { this.FieldAssertions.Remove(field); }

                this.FieldAssertions ??= new(); this.FieldAssertions[field] = assertion!;

                return field;
            }
            finally { this.Sync.Data.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SignDataFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CheckDataHashCoreHashed"]/*'/>
    protected Task<Boolean> CheckDataHashCoreHashed(String? key , Byte[]? currenthash , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(key) || currenthash is null || currenthash.Length == 0) { return Task.FromResult(false); }

            if(this.TryGetAssertionHash_NoSync(key,out Byte[]? assertionhash) && assertionhash is not null) { return Task.FromResult(assertionhash.AsSpan().SequenceEqual(currenthash)); }

            if(this.FieldIntegrityHashes is null || !this.FieldIntegrityHashes.TryGetValue(key,out Byte[]? storedhash) || storedhash is null) { return Task.FromResult(false); }

            return Task.FromResult(storedhash.AsSpan().SequenceEqual(currenthash));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Task.FromResult(false); } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetAssertionFieldState"]/*'/>*/
    private static DataFieldState GetAssertionFieldState(String field)
    {
        return String.Equals(field,DataSecurityEDKey,Ordinal) ? DataFieldState.EncryptedContent :
               String.Equals(field,DataSecurityHCKey,Ordinal) ? DataFieldState.HashCode : DataFieldState.ClearContent;
    }

    ///<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetCurrentContentIntegrityHash_NoSync"]/*'/>
    protected Byte[]? GetCurrentContentIntegrityHash_NoSync()
    {
        try { return this.GetIntegrityHash_NoSync(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ResolveAssertionCertificate"]/*'/>*/
    private static async ValueTask<X509Certificate2?> ResolveAssertionCertificate(DataItemSecurityContext security,DataFieldAssertionData data,CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(security.SigningObject.ObjectId == data.IssuerObjectId && String.Equals(security.SigningObject.Thumbprint,data.Thumbprint,Ordinal))
        {
            return security.SigningObject.Certificate;
        }

        if(security.LocalObject.ObjectId == data.IssuerObjectId && String.Equals(security.LocalObject.Thumbprint,data.Thumbprint,Ordinal))
        {
            return security.LocalObject.Certificate;
        }

        if(!security.KnownObjects.IsDefaultOrEmpty)
        {
            DataSecurityObject? known = security.KnownObjects.FirstOrDefault(_=>_.ObjectId == data.IssuerObjectId && String.Equals(_.Thumbprint,data.Thumbprint,Ordinal));

            if(known is not null)
            {
                return known.Certificate;
            }
        }

        if(security.Directory is not null)
        {
            DataSecurityObject? resolved = await security.Directory.ResolveIdentity(data.IssuerObjectId,cancel).ConfigureAwait(false);

            if(resolved is not null && String.Equals(resolved.Thumbprint,data.Thumbprint,Ordinal))
            {
                return resolved.Certificate;
            }

            DataSecurityObject? thumbprintresolved = await security.Directory.ResolveIdentity(data.Thumbprint,cancel).ConfigureAwait(false);

            if(thumbprintresolved is not null)
            {
                return thumbprintresolved.Certificate;
            }
        }

        return null;
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryGetFieldIntegrityHash_NoSync"]/*'/>*/
    protected Boolean TryGetFieldIntegrityHash_NoSync(String? key , [NotNullWhen(true)] out Byte[]? value)
    {
        value = null;

        try
        {
            return !String.IsNullOrEmpty(key) && this.FieldIntegrityHashes is not null && this.FieldIntegrityHashes.TryGetValue(key,out value) && value is not null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetFieldIntegrityHashNoSyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { value = null; return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryGetAssertionHash_NoSync"]/*'/>*/
    protected Boolean TryGetAssertionHash_NoSync(String? key , [NotNullWhen(true)] out Byte[]? value)
    {
        value = null;

        try
        {
            if(!this.TryGetFieldAssertion_NoSync(key,out Byte[]? assertionbytes) || assertionbytes is null || assertionbytes.Length == 0) { return false; }

            if(!DataFieldAssertionFactory.TryRead(assertionbytes,out DataFieldAssertion? assertion) || assertion is null || assertion.Data.Hash is null || assertion.Data.Hash.Length == 0) { return false; }

            value = assertion.Data.Hash;

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetFieldAssertionNoSyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { value = null; return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryGetFieldAssertion_NoSync"]/*'/>*/
    protected Boolean TryGetFieldAssertion_NoSync(String? key , [NotNullWhen(true)] out Byte[]? value)
    {
        value = null;

        try
        {
            return !String.IsNullOrEmpty(key) && this.FieldAssertions is not null && this.FieldAssertions.TryGetValue(key,out value) && value is not null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryGetFieldAssertionNoSyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { value = null; return false; } throw; }
    }

    ///<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateContentIntegrityHash"]/*'/>
    protected Boolean UpdateContentIntegrityHash(Boolean clear = false)
    {
        if(clear) { return this.UpdateFieldIntegrityHash(DataSecurityCNKey,Clear); }

        try
        {
            if(this.UpdateFieldIntegrityHash(DataSecurityCNKey,ComputeContentProjection)) { return true; }

            this.UpdateFieldIntegrityHash(DataSecurityCNKey,Clear);

            this.ClearAssertion(DataSecurityCNKey);

            return false;
        }
        catch
        {
            this.UpdateFieldIntegrityHash(DataSecurityCNKey,Clear);

            this.ClearAssertion(DataSecurityCNKey);

            throw;
        }
    }

    ///<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateContentIntegrityHashAsync"]/*'/>
    protected Task<Boolean> UpdateContentIntegrityHashAsync(Boolean clear = false , CancellationToken cancel = default)
    {
        return this.UpdateContentIntegrityHashAsyncCore(clear,cancel);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateContentIntegrityHashAsyncCore"]/*'/>*/
    private async Task<Boolean> UpdateContentIntegrityHashAsyncCore(Boolean clear , CancellationToken cancel)
    {
        if(clear) { return await this.UpdateFieldIntegrityHashAsync(DataSecurityCNKey,Clear,cancel:cancel).ConfigureAwait(false); }

        try
        {
            if(await this.UpdateFieldIntegrityHashAsync(DataSecurityCNKey,ComputeContentProjection,cancel:cancel).ConfigureAwait(false)) { return true; }

            await this.UpdateFieldIntegrityHashAsync(DataSecurityCNKey,Clear,cancel:cancel).ConfigureAwait(false);

            this.ClearAssertion(DataSecurityCNKey);

            return false;
        }
        catch
        {
            await this.UpdateFieldIntegrityHashAsync(DataSecurityCNKey,Clear,cancel:cancel).ConfigureAwait(false);

            this.ClearAssertion(DataSecurityCNKey);

            throw;
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateFieldIntegrityHash"]/*'/>*/
    protected virtual Boolean UpdateFieldIntegrityHash(String? key , Byte[]? data , Boolean store = false)
    {
        return this.UpdateFieldIntegrityHash(key,store ? StorePrecomputedDigest : HashRawData,data);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateFieldIntegrityHashMode"]/*'/>*/
    private Boolean UpdateFieldIntegrityHash(String? key , FieldIntegrityHashInputMode mode , Byte[]? value = null)
    {
        try
        {
            if(String.IsNullOrEmpty(key)) { return false; }

            if(mode is Clear || (mode is not ComputeContentProjection && (value is null || value.Length == 0)))
            {
                if(this.FieldIntegrityHashes is null || this.FieldIntegrityHashes.ContainsKey(key) is false) { return true; }

                if(this.FieldIntegrityHashes.Remove(key)) { if(this.FieldIntegrityHashes.Count == 0) { this.FieldIntegrityHashes = null; } return true; } return false;
            }

            Byte[] digest = mode switch
            {
                HashRawData => SHA512.HashData(value!),

                StorePrecomputedDigest => value!,

                ComputeContentProjection => this.GetIntegrityHash_NoSync(),

                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };

            this.FieldIntegrityHashes ??= new(); this.FieldIntegrityHashes[key] = digest; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateFieldIntegrityHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateFieldIntegrityHashAsync"]/*'/>*/
    protected virtual Task<Boolean> UpdateFieldIntegrityHashAsync(String? key , Byte[]? data , Boolean store = false , CancellationToken cancel = default)
    {
        return this.UpdateFieldIntegrityHashAsync(key,store ? StorePrecomputedDigest : HashRawData,data,cancel);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateFieldIntegrityHashAsyncMode"]/*'/>*/
    private Task<Boolean> UpdateFieldIntegrityHashAsync(String? key , FieldIntegrityHashInputMode mode , Byte[]? value = null , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(key)) { return Task.FromResult(false); }

            if(mode is Clear || (mode is not ComputeContentProjection && (value is null || value.Length == 0)))
            {
                if(this.FieldIntegrityHashes is null || this.FieldIntegrityHashes.ContainsKey(key) is false) { return Task.FromResult(true); }

                if(this.FieldIntegrityHashes.Remove(key)) { if(this.FieldIntegrityHashes.Count == 0) { this.FieldIntegrityHashes = null; } return Task.FromResult(true); } return Task.FromResult(false);
            }

            this.FieldIntegrityHashes ??= new();

            if(mode is StorePrecomputedDigest) { this.FieldIntegrityHashes[key] = value!; return Task.FromResult(true); }

            if(mode is ComputeContentProjection) { this.FieldIntegrityHashes[key] = this.GetIntegrityHash_NoSync(); return Task.FromResult(true); }

            if(mode is not HashRawData) { throw new ArgumentOutOfRangeException(nameof(mode)); }

            this.FieldIntegrityHashes[key] = HashDataChunked(value,cancel); return Task.FromResult(true);
        }
        catch ( OperationCanceledException ) { return Task.FromResult(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateFieldIntegrityHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Task.FromResult(false); } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="UpdateFieldAssertion"]/*'/>*/
    protected virtual Boolean UpdateFieldAssertion(String? key , DataItemSecurityContext? security)
    {
        if(security is null || String.IsNullOrEmpty(key)) { return false; }

        try
        {
            Byte[]? h = null;

            if(String.Equals(key,DataSecurityMDKey,Ordinal))
            {
                h = this.GetMetaBaseIntegrityHash();
            }

            else if(!this.TryGetFieldIntegrityHash_NoSync(key,out h))
            {
                h = null;

                if(!this.TryGetAssertionHash_NoSync(key,out h)) { h = null; }
            }

            if(h is null || h.Length == 0)
            {
                if(this.FieldAssertions is null || this.FieldAssertions.ContainsKey(key) is false) { return true; }

                if(this.FieldAssertions.Remove(key)) { if(this.FieldAssertions.Count == 0) { this.FieldAssertions = null; } return true; } return false;
            }

            Guid? dataitemid = this.GetID(); if(dataitemid is null || dataitemid == Guid.Empty) { return false; }

            DataFieldAssertionData data = DataFieldAssertionData.Create(
                dataitemid.Value,
                key,
                GetAssertionFieldState(key),
                DataFieldHashAlgorithm.Sha512,
                h,
                security.SigningObject.ObjectId,
                security.SigningObject.Thumbprint,
                purpose: security.Purpose,
                createdat: DateTimeOffset.UtcNow,
                notbefore: security.NotBefore,
                expiresat: security.ExpiresAt);

            if(!DataFieldAssertionFactory.TryCreate(data,security.SigningObject.Certificate,out Byte[]? assertion)) { return false; }

            this.FieldAssertions ??= new(); this.FieldAssertions[key] = assertion!;

            if(this.FieldIntegrityHashes is not null && this.FieldIntegrityHashes.Remove(key) && this.FieldIntegrityHashes.Count == 0) { this.FieldIntegrityHashes = null; }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,UpdateFieldAssertionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw;  }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> ValidateData(DataItemSecurityContext? security , CancellationToken cancel = default) => ValidateDataCore(security,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ValidateDataCore"]/*'/>*/
    protected virtual Task<Boolean> ValidateDataCore(DataItemSecurityContext? security , CancellationToken cancel = default)
    {
        return this.VerifyData(this.DataEncrypted ? DataSecurityEDKey : DataSecurityCNKey,security,cancel);
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> VerifyData(String? field , DataItemSecurityContext? security , CancellationToken cancel = default) => VerifyDataCore(field,security,null,null,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifyDataCoreHashed"]/*'/>*/
    protected virtual Task<Boolean> VerifyDataCoreHashed(String? field , DataItemSecurityContext? security , Byte[]? currentfieldhash , CancellationToken cancel = default) => VerifyDataFieldCore(field,security,null,null,currentfieldhash,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifyDataCore"]/*'/>*/
    protected virtual Task<Boolean> VerifyDataCore(String? field , DataItemSecurityContext? security , Byte[]? currentfielddata , Stream? currentfieldstream = null , CancellationToken cancel = default) => VerifyDataFieldCore(field,security,currentfielddata,currentfieldstream,null,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifyDataFieldCore"]/*'/>*/
    private async Task<Boolean> VerifyDataFieldCore(String? field , DataItemSecurityContext? security , Byte[]? currentfielddata , Stream? currentfieldstream , Byte[]? currentfieldhash , CancellationToken cancel)
    {
        try
        {
            if(security is null || String.IsNullOrEmpty(field)) { return false; }

            if(!String.Equals(field,DataSecurityHCKey,Ordinal) && !String.Equals(field,DataSecurityMDKey,Ordinal) && (currentfieldhash is null || currentfieldhash.Length == 0) && (currentfielddata is null || currentfielddata.Length == 0) && (currentfieldstream is null || !currentfieldstream.CanRead)) { return false; }

            if(this.FieldAssertions is null || !this.TryGetFieldAssertion_NoSync(field,out Byte[]? assertionbytes) || assertionbytes is null || assertionbytes.Length == 0) { return false; }

            if(!DataFieldAssertionFactory.TryRead(assertionbytes,out DataFieldAssertion? assertion) || assertion is null) { return false; }

            X509Certificate2? certificate = await ResolveAssertionCertificate(security,assertion.Data,cancel).ConfigureAwait(false); if(certificate is null) { return false; }

            if(!DataFieldAssertionVerifier.TryVerify(assertion,certificate,DateTimeOffset.UtcNow,out DataFieldAssertionData assertiondata)) { return false; }

            Guid? currentItemId = this.GetID(); if(currentItemId is null || currentItemId == Guid.Empty || assertiondata.DataItemId != currentItemId.Value) { return false; }

            if(!String.Equals(assertiondata.Field,field,Ordinal) || assertiondata.FieldState != GetAssertionFieldState(field) || assertiondata.HashAlgorithm != DataFieldHashAlgorithm.Sha512) { return false; }

            if(currentfieldhash is not null && currentfieldhash.Length > 0)
            {
                return assertiondata.Hash.AsSpan().SequenceEqual(currentfieldhash);
            }

            if(currentfieldstream is not null && currentfieldstream.CanRead)
            {
                Byte[] currenthash = await SHA512.HashDataAsync(currentfieldstream,cancel).ConfigureAwait(false);

                if(currentfieldstream.CanSeek) { currentfieldstream.Seek(0,SeekOrigin.Begin); }

                return assertiondata.Hash.AsSpan().SequenceEqual(currenthash);
            }

            if(currentfielddata is not null && currentfielddata.Length > 0)
            {
                Byte[] currenthash = HashDataChunked(currentfielddata,cancel);

                return assertiondata.Hash.AsSpan().SequenceEqual(currenthash);
            }

            if(String.Equals(field,DataSecurityHCKey,Ordinal))
            {
                Int32 hc = await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false); cancel.ThrowIfCancellationRequested(); Span<Byte> b = stackalloc Byte[sizeof(Int32)];

                BitConverter.TryWriteBytes(b,hc);

                Byte[] currenthash = HashDataChunked(b,cancel);

                return assertiondata.Hash.AsSpan().SequenceEqual(currenthash);
            }

            if(String.Equals(field,DataSecurityMDKey,Ordinal))
            {
                Byte[] currenthash = this.GetMetaBaseIntegrityHash();

                return assertiondata.Hash.AsSpan().SequenceEqual(currenthash);
            }

            return false;

        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyDataFieldCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifyFieldIntegrityHash"]/*'/>*/
    protected virtual Task<Boolean> VerifyFieldIntegrityHash(String? key , Byte[]? data , CancellationToken cancel = default)
    {
        try
        {
            if( String.IsNullOrEmpty(key) || data is null || data.Length == 0 ) { return Task.FromResult(false); }

            var c = HashDataChunked(data,cancel);

            if(this.FieldIntegrityHashes is null || this.FieldIntegrityHashes.ContainsKey(key) is false) { return Task.FromResult(false); }

            return Task.FromResult(this.FieldIntegrityHashes[key].AsSpan().SequenceEqual(c));
        }
        catch ( OperationCanceledException ) { return Task.FromResult(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyFieldIntegrityHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Task.FromResult(false); } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="VerifyFieldIntegrityHashStream"]/*'/>*/
    protected virtual async Task<Boolean> VerifyFieldIntegrityHash(String? key , Stream? data , CancellationToken cancel = default)
    {
        try
        {
            if( String.IsNullOrEmpty(key) || data is null || data.CanRead is false ) { return false; }

            var c = await SHA512.HashDataAsync(data,cancel).ConfigureAwait(false); if(data.CanSeek) { data.Seek(0,SeekOrigin.Begin); }

            if(this.FieldIntegrityHashes is null || this.FieldIntegrityHashes.ContainsKey(key) is false) { return false; }

            return this.FieldIntegrityHashes[key].AsSpan().SequenceEqual(c);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyFieldIntegrityHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> WipeData(DataItemSecurityContext? security = null , CancellationToken cancel = default) => Task.FromResult(false);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WipeDataCore"]/*'/>*/
    protected internal virtual Task<Boolean> WipeDataCore(DataItemSecurityContext? security , HashSet<Object> visited , CancellationToken cancel = default)
    {
        if(visited.Contains(this)) { return Task.FromResult(true); } visited.Add(this); return Task.FromResult(false);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WipeDataInMemoryOnly"]/*'/>*/
    protected internal virtual async Task<Boolean> WipeDataInMemoryOnly(DataItemSecurityContext? security , Boolean requirelocks , CancellationToken cancel = default)
    {
        try
        {
            return await this.WipeDataInMemoryOnlyCore(security,new HashSet<Object>(ReferenceEqualityComparer.Instance),requirelocks,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WipeDataInMemoryOnlyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WipeDataInMemoryOnlyCore"]/*'/>*/
    protected internal virtual Task<Boolean> WipeDataInMemoryOnlyCore(DataItemSecurityContext? security , HashSet<Object> visited , Boolean requirelocks , CancellationToken cancel = default)
    {
        if(visited.Contains(this)) { return Task.FromResult(true); } visited.Add(this); return Task.FromResult(false);
    }
}