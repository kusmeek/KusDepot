namespace KusDepot.Data;

internal static class ManagementStrings
{
    public const String ConfigFilePath              = @"C:\KusDepotConfig\Management\appsettings.json";
    public const String LogFilePath                 = @"C:\KusDepotLogs\Management.log";

    public const String Activated                   = @"Management Activated";
    public const String BackupDCAttempt             = @"Backup DataConfigs Attempting";
    public const String BackupDCAuthFail            = @"Backup DataConfigs Authorization Failed";
    public const String BackupDCEmpty               = @"Backup DataConfigs Empty Set";
    public const String BackupDCFail                = @"Backup DataConfigs Failed";
    public const String BackupDCFailStore           = @"Backup DataConfigs Failed Store";
    public const String BackupDCSuccess             = @"Backup DataConfigs Succeeded";
    public const String BackupPrefixDC              = @"backup-dataconfigs";
    public const String BadArg                      = @"An argument was not valid";
    public const String Deactivated                 = @"Management Deactivated";
    public const String HostProcessExit             = @"Management Host Process Exiting {@PID}";
    public const String ManagementFail              = @"Management Service Failed";
    public const String RegisterSuccess             = @"Management Actor Registration Succeeded {@PID}";
    public const String RestoreDCAttempt            = @"Restore DataConfigs Attempting";
    public const String RestoreDCAuthFail           = @"Restore DataConfigs Authorization Failed";
    public const String RestoreDCFail               = @"Restore DataConfigs Failed";
    public const String RestoreDCFailEmpty          = @"Restore DataConfigs Failed Newest StorageSilos Backup Empty";
    public const String RestoreDCFailSet            = @"Restore DataConfigs Failed SetStorageSilos";
    public const String RestoreDCFoundNewest        = @"Restore DataConfigs Found Newest {@Date}";
    public const String RestoreDCFoundNone          = @"Restore DataConfigs Found No Backup";
    public const String RestoreDCSuccess            = @"Restore DataConfigs Succeeded to {@Date}";
    public const String StartDiagnosticFail         = @"StartDiagnostic Failed {@Method}";
    public const String StartUpFail                 = @"Management StartUp Failed";
    public const String TraceServiceName            = @"Management";
    public const String TraceSourceName             = @"KusDepot.Data.Management";
}