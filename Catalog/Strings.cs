namespace KusDepot.Data;

internal static class CatalogStrings
{
    public const String ConfigFilePath      = @"C:\KusDepotConfig\Catalog\appsettings.json";
    public const String LogFilePath         = @"C:\KusDepotLogs\Catalog.log";

    public const String BadArg              = @"An argument was not valid";
    public const String CatalogClosing      = @"Catalog Service Closing";
    public const String CatalogFail         = @"Catalog Service Failed";
    public const String CatalogStartedURL   = @"Catalog Server Started at {@URL}";
    public const String CatalogStopped      = @"Catalog Server Stopped";
    public const String GNFailID            = @"GetNotes Failed {@ID}";
    public const String GTFailID            = @"GetTags Failed {@ID}";
    public const String HostProcessExit     = @"Catalog Host Process Exiting {@PID}";
    public const String RegisterSuccess     = @"Catalog Service Registration Succeeded {@PID}";
    public const String SSVFail             = @"SearchServices Failed";
    public const String SSVQueryFail        = @"GenerateServicesQuery Failed";
    public const String SSVUnAuth           = @"SearchServices Unauthorized";
    public const String SCFail              = @"SearchCommands Failed";
    public const String SCQueryFail         = @"GenerateCommandsQuery Failed";
    public const String SCUnAuth            = @"SearchCommands Unauthorized";
    public const String SELFail             = @"SearchElements Failed";
    public const String SELQueryFail        = @"GenerateElementsQuery Failed";
    public const String SELUnAuth           = @"SearchElements Unauthorized";
    public const String ServiceName         = @"Catalog";
    public const String SMFail              = @"SearchMedia Failed";
    public const String SMQueryFail         = @"GenerateMediaQuery Failed";
    public const String SMUnAuth            = @"SearchMedia Unauthorized";
    public const String SNFail              = @"SearchNotes Failed";
    public const String SNUnAuth            = @"SearchNotes Unauthorized";
    public const String StartDiagnosticFail = @"StartDiagnostic Failed {@Method}";
    public const String StartUpFail         = @"Catalog Service StartUp Failed";
    public const String STGFail             = @"SearchTags Failed";
    public const String STGUnAuth           = @"SearchTags Unauthorized";
    public const String TraceServiceName    = @"Catalog";
    public const String TraceSourceName     = @"KusDepot.Data.Catalog";
    public const String X509AuthFail        = @"Certificate Policy Oid Not Found";
    public const String X509Claim           = @"CatalogAccess";
    public const String X509NotFound        = @"TLS Server Certificate Not Found";
    public const String X509Policy          = @"CatalogAccess";
    public const String X509PolicyOid       = @"2.5.29.32.853207195657";
    public const String X509SubjectName     = @"KusDepot.Data.Catalog";
}