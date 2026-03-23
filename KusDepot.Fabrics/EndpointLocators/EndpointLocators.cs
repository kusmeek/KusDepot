namespace KusDepot.Data;

/**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/main/*'/>*/
public static class EndpointLocators
{
    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/property[@name="CatalogService"]/*'/>*/
    public static String CatalogService => GetCatalogServiceAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/property[@name="DataControlService"]/*'/>*/
    public static String DataControlService => GetDataControlServiceAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/property[@name="ToolFabricService"]/*'/>*/
    public static String ToolFabricService => GetToolFabricServiceAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/method[@name="GetCatalogServiceAsync"]/*'/>*/
    public static async Task<String> GetCatalogServiceAsync(CancellationToken cancel = default)
    {
        using(FabricClient _ = new FabricClient())
        {
            var __ = await _.PropertyManager.GetPropertyAsync(ServiceLocators.CatalogService,"EndpointLocator",TimeSpan.FromSeconds(5),cancel).ConfigureAwait(false); return __.GetValue<String>();
        }
    }

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/method[@name="GetDataControlServiceAsync"]/*'/>*/
    public static async Task<String> GetDataControlServiceAsync(CancellationToken cancel = default)
    {
        using(FabricClient _ = new FabricClient())
        {
            var __ = await _.PropertyManager.GetPropertyAsync(ServiceLocators.DataControlService,"EndpointLocator",TimeSpan.FromSeconds(5),cancel).ConfigureAwait(false); return __.GetValue<String>();
        }
    }

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/method[@name="GetToolFabricServiceAsync"]/*'/>*/
    public static async Task<String> GetToolFabricServiceAsync(CancellationToken cancel = default)
    {
        using(FabricClient _ = new FabricClient())
        {
            var __ = await _.PropertyManager.GetPropertyAsync(ServiceLocators.ToolFabricService,"EndpointLocator",TimeSpan.FromSeconds(5),cancel).ConfigureAwait(false); return __.GetValue<String>();
        }
    }
}