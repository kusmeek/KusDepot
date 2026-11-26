namespace KusDepot.Security;

/**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.AccessKeyToken")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public struct AccessKeyToken : IEquatable<AccessKeyToken>
{
    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/field[@name="part1"]/*'/>*/
    [JsonPropertyName("part1")] [JsonInclude] [JsonRequired] [Id(0)]
    private UInt128 part1;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/field[@name="part2"]/*'/>*/
    [JsonPropertyName("part2")] [JsonInclude] [JsonRequired] [Id(1)]
    private UInt128 part2;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/field[@name="part3"]/*'/>*/
    [JsonPropertyName("part3")] [JsonInclude] [JsonRequired] [Id(2)]
    private UInt128 part3;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/field[@name="part4"]/*'/>*/
    [JsonPropertyName("part4")] [JsonInclude] [JsonRequired] [Id(3)]
    private UInt128 part4;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeyToken(ReadOnlySpan<Byte> bytes)
    {
        if(bytes.Length != AccessKeySecret.TokenIdSize) { throw new ArgumentException("Invalid token size.",nameof(bytes)); }

        this.part1 = ReadUInt128BigEndian(bytes[..16]);

        this.part2 = ReadUInt128BigEndian(bytes.Slice(16,16));

        this.part3 = ReadUInt128BigEndian(bytes.Slice(32,16));

        this.part4 = ReadUInt128BigEndian(bytes.Slice(48,16));
    }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Create"]/*'/>*/
    public static AccessKeyToken Create(ReadOnlySpan<Byte> bytes) { return new(bytes); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(AccessKeyToken a , AccessKeyToken b) { return a.Equals(b); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(AccessKeyToken a , AccessKeyToken b) { return !(a == b); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Clear"]/*'/>*/
    public void Clear() { this.part1 = 0; this.part2 = 0; this.part3 = 0; this.part4 = 0; }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Clone"]/*'/>*/
    public readonly AccessKeyToken Clone() { return this; }

    ///<inheritdoc/>
    public override readonly Boolean Equals(Object? other) => other is AccessKeyToken t ? this.Equals(t) : false;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Equals"]/*'/>*/
    public readonly Boolean Equals(AccessKeyToken other)
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
    public override readonly Int32 GetHashCode()
    {
        try { return HashCode.Combine(this.part1,this.part2,this.part3,this.part4); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail); throw; }
    }

    ///<inheritdoc/>
    public override readonly String ToString()
    {
        try
        {
            Span<Byte> bytes = stackalloc Byte[64];

            WriteUInt128BigEndian(bytes,this.part1);

            WriteUInt128BigEndian(bytes.Slice(16,16),this.part2);

            WriteUInt128BigEndian(bytes.Slice(32,16),this.part3);

            WriteUInt128BigEndian(bytes.Slice(48,16),this.part4);

            return Convert.ToHexString(bytes);
        }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); if(NoExceptions) { return String.Empty; } throw; }
    }
}