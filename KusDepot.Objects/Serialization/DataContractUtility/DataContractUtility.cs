namespace KusDepot.Serialization;

/**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/main/*'/>*/
public static class DataContractUtility
{
    /* https://github.com/dotnet/runtime/issues/1404 */

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ToBase64String"]/*'/>*/
    public static String ToBase64String(Object instance)
    {
        try
        {
            using var m = new MemoryStream();

            using var writer = XmlDictionaryWriter.CreateBinaryWriter(m);

            var serializer = new DataContractSerializer(instance.GetType(),Settings);

            serializer.WriteObject(writer,instance); writer.Flush(); m.Position = 0;

            return m.ToArray().ToBase64FromByteArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail,instance.GetType().FullName); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="Serialize"]/*'/>*/
    public static Byte[] Serialize(Object instance)
    {
        try { return ToBase64String(instance).ToByteArrayFromBase64(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail,instance.GetType().FullName); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ParseBase64"]/*'/>*/
    public static TResult? ParseBase64<TResult>(String input) where TResult : class
    {
        if(String.IsNullOrEmpty(input)) { return null; }

        try
        {
            using var m = new MemoryStream(input.ToByteArrayFromBase64());

            using var reader = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

            var serializer = new DataContractSerializer(typeof(TResult),Settings);

            var result = serializer.ReadObject(reader) as TResult;

            if(result is not null) { return result; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail,typeof(TResult).FullName); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="TryParseBase64"]/*'/>*/
    public static Boolean TryParseBase64<TResult>(String? input , out TResult? result) where TResult : class
    {
        result = null;

        if(String.IsNullOrEmpty(input)) { return false; }

        try
        {
            using var reader = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            var serializer = new DataContractSerializer(typeof(TResult),Settings);

            var value = serializer.ReadObject(reader) as TResult;

            if(value is not null) { result = value; return true; }

            return false;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail,typeof(TResult).FullName); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="Deserialize"]/*'/>*/
    public static TResult? Deserialize<TResult>(Byte[] input) where TResult : class
    {
        if(input is null or { Length: 0 }) { return null; }

        try
        {
            using var m = new MemoryStream(input);

            using var reader = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

            var serializer = new DataContractSerializer(typeof(TResult),Settings);

            var result = serializer.ReadObject(reader) as TResult;

            if(result is not null) { return result; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail,typeof(TResult).FullName); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="FromFile"]/*'/>*/
    public static TResult? FromFile<TResult>(String path) where TResult : class
    {
        try
        {
            if(path is null) { return default; }

            if(!File.Exists(path)) { return default; }

            using FileStream fs = new(path,new FileStreamOptions{Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.SequentialScan});

            var serializer = new DataContractSerializer(typeof(TResult),Settings);

            using XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(fs,XmlDictionaryReaderQuotas.Max);

            return serializer.ReadObject(reader) as TResult;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail,typeof(TResult).FullName); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ToFile"]/*'/>*/
    public static Boolean ToFile(String path , Object instance)
    {
        try
        {
            if(File.Exists(path)) { return false; }

            using FileStream fs = new(path,new FileStreamOptions{Access = FileAccess.Write , Mode = FileMode.CreateNew , Share = FileShare.None});

            var serializer = new DataContractSerializer(instance.GetType(),Settings);

            using XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(fs);

            serializer.WriteObject(writer,instance); writer.Flush();

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToFileFail,instance.GetType().FullName); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ParseBase64Type"]/*'/>*/
    public static Object? ParseBase64(String input , Type outputtype)
    {
        if(String.IsNullOrEmpty(input) || outputtype is null) { return null; }

        try
        {
            using var m = new MemoryStream(input.ToByteArrayFromBase64());

            using var reader = XmlDictionaryReader.CreateBinaryReader(m,XmlDictionaryReaderQuotas.Max);

            var serializer = new DataContractSerializer(outputtype,Settings);

            var result = serializer.ReadObject(reader);

            if(result is not null && outputtype.IsInstanceOfType(result)) { return result; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail,outputtype?.FullName); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="TryParseBase64Type"]/*'/>*/
    public static Boolean TryParseBase64(String? input , Type outputtype , out Object? result)
    {
        result = null;

        if(String.IsNullOrEmpty(input) || outputtype is null) { return false; }

        try
        {
            using var reader = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            var serializer = new DataContractSerializer(outputtype,Settings);

            var value = serializer.ReadObject(reader);

            if(value is not null && outputtype.IsInstanceOfType(value)) { result = value; return true; }

            return false;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail,outputtype?.FullName); if(NoExceptions) { return false; } throw; }
    }

    private static readonly DataContractSerializerSettings Settings = new() { MaxItemsInObjectGraph = Int32.MaxValue };
}