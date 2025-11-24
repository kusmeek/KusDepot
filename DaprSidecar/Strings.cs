namespace KusDepot.Dapr;

internal static class SupervisorStrings
{
    public const String ConfigFilePath         = @"C:\KusDepotConfig\DaprSupervisor\sidecarsettings.json";
    public const String LogFilePath            = @"C:\KusDepotLogs\DaprSupervisor.log";

    public const String CheckServiceHealthFail  = @"CheckServiceHealth Failed {@ServiceType}";
    public const String CommandExecuteFail      = @"Command Execution Failed {@ServiceType}";
    public const String DaprSupervisorClosing   = @"DaprSupervisor Service Closing";
    public const String DaprSupervisorFail      = @"DaprSupervisor Service Failed";
    public const String DaprSupervisorStarted   = @"DaprSupervisor Service Started";
    public const String DaprSupervisorStopped   = @"DaprSupervisor Service Stopped";
    public const String ExecutingCommand        = @"Executing Command {@ServiceType} {@ManagerConfig}";
    public const String InitializeSchedulerFail = @"InitializeScheduler Failed [{@ObjectType}] [{@ObjectID}]";
    public const String InvalidProcessID        = @"Invalid Process ID {@ServiceType} {@ManagerConfig}";
    public const String ManagerStarted          = @"Manager Started {@ServiceType} {@ManagerConfig}";
    public const String ManagerStarting         = @"Manager Starting {@ServiceType} {@ManagerConfig}";
    public const String HostProcessExit         = @"DaprSupervisor Host Process Exiting {@PID}";
    public const String JobExecutionFail        = @"Job Execution Failed.";
    public const String ManageFail              = @"Manage Failed {@ServiceType} {@ManagerConfig}";
    public const String RegisterSuccess         = @"DaprSupervisor Service Registration Succeeded {@PID}";
    public const String RestartServiceFail      = @"RestartService Failed {@ServiceType}";
    public const String ServiceHealthy          = @"Service Healthy {@ServiceType} {@PID} {@ManagerConfig}";
    public const String ServiceUnHealthy        = @"Service UnHealthy {@ServiceType} {@ManagerConfig}";
    public const String Started                 = @"Service Started {@ServiceType}";
    public const String StartingFail            = @"Service Starting Failed {@ServiceType}";
    public const String StartedFail             = @"Service Started Failed {@ServiceType}";
    public const String StartFail               = @"Service Start Failed {@ServiceType}";
    public const String StartUpFail             = @"DaprSupervisor Service StartUp Failed";
    public const String StopFail                = @"Service Stop Failed {@ServiceType}";
    public const String TerminateServiceFail    = @"TerminateService Failed {@ServiceType}";
}