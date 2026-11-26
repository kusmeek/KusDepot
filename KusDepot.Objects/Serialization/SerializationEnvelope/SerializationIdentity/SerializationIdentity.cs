namespace KusDepot.Serialization;

/**<include file='SerializationIdentity.xml' path='SerializationIdentity/struct[@name="SerializationIdentity"]/main/*'/>*/
public readonly record struct SerializationIdentity
{
    /**<include file='SerializationIdentity.xml' path='SerializationIdentity/struct[@name="SerializationIdentity"]/field[@name="Kind"]/*'/>*/
    public SerializationKind Kind { get; init; }

    /**<include file='SerializationIdentity.xml' path='SerializationIdentity/struct[@name="SerializationIdentity"]/field[@name="SerializerVersion"]/*'/>*/
    public Byte[] SerializerVersion { get; init; }

    /**<include file='SerializationIdentity.xml' path='SerializationIdentity/struct[@name="SerializationIdentity"]/field[@name="LibraryVersion"]/*'/>*/
    public Byte[] LibraryVersion { get; init; }

    /**<include file='SerializationIdentity.xml' path='SerializationIdentity/struct[@name="SerializationIdentity"]/constructor[@name="Constructor"]/*'/>*/
    public SerializationIdentity(SerializationKind kind , Byte[] serializerversion , Byte[] libraryversion)
    {
        Kind = kind; SerializerVersion = serializerversion; LibraryVersion = libraryversion;
    }
}