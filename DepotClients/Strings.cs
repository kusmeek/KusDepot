namespace KusDepot.Data.Clients;

#pragma warning disable CS1591
public static class CatalogClientStrings
{
    public const String CatalogResponseDeserializationFail = "Catalog response deserialization failed";
    public const String SearchElementsFail = "SearchElements Failed";
    public const String SearchCommandsFail = "SearchCommands Failed";
    public const String SearchServicesFail = "SearchServices Failed";
    public const String SearchMediaFail    = "SearchMedia Failed";
    public const String SearchNotesFail    = "SearchNotes Failed";
    public const String SearchTagsFail     = "SearchTags Failed";
}

public static class DataControlClientStrings
{ 
    public const String DataControlResponseDeserializationFail = "DataControl response deserialization failed";
    public const String HttpClientConfigurationInvalid = "A supplied HttpClient must have no BaseAddress and no default Authorization header.";
    public const String AbortTransferFail = "AbortTransfer Failed";
    public const String CloseTransferSessionFail = "CloseTransferSession Failed";
    public const String InspectPublicationFail = "InspectPublication Failed";
    public const String PurgePublicationFail = "PurgePublication Failed";
    public const String PublishStreamTransferFail = "PublishStreamTransfer Failed";
    public const String PublishSegmentedTransferFail = "PublishSegmentedTransfer Failed";
    public const String DeleteLocalSessionFail = @"DeleteLocalSession Failed";
    public const String DownloadStreamTransferSegmentToStreamFail = "DownloadStreamTransferSegmentToStream Failed";
    public const String DownloadTransferSegmentToStreamFail = "DownloadTransferSegmentToStream Failed";
    public const String GetLocalSessionFail = @"GetLocalSession Failed";
    public const String GetLocalSessionsFail = @"GetLocalSessions Failed";
    public const String GetServerSessionsFail = @"GetServerSessions Failed";
    public const String GetStreamTransferSegmentFail = @"GetStreamTransferSegment Failed";
    public const String GetStreamTransferStateFail = "GetStreamTransferState Failed";
    public const String GetTransferStateFail = "GetTransferState Failed";
    public const String GetTransferIntegrityEvidenceFail = "GetTransferIntegrityEvidence Failed";
    public const String GetTransferSegmentFail = @"GetTransferSegment Failed";
    public const String LocalSessionPersistenceFail = "Local session persistence failed {@SessionID} {@ItemID} {@Operation}";
    public const String OpenDownloadFail = "OpenDownload Failed";
    public const String OpenFollowStreamTransferFail = "OpenFollowStreamTransfer Failed";
    public const String OpenGetTransferFail = "OpenGetTransfer Failed";
    public const String OpenUploadTransferFail = "OpenUploadTransfer Failed";
    public const String OpenStreamTransferFail = "OpenStreamTransfer Failed";
    public const String OpenUploadFail = "OpenUpload Failed";
    public const String PutStreamTransferSegmentFail = @"PutStreamTransferSegment Failed";
    public const String PutTransferSegmentFail = @"PutTransferSegment Failed";
    public const String ReOpenFollowStreamTransferFail = "ReOpenFollowStreamTransfer Failed";
    public const String ReOpenGetTransferFail = "ReOpenGetTransfer Failed";
    public const String ReOpenStreamTransferFail = "ReOpenStreamTransfer Failed";
    public const String ReOpenUploadTransferFail = "ReOpenUploadTransfer Failed";
    public const String ResumeDownloadFail = "ResumeDownload Failed";
    public const String ResumeUploadFail = "ResumeUpload Failed";
    public const String RemoveTransferFail = "RemoveTransfer Failed";
    public const String TransferOpenCleanupFail = "Transfer open compensation cleanup failed {@SessionID} {@ItemID} {@Operation}";
    public const String DeleteFail      = "Delete Failed";
    public const String RegisterSubscriptionFail = "RegisterSubscription Failed";
}