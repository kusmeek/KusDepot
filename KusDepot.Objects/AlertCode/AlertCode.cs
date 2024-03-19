namespace KusDepot;

/**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/main/*'/>*/
public enum AlertCode
{
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="ActivityAdded"]/*'/>*/
    ActivityAdded = 1,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="ActivityRemoved"]/*'/>*/
    ActivityRemoved,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="CommandRegistered"]/*'/>*/
    CommandRegistered,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="CommandUnregistered"]/*'/>*/
    CommandUnregistered,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="ContainerChanged"]/*'/>*/
    ContainerChanged,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="DataItemAdded"]/*'/>*/
    DataItemAdded,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="DataItemRemoved"]/*'/>*/
    DataItemRemoved,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="DeserializationComplete"]/*'/>*/
    DeserializationComplete,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="Disposing"]/*'/>*/
    Disposing,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="FieldChanged"]/*'/>*/
    FieldChanged,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="Initialized"]/*'/>*/
    Initialized,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="InputAdded"]/*'/>*/
    InputAdded,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="InputRemoved"]/*'/>*/
    InputRemoved,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="InputUpdated"]/*'/>*/
    InputUpdated,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="Locked"]/*'/>*/
    Locked,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="OutputAdded"]/*'/>*/
    OutputAdded,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="OutputRemoved"]/*'/>*/
    OutputRemoved,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="StatusUpdated"]/*'/>*/
    StatusUpdated,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="SerializationStarted"]/*'/>*/
    SerializationStarted,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="SerializationComplete"]/*'/>*/
    SerializationComplete,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="UnLocked"]/*'/>*/
    UnLocked,
    /**<include file='AlertCode.xml' path='AlertCode/enum[@name="AlertCode"]/value[@name="Unused"]/*'/>*/
    Unused = 0,
}