namespace KusDepot.Cryptography;

internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean DecryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null) { return false; }

        Span<Byte> aesKey = stackalloc Byte[AesKeySize]; Span<Byte> baseNonceSeed = stackalloc Byte[BaseNonceSeedSize];

        try
        {
            if(TryReadAndProcessHeader(input,certificate,aad,out var header,aesKey,baseNonceSeed) is false) { return false; }

            Int32 chunkSize = 1 << header.ChunkSizePower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> nonce = stackalloc Byte[NonceSize];
            Span<Byte> lengths = stackalloc Byte[ChunkLengthsBlockSize];
            Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];
            UInt64 totalPlain = 0; UInt32 chunkIndex = 0;

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                if(header.HasOriginalLength)
                {
                    Int64 startPos = input.Position;
                    while(true)
                    {
                        Int32 lenRead = input.Read(lengths);
                        if(lenRead == 0) { break; } if(lenRead != ChunkLengthsBlockSize) { return false; }
                        UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                        if(plainLen != cipherLen)                                        { return false; }
                        if(plainLen > (UInt32)chunkSize)                                 { return false; }
                        if(ReadExact(input,cipher,0,(Int32)cipherLen) is false)          { return false; }
                        Int32 tagLenByte = input.ReadByte(); if(tagLenByte != TagSize)   { return false; }
                        if(ReadExact(input,tag) is false)                                { return false; }
                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false)  { return false; }

                        if(BuildPerChunkAad(
                            header.Flags,
                            chunkIndex,
                            plainLen,
                            header.HasOriginalLength,
                            header.OriginalLength,
                            header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                            header.HasAadHash,
                            perChunkAad) is false)
                        { return false; }

                        gcm.Decrypt(nonce,cipher.AsSpan(0,(Int32)cipherLen),tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);

                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                    }

                    if(totalPlain != header.OriginalLength)             { return false; }
                    if(input.CanSeek && input.Position != input.Length) { return false; }
                }
                else
                {
                    while(true)
                    {
                        Int32 b = input.ReadByte(); if(b == -1)        { return false; }
                        if(b == FooterMarker)
                        {
                            Span<Byte> footerRemain = stackalloc Byte[IntSize];
                            if(ReadExact(input,footerRemain) is false) { return false; }
                            UInt32 chunkCountFooter = ReadUInt32BigEndian(footerRemain);
                            if(chunkCountFooter != chunkIndex)         { return false; }
                            if(input.ReadByte() != -1)                 { return false; }
                            break;
                        }

                        lengths[0] = (Byte)b;
                        if(ReadExact(input,lengths[1..ChunkLengthsBlockSize]) is false) { return false; }
                        UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                        if(plainLen != cipherLen)                                       { return false; }
                        if(plainLen > (UInt32)chunkSize)                                { return false; }
                        if(ReadExact(input,cipher,0,(Int32)cipherLen) is false)         { return false; }
                        Int32 tagLenByte = input.ReadByte(); if(tagLenByte != TagSize)  { return false; }
                        if(ReadExact(input,tag) is false)                               { return false; }
                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return false; }
                        if(BuildPerChunkAad(
                            header.Flags,
                            chunkIndex,
                            plainLen,
                            header.HasOriginalLength,
                            header.OriginalLength,
                            header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                            header.HasAadHash,
                            perChunkAad) is false)
                        { return false; }

                        gcm.Decrypt(nonce,cipher.AsSpan(0,(Int32)cipherLen),tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);

                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                    }
                }
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(lengths); ZeroMemory(nonce); ZeroMemory(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }

            return true;
        }
        catch ( CryptographicException _ ) { Log.Verbose(_,DecryptStreamFail); return false; }

        catch ( Exception _ ) { Log.Error(_,DecryptStreamFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> DecryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null || cancel.IsCancellationRequested) { return false; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null;

        try
        {
            var headerResult = await TryReadAndProcessHeaderAsync(input,certificate,aad,cancel).ConfigureAwait(false); if(headerResult.Success is false) { return false; }

            aesKey = headerResult.AesKey;
            var header = headerResult.Header;
            baseNonceSeed = headerResult.BaseNonceSeed;

            Int32 chunkSize = 1 << header.ChunkSizePower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize);
            Byte[] nonce = ArrayPool<Byte>.Shared.Rent(NonceSize);
            Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
            Byte[] perChunkAad = ArrayPool<Byte>.Shared.Rent(PerChunkAadHashSize);
            Byte[] singleByte = ArrayPool<Byte>.Shared.Rent(1);
            UInt64 totalPlain = 0; UInt32 chunkIndex = 0;

            try
            {
                using var gcm = new AesGcm(aesKey!,TagSize);
                if(header.HasOriginalLength)
                {
                    while(true)
                    {
                        if(cancel.IsCancellationRequested) { return false; }
                        Int32 lenRead = await input.ReadAsync(lengths.AsMemory(0,ChunkLengthsBlockSize),cancel).ConfigureAwait(false);
                        if(lenRead == 0) { break; } if(lenRead != ChunkLengthsBlockSize)                                          { return false; }
                        UInt32 plainLen = ReadUInt32BigEndian(lengths.AsSpan(0,IntSize));
                        UInt32 cipherLen = ReadUInt32BigEndian(lengths.AsSpan(IntSize,IntSize));
                        if(plainLen != cipherLen)                                                                                 { return false; }
                        if(plainLen > (UInt32)chunkSize)                                                                          { return false; }
                        if(await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherLen),cancel).ConfigureAwait(false) is false) { return false; }
                        if(await ReadExactAsync(input,singleByte.AsMemory(0,1),cancel).ConfigureAwait(false) is false)            { return false; }
                        if(singleByte[0] != TagSize)                                                                              { return false; }
                        if(await ReadExactAsync(input,tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false) is false)             { return false; }
                        if(DeriveNonce(aesKey!,baseNonceSeed!,chunkIndex,nonce) is false)                                         { return false; }

                        if(BuildPerChunkAad(
                            header.Flags,
                            chunkIndex,
                            plainLen,
                            header.HasOriginalLength,
                            header.OriginalLength,
                            header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                            header.HasAadHash,
                            perChunkAad) is false)
                        { return false; }

                        gcm.Decrypt(nonce.AsSpan(0,NonceSize),cipher.AsSpan(0,(Int32)cipherLen),tag.AsSpan(0,TagSize),plain.AsSpan(0,(Int32)plainLen),perChunkAad);

                        await output.WriteAsync(plain.AsMemory(0,(Int32)plainLen),cancel).ConfigureAwait(false);
                        totalPlain += plainLen;
                        chunkIndex++;
                    }

                    if(totalPlain != header.OriginalLength)             { return false; }
                    if(input.CanSeek && input.Position != input.Length) { return false; }
                }
                else
                {
                    while(true)
                    {
                        if(cancel.IsCancellationRequested) { return false; }
                        Int32 b = await input.ReadAsync(singleByte.AsMemory(0,1),cancel).ConfigureAwait(false); if(b == 0) { return false; }
                        if(singleByte[0] == FooterMarker)
                        {
                            Byte[] footerRemain = ArrayPool<Byte>.Shared.Rent(IntSize);
                            try
                            {
                                if(await ReadExactAsync(input,footerRemain.AsMemory(0,IntSize),cancel).ConfigureAwait(false) is false) { return false; }
                                UInt32 chunkCountFooter = ReadUInt32BigEndian(footerRemain.AsSpan(0,IntSize));
                                if(chunkCountFooter != chunkIndex)                                                                     { return false; }
                                if((await input.ReadAsync(singleByte.AsMemory(0,1),cancel).ConfigureAwait(false)) != 0)                { return false; }
                                break;
                            }
                            finally { ZeroMemory(footerRemain); ArrayPool<Byte>.Shared.Return(footerRemain); }
                        }

                        lengths[0] = singleByte[0];
                        if(await ReadExactAsync(input,lengths.AsMemory(1,ChunkLengthsBlockSize - 1),cancel).ConfigureAwait(false) is false) { return false; }
                        UInt32 plainLen = ReadUInt32BigEndian(lengths.AsSpan(0,IntSize));
                        UInt32 cipherLen = ReadUInt32BigEndian(lengths.AsSpan(IntSize,IntSize));
                        if(plainLen != cipherLen)                                                                                           { return false; }
                        if(plainLen > (UInt32)chunkSize)                                                                                    { return false; }
                        if(await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherLen),cancel).ConfigureAwait(false) is false)           { return false; }
                        if(await ReadExactAsync(input,singleByte.AsMemory(0,1),cancel).ConfigureAwait(false) is false)                      { return false; }
                        if(singleByte[0] != TagSize)                                                                                        { return false; }
                        if(await ReadExactAsync(input,tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false) is false)                       { return false; }
                        if(DeriveNonce(aesKey!,baseNonceSeed!,chunkIndex,nonce) is false)                                                   { return false; }
                        if(BuildPerChunkAad(
                            header.Flags,
                            chunkIndex,
                            plainLen,
                            header.HasOriginalLength,
                            header.OriginalLength,
                            header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                            header.HasAadHash,
                            perChunkAad) is false)
                        { return false; }

                        gcm.Decrypt(nonce.AsSpan(0,NonceSize),cipher.AsSpan(0,(Int32)cipherLen),tag.AsSpan(0,TagSize),plain.AsSpan(0,(Int32)plainLen),perChunkAad);

                        await output.WriteAsync(plain.AsMemory(0,(Int32)plainLen),cancel).ConfigureAwait(false);
                        totalPlain += plainLen;
                        chunkIndex++;
                    }
                }
            }
            finally
            {
                ZeroMemory(singleByte); ArrayPool<Byte>.Shared.Return(singleByte);
                ZeroMemory(perChunkAad); ArrayPool<Byte>.Shared.Return(perChunkAad);
                ZeroMemory(lengths); ArrayPool<Byte>.Shared.Return(lengths);
                ZeroMemory(nonce); ArrayPool<Byte>.Shared.Return(nonce);
                ZeroMemory(tag); ArrayPool<Byte>.Shared.Return(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher);
                ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }

            return true;
        }
        catch ( CryptographicException _ ) { Log.Verbose(_,DecryptStreamAsyncFail); return false; }

        catch ( Exception _ ) { Log.Error(_,DecryptStreamAsyncFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean EncryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default,
        Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null) { return false; }

        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower)                                     { return false; }

        Span<Byte> aadHash = stackalloc Byte[AadHashSize]; Span<Byte> wrappedMaterial = stackalloc Byte[WrappedMaterialSize];

        Span<Byte> aesKey = stackalloc Byte[AesKeySize]; Span<Byte> baseNonceSeed = stackalloc Byte[BaseNonceSeedSize];

        Span<Byte> rsaWrappedKey = Span<Byte>.Empty;

        try
        {
            Int64? origLen = null;
            if(includeoriginallength)
            {
                if(input.CanSeek is false) { return false; }
                origLen = input.Length - input.Position;
                if(origLen < 0) { return false; }
            }
            UInt64 originalLength = includeoriginallength ? (UInt64)origLen!.Value : 0UL;

            if(includeaadhash)
            {
                if(SHA512.TryHashData(aad,aadHash,out Int32 written) is false || written != AadHashSize) { return false; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);

            aesKey.CopyTo(wrappedMaterial); baseNonceSeed.CopyTo(wrappedMaterial.Slice(AesKeySize,BaseNonceSeedSize));

            using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null) { return false; }
            rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
            ZeroMemory(wrappedMaterial);

            if(WriteHeader(
                output,
                rsaWrappedKey,
                aadHash,
                includeaadhash,
                originalLength,
                includeoriginallength,
                (Byte)chunksizepower) is false)
            { return false; }
            ZeroMemory(rsaWrappedKey);

            Int32 chunkSize = 1 << chunksizepower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> nonce = stackalloc Byte[NonceSize];
            Span<Byte> lengths = stackalloc Byte[ChunkLengthsBlockSize];
            Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];
            UInt64 totalWritten = 0; UInt32 chunkIndex = 0; Int32 read;

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                while((read = input.Read(plain,0,chunkSize)) > 0)
                {
                    if(includeoriginallength && (totalWritten + (UInt64)read) > originalLength) { return false; }
                    if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false)             { return false; }
                    if(BuildPerChunkAad(
                        (Byte)((includeoriginallength?Flag_HasOriginalLength:0) | (includeaadhash?Flag_HasAadHash:0)),
                        chunkIndex,
                        (UInt32)read,
                        includeoriginallength,
                        originalLength,
                        includeaadhash ? aadHash : ReadOnlySpan<Byte>.Empty,
                        includeaadhash,
                        perChunkAad) is false)
                    { return false; }

                    gcm.Encrypt(nonce,plain.AsSpan(0,read),cipher.AsSpan(0,read),tag,perChunkAad);
                    WriteUInt32BigEndian(lengths[..IntSize],(UInt32)read);
                    WriteUInt32BigEndian(lengths.Slice(IntSize,IntSize),(UInt32)read);
                    output.Write(lengths);
                    output.Write(cipher,0,read);
                    output.WriteByte(TagSize);
                    output.Write(tag);
                    totalWritten += (UInt64)read;
                    chunkIndex++;
                }

                if(includeoriginallength) { if(totalWritten != originalLength) { return false; } }
                else { if(WriteFooter(output,chunkIndex) is false) { return false; } }
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(lengths); ZeroMemory(nonce); ZeroMemory(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }

            return true;
        }
        catch ( CryptographicException _ ) { Log.Verbose(_,EncryptStreamFail); return false; }

        catch ( Exception _ ) { Log.Error(_,EncryptStreamFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); ZeroMemory(rsaWrappedKey); ZeroMemory(wrappedMaterial); }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> EncryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default,
        Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null || cancel.IsCancellationRequested) { return false; }

        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower)                                                                       { return false; }

        Byte[] aesKey = Array.Empty<Byte>(); Byte[] baseNonceSeed = Array.Empty<Byte>(); Byte[] aadHash = Array.Empty<Byte>();

        Byte[] wrappedMaterial = Array.Empty<Byte>(); Byte[] rsaWrappedKey = Array.Empty<Byte>();

        try
        {
            aesKey = new Byte[AesKeySize]; baseNonceSeed = new Byte[BaseNonceSeedSize];

            Int64? origLen = null;
            if(includeoriginallength)
            {
                if(input.CanSeek is false) { return false; }
                origLen = input.Length - input.Position; if(origLen < 0) { return false; }
            }
            UInt64 originalLength = includeoriginallength ? (UInt64)origLen!.Value : 0UL;

            if(includeaadhash)
            {
                aadHash = new Byte[AadHashSize]; if(SHA512.TryHashData(aad.Span,aadHash,out Int32 written) is false || written != AadHashSize) { return false; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);
            wrappedMaterial = new Byte[WrappedMaterialSize];
            aesKey.CopyTo(wrappedMaterial.AsSpan(0,AesKeySize)); 
            baseNonceSeed.CopyTo(wrappedMaterial.AsSpan(AesKeySize,BaseNonceSeedSize));

            using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null) { return false; }
            rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
            ZeroMemory(wrappedMaterial);

            if(WriteHeader(
                output,
                rsaWrappedKey,
                aadHash,
                includeaadhash,
                originalLength,
                includeoriginallength,
                (Byte)chunksizepower) is false)
            { return false; }
            ZeroMemory(rsaWrappedKey);

            Int32 chunkSize = 1 << chunksizepower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize);
            Byte[] nonce = ArrayPool<Byte>.Shared.Rent(NonceSize);
            Byte[] perChunkAad = ArrayPool<Byte>.Shared.Rent(PerChunkAadHashSize);
            Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
            Byte[] singleByte = ArrayPool<Byte>.Shared.Rent(1); singleByte[0] = TagSize;
            UInt64 totalWritten = 0; UInt32 chunkIndex = 0; Int32 read;

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                while((read = await input.ReadAsync(plain.AsMemory(0,chunkSize),cancel).ConfigureAwait(false)) > 0)
                {
                    if(cancel.IsCancellationRequested)                                                  { return false; }
                    if(includeoriginallength && (totalWritten + (UInt64)read) > originalLength)         { return false; }
                    if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce.AsSpan(0,NonceSize)) is false) { return false; }

                    if(BuildPerChunkAad(
                        (Byte)((includeoriginallength?Flag_HasOriginalLength:0) | (includeaadhash?Flag_HasAadHash:0)),
                        chunkIndex,
                        (UInt32)read,
                        includeoriginallength,
                        originalLength,
                        includeaadhash ? aadHash: ReadOnlySpan<Byte>.Empty,
                        includeaadhash,
                        perChunkAad.AsSpan(0,PerChunkAadHashSize)) is false)
                    { return false; }

                    gcm.Encrypt(nonce.AsSpan(0,NonceSize),new ReadOnlySpan<Byte>(plain,0,read),cipher.AsSpan(0,read),tag.AsSpan(0,TagSize),perChunkAad.AsSpan(0,PerChunkAadHashSize));

                    WriteUInt32BigEndian(lengths.AsSpan(0,IntSize),(UInt32)read);
                    WriteUInt32BigEndian(lengths.AsSpan(IntSize,IntSize),(UInt32)read);
                    await output.WriteAsync(lengths.AsMemory(0,ChunkLengthsBlockSize),cancel).ConfigureAwait(false);
                    await output.WriteAsync(cipher.AsMemory(0,read),cancel).ConfigureAwait(false);
                    await output.WriteAsync(singleByte.AsMemory(0,1),cancel).ConfigureAwait(false);
                    await output.WriteAsync(tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false);
                    totalWritten += (UInt64)read; chunkIndex++;
                }

                if(includeoriginallength) { if(totalWritten != originalLength) { return false; } }
                else { if(WriteFooter(output,chunkIndex) is false)             { return false; } }
            }
            finally
            {
                ZeroMemory(singleByte); ArrayPool<Byte>.Shared.Return(singleByte);
                ZeroMemory(lengths); ArrayPool<Byte>.Shared.Return(lengths);
                ZeroMemory(perChunkAad); ArrayPool<Byte>.Shared.Return(perChunkAad);
                ZeroMemory(nonce); ArrayPool<Byte>.Shared.Return(nonce);
                ZeroMemory(tag); ArrayPool<Byte>.Shared.Return(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher);
                ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }

            return true;
        }
        catch ( CryptographicException _ ) { Log.Verbose(_,EncryptStreamAsyncFail); return false; }

        catch ( Exception _ ) { Log.Error(_,EncryptStreamAsyncFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); ZeroMemory(rsaWrappedKey); ZeroMemory(wrappedMaterial); }
    }
}