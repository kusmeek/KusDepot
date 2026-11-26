namespace KusDepot.Exceptions;

/**<include file='StagedFileTransformRollbackException.xml' path='StagedFileTransformRollbackException/class[@name="StagedFileTransformRollbackException"]/main/*'/>*/
internal sealed class StagedFileTransformRollbackException(String destinationPath , String? backupPath , String? stagingPath , Exception innerException)
    : IOException("The staged file transform rollback failed.",innerException)
{
    /**<include file='StagedFileTransformRollbackException.xml' path='StagedFileTransformRollbackException/class[@name="StagedFileTransformRollbackException"]/property[@name="DestinationPath"]/*'/>*/
    internal String DestinationPath { get; } = destinationPath;

    /**<include file='StagedFileTransformRollbackException.xml' path='StagedFileTransformRollbackException/class[@name="StagedFileTransformRollbackException"]/property[@name="BackupPath"]/*'/>*/
    internal String? BackupPath { get; } = backupPath;

    /**<include file='StagedFileTransformRollbackException.xml' path='StagedFileTransformRollbackException/class[@name="StagedFileTransformRollbackException"]/property[@name="StagingPath"]/*'/>*/
    internal String? StagingPath { get; } = stagingPath;
}
