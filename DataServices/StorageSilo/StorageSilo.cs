namespace KusDepot.Data.Services.Configuration;

/**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "StorageSilo" , Namespace = "KusDepot.Data.Services.Configuration")]
[GenerateSerializer] [Alias("KusDepot.Data.Services.Configuration.StorageSilo")] [Immutable]

public sealed record class StorageSilo : IEquatable<StorageSilo>
{
    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="AppClientID"]/*'/>*/
    [JsonPropertyName("AppClientID")] [JsonRequired]
    [DataMember(Name = "AppClientID" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String AppClientID      {get;init;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="CatalogName"]/*'/>*/
    [JsonPropertyName("CatalogName")] [JsonRequired]
    [DataMember(Name = "CatalogName" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String CatalogName      {get;init;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="ConnectionString"]/*'/>*/
    [JsonPropertyName("ConnectionString")] [JsonRequired]
    [DataMember(Name = "ConnectionString" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String ConnectionString {get;init;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String Name             {get;init;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="TenantID"]/*'/>*/
    [JsonPropertyName("TenantID")] [JsonRequired]
    [DataMember(Name = "TenantID" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String TenantID         {get;init;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/method[@name="IEquatable{StorageSilo}.Equals"]/*'/>*/
    public Boolean Equals(StorageSilo? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            return String.Equals(this.AppClientID,other.AppClientID,StringComparison.Ordinal) &&
                   String.Equals(this.CatalogName,other.CatalogName,StringComparison.Ordinal) &&
                   String.Equals(this.ConnectionString,other.ConnectionString,StringComparison.Ordinal) &&
                   String.Equals(this.Name,other.Name,StringComparison.Ordinal) &&
                   String.Equals(this.TenantID,other.TenantID,StringComparison.Ordinal);
        }
        catch { return false; }
    }

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/method[@name="FromFile"]/*'/>*/
    public static StorageSilo? FromFile(String path)
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(path); XmlNode? n = d.SelectSingleNode("Silo");

            return new StorageSilo
            {
                AppClientID      = n!.SelectSingleNode("AppClientID")     !.InnerText,
                CatalogName      = n!.SelectSingleNode("CatalogName")     !.InnerText,
                ConnectionString = n!.SelectSingleNode("ConnectionString")!.InnerText,
                Name             = n!.SelectSingleNode("Name")            !.InnerText,
                TenantID         = n!.SelectSingleNode("TenantID")        !.InnerText
            };
        }
        catch { return null; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return HashCode.Combine(this.AppClientID,this.CatalogName,this.ConnectionString,this.Name,this.TenantID); }
}