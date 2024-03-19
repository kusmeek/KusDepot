namespace TempDB;

public static class Strings
{
    public const String ConfigFilePath  = @"C:\KusDepotConfig\TempDB\settings.xml";
    public const String LogFilePath     = @"C:\KusDepotLogs\TempDB.txt";

    public const String Activated       = @"Activated {@ActorID}";
    public const String BadArg          = @"An argument was not valid";
    public const String Deactivated     = @"Deactivated {@ActorID}";
    public const String DeleteFailID    = @"Delete Failed {@ActorID} {@ID}";
    public const String DeleteSuccessID = @"Delete Succeeded {@ActorID} {@ID}";
    public const String ExistsFailID    = @"Exists Failed {@ActorID} {@ID}";
    public const String GetFailID       = @"Get Failed {@ActorID} {@ID}";
    public const String HostProcessExit = @"Host Process Exiting {@PID}";
    public const String StartUpFail     = @"StartUp Failed";
    public const String StartUpSuccess  = @"StartUp Succeeded {@PID}";
    public const String StoreFailID     = @"Store Failed {@ActorID} {@ID}";
    public const String StoreSuccessID  = @"Store Succeeded {@ActorID} {@ID}";
}