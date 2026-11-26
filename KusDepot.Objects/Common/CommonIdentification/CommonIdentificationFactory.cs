namespace KusDepot;

public static partial class CommonIdentificationFactory
{
    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryCreateArtifact"]/*'/>*/
    public static Boolean TryCreate(CommonIdentificationData data , X509Certificate2? certificate , out CommonIdentification? identification)
    {
        identification = null;

        if(!CommonIdentificationVerifier.TryValidateData(in data) || certificate is null || !String.Equals(data.Thumbprint,certificate.Thumbprint,Ordinal)) { return false; }

        try
        {
            using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null) { return false; }

            if(!TryWriteData(in data,out Byte[]? databytes)) { return false; }

            Byte[] signature = rsa.SignData(databytes!,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);

            identification = new()
            {
                Data = data,
                EnvelopeVersion = EnvelopeVersion,
                SignatureAlgorithm = SignatureAlgorithmRsaSha512Pss,
                Signature = signature
            };
            return true;
        }
        catch { identification = null; return false; }
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryCreateBytes"]/*'/>*/
    public static Boolean TryCreate(CommonIdentificationData data , X509Certificate2? certificate , out Byte[]? identification)
    {
        identification = null;

        if(!TryCreate(data,certificate,out CommonIdentification? artifact)) { return false; }

        identification = BuildEnvelope(artifact!);

        return true;
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadArtifact"]/*'/>*/
    public static Boolean TryRead(ReadOnlySpan<Byte> identification , out CommonIdentification? artifact)
    {
        artifact = null;

        if(!TryReadEnvelope(identification,out ReadOnlySpan<Byte> databytes,out ReadOnlySpan<Byte> signaturebytes)) { return false; }

        if(!TryReadData(databytes,out CommonIdentificationData data)) { return false; }

        artifact = new()
        {
            Data = data,
            EnvelopeVersion = EnvelopeVersion,
            SignatureAlgorithm = SignatureAlgorithmRsaSha512Pss,
            Signature = signaturebytes.ToArray()
        };

        return true;
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadParts"]/*'/>*/
    public static Boolean TryRead(ReadOnlySpan<Byte> identification , out CommonIdentificationData data , out Byte[]? signature)
    {
        data = default; signature = null;

        if(!TryRead(identification,out CommonIdentification? artifact)) { return false; }

        data = artifact!.Data;

        signature = artifact.Signature;

        return true;
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="BuildEnvelope"]/*'/>*/
    private static Byte[] BuildEnvelope(CommonIdentification identification)
    {
        CommonIdentificationData data = identification.Data;

        if(!TryWriteData(in data,out Byte[]? databytes)) { throw new InvalidOperationException(); }

        Byte[] signature = identification.Signature;

        Byte[] envelope = new Byte[ByteSize + ByteSize + UInt32Size + databytes!.Length + UInt32Size + signature.Length];

        var writer = new BufferWriter(envelope);

        if(!writer.TryWriteByte(identification.EnvelopeVersion) ||
           !writer.TryWriteByte(identification.SignatureAlgorithm) ||
           !writer.TryWriteUInt32BigEndian((UInt32)databytes.Length) ||
           !writer.TryWriteBytes(databytes) ||
           !writer.TryWriteUInt32BigEndian((UInt32)signature.Length) ||
           !writer.TryWriteBytes(signature))
        {
            throw new InvalidOperationException();
        }

        return envelope;
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadEnvelope"]/*'/>*/
    private static Boolean TryReadEnvelope(ReadOnlySpan<Byte> identification , out ReadOnlySpan<Byte> databytes , out ReadOnlySpan<Byte> signaturebytes)
    {
        databytes = ReadOnlySpan<Byte>.Empty;
        signaturebytes = ReadOnlySpan<Byte>.Empty;

        if(identification.IsEmpty) { return false; }

        var reader = new BufferSpanReader(identification);

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

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadData"]/*'/>*/
    private static Boolean TryReadData(ReadOnlySpan<Byte> databytes , out CommonIdentificationData data)
    {
        data = default;

        if(databytes.IsEmpty) { return false; }

        try
        {
            var reader = new BufferSpanReader(databytes);

            if(!reader.TryReadByte(out Byte version) ||
               !reader.TryReadGuid(out Guid id) ||
               !reader.TryReadGuid(out Guid issuerobjectid) ||
               !reader.TryReadByte(out Byte hassubject) || (hassubject != 0 && hassubject != 1) ||
               !TryReadOptionalGuid(ref reader,hassubject == 1,out Guid? subjectobjectid) ||
               !reader.TryReadInt64BigEndian(out Int64 createdunix) ||
               !reader.TryReadInt64BigEndian(out Int64 notbeforeunix) ||
               !reader.TryReadByte(out Byte hasexpiry) || (hasexpiry != 0 && hasexpiry != 1) ||
               !TryReadOptionalDateTimeOffset(ref reader,hasexpiry == 1,out DateTimeOffset? expiresat) ||
               !TryReadUtf8String(ref reader,out String purpose) ||
               !TryReadUtf8String(ref reader,out String thumbprint) ||
               reader.Remaining != 0)
            {
                return false;
            }

            data = new()
            {
                Version = version,
                Id = id,
                IssuerObjectId = issuerobjectid,
                SubjectObjectId = subjectobjectid,
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(createdunix),
                NotBefore = DateTimeOffset.FromUnixTimeSeconds(notbeforeunix),
                ExpiresAt = expiresat,
                Purpose = purpose,
                Thumbprint = thumbprint
            };

            return CommonIdentificationVerifier.TryValidateData(in data);
        }
        catch { data = default; return false; }
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryWriteData"]/*'/>*/
    internal static Boolean TryWriteData(in CommonIdentificationData data , out Byte[]? databytes)
    {
        databytes = null;

        if(!CommonIdentificationVerifier.TryValidateData(in data)) { return false; }

        try
        {
            Byte[] purposebytes = Encoding.UTF8.GetBytes(data.Purpose);
            Byte[] thumbprintbytes = Encoding.UTF8.GetBytes(data.Thumbprint);
            Boolean hassubject = data.SubjectObjectId.HasValue;
            Boolean hasexpiry = data.ExpiresAt.HasValue;
            Int32 length = ByteSize + GuidSize + GuidSize + ByteSize + (hassubject ? GuidSize : 0) + Int64Size + Int64Size + ByteSize + (hasexpiry ? Int64Size : 0) + UInt16Size + purposebytes.Length + UInt16Size + thumbprintbytes.Length;
            Byte[] buffer = new Byte[length];

            var writer = new BufferWriter(buffer);

            if(!writer.TryWriteByte(data.Version) || !writer.TryWriteGuid(data.Id)) { return false; }

            if(!writer.TryWriteGuid(data.IssuerObjectId) || !writer.TryWriteByte(hassubject ? (Byte)1 : (Byte)0)) { return false; }

            if(hassubject && !writer.TryWriteGuid(data.SubjectObjectId!.Value)) { return false; }

            if(!writer.TryWriteInt64BigEndian(data.CreatedAt.ToUnixTimeSeconds()) ||
               !writer.TryWriteInt64BigEndian(data.NotBefore.ToUnixTimeSeconds()) ||
               !writer.TryWriteByte(hasexpiry ? (Byte)1 : (Byte)0))
            {
                return false;
            }

            if(hasexpiry && !writer.TryWriteInt64BigEndian(data.ExpiresAt!.Value.ToUnixTimeSeconds())) { return false; }

            if(!writer.TryWriteUInt16BigEndian((UInt16)purposebytes.Length) ||
               !writer.TryWriteBytes(purposebytes) ||
               !writer.TryWriteUInt16BigEndian((UInt16)thumbprintbytes.Length) ||
               !writer.TryWriteBytes(thumbprintbytes) || writer.Position != length)
            {
                return false;
            }

            databytes = buffer;

            return true;
        }
        catch { databytes = null; return false; }
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadOptionalDateTimeOffset"]/*'/>*/
    private static Boolean TryReadOptionalDateTimeOffset(ref BufferSpanReader reader , Boolean present , out DateTimeOffset? value)
    {
        value = null;

        if(!present) { return true; }

        if(!reader.TryReadInt64BigEndian(out Int64 unix)) { return false; }

        value = DateTimeOffset.FromUnixTimeSeconds(unix);

        return true;
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadOptionalGuid"]/*'/>*/
    private static Boolean TryReadOptionalGuid(ref BufferSpanReader reader , Boolean present , out Guid? value)
    {
        value = null;

        if(!present) { return true; }

        if(!reader.TryReadGuid(out Guid guid)) { return false; }

        value = guid;

        return true;
    }

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/method[@name="TryReadUtf8String"]/*'/>*/
    private static Boolean TryReadUtf8String(ref BufferSpanReader reader , out String value)
    {
        value = String.Empty;

        if(!reader.TryReadUInt16BigEndian(out UInt16 length) || !reader.TryReadBytes(length,out ReadOnlySpan<Byte> bytes)) { return false; }

        if(!Utf8.IsValid(bytes)) { return false; }

        value = Encoding.UTF8.GetString(bytes);

        return true;
    }
}