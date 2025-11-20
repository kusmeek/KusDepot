namespace KusDepot;

/**<include file='Tag.xml' path='Tag/class[@name="ArchitectureType"]/main/*'/>*/
public static class ArchitectureType
{
    /**<include file='Tag.xml' path='Tag/class[@name="ArchitectureType"]/value[@name="ARM"]/*'/>*/
    public const String ARM   = "ARM";

    /**<include file='Tag.xml' path='Tag/class[@name="ArchitectureType"]/value[@name="ARM64"]/*'/>*/
    public const String ARM64 = "ARM64";

    /**<include file='Tag.xml' path='Tag/class[@name="ArchitectureType"]/value[@name="POWER"]/*'/>*/
    public const String POWER = "POWER";

    /**<include file='Tag.xml' path='Tag/class[@name="ArchitectureType"]/value[@name="X64"]/*'/>*/
    public const String X64   = "X64";

    /**<include file='Tag.xml' path='Tag/class[@name="ArchitectureType"]/value[@name="X86"]/*'/>*/
    public const String X86   = "X86";
}

/**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/main/*'/>*/
public static class BrowserType
{
    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Brave"]/*'/>*/
    public const String Brave   = "Brave";

    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Chrome"]/*'/>*/
    public const String Chrome  = "Chrome";

    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Edge"]/*'/>*/
    public const String Edge    = "Edge";

    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Firefox"]/*'/>*/
    public const String Firefox = "Firefox";

    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Opera"]/*'/>*/
    public const String Opera   = "Opera";

    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Safari"]/*'/>*/
    public const String Safari  = "Safari";

    /**<include file='Tag.xml' path='Tag/class[@name="BrowserType"]/value[@name="Shift"]/*'/>*/
    public const String Shift   = "Shift";
}

/**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/main/*'/>*/
public static class DatabaseType
{
    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="AzureSQL"]/*'/>*/
    public const String AzureSQL        = "AzureSQL";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="AzureBlob"]/*'/>*/
    public const String AzureBlob       = "AzureBlob";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="AzureCosmos"]/*'/>*/
    public const String AzureCosmos     = "AzureCosmos";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="AzureDataLake"]/*'/>*/
    public const String AzureDataLake   = "AzureDataLake";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="MicrosoftFabric"]/*'/>*/
    public const String MicrosoftFabric = "MicrosoftFabric";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="SQLServer"]/*'/>*/
    public const String SQLServer       = "SQLServer";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="AmazonS3"]/*'/>*/
    public const String AmazonS3        = "AmazonS3";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="GoogleAlloyDB"]/*'/>*/
    public const String GoogleAlloyDB   = "GoogleAlloyDB";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="GoogleBigtable"]/*'/>*/
    public const String GoogleBigtable  = "GoogleBigtable";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="GoogleSpanner"]/*'/>*/
    public const String GoogleSpanner   = "GoogleSpanner";

    /**<include file='Tag.xml' path='Tag/class[@name="DatabaseType"]/value[@name="OracleCloud"]/*'/>*/
    public const String OracleCloud     = "OracleCloud";
}

/**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/main/*'/>*/
public static class DeploymentType
{
    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="AzureAutomationDeployment"]/*'/>*/
    public const String AzureAutomationDeployment = "AzureAutomationDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="AzureFunctionDeployment"]/*'/>*/
    public const String AzureFunctionDeployment   = "AzureFunctionDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="BinaryDeployment"]/*'/>*/
    public const String BinaryDeployment          = "BinaryDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="ContainerDeployment"]/*'/>*/
    public const String ContainerDeployment       = "ContainerDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="DaprActor"]/*'/>*/
    public const String DaprActor                 = "DaprActor";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="EnclaveDeployment"]/*'/>*/
    public const String EnclaveDeployment         = "EnclaveDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="InternalDeployment"]/*'/>*/
    public const String InternalDeployment        = "InternalDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="Kubernetes"]/*'/>*/
    public const String Kubernetes                = "Kubernetes";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="ObfuscatedDeployment"]/*'/>*/
    public const String ObfuscatedDeployment      = "ObfuscatedDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="PackageDeployment"]/*'/>*/
    public const String PackageDeployment         = "PackageDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="PublicDeployment"]/*'/>*/
    public const String PublicDeployment          = "PublicDeployment";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="ReliableActor"]/*'/>*/
    public const String ReliableActor             = "ReliableActor";

    /**<include file='Tag.xml' path='Tag/class[@name="DeploymentType"]/value[@name="ReliableService"]/*'/>*/
    public const String ReliableService           = "ReliableService";
}

