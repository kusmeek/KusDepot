namespace KusDepot.CatalogDb;

[PrimaryKey(nameof(PrimaryKey))]
public partial class Command : ModelBase
{
    public Guid?   PrimaryKey            { get; set; }

    public override Guid? ID             { get; set; }

    public String? CommandHandle         { get; set; }

    public String? CommandSpecifications { get; set; }

    public String? CommandType           { get; set; }
}