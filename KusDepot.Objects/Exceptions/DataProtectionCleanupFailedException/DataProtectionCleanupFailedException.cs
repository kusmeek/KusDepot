namespace KusDepot.Exceptions;

/**<include file='DataProtectionCleanupFailedException.xml' path='DataProtectionCleanupFailedException/class[@name="DataProtectionCleanupFailedException"]/main/*'/>*/
public sealed class DataProtectionCleanupFailedException : Exception
{
    /**<include file='DataProtectionCleanupFailedException.xml' path='DataProtectionCleanupFailedException/class[@name="DataProtectionCleanupFailedException"]/property[@name="Operation"]/*'/>*/
    public String Operation { get; }

    /**<include file='DataProtectionCleanupFailedException.xml' path='DataProtectionCleanupFailedException/class[@name="DataProtectionCleanupFailedException"]/property[@name="BackupPath"]/*'/>*/
    public String? BackupPath { get; }

    /**<include file='DataProtectionCleanupFailedException.xml' path='DataProtectionCleanupFailedException/class[@name="DataProtectionCleanupFailedException"]/constructor[@name="Constructor"]/*'/>*/
    public DataProtectionCleanupFailedException(String operation , String? backupPath , Exception innerException)
        : base($"The {operation} operation committed, but post-commit cleanup failed.",innerException)
    {
        ArgumentException.ThrowIfNullOrEmpty(operation); ArgumentNullException.ThrowIfNull(innerException);

        this.Operation = operation; this.BackupPath = backupPath;
    }
}