namespace KusDepot.Data;

public static class SecureStrings
{
    public const String AdminFilePath      = @"C:\KusDepotConfig\AdminSilo.xml";
    public const String ConfigFilePath     = @"C:\KusDepotConfig\Secure\settings.xml";
    public const String LogFilePath        = @"C:\KusDepotLogs\Secure.txt";

    public const String Activated          = @"Activated";
    public const String ActivateFail       = @"Activate Failed";
    public const String AdminRole          = @"Admin.All";
    public const String AdminLoadFail      = @"Admin Load Failed";
    public const String BadArg             = @"An argument was not valid";
    public const String Deactivated        = @"Deactivated";
    public const String HostProcessExit    = @"Host Process Exiting {@PID}";
    public const String IsAdminFail        = @"IsAdmin Failed";
    public const String OpenIDConfigURL    = @"https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
    public const String SetAdminAuthFail   = @"SetAdmin Authorization Failed";
    public const String SetAdminFail       = @"SetAdmin Failed";
    public const String SetAdminSuccess    = @"SetAdmin Succeeded";
    public const String StartUpFail        = @"StartUp Failed";
    public const String StartUpSuccess     = @"StartUp Succeeded {@PID}";
    public const String ValidateVerifyFail = @"ValidateTokenVerifyRole Failed";
}