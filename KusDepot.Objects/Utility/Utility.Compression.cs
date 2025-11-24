namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Compress"]/*'/>*/
    public static Byte[]? Compress(this Byte[]? input)
    {
        try
        {
            if(input is null) { return null; } if(Equals(input.Length,0)) { return Array.Empty<Byte>(); }

            using MemoryStream i = new(input,writable:false); using MemoryStream o = new();

            using(ZLibStream z = new(o,new ZLibCompressionOptions{ CompressionStrategy = ZLibCompressionStrategy.Default , CompressionLevel = 9 }))
            {
                i.CopyTo(z);
            }

            return o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompressFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CompressStream"]/*'/>*/
    public static Boolean Compress(Stream? input , Stream? output , CancellationToken cancel = default)
    {
        try
        {
            if( input is null || output is null || input.CanRead is false || output.CanWrite is false ) { return false; }

            using ZLibStream z = new(output,new ZLibCompressionOptions{ CompressionStrategy = ZLibCompressionStrategy.Default , CompressionLevel = 9 });

            Byte[] b = ArrayPool<Byte>.Shared.Rent(CompressionBufferSize); Int32 r;

            try
            {
                while((r = input.Read(b,0,b.Length)) > 0)
                {
                    if(cancel.IsCancellationRequested) { return false; } z.Write(b,0,r);
                }
            }
            finally { CryptographicOperations.ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }

            return true;
        }
        catch ( OperationCanceledException ) { return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CompressFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CompressAsync"]/*'/>*/
    public static async Task<Byte[]?> CompressAsync(this Byte[]? input , CancellationToken cancel = default)
    {
        try
        {
            if(input is null) { return null; } if(Equals(input.Length,0)) { return Array.Empty<Byte>(); }

            using MemoryStream i = new(input,writable:false); using MemoryStream o = new();

            await using (ZLibStream z = new(o,new ZLibCompressionOptions{ CompressionStrategy = ZLibCompressionStrategy.Default , CompressionLevel = 9 }))
            {
                await i.CopyToAsync(z,cancel).ConfigureAwait(false);
            }

            return o.ToArray();
        }
        catch ( OperationCanceledException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CompressFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CompressStreamAsync"]/*'/>*/
    public static async Task<Boolean> CompressAsync(Stream? input , Stream? output , CancellationToken cancel = default)
    {
        try
        {
            if( input is null || output is null || input.CanRead is false || output.CanWrite is false ) { return false; }

            await using ZLibStream z = new(output,new ZLibCompressionOptions{ CompressionStrategy = ZLibCompressionStrategy.Default , CompressionLevel = 9 });

            Byte[] b = ArrayPool<Byte>.Shared.Rent(CompressionBufferSize);

            try
            {
                while(true)
                {
                    Int32 r = await input.ReadAsync(b.AsMemory(0,b.Length),cancel).ConfigureAwait(false);

                    if(r == 0) { break; } if(cancel.IsCancellationRequested) { return false; }

                    await z.WriteAsync(b.AsMemory(0,r),cancel).ConfigureAwait(false);
                }
            }
            finally { CryptographicOperations.ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }

            return true;
        }
        catch ( OperationCanceledException ) { return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,CompressFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Decompress"]/*'/>*/
    public static Byte[]? Decompress(this Byte[]? input)
    {
        try
        {
            if(input is null) { return null; } if(Equals(input.Length,0)) { return Array.Empty<Byte>(); }

            using MemoryStream i = new(input,writable:false); using ZLibStream z = new(i,CompressionMode.Decompress);

            using MemoryStream o = new(); z.CopyTo(o);

            return o.ToArray();
        }
        catch ( InvalidDataException _ ) { KusDepotLog.Verbose(_,DecompressFail); return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecompressFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecompressStream"]/*'/>*/
    public static Boolean Decompress(Stream? input , Stream? output , CancellationToken cancel = default)
    {
        try
        {
            if( input is null || output is null || input.CanRead is false || output.CanWrite is false ) { return false; }

            using ZLibStream z = new(input,CompressionMode.Decompress,leaveOpen:true);

            Byte[] b = ArrayPool<Byte>.Shared.Rent(CompressionBufferSize); Int32 r;

            try
            {
                while((r = z.Read(b,0,b.Length)) > 0)
                {
                    if(cancel.IsCancellationRequested) { return false; } output.Write(b,0,r);
                }
            }
            finally { CryptographicOperations.ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }

            return true;
        }
        catch ( OperationCanceledException ) { return false; }

        catch ( InvalidDataException _ ) { KusDepotLog.Verbose(_,DecompressFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecompressFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecompressAsync"]/*'/>*/
    public static async Task<Byte[]?> DecompressAsync(this Byte[]? input , CancellationToken cancel = default)
    {
        try
        {
            if(input is null) { return null; } if(Equals(input.Length,0)) { return Array.Empty<Byte>(); }

            using MemoryStream i = new(input,writable:false); await using ZLibStream z = new(i,CompressionMode.Decompress);

            using MemoryStream o = new(); await z.CopyToAsync(o,cancel).ConfigureAwait(false);

            return o.ToArray();
        }
        catch ( OperationCanceledException ) { return null; }

        catch ( InvalidDataException _ ) { KusDepotLog.Verbose(_,DecompressFail); return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecompressFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecompressStreamAsync"]/*'/>*/
    public static async Task<Boolean> DecompressAsync(Stream? input , Stream? output , CancellationToken cancel = default)
    {
        try
        {
            if( input is null || output is null || input.CanRead is false || output.CanWrite is false ) { return false; }

            await using ZLibStream z = new(input,CompressionMode.Decompress,leaveOpen:true);

            Byte[] b = ArrayPool<Byte>.Shared.Rent(CompressionBufferSize); Int32 r;

            try
            {
                while(true)
                {
                    r = await z.ReadAsync(b.AsMemory(0,b.Length),cancel).ConfigureAwait(false);

                    if(r == 0) { break; } if(cancel.IsCancellationRequested) { return false; }

                    await output.WriteAsync(b.AsMemory(0,r),cancel).ConfigureAwait(false);
                }
            }
            finally { CryptographicOperations.ZeroMemory(b); ArrayPool<Byte>.Shared.Return(b); }

            return true;
        }
        catch ( OperationCanceledException ) { return false; }

        catch ( InvalidDataException _ ) { KusDepotLog.Verbose(_,DecompressFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecompressFail); if(NoExceptions) { return false; } throw; }
    }
}