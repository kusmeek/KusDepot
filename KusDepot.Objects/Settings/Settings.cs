namespace KusDepot;

/**<include file='Settings.xml' path='Settings/class[@name="Settings"]/main/*'/>*/
public static class Settings
{
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

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="ExecuteCommandTimeout"]/*'/>*/
    public static TimeSpan ExecuteCommandTimeout {get;set;} = InfiniteTimeSpan;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="GetOutputTimeout"]/*'/>*/
    public static TimeSpan GetOutputTimeout {get;set;} = TimeSpan.FromMinutes(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="HostStopTimeout"]/*'/>*/
    public static TimeSpan HostStopTimeout {get;set;} = TimeSpan.FromMinutes(1);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="NoExceptions"]/*'/>*/
    public static Boolean NoExceptions {get;set;} = true;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="SyncTime"]/*'/>*/
    public static TimeSpan SyncTime {get;set;} = TimeSpan.FromSeconds(10);

}