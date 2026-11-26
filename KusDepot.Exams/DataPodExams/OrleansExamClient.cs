global using DataPodServices;

namespace KusDepot.DataPodExams;

internal static class OrleansExamClient
{
    public static void Configure(IClientBuilder builder)
    {
        Boolean useAzureContainer = DataPodAppConfiguration.IsAzureContainerProfile();

        if(useAzureContainer)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            DataPodAppConfiguration.AddOrleansProvider(configurationBuilder);

            IConfiguration configuration = configurationBuilder.Build();

            OrleansConfig? orleans = OrleansConfig.FromConfiguration(configuration,true);

            if(orleans is null) { throw new InvalidOperationException("Invalid Orleans Configuration"); }

            builder.Configure<ClusterOptions>((o) => { o.ClusterId = orleans.ClusterId; o.ServiceId = orleans.ServiceId; });

            builder.UseAzureStorageClustering((o) => o.TableServiceClient = new TableServiceClient(new Uri(orleans.AzureTableEndpoint!),DataPodKeyVault.CreateCredential()));
        }
        else
        {
            builder.UseLocalhostClustering(gatewayPort:30003,serviceId:"default",clusterId:"default");
        }

        builder.UseTransactions();
    }
}
