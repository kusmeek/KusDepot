using DataPodServices.DataControl;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Testcontainers.PostgreSql;

namespace KusDepot.DataPodExams;

internal sealed class AgentPostgresFixture : IAsyncDisposable
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("datacontrol_agents")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private NpgsqlDataSource? dataSource;

    private PooledDbContextFactory<DataControlDbContext>? contexts;

    public ITransferStateStore TransferState { get; private set; } = null!;

    public IDataControlDeletionJobStore DeletionJobs { get; private set; } = null!;

    public IDataItemTransferCoordinator Coordinator { get; private set; } = null!;

    public IPostgresAdvisoryLeaderLock LeaderLock { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await this.container.StartAsync();

        this.dataSource = NpgsqlDataSource.Create(this.container.GetConnectionString());
        DbContextOptions<DataControlDbContext> options = new DbContextOptionsBuilder<DataControlDbContext>()
            .UseNpgsql(this.dataSource)
            .Options;
        this.contexts = new PooledDbContextFactory<DataControlDbContext>(options);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync();
        await context.Database.MigrateAsync();

        this.TransferState = new PostgresTransferStateStore(this.contexts);
        this.DeletionJobs = new PostgresDataControlDeletionJobStore(this.contexts);
        this.Coordinator = new PostgresDataItemTransferCoordinator(this.dataSource);
        this.LeaderLock = new PostgresAdvisoryLeaderLock(this.dataSource);
    }

    public async Task SetSessionLastActivityAsync(Guid sessionid , DateTimeOffset activity)
    {
        await using DataControlDbContext context = await this.contexts!.CreateDbContextAsync();
        await context.TransferSessions
            .Where(item => item.SessionID == sessionid)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.LastActivity,activity));
    }

    public async Task ExecuteSqlAsync(String sql)
    {
        await using DataControlDbContext context = await this.contexts!.CreateDbContextAsync();
        await context.Database.ExecuteSqlRawAsync(sql);
    }

    public async Task<IReadOnlyList<String>> CaptureCommandsAsync(Func<Task> operation)
    {
        List<String> commands = new();
        DbContextOptions<DataControlDbContext> options = new DbContextOptionsBuilder<DataControlDbContext>()
            .UseNpgsql(this.dataSource!)
            .LogTo(commands.Add,new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted })
            .Options;
        PooledDbContextFactory<DataControlDbContext> factory = new(options);
        ITransferStateStore original = this.TransferState;
        this.TransferState = new PostgresTransferStateStore(factory);
        try { await operation(); }
        finally { this.TransferState = original; }
        return commands;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if(this.dataSource is not null) { await this.dataSource.DisposeAsync(); }
        }
        finally
        {
            await this.container.DisposeAsync();
        }
    }
}
