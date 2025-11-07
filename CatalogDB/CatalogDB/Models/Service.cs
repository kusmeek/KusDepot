namespace KusDepot.CatalogDb;

[PrimaryKey("PrimaryKey")]
public partial class Service : ModelBase
{
    public String? Application        {get;set;}

    public String? ApplicationVersion {get;set;}

    public String? BornOn             {get;set;}

    public String? ContentHash        {get;set;}

    public Boolean? ContentStreamed   {get;set;}

    public String? DistinguishedName  {get;set;}

    public String? FilePath           {get;set;}

    public override Guid? ID          {get;set;}

    public String? Modified           {get;set;}

    public String? Name               {get;set;}

    public String? ObjectType         {get;set;}

    public Guid?   PrimaryKey         {get;set;}

    public String? ServiceInterfaces  {get;set;}

    public String? ServiceType        {get;set;}

    public String? ServiceVersion     {get;set;}

    public String? Size               {get;set;}

    public String? Version            {get;set;}
}