namespace KusDepot.CatalogDb;

[PrimaryKey(nameof(PrimaryKey))]
public partial class Service : ModelBase
{
    public Guid?   PrimaryKey            { get; set; }

    public override Guid? ID             { get; set; }

    public String? ServiceInterfaces     { get; set; }

    public String? ServiceSpecifications { get; set; }

    public String? ServiceType           { get; set; }
}