namespace KusDepot;

/**<include file='Command.xml' path='Command/class[@name="Command"]/main/*'/>*/
[DataContract(Name = "Command" , Namespace = "KusDepot")]
public abstract class Command : Common
{
    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="Attach"]/*'/>*/
    public abstract Boolean Attach(ITool tool);

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="Detach"]/*'/>*/
    public abstract Boolean Detach();

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="Disable"]/*'/>*/
    public abstract Boolean Disable();

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="Enable"]/*'/>*/
    public abstract Boolean Enable();

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="ExecuteAsync"]/*'/>*/
    public abstract Activity? ExecuteAsync(Object[]? parameter);

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="GetSpecifications"]/*'/>*/
    public abstract Dictionary<String,String> GetSpecifications();
}