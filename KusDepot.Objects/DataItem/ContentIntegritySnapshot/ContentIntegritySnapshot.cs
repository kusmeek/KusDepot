namespace KusDepot;

/**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegritySnapshot"]/main/*'/>*/
internal sealed class ContentIntegritySnapshot : IDisposable
{
    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegritySnapshot"]/property[@name="Fields"]/*'/>*/
    internal ImmutableSortedDictionary<String,Object?> Fields { get; }

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegritySnapshot"]/field[@name="ownedResources"]/*'/>*/
    private readonly ImmutableArray<IDisposable> ownedResources;

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegritySnapshot"]/constructor[@name="Constructor"]/*'/>*/
    internal ContentIntegritySnapshot(IEnumerable<KeyValuePair<String,Object?>> fields , IEnumerable<IDisposable>? ownedResources = null)
    {
        ArgumentNullException.ThrowIfNull(fields);

        this.Fields = fields.ToImmutableSortedDictionary(field => field.Key,field => field.Value,StringComparer.Ordinal);

        this.ownedResources = ownedResources?.ToImmutableArray() ?? ImmutableArray<IDisposable>.Empty;
    }

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegritySnapshot"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        List<Exception>? errors = null;

        foreach(IDisposable resource in this.ownedResources)
        {
            try { resource.Dispose(); }

            catch ( Exception error ) { (errors ??= new List<Exception>()).Add(error); }
        }

        if(errors is not null) { throw new AggregateException("One or more content-integrity resources could not be disposed.",errors); }
    }
}

/**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/main/*'/>*/
internal sealed class ContentIntegrityValue : IDisposable
{
    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/field[@name="bytes"]/*'/>*/
    private readonly Byte[]? bytes;

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/field[@name="stream"]/*'/>*/
    private readonly Stream? stream;

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/constructor[@name="Constructor"]/*'/>*/
    private ContentIntegrityValue(Byte[]? bytes , Stream? stream)
    {
        this.bytes = bytes; this.stream = stream;
    }

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/method[@name="FromBytes"]/*'/>*/
    internal static ContentIntegrityValue FromBytes(Byte[]? bytes) => new(bytes,null);

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/method[@name="FromStream"]/*'/>*/
    internal static ContentIntegrityValue FromStream(Stream? stream) => new(null,stream);

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/method[@name="ComputeDigest"]/*'/>*/
    internal Byte[] ComputeDigest(CancellationToken cancel)
    {
        if(this.bytes is not null) { return SHA512.HashData(this.bytes); }

        if(this.stream is null) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

        if(!this.stream.CanRead) { throw new InvalidOperationException(ContentIntegrityDataUnavailable); }

        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

        Byte[] buffer = ArrayPool<Byte>.Shared.Rent(DataHashingBufferSize);

        try
        {
            Int32 read;

            while((read = this.stream.Read(buffer,0,buffer.Length)) > 0)
            {
                cancel.ThrowIfCancellationRequested();

                hash.AppendData(buffer,0,read);
            }

            if(this.stream.CanSeek) { this.stream.Seek(0,SeekOrigin.Begin); }

            return hash.GetHashAndReset();
        }
        finally { ZeroMemory(buffer); ArrayPool<Byte>.Shared.Return(buffer); }
    }

    /**<include file='ContentIntegritySnapshot.xml' path='ContentIntegritySnapshot/class[@name="ContentIntegrityValue"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() => this.stream?.Dispose();
}
