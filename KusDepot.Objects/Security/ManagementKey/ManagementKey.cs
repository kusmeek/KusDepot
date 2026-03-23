namespace KusDepot.Security;

/**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.ManagementKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ManagementKey" , Namespace = "KusDepot.Security")]

public abstract record class ManagementKey : SecurityKey
{
    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/property[@name="Thumbprint"]/*'/>*/
    [IgnoreDataMember] [JsonIgnore]
    public String? Thumbprint
    {
        get
        {
            if(String.IsNullOrWhiteSpace(field) is false) { return field; }

            try { return this.Key is null ? String.Empty : DeserializeCertificate(this.Key)?.Thumbprint!; }

            catch { return String.Empty; }
        }

        protected set { field = value; }
    }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/property[@name="CanSign"]/*'/>*/
    [IgnoreDataMember] [JsonIgnore]
    public Boolean CanSign
    {
        get
        {
            try
            {
                if(this.Key is null) { return false; }

                using var c = DeserializeCertificate(this.Key); if(c is null) { return false; }

                using var k = c.GetRSAPrivateKey(); return k is not null;
            }
            catch { return false; }
        }
    }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="Equals"]/*'/>*/
    public virtual Boolean Equals(ManagementKey? other)
    {
        try
        {
            if(ReferenceEquals(this,other)) { return true; } if(other is null || Equals(this.GetType(),other.GetType()) is false) { return false; }

            return Equals(this.ID,other.ID) && String.Equals(this.Thumbprint,other.Thumbprint);
        }
        catch { return false; }
    }

   ///<inheritdoc/>
    public override Int32 GetHashCode() { var _ = new HashCode(); _.Add(this.ID); _.Add(this.Thumbprint); return _.ToHashCode(); }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="ParseAny"]/*'/>*/
    public static new ManagementKey? Parse(String input , IFormatProvider? format = null)
    {
        try { return SecurityKey.Parse(input,format) as ManagementKey; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="Parse"]/*'/>*/
    public static new TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : ManagementKey
    {
        return SecurityKey.Parse<TResult>(input,format);
    }

    /**<include file='ManagementKey.xml' path='ManagementKey/class[@name="ManagementKey"]/method[@name="TryParse"]/*'/>*/
    public static new Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : ManagementKey
    {
        return SecurityKey.TryParse(input,format,out result);
    }
}