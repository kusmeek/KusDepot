namespace KusDepot;

/**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/main/*'/>*/
public static class DependencyInjection
{
    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddKusDepotToolInstance"]/*'/>*/
    public static IServiceCollection AddKusDepotTool(this IServiceCollection services , ITool tool)
    {
        return services.AddSingleton<IHostedService>(tool);
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddKusDepotToolFactory"]/*'/>*/
    public static IServiceCollection AddKusDepotTool(this IServiceCollection services , Func<ITool> toolfactory)
    {
        return services.AddSingleton<IHostedService>(toolfactory());
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddKusDepotToolInstanceAutofac"]/*'/>*/
    public static ContainerBuilder AddKusDepotTool(this ContainerBuilder containerbuilder , ITool tool)
    {
        containerbuilder.RegisterInstance<IHostedService>(tool); return containerbuilder;
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddKusDepotToolFactoryAutofac"]/*'/>*/
    public static ContainerBuilder AddKusDepotTool(this ContainerBuilder containerbuilder , Func<ITool> toolfactory)
    {
        containerbuilder.RegisterInstance<IHostedService>(toolfactory()); return containerbuilder;
    }
}