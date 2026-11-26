namespace DataPodServices.DataControl;

/**<include file='TransferSessionCleanupCandidate.xml' path='TransferSessionCleanupCandidate/record[@name="TransferSessionCleanupCandidate"]/main/*'/>*/
public sealed record TransferSessionCleanupCandidate(DateTimeOffset LastActivity , Guid SessionID , Guid ItemID , String CatalogName);
