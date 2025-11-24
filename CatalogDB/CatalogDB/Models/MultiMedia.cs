namespace KusDepot.CatalogDb;

[PrimaryKey("ID")]
public partial class MultiMedia : ModelBase
{
    public String? Album             {get;set;}

    public String? Application       {get;set;}

    public String? Artist            {get;set;}

    public String? BornOn            {get;set;}

    public Boolean? ContentStreamed  {get;set;}

    public String? DistinguishedName {get;set;}

    public String? FilePath          {get;set;}

    public override Guid? ID         {get;set;}

    public String? Modified          {get;set;}

    public String? Name              {get;set;}

    public String? Size              {get;set;}

    public String? Title             {get;set;}

    public String? Type              {get;set;}

    public String? Year              {get;set;}
}