namespace KusDepot.Data.Clients;

internal static class StorageStrings
{
    public const String InvalidArgument = @"Segmented transfer argument was not valid.";
    public const String StreamInvalidArgument = @"Stream transfer argument was not valid.";

    public const String CreateSessionFail = @"Create segmented transfer session failed {@SessionID}";
    public const String DeleteSessionFail = @"Delete segmented transfer session failed {@SessionID}";
    public const String LoadManifestFail = @"Load segmented transfer manifest failed {@SessionID}";
    public const String OverlapConflict = @"Segmented transfer overlap conflict detected {@SessionID} {@Offset} {@Length}";
    public const String ReadObjectFail = @"Read segmented transfer object payload failed {@SessionID}";
    public const String ReadRangeFail = @"Read segmented transfer range failed {@SessionID} {@Offset} {@Length}";
    public const String SaveManifestFail = @"Save segmented transfer manifest failed {@SessionID}";
    public const String SparseFileFail = @"Prepare segmented transfer sparse file failed {@Path}";
    public const String StreamCreateSessionFail = @"Create stream transfer session failed {@SessionID}";
    public const String StreamDeleteSessionFail = @"Delete stream transfer session failed {@SessionID}";
    public const String StreamLoadManifestFail = @"Load stream transfer manifest failed {@SessionID}";
    public const String StreamPrepareFileFail = @"Prepare stream transfer append file failed {@Path}";
    public const String StreamReadObjectFail = @"Read stream transfer object payload failed {@SessionID}";
    public const String StreamReadRangeFail = @"Read stream transfer range failed {@SessionID} {@Offset} {@Length}";
    public const String StreamSaveManifestFail = @"Save stream transfer manifest failed {@SessionID}";
    public const String StreamSaveObjectFail = @"Save stream transfer object payload failed {@SessionID}";
    public const String StreamWriteTailFail = @"Write stream transfer tail failed {@SessionID} {@Offset} {@Length}";
    public const String WriteRangeFail = @"Write segmented transfer range failed {@SessionID} {@Offset} {@Length}";
}