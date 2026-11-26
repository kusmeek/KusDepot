namespace KusDepot;

/**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/main/*'/>*/
internal sealed class DataItemProtectionTransaction : IAsyncDisposable
{
    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/field[@name="operation"]/*'/>*/
    private readonly String operation;

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/field[@name="operations"]/*'/>*/
    private readonly List<Operation> operations = new();

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/field[@name="state"]/*'/>*/
    private State state;

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/constructor[@name="Constructor"]/*'/>*/
    internal DataItemProtectionTransaction(String operation)
    {
        ArgumentException.ThrowIfNullOrEmpty(operation);

        this.operation = operation;
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="Enlist"]/*'/>*/
    internal void Enlist(Func<ValueTask> commit , Func<ValueTask> rollback , Func<ValueTask>? complete = null)
    {
        ArgumentNullException.ThrowIfNull(commit); ArgumentNullException.ThrowIfNull(rollback);

        if(this.state is not State.Prepared) { throw new InvalidOperationException("Operations can only be enlisted before the transaction starts."); }

        this.operations.Add(new Operation(commit,rollback,complete));
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="CommitAsync"]/*'/>*/
    internal async ValueTask CommitAsync()
    {
        if(this.state is not State.Prepared) { throw new InvalidOperationException("The protection transaction is not prepared."); }

        this.state = State.Committing;

        try
        {
            foreach(Operation operation in this.operations)
            {
                operation.CommitAttempted = true;

                operation.RollbackRequired = true;

                await operation.Commit().ConfigureAwait(false);
            }

            this.state = State.Committed;
        }
        catch ( Exception commitError )
        {
            List<Exception>? rollbackErrors = await this.RollbackAttemptedAsync().ConfigureAwait(false);

            if(rollbackErrors is null)
            {
                ExceptionDispatchInfo.Capture(commitError).Throw();

                throw;
            }

            this.state = State.Faulted;

            throw CreateRollbackFailedException(commitError,rollbackErrors);
        }
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="CompleteAsync"]/*'/>*/
    internal async ValueTask CompleteAsync()
    {
        if(this.state is not State.Committed && this.state is not State.FaultedAfterCommit) { throw new InvalidOperationException("Only a committed protection transaction can be completed."); }

        this.state = State.Completing;

        try
        {
            foreach(Operation operation in this.operations)
            {
                if(operation.CompletionCompleted) { continue; }

                if(operation.Complete is not null) { await operation.Complete().ConfigureAwait(false); }

                operation.CompletionCompleted = true;
            }

            this.state = State.Completed;
        }
        catch
        {
            this.state = State.FaultedAfterCommit;

            throw;
        }
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="RollbackAsync"]/*'/>*/
    internal async ValueTask RollbackAsync()
    {
        if(this.state is not State.Prepared && this.state is not State.Committed && this.state is not State.Faulted) { throw new InvalidOperationException("The protection transaction cannot be rolled back in its current state."); }

        Boolean rollbackAll = this.state is State.Prepared;

        if(rollbackAll) { foreach(Operation operation in this.operations) { operation.RollbackRequired = true; } }

        this.state = State.RollingBack;

        List<Exception>? errors;

        try { errors = await this.RollbackAttemptedAsync().ConfigureAwait(false); }

        catch
        {
            this.state = State.Faulted;

            throw;
        }

        if(errors is not null)
        {
            this.state = State.Faulted;

            throw CreateRollbackFailedException(null,errors);
        }
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="CreateRollbackFailedException"]/*'/>*/
    private DataProtectionRollbackFailedException CreateRollbackFailedException(Exception? operationError , IEnumerable<Exception> rollbackErrors)
    {
        List<Exception> errors = [];

        if(operationError is not null) { errors.Add(operationError); }

        errors.AddRange(rollbackErrors);

        AggregateException aggregate = new("The protection operation and one or more rollback operations failed.",errors);

        StagedFileTransformRollbackException? rollbackError = FindRollbackError(aggregate);

        return new DataProtectionRollbackFailedException(this.operation,rollbackError?.DestinationPath,rollbackError?.BackupPath,rollbackError?.StagingPath,aggregate);
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="FindRollbackError"]/*'/>*/
    private static StagedFileTransformRollbackException? FindRollbackError(Exception error)
    {
        if(error is StagedFileTransformRollbackException rollbackError) { return rollbackError; }

        if(error is AggregateException aggregate)
        {
            foreach(Exception inner in aggregate.InnerExceptions)
            {
                StagedFileTransformRollbackException? found = FindRollbackError(inner);

                if(found is not null) { return found; }
            }
        }

        return error.InnerException is null ? null : FindRollbackError(error.InnerException);
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        if(this.state is State.Prepared or State.Committed) { await this.RollbackAsync().ConfigureAwait(false); }
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/method[@name="RollbackAttemptedAsync"]/*'/>*/
    private async ValueTask<List<Exception>?> RollbackAttemptedAsync()
    {
        List<Exception>? errors = null;

        for(Int32 i = this.operations.Count - 1; i >= 0; i--)
        {
            Operation operation = this.operations[i];

            if(operation.RollbackCompleted || !operation.RollbackRequired) { continue; }

            try { await operation.Rollback().ConfigureAwait(false); }

            catch ( Exception error ) { (errors ??= new List<Exception>()).Add(error); continue; }

            operation.RollbackCompleted = true;

            operation.RollbackRequired = false;
        }

        if(errors is null) { this.state = State.RolledBack; }

        return errors;
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/*'/>*/
    private sealed class Operation(Func<ValueTask> commit , Func<ValueTask> rollback , Func<ValueTask>? complete)
    {
        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="Commit"]/*'/>*/
        internal Func<ValueTask> Commit { get; } = commit;

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="Rollback"]/*'/>*/
        internal Func<ValueTask> Rollback { get; } = rollback;

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="Complete"]/*'/>*/
        internal Func<ValueTask>? Complete { get; } = complete;

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="CommitAttempted"]/*'/>*/
        internal Boolean CommitAttempted { get; set; }

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="RollbackRequired"]/*'/>*/
        internal Boolean RollbackRequired { get; set; }

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="RollbackCompleted"]/*'/>*/
        internal Boolean RollbackCompleted { get; set; }

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/class[@name="Operation"]/property[@name="CompletionCompleted"]/*'/>*/
        internal Boolean CompletionCompleted { get; set; }
    }

    /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/main/*'/>*/
    private enum State : Byte
    {
        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="Prepared"]/*'/>*/
        Prepared,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="Committing"]/*'/>*/
        Committing,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="Committed"]/*'/>*/
        Committed,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="Completing"]/*'/>*/
        Completing,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="Completed"]/*'/>*/
        Completed,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="RollingBack"]/*'/>*/
        RollingBack,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="RolledBack"]/*'/>*/
        RolledBack,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="Faulted"]/*'/>*/
        Faulted,

        /**<include file='DataItemProtectionTransaction.xml' path='DataItemProtectionTransaction/class[@name="DataItemProtectionTransaction"]/enum[@name="State"]/value[@name="FaultedAfterCommit"]/*'/>*/
        FaultedAfterCommit
    }
}
