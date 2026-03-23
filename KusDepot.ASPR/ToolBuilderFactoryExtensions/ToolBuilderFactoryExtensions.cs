namespace KusDepot.Builders;

/**<include file='ToolBuilderFactoryExtensions.xml' path='ToolBuilderFactoryExtensions/class[@name="ToolBuilderFactoryExtensions"]/main/*'/>*/
public static class ToolBuilderFactoryExtensions
{
    extension(ToolBuilderFactory factory)
    {
        /**<include file='ToolBuilderFactoryExtensions.xml' path='ToolBuilderFactoryExtensions/class[@name="ToolBuilderFactoryExtensions"]/method[@name="CreateAspireHostBuilder"]/*'/>*/
        public static IToolAspireHostBuilder CreateAspireHostBuilder() => new ToolAspireHostBuilder();
    }
}