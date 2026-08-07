namespace KusDepot.Security.Requests;

/**<include file='AccessCredential.xml' path='AccessCredential/class[@name="AccessCredential"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.AccessCredential")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonDerivedType(typeof(JwtCredential),typeDiscriminator:nameof(JwtCredential))]
[JsonDerivedType(typeof(TokenCredential),typeDiscriminator:nameof(TokenCredential))]
[JsonDerivedType(typeof(CertificateCredential),typeDiscriminator:nameof(CertificateCredential))]
[JsonDerivedType(typeof(ManagementKeyCredential),typeDiscriminator:nameof(ManagementKeyCredential))]

public abstract record class AccessCredential
{
    /**<include file='AccessCredential.xml' path='AccessCredential/class[@name="AccessCredential"]/method[@name="ParseAny"]/*'/>*/
    public static AccessCredential? Parse(String input , IFormatProvider? format = null)
    {
        try { return JsonUtility.Parse<AccessCredential>(input); }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessCredential.xml' path='AccessCredential/class[@name="AccessCredential"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : AccessCredential
    {
        try { return JsonUtility.Parse<AccessCredential>(input) as TResult; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    ///<inheritdoc/>
    public sealed override String ToString()
    {
        try { return JsonUtility.ToJsonString<AccessCredential>(this); }

        catch ( Exception _ ) { if(NoExceptions) { return String.Empty; } KusDepotLog.Error(_,ToStringFail); throw; }
    }

    /**<include file='AccessCredential.xml' path='AccessCredential/class[@name="AccessCredential"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : AccessCredential
    {
        try { result = Parse<TResult>(input!,format); return result is not null; }

        catch { result = null; return false; }
    }
}