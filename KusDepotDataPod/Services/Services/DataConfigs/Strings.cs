namespace DataPodServices.DataConfigs;

internal static class DataConfigsStrings
{
    public const String AdminFilePath       = @"C:\KusDepotConfig\DataPod\AdminSilo.xml";
    public const String ConfigFilePath      = @"C:\KusDepotConfig\DataPod\DataConfiguration\appsettings.json";
    public const String LogFilePath         = @"C:\KusDepotLogs\DataPod\DataConfiguration.log";
    public const String StartUpSiloFilePath = @"C:\KusDepotConfig\DataPod\StartUpSilo.xml";

    public const String Activated           = @"DataConfiguration Activated";
    public const String ActivateFail        = @"DataConfiguration Activate Failed";
    public const String ActivateLoadFail    = @"DataConfiguration Activate Load StartUp File Failed";
    public const String ActivateLoadSuccess = @"DataConfiguration Activate Load StartUp File Succeeded";
    public const String BadArg              = @"DataConfiguration An argument was not valid";
    public const String DataConfigsFail     = @"DataConfiguration Service Failed";
    public const String Deactivated         = @"DataConfiguration Deactivated";
    public const String DeactivateFail      = @"DataConfiguration Deactivate Failed";
    public const String GetReadEmpty        = @"DataConfiguration GetAuthorizedReadSilo No Silos Stored";
    public const String GetReadFail         = @"DataConfiguration GetAuthorizedReadSilo Failed";
    public const String GetReadNone         = @"DataConfiguration GetAuthorizedReadSilo No Authorized Silos";
    public const String GetReadSuccess      = @"DataConfiguration GetAuthorizedReadSilo Succeeded";
    public const String GetSilosAuth        = @"DataConfiguration GetStorageSilos Authorization Failed";
    public const String GetSilosEmpty       = @"DataConfiguration GetStorageSilos No Silos Stored";
    public const String GetSilosFail        = @"DataConfiguration GetStorageSilos Failed";
    public const String GetSilosSuccess     = @"DataConfiguration GetStorageSilos Succeeded";
    public const String GetWriteEmpty       = @"DataConfiguration GetAuthorizedWriteSilo No Silos Stored";
    public const String GetWriteFail        = @"DataConfiguration GetAuthorizedWriteSilo Failed";
    public const String GetWriteNone        = @"DataConfiguration GetAuthorizedWriteSilo No Authorized Silos";
    public const String GetWriteSuccess     = @"DataConfiguration GetAuthorizedWriteSilo Succeeded";
    public const String HostProcessExit     = @"DataConfiguration Host Process Exiting {@PID}";
    public const String SetSilosAuth        = @"DataConfiguration SetStorageSilos Authorization Failed";
    public const String SetSilosFail        = @"DataConfiguration SetStorageSilos Failed";
    public const String SetSilosSuccess     = @"DataConfiguration SetStorageSilos Succeeded";
    public const String StartDiagnosticFail = @"DataConfiguration StartDiagnostic Failed {@Method}";
    public const String StartUpFail         = @"DataConfiguration StartUp Failed";
    public const String TraceServiceName    = @"DataPodConfiguration";
    public const String TraceSourceName     = @"DataPodServices.Configuration";
}