namespace KusDepot;

/**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/class[@name="ToolBuilderFactory"]/main/*'/>*/
public static class ToolBuilderFactory
{
    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/class[@name="ToolBuilderFactory"]/method[@name="CreateBuilder"]/*'/>*/
    public static IToolBuilder CreateBuilder() => new ToolBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/class[@name="ToolBuilderFactory"]/method[@name="CreateHostBuilder"]/*'/>*/
    public static IToolHostBuilder CreateHostBuilder() => new ToolHostBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/class[@name="ToolBuilderFactory"]/method[@name="CreateWebHostBuilder"]/*'/>*/
    public static IToolWebHostBuilder CreateWebHostBuilder() => new ToolWebHostBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/class[@name="ToolBuilderFactory"]/method[@name="CreateAspireHostBuilder"]/*'/>*/
    public static IToolAspireHostBuilder CreateAspireHostBuilder() => new ToolAspireHostBuilder();

    /**<include file='ToolBuilderFactory.xml' path='ToolBuilderFactory/class[@name="ToolBuilderFactory"]/method[@name="CreateGenericHostBuilder"]/*'/>*/
    public static IToolGenericHostBuilder CreateGenericHostBuilder() => new ToolGenericHostBuilder();
}