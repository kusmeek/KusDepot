namespace KusDepot.Security;

/**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/main/*'/>*/
public readonly struct MembershipCacheKey : IEquatable<MembershipCacheKey>
{
    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="CipherLength"]/*'/>*/
    public Int32 CipherLength { get; }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="Nonce"]/*'/>*/
    public UInt128 Nonce { get; }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="Tag"]/*'/>*/
    public UInt128 Tag { get; }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="Version"]/*'/>*/
    public Byte Version { get; }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/constructor[@name="Constructor"]/*'/>*/
    public MembershipCacheKey(Byte version , UInt128 nonce , UInt128 tag , Int32 cipherlength)
    {
        Version = version; Nonce = nonce; Tag = tag; CipherLength = cipherlength;
    }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="Create"]/*'/>*/
    public static Boolean TryCreate(ReadOnlySpan<Byte> secret , out MembershipCacheKey key)
    {
        key = default;

        try
        {
            if(secret.Length < MinimumEnvelopeSize) { return false; }

            Int32 cipherlength = secret.Length - MinimumEnvelopeSize; if(cipherlength < 0) { return false; }

            Span<Byte> noncebuffer = stackalloc Byte[TagSize]; secret.Slice(VersionSize,NonceSize).CopyTo(noncebuffer);

            UInt128 nonce = ReadUInt128BigEndian(noncebuffer);

            UInt128 tag = ReadUInt128BigEndian(secret.Slice(secret.Length - TagSize,TagSize));

            key = new MembershipCacheKey(secret[0],nonce,tag,cipherlength); return true;
        }
        catch { key = default; return false; }
    }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(MembershipCacheKey other)
    {
        return this.Version == other.Version && this.CipherLength == other.CipherLength && this.Nonce == other.Nonce && this.Tag == other.Tag;
    }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return other is MembershipCacheKey key && this.Equals(key); }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(MembershipCacheKey left , MembershipCacheKey right) { return left.Equals(right); }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(MembershipCacheKey left , MembershipCacheKey right) { return !(left == right); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return HashCode.Combine(this.Version,this.CipherLength,this.Nonce,this.Tag); }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="MinimumEnvelopeSize"]/*'/>*/
    private const Int32 MinimumEnvelopeSize = VersionSize + NonceSize + TagSize;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="NonceSize"]/*'/>*/
    private const Int32 NonceSize = 12;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="TagSize"]/*'/>*/
    private const Int32 TagSize = 16;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="VersionSize"]/*'/>*/
    private const Int32 VersionSize = 1;
}
