namespace KusDepot.AI;

public sealed partial class ToolPod
{
    public static Object? BindArgument(ToolPodArgument? argument)
    {
        try
        {
            if(argument is null) { return null; }

            return argument.Kind switch
            {
                ToolPodArgumentKind.Reference => BindReference(argument.RefId),

                ToolPodArgumentKind.Value     => BindValue(argument.Data,argument.Type),
                _ => null
            };
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.ArgumentBindingFailed); return null; }
    }

    public static Object?[] BindArguments(ToolPodArgument[]? arguments)
    {
        try
        {
            if(arguments is null || arguments.Length == 0) { return Array.Empty<Object?>(); }

            Object?[] values = new Object?[arguments.Length];

            for(Int32 i = 0; i < arguments.Length; i++)
            {
                values[i] = BindArgument(arguments[i]);
            }

            return values;
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.ArgumentBindingFailed); return Array.Empty<Object?>(); }
    }

    private static Object? BindReference(String? id)
    {
        try
        {
            String? resolved = ResolveId(id); if(String.IsNullOrWhiteSpace(resolved)) { return null; }

            return GetTrackedObject(resolved);
        }
        catch { return null; }
    }

    private static Object? BindValue(String? data , String? type)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(type)) { return DeserializeJsonValue(data); }

            ObjectParser p = ObjectParser.Create(type).WithContext(Context).WithData(data).WithFormatter(CultureInfo.InvariantCulture);

            if(p.TryParse()) { return p.Value; }

            TypeProvider tp = TypeProvider.Create(type).WithContext(Context); Type? t = tp.Resolve(); if(t is null) { return DeserializeJsonValue(data); }

            return JsonSerializer.Deserialize(data ?? "null",t);
        }
        catch { return null; }
    }

    private static Object? DeserializeJsonValue(String? data)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(data)) { return null; }

            JsonElement e = JsonSerializer.Deserialize<JsonElement>(data);

            return e.ValueKind switch
            {
                JsonValueKind.Null   => null,
                JsonValueKind.True   => true,
                JsonValueKind.False  => false,
                JsonValueKind.String => e.GetString(),
                JsonValueKind.Number => e.TryGetInt32(out Int32 i) ? i :
                                        e.TryGetInt64(out Int64 l) ? l :
                                        e.TryGetDecimal(out Decimal m) ? m :
                                        e.GetDouble(),
                _                    => data
            };
        }
        catch { return data; }
    }
}