namespace DataPodServices.DataControl;

internal sealed class TransferConfiguration
{
    public DataItemTransferSegmentSizePolicy? DefaultSegmentSizePolicy { get; init; }

    public TransferEnforcementConfiguration Enforcement { get; init; } = new();

    public TransferRateLimitsConfiguration PayloadRateLimits { get; init; } = new();

    public const Int64 NotificationReconnectBufferSize = 32768;
}

internal sealed class TransferEnforcementConfiguration
{
    public Boolean EnforceMinimumSegmentBytes { get; init; } = true;

    public Boolean EnforceMaximumSegmentBytes { get; init; } = true;
}

internal sealed class TransferRateLimitsConfiguration
{
    public Int64 UploadBytesPerSecond { get; init; }

    public Int64 DownloadBytesPerSecond { get; init; }
}
