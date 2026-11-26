namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    public void ConfigureDataControlServer(WebApplication server)
    {
        server.UseAzureAppConfiguration();

        server.UseExceptionHandler();

        server.UseMiddleware<PublishedTransferRequestContextMiddleware>();

        server.UseMiddleware<TransferRequestBodyRateLimitMiddleware>();

        MapSegmentedTransfer(server); MapStreamTransfer(server);

        MapGetTransferIntegrityEvidence(server); MapPublicationRecoveryAdmin(server);

        MapDelete(server);

        server.MapHub<DataControlNotificationHub>(DataControlNotificationHub.HubPath,
            (o) => { o.AllowStatefulReconnects = true; });

        server.MapGet("DataControl/Health", () => Results.Ok()).DisableHttpMetrics();

        server.MapHealthChecks("DataControl/Ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(DataPodHealthCheckTags.Ready)
        }).DisableHttpMetrics();

        SetupOpenApi(server);
    }

    public static WebApplicationOptions GetDataControlOptions()
    {
        return new(){ ApplicationName = ServiceName , EnvironmentName = DataPodAppConfiguration.GetEnvironmentName() };
    }
}