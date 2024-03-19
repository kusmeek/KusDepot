namespace KusDepot.Data;

public static class DataControlStrings
{
    public const String ConfigFilePath        = @"C:\KusDepotConfig\DataControl\appsettings.json";
    public const String LogFilePath           = @"C:\KusDepotLogs\DataControl.txt";

    public const String AddCacheFailID        = @"AddCache Failed {@ID}";
    public const String AddCacheSuccessID     = @"AddCache Succeeded {@ID}";
    public const String BadArg                = @"An argument was not valid";
    public const String DataControlClosing    = @"DataControl Closing";
    public const String DataControlFail       = @"DataControl Failed";
    public const String DataControlStartedURL = @"DataControl Started at {@URL}";
    public const String DataControlStopped    = @"DataControl Stopped";
    public const String DeleteArkFailID       = @"Delete Blob Succeeded Delete Ark Failed {@ID}";
    public const String DeleteBlobFailID      = @"Delete Blob Failed {@ID}";
    public const String DeleteFailID          = @"Delete Failed {@ID}";
    public const String DeleteNotFoundID      = @"Delete Not Found {@ID}";
    public const String DeleteSuccessID       = @"Delete Succeeded {@ID}";
    public const String DeleteUnAuth          = @"Delete Failed Unauthorized";
    public const String DeleteUniFailID       = @"Delete Blob Succeeded Delete Universe Failed {@ID}";
    public const String GetFailID             = @"Get Failed {@ID}";
    public const String GetNotFoundID         = @"Get Not Found {@ID}";
    public const String GetSuccessID          = @"Get Succeeded {@ID}";
    public const String GetSuccessCacheID     = @"Get Succeeded from CoreCache {@ID}";
    public const String GetUnAuth             = @"Get Failed Unauthorized";
    public const String HostProcessExit       = @"Host Process Exiting {@PID}";
    public const String StartUpFail           = @"StartUp Failed";
    public const String StartUpSuccess        = @"StartUp Succeeded {@PID}";
    public const String StoreArkFailID        = @"Store Blob Succeeded AddUpdate Ark Failed {@ID}";
    public const String StoreBlobFailID       = @"Store Blob Failed {@ID}";
    public const String StoreFailConflictID   = @"Store Failed Conflict {@ID}";
    public const String StoreFailDescriptor   = @"Store Failed {@Descriptor}";
    public const String StoreFailHash         = @"Store Failed Hash Mismatch";
    public const String StoreSuccessID        = @"Store Succeeded {@ID}";
    public const String StoreUnAuth           = @"Store Failed Unauthorized";
    public const String StoreUniFailID        = @"Store Blob Succeeded Add Universe Failed {@ID}";
}