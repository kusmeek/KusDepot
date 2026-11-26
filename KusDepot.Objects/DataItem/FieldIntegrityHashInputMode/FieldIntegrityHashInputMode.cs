namespace KusDepot;

/**<include file='FieldIntegrityHashInputMode.xml' path='FieldIntegrityHashInputMode/enum[@name="FieldIntegrityHashInputMode"]/main/*'/>*/
internal enum FieldIntegrityHashInputMode
{
    /**<include file='FieldIntegrityHashInputMode.xml' path='FieldIntegrityHashInputMode/enum[@name="FieldIntegrityHashInputMode"]/value[@name="HashRawData"]/*'/>*/
    HashRawData = 0,

    /**<include file='FieldIntegrityHashInputMode.xml' path='FieldIntegrityHashInputMode/enum[@name="FieldIntegrityHashInputMode"]/value[@name="StorePrecomputedDigest"]/*'/>*/
    StorePrecomputedDigest = 1,

    /**<include file='FieldIntegrityHashInputMode.xml' path='FieldIntegrityHashInputMode/enum[@name="FieldIntegrityHashInputMode"]/value[@name="ComputeContentProjection"]/*'/>*/
    ComputeContentProjection = 2,

    /**<include file='FieldIntegrityHashInputMode.xml' path='FieldIntegrityHashInputMode/enum[@name="FieldIntegrityHashInputMode"]/value[@name="Clear"]/*'/>*/
    Clear = 3
}
