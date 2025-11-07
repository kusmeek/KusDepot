namespace KusDepot.Data;

/**<include file='CatalogDBFactory.xml' path='CatalogDBFactory/class[@name="CatalogDBFactory"]/main/*'/>*/
public class CatalogDBFactory : ICatalogDBFactory
{
    /**<include file='CatalogDBFactory.xml' path='CatalogDBFactory/class[@name="CatalogDBFactory"]/method[@name="Create"]/*'/>*/
    public ICatalogDB Create(String catalogname)
    {
        return ActorProxy.Create<ICatalogDB>(new ActorId(catalogname),ServiceLocators.CatalogDBService);
    }
}