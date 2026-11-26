namespace KusDepot.Security;

/**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/main/*'/>*/
public static class DataFieldAssertionFactory
{
    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryCreateArtifact"]/*'/>*/
    public static Boolean TryCreate(DataFieldAssertionData data , X509Certificate2? certificate , out DataFieldAssertion? assertion)
    {
        assertion = null;

        if(!TryValidateData(in data) || certificate is null || !String.Equals(data.Thumbprint,certificate.Thumbprint,Ordinal)) { return false; }

        try
        {
            using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null) { return false; }

            if(!TryWriteData(in data,out Byte[]? databytes)) { return false; }

            Byte[] signature = rsa.SignData(databytes!,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);

            assertion = new()
            {
                Data = data,
                EnvelopeVersion = EnvelopeVersion,
                SignatureAlgorithm = SignatureAlgorithmRsaSha512Pss,
                Signature = signature
            };
            return true;
        }
        catch { assertion = null; return false; }
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryCreateBytes"]/*'/>*/
    public static Boolean TryCreate(DataFieldAssertionData data , X509Certificate2? certificate , out Byte[]? assertion)
    {
        assertion = null;

        if(!TryCreate(data,certificate,out DataFieldAssertion? artifact)) { return false; }

        assertion = BuildEnvelope(artifact!);

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadArtifact"]/*'/>*/
    public static Boolean TryRead(ReadOnlySpan<Byte> assertion , out DataFieldAssertion? artifact)
    {
        artifact = null;

        if(!TryReadEnvelope(assertion,out ReadOnlySpan<Byte> databytes,out ReadOnlySpan<Byte> signaturebytes)) { return false; }

        if(!TryReadData(databytes,out DataFieldAssertionData data)) { return false; }

        artifact = new()
        {
            Data = data,
            EnvelopeVersion = EnvelopeVersion,
            SignatureAlgorithm = SignatureAlgorithmRsaSha512Pss,
            Signature = signaturebytes.ToArray()
        };

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadParts"]/*'/>*/
    public static Boolean TryRead(ReadOnlySpan<Byte> assertion , out DataFieldAssertionData data , out Byte[]? signature)
    {
        data = default; signature = null;

        if(!TryRead(assertion,out DataFieldAssertion? artifact)) { return false; }

        data = artifact!.Data;
        signature = artifact.Signature;

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="BuildEnvelope"]/*'/>*/
    private static Byte[] BuildEnvelope(DataFieldAssertion assertion)
    {
        DataFieldAssertionData data = assertion.Data;

        if(!TryWriteData(in data,out Byte[]? databytes)) { throw new InvalidOperationException(); }

        Byte[] signature = assertion.Signature;
        Byte[] envelope = new Byte[ByteSize + ByteSize + UInt32Size + databytes!.Length + UInt32Size + signature.Length];
        var writer = new BufferWriter(envelope);

        if(!writer.TryWriteByte(assertion.EnvelopeVersion) ||
           !writer.TryWriteByte(assertion.SignatureAlgorithm) ||
           !writer.TryWriteUInt32BigEndian((UInt32)databytes.Length) ||
           !writer.TryWriteBytes(databytes) ||
           !writer.TryWriteUInt32BigEndian((UInt32)signature.Length) ||
           !writer.TryWriteBytes(signature))
        {
            throw new InvalidOperationException();
        }

        return envelope;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryWriteData"]/*'/>*/
    internal static Boolean TryWriteData(in DataFieldAssertionData data , out Byte[]? databytes)
    {
        databytes = null;

        if(!TryValidateData(in data)) { return false; }

        try
        {
            Byte[] fieldbytes = Encoding.UTF8.GetBytes(data.Field);
            Byte[] hash = data.Hash;
            Byte[] thumbprintbytes = Encoding.UTF8.GetBytes(data.Thumbprint);
            Boolean hassubject = data.SubjectObjectId.HasValue;
            Boolean hasnotbefore = data.NotBefore.HasValue;
            Boolean hasexpiry = data.ExpiresAt.HasValue;
            Boolean haspurpose = String.IsNullOrWhiteSpace(data.Purpose) is false;
            Byte[]? purposebytes = haspurpose ? Encoding.UTF8.GetBytes(data.Purpose!) : null;
            Int32 length = ByteSize + GuidSize + GuidSize + UInt16Size + fieldbytes.Length + ByteSize + ByteSize +
                UInt16Size + hash.Length + GuidSize + ByteSize + (hassubject ? GuidSize : 0) + Int64Size +
                ByteSize + (hasnotbefore ? Int64Size : 0) + ByteSize + (hasexpiry ? Int64Size : 0) + ByteSize +
                (haspurpose ? UInt16Size + purposebytes!.Length : 0) + UInt16Size + thumbprintbytes.Length;
            Byte[] buffer = new Byte[length];

            var writer = new BufferWriter(buffer);

            if(!writer.TryWriteByte(data.Version) ||
               !writer.TryWriteGuid(data.AssertionId) ||
               !writer.TryWriteGuid(data.DataItemId) ||
               !writer.TryWriteUInt16BigEndian((UInt16)fieldbytes.Length) ||
               !writer.TryWriteBytes(fieldbytes) ||
               !writer.TryWriteByte((Byte)data.FieldState) ||
               !writer.TryWriteByte((Byte)data.HashAlgorithm) ||
               !writer.TryWriteUInt16BigEndian((UInt16)hash.Length) ||
               !writer.TryWriteBytes(hash) ||
               !writer.TryWriteGuid(data.IssuerObjectId) ||
               !writer.TryWriteByte(hassubject ? (Byte)1 : (Byte)0))
            {
                return false;
            }

            if(hassubject && !writer.TryWriteGuid(data.SubjectObjectId!.Value)) { return false; }

            if(!writer.TryWriteInt64BigEndian(data.CreatedAt.ToUnixTimeSeconds()) ||
               !writer.TryWriteByte(hasnotbefore ? (Byte)1 : (Byte)0))
            {
                return false;
            }

            if(hasnotbefore && !writer.TryWriteInt64BigEndian(data.NotBefore!.Value.ToUnixTimeSeconds())) { return false; }

            if(!writer.TryWriteByte(hasexpiry ? (Byte)1 : (Byte)0)) { return false; }

            if(hasexpiry && !writer.TryWriteInt64BigEndian(data.ExpiresAt!.Value.ToUnixTimeSeconds())) { return false; }

            if(!writer.TryWriteByte(haspurpose ? (Byte)1 : (Byte)0)) { return false; }

            if(haspurpose)
            {
                if(!writer.TryWriteUInt16BigEndian((UInt16)purposebytes!.Length) || !writer.TryWriteBytes(purposebytes)) { return false; }
            }

            if(!writer.TryWriteUInt16BigEndian((UInt16)thumbprintbytes.Length) || !writer.TryWriteBytes(thumbprintbytes) || writer.Position != length) { return false; }

            databytes = buffer;

            return true;
        }
        catch { databytes = null; return false; }
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadData"]/*'/>*/
    private static Boolean TryReadData(ReadOnlySpan<Byte> databytes , out DataFieldAssertionData data)
    {
        data = default;

        if(databytes.IsEmpty) { return false; }

        try
        {
            var reader = new BufferSpanReader(databytes);

            if(!reader.TryReadByte(out Byte version) ||
               !reader.TryReadGuid(out Guid assertionid) ||
               !reader.TryReadGuid(out Guid dataitemid) ||
               !TryReadUtf8String(ref reader,out String field) ||
               !reader.TryReadByte(out Byte fieldstate) ||
               !reader.TryReadByte(out Byte hashalgorithm) ||
               !TryReadByteArray(ref reader,out Byte[] hash) ||
               !reader.TryReadGuid(out Guid issuerobjectid) ||
               !reader.TryReadByte(out Byte hassubject) || (hassubject != 0 && hassubject != 1) ||
               !TryReadOptionalGuid(ref reader,hassubject == 1,out Guid? subjectobjectid) ||
               !reader.TryReadInt64BigEndian(out Int64 createdunix) ||
               !reader.TryReadByte(out Byte hasnotbefore) || (hasnotbefore != 0 && hasnotbefore != 1) ||
               !TryReadOptionalDateTimeOffset(ref reader,hasnotbefore == 1,out DateTimeOffset? notbefore) ||
               !reader.TryReadByte(out Byte hasexpiry) || (hasexpiry != 0 && hasexpiry != 1) ||
               !TryReadOptionalDateTimeOffset(ref reader,hasexpiry == 1,out DateTimeOffset? expiresat) ||
               !reader.TryReadByte(out Byte haspurpose) || (haspurpose != 0 && haspurpose != 1) ||
               !TryReadOptionalUtf8String(ref reader,haspurpose == 1,out String? purpose) ||
               !TryReadUtf8String(ref reader,out String thumbprint) ||
               reader.Remaining != 0)
            {
                return false;
            }

            data = new()
            {
                Version = version,
                AssertionId = assertionid,
                DataItemId = dataitemid,
                Field = field,
                FieldState = (DataFieldState)fieldstate,
                HashAlgorithm = (DataFieldHashAlgorithm)hashalgorithm,
                Hash = hash,
                IssuerObjectId = issuerobjectid,
                SubjectObjectId = subjectobjectid,
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(createdunix),
                NotBefore = notbefore,
                ExpiresAt = expiresat,
                Purpose = purpose,
                Thumbprint = thumbprint
            };

            return TryValidateData(in data);
        }
        catch { data = default; return false; }
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadEnvelope"]/*'/>*/
    private static Boolean TryReadEnvelope(ReadOnlySpan<Byte> assertion , out ReadOnlySpan<Byte> databytes , out ReadOnlySpan<Byte> signaturebytes)
    {
        databytes = ReadOnlySpan<Byte>.Empty;
        signaturebytes = ReadOnlySpan<Byte>.Empty;

        if(assertion.IsEmpty) { return false; }

        var reader = new BufferSpanReader(assertion);

        if(!reader.TryReadByte(out Byte envelopeversion) || envelopeversion != EnvelopeVersion ||
           !reader.TryReadByte(out Byte signaturealgorithm) || signaturealgorithm != SignatureAlgorithmRsaSha512Pss ||
           !reader.TryReadUInt32BigEndian(out UInt32 datalength) || datalength == 0 || datalength > reader.Remaining ||
           !reader.TryReadBytes((Int32)datalength,out databytes) ||
           !reader.TryReadUInt32BigEndian(out UInt32 signaturelength) || signaturelength == 0 || signaturelength > reader.Remaining ||
           !reader.TryReadBytes((Int32)signaturelength,out signaturebytes) ||
           reader.Remaining != 0)
        {
            databytes = ReadOnlySpan<Byte>.Empty;
            signaturebytes = ReadOnlySpan<Byte>.Empty;

            return false;
        }

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryValidateData"]/*'/>*/
    internal static Boolean TryValidateData(in DataFieldAssertionData data)
    {
        if(data.Version != DataVersion || data.AssertionId == Guid.Empty || data.DataItemId == Guid.Empty ||
            data.IssuerObjectId == Guid.Empty || String.IsNullOrWhiteSpace(data.Field) || String.IsNullOrWhiteSpace(data.Thumbprint))
        {
            return false;
        }

        if(data.SubjectObjectId == Guid.Empty) { return false; }
        if(data.HashAlgorithm == DataFieldHashAlgorithm.None || data.Hash is null || data.Hash.Length == 0) { return false; }
        if(data.NotBefore.HasValue && data.NotBefore.Value < data.CreatedAt) { return false; }
        if(data.ExpiresAt.HasValue && data.ExpiresAt.Value <= (data.NotBefore ?? data.CreatedAt)) { return false; }

        return Enum.IsDefined(data.FieldState) && Enum.IsDefined(data.HashAlgorithm);
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadByteArray"]/*'/>*/
    private static Boolean TryReadByteArray(ref BufferSpanReader reader , out Byte[] value)
    {
        value = Array.Empty<Byte>();

        if(!reader.TryReadUInt16BigEndian(out UInt16 length) || length == 0 || !reader.TryReadBytes(length,out ReadOnlySpan<Byte> bytes)) { return false; }

        value = bytes.ToArray();

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadOptionalDateTimeOffset"]/*'/>*/
    private static Boolean TryReadOptionalDateTimeOffset(ref BufferSpanReader reader , Boolean present , out DateTimeOffset? value)
    {
        value = null;

        if(!present) { return true; }

        if(!reader.TryReadInt64BigEndian(out Int64 unix)) { return false; }

        value = DateTimeOffset.FromUnixTimeSeconds(unix);

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadOptionalGuid"]/*'/>*/
    private static Boolean TryReadOptionalGuid(ref BufferSpanReader reader , Boolean present , out Guid? value)
    {
        value = null;

        if(!present) { return true; }

        if(!reader.TryReadGuid(out Guid guid)) { return false; }

        value = guid;

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadOptionalUtf8String"]/*'/>*/
    private static Boolean TryReadOptionalUtf8String(ref BufferSpanReader reader , Boolean present , out String? value)
    {
        value = null;

        if(!present) { return true; }

        if(!TryReadUtf8String(ref reader,out String parsed)) { return false; }

        value = parsed;

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/method[@name="TryReadUtf8String"]/*'/>*/
    private static Boolean TryReadUtf8String(ref BufferSpanReader reader , out String value)
    {
        value = String.Empty;

        if(!reader.TryReadUInt16BigEndian(out UInt16 length) || !reader.TryReadBytes(length,out ReadOnlySpan<Byte> bytes)) { return false; }

        if(!Utf8.IsValid(bytes)) { return false; }

        value = Encoding.UTF8.GetString(bytes);

        return true;
    }

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/field[@name="DataVersion"]/*'/>*/
    public const Byte DataVersion = 0x01;

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/field[@name="EnvelopeVersion"]/*'/>*/
    internal const Byte EnvelopeVersion = 0x01;

    /**<include file='DataFieldAssertionFactory.xml' path='DataFieldAssertionFactory/class[@name="DataFieldAssertionFactory"]/field[@name="SignatureAlgorithmRsaSha512Pss"]/*'/>*/
    internal const Byte SignatureAlgorithmRsaSha512Pss = 0x01;

    private const Int32 ByteSize = 1;
    private const Int32 GuidSize = 16;
    private const Int32 Int64Size = 8;
    private const Int32 UInt16Size = 2;
    private const Int32 UInt32Size = 4;
}