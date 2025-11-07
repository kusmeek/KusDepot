namespace KusDepot.Data;

internal static class SecureStrings
{
    public const String AdminFilePath        = @"C:\KusDepotConfig\AdminSilo.xml";
    public const String ConfigFilePath       = @"C:\KusDepotConfig\Secure\appsettings.json";
    public const String LogFilePath          = @"C:\KusDepotLogs\Secure.log";

    public const String Activated            = @"Secure Activated";
    public const String ActivateFail         = @"Secure Activate Failed";
    public const String AdminRole            = @"Admin.All";
    public const String AdminLoadFail        = @"Admin Load Failed";
    public const String BadArg               = @"An argument was not valid";
    public const String Deactivated          = @"Secure Deactivated";
    public const String HostProcessExit      = @"Secure Host Process Exiting {@PID}";
    public const String IsAdminFail          = @"IsAdmin Failed {@User}";
    public const String IsAdminFinish        = @"IsAdmin Finished {@User}";
    public const String OpenIDConfigURL      = @"https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
    public const String RegisterSuccess      = @"Secure Actor Registration Succeeded {@PID}";
    public const String SecureFail           = @"Secure Service Failed";
    public const String SetAdminAuthFail     = @"SetAdmin Authorization Failed";
    public const String SetAdminAuthFailUser = @"SetAdmin Authorization Failed {@User}";
    public const String SetAdminFail         = @"SetAdmin Failed {@User}";
    public const String SetAdminSuccess      = @"SetAdmin Succeeded";
    public const String SetAdminSuccessUser  = @"SetAdmin Succeeded {@User}";
    public const String StartDiagnosticFail  = @"StartDiagnostic Failed {@Method}";
    public const String StartUpFail          = @"Secure StartUp Failed";
    public const String TraceServiceName     = @"Secure";
    public const String TraceSourceName      = @"KusDepot.Data.Secure";
    public const String ValidateVerifyFail   = @"ValidateTokenVerifyRole Failed {@User} {@Role}";
    public const String ValidateVerifyFinish = @"ValidateVerify Finished {@User} {@Role}";
}