namespace KusDepot.Security;

public static partial class AccessKeySecret
{
    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="BuildAssociatedData"]/*'/>*/
    private static Byte[] BuildAssociatedData(String toolschemaid , String accesskeyrealmid)
    {
        Byte[] toolschemabytes = EncodeUtf8(toolschemaid);

        Byte[] accesskeyrealmbytes = EncodeUtf8(accesskeyrealmid);

        Byte[] aad = new Byte[VersionSize + ToolSchemaLengthSize + toolschemabytes.Length + AccessKeyRealmLengthSize + accesskeyrealmbytes.Length];

        Span<Byte> buffer = aad; buffer[0] = Version;

        WriteUInt16BigEndian(buffer.Slice(VersionSize,ToolSchemaLengthSize),(UInt16)toolschemabytes.Length);

        toolschemabytes.CopyTo(buffer.Slice(VersionSize + ToolSchemaLengthSize,toolschemabytes.Length));

        Int32 realmoffset = VersionSize + ToolSchemaLengthSize + toolschemabytes.Length;

        WriteUInt16BigEndian(buffer.Slice(realmoffset,AccessKeyRealmLengthSize),(UInt16)accesskeyrealmbytes.Length);

        accesskeyrealmbytes.CopyTo(buffer.Slice(realmoffset + AccessKeyRealmLengthSize,accesskeyrealmbytes.Length));

        return aad;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="DecodeManifestHash"]/*'/>*/
    private static String DecodeManifestHash(ReadOnlySpan<Byte> manifesthashbytes)
    {
        return manifesthashbytes.ContainsAnyExcept((Byte)0) ? Convert.ToHexString(manifesthashbytes) : String.Empty;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryDecodeUtf8"]/*'/>*/
    private static Boolean TryDecodeUtf8(ReadOnlySpan<Byte> bytes , out String value)
    {
        value = String.Empty;

        if(!Utf8.IsValid(bytes)) { return false; }

        value = Encoding.UTF8.GetString(bytes);

        return true;
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="EncodeUtf8"]/*'/>*/
    private static Byte[] EncodeUtf8(String? value)
    {
        return Encoding.UTF8.GetBytes(value ?? String.Empty);
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="NormalizeStrings"]/*'/>*/
    private static String[] NormalizeStrings(IEnumerable<String>? values)
    {
        if(values is null) { return []; }

        return values.Where(_ => String.IsNullOrWhiteSpace(_) is false)
            .Select(_ => _.Trim())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(_ => _,StringComparer.Ordinal)
            .ToArray();
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryEncodeManifestHash"]/*'/>*/
    private static Boolean TryEncodeManifestHash(String? manifesthash , out Byte[] manifesthashbytes)
    {
        manifesthashbytes = new Byte[ManifestHashSize];

        if(String.IsNullOrEmpty(manifesthash)) { return true; }

        try
        {
            Byte[] decoded = Convert.FromHexString(manifesthash);

            if(decoded.Length != ManifestHashSize) { manifesthashbytes = Array.Empty<Byte>(); return false; }

            decoded.CopyTo(manifesthashbytes,0);

            ZeroMemory(decoded);

            return true;
        }
        catch
        {
            manifesthashbytes = Array.Empty<Byte>();

            return false;
        }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadAssertionBlock"]/*'/>*/
    private static Boolean TryReadAssertionBlock(ref BufferReader reader , out ImmutableArray<AccessKeyAssertion> assertions)
    {
        assertions = [];

        if(reader.Remaining == 0) { return true; } if(reader.Remaining < AssertionCountSize) { return false; }

        if(!reader.TryReadUInt16BigEndian(out UInt16 assertioncount)) { return false; }

        if(assertioncount == 0) { return reader.Remaining == 0; }

        var builder = ImmutableArray.CreateBuilder<AccessKeyAssertion>(assertioncount);

        for(Int32 i = 0; i < assertioncount; i++)
        {
            if(!reader.TryReadUInt32BigEndian(out UInt32 assertionlength) || assertionlength > Int32.MaxValue || reader.Remaining < (Int32)assertionlength) { return false; }

            if(!reader.TryReadBytes((Int32)assertionlength,out ReadOnlySpan<Byte> assertionbytes) || !AccessKeyAssertion.TryDeserialize(assertionbytes,out AccessKeyAssertion? assertion) || assertion is null) { return false; }

            builder.Add(assertion);
        }

        assertions = builder.ToImmutable();

        return true;
    }
}