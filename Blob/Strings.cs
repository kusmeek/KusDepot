namespace KusDepot.Data;

internal static class BlobStrings
{
    public const String ConfigFilePath           = @"C:\KusDepotConfig\Blob\appsettings.json";
    public const String LogFilePath              = @"C:\KusDepotLogs\Blob.log";

    public const String Activated                = @"Blob Activated";
    public const String BadArg                   = @"An argument was not valid";
    public const String BlobFail                 = @"Blob Service Failed";
    public const String Deactivated              = @"Blob Deactivated";
    public const String DeleteContainerSuccessID = @"Container Deleted Successfully {@ID}";
    public const String DeleteFail               = @"Delete Failed";
    public const String DeleteFailID             = @"Delete Failed {@ID}";
    public const String DeleteFailIDVersion      = @"Delete Failed {@ID} {@Version}";
    public const String DeleteSuccessID          = @"Deleted Successfully {@ID}";
    public const String DeleteSuccessIDVersion   = @"Deleted Successfully {@ID} {@Version}";
    public const String ExistsFailID             = @"Exists Failed {@ID}";
    public const String ExistsFailIDVersion      = @"Exists Failed {@ID} {@Version}";
    public const String ExistsSuccessID          = @"Exists Succeeded {@ID}";
    public const String ExistsSuccessIDVersion   = @"Exists Succeeded {@ID} {@Version}";
    public const String GetFail                  = @"Get Failed";
    public const String GetFailID                = @"Get Failed {@ID}";
    public const String GetFailIDVersion         = @"Get Failed {@ID} {@Version}";
    public const String GetSuccessID             = @"Get Succeeded {@ID}";
    public const String GetSuccessIDVersion      = @"Get Succeeded {@ID} {@Version}";
    public const String HostProcessExit          = @"Blob Host Process Exiting {@PID}";
    public const String RegisterSuccess          = @"Blob Actor Registration Succeeded {@PID}";
    public const String StartDiagnosticFail      = @"StartDiagnostic Failed {@Method}";
    public const String StartUpFail              = @"Blob StartUp Failed";
    public const String StoreConflict            = @"Store Conflict";
    public const String StoreConflictID          = @"Store Conflict {@ID}";
    public const String StoreFailID              = @"Store Failed {@ID}";
    public const String StoreSuccessID           = @"Stored Successfully {@ID}";
    public const String TraceServiceName         = @"Blob";
    public const String TraceSourceName          = @"KusDepot.Data.Blob";
}