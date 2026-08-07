namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    public void ConfigureDataControlServer(WebApplication server)
    {
        server.UseExceptionHandler(); SetupAuth(server);

        server.UseMiddleware<PublishedTransferRequestContextMiddleware>();

        MapSegmentedTransfer(server); MapStreamTransfer(server); MapDelete(server);

        server.MapHub<DataControlNotificationHub>(DataControlNotificationHub.HubPath,
            (o) => { o.AllowStatefulReconnects = true; }).RequireAuthorization(X509Policy);

        server.MapHealthChecks("DataControl/Health").DisableHttpMetrics().RequireAuthorization(X509Policy);

        SetupOpenApi(server);
    }

    public static WebApplicationOptions GetDataControlOptions()
    {
        try
        {
            String? _ = JsonDocument.Parse(File.ReadAllText(ConfigFilePath)).RootElement.GetProperty("Environment").GetString();

            if(new[]{Environments.Development,Environments.Production,Environments.Staging}.Contains(_))
            {
                return new(){ ApplicationName = ServiceName , EnvironmentName = _ };
            }

            return new(){ ApplicationName = ServiceName };
        }
        catch { return new(){ ApplicationName = ServiceName }; }
    }
}