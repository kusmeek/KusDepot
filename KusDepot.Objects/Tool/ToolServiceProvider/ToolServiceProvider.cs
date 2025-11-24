namespace KusDepot;

/**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/main/*'/>*/
public sealed class ToolServiceProvider
{
    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/property[@name="ServiceProvider"]/*'/>*/
    public IServiceProvider ServiceProvider { get; }

    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/constructor[@name="Constructor"]/*'/>*/
    public ToolServiceProvider(IServiceProvider serviceprovider) { ServiceProvider = serviceprovider; }
}