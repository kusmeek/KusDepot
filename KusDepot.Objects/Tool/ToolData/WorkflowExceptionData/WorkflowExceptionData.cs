namespace KusDepot;

/**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[DataContract(Name = "WorkflowExceptionData" , Namespace = "KusDepot")]
public class WorkflowExceptionData : ToolData
{
    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [NotNull]
    public String? Type {get;init;}

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="Message"]/*'/>*/
    [DataMember(Name = "Message" , EmitDefaultValue = true , IsRequired = true)] [NotNull]
    public String? Message {get;init;}

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="StackTrace"]/*'/>*/
    [DataMember(Name = "StackTrace" , EmitDefaultValue = true , IsRequired = true)] [NotNull]
    public String? StackTrace {get;init;}

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="Create"]/*'/>*/
    public static WorkflowExceptionData Create(Exception source)
    {
        return new () {Type = source.GetType().FullName , Message = source.Message , StackTrace = source.StackTrace};
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="Parse"]/*'/>*/
    public static new WorkflowExceptionData? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(WorkflowExceptionData),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            WorkflowExceptionData? _2 = _1.ReadObject(_0) as WorkflowExceptionData; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(WorkflowExceptionData),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out WorkflowExceptionData data)
    {
        data = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(WorkflowExceptionData),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            WorkflowExceptionData? _2 = _1.ReadObject(_0) as WorkflowExceptionData; if( _2 is not null ) { data = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}