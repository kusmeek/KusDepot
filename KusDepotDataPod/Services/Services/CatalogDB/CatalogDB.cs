using KusDepot.CatalogDb;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

[StatelessWorker]
public sealed partial class CatalogDBService : Grain , ICatalogDB
{
    private readonly CatalogDBContextFactory ctxfactory;

    private readonly CatalogDBDataSourceRegistry dataSources;

    private readonly CatalogDBInitializationCoordinator initializationCoordinator;

    public CatalogDBService(DataPodServiceLoggers loggers , IConfiguration configuration,
        CatalogDBDataSourceRegistry dataSources , CatalogDBInitializationCoordinator initializationCoordinator)
    {
        this.Logger = loggers.CatalogDB;

        try
        {
            this.dataSources = dataSources;

            this.initializationCoordinator = initializationCoordinator;

            SetupConfiguration(configuration); SetupDiagnostics();

            this.ctxfactory = new CatalogDBContextFactory();
        }
        catch ( Exception _ ) { Logger.Fatal(_,CatalogDBFail); throw; }
    }

    public override async Task OnActivateAsync(CancellationToken cancel)
    {
        try
        {
            await InitializeReadyDatabase(cancel);

            Logger.Information(ActivatedCatalog,GetActorID());
        }
        catch ( Exception _ ) { Logger.Error(_,ActivateFail,GetActorID()); throw; }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason , CancellationToken cancel)
    {
        try
        {
            Logger.Information(Deactivated,GetActorID());
        }
        catch ( Exception _ ) { Logger.Error(_,DeactivateFail,GetActorID()); }
    }
}