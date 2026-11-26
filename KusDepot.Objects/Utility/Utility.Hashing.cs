namespace KusDepot.Utilities;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCode"]/*'/>*/
    public static Int32 GenerateDeterministicHashCode(Object? obj)
    {
        if(obj is null) { return unchecked((Int32)0xBAADF00D); }

        try
        {
            return GenerateDeterministicHashCodeCore(obj,DeterministicHashingProfile.ContentIntegrity);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicHashCodeFail); if(NoExceptions) { return 0; } throw; }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCodeAsync"]/*'/>*/
    public static Task<Int32> GenerateDeterministicHashCodeAsync(Object? obj , CancellationToken cancel = default)
    {
        if(obj is null) { return Task.FromResult(unchecked((Int32)0xBAADF00D)); }

        cancel.ThrowIfCancellationRequested();

        try
        {
            return Task.FromResult(GenerateDeterministicHashCodeCore(obj,DeterministicHashingProfile.ContentIntegrity,cancel));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicHashCodeFail); if(NoExceptions) { return Task.FromResult(0); } throw; }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCodeCore"]/*'/>*/
    private static Int32 GenerateDeterministicHashCodeCore(Object obj , DeterministicHashingProfile profile , CancellationToken cancel = default)
    {
        return BitConverter.ToInt32(GenerateDeterministicSHA512Core(obj,profile,cancel),0);
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicSHA512"]/*'/>*/
    public static Byte[] GenerateDeterministicSHA512(Object? obj , DeterministicHashingProfile profile = DeterministicHashingProfile.ContentIntegrity)
    {
        if(obj is null) { return SHA512.HashData(BitConverter.GetBytes(unchecked((Int32)0xBAADF00D))); }

        try
        {
            return GenerateDeterministicSHA512Core(obj,profile);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicSHA512Fail); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicSHA512Async"]/*'/>*/
    public static Task<Byte[]> GenerateDeterministicSHA512Async(Object? obj , DeterministicHashingProfile profile = DeterministicHashingProfile.ContentIntegrity , CancellationToken cancel = default)
    {
        if(obj is null) { return Task.FromResult(SHA512.HashData(BitConverter.GetBytes(unchecked((Int32)0xBAADF00D)))); }

        cancel.ThrowIfCancellationRequested();

        try
        {
            return Task.FromResult(GenerateDeterministicSHA512Core(obj,profile,cancel));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicSHA512Fail); if(NoExceptions) { return Task.FromResult(Array.Empty<Byte>()); } throw; }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicSHA512Core"]/*'/>*/
    private static Byte[] GenerateDeterministicSHA512Core(Object? obj , DeterministicHashingProfile profile , CancellationToken cancel = default)
    {
        return SHA512.HashData(new CanonicalHashGraph(profile,cancel).Compute(obj));
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateContentIntegritySHA512"]/*'/>*/
    internal static Byte[] GenerateContentIntegritySHA512(DataItem owner , ContentIntegritySnapshot snapshot , CancellationToken cancel = default)
    {
        return SHA512.HashData(new CanonicalHashGraph(DeterministicHashingProfile.ContentIntegrity,cancel,owner,snapshot).Compute(null));
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="ComputeSHA512Async"]/*'/>*/
    public static Task<Byte[]> ComputeSHA512Async(Byte[] payload , Int32 chunksize = DataHashingBufferSize , CancellationToken cancel = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(payload);

            return ComputeSHA512Async((ReadOnlyMemory<Byte>)payload,chunksize,cancel);
        }
        catch ( OperationCanceledException ) { return Task.FromResult(Array.Empty<Byte>()); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ComputeSHA512AsyncFail); if(NoExceptions) { return Task.FromResult(Array.Empty<Byte>()); } throw; }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="ComputeSHA512AsyncMemory"]/*'/>*/
    public static async Task<Byte[]> ComputeSHA512Async(ReadOnlyMemory<Byte> payload , Int32 chunksize = DataHashingBufferSize , CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunksize);

            if(payload.Length == 0) { return SHA512.HashData(Array.Empty<Byte>()); }

            using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

            for(Int32 offset = 0; offset < payload.Length; offset += chunksize)
            {
                cancel.ThrowIfCancellationRequested();

                Int32 count = Math.Min(chunksize,payload.Length - offset);

                hash.AppendData(payload.Slice(offset,count).Span);
            }

            return hash.GetHashAndReset();
        }
        catch ( OperationCanceledException ) { return Array.Empty<Byte>(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ComputeSHA512AsyncFail); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }
}