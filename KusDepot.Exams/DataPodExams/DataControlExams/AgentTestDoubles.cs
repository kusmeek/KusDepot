using DataPodServices.DataControl;
using DataPodServices.DataConfigs;
using NSubstitute;
using DataPodServices.CatalogDB;

namespace KusDepot.DataPodExams;

internal sealed class AgentTestDoubles
{
    public AgentTestDoubles()
    {
        this.Catalog = Substitute.For<ICatalogDB>();
        this.GrainFactory = Substitute.For<IGrainFactory>();
        this.GrainFactory.GetGrain<ICatalogDB>(Arg.Any<String>()).Returns(this.Catalog);
        this.Catalog.FinalizeDelete(Arg.Any<Guid>(),Arg.Any<Guid>(),Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(CatalogDeleteFinalizationResult.Removed));
        this.Catalog.GetPendingDeletes(Arg.Any<Guid?>(),Arg.Any<Int32>(),Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(new CatalogPendingDeletePage(Array.Empty<CatalogItemIdentity>(),null)));
        this.Catalog.GetItemState(Arg.Any<Guid>(),Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult<CatalogItemState?>(null));
        this.Catalog.GetItemIdentity(Arg.Any<Guid>(),Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult<CatalogItemIdentity?>(null));

        this.DataConfigs = Substitute.For<IDataControlDataConfigsClient>();
        this.DataConfigs.GetConfiguredStorageForRecoveryAsync(Arg.Any<String>(),Arg.Any<CancellationToken>())
            .Returns(call => Task.FromResult(new TenantStorageResolution(CatalogResolutionStatus.Authorized,call.ArgAt<String>(0),"UseDevelopmentStorage=true",default)));
        this.DataConfigs.GetConfiguredCatalogNamesAsync(Arg.Any<CancellationToken>())
            .Returns(new ConfiguredCatalogNames(new[] { "agent-tests" },default));

        this.TenantStorage = new FakeTenantTransferStorage();
        this.TenantStorageFactory = new FakeTenantTransferStorageFactory(this.TenantStorage);
        this.StorageContext = new FakeTenantTransferStorageContext(this.TenantStorage);
    }

    public ICatalogDB Catalog { get; }

    public IGrainFactory GrainFactory { get; }

    public IDataControlDataConfigsClient DataConfigs { get; }

    public FakeTenantTransferStorageFactory TenantStorageFactory { get; }

    public ITenantTransferStorageContext StorageContext { get; }

    public FakeTenantTransferStorage TenantStorage { get; }

    public ServiceProvider CreateDeletionServices(AgentPostgresFixture database)
    {
        ServiceCollection services = new();
        services.AddSingleton<ITransferStateStore>(database.TransferState);
        services.AddSingleton<IDataControlDeletionJobStore>(database.DeletionJobs);
        services.AddSingleton<IDataControlDataConfigsClient>(this.DataConfigs);
        services.AddSingleton<IDataItemTransferCoordinator>(database.Coordinator);
        services.AddSingleton<IGrainFactory>(this.GrainFactory);
        services.AddSingleton<IDataControlPublicationJournalStore>(new PostgresPublicationJournalStore(database.TransferState,this.DataConfigs,this.TenantStorageFactory));
        services.AddSingleton<ITenantTransferStorageFactory>(this.TenantStorageFactory);
        services.AddScoped<ITenantTransferStorageContext>(_ => this.StorageContext);
        services.AddScoped<IDataControlTransferSessionStore,DataControlTransferSessionStore>();
        services.AddScoped<IDataControlPublicationTransferStore,DataControlPublicationTransferStore>();
        return services.BuildServiceProvider();
    }

    public ServiceProvider CreateTransferSessionServices(AgentPostgresFixture database)
    {
        ServiceCollection services = new();
        services.AddSingleton<ITransferStateStore>(database.TransferState);
        services.AddSingleton<IDataControlDeletionJobStore>(database.DeletionJobs);
        services.AddSingleton<IDataControlDataConfigsClient>(this.DataConfigs);
        services.AddSingleton<IDataItemTransferCoordinator>(database.Coordinator);
        services.AddSingleton<IDataItemTransferRegistry>(new PostgresDataItemTransferRegistry(database.TransferState));
        services.AddSingleton<IGrainFactory>(this.GrainFactory);
        services.AddSingleton<IDataControlPublicationJournalStore>(new PostgresPublicationJournalStore(database.TransferState,this.DataConfigs,this.TenantStorageFactory));
        services.AddScoped<IDataControlPublicationTransferStore,DataControlPublicationTransferStore>();
        services.AddSingleton<IPublishedTransferSource>(_ => Substitute.For<IPublishedTransferSource>());
        services.Configure<DataItemTransferServiceOptions>(_ => { });
        services.AddSingleton<ITenantTransferStorageFactory>(this.TenantStorageFactory);
        services.AddScoped<ITenantTransferStorageContext>(_ => this.StorageContext);
        services.AddScoped<IDataControlTransferSessionStore,DataControlTransferSessionStore>();
        services.AddScoped<IDataItemSegmentedTransferService,DataControlSegmentedTransferService>();
        services.AddScoped<IDataStreamTransferService,DataControlStreamTransferService>();
        services.AddScoped<IDataControlNotificationPublisher>(_ => Substitute.For<IDataControlNotificationPublisher>());
        return services.BuildServiceProvider();
    }

