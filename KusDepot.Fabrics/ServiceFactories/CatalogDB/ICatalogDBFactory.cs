namespace KusDepot.Data;

/**<include file='ICatalogDBFactory.xml' path='ICatalogDBFactory/interface[@name="ICatalogDBFactory"]/main/*'/>*/
public interface ICatalogDBFactory
{
    /**<include file='ICatalogDBFactory.xml' path='ICatalogDBFactory/interface[@name="ICatalogDBFactory"]/method[@name="Create"]/*'/>*/
    public ICatalogDB Create(String catalogname);
}