namespace KusDepot.Data;

internal static class DataConfigsStrings
{
    public const String ConfigFilePath      = @"C:\KusDepotConfig\DataConfiguration\appsettings.json";
    public const String LogFilePath         = @"C:\KusDepotLogs\DataConfiguration.log";
    public const String StartUpSiloFilePath = @"C:\KusDepotConfig\StartUpSilo.xml";

    public const String Activated           = @"DataConfiguration Activated";
    public const String ActivateFail        = @"DataConfiguration Activate Failed";
    public const String ActivateLoadFail    = @"DataConfiguration Activate Load StartUp File Failed";
    public const String ActivateLoadSuccess = @"DataConfiguration Activate Load StartUp File Succeeded";
    public const String BadArg              = @"An argument was not valid";
    public const String DataConfigsFail     = @"DataConfiguration Service Failed";
    public const String Deactivated         = @"DataConfiguration Deactivated";
    public const String GetReadEmpty        = @"GetAuthorizedReadSilo No Silos Stored";
    public const String GetReadFail         = @"GetAuthorizedReadSilo Failed";
    public const String GetReadNone         = @"GetAuthorizedReadSilo No Authorized Silos";
    public const String GetReadSuccess      = @"GetAuthorizedReadSilo Succeeded";
    public const String GetSilosAuth        = @"GetStorageSilos Authorization Failed";
    public const String GetSilosEmpty       = @"GetStorageSilos No Silos Stored";
    public const String GetSilosFail        = @"GetStorageSilos Failed";
    public const String GetSilosSuccess     = @"GetStorageSilos Succeeded";
    public const String GetWriteEmpty       = @"GetAuthorizedWriteSilo No Silos Stored";
    public const String GetWriteFail        = @"GetAuthorizedWriteSilo Failed";
    public const String GetWriteNone        = @"GetAuthorizedWriteSilo No Authorized Silos";
    public const String GetWriteSuccess     = @"GetAuthorizedWriteSilo Succeeded";
    public const String HostProcessExit     = @"DataConfiguration Host Process Exiting {@PID}";
    public const String RegisterSuccess     = @"DataConfiguration Actor Registration Succeeded {@PID}";
    public const String SetSilosAuth        = @"SetStorageSilos Authorization Failed";
    public const String SetSilosFail        = @"SetStorageSilos Failed";
    public const String SetSilosSuccess     = @"SetStorageSilos Succeeded";
    public const String StartDiagnosticFail = @"StartDiagnostic Failed {@Method}";
    public const String StartUpFail         = @"DataConfiguration StartUp Failed";
    public const String StateName           = @"StorageSilos";
    public const String TraceServiceName    = @"DataConfiguration";
    public const String TraceSourceName     = @"KusDepot.Data.Configuration";
}