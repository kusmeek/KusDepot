namespace KusDepot;

/**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemContentBindingMode"]/main/*'/>*/
public enum DataStreamItemContentBindingMode
{
    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemContentBindingMode"]/field[@name="Static"]/*'/>*/
    Static = 0,

    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemContentBindingMode"]/field[@name="LiveSessionManaged"]/*'/>*/
    LiveSessionManaged = 1,
}

/**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemLiveContentStatus"]/main/*'/>*/
public enum DataStreamItemLiveContentStatus
{
    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemLiveContentStatus"]/field[@name="None"]/*'/>*/
    None = 0,

    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemLiveContentStatus"]/field[@name="Open"]/*'/>*/
    Open = 1,

    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemLiveContentStatus"]/field[@name="Completed"]/*'/>*/
    Completed = 2,

    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemLiveContentStatus"]/field[@name="Aborted"]/*'/>*/
    Aborted = 3,

    /**<include file='DataStreamItemEnums.xml' path='DataStreamItemEnums/enum[@name="DataStreamItemLiveContentStatus"]/field[@name="Faulted"]/*'/>*/
    Faulted = 4,
}