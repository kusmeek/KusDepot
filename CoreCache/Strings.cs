namespace KusDepot.Data;

internal static class CoreCacheStrings
{
    public const String ConfigFilePath      = @"C:\KusDepotConfig\CoreCache\appsettings.json";
    public const String LogFilePath         = @"C:\KusDepotLogs\CoreCache.log";

    public const String Activated           = @"CoreCache Activated";
    public const String ActivateFail        = @"CoreCache Activate Failed";    
    public const String BadArg              = @"An argument was not valid";
    public const String Deactivated         = @"CoreCache Deactivated";
    public const String CleanUpStarted      = @"CleanUp Started";
    public const String CleanUpFailed       = @"CleanUp Failed";
    public const String CleanUpFailedID     = @"CleanUp Failed {@ID}";
    public const String CleanUpFinished     = @"CleanUp Finished";
    public const String CleanUpRemovedID    = @"CleanUp Removed {@ID}";
    public const String CoreCacheFail       = @"CoreCache Service Failed";
    public const String DeactivateFail      = @"CoreCache Deactivate Failed";
    public const String DeleteFail          = @"Delete Failed";
    public const String DeleteFailID        = @"Delete Failed {@ID}";
    public const String DeleteNotFound      = @"Delete Not Found";
    public const String DeleteNotFoundID    = @"Delete Not Found {@ID}";
    public const String DeleteSuccessID     = @"Delete Succeeded {@ID}";
    public const String ExistsFailID        = @"Exists Failed {@ID}";
    public const String ExistsSuccessID     = @"Exists Succeeded {@ID}";
    public const String GetFailID           = @"Get Failed {@ID}";
    public const String GetNotFound         = @"Get Not Found";
    public const String GetNotFoundID       = @"Get Not Found {@ID}";
    public const String GetSuccessID        = @"Get Succeeded {@ID}";
    public const String HostProcessExit     = @"CoreCache Host Process Exiting {@PID}";
    public const String RegisterSuccess     = @"CoreCache Actor Registration Succeeded {@PID}";
    public const String StartDiagnosticFail = @"StartDiagnostic Failed {@Method}";
    public const String StartUpFail         = @"CoreCache StartUp Failed";
    public const String StateNameCore       = @"CoreCache";
    public const String StateNameDate       = @"DateCache";
    public const String StoreConflict       = @"Store Conflict";
    public const String StoreConflictID     = @"Store Conflict {@ID}";
    public const String StoreFail           = @"Store Failed";
    public const String StoreFailID         = @"Store Failed {@ID}";
    public const String StoreSuccessID      = @"Store Succeeded {@ID}";
    public const String TraceServiceName    = @"CoreCache";
    public const String TraceSourceName     = @"KusDepot.Data.CoreCache";

    public const String RedisCloseFail      = @"Redis Close Failed";
    public const String RedisConnectFail    = @"Redis Connect Failed";
    public const String RedisConnectKey     = @"CoreCache:ConnectionString";
    public const String RedisConnectStart   = @"Redis Connect Starting";
    public const String RedisConnectSuccess = @"Redis Connect Succeeded";
    public const String RedisConnectSkipped = @"Redis Connect Skipped";
    public const String RedisGetDBFail      = @"Redis Unavailable.";
}