namespace KusDepot.Serialization;

/**<include file='SerializationKind.xml' path='SerializationKind/enum[@name="SerializationKind"]/main/*'/>*/
public enum SerializationKind : Byte
{
    /**<include file='SerializationKind.xml' path='SerializationKind/enum[@name="SerializationKind"]/field[@name="Unknown"]/*'/>*/
    Unknown = 0x00,

    /**<include file='SerializationKind.xml' path='SerializationKind/enum[@name="SerializationKind"]/field[@name="Orleans"]/*'/>*/
    Orleans = 0x01
}