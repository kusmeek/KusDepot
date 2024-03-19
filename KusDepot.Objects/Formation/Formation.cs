namespace KusDepot;

/**<include file='Formation.xml' path='Formation/class[@name="Formation"]/main/*'/>*/
public abstract class Formation
{
    /**<include file='Formation.xml' path='Formation/class[@name="Formation"]/property[@name="Ark"]/*'/>*/
    private Ark? Ark {get;set;}

    /**<include file='Formation.xml' path='Formation/class[@name="Formation"]/property[@name="Authority"]/*'/>*/
    private Tool? Authority {get;set;}

    /**<include file='Formation.xml' path='Formation/class[@name="Formation"]/property[@name="Operators"]/*'/>*/
    private Dictionary<String,Tool>? Operators {get;set;}

    /**<include file='Formation.xml' path='Formation/class[@name="Formation"]/property[@name="Repository"]/*'/>*/
    private Dictionary<String,DataItem>? Repository {get;set;}

    /**<include file='Formation.xml' path='Formation/class[@name="Formation"]/property[@name="Subformations"]/*'/>*/
    private Dictionary<String,Formation>? Subformations {get;set;}

    /**<include file='Formation.xml' path='Formation/class[@name="Formation"]/method[@name="Initialize"]/*'/>*/
    public abstract Boolean Initialize();
}