namespace KusDepot.Data;

/**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/main/*'/>*/
public static class EndpointLocators
{
    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/property[@name="CatalogService"]/*'/>*/
    public static String CatalogService
    {
        get
        {
            using(FabricClient _ = new FabricClient())
            {
                return _.PropertyManager.GetPropertyAsync(ServiceLocators.CatalogService,"EndpointLocator").Result.GetValue<String>();
            }
        }
    }

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/property[@name="DataControlService"]/*'/>*/
    public static String DataControlService
    {
        get
        {
            using(FabricClient _ = new FabricClient())
            {
                return _.PropertyManager.GetPropertyAsync(ServiceLocators.DataControlService,"EndpointLocator").Result.GetValue<String>();
            }
        }
    }

    /**<include file='EndpointLocators.xml' path='EndpointLocators/class[@name="EndpointLocators"]/property[@name="ToolFabricService"]/*'/>*/
    public static String ToolFabricService
    {
        get
        {
            using(FabricClient _ = new FabricClient())
            {
                return _.PropertyManager.GetPropertyAsync(ServiceLocators.ToolFabricService,"EndpointLocator").Result.GetValue<String>();
            }
        }
    }
}