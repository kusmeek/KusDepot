namespace KusDepot.Builders;

/**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/interface[@name="ToolBuilderFactory"]/main/*'/>*/
public interface ToolBuilderFactory
{
    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/interface[@name="ToolBuilderFactory"]/method[@name="CreateBuilder"]/*'/>*/
    public static IToolBuilder CreateBuilder() => new ToolBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/interface[@name="ToolBuilderFactory"]/method[@name="CreateHostBuilder"]/*'/>*/
    public static IToolHostBuilder CreateHostBuilder() => new ToolHostBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/interface[@name="ToolBuilderFactory"]/method[@name="CreateWebHostBuilder"]/*'/>*/
    public static IToolWebHostBuilder CreateWebHostBuilder() => new ToolWebHostBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/interface[@name="ToolBuilderFactory"]/method[@name="CreateGenericHostBuilder"]/*'/>*/
    public static IToolGenericHostBuilder CreateGenericHostBuilder() => new ToolGenericHostBuilder();
}