/**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/main/*'/>*/
public static class FrameworkVersionType
{
    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="Angular"]/*'/>*/
    public const String Angular = "Angular";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net4"]/*'/>*/
    public const String net4    = "net4";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net5"]/*'/>*/
    public const String net5    = "net5";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net6"]/*'/>*/
    public const String net6    = "net6";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net7"]/*'/>*/
    public const String net7    = "net7";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net8"]/*'/>*/
    public const String net8    = "net8";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net9"]/*'/>*/
    public const String net9    = "net9";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="net10"]/*'/>*/
    public const String net10   = "net10";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="React"]/*'/>*/
    public const String React   = "React";

    /**<include file='Tag.xml' path='Tag/class[@name="FrameworkVersionType"]/value[@name="Vue"]/*'/>*/
    public const String Vue     = "Vue";
}

/**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/main/*'/>*/
public static class HardwareType
{
    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Desktop"]/*'/>*/
    public const String Desktop    = "Desktop";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Drone"]/*'/>*/
    public const String Drone      = "Drone";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="IoT"]/*'/>*/
    public const String IoT        = "IoT";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Laptop"]/*'/>*/
    public const String Laptop     = "Laptop";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Server"]/*'/>*/
    public const String Server     = "Server";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Smartphone"]/*'/>*/
    public const String Smartphone = "Smartphone";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Surface"]/*'/>*/
    public const String Surface    = "Surface";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="Tablet"]/*'/>*/
    public const String Tablet     = "Tablet";

    /**<include file='Tag.xml' path='Tag/class[@name="HardwareType"]/value[@name="ThinClient"]/*'/>*/
    public const String ThinClient = "ThinClient";
}

/**<include file='Tag.xml' path='Tag/class[@name="HostType"]/main/*'/>*/
public static class HostType
{
    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="Core"]/*'/>*/
    public const String Core    = "Core";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="Deno"]/*'/>*/
    public const String Deno    = "Deno";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="Full"]/*'/>*/
    public const String Full    = "Full";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="Mono"]/*'/>*/
    public const String Mono    = "Mono";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="Node"]/*'/>*/
    public const String Node    = "Node";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="OpenJ9"]/*'/>*/
    public const String OpenJ9  = "OpenJ9";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="OpenJDK"]/*'/>*/
    public const String OpenJDK = "OpenJDK";

    /**<include file='Tag.xml' path='Tag/class[@name="HostType"]/value[@name="V8"]/*'/>*/
    public const String V8      = "V8";
}

