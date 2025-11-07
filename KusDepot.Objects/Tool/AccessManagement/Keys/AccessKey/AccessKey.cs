namespace KusDepot;

/**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("AccessKey")]
[DataContract(Name = "AccessKey" , Namespace = "KusDepot")]
public abstract class AccessKey : SecurityKey
{
    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="ParseAny"]/*'/>*/
    public static new AccessKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; } AccessKey? k;

            try { k = Parse<HostKey>(input,format);      if(k is not null) { return k; } } catch { }

            try { k = Parse<MyHostKey>(input,format);    if(k is not null) { return k; } } catch { }

            try { k = Parse<ClientKey>(input,format);    if(k is not null) { return k; } } catch { }

            try { k = Parse<ServiceKey>(input,format);   if(k is not null) { return k; } } catch { }

            try { k = Parse<CommandKey>(input,format);   if(k is not null) { return k; } } catch { }

            try { k = Parse<ExecutiveKey>(input,format); if(k is not null) { return k; } } catch { }

            return null;
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } Log.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="Parse"]/*'/>*/
    public static new TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : AccessKey
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } Log.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessKey.xml' path='AccessKey/class[@name="AccessKey"]/method[@name="TryParse"]/*'/>*/
    public static new Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : AccessKey
    {
        result = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { result = _2; return true; }

            return false;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } Log.Error(_,TryParseFail); throw; }
    }
}