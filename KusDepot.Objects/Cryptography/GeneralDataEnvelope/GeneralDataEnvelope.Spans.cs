namespace KusDepot.Cryptography;

internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptSpan"]/*'/>*/ 
    internal static Boolean DecryptSpan(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default)
    {
        byteswritten = 0; if(input.IsEmpty || certificate is null) { return false; }

        Span<Byte> aesKey = stackalloc Byte[AesKeySize]; Span<Byte> baseNonceSeed = stackalloc Byte[BaseNonceSeedSize];

        try
        {
            if(!TryReadAndProcessHeader(input,certificate,aad,out var header,out var body,aesKey,baseNonceSeed)) { return false; }

            Int32 chunkSize = 1 << header.ChunkSizePower;
            Span<Byte> nonce = stackalloc Byte[NonceSize];
            Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];

            try
            {
                if(header.HasOriginalLength)
                {
                    if(body.Length == 0) { return header.OriginalLength == 0; }

                    ReadOnlySpan<Byte> lengths = body[..ChunkLengthsBlockSize];
                    UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                    UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                    if(plainLen != cipherLen || plainLen > chunkSize || plainLen != header.OriginalLength) { return false; }
                    if(output.Length < (Int32)plainLen)                                                    { return false; }

                    ReadOnlySpan<Byte> ciphertext = body.Slice(ChunkLengthsBlockSize,(Int32)cipherLen);
                    if(body[ChunkLengthsBlockSize + (Int32)cipherLen] != TagSize)                          { return false; }
                    ReadOnlySpan<Byte> tag = body.Slice(ChunkLengthsBlockSize + (Int32)cipherLen + 1,TagSize);

                    if(!DeriveNonce(aesKey,baseNonceSeed,0,nonce))                                         { return false; }

                    if(BuildPerChunkAad(
                    header.Flags,
                    0,
                    plainLen,
                    header.HasOriginalLength,
                    header.OriginalLength,
                    header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                    header.HasAadHash,
                    perChunkAad) is false)
                    { return false; }

                    using var gcm = new AesGcm(aesKey,TagSize);
                    gcm.Decrypt(nonce,ciphertext,tag,output[..(Int32)plainLen],perChunkAad);
                    byteswritten = (Int32)plainLen;

                    return true;
                }
                else
                {
                    if(body.Length < 1 + IntSize)                   { return false; }
                    ReadOnlySpan<Byte> footerSpan = body[^ (1 + IntSize) ..];
                    if(!TryParseFooter(footerSpan, out var footer)) { return false; }
                    if(footer.ChunkCount != 1)                      { return false; }

                    body = body[..^(1+IntSize)];

                    ReadOnlySpan<Byte> lengths = body[..ChunkLengthsBlockSize];
                    UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                    UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                    if(plainLen != cipherLen || plainLen > chunkSize) { return false; }
                    if(output.Length < (Int32)plainLen)               { return false; }

                    ReadOnlySpan<Byte> ciphertext = body.Slice(ChunkLengthsBlockSize,(Int32)cipherLen);
                    if(body[ChunkLengthsBlockSize + (Int32)cipherLen] != TagSize) { return false; }
                    ReadOnlySpan<Byte> tag = body.Slice(ChunkLengthsBlockSize + (Int32)cipherLen + 1,TagSize);

                    if(!DeriveNonce(aesKey,baseNonceSeed,0,nonce))    { return false; }

                    if(BuildPerChunkAad(
                        header.Flags,
                        0,
                        plainLen,
                        header.HasOriginalLength,
                        header.OriginalLength,
                        header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                        header.HasAadHash,perChunkAad) is false)
                    { return false; }

                    using var gcm = new AesGcm(aesKey,TagSize);
                    gcm.Decrypt(nonce,ciphertext,tag,output[..(Int32)plainLen],perChunkAad);
                    byteswritten = (Int32)plainLen;

                    return true;
                }
            }
            finally { ZeroMemory(perChunkAad); ZeroMemory(nonce); }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptSpanFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptSpanFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptSpan"]/*'/>*/ 
    internal static Boolean EncryptSpan(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false)
    {
        byteswritten = 0; if(certificate is null) { return false; }
        if(input.Length > (1 << DefaultChunkSizePower)) { return false; }

        Span<Byte> rsaWrappedKey = Span<Byte>.Empty;
        Span<Byte> aesKey = stackalloc Byte[AesKeySize];
        Span<Byte> aadHash = stackalloc Byte[AadHashSize];
        Span<Byte> baseNonceSeed = stackalloc Byte[BaseNonceSeedSize];
        Span<Byte> wrappedMaterial = stackalloc Byte[WrappedMaterialSize];

        try
        {
            UInt64 originalLength = (UInt64)input.Length;

            if(includeaadhash)
            {
                if(!SHA512.TryHashData(aad,aadHash,out Int32 written) || written != AadHashSize) { return false; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);
            aesKey.CopyTo(wrappedMaterial); baseNonceSeed.CopyTo(wrappedMaterial.Slice(AesKeySize,BaseNonceSeedSize));

            using(RSA? rsa = certificate.GetRSAPublicKey())
            {
                if(rsa is null) { return false; }
                rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
            }
            ZeroMemory(wrappedMaterial);

            Int32 headerLength = ComputeHeaderLength((UInt16)rsaWrappedKey.Length,true,includeaadhash);
            Int32 chunkBodyLength = ChunkLengthsBlockSize + input.Length + 1 + TagSize;
            Int32 totalLength = headerLength + chunkBodyLength;

            if(output.Length < totalLength) { return false; }

            if(WriteHeaderSpan(
                output[..headerLength],
                rsaWrappedKey,
                aadHash,
                includeaadhash,
                originalLength,
                true,
                DefaultChunkSizePower) is false)
            { return false; }
            ZeroMemory(rsaWrappedKey);

            Span<Byte> body = output[headerLength..];
            Span<Byte> lengths = body[..ChunkLengthsBlockSize];
            Span<Byte> ciphertext = body.Slice(ChunkLengthsBlockSize,input.Length);
            Span<Byte> taglen = body.Slice(ChunkLengthsBlockSize + input.Length,1);
            Span<Byte> tag = body.Slice(ChunkLengthsBlockSize + input.Length + 1,TagSize);
            Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];
            Span<Byte> nonce = stackalloc Byte[NonceSize];

            try
            {
                if(!DeriveNonce(aesKey,baseNonceSeed,0,nonce)) { return false; }

                if(BuildPerChunkAad(
                    (Byte)((true?Flag_HasOriginalLength:0) | (includeaadhash?Flag_HasAadHash:0)),
                    0,
                    (UInt32)input.Length,
                    true,
                    originalLength,
                    includeaadhash ? aadHash : ReadOnlySpan<Byte>.Empty,
                    includeaadhash,
                    perChunkAad) is false)
                { return false; }

                using var gcm = new AesGcm(aesKey,TagSize);
                gcm.Encrypt(nonce,input,ciphertext,tag,perChunkAad);

                WriteUInt32BigEndian(lengths[..IntSize],(UInt32)input.Length);
                WriteUInt32BigEndian(lengths.Slice(IntSize,IntSize),(UInt32)input.Length);
                taglen[0] = TagSize;

                byteswritten = totalLength;

                return true;
            }
            finally { ZeroMemory(perChunkAad); ZeroMemory(nonce); }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,EncryptSpanFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptSpanFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); ZeroMemory(rsaWrappedKey); ZeroMemory(wrappedMaterial); }
    }
}