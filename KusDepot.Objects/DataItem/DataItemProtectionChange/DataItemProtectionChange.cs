namespace KusDepot;

/**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/main/*'/>*/
internal sealed class DataItemProtectionChange
{
    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/field[@name="commit"]/*'/>*/
    private readonly Func<ValueTask> commit;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/field[@name="rollback"]/*'/>*/
    private readonly Func<ValueTask> rollback;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/field[@name="complete"]/*'/>*/
    private readonly Func<ValueTask>? complete;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/field[@name="fieldDigest"]/*'/>*/
    private readonly Byte[]? fieldDigest;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/field[@name="encryptedData"]/*'/>*/
    private readonly Byte[]? encryptedData;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/field[@name="cleanupArtifactPath"]/*'/>*/
    private readonly Func<String?>? cleanupArtifactPath;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/constructor[@name="Constructor"]/*'/>*/
    internal DataItemProtectionChange(Func<ValueTask> commit , Func<ValueTask> rollback , Byte[]? fieldDigest = null , Byte[]? encryptedData = null , Func<ValueTask>? complete = null , Func<String?>? cleanupArtifactPath = null)
    {
        ArgumentNullException.ThrowIfNull(commit); ArgumentNullException.ThrowIfNull(rollback);

        this.commit = commit;
        this.rollback = rollback;
        this.complete = complete;
        this.cleanupArtifactPath = cleanupArtifactPath;
        this.fieldDigest = fieldDigest is null ? null : (Byte[])fieldDigest.Clone();
        this.encryptedData = encryptedData;
    }

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/property[@name="FieldDigest"]/*'/>*/
    internal Byte[]? FieldDigest => this.fieldDigest is null ? null : (Byte[])this.fieldDigest.Clone();

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/property[@name="EncryptedData"]/*'/>*/
    internal Byte[]? EncryptedData => this.encryptedData;

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/property[@name="CleanupArtifactPath"]/*'/>*/
    internal String? CleanupArtifactPath => this.cleanupArtifactPath?.Invoke();

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/method[@name="CommitAsync"]/*'/>*/
    internal ValueTask CommitAsync() => this.commit();

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/method[@name="RollbackAsync"]/*'/>*/
    internal ValueTask RollbackAsync() => this.rollback();

    /**<include file='DataItemProtectionChange.xml' path='DataItemProtectionChange/class[@name="DataItemProtectionChange"]/method[@name="CompleteAsync"]/*'/>*/
    internal ValueTask CompleteAsync() => this.complete?.Invoke() ?? ValueTask.CompletedTask;
}
