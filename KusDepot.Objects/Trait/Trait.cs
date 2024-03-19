namespace KusDepot;

/**<include file='Trait.xml' path='Trait/class[@name="Trait"]/main/*'/>*/
[DataContract(Name = "Trait" , Namespace = "KusDepot")]
public abstract class Trait : Common
{
    /**<include file='Trait.xml' path='Trait/class[@name="Trait"]/method[@name="Activate"]/*'/>*/
    public abstract Task<Boolean> Activate(CancellationToken? token = null);

    /**<include file='Trait.xml' path='Trait/class[@name="Trait"]/method[@name="Bind"]/*'/>*/
    public abstract Task<Boolean> Bind(Tool tool);

    /**<include file='Trait.xml' path='Trait/class[@name="Trait"]/method[@name="Deactivate"]/*'/>*/
    public abstract Task<Boolean> Deactivate(CancellationToken? token = null);
}