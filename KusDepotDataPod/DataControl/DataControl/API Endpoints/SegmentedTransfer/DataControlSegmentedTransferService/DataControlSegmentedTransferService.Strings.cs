namespace DataPodServices.DataControl;

internal sealed partial class DataControlSegmentedTransferService
{
    private const String AbortFailed = "Abort segmented transfer failed.";

    private const String BeginPublishUploadFailed = "Begin segmented upload publication failed.";

    private const String ConflictOpenUploadSession = "An active segmented upload transfer session already exists for the requested item.";

    private const String CatalogBindingMismatch = "Segmented transfer catalog binding does not match the authorized catalog.";

    private const String IdentityMismatch = "Segmented transfer identity mismatch.";

    private const String InvalidArgument = "Segmented transfer argument was not valid.";

    private const String InvalidObjectHash = "Segmented transfer object payload hash verification failed.";

    private const String InvalidPublishedSnapshot = "Published segmented transfer snapshot was not valid.";

    private const String InvalidSegmentHash = "Segmented transfer segment payload hash verification failed.";

    private const String InvalidSegmentPolicy = "Segmented transfer requested segment size policy is incompatible with the server policy.";

    private const String InvalidSegmentLength = "Segmented transfer segment length violates the effective session segment size policy.";

    private const String InvalidSessionState = "Segmented transfer session state does not allow this operation.";

    private const String InvalidStateVersion = "Segmented transfer expected state version does not match the current session state.";

    private const String LoadSessionFailed = "Segmented transfer session could not be loaded.";

    private const String NotFullyRealized = "Segmented transfer cannot be published until full coverage is realized.";

    private const String OpenFailed = "Open segmented transfer failed.";

    private const String PublishedSnapshotUnavailable = "Published segmented transfer snapshot could not be resolved.";

    private const String RangeUnavailable = "Requested segmented transfer range is not currently realized.";

    private const String RemoveFailed = "Remove segmented transfer failed.";

    private const String SessionNotFound = "Segmented transfer session was not found.";

    private const String StreamHashMismatch = "Segmented transfer stream hash mismatch.";

    private const String TransferReadFailed = "Read segmented transfer segment failed.";
}
