namespace KusDepot;

/**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/main/*'/>*/
public enum LifeCycleState
{
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="Active"]/*'/>*/
    Active = 1,
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="InActive"]/*'/>*/
    InActive,
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="Initialized"]/*'/>*/
    Initialized,
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="Starting"]/*'/>*/
    Starting,
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="Stopping"]/*'/>*/
    Stopping,
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="Error"]/*'/>*/
    Error,
    /**<include file='LifeCycleState.xml' path='LifeCycleState/enum[@name="LifeCycleState"]/value[@name="UnInitialized"]/*'/>*/
    UnInitialized = 0
}