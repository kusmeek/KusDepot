namespace KusDepot;

/**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/main/*'/>*/
public static class DataTags
{
    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/main/*'/>*/
    public static class Architecture
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/value[@name="ARM"]/*'/>*/
        public const String ARM   = "ARM";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/value[@name="ARM64"]/*'/>*/
        public const String ARM64 = "ARM64";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/value[@name="POWER"]/*'/>*/
        public const String POWER = "POWER";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/value[@name="X64"]/*'/>*/
        public const String X64   = "X64";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/value[@name="X86"]/*'/>*/
        public const String X86   = "X86";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Architecture"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Architecture).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/main/*'/>*/
    public static class Browser
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Brave"]/*'/>*/
        public const String Brave   = "Brave";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Chrome"]/*'/>*/
        public const String Chrome  = "Chrome";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Edge"]/*'/>*/
        public const String Edge    = "Edge";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Firefox"]/*'/>*/
        public const String Firefox = "Firefox";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Opera"]/*'/>*/
        public const String Opera   = "Opera";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Safari"]/*'/>*/
        public const String Safari  = "Safari";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/value[@name="Shift"]/*'/>*/
        public const String Shift   = "Shift";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Browser"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Browser).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/main/*'/>*/
    public static class Database
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="AzureSQL"]/*'/>*/
        public const String AzureSQL        = "AzureSQL";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="AzureBlob"]/*'/>*/
        public const String AzureBlob       = "AzureBlob";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="AzureCosmos"]/*'/>*/
        public const String AzureCosmos     = "AzureCosmos";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="AzureDataLake"]/*'/>*/
        public const String AzureDataLake   = "AzureDataLake";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="MicrosoftFabric"]/*'/>*/
        public const String MicrosoftFabric = "MicrosoftFabric";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="SQLServer"]/*'/>*/
        public const String SQLServer       = "SQLServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="AmazonS3"]/*'/>*/
        public const String AmazonS3        = "AmazonS3";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="GoogleAlloyDB"]/*'/>*/
        public const String GoogleAlloyDB   = "GoogleAlloyDB";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="GoogleBigtable"]/*'/>*/
        public const String GoogleBigtable  = "GoogleBigtable";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="GoogleSpanner"]/*'/>*/
        public const String GoogleSpanner   = "GoogleSpanner";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/value[@name="OracleCloud"]/*'/>*/
        public const String OracleCloud     = "OracleCloud";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Database"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Database).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/main/*'/>*/
    public static class Deployment
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="AzureAutomationDeployment"]/*'/>*/
        public const String AzureAutomationDeployment = "AzureAutomationDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="AzureFunctionDeployment"]/*'/>*/
        public const String AzureFunctionDeployment   = "AzureFunctionDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="BinaryDeployment"]/*'/>*/
        public const String BinaryDeployment          = "BinaryDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="ContainerDeployment"]/*'/>*/
        public const String ContainerDeployment       = "ContainerDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="DaprActor"]/*'/>*/
        public const String DaprActor                 = "DaprActor";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="EnclaveDeployment"]/*'/>*/
        public const String EnclaveDeployment         = "EnclaveDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="InternalDeployment"]/*'/>*/
        public const String InternalDeployment        = "InternalDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="Kubernetes"]/*'/>*/
        public const String Kubernetes                = "Kubernetes";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="ObfuscatedDeployment"]/*'/>*/
        public const String ObfuscatedDeployment      = "ObfuscatedDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="PackageDeployment"]/*'/>*/
        public const String PackageDeployment         = "PackageDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="PublicDeployment"]/*'/>*/
        public const String PublicDeployment          = "PublicDeployment";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="ReliableActor"]/*'/>*/
        public const String ReliableActor             = "ReliableActor";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/value[@name="ReliableService"]/*'/>*/
        public const String ReliableService           = "ReliableService";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Deployment"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Deployment).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/main/*'/>*/
    public static class FrameworkVersion
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="Angular"]/*'/>*/
        public const String Angular = "Angular";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net4"]/*'/>*/
        public const String net4    = "net4";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net5"]/*'/>*/
        public const String net5    = "net5";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net6"]/*'/>*/
        public const String net6    = "net6";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net7"]/*'/>*/
        public const String net7    = "net7";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net8"]/*'/>*/
        public const String net8    = "net8";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net9"]/*'/>*/
        public const String net9    = "net9";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net10"]/*'/>*/
        public const String net10   = "net10";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="net11"]/*'/>*/
        public const String net11   = "net11";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="React"]/*'/>*/
        public const String React   = "React";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/value[@name="Vue"]/*'/>*/
        public const String Vue     = "Vue";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="FrameworkVersion"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(FrameworkVersion).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/main/*'/>*/
    public static class Hardware
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Desktop"]/*'/>*/
        public const String Desktop    = "Desktop";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Drone"]/*'/>*/
        public const String Drone      = "Drone";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="IoT"]/*'/>*/
        public const String IoT        = "IoT";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Laptop"]/*'/>*/
        public const String Laptop     = "Laptop";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Server"]/*'/>*/
        public const String Server     = "Server";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Smartphone"]/*'/>*/
        public const String Smartphone = "Smartphone";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Surface"]/*'/>*/
        public const String Surface    = "Surface";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="Tablet"]/*'/>*/
        public const String Tablet     = "Tablet";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/value[@name="ThinClient"]/*'/>*/
        public const String ThinClient = "ThinClient";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Hardware"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Hardware).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/main/*'/>*/
    public static class Host
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="Core"]/*'/>*/
        public const String Core    = "Core";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="Deno"]/*'/>*/
        public const String Deno    = "Deno";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="Full"]/*'/>*/
        public const String Full    = "Full";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="Mono"]/*'/>*/
        public const String Mono    = "Mono";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="Node"]/*'/>*/
        public const String Node    = "Node";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="OpenJ9"]/*'/>*/
        public const String OpenJ9  = "OpenJ9";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="OpenJDK"]/*'/>*/
        public const String OpenJDK = "OpenJDK";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/value[@name="V8"]/*'/>*/
        public const String V8      = "V8";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Host"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Host).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/main/*'/>*/
    public static class Language
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="ara"]/*'/>*/
        public const String ara = "ara";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="arz"]/*'/>*/
        public const String arz = "arz";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="ces"]/*'/>*/
        public const String ces = "ces";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="dan"]/*'/>*/
        public const String dan = "dan";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="deu"]/*'/>*/
        public const String deu = "deu";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="ell"]/*'/>*/
        public const String ell = "ell";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="eng"]/*'/>*/
        public const String eng = "eng";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="spa"]/*'/>*/
        public const String spa = "spa";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="fas"]/*'/>*/
        public const String fas = "fas";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="fin"]/*'/>*/
        public const String fin = "fin";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="fra"]/*'/>*/
        public const String fra = "fra";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="gle"]/*'/>*/
        public const String gle = "gle";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="heb"]/*'/>*/
        public const String heb = "heb";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="hin"]/*'/>*/
        public const String hin = "hin";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="hun"]/*'/>*/
        public const String hun = "hun";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="ita"]/*'/>*/
        public const String ita = "ita";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="jpn"]/*'/>*/
        public const String jpn = "jpn";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="kor"]/*'/>*/
        public const String kor = "kor";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="ktu"]/*'/>*/
        public const String ktu = "ktu";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="lin"]/*'/>*/
        public const String lin = "lin";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="lit"]/*'/>*/
        public const String lit = "lit";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="lua"]/*'/>*/
        public const String lua = "lua";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="mkd"]/*'/>*/
        public const String mkd = "mkd";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="mon"]/*'/>*/
        public const String mon = "mon";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="msa"]/*'/>*/
        public const String msa = "msa";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="nob"]/*'/>*/
        public const String nob = "nob";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="nde"]/*'/>*/
        public const String nde = "nde";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="nld"]/*'/>*/
        public const String nld = "nld";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="nya"]/*'/>*/
        public const String nya = "nya";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="pan"]/*'/>*/
        public const String pan = "pan";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="pol"]/*'/>*/
        public const String pol = "pol";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="por"]/*'/>*/
        public const String por = "por";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="rus"]/*'/>*/
        public const String rus = "rus";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="sag"]/*'/>*/
        public const String sag = "sag";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="san"]/*'/>*/
        public const String san = "san";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="slv"]/*'/>*/
        public const String slv = "slv";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="sna"]/*'/>*/
        public const String sna = "sna";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="swa"]/*'/>*/
        public const String swa = "swa";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="swe"]/*'/>*/
        public const String swe = "swe";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="tha"]/*'/>*/
        public const String tha = "tha";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="tsn"]/*'/>*/
        public const String tsn = "tsn";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="tur"]/*'/>*/
        public const String tur = "tur";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="uig"]/*'/>*/
        public const String uig = "uig";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="ukr"]/*'/>*/
        public const String ukr = "ukr";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="vie"]/*'/>*/
        public const String vie = "vie";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="xho"]/*'/>*/
        public const String xho = "xho";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/value[@name="zho"]/*'/>*/
        public const String zho = "zho";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Language"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Language).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/main/*'/>*/
    public static class OperatingSystem
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="Android"]/*'/>*/
        public const String Android           = "Android";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="HoloLens"]/*'/>*/
        public const String HoloLens          = "HoloLens";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="iOS"]/*'/>*/
        public const String iOS               = "iOS";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="Linux"]/*'/>*/
        public const String Linux             = "Linux";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="Macintosh"]/*'/>*/
        public const String Macintosh         = "Macintosh";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="Windows"]/*'/>*/
        public const String Windows           = "Windows";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="WindowsServer"]/*'/>*/
        public const String WindowsServer     = "WindowsServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/value[@name="WindowsServerNano"]/*'/>*/
        public const String WindowsServerNano = "WindowsServerNano";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="OperatingSystem"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(OperatingSystem).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/main/*'/>*/
    public static class Platform
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/value[@name="Azure"]/*'/>*/
        public const String Azure     = "Azure";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/value[@name="Amazon"]/*'/>*/
        public const String Amazon    = "Amazon";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/value[@name="Google"]/*'/>*/
        public const String Google    = "Google";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/value[@name="Meta"]/*'/>*/
        public const String Meta      = "Meta";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/value[@name="TwitchTV"]/*'/>*/
        public const String TwitchTV  = "TwitchTV";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/value[@name="YouTubeTV"]/*'/>*/
        public const String YouTubeTV = "YouTubeTV";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Platform"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Platform).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }

    /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/main/*'/>*/
    public static class Usage
    {
        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Adapter"]/*'/>*/
        public const String Adapter                  = "Adapter";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Agent"]/*'/>*/
        public const String Agent                    = "Agent";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Algorithm"]/*'/>*/
        public const String Algorithm                = "Algorithm";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="APIServer"]/*'/>*/
        public const String APIServer                = "APIServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="AppProxy"]/*'/>*/
        public const String AppProxy                 = "AppProxy";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="AppServer"]/*'/>*/
        public const String AppServer                = "AppServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Architecture"]/*'/>*/
        public const String Architecture             = "Architecture";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ASPNETServer"]/*'/>*/
        public const String ASPNETServer             = "ASPNETServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="BlazorServer"]/*'/>*/
        public const String BlazorServer             = "BlazorServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="BuildServer"]/*'/>*/
        public const String BuildServer              = "BuildServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Cache"]/*'/>*/
        public const String Cache                    = "Cache";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ClientManagement"]/*'/>*/
        public const String ClientManagement         = "ClientManagement";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Cluster"]/*'/>*/
        public const String Cluster                  = "Cluster";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ClusterNode"]/*'/>*/
        public const String ClusterNode              = "ClusterNode";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Command"]/*'/>*/
        public const String Command                  = "Command";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Contract"]/*'/>*/
        public const String Contract                 = "Contract";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Corporation"]/*'/>*/
        public const String Corporation              = "Corporation";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Database"]/*'/>*/
        public const String Database                 = "Database";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Decision"]/*'/>*/
        public const String Decision                 = "Decision";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Department"]/*'/>*/
        public const String Department               = "Department";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="DHCPServer"]/*'/>*/
        public const String DHCPServer               = "DHCPServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="DomainController"]/*'/>*/
        public const String DomainController         = "DomainController";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="DNSServer"]/*'/>*/
        public const String DNSServer                = "DNSServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Emulator"]/*'/>*/
        public const String Emulator                 = "Emulator";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Event"]/*'/>*/
        public const String Event                    = "Event";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="FederationServer"]/*'/>*/
        public const String FederationServer         = "FederationServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Firewall"]/*'/>*/
        public const String Firewall                 = "Firewall";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="FSFile"]/*'/>*/
        public const String FSFile                   = "FSFile";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="FSDirectory"]/*'/>*/
        public const String FSDirectory              = "FSDirectory";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="FTPServer"]/*'/>*/
        public const String FTPServer                = "FTPServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Gateway"]/*'/>*/
        public const String Gateway                  = "Gateway";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Generator"]/*'/>*/
        public const String Generator                = "Generator";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="IntegrationLaboratory"]/*'/>*/
        public const String IntegrationLaboratory    = "IntegrationLaboratory";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Key"]/*'/>*/
        public const String Key                      = "Key";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="L2Router"]/*'/>*/
        public const String L2Router                 = "L2Router";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="L3Router"]/*'/>*/
        public const String L3Router                 = "L3Router";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="LiveSignal"]/*'/>*/
        public const String LiveSignal               = "LiveSignal";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="LoadBalancer"]/*'/>*/
        public const String LoadBalancer             = "LoadBalancer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Lock"]/*'/>*/
        public const String Lock                     = "Lock";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ManagementServer"]/*'/>*/
        public const String ManagementServer         = "ManagementServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="MediaStreamingServer"]/*'/>*/
        public const String MediaStreamingServer     = "MediaStreamingServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Message"]/*'/>*/
        public const String Message                  = "Message";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Model"]/*'/>*/
        public const String Model                    = "Model";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Monitor"]/*'/>*/
        public const String Monitor                  = "Monitor";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Network"]/*'/>*/
        public const String Network                  = "Network";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Objective"]/*'/>*/
        public const String Objective                = "Objective";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="OCSPResponder"]/*'/>*/
        public const String OCSPResponder            = "OCSPResponder";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Orchestrator"]/*'/>*/
        public const String Orchestrator             = "Orchestrator";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="OrganizationalUnit"]/*'/>*/
        public const String OrganizationalUnit       = "OrganizationalUnit";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Parser"]/*'/>*/
        public const String Parser                   = "Parser";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="PCITransactionHandler"]/*'/>*/
        public const String PCITransactionHandler    = "PCITransactionHandler";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="PKICAServer"]/*'/>*/
        public const String PKICAServer              = "PKICAServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="PKICMPServer"]/*'/>*/
        public const String PKICMPServer             = "PKICMPServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="PKIRootServer"]/*'/>*/
        public const String PKIRootServer            = "PKIRootServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Plan"]/*'/>*/
        public const String Plan                     = "Plan";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Policy"]/*'/>*/
        public const String Policy                   = "Policy";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="PrintServer"]/*'/>*/
        public const String PrintServer              = "PrintServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ProductArtifact"]/*'/>*/
        public const String ProductArtifact          = "ProductArtifact";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ProtocolProxyServer"]/*'/>*/
        public const String ProtocolProxyServer      = "ProtocolProxyServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Query"]/*'/>*/
        public const String Query                    = "Query";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Renderer"]/*'/>*/
        public const String Renderer                 = "Renderer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Request"]/*'/>*/
        public const String Request                  = "Request";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Response"]/*'/>*/
        public const String Response                 = "Response";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Result"]/*'/>*/
        public const String Result                   = "Result";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ReverseProxyServer"]/*'/>*/
        public const String ReverseProxyServer       = "ReverseProxyServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="RoboticProcessAutomation"]/*'/>*/
        public const String RoboticProcessAutomation = "RoboticProcessAutomation";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="SANServer"]/*'/>*/
        public const String SANServer                = "SANServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="SAWorkstation"]/*'/>*/
        public const String SAWorkstation            = "SAWorkstation";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Scheduler"]/*'/>*/
        public const String Scheduler                = "Scheduler";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="ServerManagement"]/*'/>*/
        public const String ServerManagement         = "ServerManagement";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="SessionCoordinator"]/*'/>*/
        public const String SessionCoordinator       = "SessionCoordinator";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="SSASInstance"]/*'/>*/
        public const String SSASInstance             = "SSASInstance";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Task"]/*'/>*/
        public const String Task                     = "Task";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Translator"]/*'/>*/
        public const String Translator               = "Translator";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Visualizer"]/*'/>*/
        public const String Visualizer               = "Visualizer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="VPNServer"]/*'/>*/
        public const String VPNServer                = "VPNServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="VSProject"]/*'/>*/
        public const String VSProject                = "VSProject";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="VSSolution"]/*'/>*/
        public const String VSSolution               = "VSSolution";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="WebServer"]/*'/>*/
        public const String WebServer                = "WebServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="WebSocketServer"]/*'/>*/
        public const String WebSocketServer          = "WebSocketServer";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Window"]/*'/>*/
        public const String Window                   = "Window";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/value[@name="Workflow"]/*'/>*/
        public const String Workflow                 = "Workflow";

        /**<include file='DataTags.xml' path='DataTags/class[@name="DataTags"]/class[@name="Usage"]/property[@name="AllTags"]/*'/>*/
        public static IReadOnlyCollection<String> AllTags
        {
            get
            {
                return typeof(Usage).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(String)).Select(f => (String)f.GetValue(null)!).ToList().AsReadOnly();
            }
        }
    }
}