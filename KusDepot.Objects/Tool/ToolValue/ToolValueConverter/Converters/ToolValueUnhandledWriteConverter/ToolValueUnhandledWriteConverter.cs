namespace KusDepot;

/**<include file='ToolValueUnhandledWriteConverter.xml' path='ToolValueUnhandledWriteConverter/class[@name="ToolValueUnhandledWriteConverter"]/main/*'/>*/
public sealed class ToolValueUnhandledWriteConverter : IToolValueConverter
{
    ///<inheritdoc/>
    public Boolean TryRead(ToolValue? value , [MaybeNullWhen(false)] out Object? result)
    {
        result = null; return false;
    }

    ///<inheritdoc/>
    public Boolean TryWrite(Object? value , [MaybeNullWhen(false)] out ToolValue? result)
    {
        result = null;

        try
        {
            if(value is null) { return false; }

            Type runtimeType = value.GetType(); String? type = runtimeType.FullName; String? data = value.ToString();

            if(String.IsNullOrWhiteSpace(type) || data is null) { return false; }

            ToolValueMode mode = ObjectParser.CanParse(runtimeType) ? ToolValueMode.Parse : ToolValueMode.Unhandled;

            result = new ToolValue(){ Mode = mode , Type = type , Data = data };

            return true;
        }
        catch { result = null; return false; }
    }
}