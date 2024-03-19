namespace KusDepot.Data;

/**<include file='ArkKeeperFactory.xml' path='ArkKeeperFactory/class[@name="ArkKeeperFactory"]/main/*'/>*/
public class ArkKeeperFactory : IArkKeeperFactory
{
    /**<include file='ArkKeeperFactory.xml' path='ArkKeeperFactory/class[@name="ArkKeeperFactory"]/method[@name="Create"]/*'/>*/
    public IArkKeeper Create(String catalogname)
    {
        return ActorProxy.Create<IArkKeeper>(new ActorId(catalogname),ServiceLocators.ArkKeeperService);
    }
}