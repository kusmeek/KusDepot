namespace KusDepot.Cryptography;

internal static partial class EncryptionEnvelopeV1
{
    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryGetRemainingMemory"]/*'/>*/
    private static Boolean TryGetRemainingMemory(Stream? stream , out ReadOnlyMemory<Byte> memory)
    {
        memory = default;

        if(stream is not MemoryStream ms || ms.TryGetBuffer(out ArraySegment<Byte> buffer) is false || buffer.Array is null) { return false; }

        Int64 position = ms.Position;

        if(position < 0 || position > buffer.Count) { return false; }

        Int32 offset = buffer.Offset + (Int32)position;
        Int32 count = buffer.Count - (Int32)position;

        memory = new ReadOnlyMemory<Byte>(buffer.Array,offset,count);

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="GetMemoryStreamArray"]/*'/>*/
    private static Byte[] GetMemoryStreamArray(MemoryStream stream)
    {
        if(stream.TryGetBuffer(out ArraySegment<Byte> buffer) && buffer.Offset == 0 && buffer.Array is not null && buffer.Array.Length == buffer.Count) { return buffer.Array; }

        return stream.ToArray();
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="GetEncryptedArrayCapacity"]/*'/>*/
    internal static Int32? GetEncryptedArrayCapacity(X509Certificate2 certificate , Int64 plaintextLength , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(plaintextLength < 0 || plaintextLength > Int32.MaxValue || chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return null; }

        using RSA? rsa = certificate.GetRSAPublicKey();

        if(rsa is null || rsa.KeySize <= 0 || (rsa.KeySize & 7) != 0) { return null; }

        Int64 rsaWrappedLen = rsa.KeySize / 8;
        Int64 chunkSize = 1L << chunksizepower;
        Int64 chunkCount = plaintextLength / chunkSize;
        if(plaintextLength % chunkSize != 0) { chunkCount = checked(chunkCount + 1); }
        Int64 headerLength = FixedHeaderSize + rsaWrappedLen + (includeoriginallength ? LongSize : 0) + (includeaadhash ? AadHashSize : 0);
        Int64 bodyLength = plaintextLength + (chunkCount * (ChunkLengthsBlockSize + TagSizeLengthFieldSize + TagSize));
        Int64 totalLength = checked(headerLength + bodyLength + TerminalRecordSize);

        return totalLength > Int32.MaxValue ? null : (Int32)totalLength;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="BuildPerChunkAad"]/*'/>*/
    private static Boolean BuildPerChunkAad(Byte flags , UInt64 chunkindex , UInt32 plainlen , Boolean hasoriginallength , UInt64 originallength , ReadOnlySpan<Byte> aadhash , Boolean hasaadhash , Span<Byte> aadout)
    {
        if(aadout.Length < PerChunkAadHashSize) { return false; }

        Byte[]? rent = null;
        Int32 rawLen = 1 + 1 + LongSize + IntSize + (hasoriginallength ? LongSize : 0) + (hasaadhash ? AadHashSize : 0);
        Span<Byte> raw = rawLen <= 128 ? stackalloc Byte[rawLen] : (rent = ArrayPool<Byte>.Shared.Rent(rawLen)).AsSpan(0,rawLen);
        try
        {
            Int32 offset = 0; raw[offset++] = Version; raw[offset++] = flags;
            WriteUInt64BigEndian(raw.Slice(offset,LongSize),chunkindex); offset += LongSize;
            WriteUInt32BigEndian(raw.Slice(offset,IntSize),plainlen); offset += IntSize;
            if(hasoriginallength)
            {
                WriteUInt64BigEndian(raw.Slice(offset,LongSize),originallength); offset += LongSize;
            }
            if(hasaadhash)
            {
                if(aadhash.Length != AadHashSize) { return false; }
                aadhash.CopyTo(raw.Slice(offset,AadHashSize)); offset += AadHashSize;
            }
            if(offset != rawLen) { return false; }
            if(SHA512.TryHashData(raw,aadout,out Int32 written) is false || written != PerChunkAadHashSize) { return false; }

            return true;
        }
        finally { if(rent is not null) { ZeroMemory(rent); ArrayPool<Byte>.Shared.Return(rent); } }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="ComputeHeaderLength"]/*'/>*/
    private static Int32 ComputeHeaderLength(UInt16 rsawrappedlen , Boolean hasoriginallen , Boolean hasaadhash)
    {
        Int32 len = FixedHeaderSize + rsawrappedlen;
        if(hasoriginallen) { len += LongSize; }
        if(hasaadhash) { len += AadHashSize; }

        return len;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DeriveNonce"]/*'/>*/
    private static Boolean DeriveNonce(ReadOnlySpan<Byte> aeskey , ReadOnlySpan<Byte> basenonceseed , UInt64 chunkindex , Span<Byte> nonceout)
    {
        if(aeskey.Length != AesKeySize)               { return false; }
        if(nonceout.Length < NonceSize)               { return false; }
        if(basenonceseed.Length != BaseNonceSeedSize) { return false; }

        Span<Byte> input = stackalloc Byte[BaseNonceSeedSize + LongSize];
        basenonceseed.CopyTo(input); WriteUInt64BigEndian(input.Slice(BaseNonceSeedSize,LongSize),chunkindex);
        Span<Byte> mac = stackalloc Byte[HMACSHA512.HashSizeInBytes];
        if(HMACSHA512.TryHashData(aeskey,input,mac,out Int32 written) is false || written != HMACSHA512.HashSizeInBytes) { return false; }

        mac[..NonceSize].CopyTo(nonceout); ZeroMemory(mac);

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="ParseHeaderForAsync"]/*'/>*/
    private static Boolean ParseHeaderForAsync(ReadOnlySpan<Byte> headerbuffer , out Header header, out Byte[] rsawrappedcopy)
    {
        if(TryParseHeader(headerbuffer,out header,out ReadOnlySpan<Byte> rsaWrappedKey) is false) { rsawrappedcopy = Array.Empty<Byte>(); return false; }
        rsawrappedcopy = rsaWrappedKey.ToArray();

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="ReadExact"]/*'/>*/
    private static Boolean ReadExact(Stream s , Span<Byte> span)
    {
        Int32 total = 0; Int32 read;
        while(total < span.Length && (read = s.Read(span[total..])) > 0) { total += read; }

        return total == span.Length;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="ReadExactArray"]/*'/>*/
    private static Boolean ReadExact(Stream s , Byte[] buffer , Int32 offset , Int32 count)
    {
        Int32 total = 0; Int32 read;
        while(total < count && (read = s.Read(buffer,offset + total,count - total)) > 0) { total += read; }

        return total == count;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="ReadExactAsync"]/*'/>*/
    private static async Task<Boolean> ReadExactAsync(Stream s , Memory<Byte> buffer , CancellationToken cancel)
    {
        Int32 total = 0;
        while(total < buffer.Length)
        {
            Int32 r = await s.ReadAsync(buffer[total..],cancel).ConfigureAwait(false); if(r == 0) { return false; }
            total += r;
        }

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="BuildTerminalAad"]/*'/>*/
    private static Boolean BuildTerminalAad(in Header header , ReadOnlySpan<Byte> aadhash , UInt64 finalchunkcount , UInt64 totalplaintextlength , Span<Byte> aadout)
    {
        if(aadout.Length < PerChunkAadHashSize || (header.HasAadHash ? aadhash.Length != AadHashSize : !aadhash.IsEmpty)) { return false; }

        Int32 rawLength = VersionSize + FlagsSize + ChunkSizePowerSize + (header.HasOriginalLength ? LongSize : 0) + (header.HasAadHash ? AadHashSize : 0) + 2 * LongSize;
        Span<Byte> raw = stackalloc Byte[rawLength];
        Int32 offset = 0;
        raw[offset++] = header.VersionByte;
        raw[offset++] = header.Flags;
        raw[offset++] = header.ChunkSizePower;
        if(header.HasOriginalLength) { WriteUInt64BigEndian(raw.Slice(offset,LongSize),header.OriginalLength); offset += LongSize; }
        if(header.HasAadHash) { aadhash.CopyTo(raw.Slice(offset,AadHashSize)); offset += AadHashSize; }
        WriteUInt64BigEndian(raw.Slice(offset,LongSize),finalchunkcount); offset += LongSize;
        WriteUInt64BigEndian(raw.Slice(offset,LongSize),totalplaintextlength); offset += LongSize;

        return offset == rawLength && SHA512.TryHashData(raw,aadout,out Int32 written) && written == PerChunkAadHashSize;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryParseTerminalRecord"]/*'/>*/
    private static Boolean TryParseTerminalRecord(ReadOnlySpan<Byte> buffer , out TerminalRecord terminal)
    {
        terminal = default;
        if(buffer.Length != TerminalRecordSize || buffer[0] != TerminalMarker) { return false; }

        terminal.FinalChunkCount = ReadUInt64BigEndian(buffer.Slice(1,LongSize));
        terminal.TotalPlaintextLength = ReadUInt64BigEndian(buffer.Slice(1 + LongSize,LongSize));

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryAuthenticateTerminalRecord"]/*'/>*/
    private static Boolean TryAuthenticateTerminalRecord(ReadOnlySpan<Byte> buffer , in Header header , ReadOnlySpan<Byte> aeskey , ReadOnlySpan<Byte> basenonceseed , UInt64 observedchunkcount , UInt64 observedplaintextlength)
    {
        if(!TryParseTerminalRecord(buffer,out TerminalRecord terminal) || terminal.FinalChunkCount != observedchunkcount || terminal.TotalPlaintextLength != observedplaintextlength || (header.HasOriginalLength && header.OriginalLength != observedplaintextlength)) { return false; }

        Span<Byte> nonce = stackalloc Byte[NonceSize];
        Span<Byte> aad = stackalloc Byte[PerChunkAadHashSize];

        try
        {
            if(!DeriveNonce(aeskey,basenonceseed,terminal.FinalChunkCount,nonce) || !BuildTerminalAad(header,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,terminal.FinalChunkCount,terminal.TotalPlaintextLength,aad)) { return false; }

            using var gcm = new AesGcm(aeskey,TagSize);
            gcm.Decrypt(nonce,ReadOnlySpan<Byte>.Empty,buffer.Slice(1 + 2 * LongSize,TerminalTagSize),Span<Byte>.Empty,aad);

            return true;
        }
        catch(CryptographicException) { return false; }
        finally { ZeroMemory(nonce); ZeroMemory(aad); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="WriteTerminalRecordStream"]/*'/>*/
    private static Boolean WriteTerminalRecordStream(Stream output , in Header header , ReadOnlySpan<Byte> aadhash , ReadOnlySpan<Byte> aeskey , ReadOnlySpan<Byte> basenonceseed , UInt64 finalchunkcount , UInt64 totalplaintextlength)
    {
        Span<Byte> record = stackalloc Byte[TerminalRecordSize];
        Span<Byte> nonce = stackalloc Byte[NonceSize];
        Span<Byte> aad = stackalloc Byte[PerChunkAadHashSize];

        try
        {
            if(!DeriveNonce(aeskey,basenonceseed,finalchunkcount,nonce) || !BuildTerminalAad(header,aadhash,finalchunkcount,totalplaintextlength,aad)) { return false; }
            using var gcm = new AesGcm(aeskey,TagSize);
            gcm.Encrypt(nonce,ReadOnlySpan<Byte>.Empty,Span<Byte>.Empty,record.Slice(1 + 2 * LongSize,TerminalTagSize),aad);

            record[0] = TerminalMarker;
            WriteUInt64BigEndian(record.Slice(1,LongSize),finalchunkcount);
            WriteUInt64BigEndian(record.Slice(1 + LongSize,LongSize),totalplaintextlength);
            output.Write(record);

            return true;
        }
        catch { return false; }
        finally { ZeroMemory(record); ZeroMemory(nonce); ZeroMemory(aad); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="WriteTerminalRecordSpan"]/*'/>*/
    private static Boolean WriteTerminalRecordSpan(Span<Byte> output , in Header header , ReadOnlySpan<Byte> aadhash , ReadOnlySpan<Byte> aeskey , ReadOnlySpan<Byte> basenonceseed , UInt64 finalchunkcount , UInt64 totalplaintextlength)
    {
        if(output.Length < TerminalRecordSize) { return false; }

        Span<Byte> nonce = stackalloc Byte[NonceSize];
        Span<Byte> aad = stackalloc Byte[PerChunkAadHashSize];
        try
        {
            if(!DeriveNonce(aeskey,basenonceseed,finalchunkcount,nonce) || !BuildTerminalAad(header,aadhash,finalchunkcount,totalplaintextlength,aad)) { return false; }
            using var gcm = new AesGcm(aeskey,TagSize);
            gcm.Encrypt(nonce,ReadOnlySpan<Byte>.Empty,Span<Byte>.Empty,output.Slice(1 + 2 * LongSize,TerminalTagSize),aad);
            output[0] = TerminalMarker;
            WriteUInt64BigEndian(output.Slice(1,LongSize),finalchunkcount);
            WriteUInt64BigEndian(output.Slice(1 + LongSize,LongSize),totalplaintextlength);

            return true;
        }
        finally { ZeroMemory(nonce); ZeroMemory(aad); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryParseHeader"]/*'/>*/
    private static Boolean TryParseHeader(ReadOnlySpan<Byte> buffer , out Header parsed , out ReadOnlySpan<Byte> rsawrappedkey)
    {
        parsed = default;
        rsawrappedkey = default;
        if(buffer.Length < FixedHeaderSize)                                                                       { return false; }
        parsed.VersionByte = buffer[0]; if(parsed.VersionByte != Version)                                         { return false; }
        parsed.Flags = buffer[VersionSize]; if((parsed.Flags & ~(Flag_HasOriginalLength | Flag_HasAadHash)) != 0) { return false; }
        parsed.HasOriginalLength = (parsed.Flags & Flag_HasOriginalLength) != 0;
        parsed.HasAadHash = (parsed.Flags & Flag_HasAadHash) != 0;
        parsed.RsaWrappedLen = ReadUInt16BigEndian(buffer.Slice(VersionSize + FlagsSize,RsaWrappedLenSize));
        Int32 offset = VersionSize + FlagsSize + RsaWrappedLenSize;
        if(buffer.Length < offset + parsed.RsaWrappedLen + ChunkSizePowerSize)                                    { return false; }
        rsawrappedkey = buffer.Slice(offset,parsed.RsaWrappedLen); offset += parsed.RsaWrappedLen;
        parsed.ChunkSizePower = buffer[offset++];
        if(parsed.ChunkSizePower < MinChunkSizePower || parsed.ChunkSizePower > MaxChunkSizePower)                { return false; }
        if(parsed.HasOriginalLength)
        {
            if(buffer.Length < offset + LongSize)                                                                 { return false; }
            parsed.OriginalLength = ReadUInt64BigEndian(buffer.Slice(offset,LongSize)); offset += LongSize;
        }
        if(parsed.HasAadHash)
        {
            if(buffer.Length < offset + AadHashSize)                                                              { return false; }
            parsed.AadHash = buffer.Slice(offset,AadHashSize).ToArray(); offset += AadHashSize;
        }
        parsed.HeaderTotalLength = offset;

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryReadAndProcessHeaderSpan"]/*'/>*/
    private static Boolean TryReadAndProcessHeader(ReadOnlySpan<Byte> input , X509Certificate2 certificate , ReadOnlySpan<Byte> aad , out Header header , out ReadOnlySpan<Byte> body , Span<Byte> aeskey , Span<Byte> basenonceseed)
    {
        header = default; body = default;
        Span<Byte> wrappedMaterial = stackalloc Byte[WrappedMaterialSize];
        Span<Byte> calc = stackalloc Byte[AadHashSize];

        try
        {
            if(!TryParseHeader(input,out header,out var rsaWrappedKey)) { return false; }

            using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null) { return false; }

            if(!rsa.TryDecrypt(rsaWrappedKey,wrappedMaterial,RSAEncryptionPadding.OaepSHA512,out Int32 written) || written != WrappedMaterialSize) { return false; }

            wrappedMaterial[..AesKeySize].CopyTo(aeskey);
            wrappedMaterial.Slice(AesKeySize,BaseNonceSeedSize).CopyTo(basenonceseed);

            if(header.HasAadHash)
            {
                if(!SHA512.TryHashData(aad,calc,out Int32 w) || w != AadHashSize) { return false; }
                if(!FixedTimeEquals(calc,header.AadHash!)) { return false; }
            }

            body = input[header.HeaderTotalLength..];

            return true;
        }
        finally
        {
            ZeroMemory(wrappedMaterial); ZeroMemory(calc);
        }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryReadAndProcessHeaderAsync"]/*'/>*/
    private static async Task<(Boolean Success , Header Header , Byte[]? KeyMaterial)> TryReadAndProcessHeaderAsync(Stream input , X509Certificate2 certificate , ReadOnlyMemory<Byte> aad , CancellationToken cancel)
    {
        Byte[]? headerFull = null; Byte[]? rsaWrappedKey = null; Byte[]? keyMaterial = null; Byte[]? calc = null;

        try
        {
            var first4 = new Byte[IntSize];

            if(!await ReadExactAsync(input,first4,cancel).ConfigureAwait(false)) { return (false,default,null); }

            if(first4[0] != Version) { return (false,default,null); }

            Byte flags = first4[1];
            Boolean hasOriginalLength = (flags & Flag_HasOriginalLength) != 0;
            Boolean hasAadHash = (flags & Flag_HasAadHash) != 0;

            if((flags & ~(Flag_HasOriginalLength | Flag_HasAadHash)) != 0) { return (false,default,null); }
            
            UInt16 rsaLen = ReadUInt16BigEndian(first4.AsSpan(2,2)); if(rsaLen == 0) { return (false,default,null); }

            Int32 remainder = rsaLen + 1 + (hasOriginalLength ? LongSize : 0) + (hasAadHash ? AadHashSize : 0); if(remainder < 0) { return (false,default,null); }

            headerFull = ArrayPool<Byte>.Shared.Rent(IntSize + remainder);
            var headerSpan = headerFull.AsMemory(0,IntSize + remainder);
            first4.CopyTo(headerSpan);

            if(!await ReadExactAsync(input,headerSpan[IntSize..],cancel).ConfigureAwait(false)) { return (false,default,null); }

            if(!ParseHeaderForAsync(headerSpan.Span,out var header,out rsaWrappedKey)) { return (false,default,null); }

            using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null) { return (false,default,null); }

            keyMaterial = rsa.Decrypt(rsaWrappedKey,RSAEncryptionPadding.OaepSHA512);

            if(keyMaterial.Length != WrappedMaterialSize) { return (false,default,null); }

            if(header.HasAadHash)
            {
                calc = new Byte[AadHashSize];
                if(!SHA512.TryHashData(aad.Span,calc,out Int32 w) || w != AadHashSize) { return (false,default,null); }
                if(!FixedTimeEquals(calc,header.AadHash!)) { return (false,default,null); }
            }

            var result = (true,header,keyMaterial);
            keyMaterial = null;

            return result;
        }
        finally
        {
            if(headerFull is not null) { ZeroMemory(headerFull); ArrayPool<Byte>.Shared.Return(headerFull); }
            if(rsaWrappedKey is not null) { ZeroMemory(rsaWrappedKey); }
            if(keyMaterial is not null) { ZeroMemory(keyMaterial); }
            if(calc is not null) { ZeroMemory(calc); }
        }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="TryReadAndProcessHeader"]/*'/>*/
    private static Boolean TryReadAndProcessHeader(Stream input , X509Certificate2 certificate , ReadOnlySpan<Byte> aad , out Header header , Span<Byte> aeskey , Span<Byte> basenonceseed)
    {
        header = default;
        Byte[]? headerFull = null;
        Span<Byte> wrappedMaterial = stackalloc Byte[WrappedMaterialSize];
        Span<Byte> calc = stackalloc Byte[AadHashSize];

        try
        {
            Span<Byte> first4 = stackalloc Byte[IntSize]; if(!ReadExact(input,first4)) { return false; }

            if(first4[0] != Version) { return false; }

            Byte flags = first4[1];
            Boolean hasOriginalLength = (flags & Flag_HasOriginalLength) != 0;
            Boolean hasAadHash = (flags & Flag_HasAadHash) != 0;

            if((flags & ~(Flag_HasOriginalLength | Flag_HasAadHash)) != 0) { return false; }

            UInt16 rsaLen = ReadUInt16BigEndian(first4.Slice(2,2)); if(rsaLen == 0) { return false; }

            Int32 remainder = rsaLen + 1 + (hasOriginalLength ? LongSize : 0) + (hasAadHash ? AadHashSize : 0); if(remainder < 0) { return false; }

            headerFull = ArrayPool<Byte>.Shared.Rent(IntSize + remainder);
            var headerSpan = headerFull.AsSpan(0,IntSize + remainder);
            first4.CopyTo(headerSpan);

            if(!ReadExact(input,headerSpan[IntSize..])) { return false; }

            if(!TryParseHeader(headerSpan,out header,out var rsaWrappedKey)) { return false; }

            using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null) { return false; }

            if(!rsa.TryDecrypt(rsaWrappedKey,wrappedMaterial,RSAEncryptionPadding.OaepSHA512,out Int32 written) || written != WrappedMaterialSize) { return false; }

            wrappedMaterial[..AesKeySize].CopyTo(aeskey);
            wrappedMaterial.Slice(AesKeySize,BaseNonceSeedSize).CopyTo(basenonceseed);

            if(header.HasAadHash)
            {
                if(!SHA512.TryHashData(aad,calc,out Int32 w) || w != AadHashSize) { return false; }
                if(!FixedTimeEquals(calc,header.AadHash!)) { return false; }
            }

            return true;
        }
        finally
        {
            ZeroMemory(wrappedMaterial); ZeroMemory(calc); if(headerFull is not null) { ZeroMemory(headerFull); ArrayPool<Byte>.Shared.Return(headerFull); }
        }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="WriteHeader"]/*'/>*/
    private static Boolean WriteHeader(
        Stream output,
        ReadOnlySpan<Byte> rsawrappedkey,
        ReadOnlySpan<Byte> aadhash,
        Boolean includeaadhash,
        UInt64 originallength,
        Boolean includeoriginallength,
        Byte chunksizepower)
    {
        if(output is null)                                                           { return false; }
        if(includeaadhash && aadhash.Length != AadHashSize)                          { return false; }
        if(rsawrappedkey.Length == 0 || rsawrappedkey.Length > UInt16.MaxValue)      { return false; }
        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return false; }

        Byte flags = 0;
        if(includeoriginallength) { flags |= Flag_HasOriginalLength; }
        if(includeaadhash)        { flags |= Flag_HasAadHash; }

        UInt16 rsaLen = (UInt16)rsawrappedkey.Length;
        Int32 total = ComputeHeaderLength(rsaLen,includeoriginallength,includeaadhash);
        Byte[] header = new Byte[total];
        Int32 offset = 0; header[offset++] = Version; header[offset++] = flags;
        WriteUInt16BigEndian(header.AsSpan(offset,2),rsaLen); offset += 2;
        rsawrappedkey.CopyTo(header.AsSpan(offset)); offset += rsaLen;
        header[offset++] = chunksizepower;
        if(includeoriginallength)
        {
            WriteUInt64BigEndian(header.AsSpan(offset,LongSize),originallength); offset += LongSize;
        }
        if(includeaadhash)
        {
            aadhash.CopyTo(header.AsSpan(offset)); offset += AadHashSize;
        }
        if(offset != total) { return false; }

        try { output.Write(header,0,header.Length); }
        catch { return false; }

        return true;
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="WriteHeaderSpan"]/*'/>*/
    private static Boolean WriteHeaderSpan(
        Span<Byte> output,
        ReadOnlySpan<Byte> rsawrappedkey,
        ReadOnlySpan<Byte> aadhash,
        Boolean includeaadhash,
        UInt64 originallength,
        Boolean includeoriginallength,
        Byte chunksizepower)
    {
        if(includeaadhash && aadhash.Length != AadHashSize)                          { return false; }
        if(rsawrappedkey.Length == 0 || rsawrappedkey.Length > UInt16.MaxValue)      { return false; }
        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return false; }

        Byte flags = 0;
        if(includeoriginallength) { flags |= Flag_HasOriginalLength; }
        if(includeaadhash)        { flags |= Flag_HasAadHash; }

        UInt16 rsaLen = (UInt16)rsawrappedkey.Length;
        Int32 total = ComputeHeaderLength(rsaLen,includeoriginallength,includeaadhash);
        if(output.Length < total) { return false; }

        Int32 offset = 0;
        output[offset++] = Version;
        output[offset++] = flags;
        WriteUInt16BigEndian(output.Slice(offset,2),rsaLen); offset += 2;
        rsawrappedkey.CopyTo(output[offset..]); offset += rsaLen;
        output[offset++] = chunksizepower;
        if(includeoriginallength)
        {
            WriteUInt64BigEndian(output.Slice(offset,LongSize),originallength); offset += LongSize;
        }
        if(includeaadhash)
        {
            aadhash.CopyTo(output[offset..]); offset += AadHashSize;
        }
        if(offset != total) { return false; }

        return true;
    }
}