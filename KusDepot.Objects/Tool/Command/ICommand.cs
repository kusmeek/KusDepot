namespace KusDepot;

/**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/main/*'/>*/
public interface ICommand : ICommon , IComparable<ICommand> , IEquatable<ICommand>
{
    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/property[@name="Enabled"]/*'/>*/
    public Boolean Enabled { get; }

    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/method[@name="Detach"]/*'/>*/
    Boolean Detach(AccessKey? key = null);

    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/method[@name="Attach"]/*'/>*/
    Boolean Attach(ITool tool , AccessKey key);

    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/method[@name="Disable"]/*'/>*/
    Task<Boolean> Disable(CancellationToken cancel = default , AccessKey? key = null);

    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/method[@name="Enable"]/*'/>*/
    Task<Boolean> Enable(CancellationToken cancel = default , AccessKey? key = null);

    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/method[@name="ExecuteAsync"]/*'/>*/
    Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null);

    /**<include file='ICommand.xml' path='ICommand/interface[@name="ICommand"]/method[@name="Execute"]/*'/>*/
    Guid? Execute(Activity? activity = null , AccessKey? key = null);
}