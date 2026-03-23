namespace KusDepot.AI;

public sealed partial class ToolPod
{
    private static readonly HashSet<Type> InlineTypes = new()
    {
        typeof(Boolean),
        typeof(Byte),
        typeof(SByte),
        typeof(Int16),
        typeof(UInt16),
        typeof(Int32),
        typeof(UInt32),
        typeof(Int64),
        typeof(UInt64),
        typeof(Single),
        typeof(Double),
        typeof(Decimal),
        typeof(String),
        typeof(Guid),
        typeof(DateTime),
        typeof(DateTimeOffset)
    };

    public static ToolPodResult ErrorResult(String? error)
    {
        return new()
        {
            Success = false,
            Kind = ToolPodResultKind.Error,
            Error = error,
            Data = null,
            Type = null,
            Reference = null,
            ToolValue = null
        };
    }

    public static ToolPodResult ReferenceResult(ToolPodRef? reference)
    {
        return reference is null ? ErrorResult(ErrorStrings.ReferenceCreationFailed) :
        new()
        {
            Success = true,
            Kind = ToolPodResultKind.Reference,
            Error = null,
            Data = null,
            Type = reference.Type,
            Reference = reference,
            ToolValue = null
        };
    }

    public static ToolPodResult ToolValueResult(ToolValue? toolvalue)
    {
        return toolvalue is null ? ErrorResult(ErrorStrings.ToolValueCreationFailed) :
        new()
        {
            Success = true,
            Kind = ToolPodResultKind.ToolValue,
            Error = null,
            Data = null,
            Type = toolvalue.Type,
            Reference = null,
            ToolValue = toolvalue
        };
    }

    public static ToolPodResult ValueResult(String? data , String? type)
    {
        return new()
        {
            Success = true,
            Kind = ToolPodResultKind.Value,
            Error = null,
            Data = data,
            Type = type,
            Reference = null,
            ToolValue = null
        };
    }

    public static ToolPodResult VoidResult()
    {
        return new()
        {
            Success = true,
            Kind = ToolPodResultKind.Void,
            Error = null,
            Data = null,
            Type = null,
            Reference = null,
            ToolValue = null
        };
    }

    public static ToolPodResult NormalizeResult(Object? value , String? alias = null)
    {
        try
        {
            if(value is null) { return ValueResult("null",null); }

            if(value is ToolValue toolvaluein) { return ToolValueResult(toolvaluein); }

            Type t = value.GetType(); Type? u = Nullable.GetUnderlyingType(t); if(u is not null) { t = u; }

            if(t.IsEnum || InlineTypes.Contains(t) || value is Descriptor || value is CommandDescriptor || value is ToolDescriptor)
            {
                return ValueResult(JsonSerializer.Serialize(value),t.FullName ?? t.Name);
            }

            if(String.IsNullOrWhiteSpace(alias))
            {
                ToolValue? toolvalue = ToolValueConverter.ToToolValue(value);

                if(toolvalue is not null && toolvalue.Mode != ToolValueMode.Unhandled)
                {
                    return ToolValueResult(toolvalue);
                }
            }

            return ReferenceResult(RegisterObject(value,alias));
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.ResultNormalizationFailed); return ErrorResult(_.Message); }
    }
}