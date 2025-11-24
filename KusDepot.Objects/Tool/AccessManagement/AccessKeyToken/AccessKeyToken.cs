namespace KusDepot;

/**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.AccessKeyToken")]
[DataContract(Name = "AccessKeyToken" , Namespace = "KusDepot")]
public readonly struct AccessKeyToken : IEquatable<AccessKeyToken>
{
    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/field[@name="token"]/*'/>*/
    [DataMember(Name = "token" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    private readonly Byte[] token;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeyToken(ReadOnlySpan<Byte> bytes) { token = bytes.ToArray(); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Create"]/*'/>*/
    public static AccessKeyToken Create(ReadOnlySpan<Byte> bytes) { return new(bytes); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(AccessKeyToken a , AccessKeyToken b) { return a.Equals(b); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(AccessKeyToken a , AccessKeyToken b) { return !(a == b); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Clear"]/*'/>*/
    public void Clear() { ZeroMemory(this.token); }

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Clone"]/*'/>*/
    public AccessKeyToken Clone() { return new(this.token); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) => other is AccessKeyToken t ? this.Equals(t) : false;

    /**<include file='AccessKeyToken.xml' path='AccessKeyToken/struct[@name="AccessKeyToken"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(AccessKeyToken other)
    {
        try
        {
            if(token is null || other.token is null) { return false; }

            return FixedTimeEquals(token,other.token);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try { var _ = new HashCode(); _.AddBytes(token); return _.ToHashCode(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail); throw; }
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        try { return token is null ? String.Empty : Convert.ToHexString(token); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); if(NoExceptions) { return String.Empty; } throw; }
    }
}