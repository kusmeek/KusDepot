namespace KusDepot;

/**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/main/*'/>*/
public static class ToolValueConverter
{
    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/field[@name="ConverterSnapshot"]/*'/>*/
    private static ToolValueConverterRegistration[] ConverterSnapshot = [];

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/field[@name="ConverterRegistrations"]/*'/>*/
    private static readonly Dictionary<String,ToolValueConverterRegistration> ConverterRegistrations = new(StringComparer.Ordinal);

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/field[@name="DefaultConverterRegistrations"]/*'/>*/
    private static readonly ToolValueConverterRegistration[] DefaultConverterRegistrations =
    [
        new(){ Name = nameof(ToolValueParseConverter)               , Order = 0       , Converter = new ToolValueParseConverter() },
        new(){ Name = nameof(ToolValueConfigurationConverter)       , Order = 2000  , Converter = new ToolValueConfigurationConverter() },
        new(){ Name = nameof(ToolValueToolDataConverter)            , Order = 4000  , Converter = new ToolValueToolDataConverter() },
        new(){ Name = nameof(ToolValueGenericDataContractConverter) , Order = 6000  , Converter = new ToolValueGenericDataContractConverter() },
        new(){ Name = nameof(ToolValueGenericJsonConverter)         , Order = 8000  , Converter = new ToolValueGenericJsonConverter() },
        new(){ Name = nameof(ToolValueBuildConverter)               , Order = 10_000 , Converter = new ToolValueBuildConverter() },

        new(){ Name = nameof(ToolValueUnhandledWriteConverter)      , Order = Int32.MaxValue , Converter = new ToolValueUnhandledWriteConverter() }
    ];

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/field[@name="Sync"]/*'/>*/
    private static readonly Lock Sync = new();