/**<include file='Tag.xml' path='Tag/class[@name="Language"]/main/*'/>*/
public static class Language
{
    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="ara"]/*'/>*/
    public const String ara = "ara";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="arz"]/*'/>*/
    public const String arz = "arz";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="ces"]/*'/>*/
    public const String ces = "ces";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="dan"]/*'/>*/
    public const String dan = "dan";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="deu"]/*'/>*/
    public const String deu = "deu";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="ell"]/*'/>*/
    public const String ell = "ell";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="eng"]/*'/>*/
    public const String eng = "eng";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="spa"]/*'/>*/
    public const String spa = "spa";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="fas"]/*'/>*/
    public const String fas = "fas";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="fin"]/*'/>*/
    public const String fin = "fin";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="fra"]/*'/>*/
    public const String fra = "fra";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="gle"]/*'/>*/
    public const String gle = "gle";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="heb"]/*'/>*/
    public const String heb = "heb";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="hin"]/*'/>*/
    public const String hin = "hin";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="hun"]/*'/>*/
    public const String hun = "hun";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="ita"]/*'/>*/
    public const String ita = "ita";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="jpn"]/*'/>*/
    public const String jpn = "jpn";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="kor"]/*'/>*/
    public const String kor = "kor";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="ktu"]/*'/>*/
    public const String ktu = "ktu";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="lin"]/*'/>*/
    public const String lin = "lin";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="lit"]/*'/>*/
    public const String lit = "lit";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="lua"]/*'/>*/
    public const String lua = "lua";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="mkd"]/*'/>*/
    public const String mkd = "mkd";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="mon"]/*'/>*/
    public const String mon = "mon";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="msa"]/*'/>*/
    public const String msa = "msa";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="nob"]/*'/>*/
    public const String nob = "nob";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="nde"]/*'/>*/
    public const String nde = "nde";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="nld"]/*'/>*/
    public const String nld = "nld";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="nya"]/*'/>*/
    public const String nya = "nya";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="pan"]/*'/>*/
    public const String pan = "pan";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="pol"]/*'/>*/
    public const String pol = "pol";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="por"]/*'/>*/
    public const String por = "por";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="rus"]/*'/>*/
    public const String rus = "rus";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="sag"]/*'/>*/
    public const String sag = "sag";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="san"]/*'/>*/
    public const String san = "san";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="slv"]/*'/>*/
    public const String slv = "slv";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="sna"]/*'/>*/
    public const String sna = "sna";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="swa"]/*'/>*/
    public const String swa = "swa";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="swe"]/*'/>*/
    public const String swe = "swe";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="tha"]/*'/>*/
    public const String tha = "tha";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="tsn"]/*'/>*/
    public const String tsn = "tsn";
        
    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="tur"]/*'/>*/
    public const String tur = "tur";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="uig"]/*'/>*/
    public const String uig = "uig";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="ukr"]/*'/>*/
    public const String ukr = "ukr";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="vie"]/*'/>*/
    public const String vie = "vie";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="xho"]/*'/>*/
    public const String xho = "xho";

    /**<include file='Tag.xml' path='Tag/class[@name="Language"]/value[@name="zho"]/*'/>*/
    public const String zho = "zho";
}

/**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/main/*'/>*/
public static class OperatingSystemType
{
    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="Android"]/*'/>*/
    public const String Android           = "Android";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="HoloLens"]/*'/>*/
    public const String HoloLens          = "HoloLens";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="iOS"]/*'/>*/
    public const String iOS               = "iOS";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="Linux"]/*'/>*/
    public const String Linux             = "Linux";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="Macintosh"]/*'/>*/
    public const String Macintosh         = "Macintosh";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="Windows"]/*'/>*/
    public const String Windows           = "Windows";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="WindowsServer"]/*'/>*/
    public const String WindowsServer     = "WindowsServer";

    /**<include file='Tag.xml' path='Tag/class[@name="OperatingSystemType"]/value[@name="WindowsServer"]/*'/>*/
    public const String WindowsServerNano = "WindowsServerNano";
}

/**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/main/*'/>*/
public static class PlatformType
{
    /**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/value[@name="Azure"]/*'/>*/
    public const String Azure     = "Azure";

    /**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/value[@name="Amazon"]/*'/>*/
    public const String Amazon    = "Amazon";

    /**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/value[@name="Google"]/*'/>*/
    public const String Google    = "Google";

    /**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/value[@name="Meta"]/*'/>*/
    public const String Meta      = "Meta";

    /**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/value[@name="TwitchTV"]/*'/>*/
    public const String TwitchTV  = "TwitchTV";

    /**<include file='Tag.xml' path='Tag/class[@name="PlatformType"]/value[@name="YouTubeTV"]/*'/>*/
    public const String YouTubeTV = "YouTubeTV";
}

