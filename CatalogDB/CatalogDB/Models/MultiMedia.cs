namespace KusDepot.CatalogDb;

[PrimaryKey(nameof(ID))]
public partial class MultiMedia : ModelBase
{
    public override Guid? ID   { get; set; }

    public String? Album       { get; set; }

    public String? Artist      { get; set; }

    public String? Title       { get; set; }

    public String? Year        { get; set; }
}