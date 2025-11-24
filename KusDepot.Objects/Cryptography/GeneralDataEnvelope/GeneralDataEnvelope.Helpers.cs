namespace KusDepot.Cryptography;

internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="BuildPerChunkAad"]/*'/>*/
    private static Boolean BuildPerChunkAad(Byte flags , UInt32 chunkindex , UInt32 plainlen , Boolean hasoriginallength , UInt64 originallength , ReadOnlySpan<Byte> aadhash , Boolean hasaadhash , Span<Byte> aadout)
    {
        if(aadout.Length < PerChunkAadHashSize) { return false; }

        Byte[]? rent = null;
        Int32 rawLen = 1 + 1 + IntSize + IntSize + (hasoriginallength ? LongSize : 0) + (hasaadhash ? AadHashSize : 0);
        Span<Byte> raw = rawLen <= 128 ? stackalloc Byte[rawLen] : (rent = ArrayPool<Byte>.Shared.Rent(rawLen)).AsSpan(0,rawLen);
        try
        {
            Int32 offset = 0; raw[offset++] = Version; raw[offset++] = flags;
            WriteUInt32BigEndian(raw.Slice(offset,IntSize),chunkindex); offset += IntSize;
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="ComputeHeaderLength"]/*'/>*/
    private static Int32 ComputeHeaderLength(UInt16 rsawrappedlen , Boolean hasoriginallen , Boolean hasaadhash)
    {
        Int32 len = FixedHeaderSize + rsawrappedlen;
        if(hasoriginallen) { len += LongSize; }
        if(hasaadhash) { len += AadHashSize; }
        return len;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DeriveNonce"]/*'/>*/
    private static Boolean DeriveNonce(ReadOnlySpan<Byte> aeskey , ReadOnlySpan<Byte> basenonceseed , UInt64 chunkindex , Span<Byte> nonceout)
    {
        if(aeskey.Length != AesKeySize)               { return false; }
        if(nonceout.Length < NonceSize)               { return false; }
        if(basenonceseed.Length != BaseNonceSeedSize) { return false; }

        Span<Byte> input = stackalloc Byte[BaseNonceSeedSize + LongSize];
        basenonceseed.CopyTo(input); WriteUInt64BigEndian(input.Slice(BaseNonceSeedSize,LongSize),chunkindex);
        Span<Byte> mac = stackalloc Byte[HMACSHA512.HashSizeInBytes];
        if(HMACSHA512.TryHashData(aeskey,input,mac,out Int32 written) is false || written != HMACSHA512.HashSizeInBytes) { return false; }

        mac[..NonceSize].CopyTo(nonceout); ZeroMemory(mac); return true;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="ParseHeaderForAsync"]/*'/>*/
    private static Boolean ParseHeaderForAsync(Byte[] headerbuffer , out Header header, out Byte[] rsawrappedcopy)
    {
        if(TryParseHeader(headerbuffer,out header,out ReadOnlySpan<Byte> rsaWrappedKey) is false) { rsawrappedcopy = Array.Empty<Byte>(); return false; }
        rsawrappedcopy = rsaWrappedKey.ToArray();
        return true;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="ReadExact"]/*'/>*/
    private static Boolean ReadExact(Stream s , Span<Byte> span)
    {
        Int32 total = 0; Int32 read;
        while(total < span.Length && (read = s.Read(span[total..])) > 0) { total += read; }
        return total == span.Length;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="ReadExact_overload"]/*'/>*/
    private static Boolean ReadExact(Stream s , Byte[] buffer , Int32 offset , Int32 count)
    {
        Int32 total = 0; Int32 read;
        while(total < count && (read = s.Read(buffer,offset + total,count - total)) > 0) { total += read; }
        return total == count;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="ReadExactAsync"]/*'/>*/
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="TryParseFooter"]/*'/>*/
    private static Boolean TryParseFooter(ReadOnlySpan<Byte> buffer , out Footer footer)
    {
        footer = default;
        if(buffer.Length < 1 + IntSize) { return false; }
        if(buffer[0] != FooterMarker) { return false; }
        footer.ChunkCount = ReadUInt32BigEndian(buffer.Slice(1,IntSize));
        return true;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="TryParseHeader"]/*'/>*/
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="TryReadAndProcessHeaderSpan"]/*'/>*/
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="TryReadAndProcessHeaderAsync"]/*'/>*/
    private static async Task<(Boolean Success , Header Header , Byte[]? AesKey , Byte[]? BaseNonceSeed)> TryReadAndProcessHeaderAsync(Stream input , X509Certificate2 certificate , ReadOnlyMemory<Byte> aad , CancellationToken cancel)
    {
        Byte[]? headerFull = null; Byte[]? rsaWrappedKey = null; Byte[]? wrappedMaterial = null; Byte[]? calc = null;

        try
        {
            var first4 = new Byte[IntSize];

            if(!await ReadExactAsync(input,first4,cancel).ConfigureAwait(false)) { return (false,default,null,null); }

            if(first4[0] != Version) { return (false,default,null,null); }

            Byte flags = first4[1];
            Boolean hasOriginalLength = (flags & Flag_HasOriginalLength) != 0;
            Boolean hasAadHash = (flags & Flag_HasAadHash) != 0;

            if((flags & ~(Flag_HasOriginalLength | Flag_HasAadHash)) != 0) { return (false,default,null,null); }
            
            UInt16 rsaLen = ReadUInt16BigEndian(first4.AsSpan(2,2)); if(rsaLen == 0) { return (false,default,null,null); }

            Int32 remainder = rsaLen + 1 + (hasOriginalLength ? LongSize : 0) + (hasAadHash ? AadHashSize : 0); if(remainder < 0) { return (false,default,null,null); }

            headerFull = ArrayPool<Byte>.Shared.Rent(IntSize + remainder);
            var headerSpan = headerFull.AsMemory(0,IntSize + remainder);
            first4.CopyTo(headerSpan);

            if(!await ReadExactAsync(input,headerSpan[IntSize..],cancel).ConfigureAwait(false)) { return (false,default,null,null); }

            if(!ParseHeaderForAsync(headerSpan.ToArray(),out var header,out rsaWrappedKey)) { return (false,default,null,null); }

            using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null) { return (false,default,null,null); }

            wrappedMaterial = rsa.Decrypt(rsaWrappedKey,RSAEncryptionPadding.OaepSHA512);

            if(wrappedMaterial.Length != WrappedMaterialSize) { return (false,default,null,null); }

            var aesKey = new Byte[AesKeySize];
            var baseNonceSeed = new Byte[BaseNonceSeedSize];
            wrappedMaterial.AsSpan(0,AesKeySize).CopyTo(aesKey);
            wrappedMaterial.AsSpan(AesKeySize,BaseNonceSeedSize).CopyTo(baseNonceSeed);

            if(header.HasAadHash)
            {
                calc = new Byte[AadHashSize];
                if(!SHA512.TryHashData(aad.Span,calc,out Int32 w) || w != AadHashSize) { return (false,default,null,null); }
                if(!FixedTimeEquals(calc,header.AadHash!)) { return (false,default,null,null); }
            }

            return (true,header,aesKey,baseNonceSeed);
        }
        finally
        {
            if(headerFull is not null) { ZeroMemory(headerFull); ArrayPool<Byte>.Shared.Return(headerFull); }
            if(rsaWrappedKey is not null) { ZeroMemory(rsaWrappedKey); }
            if(wrappedMaterial is not null) { ZeroMemory(wrappedMaterial); }
            if(calc is not null) { ZeroMemory(calc); }
        }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="TryReadAndProcessHeader"]/*'/>*/
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="WriteFooter"]/*'/>*/
    private static Boolean WriteFooter(Stream output , UInt32 chunkcount)
    {
        if(output is null) { return false; }
        Span<Byte> footer = stackalloc Byte[1 + IntSize]; footer[0] = FooterMarker;
        WriteUInt32BigEndian(footer.Slice(1,IntSize),chunkcount);
        try { output.Write(footer); } catch { return false; }
        return true;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="WriteHeader"]/*'/>*/
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="WriteHeaderSpan"]/*'/>*/
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
