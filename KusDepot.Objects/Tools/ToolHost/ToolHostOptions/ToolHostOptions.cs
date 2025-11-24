namespace KusDepot;

/**<include file='ToolHostOptions.xml' path='ToolHostOptions/class[@name="ToolHostOptions"]/main/*'/>*/
public class ToolHostOptions
{
    /**<include file='ToolHostOptions.xml' path='ToolHostOptions/class[@name="ToolHostOptions"]/property[@name="ServicesStartConcurrently"]/*'/>*/
    public Boolean ServicesStartConcurrently {get;set;} = true;

    /**<include file='ToolHostOptions.xml' path='ToolHostOptions/class[@name="ToolHostOptions"]/property[@name="ServicesStopConcurrently"]/*'/>*/
    public Boolean ServicesStopConcurrently  {get;set;} = true;

    /**<include file='ToolHostOptions.xml' path='ToolHostOptions/class[@name="ToolHostOptions"]/property[@name="StartupTimeout"]/*'/>*/
    public TimeSpan StartupTimeout           {get;set;} = Timeout.InfiniteTimeSpan;

    /**<include file='ToolHostOptions.xml' path='ToolHostOptions/class[@name="ToolHostOptions"]/property[@name="ShutdownTimeout"]/*'/>*/
    public TimeSpan ShutdownTimeout          {get;set;} = TimeSpan.FromSeconds(120);
}