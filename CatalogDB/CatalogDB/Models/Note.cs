namespace KusDepot.CatalogDb;

[PrimaryKey(nameof(PrimaryKey))]
public sealed partial class Note : ModelBase
{
    public override Guid? ID {get;set;}

    public Guid? PrimaryKey  {get;set;}

    public String Value      {get;set;} = String.Empty;
}