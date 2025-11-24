namespace KusDepot;

/**<include file='IToolConnect.xml' path='IToolConnect/interface[@name="IToolConnect"]/main/*'/>*/
public interface IToolConnect
{
    /**<include file='IToolConnect.xml' path='IToolConnect/interface[@name="IToolConnect"]/property[@name="Tool"]/*'/>*/
    ITool? Tool { get; set; }

    /**<include file='IToolConnect.xml' path='IToolConnect/interface[@name="IToolConnect"]/property[@name="ToolHost"]/*'/>*/
    IToolHost? ToolHost { get; set; }

    /**<include file='IToolConnect.xml' path='IToolConnect/interface[@name="IToolConnect"]/property[@name="ToolWebHost"]/*'/>*/
    IToolWebHost? ToolWebHost { get; set; }

    /**<include file='IToolConnect.xml' path='IToolConnect/interface[@name="IToolConnect"]/property[@name="ToolAspireHost"]/*'/>*/
    IToolAspireHost? ToolAspireHost { get; set; }

    /**<include file='IToolConnect.xml' path='IToolConnect/interface[@name="IToolConnect"]/property[@name="ToolGenericHost"]/*'/>*/
    IToolGenericHost? ToolGenericHost { get; set; }
}