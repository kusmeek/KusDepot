namespace KusDepot;

/**<include file='Settings.xml' path='Settings/class[@name="Settings"]/main/*'/>*/
public static class Settings
{
    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="CompressionBufferSize"]/*'/>*/
    public const Int32 CompressionBufferSize = 64 * 1024;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="ConversionBufferSize"]/*'/>*/
    public const Int32 ConversionBufferSize = 4 * 1024;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="ConfigurationDataEnvironmentVariable"]/*'/>*/
    public static String ConfigurationDataEnvironmentVariable {get;set;} = "ToolConfigurationData";

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="ConfigurationPathEnvironmentVariable"]/*'/>*/
    public static String ConfigurationPathEnvironmentVariable {get;set;} = String.Empty;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="ConsoleStopHostTimeout"]/*'/>*/
    public static TimeSpan ConsoleStopHostTimeout {get;set;} = TimeSpan.FromSeconds(30);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="DataImportBufferLimit"]/*'/>*/
    public static Int64 DataImportBufferLimit {get;set;} = 2_000_000_000;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="DataEncryptionConcurrency"]/*'/>*/
    public static Int32 DataEncryptionConcurrency {get;set;} = Environment.ProcessorCount;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DataSecurityEDKey"]/*'/>*/ 
    public const String DataSecurityEDKey = "EncryptedData";

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DataSecurityHCKey"]/*'/>*/
    public const String DataSecurityHCKey = "HashCode";

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DataSecurityCNKey"]/*'/>*/
    public const String DataSecurityCNKey = "Content";

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DefaultCryptoChunkPower"]/*'/>*/
    public const Int32 DefaultCryptoChunkPower = 20;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="ExecuteCommandTimeout"]/*'/>*/
    public static TimeSpan ExecuteCommandTimeout {get;set;} = InfiniteTimeSpan;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="GetOutputInterval"]/*'/>*/
    public static Int32 GetOutputInterval {get;set;} = 4000;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="GetOutputTimeout"]/*'/>*/
    public static TimeSpan GetOutputTimeout {get;set;} = TimeSpan.FromMinutes(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="HostStopTimeout"]/*'/>*/
    public static TimeSpan HostStopTimeout {get;set;} = TimeSpan.FromMinutes(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="NoExceptions"]/*'/>*/
    public static Boolean NoExceptions {get;set;} = true;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="SyncTime"]/*'/>*/
    public static TimeSpan SyncTime {get;set;} = TimeSpan.FromSeconds(10);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="ZeroMemoryBufferSize"]/*'/>*/
    public const Int32 ZeroMemoryBufferSize = 81920;
}