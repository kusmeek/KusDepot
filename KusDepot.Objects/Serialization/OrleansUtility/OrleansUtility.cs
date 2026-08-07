namespace KusDepot.Serialization;

/**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/main/*'/>*/
public static class OrleansUtility
{
    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/field[@name="Identity"]/*'/>*/
    private static readonly SerializationIdentity Identity = new(SerializationKind.Orleans,
        Encoding.UTF8.GetBytes(typeof(Serializer).Assembly.GetName().Version?.ToString() ?? String.Empty),
        Encoding.UTF8.GetBytes(typeof(OrleansUtility).Assembly.GetName().Version?.ToString() ?? String.Empty));

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/field[@name="Header"]/*'/>*/
    private static readonly Byte[] Header = SerializationEnvelope.BuildHeader(Identity);

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/field[@name="ManifestProviderBaseTypeName"]/*'/>*/
    private const String ManifestProviderBaseTypeName = "Orleans.Serialization.Configuration.TypeManifestProviderBase";

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/field[@name="Provider"]/*'/>*/
    private static readonly Lazy<ServiceProvider> Provider = new(CreateProvider);

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/field[@name="Serializer"]/*'/>*/
    private static readonly Lazy<Serializer> Serializer = new(() => Provider.Value.GetRequiredService<Serializer>());

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="Serialize"]/*'/>*/
    public static Byte[] Serialize<T>(T instance)
    {
        try
        {
            using SerializerSession session = Serializer.Value.SessionPool.GetSession();

            var writer = Writer.CreatePooled(session);

            try
            {
                writer.Write(Header);

                Serializer.Value.Serialize(instance,ref writer); writer.Commit();

                return writer.Output.ToArray();
            }
            finally { writer.Dispose(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail,typeof(T).FullName); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="ToBase64String"]/*'/>*/
    public static String ToBase64String<T>(T instance)
    {
        try { return Serialize(instance).ToBase64FromByteArray(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToBase64StringFail,typeof(T).FullName); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="Deserialize"]/*'/>*/
    public static TResult Deserialize<TResult>(Byte[] input)
    {
        try { return Serializer.Value.Deserialize<TResult>(SerializationEnvelope.Unwrap(input)); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail,typeof(TResult).FullName); if(NoExceptions) { return default!; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="DeserializeMemory"]/*'/>*/
    public static TResult Deserialize<TResult>(ReadOnlyMemory<Byte> input)
    {
        try { return Serializer.Value.Deserialize<TResult>(SerializationEnvelope.Unwrap(input)); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail,typeof(TResult).FullName); if(NoExceptions) { return default!; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="ParseBase64"]/*'/>*/
    public static TResult? ParseBase64<TResult>(String input)
    {
        if(String.IsNullOrEmpty(input)) { return default; }

        try { return Deserialize<TResult>(input.ToByteArrayFromBase64()); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseBase64Fail,typeof(TResult).FullName); if(NoExceptions) { return default; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="TryParseBase64"]/*'/>*/
    public static Boolean TryParseBase64<TResult>(String? input , out TResult? result)
    {
        result = default;

        if(String.IsNullOrEmpty(input)) { return false; }

        try { result = Deserialize<TResult>(input.ToByteArrayFromBase64()); return result is not null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseBase64Fail,typeof(TResult).FullName); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="FromFile"]/*'/>*/
    public static TResult? FromFile<TResult>(String path)
    {
        try
        {
            if(path is null) { return default; }

            if(!File.Exists(path)) { return default; }

            return Deserialize<TResult>(File.ReadAllBytes(path));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail,typeof(TResult).FullName); if(NoExceptions) { return default; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="ToFile"]/*'/>*/
    public static Boolean ToFile<T>(String path , T instance)
    {
        try
        {
            if(File.Exists(path)) { return false; }

            File.WriteAllBytes(path,Serialize(instance));

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToFileFail,typeof(T).FullName); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="CreateProvider"]/*'/>*/
    private static ServiceProvider CreateProvider()
    {
        try
        {
            ServiceCollection services = new(); services.AddSerializer();

            PruneManifests(services);

            return services.BuildServiceProvider();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateProviderFail); throw; }
    }

    /**<include file='OrleansUtility.xml' path='OrleansUtility/class[@name="OrleansUtility"]/method[@name="PruneManifests"]/*'/>*/
    private static void PruneManifests(ServiceCollection services)
    {
        try
        {
            var ownContext = AssemblyLoadContext.GetLoadContext(typeof(OrleansUtility).Assembly);

            for(Int32 i = services.Count - 1; i >= 0; i--)
            {
                if(services[i].ImplementationType is { } type
                    && String.Equals(type.BaseType?.FullName,ManifestProviderBaseTypeName,Ordinal)
                    && AssemblyLoadContext.GetLoadContext(type.Assembly) != ownContext)
                {
                    services.RemoveAt(i);
                }
            }
        }
        catch { }
    }
}