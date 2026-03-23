namespace KusDepot.Serialization;

/**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/main/*'/>*/
public static class JsonUtility
{
    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/field[@name="Options"]/*'/>*/
    private static JsonSerializerOptions Options = new()
    {
        WriteIndented = false , AllowDuplicateProperties = false,

        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,

        PropertyNamingPolicy = null , PropertyNameCaseInsensitive = false,

        PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
    };

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="Serialize"]/*'/>*/
    public static Byte[] Serialize<T>(T? value)
    {
        try { return JsonSerializer.SerializeToUtf8Bytes(value,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail,typeof(T).FullName,null); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="SerializeType"]/*'/>*/
    public static Byte[] Serialize(Object? value , Type inputtype)
    {
        try { return JsonSerializer.SerializeToUtf8Bytes(value,inputtype,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail,inputtype?.FullName,null); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="Deserialize"]/*'/>*/
    public static T? Deserialize<T>(Byte[]? input)
    {
        if(input is null || input.Length == 0) { return default; }

        try { return JsonSerializer.Deserialize<T>(input,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail,typeof(T).FullName,null); if(NoExceptions) { return default; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="DeserializeType"]/*'/>*/
    public static Object? Deserialize(Byte[]? input , Type outputtype)
    {
        if(input is null || input.Length == 0) { return null; }

        try { return JsonSerializer.Deserialize(input,outputtype,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail,outputtype?.FullName,null); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="Parse"]/*'/>*/
    public static T? Parse<T>(String? input)
    {
        if(String.IsNullOrWhiteSpace(input)) { return default; }

        try { return JsonSerializer.Deserialize<T>(input,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail,typeof(T).FullName,null); if(NoExceptions) { return default; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="ParseType"]/*'/>*/
    public static Object? Parse(String? input , Type outputtype)
    {
        if(String.IsNullOrWhiteSpace(input)) { return null; }

        try { return JsonSerializer.Deserialize(input,outputtype,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail,outputtype?.FullName,null); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="ToString"]/*'/>*/
    public static String ToJsonString<T>(T? value)
    {
        try { return JsonSerializer.Serialize(value,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail,typeof(T).FullName,null); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="ToStringType"]/*'/>*/
    public static String ToJsonString(Object? value , Type inputtype)
    {
        try { return JsonSerializer.Serialize(value,inputtype,Options); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail,inputtype?.FullName,null); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<T>(String? input , [MaybeNullWhen(false)] out T value)
    {
        try { value = Parse<T>(input); return value is not null; }

        catch ( Exception _ ) { value = default; KusDepotLog.Error(_,TryParseFail,typeof(T).FullName,null); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="TryParseType"]/*'/>*/
    public static Boolean TryParse(String? input , Type outputtype , out Object? value)
    {
        try { value = Parse(input,outputtype); return value is not null; }

        catch ( Exception _ ) { value = null; KusDepotLog.Error(_,TryParseFail,outputtype?.FullName,null); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="ToFile"]/*'/>*/
    public static Boolean ToFile<T>(String path , T? value)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(path) || File.Exists(path)) { return false; }

            String fullPath = Path.GetFullPath(path); String? directory = Path.GetDirectoryName(fullPath);

            if(String.IsNullOrWhiteSpace(directory) is false && Directory.Exists(directory) is false)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(fullPath,Serialize(value));

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToFileFail,typeof(T).FullName,null); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="ToFileType"]/*'/>*/
    public static Boolean ToFile(String path , Object? value , Type inputtype)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(path) || File.Exists(path)) { return false; }

            String fullPath = Path.GetFullPath(path); String? directory = Path.GetDirectoryName(fullPath);

            if(String.IsNullOrWhiteSpace(directory) is false && Directory.Exists(directory) is false)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(fullPath,Serialize(value,inputtype));

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToFileFail,inputtype?.FullName,null); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="FromFile"]/*'/>*/
    public static T? FromFile<T>(String path)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(path)) { return default; }

            String fullPath = Path.GetFullPath(path);

            return File.Exists(fullPath) ? Deserialize<T>(File.ReadAllBytes(fullPath)) : default;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail,typeof(T).FullName,null); if(NoExceptions) { return default; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="FromFileType"]/*'/>*/
    public static Object? FromFile(String path , Type outputtype)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(path)) { return null; }

            String fullPath = Path.GetFullPath(path);

            return File.Exists(fullPath) ? Deserialize(File.ReadAllBytes(fullPath),outputtype) : null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FromFileFail,outputtype?.FullName,null); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='JsonUtility.xml' path='JsonUtility/class[@name="JsonUtility"]/method[@name="WriteIndented"]/*'/>*/
    public static String WriteIndented(Object? value , Type inputtype)
    {
        try
        {
            JsonSerializerOptions options = new(Options)
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(value,inputtype,options);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,WriteIndentedFail,inputtype?.FullName,null); if(NoExceptions) { return String.Empty; } throw; }
    }
}