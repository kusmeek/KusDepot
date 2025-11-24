namespace KusDepot.ToolActor;

/**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/main/*'/>*/
public interface IToolActor : IActor
{
    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="ExecuteCommand"]/*'/>*/
    Task<Guid?> ExecuteCommand(CommandDetails? details , AccessKey? key);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="ExecuteCommandCab"]/*'/>*/
    Task<Guid?> ExecuteCommandCab(KusDepotCab? cab , AccessKey? key);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetOutput"]/*'/>*/
    Task<Object?> GetOutput(Guid? id , AccessKey? key);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RequestAccess"]/*'/>*/
    Task<AccessKey?> RequestAccess(AccessRequest? request);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RevokeAccess"]/*'/>*/
    Task<Boolean> RevokeAccess(AccessKey? key);
}