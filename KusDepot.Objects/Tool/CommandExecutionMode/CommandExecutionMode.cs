namespace KusDepot;

/**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/main/*'/>*/
public sealed class CommandExecutionMode
{
    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowAll"]/*'/>*/
    public Boolean AllowAll() => AllowBothFreeMode();

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowBoth"]/*'/>*/
    public Boolean AllowBoth() => Configure(true,true,false);

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowBothFreeMode"]/*'/>*/
    public Boolean AllowBothFreeMode() => Configure(true,true,true);

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowAsynchronousOnly"]/*'/>*/
    public Boolean AllowAsynchronousOnly() => Configure(false,true,false);

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowAsynchronousFreeMode"]/*'/>*/
    public Boolean AllowAsynchronousFreeMode() => Configure(false,true,true);

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowSynchronousOnly"]/*'/>*/
    public Boolean AllowSynchronousOnly() => Configure(true,false,false);

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="Lock"]/*'/>*/
    public Boolean Lock() { if(Locked) { return false; } Locked = true; return true; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="Configure"]/*'/>*/
    public Boolean Configure(Boolean allowsynchronous , Boolean allowasynchronous , Boolean allowfreemode)
    {
        if(Locked || (allowfreemode && allowasynchronous is false)) { return false; }

        AllowSynchronous = allowsynchronous; AllowAsynchronous = allowasynchronous; AllowFreeMode = allowfreemode;

        return true;
    }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/field[@name="Locked"]/*'/>*/
    private Boolean Locked;

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/property[@name="AllowFreeMode"]/*'/>*/
    public Boolean AllowFreeMode { get => field; private set => field = value; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/property[@name="AllowSynchronous"]/*'/>*/
    public Boolean AllowSynchronous { get => field; private set => field = value; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/property[@name="AllowAsynchronous"]/*'/>*/
    public Boolean AllowAsynchronous { get => field; private set => field = value; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/constructor[@name="Constructor"]/*'/>*/
    public CommandExecutionMode() { AllowSynchronous = true; AllowAsynchronous = false; AllowFreeMode = false; }
}