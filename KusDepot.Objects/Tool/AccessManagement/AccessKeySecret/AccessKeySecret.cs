namespace KusDepot;

/**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/main/*'/>*/
public static partial class AccessKeySecret
{
    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryCreate"]/*'/>*/
    public static Boolean TryCreate(X509Certificate2 certificate , Guid toolid , Guid accessmanagerid , String subject,
        IEnumerable<Int32> operations , TimeSpan? lifetime , out Byte[]? secret , out AccessKeyToken token)
    {
        subject ??= String.Empty; secret = null; token = default;

        try
        {
            if(certificate is null || Equals(toolid,Guid.Empty) || Equals(accessmanagerid,Guid.Empty)) { return false; }

            Byte[] subjectbytes = subject.ToByteArrayFromUTF16String();

            if(subjectbytes.Length > UInt16.MaxValue) { return false; }

            Int32 maxop = -1; foreach(Int32 op in operations) { if(op > maxop) { maxop = op; } }

            if(maxop >= MaxOperations) { return false; }

            UInt16 blockcount = maxop < 0 ? (UInt16)0 : (UInt16)((maxop / BitsPerBlock) + 1);

            Span<UInt128> blocks = stackalloc UInt128[blockcount]; blocks.Clear();

            foreach(Int32 op in operations)
            {
                if(op < 0) { return false; }

                Int32 block = op / BitsPerBlock; Int32 bit = op % BitsPerBlock;

                blocks[block] |= (UInt128.One << bit);
            }

            Int64 issued = DateTimeOffset.Now.ToUnixTimeSeconds();

            Int64 notafter = lifetime.HasValue && lifetime.Value > TimeSpan.Zero ? issued + (Int64)lifetime.Value.TotalSeconds : 0L;

            Int32 plainlen = FixedPrefixBeforeSubject + subjectbytes.Length + PermissionBlockCountSize + (blockcount * BytesPerBlock) + FixedTailSize;

            Span<Byte> plain = stackalloc Byte[plainlen];

            var writer = new BufferCursor(plain);

            if( !writer.TryWriteByte(Version) ||
                !writer.TryWriteRandom(NonceSize) ||
                !writer.TryWriteGuid(toolid) ||
                !writer.TryWriteGuid(accessmanagerid) ||
                !writer.TryWriteInt64BigEndian(issued) ||
                !writer.TryWriteInt64BigEndian(notafter) ||
                !writer.TryWriteUInt16BigEndian((UInt16)subjectbytes.Length) ||
                !writer.TryWriteBytes(subjectbytes) ||
                !writer.TryWriteUInt16BigEndian(blockcount) )
            {
                return false;
            }

            foreach(var block in blocks)
            {
                if(!writer.TryWriteUInt128BigEndian(block)) { return false; }
            }

            if(!writer.TryWriteRandom(SaltSize)) { return false; }

            ReadOnlySpan<Byte> hashdata = writer.WrittenSpan;

            Span<Byte> tokenhash = plain.Slice(writer.Position,TokenHashSize);

            SHA512.HashData(hashdata,tokenhash); token = new AccessKeyToken(tokenhash);

            if(!writer.TryWriteBytes(tokenhash)) { return false; }

            if(writer.Position != plainlen) { return false; }

            Span<Byte> aad = stackalloc Byte[1 + GuidSize + GuidSize]; aad[0] = Version;

            toolid.TryWriteBytes(aad.Slice(1,GuidSize));

            accessmanagerid.TryWriteBytes(aad.Slice(1 + GuidSize,GuidSize));

            Span<Byte> secretspan = stackalloc Byte[plainlen + GetEncryptionOverhead(certificate)];

            if(Encrypt(writer.WrittenSpan,secretspan,certificate,out Int32 written,aad) && written > 0)
            {
                secret = secretspan[..written].ToArray();
            }

            if(secret is null) { token = default; return false; }

            return true;
        }
        catch { secret = null; token = default; return false; }

        finally { }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="GetEncryptionOverhead"]/*'/>*/
    private static Int32 GetEncryptionOverhead(X509Certificate2 certificate) { return (certificate.GetRSAPublicKey()?.KeySize / 8 ?? 0) + 256; }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(ReadOnlySpan<Byte> secret , X509Certificate2 certificate , Guid toolid , Guid accessmanagerid , out String? subject,
        out ReadOnlyMemory<Byte> permissions , out DateTimeOffset issuedat , out DateTimeOffset? notafter , out AccessKeyToken token , out Boolean expired)
    {
        subject = null; permissions = ReadOnlyMemory<Byte>.Empty; issuedat = default; notafter = null; token = default; expired = false;

        try
        {
            if(certificate is null || secret.IsEmpty || Equals(toolid,Guid.Empty) || Equals(accessmanagerid,Guid.Empty)) { return false; }

            Span<Byte> aad = stackalloc Byte[1 + GuidSize + GuidSize]; aad[0] = Version;

            toolid.TryWriteBytes(aad.Slice(1,GuidSize)); accessmanagerid.TryWriteBytes(aad.Slice(1 + GuidSize,GuidSize));

            Span<Byte> decrypted = stackalloc Byte[secret.Length];

            if(!Decrypt(secret,decrypted,certificate,out Int32 written,aad) || written == 0) { return false; }

            var data = decrypted[..written]; if(data.Length < MinPlaintextLength) { return false; }

            var reader = new BufferReader(data);

            if( !reader.TryReadByte(out Byte version) || version != Version ||
                !reader.TryReadBytes(NonceSize,out _) ||
                !reader.TryReadGuid(out Guid issuer) || issuer != toolid ||
                !reader.TryReadGuid(out Guid manager) || manager != accessmanagerid ||
                !reader.TryReadInt64BigEndian(out Int64 issuedunix) ||
                !reader.TryReadInt64BigEndian(out Int64 notafterunix) ||
                !reader.TryReadUInt16BigEndian(out UInt16 subjectlen) ||
                reader.Remaining < subjectlen + PermissionBlockCountSize )
            { return false; }

            if(!reader.TryReadBytes(subjectlen, out ReadOnlySpan<Byte> subjectbytes)) { return false; }

            subject = subjectbytes.ToArray().ToUTF16StringFromByteArray();

            if( !reader.TryReadUInt16BigEndian(out UInt16 blockcount) || blockcount > MaxPermissionBlocks ||
                reader.Remaining < (blockcount * BytesPerBlock) + FixedTailSize )
            { return false; }

            if( !reader.TryReadBytes(blockcount * BytesPerBlock, out ReadOnlySpan<Byte> bitmap) ||
                !reader.TryReadBytes(SaltSize,out _) ||
                !reader.TryReadBytes(TokenHashSize,out ReadOnlySpan<Byte> tokenhash) )
            { return false; }

            permissions = bitmap.ToArray();

            ReadOnlySpan<Byte> verifydata = data[..^TokenHashSize];

            Span<Byte> recompute = stackalloc Byte[TokenHashSize];

            SHA512.HashData(verifydata,recompute);

            if(tokenhash.SequenceEqual(recompute) is false) { return false; }

            token = new AccessKeyToken(tokenhash);

            issuedat = DateTimeOffset.FromUnixTimeSeconds(issuedunix);

            if(notafterunix != 0)
            {
                notafter = DateTimeOffset.FromUnixTimeSeconds(notafterunix); expired = DateTimeOffset.Now > notafter.Value;
            }
            else { expired = false; }

            return true;
        }
        catch
        {
            subject = null; permissions = ReadOnlyMemory<Byte>.Empty; issuedat = default; notafter = null; token = default; expired = false;

            return false;
        }
        finally { }
    }
}