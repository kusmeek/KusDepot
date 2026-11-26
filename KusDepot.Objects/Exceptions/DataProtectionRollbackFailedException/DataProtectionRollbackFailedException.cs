namespace KusDepot.Exceptions;

/**<include file='DataProtectionRollbackFailedException.xml' path='DataProtectionRollbackFailedException/class[@name="DataProtectionRollbackFailedException"]/main/*'/>*/
public sealed class DataProtectionRollbackFailedException : Exception
{
    /**<include file='DataProtectionRollbackFailedException.xml' path='DataProtectionRollbackFailedException/class[@name="DataProtectionRollbackFailedException"]/property[@name="Operation"]/*'/>*/
    public String Operation { get; }

    /**<include file='DataProtectionRollbackFailedException.xml' path='DataProtectionRollbackFailedException/class[@name="DataProtectionRollbackFailedException"]/property[@name="DestinationPath"]/*'/>*/
    public String? DestinationPath { get; }

    /**<include file='DataProtectionRollbackFailedException.xml' path='DataProtectionRollbackFailedException/class[@name="DataProtectionRollbackFailedException"]/property[@name="BackupPath"]/*'/>*/
    public String? BackupPath { get; }

    /**<include file='DataProtectionRollbackFailedException.xml' path='DataProtectionRollbackFailedException/class[@name="DataProtectionRollbackFailedException"]/property[@name="StagingPath"]/*'/>*/
    public String? StagingPath { get; }

    /**<include file='DataProtectionRollbackFailedException.xml' path='DataProtectionRollbackFailedException/class[@name="DataProtectionRollbackFailedException"]/constructor[@name="Constructor"]/*'/>*/
    public DataProtectionRollbackFailedException(String operation , String? destinationPath , String? backupPath , String? stagingPath , Exception innerException)
        : base($"The {operation} operation failed and rollback also failed.",innerException)
    {
        ArgumentException.ThrowIfNullOrEmpty(operation); ArgumentNullException.ThrowIfNull(innerException);

        this.Operation = operation;
        this.DestinationPath = destinationPath;
        this.BackupPath = backupPath;
        this.StagingPath = stagingPath;
    }
}
