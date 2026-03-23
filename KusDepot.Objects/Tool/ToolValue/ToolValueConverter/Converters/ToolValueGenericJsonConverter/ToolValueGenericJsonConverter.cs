namespace KusDepot;

/**<include file='ToolValueGenericJsonConverter.xml' path='ToolValueGenericJsonConverter/class[@name="ToolValueGenericJsonConverter"]/main/*'/>*/
public sealed class ToolValueGenericJsonConverter : IToolValueConverter
{
    ///<inheritdoc/>
    public Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result)
    {
        result = null;

        try
        {
            if(value is null || value.Mode != ToolValueMode.Custom || String.IsNullOrWhiteSpace(value.Type) || value.Data is null) { return false; }

            Type? t = TypeProvider.Create(value.Type).Resolve(); if(t is null || !IsSupportedType(t)) { return false; }

            result = JsonUtility.Parse(value.Data,t);

            return result is not null;
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

            String data = JsonUtility.ToJsonString(value,t); if(String.IsNullOrWhiteSpace(data)) { return false; }

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

    /**<include file='ToolValueGenericJsonConverter.xml' path='ToolValueGenericJsonConverter/class[@name="ToolValueGenericJsonConverter"]/method[@name="IsSupportedType"]/*'/>*/
    private static Boolean IsSupportedType(Type type)
    {
        try
        {
            Type[] unsupported = [ typeof(Object) , typeof(ToolValue) , typeof(ToolValueArgument) , typeof(ToolValueArgumentParameter) ];

            if(type.IsAbstract || type.IsInterface || type.ContainsGenericParameters) { return false; }

            if(typeof(ToolData).IsAssignableFrom(type)) { return false; }

            return unsupported.Contains(type) is false;
        }
        catch { return false; }
    }
}