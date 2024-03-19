namespace KusDepot.Data;

/**<include file='IArkKeeperFactory.xml' path='IArkKeeperFactory/interface[@name="IArkKeeperFactory"]/main/*'/>*/
public interface IArkKeeperFactory
{
    /**<include file='IArkKeeperFactory.xml' path='IArkKeeperFactory/interface[@name="IArkKeeperFactory"]/method[@name="Create"]/*'/>*/
    public IArkKeeper Create(String catalogname);
}