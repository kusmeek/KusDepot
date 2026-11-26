namespace DataPodServices.DataControl;

internal sealed partial class DataControlStreamTransferService
{
    private const String AbortFailed = "Abort stream transfer failed.";

    private const String AppendFailed = "Append stream transfer segment failed.";

    private const String AppendOffsetMismatch = "Stream transfer append offset does not match the current durable tail.";

    private const String PublishFailed = "Publish stream transfer failed.";

    private const String ConflictOpenSession = "An active stream transfer session already exists for the requested item.";

    private const String CatalogBindingMismatch = "Stream transfer catalog binding does not match the authorized catalog.";

    private const String IdentityMismatch = "Stream transfer identity mismatch.";

    private const String InvalidArgument = "Stream transfer argument was not valid.";

    private const String InvalidObjectHash = "Stream transfer object payload hash verification failed.";

    private const String InvalidSegmentHash = "Stream transfer segment payload hash verification failed.";

    private const String InvalidSegmentLength = "Stream transfer segment length violates the effective session segment size policy.";

    private const String InvalidSegmentPolicy = "Stream transfer requested segment size policy is incompatible with the server policy.";

    private const String InvalidSessionState = "Stream transfer session state does not allow this operation.";

    private const String InvalidStateVersion = "Stream transfer expected state version does not match the current session state.";

    private const String OpenFailed = "Open stream transfer failed.";

    private const String RangeUnavailable = "Requested stream transfer range is not currently realized.";

    private const String RemoveFailed = "Remove stream transfer failed.";

    private const String SessionNotFound = "Stream transfer session was not found.";

    private const String SourceSessionNotFound = "Stream transfer source session was not found.";

    private const String StreamHashMismatch = "Stream transfer stream hash mismatch.";

    private const String TransferReadFailed = "Read stream transfer segment failed.";

    private const String ReOpenFailed = "Reopen stream transfer failed.";
}
