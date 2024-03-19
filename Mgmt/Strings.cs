namespace KusDepot.Data;

public static class ManagementStrings
{
    public const String ConfigFilePath              = @"C:\KusDepotLogs\Management\settings.xml";
    public const String LogFilePath                 = @"C:\KusDepotLogs\Management.txt";

    public const String Activated                   = @"Activated";
    public const String ActivateFail                = @"Activate Failed";
    public const String BackupArkAttemptCatalog     = @"Backup Ark Attempting {@Catalog}";
    public const String BackupArkAuthFail           = @"Backup Ark Authorization Failed";
    public const String BackupArkSuccessCatalog     = @"Backup Ark Succeeded {@Catalog}";
    public const String BackupArkFailCatalog        = @"Backup Ark Failed {@Catalog}";
    public const String BackupDCAttempt             = @"Backup DataConfigs Attempting";
    public const String BackupDCAuthFail            = @"Backup DataConfigs Authorization Failed";
    public const String BackupDCEmpty               = @"Backup DataConfigs Empty Set";
    public const String BackupDCFail                = @"Backup DataConfigs Failed";
    public const String BackupDCSuccess             = @"Backup DataConfigs Succeeded";
    public const String BackupPrefixArk             = @"backup-arkkeeper";
    public const String BackupPrefixDC              = @"backup-dataconfigs";
    public const String BackupPrefixU               = @"backup-universe";
    public const String BackupUAttempt              = @"Backup Universe Attempting";
    public const String BackupUAuthFail             = @"Backup Universe Authorization Failed";
    public const String BackupUEmpty                = @"Backup Universe Empty Set";
    public const String BackupUFail                 = @"Backup Universe Failed";
    public const String BackupUSuccess              = @"Backup Universe Succeeded";
    public const String BadArg                      = @"An argument was not valid";
    public const String Deactivated                 = @"Deactivated";
    public const String HostProcessExit             = @"Host Process Exiting {@PID}";
    public const String RestoreArkAttemptCatalog    = @"Restore Ark Attempting {@Catalog}";
    public const String RestoreArkAuthFail          = @"Restore Ark Authorization Failed";
    public const String RestoreArkFail              = @"Restore Ark Failed {@Catalog}";
    public const String RestoreArkFoundNewest       = @"Restore Found Newest Ark {@Date}";
    public const String RestoreArkFoundNone         = @"Restore Found No Ark {@Catalog}";
    public const String RestoreArkSuccess           = @"Restore Ark {@Catalog} Succeeded to {@Date}";
    public const String RestoreDCAttempt            = @"Restore DataConfigs Attempting";
    public const String RestoreDCAuthFail           = @"Restore DataConfigs Authorization Failed";
    public const String RestoreDCFail               = @"Restore DataConfigs Failed";
    public const String RestoreDCFoundNewest        = @"Restore DataConfigs Found Newest {@Date}";
    public const String RestoreDCFoundNone          = @"Restore DataConfigs Found No Backup";
    public const String RestoreDCSuccess            = @"Restore DataConfigs Succeeded to {@Date}";
    public const String RestoreUAttempt             = @"Restore Universe Attempting";
    public const String RestoreUAuthFail            = @"Restore Universe Authorization Failed";
    public const String RestoreUFail                = @"Restore Universe Failed";
    public const String RestoreUFoundNewest         = @"Restore Universe Found Newest {@Date}";
    public const String RestoreUFoundNone           = @"Restore Universe Found No Backup";
    public const String RestoreUSuccess             = @"Restore Universe Succeeded to {@Date}";
    public const String SetAdminAuthFail            = @"SetAdmin Authorization Failed";
    public const String SetAdminFail                = @"SetAdmin Failed";
    public const String SetAdminSuccess             = @"SetAdmin Succeeded";
    public const String StartUpFail                 = @"StartUp Failed";
    public const String StartUpSuccess              = @"StartUp Succeeded {@PID}";
}