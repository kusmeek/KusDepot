namespace KusDepot.Security.Data;

internal static partial class DataItemSecurityEnvelopeV1
{
    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="BuildPerChunkAad"]/*'/>*/
    private static Boolean BuildPerChunkAad(in Header header , UInt32 chunkindex , UInt32 plainlength , Span<Byte> aadout)
        => BuildPerChunkAadCore(header.RootContextHash,header.EnvelopeMetadataHash,chunkindex,plainlength,aadout);

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="BuildPerChunkAadCore"]/*'/>*/
    private static Boolean BuildPerChunkAadCore(ReadOnlySpan<Byte> rootcontexthash , ReadOnlySpan<Byte> envelopemetadatahash , UInt32 chunkindex , UInt32 plainlength , Span<Byte> aadout)
    {
        if(aadout.Length < RootContextHashSize || rootcontexthash.Length != RootContextHashSize || envelopemetadatahash.Length != EnvelopeMetadataHashSize) { return false; }

        Byte[]? rent = null;
        Int32 rawlength = RootContextHashSize + EnvelopeMetadataHashSize + 4 + 4;
        Span<Byte> raw = rawlength <= 128 ? stackalloc Byte[rawlength] : (rent = ArrayPool<Byte>.Shared.Rent(rawlength)).AsSpan(0,rawlength);

        try
        {
            Int32 offset = 0;
            rootcontexthash.CopyTo(raw.Slice(offset,RootContextHashSize)); offset += RootContextHashSize;
            envelopemetadatahash.CopyTo(raw.Slice(offset,EnvelopeMetadataHashSize)); offset += EnvelopeMetadataHashSize;
            WriteUInt32BigEndian(raw.Slice(offset,4),chunkindex); offset += 4;
            WriteUInt32BigEndian(raw.Slice(offset,4),plainlength); offset += 4;

            return offset == rawlength && SHA512.TryHashData(raw,aadout,out Int32 written) && written == RootContextHashSize;
        }
        finally
        {
            if(rent is not null)
            {
                ZeroMemory(rent);
                ArrayPool<Byte>.Shared.Return(rent);
            }
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="CreateHeader"]/*'/>*/
    private static Header CreateHeader(ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , UInt64 originallength , Byte chunksizepower , ReadOnlySpan<Byte> rootcontext)
    {
        Byte flags = 0;

        if(issuer is not null) { flags |= 1 << 0; }
        if(recipients.Any(_=>_.ObjectId.HasValue)) { flags |= 1 << 1; }
        if(recipients.Any(_=>_.PublicKeyHash is not null && _.PublicKeyHash.Length > 0)) { flags |= 1 << 2; }

        Byte[] root = SHA512.HashData(rootcontext);
        Int32 recipientTableLength = GetRecipientTableLength(recipients);

        return new Header(Version,flags,AlgorithmSuiteRsaOaepSha512Aes256Gcm,chunksizepower,(UInt16)recipients.Length,(UInt32)recipientTableLength,originallength,root,new Byte[EnvelopeMetadataHashSize]);
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="ComputeEnvelopeMetadataHash"]/*'/>*/
    private static Boolean ComputeEnvelopeMetadataHash(in Header header , ReadOnlySpan<Byte> recipienttablehash , Span<Byte> metadatahashout)
    {
        if(recipienttablehash.Length != RootContextHashSize || metadatahashout.Length < EnvelopeMetadataHashSize) { return false; }

        Byte[]? rent = null;
        Int32 rawlength = 1 + 1 + 1 + 1 + RecipientCountSize + RecipientTableLengthSize + 8 + RootContextHashSize + RootContextHashSize;
        Span<Byte> raw = rawlength <= 192 ? stackalloc Byte[rawlength] : (rent = ArrayPool<Byte>.Shared.Rent(rawlength)).AsSpan(0,rawlength);

        try
        {
            Int32 offset = 0;
            raw[offset++] = header.VersionByte;
            raw[offset++] = header.Flags;
            raw[offset++] = header.AlgorithmSuite;
            raw[offset++] = header.ChunkSizePower;
            WriteUInt16BigEndian(raw.Slice(offset,RecipientCountSize),header.RecipientCount); offset += RecipientCountSize;
            WriteUInt32BigEndian(raw.Slice(offset,RecipientTableLengthSize),header.RecipientTableLength); offset += RecipientTableLengthSize;
            WriteUInt64BigEndian(raw.Slice(offset,8),header.OriginalLength); offset += 8;
            header.RootContextHash.AsSpan().CopyTo(raw.Slice(offset,RootContextHashSize)); offset += RootContextHashSize;
            recipienttablehash.CopyTo(raw.Slice(offset,RootContextHashSize)); offset += RootContextHashSize;

            return offset == rawlength && SHA512.TryHashData(raw,metadatahashout,out Int32 written) && written == EnvelopeMetadataHashSize;
        }
        finally
        {
            if(rent is not null)
            {
                ZeroMemory(rent);
                ArrayPool<Byte>.Shared.Return(rent);
            }
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="ComputeRecipientFlags"]/*'/>*/
    private static Byte ComputeRecipientFlags(DataSecurityRecipient recipient)
    {
        Byte flags = 0;

        if(recipient.ObjectId.HasValue) { flags |= RecipientObjectIdFlag; }
        if(recipient.PublicKeyHash is not null && recipient.PublicKeyHash.Length > 0) { flags |= RecipientPublicKeyHashFlag; }

        return flags;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="ComputeRecipientTableHash"]/*'/>*/
    private static Boolean ComputeRecipientTableHash(ReadOnlySpan<Byte> recipienttable , Span<Byte> hashout)
    {
        return hashout.Length >= RootContextHashSize && SHA512.TryHashData(recipienttable,hashout,out Int32 written) && written == RootContextHashSize;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DeriveNonce"]/*'/>*/
    private static Boolean DeriveNonce(ReadOnlySpan<Byte> aeskey , ReadOnlySpan<Byte> basenonceseed , UInt64 chunkindex , Span<Byte> nonceout)
    {
        if(aeskey.Length != AesKeySize || basenonceseed.Length != BaseNonceSeedSize || nonceout.Length < 12) { return false; }

        Span<Byte> input = stackalloc Byte[BaseNonceSeedSize + 8];
        basenonceseed.CopyTo(input);
        WriteUInt64BigEndian(input.Slice(BaseNonceSeedSize,8),chunkindex);
        Span<Byte> mac = stackalloc Byte[HMACSHA512.HashSizeInBytes];

        if(!HMACSHA512.TryHashData(aeskey,input,mac,out Int32 written) || written != HMACSHA512.HashSizeInBytes) { return false; }

        mac[..12].CopyTo(nonceout);
        ZeroMemory(mac);

        return true;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="GetEncryptedCapacity"]/*'/>*/
    internal static Int32? GetEncryptedCapacity(ImmutableArray<DataSecurityRecipient> recipients , Int64 plaintextlength , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(plaintextlength < 0 || chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower || recipients.IsDefaultOrEmpty) { return null; }

        Int32 recipientTableLength = 0;

        foreach(var recipient in recipients)
        {
            Int32? entryLength = GetRecipientEntryLength(recipient);
            if(!entryLength.HasValue) { return null; }

            recipientTableLength = checked(recipientTableLength + entryLength.Value);
        }

        Int64 chunkSize = 1L << chunksizepower;
        Int64 chunkCount = plaintextlength == 0 ? 0 : (plaintextlength + chunkSize - 1) / chunkSize;
        Int64 bodyLength = plaintextlength + (chunkCount * (ChunkLengthBlockSize + TagSizeLengthFieldSize + TagSize));
        Int64 totalLength = FixedHeaderSize + recipientTableLength + bodyLength;

        return totalLength > Int32.MaxValue ? null : (Int32)totalLength;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="GetMemoryStreamArray"]/*'/>*/
    private static Byte[] GetMemoryStreamArray(MemoryStream stream)
    {
        if(stream.TryGetBuffer(out ArraySegment<Byte> buffer) && buffer.Offset == 0 && buffer.Array is not null && buffer.Array.Length == buffer.Count) { return buffer.Array; }

        return stream.ToArray();
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="GetRecipientEntryLength"]/*'/>*/
    private static Int32 GetRecipientEntryLength(in RecipientEntry recipient)
    {
        Int32 length = 1 + 2 + Encoding.UTF8.GetByteCount(recipient.Thumbprint) + 2 + recipient.WrappedMaterial.Length;

        if(recipient.ObjectId.HasValue) { length += GuidSize; }
        if(!recipient.PublicKeyHash.IsDefaultOrEmpty) { length += recipient.PublicKeyHash.Length; }

        return length;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="GetRecipientEntryLengthRecipient"]/*'/>*/
    private static Int32? GetRecipientEntryLength(DataSecurityRecipient recipient)
    {
        String? thumbprint = NormalizeThumbprint(recipient.Thumbprint ?? recipient.Certificate?.Thumbprint);
        Int32? wrappedMaterialLength = GetWrappedMaterialLength(recipient.Certificate);

        if(String.IsNullOrWhiteSpace(thumbprint) || !wrappedMaterialLength.HasValue) { return null; }
        if(recipient.PublicKeyHash is not null && recipient.PublicKeyHash.Length != 0 && recipient.PublicKeyHash.Length != PublicKeyHashSize) { return null; }

        Int32 length = 1 + 2 + Encoding.UTF8.GetByteCount(thumbprint) + 2 + wrappedMaterialLength.Value;

        if(recipient.ObjectId.HasValue) { length += GuidSize; }
        if(recipient.PublicKeyHash is not null && recipient.PublicKeyHash.Length > 0) { length += PublicKeyHashSize; }

        return length;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="GetRecipientTableLength"]/*'/>*/
    private static Int32 GetRecipientTableLength(ImmutableArray<DataSecurityRecipient> recipients)
    {
        return GetEncryptedCapacity(recipients,0,DefaultChunkSizePower) is Int32 total ? total - FixedHeaderSize : 0;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="GetWrappedMaterialLength"]/*'/>*/
    private static Int32? GetWrappedMaterialLength(X509Certificate2? certificate)
    {
        if(certificate is null) { return null; }

        using RSA? rsa = certificate.GetRSAPublicKey();

        if(rsa is null || rsa.KeySize <= 0 || (rsa.KeySize & 7) != 0) { return null; }

        return rsa.KeySize >> 3;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="NormalizeThumbprint"]/*'/>*/
    private static String? NormalizeThumbprint(String? thumbprint)
    {
        if(String.IsNullOrWhiteSpace(thumbprint)) { return null; }

        return thumbprint.Replace(" ",String.Empty,Ordinal).ToUpperInvariant();
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="ReadExact"]/*'/>*/
    private static Boolean ReadExact(Stream stream , Span<Byte> buffer)
    {
        Int32 total = 0;

        while(total < buffer.Length)
        {
            Int32 read = stream.Read(buffer[total..]);
            if(read == 0) { return false; }
            total += read;
        }

        return true;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="ReadExactAsync"]/*'/>*/
    private static async Task<Boolean> ReadExactAsync(Stream stream , Memory<Byte> buffer , CancellationToken cancel)
    {
        Int32 total = 0;

        while(total < buffer.Length)
        {
            Int32 read = await stream.ReadAsync(buffer[total..],cancel).ConfigureAwait(false);
            if(read == 0) { return false; }
            total += read;
        }

        return true;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="SerializeHeader"]/*'/>*/
    private static Byte[] SerializeHeader(in Header header)
    {
        Byte[] buffer = new Byte[FixedHeaderSize];
        Int32 offset = 0;

        buffer[offset++] = header.VersionByte;
        buffer[offset++] = header.Flags;
        buffer[offset++] = header.AlgorithmSuite;
        buffer[offset++] = header.ChunkSizePower;
        WriteUInt16BigEndian(buffer.AsSpan(offset,RecipientCountSize),header.RecipientCount); offset += RecipientCountSize;
        WriteUInt32BigEndian(buffer.AsSpan(offset,RecipientTableLengthSize),header.RecipientTableLength); offset += RecipientTableLengthSize;
        WriteUInt64BigEndian(buffer.AsSpan(offset,8),header.OriginalLength); offset += 8;
        header.RootContextHash.AsSpan().CopyTo(buffer.AsSpan(offset,RootContextHashSize));

        return buffer;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="SerializeRecipientTable"]/*'/>*/
    private static Byte[] SerializeRecipientTable(ImmutableArray<DataSecurityRecipient> recipients , ReadOnlySpan<Byte> wrappedmaterial)
    {
        Int32 length = GetRecipientTableLength(recipients);
        Byte[] buffer = new Byte[length];
        var writer = new BufferWriter(buffer);

        foreach(var recipient in recipients)
        {
            X509Certificate2 certificate = recipient.Certificate ?? throw new InvalidOperationException();
            String thumbprint = NormalizeThumbprint(recipient.Thumbprint ?? certificate.Thumbprint) ?? throw new InvalidOperationException();
            using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null) { throw new InvalidOperationException(); }

            Byte flags = ComputeRecipientFlags(recipient);
            Byte[] thumbprintbytes = Encoding.UTF8.GetBytes(thumbprint);
            Byte[] wrapped = rsa.Encrypt(wrappedmaterial,RSAEncryptionPadding.OaepSHA512);

            if(!writer.TryWriteByte(flags) || !writer.TryWriteUInt16BigEndian((UInt16)thumbprintbytes.Length) || !writer.TryWriteBytes(thumbprintbytes)) { throw new InvalidOperationException(); }
            if(recipient.ObjectId.HasValue && !writer.TryWriteGuid(recipient.ObjectId.Value)) { throw new InvalidOperationException(); }
            if(recipient.PublicKeyHash is not null && recipient.PublicKeyHash.Length > 0 && !writer.TryWriteBytes(recipient.PublicKeyHash)) { throw new InvalidOperationException(); }
            if(!writer.TryWriteUInt16BigEndian((UInt16)wrapped.Length) || !writer.TryWriteBytes(wrapped)) { throw new InvalidOperationException(); }
        }

        if(writer.Position != length) { throw new InvalidOperationException(); }

        return buffer;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="TryGetRemainingMemory"]/*'/>*/
    private static Boolean TryGetRemainingMemory(Stream? stream , out ReadOnlyMemory<Byte> memory)
    {
        memory = default;

        if(stream is not MemoryStream ms || !ms.TryGetBuffer(out ArraySegment<Byte> buffer) || buffer.Array is null) { return false; }

        Int64 position = ms.Position;
        if(position < 0 || position > buffer.Count) { return false; }

        Int32 offset = buffer.Offset + (Int32)position;
        Int32 count = buffer.Count - (Int32)position;
        memory = new ReadOnlyMemory<Byte>(buffer.Array,offset,count);

        return true;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="TryParseHeader"]/*'/>*/
    private static Boolean TryParseHeader(ReadOnlySpan<Byte> input , out Header header)
    {
        header = default!;

        if(input.Length < FixedHeaderSize) { return false; }

        Byte version = input[0];
        Byte flags = input[1];
        Byte algorithmsuite = input[2];
        Byte chunksizepower = input[3];
        UInt16 recipientcount = ReadUInt16BigEndian(input.Slice(4,RecipientCountSize));
        UInt32 recipienttablelength = ReadUInt32BigEndian(input.Slice(6,RecipientTableLengthSize));
        UInt64 originallength = ReadUInt64BigEndian(input.Slice(10,8));
        Byte[] rootcontexthash = input.Slice(18,RootContextHashSize).ToArray();
        Byte[] envelopemetadatahash = input.Slice(18 + RootContextHashSize,EnvelopeMetadataHashSize).ToArray();

        if(version != Version || algorithmsuite != AlgorithmSuiteRsaOaepSha512Aes256Gcm || (flags & ~((Byte)0x07)) != 0 ||
            recipientcount == 0 || chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower)
        { return false; }

        header = new Header(version,flags,algorithmsuite,chunksizepower,recipientcount,recipienttablelength,originallength,rootcontexthash,envelopemetadatahash);

        return true;
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="TryParseRecipientTable"]/*'/>*/
    private static Boolean TryParseRecipientTable(ReadOnlySpan<Byte> input , UInt16 recipientcount , out ImmutableArray<RecipientEntry> recipients)
    {
        recipients = [];

        try
        {
            var reader = new BufferSpanReader(input);
            var builder = ImmutableArray.CreateBuilder<RecipientEntry>(recipientcount);

            for(Int32 i = 0; i < recipientcount; i++)
            {
                if(!reader.TryReadByte(out Byte flags) || !reader.TryReadUInt16BigEndian(out UInt16 thumbprintlength) || thumbprintlength == 0 ||
                    !reader.TryReadBytes((Int32)thumbprintlength,out ReadOnlySpan<Byte> thumbprintbytes) || !Utf8.IsValid(thumbprintbytes))
                { return false; }

                String thumbprint = NormalizeThumbprint(Encoding.UTF8.GetString(thumbprintbytes)) ?? String.Empty;
                Guid? objectid = null;
                ImmutableArray<Byte> publickeyhash = [];

                if((flags & RecipientObjectIdFlag) != 0)
                {
                    if(!reader.TryReadGuid(out Guid parsed)) { return false; }

                    objectid = parsed;
                }

                if((flags & RecipientPublicKeyHashFlag) != 0)
                {
                    if(!reader.TryReadBytes(PublicKeyHashSize,out ReadOnlySpan<Byte> hashbytes)) { return false; }

                    publickeyhash = ImmutableArray.Create(hashbytes.ToArray());
                }

                if(!reader.TryReadUInt16BigEndian(out UInt16 wrappedlength) || wrappedlength == 0 || !reader.TryReadBytes((Int32)wrappedlength,out ReadOnlySpan<Byte> wrappedbytes)) { return false; }

                builder.Add(new RecipientEntry(flags,thumbprint,objectid,publickeyhash,ImmutableArray.Create(wrappedbytes.ToArray())));
            }

            if(reader.Remaining != 0) { return false; }

            recipients = builder.MoveToImmutable();

            return true;
        }
        catch
        {
            recipients = [];

            return false;
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="TryResolveRecipientMaterial"]/*'/>*/
    private static Boolean TryResolveRecipientMaterial(ImmutableArray<RecipientEntry> recipients , X509Certificate2 certificate , out Byte[]? aeskey , out Byte[]? basenonceseed)
    {
        aeskey = null;
        basenonceseed = null;

        using RSA? rsa = certificate.GetRSAPrivateKey();
        if(rsa is null) { return false; }

        String? thumbprint = NormalizeThumbprint(certificate.Thumbprint);
        Byte[]? publickeyhash = null;

        try
        {
            Byte[]? publickey = certificate.GetPublicKey();
            if(publickey is not null && publickey.Length > 0) { publickeyhash = SHA256.HashData(publickey); }

            IEnumerable<RecipientEntry> ordered = recipients
                .OrderByDescending(_=>String.Equals(_.Thumbprint,thumbprint,Ordinal))
                .ThenByDescending(_=>publickeyhash is not null && !_.PublicKeyHash.IsDefaultOrEmpty && FixedTimeEquals(_.PublicKeyHash.AsSpan(),publickeyhash));

            foreach(var recipient in ordered)
            {
                try
                {
                    Byte[] material = rsa.Decrypt(recipient.WrappedMaterial.AsSpan(),RSAEncryptionPadding.OaepSHA512);
                    if(material.Length != WrappedMaterialSize) { continue; }

                    aeskey = material.AsSpan(0,AesKeySize).ToArray();
                    basenonceseed = material.AsSpan(AesKeySize,BaseNonceSeedSize).ToArray();

                    ZeroMemory(material);

                    return true;
                }
                catch ( CryptographicException ) { }
            }

            return false;
        }
        finally
        {
            ZeroMemory(publickeyhash);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="TryValidateRootContext"]/*'/>*/
    private static Boolean TryValidateRootContext(ReadOnlySpan<Byte> rootcontext , ReadOnlySpan<Byte> expectedhash)
    {
        Span<Byte> computed = stackalloc Byte[RootContextHashSize];

        if(!SHA512.TryHashData(rootcontext,computed,out Int32 written) || written != RootContextHashSize) { return false; }

        return FixedTimeEquals(computed,expectedhash);
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="TryValidateEnvelopeMetadata"]/*'/>*/
    private static Boolean TryValidateEnvelopeMetadata(in Header header , ReadOnlySpan<Byte> recipienttable , ReadOnlySpan<Byte> expectedmetadatahash)
    {
        if(expectedmetadatahash.Length != EnvelopeMetadataHashSize) { return false; }

        Span<Byte> recipienttablehash = stackalloc Byte[RootContextHashSize];
        Span<Byte> computedmetadatahash = stackalloc Byte[EnvelopeMetadataHashSize];

        if(!ComputeRecipientTableHash(recipienttable,recipienttablehash) || !ComputeEnvelopeMetadataHash(header,recipienttablehash,computedmetadatahash)) { return false; }

        return FixedTimeEquals(computedmetadatahash,expectedmetadatahash);
    }
}