/**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/main/*'/>*/
public static class ServiceReference
{
    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="Blob"]/*'/>*/
    public const String Blob           = "Blob";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="Catalog"]/*'/>*/
    public const String Catalog        = "Catalog";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="CatalogDB"]/*'/>*/
    public const String CatalogDB      = "CatalogDB";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="CoreCache"]/*'/>*/
    public const String CoreCache      = "CoreCache";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="DaprActor"]/*'/>*/
    public const String DaprActor      = "DaprActor";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="DaprSupervisor"]/*'/>*/
    public const String DaprSupervisor = "DaprSupervisor";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="DataConfigs"]/*'/>*/
    public const String DataConfigs    = "DataConfigs";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="DataControl"]/*'/>*/
    public const String DataControl    = "DataControl";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="Management"]/*'/>*/
    public const String Management     = "Management";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="Secure"]/*'/>*/
    public const String Secure         = "Secure";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="ToolActor"]/*'/>*/
    public const String ToolActor      = "ToolActor";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="ToolFabric"]/*'/>*/
    public const String ToolFabric     = "ToolFabric";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="ToolGrain"]/*'/>*/
    public const String ToolGrain      = "ToolGrain";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="ToolService"]/*'/>*/
    public const String ToolService    = "ToolService";

    /**<include file='Tag.xml' path='Tag/class[@name="ServiceReference"]/value[@name="ToolWorkflow"]/*'/>*/
    public const String ToolWorkflow   = "ToolWorkflow";
}

