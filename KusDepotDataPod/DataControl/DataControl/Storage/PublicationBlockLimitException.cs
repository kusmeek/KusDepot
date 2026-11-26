namespace DataPodServices.DataControl;

internal sealed class PublicationBlockLimitException : Exception
{
    public PublicationBlockLimitException(Guid sessionid , Int64 currentBlocks , Int64 requestedBlocks)
        : base("The publication block limit would be exceeded.")
    {
        this.SessionID = sessionid;
        this.CurrentBlocks = currentBlocks;
        this.RequestedBlocks = requestedBlocks;
    }

    public Guid SessionID { get; }

    public Int64 CurrentBlocks { get; }

    public Int64 RequestedBlocks { get; }
}
