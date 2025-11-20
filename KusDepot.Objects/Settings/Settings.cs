namespace KusDepot;

/**<include file='Settings.xml' path='Settings/class[@name="Settings"]/main/*'/>*/
public static class Settings
{
    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="CompressionBufferSize"]/*'/>*/
    public const Int32 CompressionBufferSize = 64 * 1024;

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

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DefaultCommandTimeout"]/*'/>*/
    public static readonly TimeSpan DefaultCommandTimeout = TimeSpan.FromHours(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DefaultCryptoChunkPower"]/*'/>*/
    public const Int32 DefaultCryptoChunkPower = 20;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DefaultGetOutputInterval"]/*'/>*/
    public static readonly Int32 DefaultGetOutputInterval = 4000;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DefaultGetOutputTimeout"]/*'/>*/
    public static readonly TimeSpan DefaultGetOutputTimeout = TimeSpan.FromMinutes(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="DefaultHostStopTimeout"]/*'/>*/
    public static readonly TimeSpan DefaultHostStopTimeout = TimeSpan.FromMinutes(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="NoExceptions"]/*'/>*/
    public static Boolean NoExceptions {get;set;} = true;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="SyncTime"]/*'/>*/
    public static readonly TimeSpan SyncTime = TimeSpan.FromSeconds(10);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="ZeroMemoryBufferSize"]/*'/>*/
    public const Int32 ZeroMemoryBufferSize = 81920;
}