    internal sealed class FakeTenantTransferStorageFactory(FakeTenantTransferStorage storage) : ITenantTransferStorageFactory
    {
        public Int32 DeleteInstanceTransfersCallCount { get; private set; }

        public Guid LastDeletedItemID { get; private set; }

        public Guid LastDeletedItemInstanceToken { get; private set; }

        public ITenantTransferStorage Create(StorageSilo silo , Guid sessionid , Guid itemid , Guid itemInstanceToken) => storage;

        public Task<Boolean> DeleteInstanceTransfersAsync(StorageSilo silo , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
        {
            this.DeleteInstanceTransfersCallCount++;
            this.LastDeletedItemID = itemid;
            this.LastDeletedItemInstanceToken = itemInstanceToken;
            return Task.FromResult(true);
        }
    }

    private sealed class FakeTenantTransferStorageContext(FakeTenantTransferStorage storage) : ITenantTransferStorageContext
    {
        public String CatalogName { get; private set; } = "agent-tests";

        public void Set(StorageSilo silo) => this.CatalogName = silo.CatalogName;

        public ITenantTransferStorage Get(Guid sessionid , Guid itemid , Guid itemInstanceToken) => storage;
    }

    internal sealed class FakeTenantTransferStorage : ITenantTransferStorage
    {
        public TenantTransferBlobReference? PublicationObjectReference { get; set; }

        public Byte[]? PublicationObjectBytes { get; set; }

        public Byte[]? ObjectBytes { get; set; }

        public Boolean AcceptVerifiedRanges { get; set; }

        public Int32 VerifiedRangeWrites { get; private set; }

        private readonly Dictionary<Int64,Byte[]> ranges = new();

        public Task<PublishedArtifactReference?> AssembleStreamAsync(TransferBlockPlan plan , CancellationToken cancel = default) => Task.FromResult<PublishedArtifactReference?>(null);

        public Task<Boolean> DeleteTransferAsync(CancellationToken cancel = default) => Task.FromResult(true);

        public Task<Stream?> OpenChunkAsync(Guid operationid , CancellationToken cancel = default) => Task.FromResult<Stream?>(null);

        public Task<Stream?> OpenObjectAsync(CancellationToken cancel = default) => Task.FromResult<Stream?>(this.ObjectBytes is null ? null : new MemoryStream(this.ObjectBytes,false));

        public Task<Stream?> OpenPublicationObjectAsync(Guid operationid , CancellationToken cancel = default) => Task.FromResult<Stream?>(this.PublicationObjectBytes is null ? null : new MemoryStream(this.PublicationObjectBytes,false));

        public Task<Stream?> OpenRangeAsync(DataItemTransferRange range , CancellationToken cancel = default) => Task.FromResult<Stream?>(this.ranges.TryGetValue(range.Offset,out Byte[]? bytes) ? new MemoryStream(bytes,false) : null);

        public Task<PublishedArtifactReference?> PublishObjectAsync(TenantTransferBlobReference source , CancellationToken cancel = default) => Task.FromResult<PublishedArtifactReference?>(null);

        public Task<TenantTransferBlobReference?> ReplaceObjectAsync(Stream payload , CancellationToken cancel = default) => Task.FromResult<TenantTransferBlobReference?>(null);

        public Task<PublishedArtifactReference?> SaveIntegrityAsync(Stream payload , CancellationToken cancel = default) => Task.FromResult<PublishedArtifactReference?>(null);

        public Task<PublishedArtifactReference?> TryLoadPublishedObjectAsync(CancellationToken cancel = default) => Task.FromResult<PublishedArtifactReference?>(null);

        public Task<PublishedArtifactReference?> TryLoadPublishedStreamAsync(CancellationToken cancel = default) => Task.FromResult<PublishedArtifactReference?>(null);

        public Task<PublishedArtifactReference?> TryLoadPublishedIntegrityAsync(CancellationToken cancel = default) => Task.FromResult<PublishedArtifactReference?>(null);

        public Task<TenantTransferBlobReference?> SaveObjectAsync(Stream payload , CancellationToken cancel = default) => Task.FromResult<TenantTransferBlobReference?>(null);

        public Task<TenantTransferBlobReference?> SavePublicationObjectAsync(Guid operationid , Stream payload , CancellationToken cancel = default) => Task.FromResult(this.PublicationObjectReference);

        public Task<TenantTransferBlobReference?> SaveRangeAsync(DataItemTransferRange range , Stream payload , CancellationToken cancel = default) => Task.FromResult<TenantTransferBlobReference?>(null);

        public Task<TenantTransferBlobReference?> SaveVerifiedChunkAsync(Guid operationid , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default) => Task.FromResult<TenantTransferBlobReference?>(null);

        public async Task<TenantTransferBlobReference?> SaveVerifiedRangeAsync(DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default)
        {
            this.VerifiedRangeWrites++;
            if(!this.AcceptVerifiedRanges) { return null; }

            using MemoryStream data = new();
            await payload.CopyToAsync(data,cancel);
            Byte[] bytes = data.ToArray();
            if(bytes.LongLength != expectedLength || !SHA512.HashData(bytes).AsSpan().SequenceEqual(expectedSHA512)) { return null; }
            this.ranges[range.Offset] = bytes;
            return new TenantTransferBlobReference { BlobName = "verified-range", Length = expectedLength, SHA512 = expectedSHA512 };
        }
    }
}
