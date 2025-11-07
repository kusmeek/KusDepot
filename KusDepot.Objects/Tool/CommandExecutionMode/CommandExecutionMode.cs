namespace KusDepot;

/**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/main/*'/>*/
public sealed class CommandExecutionMode
{
    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/property[@name="AllowAsynchronous"]/*'/>*/
    public Boolean AllowAsynchronous { get => field; private set => field = value; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/property[@name="AllowSynchronous"]/*'/>*/
    public Boolean AllowSynchronous { get => field; private set => field = value; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/constructor[@name="Constructor"]/*'/>*/
    public CommandExecutionMode() { AllowSynchronous = true; AllowAsynchronous = false; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowBoth"]/*'/>*/
    public Boolean AllowBoth() { if(Locked) { return false; } AllowSynchronous = true; AllowAsynchronous = true; return true; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowAsynchronousOnly"]/*'/>*/
    public Boolean AllowAsynchronousOnly() { if(Locked) { return false; } AllowSynchronous = false; AllowAsynchronous = true; return true; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="AllowSynchronousOnly"]/*'/>*/
    public Boolean AllowSynchronousOnly() { if(Locked) { return false; } AllowSynchronous = true; AllowAsynchronous = false; return true; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/method[@name="Lock"]/*'/>*/
    public Boolean Lock() { if(Locked) { return false; } Locked = true; return true; }

    /**<include file='CommandExecutionMode.xml' path='CommandExecutionMode/class[@name="CommandExecutionMode"]/field[@name="Locked"]/*'/>*/
    private Boolean Locked;
}