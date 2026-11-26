namespace KusDepot.Security;

public static partial class AccessKeySecret
{
    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="IsSigned"]/*'/>*/
    public static Boolean IsSigned(ReadOnlySpan<Byte> secret)
    {
        if(secret.Length < OuterHeaderFixedSize + TagSize + MinPlaintextLength) { return false; }

        if(secret[0] != Version) { return false; }

        if((secret[1] & SignedFlag) != SignedFlag) { return false; }

        AccessKeySignatureAlgorithm algorithm = (AccessKeySignatureAlgorithm)secret[2];
        AccessKeySignerKeyIdFormat signerkeyidformat = (AccessKeySignerKeyIdFormat)secret[3];
        if(!Enum.IsDefined(algorithm) || !Enum.IsDefined(signerkeyidformat)) { return false; }

        if(ReadUInt16BigEndian(secret.Slice(4,ReservedSize)) != 0) { return false; }

        Guid signertoolid = new(secret.Slice(6,SignerToolIdSize));

        Guid signeraccessmanagerid = new(secret.Slice(22,SignerAccessManagerIdSize));

        UInt16 signerkeyidlength = ReadUInt16BigEndian(secret.Slice(38,SignerKeyIdLengthSize));
        UInt16 signaturelength = ReadUInt16BigEndian(secret.Slice(40,SignatureLengthSize));
        UInt32 ciphertextlength32 = ReadUInt32BigEndian(secret.Slice(42,CiphertextLengthSize));

        if(ciphertextlength32 > Int32.MaxValue) { return false; }

        Int32 requiredRemaining = signerkeyidlength + (Int32)ciphertextlength32 + TagSize + signaturelength;

        return algorithm != AccessKeySignatureAlgorithm.None &&
            signerkeyidformat != AccessKeySignerKeyIdFormat.None &&
            signertoolid != Guid.Empty &&
            signeraccessmanagerid != Guid.Empty &&
            signerkeyidlength > 0 &&
            signaturelength > 0 &&
            secret.Length == OuterHeaderFixedSize + requiredRemaining;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="BuildSignedEnvelope"]/*'/>*/
    private static Byte[] BuildSignedEnvelope(
        ReadOnlySpan<Byte> ciphertext , ReadOnlySpan<Byte> nonce , ReadOnlySpan<Byte> tag , Guid signertoolid , Guid signeraccessmanagerid,
        AccessKeySignatureAlgorithm algorithm , AccessKeySignerKeyIdFormat signerkeyidformat , ReadOnlySpan<Byte> signerkeyid , ReadOnlySpan<Byte> signature)
    {
        ArgumentOutOfRangeException.ThrowIfZero(signerkeyid.Length); ArgumentOutOfRangeException.ThrowIfZero(signature.Length);

        ArgumentOutOfRangeException.ThrowIfEqual((Byte)AccessKeySignatureAlgorithm.None,(Byte)algorithm);

        ArgumentOutOfRangeException.ThrowIfEqual((Byte)AccessKeySignerKeyIdFormat.None,(Byte)signerkeyidformat);

        ArgumentOutOfRangeException.ThrowIfEqual(Guid.Empty,signertoolid);

        ArgumentOutOfRangeException.ThrowIfEqual(Guid.Empty,signeraccessmanagerid);

        return BuildEnvelope(ciphertext,nonce,tag,SignedFlag,algorithm,signerkeyidformat,signertoolid,signeraccessmanagerid,signerkeyid,signature);
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="BuildUnsignedEnvelope"]/*'/>*/
    private static Byte[] BuildUnsignedEnvelope(ReadOnlySpan<Byte> ciphertext , ReadOnlySpan<Byte> nonce , ReadOnlySpan<Byte> tag)
    {
        return BuildEnvelope(ciphertext,nonce,tag,0,AccessKeySignatureAlgorithm.None,AccessKeySignerKeyIdFormat.None,Guid.Empty,Guid.Empty,[],[]);
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadSignature"]/*'/>*/
    public static Boolean TryReadSignature(ReadOnlySpan<Byte> secret , out AccessKeySignatureDescriptor signature)
    {
        signature = default;

        try
        {
            if(!TryParseEnvelope(secret,out ParsedAccessKeySecretEnvelope envelope)) { return false; }

            signature = new()
            {
                Algorithm = envelope.Algorithm,
                Signed = envelope.Signed,
                SignerAccessManagerID = envelope.SignerAccessManagerID,
                SignerKeyID = envelope.SignerKeyIDMemory.ToArray().ToImmutableArray(),
                SignerKeyIDFormat = envelope.SignerKeyIDFormat,
                SignerToolID = envelope.SignerToolID
            };

            return true;
        }
        catch
        {
            signature = default;

            return false;
        }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadSignatureBytes"]/*'/>*/
    internal static Boolean TryReadSignatureBytes(ReadOnlySpan<Byte> secret , out ReadOnlyMemory<Byte> signaturebytes)
    {
        signaturebytes = ReadOnlyMemory<Byte>.Empty;

        if(!TryParseEnvelope(secret,out ParsedAccessKeySecretEnvelope envelope)) { return false; }

        signaturebytes = envelope.SignatureMemory;

        return true;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadSignatureBytesArray"]/*'/>*/
    internal static Boolean TryReadSignatureBytesArray(Byte[] secret , out Byte[] signaturebytes)
    {
        signaturebytes = []; ArgumentNullException.ThrowIfNull(secret);

        if(!TryReadSignatureBytes(secret.AsSpan(),out ReadOnlyMemory<Byte> signaturememory)) { return false; }

        signaturebytes = signaturememory.ToArray();

        return true;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadSignedRegion"]/*'/>*/
    internal static Boolean TryReadSignedRegion(ReadOnlySpan<Byte> secret , out ReadOnlyMemory<Byte> signedregion)
    {
        signedregion = ReadOnlyMemory<Byte>.Empty;

        if(!TryParseEnvelope(secret,out ParsedAccessKeySecretEnvelope envelope)) { return false; }

        signedregion = envelope.SignedRegionMemory;

        return true;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadSignedRegionArray"]/*'/>*/
    internal static Boolean TryReadSignedRegionArray(Byte[] secret , out Byte[] signedregion)
    {
        signedregion = []; ArgumentNullException.ThrowIfNull(secret);

        if(!TryReadSignedRegion(secret.AsSpan(),out ReadOnlyMemory<Byte> signedregionmemory)) { return false; }

        signedregion = signedregionmemory.ToArray();

        return true;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="BuildEnvelope"]/*'/>*/
    private static Byte[] BuildEnvelope(
        ReadOnlySpan<Byte> ciphertext , ReadOnlySpan<Byte> nonce , ReadOnlySpan<Byte> tag , Byte flags , AccessKeySignatureAlgorithm algorithm,
        AccessKeySignerKeyIdFormat signerkeyidformat , Guid signertoolid , Guid signeraccessmanagerid , ReadOnlySpan<Byte> signerkeyid , ReadOnlySpan<Byte> signature)
    {
        Int32 envelopeLength = OuterHeaderFixedSize + signerkeyid.Length + ciphertext.Length + TagSize + signature.Length;
        Byte[] envelope = new Byte[envelopeLength];

        var writer = new BufferWriter(envelope);

        if(!writer.TryWriteByte(Version) ||
           !writer.TryWriteByte(flags) ||
           !writer.TryWriteByte((Byte)algorithm) ||
           !writer.TryWriteByte((Byte)signerkeyidformat) ||
           !writer.TryWriteUInt16BigEndian(0) ||
           !writer.TryWriteGuid(signertoolid) ||
           !writer.TryWriteGuid(signeraccessmanagerid) ||
           !writer.TryWriteUInt16BigEndian((UInt16)signerkeyid.Length) ||
           !writer.TryWriteUInt16BigEndian((UInt16)signature.Length) ||
           !writer.TryWriteUInt32BigEndian((UInt32)ciphertext.Length) ||
           !writer.TryWriteBytes(nonce) ||
           !writer.TryWriteBytes(signerkeyid) ||
           !writer.TryWriteBytes(ciphertext) ||
           !writer.TryWriteBytes(tag) ||
           !writer.TryWriteBytes(signature) ||
           writer.Position != envelopeLength)
        {
            ZeroMemory(envelope);
            throw new InvalidOperationException();
        }

        return envelope;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryValidateSigningInfo"]/*'/>*/
    private static Boolean TryValidateSigningInfo(in AccessKeySignatureInfo signinginfo)
    {
        return signinginfo.Algorithm != AccessKeySignatureAlgorithm.None &&
            signinginfo.SignerKeyIDFormat != AccessKeySignerKeyIdFormat.None &&
            signinginfo.SignatureLength > 0 &&
            signinginfo.SignatureLength <= UInt16.MaxValue &&
            !signinginfo.SignerKeyID.IsDefaultOrEmpty &&
            signinginfo.SignerKeyID.Length <= UInt16.MaxValue;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="HasConsistentIssuerIdentity"]/*'/>*/
    private static Boolean HasConsistentIssuerIdentity(in ParsedAccessKeySecretEnvelope envelope , ParsedAccessKeySecretPayload payload)
    {
        return !envelope.Signed || (envelope.SignerToolID == payload.IssuerToolID && envelope.SignerAccessManagerID == payload.IssuerAccessManagerID);
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryParseEnvelope"]/*'/>*/
    private static Boolean TryParseEnvelope(ReadOnlySpan<Byte> secret , out ParsedAccessKeySecretEnvelope envelope)
    {
        envelope = default;

        if(secret.Length < OuterHeaderFixedSize + TagSize + MinPlaintextLength) { return false; }

        Byte[] envelopebytes = secret.ToArray();
        ReadOnlySpan<Byte> signertoolidbytes = default;
        ReadOnlySpan<Byte> signeraccessmanageridbytes = default;

        var reader = new BufferReader(envelopebytes);

        if(!reader.TryReadByte(out Byte version) || version != Version ||
           !reader.TryReadByte(out Byte flags) ||
           !reader.TryReadByte(out Byte algorithmbyte) ||
           !reader.TryReadByte(out Byte signerkeyidformatbyte) ||
           !reader.TryReadUInt16BigEndian(out UInt16 reserved) ||
           !reader.TryReadBytes(GuidSize,out signertoolidbytes) ||
           !reader.TryReadBytes(GuidSize,out signeraccessmanageridbytes) ||
           !reader.TryReadUInt16BigEndian(out UInt16 signerkeyidlength) ||
           !reader.TryReadUInt16BigEndian(out UInt16 signaturelength) ||
           !reader.TryReadUInt32BigEndian(out UInt32 ciphertextlength32) ||
           !reader.TryReadMemory(NonceSize,out ReadOnlyMemory<Byte> noncememory))
        {
            return false;
        }

        if(ciphertextlength32 > Int32.MaxValue) { return false; }

        Int32 ciphertextlength = (Int32)ciphertextlength32;
        Int32 requiredRemaining = signerkeyidlength + ciphertextlength + TagSize + signaturelength;
        if(reader.Remaining != requiredRemaining) { return false; }

        if(!reader.TryReadMemory(signerkeyidlength,out ReadOnlyMemory<Byte> signerkeyidmemory) ||
           !reader.TryReadMemory(ciphertextlength,out ReadOnlyMemory<Byte> ciphertextmemory) ||
           !reader.TryReadMemory(TagSize,out ReadOnlyMemory<Byte> tagmemory) ||
           !reader.TryReadMemory(signaturelength,out ReadOnlyMemory<Byte> signaturememory))
        {
            return false;
        }

        AccessKeySignatureAlgorithm algorithm = Enum.IsDefined(typeof(AccessKeySignatureAlgorithm),algorithmbyte)
            ? (AccessKeySignatureAlgorithm)algorithmbyte
            : default;

        AccessKeySignerKeyIdFormat signerkeyidformat = Enum.IsDefined(typeof(AccessKeySignerKeyIdFormat),signerkeyidformatbyte)
            ? (AccessKeySignerKeyIdFormat)signerkeyidformatbyte
            : default;

        Guid signertoolid = new(signertoolidbytes);
        Guid signeraccessmanagerid = new(signeraccessmanageridbytes);
        Boolean signed = (flags & SignedFlag) == SignedFlag;
        Boolean valid = signed
            ? reserved == 0 && algorithm != AccessKeySignatureAlgorithm.None && signerkeyidformat != AccessKeySignerKeyIdFormat.None && signertoolid != Guid.Empty && signeraccessmanagerid != Guid.Empty && signerkeyidlength > 0 && signaturelength > 0
            : reserved == 0 && algorithm == AccessKeySignatureAlgorithm.None && signerkeyidformat == AccessKeySignerKeyIdFormat.None && signertoolid == Guid.Empty && signeraccessmanagerid == Guid.Empty && signerkeyidlength == 0 && signaturelength == 0;
        if(!valid) { return false; }

        envelope = new()
        {
            Algorithm = algorithm,
            CiphertextLength = ciphertextlength,
            CiphertextMemory = ciphertextmemory,
            Flags = flags,
            NonceMemory = noncememory,
            SignatureLength = signaturelength,
            SignatureMemory = signaturememory,
            SignerAccessManagerID = signeraccessmanagerid,
            SignerKeyIDFormat = signerkeyidformat,
            SignerKeyIDMemory = signerkeyidmemory,
            SignerToolID = signertoolid,
            SignedRegionMemory = envelopebytes.AsMemory(0,secret.Length - signaturelength),
            TagMemory = tagmemory
        };

        return true;
    }
}