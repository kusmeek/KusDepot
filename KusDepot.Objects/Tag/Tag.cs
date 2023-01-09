namespace KusDepot
{
    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "ArchitectureType"]'/> */
    public static class ArchitectureType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "ArchitectureType"]/value[@Name = "ARM"]'/> */
        public const String ARM   = "ARM";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "ArchitectureType"]/value[@Name = "ARM64"]'/> */
        public const String ARM64 = "ARM64";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "ArchitectureType"]/value[@Name = "POWER"]'/> */
        public const String POWER = "POWER";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "ArchitectureType"]/value[@Name = "X64"]'/> */
        public const String X64   = "X64";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "ArchitectureType"]/value[@Name = "X86"]'/> */
        public const String X86   = "X86";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]'/> */
    public static class BrowserType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]/value[@Name = "Brave"]'/> */
        public const String Brave   = "Brave";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]/value[@Name = "Chrome"]'/> */
        public const String Chrome  = "Chrome";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]/value[@Name = "Edge"]'/> */
        public const String Edge    = "Edge";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]/value[@Name = "Firefox"]'/> */
        public const String Firefox = "Firefox";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]/value[@Name = "Opera"]'/> */
        public const String Opera   = "Opera";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "BrowserType"]/value[@Name = "Safari"]'/> */
        public const String Safari  = "Safari";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]'/> */
    public static class DatabaseType
    { 
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "AzureBlob"]'/> */
        public const String AzureBlob      = "AzureBlob";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "AzureCosmos"]'/> */
        public const String AzureCosmos    = "AzureCosmos";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "AzureDataLake"]'/> */
        public const String AzureDataLake  = "AzureDataLake";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "AzureSQL"]'/> */
        public const String AzureSQL       = "AzureSQL";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "AmazonS3"]'/> */
        public const String AmazonS3       = "AmazonS3";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "GoogleAlloyDB"]'/> */
        public const String GoogleAlloyDB  = "GoogleAlloyDB";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "GoogleBigtable"]'/> */
        public const String GoogleBigtable = "GoogleBigtable";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "GoogleSpanner"]'/> */
        public const String GoogleSpanner  = "GoogleSpanner";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "OracleCloud"]'/> */
        public const String OracleCloud    = "OracleCloud";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DatabaseType"]/value[@Name = "SQLServer"]'/> */
        public const String SQLServer      = "SQLServer";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]'/> */
    public static class DeploymentType
    {           
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "AzureAutomationDeployment"]'/> */
        public const String AzureAutomationDeployment = "AzureAutomationDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "AzureFunctionDeployment"]'/> */
        public const String AzureFunctionDeployment   = "AzureFunctionDeployment";
        
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "BinaryDeployment"]'/> */
        public const String BinaryDeployment          = "BinaryDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "ContainerDeployment"]'/> */
        public const String ContainerDeployment       = "ContainerDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "EnclaveDeployment"]'/> */
        public const String EnclaveDeployment         = "EnclaveDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "InternalDeployment"]'/> */
        public const String InternalDeployment        = "InternalDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "Kubernetes"]'/> */
        public const String Kubernetes                = "Kubernetes";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "ObfuscatedDeployment"]'/> */
        public const String ObfuscatedDeployment      = "ObfuscatedDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "PackageDeployment"]'/> */
        public const String PackageDeployment         = "PackageDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "PublicDeployment"]'/> */
        public const String PublicDeployment          = "PublicDeployment";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "ReliableActor"]'/> */
        public const String ReliableActor             = "ReliableActor";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "DeploymentType"]/value[@Name = "ReliableService"]'/> */
        public const String ReliableService           = "ReliableService";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]'/> */
    public static class FrameworkVersionType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "Angular"]'/> */
        public const String Angular = "Angular";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "net4"]'/> */
        public const String net4    = "net4";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "net5"]'/> */
        public const String net5    = "net5";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "net6"]'/> */
        public const String net6    = "net6";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "net7"]'/> */
        public const String net7    = "net7";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "React"]'/> */
        public const String React   = "React";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "FrameworkVersionType"]/value[@Name = "Vue"]'/> */
        public const String Vue     = "Vue";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]'/> */
    public static class HostType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "Core"]'/> */
        public const String Core    = "Core";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "Deno"]'/> */
        public const String Deno    = "Deno";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "Full"]'/> */
        public const String Full    = "Full";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "Mono"]'/> */
        public const String Mono    = "Mono";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "Node"]'/> */
        public const String Node    = "Node";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "OpenJ9"]'/> */
        public const String OpenJ9  = "OpenJ9";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "OpenJDK"]'/> */
        public const String OpenJDK = "OpenJDK";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "HostType"]/value[@Name = "V8"]'/> */
        public const String V8      = "V8";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]'/> */
    public static class Language
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "ara"]'/> */
        public const String ara = "ara";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "arz"]'/> */
        public const String arz = "arz";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "ces"]'/> */
        public const String ces = "ces";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "dan"]'/> */
        public const String dan = "dan";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "deu"]'/> */
        public const String deu = "deu";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "ell"]'/> */
        public const String ell = "ell";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "eng"]'/> */
        public const String eng = "eng";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "spa"]'/> */
        public const String spa = "spa";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "fas"]'/> */
        public const String fas = "fas";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "fin"]'/> */
        public const String fin = "fin";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "fra"]'/> */
        public const String fra = "fra";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "gle"]'/> */
        public const String gle = "gle";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "heb"]'/> */
        public const String heb = "heb";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "hin"]'/> */
        public const String hin = "hin";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "hun"]'/> */
        public const String hun = "hun";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "ita"]'/> */
        public const String ita = "ita";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "jpn"]'/> */
        public const String jpn = "jpn";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "kor"]'/> */
        public const String kor = "kor";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "ktu"]'/> */
        public const String ktu = "ktu";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "lin"]'/> */
        public const String lin = "lin";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "lua"]'/> */
        public const String lua = "lua";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "mkd"]'/> */
        public const String mkd = "mkd";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "mon"]'/> */
        public const String mon = "mon";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "msa"]'/> */
        public const String msa = "msa";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "nob"]'/> */
        public const String nob = "nob";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "nde"]'/> */
        public const String nde = "nde";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "nld"]'/> */
        public const String nld = "nld";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "nya"]'/> */
        public const String nya = "nya";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "pan"]'/> */
        public const String pan = "pan";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "pol"]'/> */
        public const String pol = "pol";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "por"]'/> */
        public const String por = "por";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "rus"]'/> */
        public const String rus = "rus";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "sag"]'/> */
        public const String sag = "sag";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "san"]'/> */
        public const String san = "san";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "slv"]'/> */
        public const String slv = "slv";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "sna"]'/> */
        public const String sna = "sna";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "swa"]'/> */
        public const String swa = "swa";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "swe"]'/> */
        public const String swe = "swe";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "tha"]'/> */
        public const String tha = "tha";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "tsn"]'/> */
        public const String tsn = "tsn";
        
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "tur"]'/> */
        public const String tur = "tur";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "uig"]'/> */
        public const String uig = "uig";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "ukr"]'/> */
        public const String ukr = "ukr";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "vie"]'/> */
        public const String vie = "vie";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "xho"]'/> */
        public const String xho = "xho";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "Language"]/value[@Name = "zho"]'/> */
        public const String zho = "zho";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]'/> */
    public static class OperatingSystemType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "Android"]'/> */
        public const String Android       = "Android";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "HoloLens"]'/> */
        public const String HoloLens      = "HoloLens";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "iOS"]'/> */
        public const String iOS           = "iOS";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "Linux"]'/> */
        public const String Linux         = "Linux";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "Macintosh"]'/> */
        public const String Macintosh     = "Macintosh";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "Windows"]'/> */
        public const String Windows       = "Windows";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "OperatingSystemType"]/value[@Name = "WindowsServer"]'/> */
        public const String WindowsServer = "WindowsServer";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]'/> */
    public static class PlatformType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]/value[@Name = "Azure"]'/> */
        public const String Azure     = "Azure";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]/value[@Name = "Amazon"]'/> */
        public const String Amazon    = "Amazon";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]/value[@Name = "Google"]'/> */
        public const String Google    = "Google";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]/value[@Name = "Meta"]'/> */
        public const String Meta      = "Meta";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]/value[@Name = "TwitchTV"]'/> */
        public const String TwitchTV  = "TwitchTV";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "PlatformType"]/value[@Name = "YouTubeTV"]'/> */
        public const String YouTubeTV = "YouTubeTV";
    }

    /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]'/> */
    public static class UsageType
    {
        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Adapter"]'/> */
        public const String Adapter                  = "Adapter";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "APIServer"]'/> */
        public const String APIServer                = "APIServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "AppProxy"]'/> */
        public const String AppProxy                 = "AppProxy";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "AppServer"]'/> */
        public const String AppServer                = "AppServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Architecture"]'/> */
        public const String Architecture             = "Architecture";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ASPNETServer"]'/> */
        public const String ASPNETServer             = "ASPNETServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "BlazorServer"]'/> */
        public const String BlazorServer             = "BlazorServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Bot"]'/> */
        public const String Bot                      = "Bot";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "BuildServer"]'/> */
        public const String BuildServer              = "BuildServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Cache"]'/> */
        public const String Cache                    = "Cache";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ClientManagement"]'/> */
        public const String ClientManagement         = "ClientManagement";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Cluster"]'/> */
        public const String Cluster                  = "Cluster";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ClusterNode"]'/> */
        public const String ClusterNode              = "ClusterNode";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Contract"]'/> */
        public const String Contract                 = "Contract";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Corporation"]'/> */
        public const String Corporation              = "Corporation";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Database"]'/> */
        public const String Database                 = "Database";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Decision"]'/> */
        public const String Decision                 = "Decision";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Department"]'/> */
        public const String Department               = "Department";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "DHCPServer"]'/> */
        public const String DHCPServer               = "DHCPServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "DomainController"]'/> */
        public const String DomainController         = "DomainController";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "DNSServer"]'/> */
        public const String DNSServer                = "DNSServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Event"]'/> */
        public const String Event                    = "Event";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "FederationServer"]'/> */
        public const String FederationServer         = "FederationServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Firewall"]'/> */
        public const String Firewall                 = "Firewall";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "FSFile"]'/> */
        public const String FSFile                   = "FSFile";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "FSDirectory"]'/> */
        public const String FSDirectory              = "FSDirectory";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "FTPServer"]'/> */
        public const String FTPServer                = "FTPServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Generator"]'/> */
        public const String Generator                = "Generator";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "IntegrationLaboratory"]'/> */
        public const String IntegrationLaboratory    = "IntegrationLaboratory";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Key"]'/> */
        public const String Key                      = "Key";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "L2Router"]'/> */
        public const String L2Router                 = "L2Router";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "L3Router"]'/> */
        public const String L3Router                 = "L3Router";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "LiveSignal"]'/> */
        public const String LiveSignal               = "LiveSignal";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Lock"]'/> */
        public const String Lock                     = "Lock";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ManagementServer"]'/> */
        public const String ManagementServer         = "ManagementServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "MediaStreamingServer"]'/> */
        public const String MediaStreamingServer     = "MediaStreamingServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Model"]'/> */
        public const String Model                    = "Model";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Monitor"]'/> */
        public const String Monitor                  = "Monitor";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Objective"]'/> */
        public const String Objective                = "Objective";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "OCSPResponder"]'/> */
        public const String OCSPResponder            = "OCSPResponder";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Orchestrator"]'/> */
        public const String Orchestrator             = "Orchestrator";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "OrganizationalUnit"]'/> */
        public const String OrganizationalUnit       = "OrganizationalUnit";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Parser"]'/> */
        public const String Parser                   = "Parser";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "PCITransactionHandler"]'/> */
        public const String PCITransactionHandler    = "PCITransactionHandler";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "PKICAServer"]'/> */
        public const String PKICAServer              = "PKICAServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "PKICMPServer"]'/> */
        public const String PKICMPServer             = "PKICMPServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "PKIRootServer"]'/> */
        public const String PKIRootServer            = "PKIRootServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Plan"]'/> */
        public const String Plan                     = "Plan";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Policy"]'/> */
        public const String Policy                   = "Policy";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "PrintServer"]'/> */
        public const String PrintServer              = "PrintServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ProductArtifact"]'/> */
        public const String ProductArtifact          = "ProductArtifact";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ProtocolProxyServer"]'/> */
        public const String ProtocolProxyServer      = "ProtocolProxyServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Result"]'/> */
        public const String Result                   = "Result";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ReverseProxyServer"]'/> */
        public const String ReverseProxyServer       = "ReverseProxyServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "RoboticProcessAutomation"]'/> */
        public const String RoboticProcessAutomation = "RoboticProcessAutomation";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "SANServer"]'/> */
        public const String SANServer                = "SANServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "SAWorkstation"]'/> */
        public const String SAWorkstation            = "SAWorkstation";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Scheduler"]'/> */
        public const String Scheduler                = "Scheduler";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "ServerManagement"]'/> */
        public const String ServerManagement         = "ServerManagement";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "SessionCoordinator"]'/> */
        public const String SessionCoordinator       = "SessionCoordinator";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "SSASInstance"]'/> */
        public const String SSASInstance             = "SSASInstance";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Task"]'/> */
        public const String Task                     = "Task";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Translator"]'/> */
        public const String Translator               = "Translator";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Visualizer"]'/> */
        public const String Visualizer               = "Visualizer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "VPNServer"]'/> */
        public const String VPNServer                = "VPNServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "VSProject"]'/> */
        public const String VSProject                = "VSProject";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "VSSolution"]'/> */
        public const String VSSolution               = "VSSolution";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "WebServer"]'/> */
        public const String WebServer                = "WebServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "WebSocketServer"]'/> */
        public const String WebSocketServer          = "WebSocketServer";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Window"]'/> */
        public const String Window                   = "Window";

        /** <include file = 'Tag.xml' path = 'Tag/class[@Name = "UsageType"]/value[@Name = "Workflow"]'/> */
        public const String Workflow                 = "Workflow";
    }

}