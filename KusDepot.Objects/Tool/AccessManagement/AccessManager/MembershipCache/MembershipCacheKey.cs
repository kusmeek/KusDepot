namespace KusDepot.Security;

/**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/main/*'/>*/
public readonly struct MembershipCacheKey : IEquatable<MembershipCacheKey>
{
    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="part1"]/*'/>*/
    private readonly UInt128 part1;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="part2"]/*'/>*/
    private readonly UInt128 part2;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="part3"]/*'/>*/
    private readonly UInt128 part3;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="part4"]/*'/>*/
    private readonly UInt128 part4;

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/constructor[@name="ConstructorDigest"]/*'/>*/
    private MembershipCacheKey(ReadOnlySpan<Byte> digest)
    {
        if(digest.Length != DigestSize) { throw new ArgumentException("Invalid digest size.",nameof(digest)); }

        this.part1 = ReadUInt128BigEndian(digest[..16]);

        this.part2 = ReadUInt128BigEndian(digest.Slice(16,16));

        this.part3 = ReadUInt128BigEndian(digest.Slice(32,16));

        this.part4 = ReadUInt128BigEndian(digest.Slice(48,16));
    }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="TryCreate"]/*'/>*/
    public static Boolean TryCreate(ReadOnlySpan<Byte> credential , out MembershipCacheKey key)
    {
        key = default;

        if(credential.IsEmpty) { return false; }

        Span<Byte> digest = stackalloc Byte[DigestSize];

        if(!SHA512.TryHashData(credential,digest,out Int32 written) || written != DigestSize) { return false; }

        key = new(digest); return true;
    }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="Equals"]/*'/>*/
    public readonly Boolean Equals(MembershipCacheKey other)
    {
        try
        {
            UInt128 diff = this.part1 ^ other.part1;

            diff |= this.part2 ^ other.part2;

            diff |= this.part3 ^ other.part3;

            diff |= this.part4 ^ other.part4;

            return diff == 0;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override readonly Boolean Equals(Object? other) { return other is MembershipCacheKey key && this.Equals(key); }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(MembershipCacheKey left , MembershipCacheKey right) { return left.Equals(right); }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(MembershipCacheKey left , MembershipCacheKey right) { return !(left == right); }

    ///<inheritdoc/>
    public override readonly Int32 GetHashCode()
    {
        try { return HashCode.Combine(this.part1,this.part2,this.part3,this.part4); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail); throw; }
    }

    /**<include file='MembershipCacheKey.xml' path='MembershipCacheKey/struct[@name="MembershipCacheKey"]/field[@name="DigestSize"]/*'/>*/
    private const Int32 DigestSize = SHA512.HashSizeInBytes;
}
