namespace DataPodServices.CatalogDB;

/**<include file='CatalogItemIdentity.xml' path='CatalogItemIdentity/record[@name="CatalogItemIdentity"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("DataPodServices.CatalogDB.CatalogItemIdentity")]
public sealed record CatalogItemIdentity
{
    /**<include file='CatalogItemIdentity.xml' path='CatalogItemIdentity/record[@name="CatalogItemIdentity"]/property[@name="ItemID"]/*'/>*/
    [Id(0)]
    public Guid ItemID { get; init; }

    /**<include file='CatalogItemIdentity.xml' path='CatalogItemIdentity/record[@name="CatalogItemIdentity"]/property[@name="ItemInstanceToken"]/*'/>*/
    [Id(1)]
    public Guid ItemInstanceToken { get; init; }

    /**<include file='CatalogItemIdentity.xml' path='CatalogItemIdentity/record[@name="CatalogItemIdentity"]/constructor[@name="CatalogItemIdentity"]/*'/>*/
    public CatalogItemIdentity(Guid itemID , Guid itemInstanceToken)
    {
        if(itemID == Guid.Empty || itemInstanceToken == Guid.Empty) { throw new ArgumentException("A non-empty item ID and item instance token are required."); }

        this.ItemID = itemID;
        this.ItemInstanceToken = itemInstanceToken;
    }
}
