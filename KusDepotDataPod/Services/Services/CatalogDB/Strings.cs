namespace DataPodServices.CatalogDB;

internal static class CatalogDBStrings
{
    public const String ConfigFilePath             = @"C:\KusDepotConfig\DataPod\CatalogDB\appsettings.json";
    public const String LogFilePath                = @"C:\KusDepotLogs\DataPod\CatalogDB\CatalogDB.log";
    public const String LogDirectory               = @"C:\KusDepotLogs\DataPod\CatalogDB\";

    public const String Activated                  = @"CatalogDB Activated";
    public const String ActivatedCatalog           = @"CatalogDB Activated {@Catalog}";
    public const String ActivateFail               = @"CatalogDB Activate Failed {@Catalog}";
    public const String AddUpdateFail              = @"CatalogDB AddUpdate Failed";
    public const String AddUpdateFailDescriptor    = @"CatalogDB AddUpdate Failed {@Catalog} {@Descriptor}";
    public const String AddUpdateSuccess           = @"CatalogDB AddUpdate Succeeded {@Catalog} {@Descriptor}";
    public const String CatalogDBFail              = @"CatalogDB CatalogDB Service Failed {@Catalog}";
    public const String BadArg                     = @"CatalogDB An argument was not valid";
    public const String Deactivated                = @"CatalogDB Deactivated {@Catalog}";
    public const String DeactivateFail             = @"CatalogDB Deactivate Failed {@Catalog}";
    public const String ExistsFailDescriptor       = @"CatalogDB Exists Failed {@Catalog} {@Descriptor}";
    public const String ExistsFailID               = @"CatalogDB Exists Failed {@Catalog} {@ID}";
    public const String ExistsSuccessID            = @"CatalogDB Exists Succeeded {@Catalog} {@ID}";
    public const String HostProcessExit            = @"CatalogDB Host Process Exiting {@Catalog} {@PID}";
    public const String RegisterSuccess            = @"CatalogDB Actor Registration Succeeded {@PID}";
    public const String RemoveFail                 = @"CatalogDB Remove Failed";
    public const String RemoveFailID               = @"CatalogDB Remove Failed {@Catalog} {@ID}";
    public const String RemoveFailDescriptor       = @"CatalogDB Remove Failed {@Catalog} {@Descriptor}";
    public const String RemoveSuccessDescriptor    = @"CatalogDB Remove Succeeded {@Catalog} {@Descriptor}";
    public const String RemoveSuccessID            = @"CatalogDB Remove Succeeded {@Catalog} {@ID}";
    public const String SCFail                     = @"CatalogDB SearchCommands Failed";
    public const String SELFail                    = @"CatalogDB SearchElements Failed";
    public const String SMFail                     = @"CatalogDB SearchMedia Failed";
    public const String SNFail                     = @"CatalogDB SearchNotes Failed";
    public const String SSVFail                    = @"CatalogDB SearchServices Failed";
    public const String STGFail                    = @"CatalogDB SearchTags Failed";
    public const String StartDiagnosticFail        = @"CatalogDB StartDiagnostic Failed {@Catalog} {@Method}";
    public const String StartUpFail                = @"CatalogDB StartUp Failed";
    public const String TraceServiceName           = @"DataPodCatalogDB";
    public const String TraceSourceName            = @"DataPodServices.CatalogDB";
}