/**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/main/*'/>*/
public static class UsageType
{
    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Adapter"]/*'/>*/
    public const String Adapter                  = "Adapter";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Agent"]/*'/>*/
    public const String Agent                    = "Agent";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Algorithm"]/*'/>*/
    public const String Algorithm                = "Algorithm";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="APIServer"]/*'/>*/
    public const String APIServer                = "APIServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="AppProxy"]/*'/>*/
    public const String AppProxy                 = "AppProxy";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="AppServer"]/*'/>*/
    public const String AppServer                = "AppServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Architecture"]/*'/>*/
    public const String Architecture             = "Architecture";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ASPNETServer"]/*'/>*/
    public const String ASPNETServer             = "ASPNETServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="BlazorServer"]/*'/>*/
    public const String BlazorServer             = "BlazorServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="BuildServer"]/*'/>*/
    public const String BuildServer              = "BuildServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Cache"]/*'/>*/
    public const String Cache                    = "Cache";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ClientManagement"]/*'/>*/
    public const String ClientManagement         = "ClientManagement";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Cluster"]/*'/>*/
    public const String Cluster                  = "Cluster";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ClusterNode"]/*'/>*/
    public const String ClusterNode              = "ClusterNode";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Command"]/*'/>*/
    public const String Command                  = "Command";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Contract"]/*'/>*/
    public const String Contract                 = "Contract";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Corporation"]/*'/>*/
    public const String Corporation              = "Corporation";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Database"]/*'/>*/
    public const String Database                 = "Database";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Decision"]/*'/>*/
    public const String Decision                 = "Decision";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Department"]/*'/>*/
    public const String Department               = "Department";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="DHCPServer"]/*'/>*/
    public const String DHCPServer               = "DHCPServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="DomainController"]/*'/>*/
    public const String DomainController         = "DomainController";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="DNSServer"]/*'/>*/
    public const String DNSServer                = "DNSServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Emulator"]/*'/>*/
    public const String Emulator                 = "Emulator";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Event"]/*'/>*/
    public const String Event                    = "Event";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="FederationServer"]/*'/>*/
    public const String FederationServer         = "FederationServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Firewall"]/*'/>*/
    public const String Firewall                 = "Firewall";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="FSFile"]/*'/>*/
    public const String FSFile                   = "FSFile";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="FSDirectory"]/*'/>*/
    public const String FSDirectory              = "FSDirectory";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="FTPServer"]/*'/>*/
    public const String FTPServer                = "FTPServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Gateway"]/*'/>*/
    public const String Gateway                  = "Gateway";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Generator"]/*'/>*/
    public const String Generator                = "Generator";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="IntegrationLaboratory"]/*'/>*/
    public const String IntegrationLaboratory    = "IntegrationLaboratory";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Key"]/*'/>*/
    public const String Key                      = "Key";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="L2Router"]/*'/>*/
    public const String L2Router                 = "L2Router";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="L3Router"]/*'/>*/
    public const String L3Router                 = "L3Router";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="LiveSignal"]/*'/>*/
    public const String LiveSignal               = "LiveSignal";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="LoadBalancer"]/*'/>*/
    public const String LoadBalancer             = "LoadBalancer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Lock"]/*'/>*/
    public const String Lock                     = "Lock";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ManagementServer"]/*'/>*/
    public const String ManagementServer         = "ManagementServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="MediaStreamingServer"]/*'/>*/
    public const String MediaStreamingServer     = "MediaStreamingServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Message"]/*'/>*/
    public const String Message                  = "Message";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Model"]/*'/>*/
    public const String Model                    = "Model";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Monitor"]/*'/>*/
    public const String Monitor                  = "Monitor";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Network"]/*'/>*/
    public const String Network                  = "Network";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Objective"]/*'/>*/
    public const String Objective                = "Objective";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="OCSPResponder"]/*'/>*/
    public const String OCSPResponder            = "OCSPResponder";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Orchestrator"]/*'/>*/
    public const String Orchestrator             = "Orchestrator";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="OrganizationalUnit"]/*'/>*/
    public const String OrganizationalUnit       = "OrganizationalUnit";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Parser"]/*'/>*/
    public const String Parser                   = "Parser";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="PCITransactionHandler"]/*'/>*/
    public const String PCITransactionHandler    = "PCITransactionHandler";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="PKICAServer"]/*'/>*/
    public const String PKICAServer              = "PKICAServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="PKICMPServer"]/*'/>*/
    public const String PKICMPServer             = "PKICMPServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="PKIRootServer"]/*'/>*/
    public const String PKIRootServer            = "PKIRootServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Plan"]/*'/>*/
    public const String Plan                     = "Plan";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Policy"]/*'/>*/
    public const String Policy                   = "Policy";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="PrintServer"]/*'/>*/
    public const String PrintServer              = "PrintServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ProductArtifact"]/*'/>*/
    public const String ProductArtifact          = "ProductArtifact";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ProtocolProxyServer"]/*'/>*/
    public const String ProtocolProxyServer      = "ProtocolProxyServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Query"]/*'/>*/
    public const String Query                    = "Query";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Renderer"]/*'/>*/
    public const String Renderer                 = "Renderer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Request"]/*'/>*/
    public const String Request                  = "Request";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Response"]/*'/>*/
    public const String Response                 = "Response";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Result"]/*'/>*/
    public const String Result                   = "Result";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ReverseProxyServer"]/*'/>*/
    public const String ReverseProxyServer       = "ReverseProxyServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="RoboticProcessAutomation"]/*'/>*/
    public const String RoboticProcessAutomation = "RoboticProcessAutomation";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="SANServer"]/*'/>*/
    public const String SANServer                = "SANServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="SAWorkstation"]/*'/>*/
    public const String SAWorkstation            = "SAWorkstation";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Scheduler"]/*'/>*/
    public const String Scheduler                = "Scheduler";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="ServerManagement"]/*'/>*/
    public const String ServerManagement         = "ServerManagement";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="SessionCoordinator"]/*'/>*/
    public const String SessionCoordinator       = "SessionCoordinator";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="SSASInstance"]/*'/>*/
    public const String SSASInstance             = "SSASInstance";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Task"]/*'/>*/
    public const String Task                     = "Task";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Translator"]/*'/>*/
    public const String Translator               = "Translator";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Visualizer"]/*'/>*/
    public const String Visualizer               = "Visualizer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="VPNServer"]/*'/>*/
    public const String VPNServer                = "VPNServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="VSProject"]/*'/>*/
    public const String VSProject                = "VSProject";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="VSSolution"]/*'/>*/
    public const String VSSolution               = "VSSolution";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="WebServer"]/*'/>*/
    public const String WebServer                = "WebServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="WebSocketServer"]/*'/>*/
    public const String WebSocketServer          = "WebSocketServer";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Window"]/*'/>*/
    public const String Window                   = "Window";

    /**<include file='Tag.xml' path='Tag/class[@name="UsageType"]/value[@name="Workflow"]/*'/>*/
    public const String Workflow                 = "Workflow";
}