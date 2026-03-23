namespace KusDepot;

/**<include file='ToolValueParseConverter.xml' path='ToolValueParseConverter/class[@name="ToolValueParseConverter"]/main/*'/>*/
public sealed class ToolValueParseConverter : IToolValueConverter
{
    ///<inheritdoc/>
    public Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result)
    {
        result = null;

        try
        {
            if(value is null || value.Mode != ToolValueMode.Parse || String.IsNullOrWhiteSpace(value.Type) || value.Data is null) { return false; }

            if(value.Arguments is not null && value.Arguments.Length > 0) { return false; }

            var parser = ObjectParser.Create(value.Type).WithData(value.Data);

            result = parser.Parse();

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

            if(typeof(ToolData).IsAssignableFrom(value.GetType())) { return false; }

            String? type = value.GetType().FullName; String? data = value.ToString();

            if(String.IsNullOrWhiteSpace(type) || data is null) { return false; }

            var parser = ObjectParser.Create(type).WithData(data);

            if(parser.TryParse() is false) { return false; }

            result = new ToolValue(){ Mode = ToolValueMode.Parse , Type = type , Data = data };

            return true;
        }
        catch { result = null; return false; }
    }
}