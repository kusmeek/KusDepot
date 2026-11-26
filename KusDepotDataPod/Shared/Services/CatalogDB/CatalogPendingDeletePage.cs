namespace DataPodServices.CatalogDB;

/**<include file='CatalogPendingDeletePage.xml' path='CatalogPendingDeletePage/record[@name="CatalogPendingDeletePage"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("DataPodServices.CatalogDB.CatalogPendingDeletePage")]
public sealed record CatalogPendingDeletePage
{
    /**<include file='CatalogPendingDeletePage.xml' path='CatalogPendingDeletePage/record[@name="CatalogPendingDeletePage"]/property[@name="Items"]/*'/>*/
    [Id(0)]
    public CatalogItemIdentity[] Items { get; init; }

    /**<include file='CatalogPendingDeletePage.xml' path='CatalogPendingDeletePage/record[@name="CatalogPendingDeletePage"]/property[@name="NextAfterItemID"]/*'/>*/
    [Id(1)]
    public Guid? NextAfterItemID { get; init; }

    /**<include file='CatalogPendingDeletePage.xml' path='CatalogPendingDeletePage/record[@name="CatalogPendingDeletePage"]/constructor[@name="CatalogPendingDeletePage"]/*'/>*/
    public CatalogPendingDeletePage(CatalogItemIdentity[] items , Guid? nextAfterItemID)
    {
        this.Items = items ?? throw new ArgumentNullException(nameof(items));
        this.NextAfterItemID = nextAfterItemID;
    }
}
