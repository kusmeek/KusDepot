namespace DataPodServices.CatalogDB;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.CatalogDB.CatalogItemState")]
/**<include file='CatalogItemState.xml' path='CatalogItemState/record[@name="CatalogItemState"]/summary/*'/>*/
public sealed record CatalogItemState
{
    [Id(0)]
    /**<include file='CatalogItemState.xml' path='CatalogItemState/record[@name="CatalogItemState"]/property[@name="ItemID"]/summary/*'/>*/
    public Guid ItemID { get; init; }

    [Id(1)]
    /**<include file='CatalogItemState.xml' path='CatalogItemState/record[@name="CatalogItemState"]/property[@name="ItemInstanceToken"]/summary/*'/>*/
    public Guid ItemInstanceToken { get; init; }

    [Id(2)]
    /**<include file='CatalogItemState.xml' path='CatalogItemState/record[@name="CatalogItemState"]/property[@name="IsPendingDelete"]/summary/*'/>*/
    public Boolean IsPendingDelete { get; init; }

    /**<include file='CatalogItemState.xml' path='CatalogItemState/record[@name="CatalogItemState"]/constructor[@name="CatalogItemState"]/*'/>*/
    public CatalogItemState(Guid itemID , Guid itemInstanceToken , Boolean isPendingDelete)
    {
        if(itemID == Guid.Empty || itemInstanceToken == Guid.Empty) { throw new ArgumentException("A non-empty item ID and item instance token are required."); }
        this.ItemID = itemID;
        this.ItemInstanceToken = itemInstanceToken;
        this.IsPendingDelete = isPendingDelete;
    }
}
