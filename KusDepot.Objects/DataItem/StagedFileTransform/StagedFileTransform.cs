namespace KusDepot;

/**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/main/*'/>*/
internal sealed class StagedFileTransform : IDisposable
{
    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/field[@name="destinationPath"]/*'/>*/
    private readonly String destinationPath;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/field[@name="digest"]/*'/>*/
    private readonly Byte[]? digest;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/field[@name="stagingPath"]/*'/>*/
    private String? stagingPath;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/field[@name="backupPath"]/*'/>*/
    private String? backupPath;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/field[@name="state"]/*'/>*/
    private State state;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/constructor[@name="Constructor"]/*'/>*/
    internal StagedFileTransform(String destinationPath , String stagingPath , Byte[]? digest = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(destinationPath); ArgumentException.ThrowIfNullOrEmpty(stagingPath);

        if(digest is not null && digest.Length != 64) { throw new ArgumentException("A SHA-512 transformed-file digest is required.",nameof(digest)); }

        if(String.Equals(Path.GetFullPath(destinationPath),Path.GetFullPath(stagingPath),OrdinalIgnoreCase)) { throw new ArgumentException("The staging path must differ from the destination path.",nameof(stagingPath)); }

        String? destinationDirectory = Path.GetDirectoryName(Path.GetFullPath(destinationPath));

        String? stagingDirectory = Path.GetDirectoryName(Path.GetFullPath(stagingPath));

        if(!String.Equals(destinationDirectory,stagingDirectory,OrdinalIgnoreCase)) { throw new ArgumentException("The staging file must be in the destination directory.",nameof(stagingPath)); }

        this.destinationPath = Path.GetFullPath(destinationPath);

        this.stagingPath = Path.GetFullPath(stagingPath);

        this.digest = digest is null ? null : (Byte[])digest.Clone();
    }

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/property[@name="DestinationPath"]/*'/>*/
    internal String DestinationPath => this.destinationPath;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/property[@name="StagingPath"]/*'/>*/
    internal String? StagingPath => this.stagingPath;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/property[@name="BackupPath"]/*'/>*/
    internal String? BackupPath => this.backupPath;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/property[@name="Digest"]/*'/>*/
    internal Byte[]? Digest => this.digest is null ? null : (Byte[])this.digest.Clone();

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/property[@name="IsCommitted"]/*'/>*/
    internal Boolean IsCommitted => this.state is State.Committed;

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/method[@name="Commit"]/*'/>*/
    internal void Commit()
    {
        if(this.state is not State.Prepared || this.stagingPath is null) { throw new InvalidOperationException("The staged file transform is not prepared."); }

        if(File.Exists(this.stagingPath) is false || File.Exists(this.destinationPath) is false) { throw new FileNotFoundException("The staged file or destination file is unavailable."); }

        String? directory = Path.GetDirectoryName(this.destinationPath);

        if(String.IsNullOrEmpty(directory)) { throw new IOException("The destination file does not have a parent directory."); }

        String backup;

        do { backup = Path.Combine(directory,$".kusdepot-{Guid.NewGuid():N}.bak"); }

        while(File.Exists(backup));

        this.backupPath = backup;

        try
        {
            File.Replace(this.stagingPath,this.destinationPath,backup);

            this.stagingPath = null;

            this.state = State.Committed;
        }
        catch
        {
            Boolean backupExists = File.Exists(backup);

            Boolean stagingExists = this.stagingPath is not null && File.Exists(this.stagingPath);

            if(backupExists && stagingExists is false)
            {
                this.stagingPath = null;

                this.state = State.Committed;
            }
            else if(backupExists is false) { this.backupPath = null; }

            throw;
        }
    }

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/method[@name="Rollback"]/*'/>*/
    internal void Rollback()
    {
        try
        {
            if(this.state is State.Prepared)
            {
                Boolean destinationExists = File.Exists(this.destinationPath);

                Boolean backupExists = this.backupPath is not null && File.Exists(this.backupPath);

                if(backupExists)
                {
                    if(destinationExists) { throw new IOException("The destination and backup files both exist; rollback cannot determine which file is authoritative."); }

                    File.Move(this.backupPath!,this.destinationPath);

                    this.backupPath = null;
                }
                else if(destinationExists is false) { throw new IOException("The destination file is missing and no backup is available for restoration."); }

                if(this.stagingPath is not null) { File.Delete(this.stagingPath); this.stagingPath = null; }

                this.state = State.RolledBack;

                return;
            }

            if(this.state is not State.Committed || this.backupPath is null) { throw new InvalidOperationException("The staged file transform cannot be rolled back in its current state."); }

            File.Replace(this.backupPath,this.destinationPath,null);

            this.backupPath = null;

            this.state = State.RolledBack;
        }
        catch ( Exception error ) { throw new StagedFileTransformRollbackException(this.destinationPath,this.backupPath,this.stagingPath,error); }
    }

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/method[@name="Complete"]/*'/>*/
    internal void Complete()
    {
        if(this.state is not State.Committed || this.backupPath is null) { throw new InvalidOperationException("Only a committed staged file transform can be completed."); }

        File.Delete(this.backupPath);

        this.backupPath = null;

        this.state = State.Completed;
    }

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        if(this.state is State.Prepared) { this.Rollback(); }

        else if(this.state is State.Committed) { this.Rollback(); }
    }

    /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/enum[@name="State"]/main/*'/>*/
    private enum State : Byte
    {
        /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/enum[@name="State"]/value[@name="Prepared"]/*'/>*/
        Prepared,

        /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/enum[@name="State"]/value[@name="Committed"]/*'/>*/
        Committed,

        /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/enum[@name="State"]/value[@name="Completed"]/*'/>*/
        Completed,

        /**<include file='StagedFileTransform.xml' path='StagedFileTransform/class[@name="StagedFileTransform"]/enum[@name="State"]/value[@name="RolledBack"]/*'/>*/
        RolledBack
    }
}
