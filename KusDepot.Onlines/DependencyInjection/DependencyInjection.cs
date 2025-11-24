namespace KusDepot;

/**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/main/*'/>*/
public static class DependencyInjection
{
    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddKusDepotToolInstance"]/*'/>*/
    public static IServiceCollection AddKusDepotTool(this IServiceCollection services , ITool tool)
    {
        return services.AddSingletonWithInterfaces(tool);
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddKusDepotToolFactory"]/*'/>*/
    public static IServiceCollection AddKusDepotTool(this IServiceCollection services , Func<ITool> toolfactory)
    {
        return services.AddSingletonWithInterfaces(toolfactory());
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddScopedWithInterfaces_T"]/*'/>*/
    public static IServiceCollection AddScopedWithInterfaces<T>(this IServiceCollection services) where T : class
    {
        return services.AddWithInterfaces<T>(ServiceLifetime.Scoped);
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddSingletonWithInterfaces_T"]/*'/>*/
    public static IServiceCollection AddSingletonWithInterfaces<T>(this IServiceCollection services) where T : class
    {
        return services.AddWithInterfaces<T>(ServiceLifetime.Singleton);
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddTransientWithInterfaces_T"]/*'/>*/
    public static IServiceCollection AddTransientWithInterfaces<T>(this IServiceCollection services) where T : class
    {
        return services.AddWithInterfaces<T>(ServiceLifetime.Transient);
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddSingletonWithInterfaces_T_instance"]/*'/>*/
    public static IServiceCollection AddSingletonWithInterfaces<T>(this IServiceCollection services , T instance) where T : class
    {
        services.AddSingleton(instance);

        foreach(var _ in typeof(T).GetInterfaces()) { services.AddSingleton(_,provider => instance); }

        return services;
    }

    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddWithInterfaces_T"]/*'/>*/
    public static IServiceCollection AddWithInterfaces<T>(this IServiceCollection services , ServiceLifetime lifetime) where T : class
    {
        services.Add(new ServiceDescriptor(typeof(T),typeof(T),lifetime));

        foreach(var _ in typeof(T).GetInterfaces())
        {
            services.Add(new ServiceDescriptor(_,provider => provider.GetRequiredService<T>(),lifetime));
        }

        return services;
    }
}