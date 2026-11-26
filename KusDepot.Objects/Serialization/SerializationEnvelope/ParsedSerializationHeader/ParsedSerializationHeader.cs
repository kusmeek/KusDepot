namespace KusDepot.Serialization;

/**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/main/*'/>*/
public readonly record struct ParsedSerializationHeader
{
    /**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/field[@name="FormatVersion"]/*'/>*/
    public Byte FormatVersion { get; init; }

    /**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/field[@name="Kind"]/*'/>*/
    public SerializationKind Kind { get; init; }

    /**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/field[@name="SerializerVersion"]/*'/>*/
    public String SerializerVersion { get; init; }

    /**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/field[@name="LibraryVersion"]/*'/>*/
    public String LibraryVersion { get; init; }

    /**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/field[@name="HeaderLength"]/*'/>*/
    public Int32 HeaderLength { get; init; }

        /**<include file='ParsedSerializationHeader.xml' path='ParsedSerializationHeader/struct[@name="ParsedSerializationHeader"]/constructor[@name="Constructor"]/*'/>*/
    public ParsedSerializationHeader(Byte formatversion , SerializationKind kind , String serializerversion , String libraryversion , Int32 headerlength)
    {
        FormatVersion = formatversion; Kind = kind;
        SerializerVersion = serializerversion;
        LibraryVersion = libraryversion;
        HeaderLength = headerlength;
    }
}