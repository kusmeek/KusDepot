namespace KusDepot;

/**<include file='ToolValueConfigurationConverter.xml' path='ToolValueConfigurationConverter/class[@name="ToolValueConfigurationConverter"]/main/*'/>*/
public sealed class ToolValueConfigurationConverter : IToolValueConverter
{
    /**<include file='ToolValueConfigurationConverter.xml' path='ToolValueConfigurationConverter/class[@name="ToolValueConfigurationConverter"]/method[@name="TryRead"]/*'/>*/
    public Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result)
    {
        result = null;

        try
        {
            if(value is null || value.Mode != ToolValueMode.Custom || value.Data is null || String.IsNullOrWhiteSpace(value.Type)) { return false; }

            if(String.Equals(value.Type,typeof(IConfiguration).FullName,Ordinal) is false) { return false; }

            result = ToolConfiguration.Deserialize(value.Data);

            return result is IConfiguration;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueConfigurationConverter.xml' path='ToolValueConfigurationConverter/class[@name="ToolValueConfigurationConverter"]/method[@name="TryWrite"]/*'/>*/
    public Boolean TryWrite(Object? value , [MaybeNullWhen(false)] out ToolValue? result)
    {
        result = null;

        try
        {
            if(value is not IConfiguration config) { return false; }

            String? data = ToolConfiguration.Serialize(config); if(data is null) { return false; }

            result = new ToolValue()
            {
                Mode = ToolValueMode.Custom,

                Type = typeof(IConfiguration).FullName,

                Data = data
            };

            return true;
        }
        catch { result = null; return false; }
    }
}