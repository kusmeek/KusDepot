namespace KusDepot.Data;

/**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/main/*'/>*/
public class StorageSilo : IEquatable<StorageSilo>
{
    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="AppClientID"]/*'/>*/
    public String AppClientID      {get;set;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="CatalogName"]/*'/>*/
    public String CatalogName      {get;set;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="ConnectionString"]/*'/>*/
    public String ConnectionString {get;set;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="Name"]/*'/>*/
    public String Name             {get;set;} = String.Empty;

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/property[@name="TenantID"]/*'/>*/
    public String TenantID         {get;set;} = String.Empty;

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

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/method[@name="EqualsObject"]/*'/>*/
    public override Boolean Equals(Object? other) { return this.Equals(other as StorageSilo); }

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

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/method[@name="GetHashCode"]/*'/>*/
    public override Int32 GetHashCode() { return HashCode.Combine(this.AppClientID,this.CatalogName,this.ConnectionString,this.Name,this.TenantID); }

    /**<include file='StorageSilo.xml' path='StorageSilo/class[@name="StorageSilo"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        return String.Format(InvariantCulture,"AppClientID: {0}\nCatalogName: {1}\nConnectionString: {2}\nName: {3}\nTenantID: {4}",
                             this.AppClientID,this.CatalogName,this.ConnectionString,this.Name,this.TenantID);
    }
}