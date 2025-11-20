namespace KusDepot.Data;

/**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/main/*'/>*/
public static class ServiceLocators
{
    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="BlobService"]/*'/>*/
    public static Uri BlobService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/BlobActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="CatalogService"]/*'/>*/
    public static Uri CatalogService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/Catalog");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="CatalogDBService"]/*'/>*/
    public static Uri CatalogDBService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/CatalogDBActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="CoreCacheService"]/*'/>*/
    public static Uri CoreCacheService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/CoreCacheActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="DataConfigsService"]/*'/>*/
    public static Uri DataConfigsService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/DataConfigsActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="DataControlService"]/*'/>*/
    public static Uri DataControlService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/DataControl");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="ManagementService"]/*'/>*/
    public static Uri ManagementService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/ManagementActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="SecureService"]/*'/>*/
    public static Uri SecureService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Data/SecureActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="TempDBService"]/*'/>*/
    public static Uri TempDBService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Act/TempDBActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="ToolActorService"]/*'/>*/
    public static Uri ToolActorService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Act/ToolActorService");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="ToolFabricService"]/*'/>*/
    public static Uri ToolFabricService
    {
        get
        {
            return new Uri("fabric:/KusDepot.ToolServiceFabric/ToolFabric");
        }
    }

    /**<include file='ServiceLocators.xml' path='ServiceLocators/class[@name="ServiceLocators"]/property[@name="WatchService"]/*'/>*/
    public static Uri WatchService
    {
        get
        {
            return new Uri("fabric:/KusDepot.Act/WatchActorService");
        }
    }
}