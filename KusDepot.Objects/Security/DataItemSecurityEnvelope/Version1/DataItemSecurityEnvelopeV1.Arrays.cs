namespace KusDepot.Security.Data;

/**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/main/*'/>*/
internal static partial class DataItemSecurityEnvelopeV1
{
    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptArray"]/*'/>*/
    internal static Byte[]? DecryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> rootcontext = default)
    {
        if(input is null || certificate is null) { return null; }

        Byte[]? aeskey = null; Byte[]? basenonceseed = null;

        try
        {
            ReadOnlySpan<Byte> source = input;

            if(!TryParseHeader(source,out Header header)) { return null; }
            if(!TryValidateRootContext(rootcontext,header.RootContextHash.AsSpan())) { return null; }
            if(source.Length < FixedHeaderSize + header.RecipientTableLength) { return null; }
            ReadOnlySpan<Byte> recipientTable = source.Slice(FixedHeaderSize,(Int32)header.RecipientTableLength);
            if(!TryValidateEnvelopeMetadata(header,recipientTable,header.EnvelopeMetadataHash.AsSpan())) { return null; }
            if(!TryParseRecipientTable(recipientTable,header.RecipientCount,out ImmutableArray<RecipientEntry> recipients)) { return null; }
            if(!TryResolveRecipientMaterial(recipients,certificate,out aeskey,out basenonceseed) || aeskey is null || basenonceseed is null) { return null; }

            Int64 originalLength = checked((Int64)header.OriginalLength);
            using var output = originalLength <= Int32.MaxValue ? new MemoryStream((Int32)originalLength) : new MemoryStream();
            using var gcm = new AesGcm(aeskey,TagSize);

            Int32 chunkSize = 1 << header.ChunkSizePower;
            UInt32 chunkIndex = 0;
            Int64 totalPlain = 0;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> nonce = stackalloc Byte[12];
            Span<Byte> aad = stackalloc Byte[RootContextHashSize];

            try
            {
                ReadOnlySpan<Byte> remaining = source[(FixedHeaderSize + (Int32)header.RecipientTableLength)..];

                while(totalPlain < originalLength)
                {
                    if(remaining.Length < ChunkLengthBlockSize + TagSizeLengthFieldSize + TagSize) { return null; }

                    UInt32 plainLength = ReadUInt32BigEndian(remaining[..4]);
                    UInt32 cipherLength = ReadUInt32BigEndian(remaining.Slice(4,4));

                    if(plainLength != cipherLength || plainLength > (UInt32)chunkSize) { return null; }

                    Int32 chunkTotal = ChunkLengthBlockSize + (Int32)cipherLength + TagSizeLengthFieldSize + TagSize;
                    if(remaining.Length < chunkTotal) { return null; }
                    if(remaining[ChunkLengthBlockSize + (Int32)cipherLength] != TagSize) { return null; }

                    ReadOnlySpan<Byte> cipher = remaining.Slice(ChunkLengthBlockSize,(Int32)cipherLength);
                    remaining.Slice(ChunkLengthBlockSize + (Int32)cipherLength + TagSizeLengthFieldSize,TagSize).CopyTo(tag);

                    if(!DeriveNonce(aeskey,basenonceseed,chunkIndex,nonce) || !BuildPerChunkAad(header,chunkIndex,plainLength,aad)) { return null; }

                    gcm.Decrypt(nonce,cipher,tag,plain.AsSpan(0,(Int32)plainLength),aad);
                    output.Write(plain,0,(Int32)plainLength);

                    totalPlain += plainLength;
                    chunkIndex++;
                    remaining = remaining[chunkTotal..];
                }

                if(totalPlain != originalLength || !remaining.IsEmpty) { return null; }

                return GetMemoryStreamArray(output);
            }
            finally
            {
                ZeroMemory(plain);
                ArrayPool<Byte>.Shared.Return(plain);
                ZeroMemory(tag);
                ZeroMemory(nonce);
                ZeroMemory(aad);
            }
        }
        catch ( CryptographicException ) { return null; }
        finally
        {
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptArrayWithHash"]/*'/>*/
    internal static DataItemSecurityEnvelopeArrayHashResult? DecryptArrayWithHash(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> rootcontext = default)
    {
        if(input is null || certificate is null) { return null; }

        Byte[]? aeskey = null; Byte[]? basenonceseed = null;

        try
        {
            ReadOnlySpan<Byte> source = input;

            if(!TryParseHeader(source,out Header header)) { return null; }
            if(!TryValidateRootContext(rootcontext,header.RootContextHash.AsSpan())) { return null; }
            if(source.Length < FixedHeaderSize + header.RecipientTableLength) { return null; }
            ReadOnlySpan<Byte> recipientTable = source.Slice(FixedHeaderSize,(Int32)header.RecipientTableLength);
            if(!TryValidateEnvelopeMetadata(header,recipientTable,header.EnvelopeMetadataHash.AsSpan())) { return null; }
            if(!TryParseRecipientTable(recipientTable,header.RecipientCount,out ImmutableArray<RecipientEntry> recipients)) { return null; }
            if(!TryResolveRecipientMaterial(recipients,certificate,out aeskey,out basenonceseed) || aeskey is null || basenonceseed is null) { return null; }

            Int64 originalLength = checked((Int64)header.OriginalLength);
            using var output = originalLength <= Int32.MaxValue ? new MemoryStream((Int32)originalLength) : new MemoryStream();
            using var hashingOutput = new HashingWriteStream(output);
            using var gcm = new AesGcm(aeskey,TagSize);

            Int32 chunkSize = 1 << header.ChunkSizePower;
            UInt32 chunkIndex = 0;
            Int64 totalPlain = 0;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> nonce = stackalloc Byte[12];
            Span<Byte> aad = stackalloc Byte[RootContextHashSize];

            try
            {
                ReadOnlySpan<Byte> remaining = source[(FixedHeaderSize + (Int32)header.RecipientTableLength)..];

                while(totalPlain < originalLength)
                {
                    if(remaining.Length < ChunkLengthBlockSize + TagSizeLengthFieldSize + TagSize) { return null; }

                    UInt32 plainLength = ReadUInt32BigEndian(remaining[..4]);
                    UInt32 cipherLength = ReadUInt32BigEndian(remaining.Slice(4,4));

                    if(plainLength != cipherLength || plainLength > (UInt32)chunkSize) { return null; }

                    Int32 chunkTotal = ChunkLengthBlockSize + (Int32)cipherLength + TagSizeLengthFieldSize + TagSize;
                    if(remaining.Length < chunkTotal) { return null; }
                    if(remaining[ChunkLengthBlockSize + (Int32)cipherLength] != TagSize) { return null; }

                    ReadOnlySpan<Byte> cipher = remaining.Slice(ChunkLengthBlockSize,(Int32)cipherLength);
                    remaining.Slice(ChunkLengthBlockSize + (Int32)cipherLength + TagSizeLengthFieldSize,TagSize).CopyTo(tag);

                    if(!DeriveNonce(aeskey,basenonceseed,chunkIndex,nonce) || !BuildPerChunkAad(header,chunkIndex,plainLength,aad)) { return null; }

                    gcm.Decrypt(nonce,cipher,tag,plain.AsSpan(0,(Int32)plainLength),aad);
                    hashingOutput.Write(plain,0,(Int32)plainLength);

                    totalPlain += plainLength;
                    chunkIndex++;
                    remaining = remaining[chunkTotal..];
                }

                if(totalPlain != originalLength || !remaining.IsEmpty) { return null; }

                return new(GetMemoryStreamArray(output),hashingOutput.GetHashAndReset());
            }
            finally
            {
                ZeroMemory(plain);
                ArrayPool<Byte>.Shared.Return(plain);
                ZeroMemory(tag);
                ZeroMemory(nonce);
                ZeroMemory(aad);
            }
        }
        catch ( CryptographicException ) { return null; }
        finally
        {
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptArrayAsync"]/*'/>*/
    internal static Task<Byte[]?> DecryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext = default , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromResult<Byte[]?>(null); }

        return Task.FromResult(DecryptArray(input,certificate,rootcontext.Span));
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptArrayWithHashAsync"]/*'/>*/
    internal static Task<DataItemSecurityEnvelopeArrayHashResult?> DecryptArrayWithHashAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext = default , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromResult<DataItemSecurityEnvelopeArrayHashResult?>(null); }

        return Task.FromResult(DecryptArrayWithHash(input,certificate,rootcontext.Span));
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptArray"]/*'/>*/
    internal static Byte[]? EncryptArray(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlySpan<Byte> rootcontext = default , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || recipients.IsDefaultOrEmpty || chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return null; }

        Byte[] wrappedMaterial = new Byte[WrappedMaterialSize];
        Byte[] aeskey = new Byte[AesKeySize];
        Byte[] basenonceseed = new Byte[BaseNonceSeedSize];

        try
        {
            RandomNumberGenerator.Fill(aeskey);
            RandomNumberGenerator.Fill(basenonceseed);
            aeskey.CopyTo(wrappedMaterial,0);
            basenonceseed.CopyTo(wrappedMaterial,AesKeySize);

            Header header = CreateHeader(recipients,issuer,(UInt64)input.LongLength,(Byte)chunksizepower,rootcontext);
            Byte[] recipientBytes = SerializeRecipientTable(recipients,wrappedMaterial);
            Span<Byte> recipientTableHash = stackalloc Byte[RootContextHashSize];
            Span<Byte> envelopeMetadataHash = stackalloc Byte[EnvelopeMetadataHashSize];
            if(!ComputeRecipientTableHash(recipientBytes,recipientTableHash) || !ComputeEnvelopeMetadataHash(header,recipientTableHash,envelopeMetadataHash)) { return null; }
            Byte[] envelopeMetadataHashBytes = envelopeMetadataHash.ToArray();
            header = new Header(header.VersionByte,header.Flags,header.AlgorithmSuite,header.ChunkSizePower,header.RecipientCount,header.RecipientTableLength,header.OriginalLength,header.RootContextHash,envelopeMetadataHashBytes);
            Byte[] headerBytes = SerializeHeader(header);
            envelopeMetadataHashBytes.CopyTo(headerBytes.AsSpan(18 + RootContextHashSize,EnvelopeMetadataHashSize));
            Int32 capacity = GetEncryptedCapacity(recipients,input.LongLength,chunksizepower) ?? 0;

            using var output = capacity > 0 ? new MemoryStream(capacity) : new MemoryStream();
            output.Write(headerBytes,0,headerBytes.Length);
            output.Write(recipientBytes,0,recipientBytes.Length);

            using var gcm = new AesGcm(aeskey,TagSize);
            Int32 chunkSize = 1 << chunksizepower;
            UInt32 chunkIndex = 0;
            Span<Byte> nonce = stackalloc Byte[12];
            Span<Byte> aad = stackalloc Byte[RootContextHashSize];
            Span<Byte> tag = stackalloc Byte[TagSize];
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] lengths = new Byte[ChunkLengthBlockSize];

            try
            {
                for(Int32 offset = 0; offset < input.Length; chunkIndex++)
                {
                    Int32 readLength = Math.Min(chunkSize,input.Length - offset);

                    if(!DeriveNonce(aeskey,basenonceseed,chunkIndex,nonce) || !BuildPerChunkAadCore(header.RootContextHash,envelopeMetadataHashBytes,chunkIndex,(UInt32)readLength,aad)) { return null; }

                    gcm.Encrypt(nonce,input.AsSpan(offset,readLength),cipher.AsSpan(0,readLength),tag,aad);
                    WriteUInt32BigEndian(lengths.AsSpan(0,4),(UInt32)readLength);
                    WriteUInt32BigEndian(lengths.AsSpan(4,4),(UInt32)readLength);

                    output.Write(lengths,0,lengths.Length);
                    output.Write(cipher,0,readLength);
                    output.WriteByte(TagSize);
                    output.Write(tag);

                    offset += readLength;
                }
            }
            finally
            {
                ZeroMemory(cipher);
                ArrayPool<Byte>.Shared.Return(cipher);
                ZeroMemory(tag);
                ZeroMemory(nonce);
                ZeroMemory(aad);
            }

            return GetMemoryStreamArray(output);
        }
        catch ( CryptographicException ) { return null; }
        finally
        {
            ZeroMemory(wrappedMaterial);
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptArrayWithHash"]/*'/>*/
    internal static DataItemSecurityEnvelopeArrayHashResult? EncryptArrayWithHash(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlySpan<Byte> rootcontext = default , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || recipients.IsDefaultOrEmpty || chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return null; }

        Byte[] wrappedMaterial = new Byte[WrappedMaterialSize];
        Byte[] aeskey = new Byte[AesKeySize];
        Byte[] basenonceseed = new Byte[BaseNonceSeedSize];

        try
        {
            RandomNumberGenerator.Fill(aeskey);
            RandomNumberGenerator.Fill(basenonceseed);
            aeskey.CopyTo(wrappedMaterial,0);
            basenonceseed.CopyTo(wrappedMaterial,AesKeySize);

            Header header = CreateHeader(recipients,issuer,(UInt64)input.LongLength,(Byte)chunksizepower,rootcontext);
            Byte[] recipientBytes = SerializeRecipientTable(recipients,wrappedMaterial);
            Span<Byte> recipientTableHash = stackalloc Byte[RootContextHashSize];
            Span<Byte> envelopeMetadataHash = stackalloc Byte[EnvelopeMetadataHashSize];
            if(!ComputeRecipientTableHash(recipientBytes,recipientTableHash) || !ComputeEnvelopeMetadataHash(header,recipientTableHash,envelopeMetadataHash)) { return null; }
            Byte[] envelopeMetadataHashBytes = envelopeMetadataHash.ToArray();
            header = new Header(header.VersionByte,header.Flags,header.AlgorithmSuite,header.ChunkSizePower,header.RecipientCount,header.RecipientTableLength,header.OriginalLength,header.RootContextHash,envelopeMetadataHashBytes);
            Byte[] headerBytes = SerializeHeader(header);
            envelopeMetadataHashBytes.CopyTo(headerBytes.AsSpan(18 + RootContextHashSize,EnvelopeMetadataHashSize));
            Int32 capacity = GetEncryptedCapacity(recipients,input.LongLength,chunksizepower) ?? 0;

            using var output = capacity > 0 ? new MemoryStream(capacity) : new MemoryStream();
            using var hashingOutput = new HashingWriteStream(output);
            hashingOutput.Write(headerBytes,0,headerBytes.Length);
            hashingOutput.Write(recipientBytes,0,recipientBytes.Length);

            using var gcm = new AesGcm(aeskey,TagSize);
            Int32 chunkSize = 1 << chunksizepower;
            UInt32 chunkIndex = 0;
            Span<Byte> nonce = stackalloc Byte[12];
            Span<Byte> aad = stackalloc Byte[RootContextHashSize];
            Span<Byte> tag = stackalloc Byte[TagSize];
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] lengths = new Byte[ChunkLengthBlockSize];

            try
            {
                for(Int32 offset = 0; offset < input.Length; chunkIndex++)
                {
                    Int32 readLength = Math.Min(chunkSize,input.Length - offset);

                    if(!DeriveNonce(aeskey,basenonceseed,chunkIndex,nonce) || !BuildPerChunkAadCore(header.RootContextHash,envelopeMetadataHashBytes,chunkIndex,(UInt32)readLength,aad)) { return null; }

                    gcm.Encrypt(nonce,input.AsSpan(offset,readLength),cipher.AsSpan(0,readLength),tag,aad);
                    WriteUInt32BigEndian(lengths.AsSpan(0,4),(UInt32)readLength);
                    WriteUInt32BigEndian(lengths.AsSpan(4,4),(UInt32)readLength);

                    hashingOutput.Write(lengths,0,lengths.Length);
                    hashingOutput.Write(cipher,0,readLength);
                    hashingOutput.WriteByte(TagSize);
                    hashingOutput.Write(tag);

                    offset += readLength;
                }

                return new(GetMemoryStreamArray(output),hashingOutput.GetHashAndReset());
            }
            finally
            {
                ZeroMemory(cipher);
                ArrayPool<Byte>.Shared.Return(cipher);
                ZeroMemory(tag);
                ZeroMemory(nonce);
                ZeroMemory(aad);
            }
        }
        catch ( CryptographicException ) { return null; }
        finally
        {
            ZeroMemory(wrappedMaterial);
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptArrayAsync"]/*'/>*/
    internal static Task<Byte[]?> EncryptArrayAsync(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext = default , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromResult<Byte[]?>(null); }

        return Task.FromResult(EncryptArray(input,recipients,issuer,rootcontext.Span,chunksizepower));
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptArrayWithHashAsync"]/*'/>*/
    internal static Task<DataItemSecurityEnvelopeArrayHashResult?> EncryptArrayWithHashAsync(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext = default , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromResult<DataItemSecurityEnvelopeArrayHashResult?>(null); }

        return Task.FromResult(EncryptArrayWithHash(input,recipients,issuer,rootcontext.Span,chunksizepower));
    }
}
