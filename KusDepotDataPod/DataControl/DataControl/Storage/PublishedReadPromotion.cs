namespace DataPodServices.DataControl;

/**<include file='PublishedReadPromotion.xml' path='PublishedReadPromotion/record[@name="PublishedReadPromotion"]/main/*'/>*/
internal sealed record PublishedReadPromotion
{
    /**<include file='PublishedReadPromotion.xml' path='PublishedReadPromotion/record[@name="PublishedReadPromotion"]/property[@name="ItemID"]/*'/>*/
    public Guid ItemID { get; init; }

    /**<include file='PublishedReadPromotion.xml' path='PublishedReadPromotion/record[@name="PublishedReadPromotion"]/property[@name="SessionID"]/*'/>*/
    public Guid SessionID { get; init; }

    /**<include file='PublishedReadPromotion.xml' path='PublishedReadPromotion/record[@name="PublishedReadPromotion"]/property[@name="PreviousSourceSessionID"]/*'/>*/
    public Guid? PreviousSourceSessionID { get; init; }

    /**<include file='PublishedReadPromotion.xml' path='PublishedReadPromotion/record[@name="PublishedReadPromotion"]/property[@name="Snapshot"]/*'/>*/
    public PublishedTransferSnapshot Snapshot { get; init; } = new();
}
