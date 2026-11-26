namespace DataPodServices.DataControl;

/**<include file='TransferPublicationRecoveryCandidate.xml' path='TransferPublicationRecoveryCandidate/record[@name="TransferPublicationRecoveryCandidate"]/main/*'/>*/
public sealed record TransferPublicationRecoveryCandidate(DateTimeOffset Updated , Guid SessionID , Guid ItemID , Guid OperationID , String CatalogName);
