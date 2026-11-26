namespace DataPodServices.DataControl;

/**<include file='PostgresDataControlDeletionJobStore.xml' path='PostgresDataControlDeletionJobStore/class[@name="PostgresDataControlDeletionJobStore"]/main/*'/>*/
public sealed class PostgresDataControlDeletionJobStore : IDataControlDeletionJobStore
{
    /**<include file='PostgresDataControlDeletionJobStore.xml' path='PostgresDataControlDeletionJobStore/class[@name="PostgresDataControlDeletionJobStore"]/field[@name="contexts"]/*'/>*/
    private readonly IDbContextFactory<DataControlDbContext> contexts;

    /**<include file='PostgresDataControlDeletionJobStore.xml' path='PostgresDataControlDeletionJobStore/class[@name="PostgresDataControlDeletionJobStore"]/constructor[@name="PostgresDataControlDeletionJobStore"]/*'/>*/
    public PostgresDataControlDeletionJobStore(IDbContextFactory<DataControlDbContext> contexts)
    {
        this.contexts = contexts ?? throw new ArgumentNullException(nameof(contexts));
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='EnsureDeletionJobAsync']/*"/>*/
    public async Task<DataControlDeletionJobState> EnsureDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
    {
        ValidateIdentity(catalogname,itemid,itemInstanceToken);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await context.Database.ExecuteSqlAsync($"""
            INSERT INTO "DeletionJobs" ("CatalogName", "ItemID", "ItemInstanceToken", "State", "AttemptCount", "Created", "Updated")
            VALUES ({catalogname}, {itemid}, {itemInstanceToken}, {(Int32)DataControlDeletionProgress.CatalogPending}, 0, {now}, {now})
            ON CONFLICT ("CatalogName", "ItemID", "ItemInstanceToken") DO NOTHING
            """,cancel);

        DataControlDeletionJobRow? row = await context.DeletionJobs.AsNoTracking().SingleOrDefaultAsync(job => job.CatalogName == catalogname && job.ItemID == itemid && job.ItemInstanceToken == itemInstanceToken,cancel);

        if(row is null) { throw new InvalidOperationException("The deletion job could not be created or loaded."); }

        return ToState(row);
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='LoadDeletionJobAsync']/*"/>*/
    public async Task<DataControlDeletionJobState?> LoadDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
    {
        ValidateIdentity(catalogname,itemid,itemInstanceToken);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlDeletionJobRow? row = await context.DeletionJobs.AsNoTracking().SingleOrDefaultAsync(job => job.CatalogName == catalogname && job.ItemID == itemid && job.ItemInstanceToken == itemInstanceToken,cancel);

        return row is null ? null : ToState(row);
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='LoadDueDeletionJobsAsync']/*"/>*/
    public async Task<IReadOnlyList<DataControlDeletionJobState>> LoadDueDeletionJobsAsync(DateTimeOffset now , Int32 pageSize , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        List<DataControlDeletionJobRow> rows = await context.DeletionJobs.AsNoTracking()
            .Where(job => job.NextAttempt == null || job.NextAttempt <= now)
            .OrderBy(job => job.NextAttempt).ThenBy(job => job.Updated).Take(pageSize).ToListAsync(cancel);

        return rows.Select(ToState).ToArray();
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='AdvanceDeletionAsync']/*"/>*/
    public async Task<Boolean> AdvanceDeletionAsync(String catalogname , Guid itemid , Guid itemInstanceToken , DataControlDeletionProgress expectedprogress , DataControlDeletionProgress progress , CancellationToken cancel = default)
    {
        ValidateIdentity(catalogname,itemid,itemInstanceToken);

        if((Int32)progress != (Int32)expectedprogress + 1) { throw new ArgumentException("Deletion progress must advance exactly one step.",nameof(progress)); }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 updated = await context.DeletionJobs
            .Where(job => job.CatalogName == catalogname && job.ItemID == itemid && job.ItemInstanceToken == itemInstanceToken && job.State == (Int32)expectedprogress)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(job => job.State,(Int32)progress)
            .SetProperty(job => job.LastError,(String?)null)
            .SetProperty(job => job.NextAttempt,(DateTimeOffset?)null)
            .SetProperty(job => job.Updated,DateTimeOffset.UtcNow),cancel);

        return updated == 1;
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='RecordDeletionFailureAsync']/*"/>*/
    public async Task<Boolean> RecordDeletionFailureAsync(String catalogname , Guid itemid , Guid itemInstanceToken , String error , DateTimeOffset nextattempt , CancellationToken cancel = default)
    {
        ValidateIdentity(catalogname,itemid,itemInstanceToken); ArgumentException.ThrowIfNullOrWhiteSpace(error);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 updated = await context.DeletionJobs
            .Where(job => job.CatalogName == catalogname && job.ItemID == itemid && job.ItemInstanceToken == itemInstanceToken)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(job => job.LastError,error)
            .SetProperty(job => job.AttemptCount,job => job.AttemptCount + 1)
            .SetProperty(job => job.LastAttempt,DateTimeOffset.UtcNow)
            .SetProperty(job => job.NextAttempt,nextattempt)
            .SetProperty(job => job.Updated,DateTimeOffset.UtcNow),cancel);

        return updated == 1;
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='DeleteDeletionJobAsync']/*"/>*/
    public async Task<Boolean> DeleteDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
    {
        ValidateIdentity(catalogname,itemid,itemInstanceToken);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 deleted = await context.DeletionJobs.Where(job => job.CatalogName == catalogname && job.ItemID == itemid && job.ItemInstanceToken == itemInstanceToken).ExecuteDeleteAsync(cancel);

        return deleted == 1;
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='ValidateIdentity']/*"/>*/
    private static void ValidateIdentity(String catalogname , Guid itemid , Guid itemInstanceToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname); ArgumentOutOfRangeException.ThrowIfEqual(itemid,Guid.Empty); ArgumentOutOfRangeException.ThrowIfEqual(itemInstanceToken,Guid.Empty);
    }

    /**<include file='PostgresDataControlDeletionJobStore.xml' path="PostgresDataControlDeletionJobStore/class[@name='PostgresDataControlDeletionJobStore']/method[@name='ToState']/*"/>*/
    private static DataControlDeletionJobState ToState(DataControlDeletionJobRow row)
    {
        return new DataControlDeletionJobState
        {
            CatalogName = row.CatalogName,
            ItemID = row.ItemID,
            ItemInstanceToken = row.ItemInstanceToken,
            Progress = (DataControlDeletionProgress)row.State,
            LastError = row.LastError,
            AttemptCount = row.AttemptCount,
            LastAttempt = row.LastAttempt,
            NextAttempt = row.NextAttempt,
            Created = row.Created,
            Updated = row.Updated,
        };
    }
}
