namespace KusDepot.Serialization;

/**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/main/*'/>*/
internal sealed class SerializationData
{
    /**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/property[@name="ID"]/*'/>*/
    internal String? ID { get; }

    /**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/property[@name="TypeName"]/*'/>*/
    internal String? TypeName { get; }

    /**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/property[@name="NoExceptions"]/*'/>*/
    internal Boolean NoExceptions { get; }

    /**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/constructor[@name="Constructor"]/*'/>*/
    internal SerializationData(String? typename , String? id , Boolean noexceptions)
    {
        ID = id; TypeName = typename; NoExceptions = noexceptions;
    }

    /**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/method[@name="ForType"]/*'/>*/
    internal static SerializationData ForType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type); return new SerializationData(type.FullName,null,Settings.NoExceptions);
    }

    /**<include file='SerializationData.xml' path='SerializationData/class[@name="SerializationData"]/method[@name="ForCommon"]/*'/>*/
    internal static SerializationData ForCommon(Common instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        return new SerializationData(instance.GetType().FullName,instance.MyID?.ToString(),Settings.NoExceptions||!instance.MyExceptionsEnabled());
    }
}