namespace KusDepot.Data.Services.DataTransfer;

/**<include file='StreamTransferReadResult.xml' path='StreamTransferReadResult/record[@name="StreamTransferReadResult"]/main/*'/>*/
public sealed record StreamTransferReadResult : IDisposable
{
    /**<include file='StreamTransferReadResult.xml' path='StreamTransferReadResult/record[@name="StreamTransferReadResult"]/property[@name="Footer"]/*'/>*/
    public StreamTransferSegmentFooter Footer { get; init; } = new();

    /**<include file='StreamTransferReadResult.xml' path='StreamTransferReadResult/record[@name="StreamTransferReadResult"]/property[@name="Payload"]/*'/>*/
    public Stream Payload { get; init; } = Stream.Null;

    /**<include file='StreamTransferReadResult.xml' path='StreamTransferReadResult/record[@name="StreamTransferReadResult"]/method[@name="FinalizeFooter"]/*'/>*/
    public StreamTransferSegmentFooter FinalizeFooter()
    {
        if(this.Payload is not HashingReadStream hashingPayload) { throw new InvalidOperationException(); }

        return this.Footer with { SegmentSHA512 = hashingPayload.GetHashAndReset() };
    }

    /**<include file='StreamTransferReadResult.xml' path='StreamTransferReadResult/record[@name="StreamTransferReadResult"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() => this.Payload.Dispose();
}
