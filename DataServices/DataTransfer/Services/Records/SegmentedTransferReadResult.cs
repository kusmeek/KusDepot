namespace KusDepot.Data.Services.DataTransfer;

/**<include file='SegmentedTransferReadResult.xml' path='SegmentedTransferReadResult/record[@name="SegmentedTransferReadResult"]/main/*'/>*/
public sealed record SegmentedTransferReadResult : IDisposable
{
    /**<include file='SegmentedTransferReadResult.xml' path='SegmentedTransferReadResult/record[@name="SegmentedTransferReadResult"]/property[@name="Footer"]/*'/>*/
    public TransferSegmentFooter Footer { get; init; } = new();

    /**<include file='SegmentedTransferReadResult.xml' path='SegmentedTransferReadResult/record[@name="SegmentedTransferReadResult"]/property[@name="Payload"]/*'/>*/
    public Stream Payload { get; init; } = Stream.Null;

    /**<include file='SegmentedTransferReadResult.xml' path='SegmentedTransferReadResult/record[@name="SegmentedTransferReadResult"]/method[@name="FinalizeFooter"]/*'/>*/
    public TransferSegmentFooter FinalizeFooter()
    {
        if(this.Payload is not HashingReadStream hashingPayload) { throw new InvalidOperationException(); }

        return this.Footer with { SegmentSHA512 = hashingPayload.GetHashAndReset() };
    }

    /**<include file='SegmentedTransferReadResult.xml' path='SegmentedTransferReadResult/record[@name="SegmentedTransferReadResult"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() => this.Payload.Dispose();
}
