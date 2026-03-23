namespace DataPodServices.CoreCache;

internal static class CoreCacheStrings
{
    public const String ConfigFilePath      = @"C:\KusDepotConfig\DataPod\CoreCache\appsettings.json";
    public const String LogFilePath         = @"C:\KusDepotLogs\DataPod\CoreCache.log";
    public const String LogDirectory        = @"C:\KusDepotLogs\DataPod\CoreCache\";

    public const String Activated           = @"CoreCache Activated";
    public const String ActivateFail        = @"CoreCache Activate Failed";    
    public const String BadArg              = @"An argument was not valid";
    public const String Deactivated         = @"CoreCache Deactivated";
    public const String CleanUpStarted      = @"CoreCache CleanUp Started";
    public const String CleanUpFailed       = @"CoreCache CleanUp Failed";
    public const String CleanUpFailedID     = @"CoreCache CleanUp Failed {@ID}";
    public const String CleanUpFinished     = @"CoreCache CleanUp Finished";
    public const String CleanUpRemovedID    = @"CoreCache CleanUp Removed {@ID}";
    public const String CoreCacheFail       = @"CoreCache Service Failed";
    public const String DeactivateFail      = @"CoreCache Deactivate Failed";
    public const String DeleteFail          = @"CoreCache Delete Failed";
    public const String DeleteFailID        = @"CoreCache Delete Failed {@ID}";
    public const String DeleteNotFound      = @"CoreCache Delete Not Found";
    public const String DeleteNotFoundID    = @"CoreCache Delete Not Found {@ID}";
    public const String DeleteSuccessID     = @"CoreCache Delete Succeeded {@ID}";
    public const String ExistsFailID        = @"CoreCache Exists Failed {@ID}";
    public const String ExistsSuccessID     = @"CoreCache Exists Succeeded {@ID}";
    public const String GetFailID           = @"CoreCache Get Failed {@ID}";
    public const String GetNotFound         = @"CoreCache Get Not Found";
    public const String GetNotFoundID       = @"CoreCache Get Not Found {@ID}";
    public const String GetSuccessID        = @"CoreCache Get Succeeded {@ID}";
    public const String HostProcessExit     = @"CoreCache Host Process Exiting {@PID}";
    public const String RegisterSuccess     = @"CoreCache Actor Registration Succeeded {@PID}";
    public const String StartDiagnosticFail = @"CoreCache StartDiagnostic Failed {@Method}";
    public const String StartUpFail         = @"CoreCache StartUp Failed";
    public const String StoreConflict       = @"CoreCache Store Conflict";
    public const String StoreConflictID     = @"CoreCache Store Conflict {@ID}";
    public const String StoreFail           = @"CoreCache Store Failed";
    public const String StoreFailID         = @"CoreCache Store Failed {@ID}";
    public const String StoreSuccessID      = @"CoreCache Store Succeeded {@ID}";
    public const String TraceServiceName    = @"DataPodCoreCache";
    public const String TraceSourceName     = @"DataPodServices.CoreCache";

    public const String RedisCloseFail      = @"Redis Close Failed";
    public const String RedisConnectFail    = @"Redis Connect Failed";
    public const String RedisConnectKey     = @"CoreCache:ConnectionString";
    public const String RedisConnectStart   = @"Redis Connect Starting";
    public const String RedisConnectSuccess = @"Redis Connect Succeeded";
    public const String RedisConnectSkipped = @"Redis Connect Skipped";
    public const String RedisGetDBFail      = @"Redis Unavailable.";
}