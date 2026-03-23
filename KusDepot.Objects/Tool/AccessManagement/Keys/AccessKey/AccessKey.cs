namespace KusDepot.Security;

/**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.Security.AccessKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "AccessKey" , Namespace = "KusDepot.Security")]

public abstract record class AccessKey : SecurityKey
{
    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="Equals"]/*'/>*/
    public virtual Boolean Equals(AccessKey? other) { return base.Equals(other); }

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="ParseAny"]/*'/>*/
    public static new AccessKey? Parse(String input , IFormatProvider? format = null)
    {
        try { return SecurityKey.Parse(input,format) as AccessKey; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="Parse"]/*'/>*/
    public static new TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : AccessKey
    {
        return SecurityKey.Parse<TResult>(input,format);
    }

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="TryParse"]/*'/>*/
    public static new Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : AccessKey
    {
        return SecurityKey.TryParse(input,format,out result);
    }
}