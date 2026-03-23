namespace KusDepot;

/**<include file='ToolValueGenericDataContractConverter.xml' path='ToolValueGenericDataContractConverter/class[@name="ToolValueGenericDataContractConverter"]/main/*'/>*/
public sealed class ToolValueGenericDataContractConverter : IToolValueConverter
{
    ///<inheritdoc/>
    public Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result)
    {
        result = null;

        try
        {
            if(value is null || value.Mode != ToolValueMode.Custom || String.IsNullOrWhiteSpace(value.Type) || value.Data is null) { return false; }

            Type? t = TypeProvider.Create(value.Type).Resolve(); if(t is null || !IsSupportedType(t)) { return false; }

            result = DataContractUtility.ParseBase64(value.Data,t,SerializationData.ForType(t));

            return result is not null && t.IsInstanceOfType(result);
        }
        catch { result = null; return false; }
    }

    ///<inheritdoc/>
    public Boolean TryWrite(Object? value , [MaybeNullWhen(false)] out ToolValue? result)
    {
        result = null;

        try
        {
            if(value is null) { return false; }

            Type t = value.GetType(); if(!IsSupportedType(t)) { return false; }

            String data = DataContractUtility.ToBase64String(value,SerializationData.ForType(t)); if(String.IsNullOrWhiteSpace(data)) { return false; }

            result = new ToolValue()
            {
                Mode = ToolValueMode.Custom,

                Type = t.FullName ?? t.Name,

                Data = data
            };

            return true;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueGenericDataContractConverter.xml' path='ToolValueGenericDataContractConverter/class[@name="ToolValueGenericDataContractConverter"]/method[@name="IsSupportedType"]/*'/>*/
    private static Boolean IsSupportedType(Type type)
    {
        try
        {
            Type[] unsupported = [ typeof(ToolValue) , typeof(ToolValueArgument) , typeof(ToolValueArgumentParameter) ];

            if(type.IsAbstract || type.IsInterface || type.ContainsGenericParameters) { return false; }

            if(typeof(ToolData).IsAssignableFrom(type)) { return false; }

            return type.GetCustomAttribute<DataContractAttribute>(false) is not null;
        }
        catch { return false; }
    }
}