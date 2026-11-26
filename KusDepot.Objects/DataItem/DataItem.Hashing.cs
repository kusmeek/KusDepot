namespace KusDepot;

public abstract partial class DataItem
{
    ///<inheritdoc/>
    public virtual Task<Boolean> CheckDataHash(CancellationToken cancel = default) => CheckDataHashCore(null,null,cancel);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CheckDataHashCore"]/*'/>*/
    protected async Task<Boolean> CheckDataHashCore(Byte[]? data , Stream? stream , CancellationToken cancel = default)
    {
        try
        {
            String k = DataEncrypted ? DataSecurityEDKey : DataSecurityCNKey;

            if(this.TryGetAssertionHash_NoSync(k,out Byte[]? assertionhash) && assertionhash is not null)
            {
                if(ContentStreamed is false && data is not null)
                {
                    return assertionhash.AsSpan().SequenceEqual(HashDataChunked(data,cancel));
                }

                if(ContentStreamed && stream is not null)
                {
                    Byte[] currenthash = await SHA512.HashDataAsync(stream,cancel).ConfigureAwait(false);

                    if(stream.CanSeek) { stream.Seek(0,SeekOrigin.Begin); }

                    return assertionhash.AsSpan().SequenceEqual(currenthash);
                }

                return false;
            }

            Boolean ok = false;

            if(ContentStreamed is false && data is not null) { ok = await VerifyFieldIntegrityHash(k,data,cancel).ConfigureAwait(false); }

            if(ContentStreamed && stream is not null) { ok = await VerifyFieldIntegrityHash(k,stream,cancel).ConfigureAwait(false); }

            return ok;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CheckDataHashCoreFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        Boolean ds = false;
        try
        {
            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; } ds = true;

            return this.GetHashCode_NoSync();
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync.Data.Release(); } }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCodeAsync"]/*'/>*/
    public virtual async Task<Int32> GetHashCodeAsync(CancellationToken cancel = default)
    {
        Boolean ds = false;
        try
        {
            if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } ds = true;

            return await this.GetHashCode_NoSyncAsync(cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync.Data.Release(); } }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCode_NoSync"]/*'/>*/
    protected virtual Int32 GetHashCode_NoSync()
    {
        try
        {
            Span<Byte> h = stackalloc Byte[64]; HashGuid(this.ID!.Value,h);

            return ReadHashPrefixInt32(h);
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetHashCode_NoSyncAsync"]/*'/>*/
    protected virtual Task<Int32> GetHashCode_NoSyncAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested(); Span<Byte> h = stackalloc Byte[64]; HashGuid(this.ID!.Value,h,cancel);

        try { return Task.FromResult(ReadHashPrefixInt32(h)); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetIntegrityHash_NoSync"]/*'/>*/
    protected internal virtual Byte[] GetIntegrityHash_NoSync()
    {
        try { return GenerateContentIntegritySHA512(this,this.CaptureContentIntegritySnapshot_NoSync()); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetIntegrityHash_NoSyncAsync"]/*'/>*/
    protected internal virtual Task<Byte[]> GetIntegrityHash_NoSyncAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try { return Task.FromResult(GenerateContentIntegritySHA512(this,this.CaptureContentIntegritySnapshot_NoSync(),cancel)); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetIntegrityHashFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CaptureContentIntegritySnapshot_NoSync"]/*'/>*/
    internal virtual ContentIntegritySnapshot CaptureContentIntegritySnapshot_NoSync()
    {
        throw new NotSupportedException(ContentIntegrityDataUnavailable);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CaptureContentMutationMemento_NoSync"]/*'/>*/
    internal virtual DataItemContentMutationMemento? CaptureContentMutationMemento_NoSync() => null;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CaptureContentIntegritySnapshotForHashing"]/*'/>*/
    internal ContentIntegritySnapshot CaptureContentIntegritySnapshotForHashing()
    {
        if(!this.Sync.Data.Wait(0)) { throw new InvalidOperationException("A DataItem data gate is unavailable for content-integrity hashing."); }

        try { return this.CaptureContentIntegritySnapshot_NoSync(); }

        finally { this.Sync.Data.Release(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetDataItemKnownTypes();

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="HashDataChunked"]/*'/>*/
    protected static Byte[] HashDataChunked(ReadOnlySpan<Byte> data , CancellationToken cancel = default)
    {
        using IncrementalHash h = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

        while(!data.IsEmpty)
        {
            cancel.ThrowIfCancellationRequested();

            Int32 n = Math.Min(DataHashingBufferSize,data.Length);

            h.AppendData(data[..n]); data = data[n..];
        }

        return h.GetHashAndReset();
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="HashGuidSpan"]/*'/>*/
    protected static void HashGuid(Guid value , Span<Byte> destination , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        if(destination.Length < 64) { throw new ArgumentException(null,nameof(destination)); }

        Span<Byte> b = stackalloc Byte[16]; WriteGuidBytes(value,b);

        if(!SHA512.TryHashData(b,destination,out Int32 written) || written != 64) { throw new InvalidOperationException(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="HashGuid"]/*'/>*/
    protected static Byte[] HashGuid(Guid value , CancellationToken cancel = default)
    {
        Byte[] h = new Byte[64]; HashGuid(value,h,cancel);

        return h;
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ReadHashPrefixInt32"]/*'/>*/
    protected static Int32 ReadHashPrefixInt32(ReadOnlySpan<Byte> hash)
    {
        return hash[0] | (hash[1] << 8) | (hash[2] << 16) | (hash[3] << 24);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="WriteGuidBytes"]/*'/>*/
    protected static void WriteGuidBytes(Guid value , Span<Byte> buffer)
    {
        if(!value.TryWriteBytes(buffer)) { throw new InvalidOperationException(); }
    }
}