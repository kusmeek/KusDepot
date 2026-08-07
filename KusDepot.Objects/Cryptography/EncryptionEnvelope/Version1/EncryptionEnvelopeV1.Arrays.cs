namespace KusDepot.Cryptography;

internal static partial class EncryptionEnvelopeV1
{
    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DecryptArray"]/*'/>*/
    internal static Byte[]? DecryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        if(input is null || certificate is null) { return null; }

        try
        {
            return DecryptArrayCore(input,certificate,aad);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DecryptArrayCore"]/*'/>*/
    private static Byte[]? DecryptArrayCore(ReadOnlySpan<Byte> input , X509Certificate2 certificate , ReadOnlySpan<Byte> aad = default)
    {
        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null;

        try
        {
            Span<Byte> aesKeySpan = stackalloc Byte[AesKeySize];
            Span<Byte> baseNonceSeedSpan = stackalloc Byte[BaseNonceSeedSize];

            if(!TryReadAndProcessHeader(input,certificate,aad,out var header,out var body,aesKeySpan,baseNonceSeedSpan)) { return null; }

            aesKey = aesKeySpan.ToArray(); baseNonceSeed = baseNonceSeedSpan.ToArray();
            using var output = header.HasOriginalLength && header.OriginalLength <= Int32.MaxValue ? new MemoryStream((Int32)header.OriginalLength) : new MemoryStream();

            Int32 chunkSize = 1 << header.ChunkSizePower; UInt64 totalPlain = 0; UInt32 chunkIndex = 0;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize); Span<Byte> tag = stackalloc Byte[TagSize]; Span<Byte> nonce = stackalloc Byte[NonceSize]; Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                ReadOnlySpan<Byte> remaining = body;

                if(header.HasOriginalLength)
                {
                    while(totalPlain < header.OriginalLength)
                    {
                        if(remaining.Length < ChunkLengthsBlockSize) { return null; }

                        UInt32 plainLen = ReadUInt32BigEndian(remaining[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(remaining.Slice(IntSize,IntSize));

                        if(plainLen != cipherLen || plainLen > (UInt32)chunkSize) { return null; }

                        Int32 chunkTotal = ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize + TagSize;
                        if(remaining.Length < chunkTotal) { return null; }
                        if(remaining[ChunkLengthsBlockSize + (Int32)cipherLen] != TagSize) { return null; }

                        ReadOnlySpan<Byte> cipher = remaining.Slice(ChunkLengthsBlockSize,(Int32)cipherLen);
                        remaining.Slice(ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize,TagSize).CopyTo(tag);

                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return null; }
                        if(BuildPerChunkAad(header.Flags,chunkIndex,plainLen,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { return null; }

                        gcm.Decrypt(nonce,cipher,tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);
                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                        remaining = remaining[chunkTotal..];
                    }

                    if(totalPlain != header.OriginalLength || !remaining.IsEmpty) { return null; }
                }
                else
                {
                    while(true)
                    {
                        if(remaining.IsEmpty) { return null; }
                        if(remaining[0] == FooterMarker)
                        {
                            if(remaining.Length != FooterSize) { return null; }
                            if(ReadUInt32BigEndian(remaining.Slice(1,IntSize)) != chunkIndex) { return null; }
                            break;
                        }

                        if(remaining.Length < ChunkLengthsBlockSize) { return null; }

                        UInt32 plainLen = ReadUInt32BigEndian(remaining[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(remaining.Slice(IntSize,IntSize));
                        if(plainLen != cipherLen || plainLen > (UInt32)chunkSize) { return null; }

                        Int32 chunkTotal = ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize + TagSize;
                        if(remaining.Length < chunkTotal) { return null; }
                        if(remaining[ChunkLengthsBlockSize + (Int32)cipherLen] != TagSize) { return null; }

                        ReadOnlySpan<Byte> cipher = remaining.Slice(ChunkLengthsBlockSize,(Int32)cipherLen);
                        remaining.Slice(ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize,TagSize).CopyTo(tag);

                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return null; }
                        if(BuildPerChunkAad(header.Flags,chunkIndex,plainLen,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { return null; }

                        gcm.Decrypt(nonce,cipher,tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);
                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                        remaining = remaining[chunkTotal..];
                    }
                }

                return GetMemoryStreamArray(output);
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(nonce); ZeroMemory(tag); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptArrayFail); return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayFail); if(NoExceptions) { return null; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DecryptArrayAsync"]/*'/>*/
    internal static Task<Byte[]?> DecryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        if(input is null || certificate is null || cancel.IsCancellationRequested) { return Task.FromResult<Byte[]?>(null); }

        try
        {
            return Task.FromResult(DecryptArrayAsyncCore(input,certificate,aad,cancel));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayAsyncFail); if(NoExceptions) { return Task.FromResult<Byte[]?>(null);; } throw; }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DecryptArrayAsyncCore"]/*'/>*/
    private static Byte[]? DecryptArrayAsyncCore(ReadOnlyMemory<Byte> input , X509Certificate2 certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return null; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null;

        try
        {
            Span<Byte> aesKeySpan = stackalloc Byte[AesKeySize];
            Span<Byte> baseNonceSeedSpan = stackalloc Byte[BaseNonceSeedSize];

            if(!TryReadAndProcessHeader(input.Span,certificate,aad.Span,out var header,out var body,aesKeySpan,baseNonceSeedSpan)) { return null; }

            aesKey = aesKeySpan.ToArray(); baseNonceSeed = baseNonceSeedSpan.ToArray();
            using var output = header.HasOriginalLength && header.OriginalLength <= Int32.MaxValue ? new MemoryStream((Int32)header.OriginalLength) : new MemoryStream();

            Int32 chunkSize = 1 << header.ChunkSizePower; UInt64 totalPlain = 0; UInt32 chunkIndex = 0;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize); Span<Byte> tag = stackalloc Byte[TagSize]; Span<Byte> nonce = stackalloc Byte[NonceSize]; Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                ReadOnlySpan<Byte> remaining = body;

                if(header.HasOriginalLength)
                {
                    while(totalPlain < header.OriginalLength)
                    {
                        cancel.ThrowIfCancellationRequested();
                        if(remaining.Length < ChunkLengthsBlockSize) { return null; }

                        UInt32 plainLen = ReadUInt32BigEndian(remaining[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(remaining.Slice(IntSize,IntSize));

                        if(plainLen != cipherLen || plainLen > (UInt32)chunkSize) { return null; }

                        Int32 chunkTotal = ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize + TagSize;
                        if(remaining.Length < chunkTotal) { return null; }
                        if(remaining[ChunkLengthsBlockSize + (Int32)cipherLen] != TagSize) { return null; }

                        ReadOnlySpan<Byte> cipher = remaining.Slice(ChunkLengthsBlockSize,(Int32)cipherLen);
                        remaining.Slice(ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize,TagSize).CopyTo(tag);

                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return null; }
                        if(BuildPerChunkAad(header.Flags,chunkIndex,plainLen,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { return null; }

                        gcm.Decrypt(nonce,cipher,tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);
                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                        remaining = remaining[chunkTotal..];
                    }

                    if(totalPlain != header.OriginalLength || !remaining.IsEmpty) { return null; }
                }
                else
                {
                    while(true)
                    {
                        cancel.ThrowIfCancellationRequested();
                        if(remaining.IsEmpty) { return null; }
                        if(remaining[0] == FooterMarker)
                        {
                            if(remaining.Length != FooterSize) { return null; }
                            if(ReadUInt32BigEndian(remaining.Slice(1,IntSize)) != chunkIndex) { return null; }
                            break;
                        }

                        if(remaining.Length < ChunkLengthsBlockSize) { return null; }

                        UInt32 plainLen = ReadUInt32BigEndian(remaining[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(remaining.Slice(IntSize,IntSize));
                        if(plainLen != cipherLen || plainLen > (UInt32)chunkSize) { return null; }

                        Int32 chunkTotal = ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize + TagSize;
                        if(remaining.Length < chunkTotal) { return null; }
                        if(remaining[ChunkLengthsBlockSize + (Int32)cipherLen] != TagSize) { return null; }

                        ReadOnlySpan<Byte> cipher = remaining.Slice(ChunkLengthsBlockSize,(Int32)cipherLen);
                        remaining.Slice(ChunkLengthsBlockSize + (Int32)cipherLen + TagSizeLengthFieldSize,TagSize).CopyTo(tag);

                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return null; }
                        if(BuildPerChunkAad(header.Flags,chunkIndex,plainLen,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { return null; }

                        gcm.Decrypt(nonce,cipher,tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);
                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                        remaining = remaining[chunkTotal..];
                    }
                }

                return GetMemoryStreamArray(output);
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(nonce); ZeroMemory(tag); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }
        }
        catch ( OperationCanceledException ) { return null; }

        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptArrayAsyncFail); return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayAsyncFail); if(NoExceptions) { return null; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="EncryptArray"]/*'/>*/
    internal static Byte[]? EncryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || certificate is null) { return null; }

        try
        {
            return EncryptArrayCore(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="EncryptArrayCore"]/*'/>*/
    private static Byte[]? EncryptArrayCore(ReadOnlySpan<Byte> input , X509Certificate2 certificate , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return null; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null; Byte[]? aadHash = null;

        try
        {
            aesKey = new Byte[AesKeySize]; baseNonceSeed = new Byte[BaseNonceSeedSize];
            UInt64 originalLength = includeoriginallength ? (UInt64)input.Length : 0UL;

            if(includeaadhash)
            {
                aadHash = new Byte[AadHashSize]; if(SHA512.TryHashData(aad,aadHash,out Int32 written) is false || written != AadHashSize) { return null; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);

            using var output = GetEncryptedArrayCapacity(certificate,input.Length,includeaadhash,includeoriginallength,chunksizepower) is Int32 capacity ? new MemoryStream(capacity) : new MemoryStream();

            using(var rsa = certificate.GetRSAPublicKey())
            {
                if(rsa is null) { return null; }
                Byte[] wrappedMaterial = new Byte[WrappedMaterialSize];
                aesKey.CopyTo(wrappedMaterial.AsSpan(0,AesKeySize));
                baseNonceSeed.CopyTo(wrappedMaterial.AsSpan(AesKeySize,BaseNonceSeedSize));
                Byte[] rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
                ZeroMemory(wrappedMaterial);
                if(WriteHeader(output,rsaWrappedKey,aadHash,includeaadhash,originalLength,includeoriginallength,(Byte)chunksizepower) is false) { return null; }
                ZeroMemory(rsaWrappedKey);
            }

            Int32 chunkSize = 1 << chunksizepower; UInt32 chunkIndex = 0; Byte[] tagLength = [TagSize];
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize); Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize); Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
            Span<Byte> nonce = stackalloc Byte[NonceSize]; Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                ReadOnlySpan<Byte> remaining = input;

                while(!remaining.IsEmpty)
                {
                    Int32 read = Math.Min(remaining.Length,chunkSize);

                    if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return null; }
                    if(BuildPerChunkAad((Byte)((includeoriginallength ? Flag_HasOriginalLength : 0) | (includeaadhash ? Flag_HasAadHash : 0)),chunkIndex,(UInt32)read,includeoriginallength,originalLength,includeaadhash ? aadHash! : ReadOnlySpan<Byte>.Empty,includeaadhash,perChunkAad) is false) { return null; }

                    gcm.Encrypt(nonce,remaining[..read],cipher.AsSpan(0,read),tag.AsSpan(0,TagSize),perChunkAad);
                    WriteUInt32BigEndian(lengths.AsSpan(0,IntSize),(UInt32)read);
                    WriteUInt32BigEndian(lengths.AsSpan(IntSize,IntSize),(UInt32)read);
                    output.Write(lengths,0,ChunkLengthsBlockSize);
                    output.Write(cipher,0,read);
                    output.Write(tagLength,0,TagSizeLengthFieldSize);
                    output.Write(tag,0,TagSize);

                    remaining = remaining[read..];
                    chunkIndex++;
                }

                if(includeoriginallength is false && WriteFooter(output,chunkIndex) is false) { return null; }

                return GetMemoryStreamArray(output);
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(nonce); ZeroMemory(lengths); ArrayPool<Byte>.Shared.Return(lengths);
                ZeroMemory(tag); ArrayPool<Byte>.Shared.Return(tag); ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(tagLength);
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,EncryptArrayFail); return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayFail); if(NoExceptions) { return null; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="EncryptArrayAsync"]/*'/>*/
    internal static Task<Byte[]?> EncryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(input is null || certificate is null || cancel.IsCancellationRequested) { return Task.FromResult<Byte[]?>(null);; }

        try
        {
            return Task.FromResult(EncryptArrayAsyncCore(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayAsyncFail); if(NoExceptions) { return Task.FromResult<Byte[]?>(null);; } throw; }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="EncryptArrayAsyncCore"]/*'/>*/
    private static Byte[]? EncryptArrayAsyncCore(ReadOnlyMemory<Byte> input , X509Certificate2 certificate , ReadOnlyMemory<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested || chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return null; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null; Byte[]? aadHash = null;

        try
        {
            aesKey = new Byte[AesKeySize]; baseNonceSeed = new Byte[BaseNonceSeedSize];
            UInt64 originalLength = includeoriginallength ? (UInt64)input.Length : 0UL;

            if(includeaadhash)
            {
                aadHash = new Byte[AadHashSize]; if(SHA512.TryHashData(aad.Span,aadHash,out Int32 written) is false || written != AadHashSize) { return null; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);

            using var output = GetEncryptedArrayCapacity(certificate,input.Length,includeaadhash,includeoriginallength,chunksizepower) is Int32 capacity ? new MemoryStream(capacity) : new MemoryStream();

            using(var rsa = certificate.GetRSAPublicKey())
            {
                if(rsa is null) { return null; }
                Byte[] wrappedMaterial = new Byte[WrappedMaterialSize];
                aesKey.CopyTo(wrappedMaterial.AsSpan(0,AesKeySize));
                baseNonceSeed.CopyTo(wrappedMaterial.AsSpan(AesKeySize,BaseNonceSeedSize));
                Byte[] rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
                ZeroMemory(wrappedMaterial);
                if(WriteHeader(output,rsaWrappedKey,aadHash,includeaadhash,originalLength,includeoriginallength,(Byte)chunksizepower) is false) { return null; }
                ZeroMemory(rsaWrappedKey);
            }

            Int32 chunkSize = 1 << chunksizepower; UInt32 chunkIndex = 0; Byte[] tagLength = [TagSize];
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize); Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize); Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
            Span<Byte> nonce = stackalloc Byte[NonceSize]; Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                ReadOnlySpan<Byte> remaining = input.Span;

                while(!remaining.IsEmpty)
                {
                    cancel.ThrowIfCancellationRequested();

                    Int32 read = Math.Min(remaining.Length,chunkSize);

                    if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return null; }
                    if(BuildPerChunkAad((Byte)((includeoriginallength ? Flag_HasOriginalLength : 0) | (includeaadhash ? Flag_HasAadHash : 0)),chunkIndex,(UInt32)read,includeoriginallength,originalLength,includeaadhash ? aadHash! : ReadOnlySpan<Byte>.Empty,includeaadhash,perChunkAad) is false) { return null; }

                    gcm.Encrypt(nonce,remaining[..read],cipher.AsSpan(0,read),tag.AsSpan(0,TagSize),perChunkAad);
                    WriteUInt32BigEndian(lengths.AsSpan(0,IntSize),(UInt32)read);
                    WriteUInt32BigEndian(lengths.AsSpan(IntSize,IntSize),(UInt32)read);
                    output.Write(lengths,0,ChunkLengthsBlockSize);
                    output.Write(cipher,0,read);
                    output.Write(tagLength,0,TagSizeLengthFieldSize);
                    output.Write(tag,0,TagSize);

                    remaining = remaining[read..];
                    chunkIndex++;
                }

                if(includeoriginallength is false && WriteFooter(output,chunkIndex) is false) { return null; }

                return GetMemoryStreamArray(output);
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(nonce); ZeroMemory(lengths); ArrayPool<Byte>.Shared.Return(lengths);
                ZeroMemory(tag); ArrayPool<Byte>.Shared.Return(tag); ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(tagLength);
            }
        }
        catch ( OperationCanceledException ) { return null; }

        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,EncryptArrayAsyncFail); return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayAsyncFail); if(NoExceptions) { return null; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); }
    }
}