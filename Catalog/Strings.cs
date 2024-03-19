namespace KusDepot.Data;

public static class CatalogStrings
{
    public const String ConfigFilePath    = @"C:\KusDepotConfig\Catalog\appsettings.json";
    public const String LogFilePath       = @"C:\KusDepotLogs\Catalog.txt";

    public const String BadArg            = @"An argument was not valid";
    public const String CatalogClosing    = @"Catalog Closing";
    public const String CatalogFail       = @"Catalog Failed";
    public const String CatalogStartedURL = @"Catalog Started at {@URL}";
    public const String CatalogStopped    = @"Catalog Stopped";
    public const String GNFail            = @"GetNotes Failed";
    public const String GTFail            = @"GetTags Failed";
    public const String HostProcessExit   = @"Host Process Exiting {@PID}";
    public const String SASArkNull        = @"SearchActiveServices Ark Was Null";
    public const String SASFail           = @"SearchActiveServices Failed";
    public const String SASQueryFail      = @"GenerateActiveServicesQuery Failed";
    public const String SASUnAuth         = @"SearchActiveServices Unauthorized";
    public const String SELArkNull        = @"SearchElements Ark Was Null";
    public const String SELFail           = @"SearchElements Failed";
    public const String SELQueryFail      = @"GenerateElementsQuery Failed";
    public const String SELUnAuth         = @"SearchElements Unauthorized";
    public const String SMArkNull         = @"SearchMedia Ark Was Null";
    public const String SMFail            = @"SearchMedia Failed";
    public const String SMQueryFail       = @"GenerateMediaQuery Failed";
    public const String SMUnAuth          = @"SearchMedia Unauthorized";
    public const String SNArkNull         = @"SearchNotes Ark Was Null";
    public const String SNFail            = @"SearchNotes Failed";
    public const String SNUnAuth          = @"SearchNotes Unauthorized";
    public const String StartUpFail       = @"StartUp Failed";
    public const String StartUpSuccess    = @"StartUp Succeeded {@PID}";
    public const String STArkNull         = @"SearchTags Ark Was Null";
    public const String STFail            = @"SearchTags Failed";
    public const String STUnAuth          = @"SearchTags Unauthorized";
}