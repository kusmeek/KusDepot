namespace KusDepot.Data;

public static class DataConfigsStrings
{
    public const String ConfigFilePath      = @"C:\KusDepotConfig\DataConfigs\settings.xml";
    public const String LogFilePath         = @"C:\KusDepotLogs\DataConfigs.txt";
    public const String StartUpSiloFilePath = @"C:\KusDepotConfig\StartUpSilo.xml";

    public const String Activated           = @"Activated";
    public const String ActivateFail        = @"Activate Failed";
    public const String BadArg              = @"An argument was not valid";
    public const String Deactivated         = @"Deactivated";
    public const String GetReadEmpty        = @"GetAuthorizedReadSilo No Silos Stored";
    public const String GetReadFail         = @"GetAuthorizedReadSilo Failed";
    public const String GetReadNone         = @"GetAuthorizedReadSilo No Authorized Silos";
    public const String GetReadSuccess      = @"GetAuthorizedReadSilo Succeeded";
    public const String GetSilosAuth        = @"GetStorageSilos Authorization Failed";
    public const String GetSilosEmpty       = @"GetStorageSilos No Silos Stored";
    public const String GetSilosFail        = @"GetStorageSilos Failed";
    public const String GetWriteEmpty       = @"GetAuthorizedWriteSilo No Silos Stored";
    public const String GetWriteFail        = @"GetAuthorizedWriteSilo Failed";
    public const String GetWriteNone        = @"GetAuthorizedWriteSilo No Authorized Silos";
    public const String GetWriteSuccess     = @"GetAuthorizedWriteSilo Succeeded";
    public const String HostProcessExit     = @"Host Process Exiting {@PID}";
    public const String SetSilosAuth        = @"SetStorageSilos Authorization Failed";
    public const String SetSilosFail        = @"SetStorageSilos Failed";
    public const String SetSilosSuccess     = @"SetStorageSilos Succeeded";
    public const String StartUpFail         = @"StartUp Failed";
    public const String StartUpLoadFail     = @"StartUp Load Failed";
    public const String StartUpLoadSuccess  = @"StartUp Load Succeeded";
    public const String StartUpSuccess      = @"StartUp Succeeded {@PID}";
    public const String StateName           = @"StorageSilos";
}