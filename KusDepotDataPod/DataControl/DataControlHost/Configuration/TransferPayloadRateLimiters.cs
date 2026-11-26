namespace DataPodServices.DataControl;

internal sealed class TransferPayloadRateLimiters
{
    public AsyncByteRateLimiter? Upload { get; }

    public AsyncByteRateLimiter? Download { get; }

    public TransferPayloadRateLimiters(Int64 uploadbytespersecond , Int64 downloadbytespersecond)
    {
        this.Upload = uploadbytespersecond > 0 ? new AsyncByteRateLimiter(uploadbytespersecond) : null;

        this.Download = downloadbytespersecond > 0 ? new AsyncByteRateLimiter(downloadbytespersecond) : null;
    }
}
