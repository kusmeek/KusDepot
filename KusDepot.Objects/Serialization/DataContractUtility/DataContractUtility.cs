namespace KusDepot.Serialization;

/**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/main/*'/>*/
internal static class DataContractUtility
{
    /* https://github.com/dotnet/runtime/issues/1404 */

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ToBase64String"]/*'/>*/
    internal static String ToBase64String(Object instance , SerializationData data)
    {
        try
        {
            using var m = new MemoryStream();

            using var writer = XmlDictionaryWriter.CreateBinaryWriter(m);

            var serializer = new DataContractSerializer(instance.GetType(),Settings);

            serializer.WriteObject(writer,instance); writer.Flush(); m.Position = 0;

            return m.ToArray().ToBase64FromByteArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail,data.TypeName,data.ID); if(data.NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="Serialize"]/*'/>*/
    internal static Byte[] Serialize(Object instance , SerializationData data)
    {
        try { return ToBase64String(instance,data).ToByteArrayFromBase64(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail,data.TypeName,data.ID); if(data.NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ParseBase64"]/*'/>*/
    internal static TResult? ParseBase64<TResult>(String input , SerializationData data) where TResult : class
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

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail,data.TypeName,data.ID); if(data.NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="TryParseBase64"]/*'/>*/
    internal static Boolean TryParseBase64<TResult>(String? input , SerializationData data , out TResult? result) where TResult : class
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

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail,data.TypeName,data.ID); if(data.NoExceptions) { return false; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="Deserialize"]/*'/>*/
    internal static TResult? Deserialize<TResult>(Byte[] input , SerializationData data) where TResult : class
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

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail,data.TypeName,data.ID); if(data.NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="FromFile"]/*'/>*/
    internal static TResult? FromFile<TResult>(String path , SerializationData data) where TResult : class
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
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail,data.TypeName,data.ID); if(data.NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ToFile"]/*'/>*/
    internal static Boolean ToFile(String path , Object instance , SerializationData data)
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
        catch ( Exception _ ) { KusDepotLog.Error(_,ToFileFail,data.TypeName,data.ID); if(data.NoExceptions) { return false; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="ParseBase64Type"]/*'/>*/
    internal static Object? ParseBase64(String input , Type outputtype , SerializationData data)
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

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail,data.TypeName,data.ID); if(data.NoExceptions) { return null; } throw; }
    }

    /**<include file='DataContractUtility.xml' path='DataContractUtility/class[@name="DataContractUtility"]/method[@name="TryParseBase64Type"]/*'/>*/
    internal static Boolean TryParseBase64(String? input , Type outputtype , SerializationData data , out Object? result)
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

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail,data.TypeName,data.ID); if(data.NoExceptions) { return false; } throw; }
    }

    private static readonly DataContractSerializerSettings Settings = new() { MaxItemsInObjectGraph = Int32.MaxValue };
}