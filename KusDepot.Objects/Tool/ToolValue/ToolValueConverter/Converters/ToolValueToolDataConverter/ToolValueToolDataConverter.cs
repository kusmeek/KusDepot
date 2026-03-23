namespace KusDepot;

/**<include file='ToolValueToolDataConverter.xml' path='ToolValueToolDataConverter/class[@name="ToolValueToolDataConverter"]/main/*'/>*/
public sealed class ToolValueToolDataConverter : IToolValueConverter
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
            if(value is not ToolData data) { return false; }

            Type t = data.GetType(); if(!IsSupportedType(t)) { return false; }

            String json = JsonUtility.ToJsonString(data,t); if(String.IsNullOrWhiteSpace(json)) { return false; }

            result = new ToolValue()
            {
                Mode = ToolValueMode.Custom,

                Type = t.FullName ?? t.Name,

                Data = json
            };

            return true;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueToolDataConverter.xml' path='ToolValueToolDataConverter/class[@name="ToolValueToolDataConverter"]/method[@name="IsSupportedType"]/*'/>*/
    private static Boolean IsSupportedType(Type type)
    {
        try
        {
            return typeof(ToolData).IsAssignableFrom(type) && type.IsAbstract is false && type.IsInterface is false && type.ContainsGenericParameters is false;
        }
        catch { return false; }
    }
}
