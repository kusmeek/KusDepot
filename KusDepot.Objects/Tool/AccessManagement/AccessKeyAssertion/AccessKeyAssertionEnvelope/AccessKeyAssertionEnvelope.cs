namespace KusDepot.Security.Assertions;

/**<include file='AccessKeyAssertionEnvelope.xml' path='AccessKeyAssertionEnvelope/struct[@name="AccessKeyAssertionEnvelope"]/main/*'/>*/
internal readonly struct AccessKeyAssertionEnvelope
{
    /**<include file='AccessKeyAssertionEnvelope.xml' path='AccessKeyAssertionEnvelope/struct[@name="AccessKeyAssertionEnvelope"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeyAssertionEnvelope(Byte version , String serializationidentifier , ReadOnlyMemory<Byte> payload)
    {
        Version = version;
        SerializationIdentifier = serializationidentifier ?? String.Empty;
        Payload = payload;
    }

    /**<include file='AccessKeyAssertionEnvelope.xml' path='AccessKeyAssertionEnvelope/struct[@name="AccessKeyAssertionEnvelope"]/property[@name="Version"]/*'/>*/
    public Byte Version { get; }

    /**<include file='AccessKeyAssertionEnvelope.xml' path='AccessKeyAssertionEnvelope/struct[@name="AccessKeyAssertionEnvelope"]/property[@name="SerializationIdentifier"]/*'/>*/
    public String SerializationIdentifier { get; }

    /**<include file='AccessKeyAssertionEnvelope.xml' path='AccessKeyAssertionEnvelope/struct[@name="AccessKeyAssertionEnvelope"]/property[@name="Payload"]/*'/>*/
    public ReadOnlyMemory<Byte> Payload { get; }
}