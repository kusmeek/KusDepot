namespace DataPodServices.CatalogDB;

internal static class CatalogDBStrings
{
    public const String Activated                  = @"CatalogDB Activated";
    public const String ActivatedCatalog           = @"CatalogDB Activated {@Catalog}";
    public const String ActivateFail               = @"CatalogDB Activate Failed {@Catalog}";
    public const String AddUpdateFail              = @"AddUpdate Failed";
    public const String AddUpdateFailDescriptor    = @"AddUpdate Failed {@Catalog} {@Descriptor}";
    public const String AddUpdateSuccess           = @"AddUpdate Succeeded {@Catalog} {@ID}";
    public const String CatalogDBFail              = @"CatalogDB CatalogDB Service Failed {@Catalog}";
    public const String BadArg                     = @"An argument was not valid";
    public const String Deactivated                = @"CatalogDB Deactivated {@Catalog}";
    public const String DeactivateFail             = @"CatalogDB Deactivate Failed {@Catalog}";
    public const String ExistsFailID               = @"Exists Failed {@Catalog} {@ID}";
    public const String ExistsSuccessID            = @"Exists Succeeded {@Catalog} {@ID}";
    public const String HostProcessExit            = @"Host Process Exiting {@Catalog} {@PID}";
    public const String InitializeReadyDatabaseFail = @"InitializeReadyDatabase Failed {@Catalog}";
    public const String RemoveFail                 = @"Remove Failed";
    public const String RemoveFailID               = @"Remove Failed {@Catalog} {@ID}";
    public const String RemoveSuccessID            = @"Remove Succeeded {@Catalog} {@ID}";
    public const String SCFail                     = @"SearchCommands Failed";
    public const String SELFail                    = @"SearchElements Failed";
    public const String SMFail                     = @"SearchMedia Failed";
    public const String SNFail                     = @"SearchNotes Failed";
    public const String SSVFail                    = @"SearchServices Failed";
    public const String STGFail                    = @"SearchTags Failed";
    public const String StartDiagnosticFail        = @"StartDiagnostic Failed {@Catalog} {@Method}";
    public const String TraceServiceName           = @"DataPodCatalogDB";
    public const String TraceSourceName            = DataPodTelemetrySources.CatalogDB;
}