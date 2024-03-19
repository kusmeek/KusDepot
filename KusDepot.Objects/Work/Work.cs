namespace KusDepot;

/**<include file='Work.xml' path='Work/class[@name="Work"]/main/*'/>*/
public abstract class Work
{
    /**<include file='Work.xml' path='Work/class[@name="Work"]/property[@name="Ark"]/*'/>*/
    private Ark? Ark {get;set;}

    /**<include file='Work.xml' path='Work/class[@name="Work"]/property[@name="Assets"]/*'/>*/
    private Dictionary<String,DataItem>? Assets {get;set;}

    /**<include file='Work.xml' path='Work/class[@name="Work"]/property[@name="Formations"]/*'/>*/
    private Dictionary<String,Formation>? Formations {get;set;}

    /**<include file='Work.xml' path='Work/class[@name="Work"]/property[@name="Operators"]/*'/>*/
    private Dictionary<String,Tool>? Operators {get;set;}

    /**<include file='Work.xml' path='Work/class[@name="Work"]/method[@name="Initialize"]/*'/>*/
    public abstract Boolean Initialize();
}