namespace KusDepot;

/**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/main/*'/>*/
public enum ActivityState
{
    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="None"]/*'/>*/
    None = 0,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="Success"]/*'/>*/
    Success = 1,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="Completed"]/*'/>*/
    Completed = 2,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="CompletedFreeMode"]/*'/>*/
    CompletedFreeMode = 3,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="Failed"]/*'/>*/
    Failed = 4,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="Running"]/*'/>*/
    Running = 5,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="RunningFreeMode"]/*'/>*/
    RunningFreeMode = 6,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="Canceled"]/*'/>*/
    Canceled = 7,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="CanceledRunning"]/*'/>*/
    CanceledRunning = 8,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="TimedOut"]/*'/>*/
    TimedOut = 9,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="TimedOutRunning"]/*'/>*/
    TimedOutRunning = 10,

    /**<include file='ActivityState.xml' path='ActivityState/enum[@name="ActivityState"]/field[@name="Faulted"]/*'/>*/
    Faulted = 11
}