    static ToolValueConverter() { ResetConverterRegistrations(); }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="GetConverterRegistrations"]/*'/>*/
    public static ToolValueConverterRegistration[] GetConverterRegistrations()
    {
        try
        {
            lock(Sync) { return [ .. ConverterSnapshot ]; }
        }
        catch { return []; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryAddConverterRegistrationParts"]/*'/>*/
    public static Boolean TryAddConverterRegistration(String? name , Int32 order , IToolValueConverter? converter)
    {
        try
        {
            return TryAddConverterRegistration(new ToolValueConverterRegistration()
            {
                Name = name ?? String.Empty,
                Order = order,
                Converter = converter
            });
        }
        catch { return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryAddConverterRegistration"]/*'/>*/
    public static Boolean TryAddConverterRegistration(ToolValueConverterRegistration? registration)
    {
        try
        {
            if(registration is null || String.IsNullOrWhiteSpace(registration.Name) || registration.Converter is null) { return false; }

            lock(Sync)
            {
                if(ConverterRegistrations.ContainsKey(registration.Name)) { return false; }

                ToolValueConverterRegistration[] next = [ .. ConverterRegistrations.Values , registration ];

                if(TryBuildOrderedSnapshot(next,out ToolValueConverterRegistration[]? snapshot) is false || snapshot is null) { return false; }

                ConverterRegistrations.Add(registration.Name,registration); ConverterSnapshot = snapshot; return true;
            }
        }
        catch { return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TrySetConverterRegistration"]/*'/>*/
    public static Boolean TrySetConverterRegistration(ToolValueConverterRegistration? registration)
    {
        try
        {
            if(registration is null || String.IsNullOrWhiteSpace(registration.Name) || registration.Converter is null) { return false; }

            lock(Sync)
            {
                ToolValueConverterRegistration[] next = [ .. ConverterRegistrations.Values.Where(_ => !String.Equals(_.Name,registration.Name,Ordinal)) , registration ];

                if(TryBuildOrderedSnapshot(next,out ToolValueConverterRegistration[]? snapshot) is false || snapshot is null) { return false; }

                ConverterRegistrations[registration.Name] = registration; ConverterSnapshot = snapshot; return true;
            }
        }
        catch { return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryRemoveConverterRegistration"]/*'/>*/
    public static Boolean TryRemoveConverterRegistration(String? name)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(name)) { return false; }

            lock(Sync)
            {
                if(ConverterRegistrations.ContainsKey(name) is false) { return false; }

                ToolValueConverterRegistration[] next = [ .. ConverterRegistrations.Values.Where(_ => !String.Equals(_.Name,name,Ordinal)) ];

                if(TryBuildOrderedSnapshot(next,out ToolValueConverterRegistration[]? snapshot) is false || snapshot is null) { return false; }

                _ = ConverterRegistrations.Remove(name); ConverterSnapshot = snapshot; return true;
            }
        }
        catch { return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="ResetConverterRegistrations"]/*'/>*/
    public static void ResetConverterRegistrations()
    {
        try
        {
            lock(Sync)
            {
                ConverterRegistrations.Clear();

                foreach(ToolValueConverterRegistration registration in DefaultConverterRegistrations)
                {
                    ConverterRegistrations[registration.Name] = registration;
                }

                if(TryBuildOrderedSnapshot(ConverterRegistrations.Values,out ToolValueConverterRegistration[]? snapshot) && snapshot is not null)
                {
                    ConverterSnapshot = snapshot;
                }
                else
                {
                    ConverterSnapshot = [];
                }
            }
        }
        catch {}
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="ToToolValue"]/*'/>*/
    public static ToolValue? ToToolValue(Object? value , IEnumerable<IToolValueConverter>? converters = null)
    {
        return TryToToolValue(value,out ToolValue? result,converters) ? result : null;
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryToToolValue"]/*'/>*/
    public static Boolean TryToToolValue(Object? value , [MaybeNullWhen(false)] out ToolValue? result , IEnumerable<IToolValueConverter>? converters = null)
    {
        result = null;

        try
        {
            if(value is null) { return true; }

            if(value is ToolValue toolvalue) { result = toolvalue; return true; }

            foreach(IToolValueConverter converter in EnumerateConverters(converters))
            {
                if(converter.TryWrite(value,out result)) { return result is not null; }
            }

            result = null; return false;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="FromToolValue"]/*'/>*/
    public static Object? FromToolValue(ToolValue? value , IEnumerable<IToolValueConverter>? converters = null)
    {
        return TryFromToolValue(value,out Object? result,converters) ? result : null;
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryFromToolValue"]/*'/>*/
    public static Boolean TryFromToolValue(ToolValue? value , [MaybeNullWhen(false)] out Object? result , IEnumerable<IToolValueConverter>? converters = null)
    {
        result = null;

        try
        {
            if(value is null) { return true; }

            foreach(IToolValueConverter converter in EnumerateConverters(converters))
            {
                if(converter.TryRead(value,out result)) { return result is not null; }
            }

            result = null; return false;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="ToToolValues"]/*'/>*/
    public static Dictionary<String,ToolValue?>? ToToolValues(IDictionary<String,Object?>? values , IEnumerable<IToolValueConverter>? converters = null)
    {
        return TryToToolValues(values,out Dictionary<String,ToolValue?>? result,converters) ? result : null;
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryToToolValues"]/*'/>*/
    public static Boolean TryToToolValues(IDictionary<String,Object?>? values , [MaybeNullWhen(false)] out Dictionary<String,ToolValue?>? result , IEnumerable<IToolValueConverter>? converters = null)
    {
        result = null;

        try
        {
            if(values is null) { return true; }

            Dictionary<String,ToolValue?> output = new();

            foreach(var value in values)
            {
                if(TryToToolValue(value.Value,out ToolValue? converted,converters) is false) { return false; }

                output[value.Key] = converted;
            }

            result = output; return true;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="FromToolValues"]/*'/>*/
    public static Dictionary<String,Object?>? FromToolValues(IDictionary<String,ToolValue?>? values , IEnumerable<IToolValueConverter>? converters = null)
    {
        return TryFromToolValues(values,out Dictionary<String,Object?>? result,converters) ? result : null;
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryFromToolValues"]/*'/>*/
    public static Boolean TryFromToolValues(IDictionary<String,ToolValue?>? values , [MaybeNullWhen(false)] out Dictionary<String,Object?>? result , IEnumerable<IToolValueConverter>? converters = null)
    {
        result = null;

        try
        {
            if(values is null) { return true; }

            Dictionary<String,Object?> output = new();

            foreach(var value in values)
            {
                if(TryFromToolValue(value.Value,out Object? converted,converters) is false) { return false; }

                output[value.Key] = converted;
            }

            result = output; return true;
        }
        catch { result = null; return false; }
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="EnumerateConverters"]/*'/>*/
    private static IEnumerable<IToolValueConverter> EnumerateConverters(IEnumerable<IToolValueConverter>? converters)
    {
        ToolValueConverterRegistration[] snapshot = ConverterSnapshot;

        return (converters ?? Array.Empty<IToolValueConverter>()).Concat(snapshot.Select(_ => _.Converter!));
    }

    /**<include file='ToolValueConverter.xml' path='ToolValueConverter/class[@name="ToolValueConverter"]/method[@name="TryBuildOrderedSnapshot"]/*'/>*/
    private static Boolean TryBuildOrderedSnapshot(IEnumerable<ToolValueConverterRegistration>? registrations , [MaybeNullWhen(false)] out ToolValueConverterRegistration[]? snapshot)
    {
        snapshot = null;

        try
        {
            if(registrations is null)
            {
                snapshot = []; return true;
            }

            ToolValueConverterRegistration[] ordered = [ .. registrations.OrderBy(_ => _.Order).ThenBy(_ => _.Name,StringComparer.Ordinal) ];

            HashSet<Int32> usedorders = new();

            foreach(ToolValueConverterRegistration registration in ordered)
            {
                if(String.IsNullOrWhiteSpace(registration.Name) || registration.Converter is null) { return false; }

                if(usedorders.Add(registration.Order) is false) { return false; }
            }

            snapshot = ordered; return true;
        }
        catch { snapshot = null; return false; }
    }
}