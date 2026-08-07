namespace KusDepot.Serialization;

/**<include file='JsonContext.xml' path='JsonContext/class[@name="JsonContext"]/main/*'/>*/

[JsonSerializable(typeof(Uri))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(BigInteger))]
[JsonSerializable(typeof(DateTimeOffset))]

[JsonSerializable(typeof(Char[]))]
[JsonSerializable(typeof(Char?[]))]
[JsonSerializable(typeof(Int32[]))]
[JsonSerializable(typeof(Int32?[]))]
[JsonSerializable(typeof(Byte[]))]
[JsonSerializable(typeof(Byte?[]))]
[JsonSerializable(typeof(SByte[]))]
[JsonSerializable(typeof(SByte?[]))]
[JsonSerializable(typeof(Int16[]))]
[JsonSerializable(typeof(Int16?[]))]
[JsonSerializable(typeof(UInt16[]))]
[JsonSerializable(typeof(UInt16?[]))]
[JsonSerializable(typeof(UInt32[]))]
[JsonSerializable(typeof(UInt32?[]))]
[JsonSerializable(typeof(Int64[]))]
[JsonSerializable(typeof(Int64?[]))]
[JsonSerializable(typeof(UInt64[]))]
[JsonSerializable(typeof(UInt64?[]))]
[JsonSerializable(typeof(Single[]))]
[JsonSerializable(typeof(Single?[]))]
[JsonSerializable(typeof(Double[]))]
[JsonSerializable(typeof(Double?[]))]
[JsonSerializable(typeof(Decimal[]))]
[JsonSerializable(typeof(Decimal?[]))]

[JsonSerializable(typeof(HashSet<Guid>))]
[JsonSerializable(typeof(HashSet<Guid?>))]

[JsonSerializable(typeof(List<String>))]
[JsonSerializable(typeof(List<String?>))]

[JsonSerializable(typeof(Dictionary<String,String>))]
[JsonSerializable(typeof(Dictionary<String,String?>))]
[JsonSerializable(typeof(Dictionary<String,Int32>))]
[JsonSerializable(typeof(Dictionary<String,Int32?>))]

[JsonSerializable(typeof(HashSet<DataItem>))]
[JsonSerializable(typeof(SortedList<Int32,MSBuildItem>))]

[JsonSerializable(typeof(KusDepot.Data.Models.Command))]
[JsonSerializable(typeof(KusDepot.Data.Models.CommandQuery))]
[JsonSerializable(typeof(KusDepot.Data.Models.CommandResponse))]
[JsonSerializable(typeof(KusDepot.Data.Models.Element))]
[JsonSerializable(typeof(KusDepot.Data.Models.ElementQuery))]
[JsonSerializable(typeof(KusDepot.Data.Models.ElementResponse))]
[JsonSerializable(typeof(KusDepot.Data.Models.Media))]
[JsonSerializable(typeof(KusDepot.Data.Models.MediaQuery))]
[JsonSerializable(typeof(KusDepot.Data.Models.MediaResponse))]
[JsonSerializable(typeof(KusDepot.Data.Models.NoteQuery))]
[JsonSerializable(typeof(KusDepot.Data.Models.NoteResponse))]
[JsonSerializable(typeof(KusDepot.Data.Models.Service))]
[JsonSerializable(typeof(KusDepot.Data.Models.ServiceQuery))]
[JsonSerializable(typeof(KusDepot.Data.Models.ServiceResponse))]
[JsonSerializable(typeof(KusDepot.Data.Models.TagQuery))]
[JsonSerializable(typeof(KusDepot.Data.Models.TagResponse))]

[JsonSerializable(typeof(AccessRequest))]
[JsonSerializable(typeof(BinaryItem))]
[JsonSerializable(typeof(CodeItem))]
[JsonSerializable(typeof(CommandDescriptor))]
[JsonSerializable(typeof(CommandWorkflow))]
[JsonSerializable(typeof(DataItem))]
[JsonSerializable(typeof(DataSetItem))]
[JsonSerializable(typeof(DataStreamItem))]
[JsonSerializable(typeof(Descriptor))]
[JsonSerializable(typeof(GenericItem))]
[JsonSerializable(typeof(GuidReferenceItem))]
[JsonSerializable(typeof(KeySet))]
[JsonSerializable(typeof(KusDepotCab))]
[JsonSerializable(typeof(MSBuildItem))]
[JsonSerializable(typeof(MultiMediaItem))]
[JsonSerializable(typeof(SecurityKey))]
[JsonSerializable(typeof(TextItem))]
[JsonSerializable(typeof(ToolDescriptor))]


[JsonSourceGenerationOptions(

    WriteIndented = false,

    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,

    DefaultIgnoreCondition = JsonIgnoreCondition.Never,

    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace)]

internal sealed partial class JsonContext : JsonSerializerContext {}