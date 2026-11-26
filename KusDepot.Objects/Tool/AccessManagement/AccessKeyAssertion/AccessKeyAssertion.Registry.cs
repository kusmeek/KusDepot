namespace KusDepot.Security.Assertions;

public abstract partial record class AccessKeyAssertion
{
    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="RegisterDeserializer"]/*'/>*/
    public static Boolean RegisterDeserializer(String serializationidentifier , Func<ReadOnlyMemory<Byte>,AccessKeyAssertion?>? deserializer)
    {
        if(deserializer is null || String.IsNullOrWhiteSpace(serializationidentifier)) { return false; }

        String normalizedidentifier = serializationidentifier.Trim();

        lock(Deserializers)
        {
            return Deserializers.TryAdd(normalizedidentifier,deserializer);
        }
    }
}