namespace DataPodServices;

public static class DataPodPaths
{
    public const String HomeEnvironmentVariable = "KUSDEPOT_DATAPOD_HOME";

    public static String HomePath => ResolveHomePath();

    public static String ConfigRootPath => Path.Combine(HomePath,"config");

    public static String LogsRootPath => Path.Combine(HomePath,"logs");

    public static String DataRootPath => Path.Combine(HomePath,"data");

    public static String CertificatesRootPath => Path.Combine(HomePath,"certificates");

    public static String CatalogCertificatesDirectoryPath => Path.Combine(CertificatesRootPath,"catalog");

    public static String CatalogConfigFilePath => Path.Combine(ConfigRootPath,"Catalog","appsettings.json");

    public static String CatalogDBConfigFilePath => Path.Combine(ConfigRootPath,"CatalogDB","appsettings.json");

    public static String CatalogDBLogDirectoryPath => Path.Combine(LogsRootPath,"CatalogDB");

    public static String CatalogLogDirectoryPath => Path.Combine(LogsRootPath,"Catalog");

    public static String CatalogLogFilePath => Path.Combine(CatalogLogDirectoryPath,"Catalog.log");

    public static String CatalogServerCertificateFilePath => Path.Combine(CatalogCertificatesDirectoryPath,"server.pfx");

    public static String DataConfigurationConfigFilePath => Path.Combine(ConfigRootPath,"DataConfiguration","appsettings.json");

    public static String DataConfigurationAdminSiloFilePath => Path.Combine(ConfigRootPath,"AdminSilo.xml");

    public static String DataConfigurationLogDirectoryPath => Path.Combine(LogsRootPath,"DataConfiguration");

    public static String DataConfigurationLogFilePath => Path.Combine(DataConfigurationLogDirectoryPath,"DataConfiguration.log");

    public static String DataConfigurationStartUpSiloFilePath => Path.Combine(ConfigRootPath,"StartUpSilo.xml");

    public static String DataControlConfigFilePath => Path.Combine(ConfigRootPath,"DataControl","appsettings.json");

    public static String DataControlCertificatesDirectoryPath => Path.Combine(CertificatesRootPath,"datacontrol");

    public static String DataControlLogDirectoryPath => Path.Combine(LogsRootPath,"DataControl");

    public static String DataControlLogFilePath => Path.Combine(DataControlLogDirectoryPath,"DataControl.log");

    public static String DataControlServerCertificateFilePath => Path.Combine(DataControlCertificatesDirectoryPath,"server.pfx");

    public static String DataPodSiloConfigFilePath => Path.Combine(ConfigRootPath,"DataPodSilo","appsettings.json");

    public static String DataPodSiloLogDirectoryPath => Path.Combine(LogsRootPath,"DataPodSilo");

    public static String RootCertificatesDirectoryPath => Path.Combine(CertificatesRootPath,"root");

    public static String GetCatalogDBLogFilePath(String actorid)
    {
        if(String.IsNullOrWhiteSpace(actorid)) { throw new ArgumentException("Value cannot be null or whitespace.",nameof(actorid)); }

        return Path.Combine(CatalogDBLogDirectoryPath,$"CatalogDB-{actorid}.log");
    }

    public static String GetDataPodSiloLogFilePath(Int32 processid)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(processid);

        return Path.Combine(DataPodSiloLogDirectoryPath,$"DataPodSilo-{processid}.log");
    }

    private static String ResolveHomePath()
    {
        String? path = Environment.GetEnvironmentVariable(HomeEnvironmentVariable);

        if(String.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException($"Required environment variable '{HomeEnvironmentVariable}' is not set.");
        }

        return Path.GetFullPath(path);
    }
}
