namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class ToolDataConverterExam
{
    [Test, TestCaseSource(nameof(GetAllKnownTypeWriteCases))]
    public void ToolData_MyData_Getter_KnownTypes_WriteCases(Object? value , Type expectedType , ToolValueMode expectedMode)
    {
        ToolData data = new() { Data = value };

        ToolValue? back = data.MyData;

        Check.That(back).IsNotNull();
        Check.That(back!.Type).IsEqualTo(expectedType.FullName);
        Check.That(back.Mode).IsEqualTo(expectedMode);
    }

    private static IEnumerable<TestCaseData> GetAllKnownTypeWriteCases()
    {
        yield return WriteCase(Guid.Parse("11111111-1111-1111-1111-111111111111"), typeof(Guid), ToolValueMode.Parse, "KnownType_Write_Guid");
        yield return WriteCase((Guid?)Guid.Parse("22222222-2222-2222-2222-222222222222"), typeof(Guid), ToolValueMode.Parse, "KnownType_Write_NullableGuid");
        yield return WriteCase(new Version(1, 2, 3, 4), typeof(Version), ToolValueMode.Parse, "KnownType_Write_Version");
        yield return WriteCase(new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc), typeof(DateTime), ToolValueMode.Parse, "KnownType_Write_DateTime");
        yield return WriteCase(TimeSpan.FromMinutes(5), typeof(TimeSpan), ToolValueMode.Parse, "KnownType_Write_TimeSpan");
        yield return WriteCase(new BigInteger(123456789), typeof(BigInteger), ToolValueMode.Parse, "KnownType_Write_BigInteger");
        yield return WriteCase(new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero), typeof(DateTimeOffset), ToolValueMode.Parse, "KnownType_Write_DateTimeOffset");

        yield return WriteCase(new Uri("https://example.org/"), typeof(Uri), ToolValueMode.Custom, "KnownType_Write_Uri");
        yield return WriteCase(new StringBuilder("builder"), typeof(StringBuilder), ToolValueMode.Custom, "KnownType_Write_StringBuilder");
        yield return WriteCase(new[] { 'a', 'b' }, typeof(Char[]), ToolValueMode.Custom, "KnownType_Write_CharArray");
        yield return WriteCase(new Char?[] { 'a', null, 'b' }, typeof(Char?[]), ToolValueMode.Custom, "KnownType_Write_NullableCharArray");
        yield return WriteCase(new[] { 1, 2, 3 }, typeof(Int32[]), ToolValueMode.Custom, "KnownType_Write_Int32Array");
        yield return WriteCase(new Int32?[] { 1, null, 3 }, typeof(Int32?[]), ToolValueMode.Custom, "KnownType_Write_NullableInt32Array");
        yield return WriteCase(new Byte[] { 1, 2, 3 }, typeof(Byte[]), ToolValueMode.Custom, "KnownType_Write_ByteArray");
        yield return WriteCase(new Byte?[] { 1, null, 3 }, typeof(Byte?[]), ToolValueMode.Custom, "KnownType_Write_NullableByteArray");
        yield return WriteCase(new SByte[] { 1, 2, 3 }, typeof(SByte[]), ToolValueMode.Custom, "KnownType_Write_SByteArray");
        yield return WriteCase(new SByte?[] { 1, null, 3 }, typeof(SByte?[]), ToolValueMode.Custom, "KnownType_Write_NullableSByteArray");
        yield return WriteCase(new Int16[] { 1, 2, 3 }, typeof(Int16[]), ToolValueMode.Custom, "KnownType_Write_Int16Array");
        yield return WriteCase(new Int16?[] { 1, null, 3 }, typeof(Int16?[]), ToolValueMode.Custom, "KnownType_Write_NullableInt16Array");
        yield return WriteCase(new UInt16[] { 1, 2, 3 }, typeof(UInt16[]), ToolValueMode.Custom, "KnownType_Write_UInt16Array");
        yield return WriteCase(new UInt16?[] { 1, null, 3 }, typeof(UInt16?[]), ToolValueMode.Custom, "KnownType_Write_NullableUInt16Array");
        yield return WriteCase(new UInt32[] { 1, 2, 3 }, typeof(UInt32[]), ToolValueMode.Custom, "KnownType_Write_UInt32Array");
        yield return WriteCase(new UInt32?[] { 1, null, 3 }, typeof(UInt32?[]), ToolValueMode.Custom, "KnownType_Write_NullableUInt32Array");
        yield return WriteCase(new Int64[] { 1, 2, 3 }, typeof(Int64[]), ToolValueMode.Custom, "KnownType_Write_Int64Array");
        yield return WriteCase(new Int64?[] { 1, null, 3 }, typeof(Int64?[]), ToolValueMode.Custom, "KnownType_Write_NullableInt64Array");
        yield return WriteCase(new UInt64[] { 1, 2, 3 }, typeof(UInt64[]), ToolValueMode.Custom, "KnownType_Write_UInt64Array");
        yield return WriteCase(new UInt64?[] { 1, null, 3 }, typeof(UInt64?[]), ToolValueMode.Custom, "KnownType_Write_NullableUInt64Array");
        yield return WriteCase(new Single[] { 1f, 2f, 3f }, typeof(Single[]), ToolValueMode.Custom, "KnownType_Write_SingleArray");
        yield return WriteCase(new Single?[] { 1f, null, 3f }, typeof(Single?[]), ToolValueMode.Custom, "KnownType_Write_NullableSingleArray");
        yield return WriteCase(new Double[] { 1d, 2d, 3d }, typeof(Double[]), ToolValueMode.Custom, "KnownType_Write_DoubleArray");
        yield return WriteCase(new Double?[] { 1d, null, 3d }, typeof(Double?[]), ToolValueMode.Custom, "KnownType_Write_NullableDoubleArray");
        yield return WriteCase(new Decimal[] { 1m, 2m, 3m }, typeof(Decimal[]), ToolValueMode.Custom, "KnownType_Write_DecimalArray");
        yield return WriteCase(new Decimal?[] { 1m, null, 3m }, typeof(Decimal?[]), ToolValueMode.Custom, "KnownType_Write_NullableDecimalArray");

        yield return WriteCase(new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() }, typeof(HashSet<Guid>), ToolValueMode.Custom, "KnownType_Write_HashSetGuid");
        yield return WriteCase(new HashSet<Guid?> { Guid.NewGuid(), null }, typeof(HashSet<Guid?>), ToolValueMode.Custom, "KnownType_Write_HashSetNullableGuid");
        yield return WriteCase(new HashSet<DataItem> { new TextItem("hello") }, typeof(HashSet<DataItem>), ToolValueMode.Custom, "KnownType_Write_HashSetDataItem");

        yield return WriteCase(new Stack<Object>(new Object[] { 1, "two", 3.0 }), typeof(Stack<Object>), ToolValueMode.Custom, "KnownType_Write_StackObject");
        yield return WriteCase(new Stack<Object?>(new Object?[] { 1, null, "two" }), typeof(Stack<Object?>), ToolValueMode.Custom, "KnownType_Write_StackNullableObject");
        yield return WriteCase(new Queue<Object>(new Object[] { 1, "two", 3.0 }), typeof(Queue<Object>), ToolValueMode.Custom, "KnownType_Write_QueueObject");
        yield return WriteCase(new Queue<Object?>(new Object?[] { 1, null, "two" }), typeof(Queue<Object?>), ToolValueMode.Custom, "KnownType_Write_QueueNullableObject");

        yield return WriteCase(new List<Object> { 1, "two", 3.0 }, typeof(List<Object>), ToolValueMode.Custom, "KnownType_Write_ListObject");
        yield return WriteCase(new List<Object?> { 1, null, "two" }, typeof(List<Object?>), ToolValueMode.Custom, "KnownType_Write_ListNullableObject");
        yield return WriteCase(new List<String> { "one", "two" }, typeof(List<String>), ToolValueMode.Custom, "KnownType_Write_ListString");
        yield return WriteCase(new List<String?> { "one", null, "two" }, typeof(List<String?>), ToolValueMode.Custom, "KnownType_Write_ListNullableString");

        yield return WriteCase(new Dictionary<String,Object> { ["A"] = 1, ["B"] = "two" }, typeof(Dictionary<String,Object>), ToolValueMode.Custom, "KnownType_Write_DictionaryStringObject");
        yield return WriteCase(new Dictionary<String,Object?> { ["A"] = 1, ["B"] = null }, typeof(Dictionary<String,Object?>), ToolValueMode.Custom, "KnownType_Write_DictionaryStringNullableObject");
        yield return WriteCase(new Dictionary<String,String> { ["A"] = "one", ["B"] = "two" }, typeof(Dictionary<String,String>), ToolValueMode.Custom, "KnownType_Write_DictionaryStringString");
        yield return WriteCase(new Dictionary<String,String?> { ["A"] = "one", ["B"] = null }, typeof(Dictionary<String,String?>), ToolValueMode.Custom, "KnownType_Write_DictionaryStringNullableString");
        yield return WriteCase(new Dictionary<String,Int32> { ["A"] = 1, ["B"] = 2 }, typeof(Dictionary<String,Int32>), ToolValueMode.Custom, "KnownType_Write_DictionaryStringInt32");
        yield return WriteCase(new Dictionary<String,Int32?> { ["A"] = 1, ["B"] = null }, typeof(Dictionary<String,Int32?>), ToolValueMode.Custom, "KnownType_Write_DictionaryStringNullableInt32");
        yield return WriteCase(new SortedList<Int32,MSBuildItem> { [0] = new MSBuildItem("build") }, typeof(SortedList<Int32,MSBuildItem>), ToolValueMode.Custom, "KnownType_Write_SortedListMSBuildItem");

        yield return WriteCase(new CommandDescriptor { Type = typeof(Tool000).FullName, Specifications = "spec", ExtendedData = new List<String> { "x" }, RegisteredHandles = new List<String> { "h" } }, typeof(CommandDescriptor), ToolValueMode.Parse, "KnownType_Write_CommandDescriptor");
        yield return WriteCase(new CommandWorkflow(), typeof(CommandWorkflow), ToolValueMode.Parse, "KnownType_Write_CommandWorkflow");
        yield return WriteCase(new CommandDetails(handle: "H", arguments: new Dictionary<String,Object?> { ["A"] = 1 }, id: Guid.NewGuid(), workflow: new CommandWorkflow()), typeof(CommandDetails), ToolValueMode.Parse, "KnownType_Write_CommandDetails");
        yield return WriteCase(new ToolDescriptor { ID = Guid.NewGuid(), Type = typeof(Tool000).FullName, Specifications = "spec", ExtendedData = new List<String> { "x" } }, typeof(ToolDescriptor), ToolValueMode.Parse, "KnownType_Write_ToolDescriptor");
        yield return WriteCase(new KusDepotCab { Type = typeof(StandardRequest).AssemblyQualifiedName, Data = JsonUtility.ToJsonString(new StandardRequest("cab"), typeof(StandardRequest)) }, typeof(KusDepotCab), ToolValueMode.Parse, "KnownType_Write_KusDepotCab");
        yield return WriteCase(new Descriptor { Application = "App", ApplicationVersion = "1", Artist = "A", BornOn = "Now", Commands = new HashSet<CommandDescriptor>(), ContentStreamed = false, DataType = "Data", DistinguishedName = "DN", FILE = "file", ID = Guid.NewGuid(), LiveStream = false, Modified = "Now", Name = "Name", Notes = new HashSet<String>(), ObjectType = typeof(TextItem).FullName, Services = new HashSet<ToolDescriptor>(), ServiceVersion = "1", Size = "1", Tags = new HashSet<String>(), Title = "Title", Version = "1", Year = "2026" }, typeof(Descriptor), ToolValueMode.Parse, "KnownType_Write_Descriptor");
        yield return WriteCase(new ToolOutput { ID = Guid.NewGuid(), Data = new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero) }, typeof(ToolOutput), ToolValueMode.Custom, "KnownType_Write_ToolOutput");
        yield return WriteCase(new ToolInput { Data = new Uri("https://example.org/") }, typeof(ToolInput), ToolValueMode.Custom, "KnownType_Write_ToolInput");
        yield return WriteCase(new ToolData { Data = new Version(1, 2, 3, 4) }, typeof(ToolData), ToolValueMode.Custom, "KnownType_Write_ToolData");

        yield return WriteCase((Object)new KeySet(new SecurityKey[] { new ServiceKey(Bytes(10), Guid.NewGuid()) }), typeof(KeySet), ToolValueMode.Parse, "KnownType_Write_KeySet_DataItemKnownType");
        yield return WriteCase(new TextItem("hello"), typeof(TextItem), ToolValueMode.Parse, "KnownType_Write_TextItem");
        yield return WriteCase(new CodeItem("code"), typeof(CodeItem), ToolValueMode.Parse, "KnownType_Write_CodeItem");
        yield return WriteCase(new BinaryItem(new Byte[] { 1, 2, 3 }), typeof(BinaryItem), ToolValueMode.Parse, "KnownType_Write_BinaryItem");
        yield return WriteCase(new MSBuildItem("build"), typeof(MSBuildItem), ToolValueMode.Parse, "KnownType_Write_MSBuildItem");
        yield return WriteCase(new GenericItem(new Object[] { "nested", 42 }), typeof(GenericItem), ToolValueMode.Parse, "KnownType_Write_GenericItem");
        yield return WriteCase(new DataSetItem(new DataItem[] { new TextItem("nested") }), typeof(DataSetItem), ToolValueMode.Parse, "KnownType_Write_DataSetItem");
        yield return WriteCase(new MultiMediaItem(), typeof(MultiMediaItem), ToolValueMode.Parse, "KnownType_Write_MultiMediaItem");
        yield return WriteCase(new GuidReferenceItem(Guid.NewGuid()), typeof(GuidReferenceItem), ToolValueMode.Parse, "KnownType_Write_GuidReferenceItem");

        yield return WriteCase((Object)new KeySet(new SecurityKey[] { new ServiceKey(Bytes(10), Guid.NewGuid()) }), typeof(KeySet), ToolValueMode.Parse, "KnownType_Write_KeySet_SecurityKnownType");
        yield return WriteCase(new HostKey(Bytes(1), Guid.NewGuid()), typeof(HostKey), ToolValueMode.Parse, "KnownType_Write_HostKey");
        yield return WriteCase(new TokenKey(new Byte[] { 1, 2, 3 }, Guid.NewGuid(), TokenKeyType.Jwt), typeof(TokenKey), ToolValueMode.Parse, "KnownType_Write_TokenKey");
        using(var ownercert = CreateCertificate(Guid.NewGuid(), "Owner"))
        {
            yield return WriteCase(new OwnerKey(ownercert!, Guid.NewGuid()), typeof(OwnerKey), ToolValueMode.Parse, "KnownType_Write_OwnerKey");
        }
        yield return WriteCase(new MyHostKey(Bytes(2), Guid.NewGuid()), typeof(MyHostKey), ToolValueMode.Parse, "KnownType_Write_MyHostKey");
        yield return WriteCase((Object)new ServiceKey(Bytes(3), Guid.NewGuid()), typeof(ServiceKey), ToolValueMode.Parse, "KnownType_Write_AccessKeyRepresentative");
        yield return WriteCase(new ClientKey(Bytes(4), Guid.NewGuid()), typeof(ClientKey), ToolValueMode.Parse, "KnownType_Write_ClientKey");
        using(var managercert = CreateCertificate(Guid.NewGuid(), "Manager"))
        {
            yield return WriteCase(new ManagerKey(managercert!, Guid.NewGuid()), typeof(ManagerKey), ToolValueMode.Parse, "KnownType_Write_ManagerKey");
        }
        yield return WriteCase(new ServiceKey(Bytes(5), Guid.NewGuid()), typeof(ServiceKey), ToolValueMode.Parse, "KnownType_Write_ServiceKey");
        yield return WriteCase(new CommandKey(Bytes(6), Guid.NewGuid()), typeof(CommandKey), ToolValueMode.Parse, "KnownType_Write_CommandKey");
        yield return WriteCase(new ExecutiveKey(Bytes(7), Guid.NewGuid()), typeof(ExecutiveKey), ToolValueMode.Parse, "KnownType_Write_ExecutiveKey");
        using(var managementcert = CreateCertificate(Guid.NewGuid(), "Management"))
        {
            yield return WriteCase((Object)new ManagerKey(managementcert!, Guid.NewGuid()), typeof(ManagerKey), ToolValueMode.Parse, "KnownType_Write_ManagementKeyRepresentative");
        }
        yield return WriteCase(new ServiceRequest(null, "service-request"), typeof(ServiceRequest), ToolValueMode.Parse, "KnownType_Write_ServiceRequest");
        yield return WriteCase(new StandardRequest("standard-request"), typeof(StandardRequest), ToolValueMode.Parse, "KnownType_Write_StandardRequest");
        yield return WriteCase(new ManagementRequest("management-request"), typeof(ManagementRequest), ToolValueMode.Parse, "KnownType_Write_ManagementRequest");
    }

    [Test, TestCaseSource(nameof(GetReadableKnownTypeRoundtripCases))]
    public void ToolData_MyData_Setter_KnownTypes_Readable_Roundtrip(ToolValue value , Type expectedType , String? expectedBackType = null)
    {
        ToolData data = new() { MyData = value };

        Check.That(data.Data).IsNotNull();
        Check.That(data.Data!.GetType()).IsEqualTo(expectedType);

        ToolValue? back = data.MyData;
        Check.That(back).IsNotNull();
        Check.That(back!.Type).IsEqualTo(expectedBackType ?? expectedType.FullName);
    }

    private static IEnumerable<TestCaseData> GetReadableKnownTypeRoundtripCases()
    {
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Parse, Type = typeof(Guid).FullName, Data = Guid.Parse("11111111-1111-1111-1111-111111111111").ToString() }, typeof(Guid), "KnownType_Read_Guid");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Parse, Type = typeof(Version).FullName, Data = "1.2.3.4" }, typeof(Version), "KnownType_Read_Version");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Parse, Type = typeof(DateTime).FullName, Data = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc).ToString("O") }, typeof(DateTime), "KnownType_Read_DateTime");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Parse, Type = typeof(TimeSpan).FullName, Data = TimeSpan.FromMinutes(5).ToString() }, typeof(TimeSpan), "KnownType_Read_TimeSpan");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Parse, Type = typeof(BigInteger).FullName, Data = new BigInteger(123456789).ToString() }, typeof(BigInteger), "KnownType_Read_BigInteger");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Parse, Type = typeof(DateTimeOffset).FullName, Data = new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero).ToString("O") }, typeof(DateTimeOffset), "KnownType_Read_DateTimeOffset");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(IConfiguration).FullName, Data = "{\"Alpha\":\"One\",\"Section:Beta\":\"Two\"}" }, typeof(ConfigurationManager), "KnownType_Read_IConfigurationCustom", typeof(IConfiguration).FullName);
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Uri).FullName, Data = JsonUtility.ToJsonString(new Uri("https://example.org/"), typeof(Uri)) }, typeof(Uri), "KnownType_Read_Uri");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(StringBuilder).FullName, Data = JsonUtility.ToJsonString(new StringBuilder("builder"), typeof(StringBuilder)) }, typeof(StringBuilder), "KnownType_Read_StringBuilder");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Char[]).FullName, Data = JsonUtility.ToJsonString(new[] { 'a', 'b' }, typeof(Char[])) }, typeof(Char[]), "KnownType_Read_CharArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Char?[]).FullName, Data = JsonUtility.ToJsonString(new Char?[] { 'a', null, 'b' }, typeof(Char?[])) }, typeof(Char?[]), "KnownType_Read_NullableCharArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Int32[]).FullName, Data = JsonUtility.ToJsonString(new[] { 1, 2, 3 }, typeof(Int32[])) }, typeof(Int32[]), "KnownType_Read_Int32Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Int32?[]).FullName, Data = JsonUtility.ToJsonString(new Int32?[] { 1, null, 3 }, typeof(Int32?[])) }, typeof(Int32?[]), "KnownType_Read_NullableInt32Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Byte[]).FullName, Data = JsonUtility.ToJsonString(new Byte[] { 1, 2, 3 }, typeof(Byte[])) }, typeof(Byte[]), "KnownType_Read_ByteArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Byte?[]).FullName, Data = JsonUtility.ToJsonString(new Byte?[] { 1, null, 3 }, typeof(Byte?[])) }, typeof(Byte?[]), "KnownType_Read_NullableByteArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(SByte[]).FullName, Data = JsonUtility.ToJsonString(new SByte[] { 1, 2, 3 }, typeof(SByte[])) }, typeof(SByte[]), "KnownType_Read_SByteArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(SByte?[]).FullName, Data = JsonUtility.ToJsonString(new SByte?[] { 1, null, 3 }, typeof(SByte?[])) }, typeof(SByte?[]), "KnownType_Read_NullableSByteArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Int16[]).FullName, Data = JsonUtility.ToJsonString(new Int16[] { 1, 2, 3 }, typeof(Int16[])) }, typeof(Int16[]), "KnownType_Read_Int16Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Int16?[]).FullName, Data = JsonUtility.ToJsonString(new Int16?[] { 1, null, 3 }, typeof(Int16?[])) }, typeof(Int16?[]), "KnownType_Read_NullableInt16Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(UInt16[]).FullName, Data = JsonUtility.ToJsonString(new UInt16[] { 1, 2, 3 }, typeof(UInt16[])) }, typeof(UInt16[]), "KnownType_Read_UInt16Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(UInt16?[]).FullName, Data = JsonUtility.ToJsonString(new UInt16?[] { 1, null, 3 }, typeof(UInt16?[])) }, typeof(UInt16?[]), "KnownType_Read_NullableUInt16Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(UInt32[]).FullName, Data = JsonUtility.ToJsonString(new UInt32[] { 1, 2, 3 }, typeof(UInt32[])) }, typeof(UInt32[]), "KnownType_Read_UInt32Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(UInt32?[]).FullName, Data = JsonUtility.ToJsonString(new UInt32?[] { 1, null, 3 }, typeof(UInt32?[])) }, typeof(UInt32?[]), "KnownType_Read_NullableUInt32Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Int64[]).FullName, Data = JsonUtility.ToJsonString(new Int64[] { 1, 2, 3 }, typeof(Int64[])) }, typeof(Int64[]), "KnownType_Read_Int64Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Int64?[]).FullName, Data = JsonUtility.ToJsonString(new Int64?[] { 1, null, 3 }, typeof(Int64?[])) }, typeof(Int64?[]), "KnownType_Read_NullableInt64Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(UInt64[]).FullName, Data = JsonUtility.ToJsonString(new UInt64[] { 1, 2, 3 }, typeof(UInt64[])) }, typeof(UInt64[]), "KnownType_Read_UInt64Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(UInt64?[]).FullName, Data = JsonUtility.ToJsonString(new UInt64?[] { 1, null, 3 }, typeof(UInt64?[])) }, typeof(UInt64?[]), "KnownType_Read_NullableUInt64Array");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Single[]).FullName, Data = JsonUtility.ToJsonString(new Single[] { 1f, 2f, 3f }, typeof(Single[])) }, typeof(Single[]), "KnownType_Read_SingleArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Single?[]).FullName, Data = JsonUtility.ToJsonString(new Single?[] { 1f, null, 3f }, typeof(Single?[])) }, typeof(Single?[]), "KnownType_Read_NullableSingleArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Double[]).FullName, Data = JsonUtility.ToJsonString(new Double[] { 1d, 2d, 3d }, typeof(Double[])) }, typeof(Double[]), "KnownType_Read_DoubleArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Double?[]).FullName, Data = JsonUtility.ToJsonString(new Double?[] { 1d, null, 3d }, typeof(Double?[])) }, typeof(Double?[]), "KnownType_Read_NullableDoubleArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Decimal[]).FullName, Data = JsonUtility.ToJsonString(new Decimal[] { 1m, 2m, 3m }, typeof(Decimal[])) }, typeof(Decimal[]), "KnownType_Read_DecimalArray");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Decimal?[]).FullName, Data = JsonUtility.ToJsonString(new Decimal?[] { 1m, null, 3m }, typeof(Decimal?[])) }, typeof(Decimal?[]), "KnownType_Read_NullableDecimalArray");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(HashSet<Guid>).FullName, Data = JsonUtility.ToJsonString(new HashSet<Guid> { Guid.NewGuid() , Guid.NewGuid() }, typeof(HashSet<Guid>)) }, typeof(HashSet<Guid>), "KnownType_Read_HashSetGuid");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(HashSet<Guid?>).FullName, Data = JsonUtility.ToJsonString(new HashSet<Guid?> { Guid.NewGuid() , null }, typeof(HashSet<Guid?>)) }, typeof(HashSet<Guid?>), "KnownType_Read_HashSetNullableGuid");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(HashSet<DataItem>).FullName, Data = JsonUtility.ToJsonString(new HashSet<DataItem> { new TextItem("hello") }, typeof(HashSet<DataItem>)) }, typeof(HashSet<DataItem>), "KnownType_Read_HashSetDataItem");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Stack<Object>).FullName, Data = JsonUtility.ToJsonString(new Stack<Object>(new Object[] { 1, "two", 3.0 }), typeof(Stack<Object>)) }, typeof(Stack<Object>), "KnownType_Read_StackObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Stack<Object?>).FullName, Data = JsonUtility.ToJsonString(new Stack<Object?>(new Object?[] { 1, null, "two" }), typeof(Stack<Object?>)) }, typeof(Stack<Object?>), "KnownType_Read_StackNullableObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Queue<Object>).FullName, Data = JsonUtility.ToJsonString(new Queue<Object>(new Object[] { 1, "two", 3.0 }), typeof(Queue<Object>)) }, typeof(Queue<Object>), "KnownType_Read_QueueObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Queue<Object?>).FullName, Data = JsonUtility.ToJsonString(new Queue<Object?>(new Object?[] { 1, null, "two" }), typeof(Queue<Object?>)) }, typeof(Queue<Object?>), "KnownType_Read_QueueNullableObject");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(List<Object>).FullName, Data = JsonUtility.ToJsonString(new List<Object> { 1, "two", 3.0 }, typeof(List<Object>)) }, typeof(List<Object>), "KnownType_Read_ListObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(List<Object?>).FullName, Data = JsonUtility.ToJsonString(new List<Object?> { 1, null, "two" }, typeof(List<Object?>)) }, typeof(List<Object?>), "KnownType_Read_ListNullableObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(List<String>).FullName, Data = JsonUtility.ToJsonString(new List<String> { "one", "two" }, typeof(List<String>)) }, typeof(List<String>), "KnownType_Read_ListString");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(List<String?>).FullName, Data = JsonUtility.ToJsonString(new List<String?> { "one", null, "two" }, typeof(List<String?>)) }, typeof(List<String?>), "KnownType_Read_ListNullableString");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Dictionary<String,Object>).FullName, Data = JsonUtility.ToJsonString(new Dictionary<String,Object> { ["A"] = 1, ["B"] = "two" }, typeof(Dictionary<String,Object>)) }, typeof(Dictionary<String,Object>), "KnownType_Read_DictionaryStringObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Dictionary<String,Object?>).FullName, Data = JsonUtility.ToJsonString(new Dictionary<String,Object?> { ["A"] = 1, ["B"] = null }, typeof(Dictionary<String,Object?>)) }, typeof(Dictionary<String,Object?>), "KnownType_Read_DictionaryStringNullableObject");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Dictionary<String,String>).FullName, Data = JsonUtility.ToJsonString(new Dictionary<String,String> { ["A"] = "one", ["B"] = "two" }, typeof(Dictionary<String,String>)) }, typeof(Dictionary<String,String>), "KnownType_Read_DictionaryStringString");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Dictionary<String,String?>).FullName, Data = JsonUtility.ToJsonString(new Dictionary<String,String?> { ["A"] = "one", ["B"] = null }, typeof(Dictionary<String,String?>)) }, typeof(Dictionary<String,String?>), "KnownType_Read_DictionaryStringNullableString");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Dictionary<String,Int32>).FullName, Data = JsonUtility.ToJsonString(new Dictionary<String,Int32> { ["A"] = 1, ["B"] = 2 }, typeof(Dictionary<String,Int32>)) }, typeof(Dictionary<String,Int32>), "KnownType_Read_DictionaryStringInt32");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Dictionary<String,Int32?>).FullName, Data = JsonUtility.ToJsonString(new Dictionary<String,Int32?> { ["A"] = 1, ["B"] = null }, typeof(Dictionary<String,Int32?>)) }, typeof(Dictionary<String,Int32?>), "KnownType_Read_DictionaryStringNullableInt32");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(SortedList<Int32,MSBuildItem>).FullName, Data = JsonUtility.ToJsonString(new SortedList<Int32,MSBuildItem> { [0] = new MSBuildItem("build") }, typeof(SortedList<Int32,MSBuildItem>)) }, typeof(SortedList<Int32,MSBuildItem>), "KnownType_Read_SortedListMSBuildItem");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(CommandWorkflow).FullName, Data = new CommandWorkflow().ToString() }, typeof(CommandWorkflow), "KnownType_Read_CommandWorkflow");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(CommandDetails).FullName, Data = JsonUtility.ToJsonString(new CommandDetails(handle: "H", arguments: new Dictionary<String,Object?> { ["A"] = 1 }, id: Guid.NewGuid(), workflow: new CommandWorkflow()), typeof(CommandDetails)) }, typeof(CommandDetails), "KnownType_Read_CommandDetails");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(CommandDescriptor).FullName, Data = JsonUtility.ToJsonString(new CommandDescriptor { Type = typeof(Tool000).FullName, Specifications = "spec", ExtendedData = new List<String> { "x" }, RegisteredHandles = new List<String> { "h" } }, typeof(CommandDescriptor)) }, typeof(CommandDescriptor), "KnownType_Read_CommandDescriptor");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ToolDescriptor).FullName, Data = JsonUtility.ToJsonString(new ToolDescriptor { ID = Guid.NewGuid(), Type = typeof(Tool000).FullName, Specifications = "spec", ExtendedData = new List<String> { "x" } }, typeof(ToolDescriptor)) }, typeof(ToolDescriptor), "KnownType_Read_ToolDescriptor");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(KusDepotCab).FullName, Data = JsonUtility.ToJsonString(new KusDepotCab { Type = typeof(StandardRequest).AssemblyQualifiedName, Data = JsonUtility.ToJsonString(new StandardRequest("cab"), typeof(StandardRequest)) }, typeof(KusDepotCab)) }, typeof(KusDepotCab), "KnownType_Read_KusDepotCab");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(Descriptor).FullName, Data = JsonUtility.ToJsonString(new Descriptor { Application = "App", ApplicationVersion = "1", Artist = "A", BornOn = "Now", Commands = new HashSet<CommandDescriptor>(), ContentStreamed = false, DataType = "Data", DistinguishedName = "DN", FILE = "file", ID = Guid.NewGuid(), LiveStream = false, Modified = "Now", Name = "Name", Notes = new HashSet<String>(), ObjectType = typeof(TextItem).FullName, Services = new HashSet<ToolDescriptor>(), ServiceVersion = "1", Size = "1", Tags = new HashSet<String>(), Title = "Title", Version = "1", Year = "2026" }, typeof(Descriptor)) }, typeof(Descriptor), "KnownType_Read_Descriptor");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ToolData).FullName, Data = JsonUtility.ToJsonString(new ToolData { Data = new Version(1, 2, 3, 4) }, typeof(ToolData)) }, typeof(ToolData), "KnownType_Read_ToolData");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ToolInput).FullName, Data = JsonUtility.ToJsonString(new ToolInput { Data = new Uri("https://example.org/") }, typeof(ToolInput)) }, typeof(ToolInput), "KnownType_Read_ToolInput");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ToolOutput).FullName, Data = JsonUtility.ToJsonString(new ToolOutput { ID = Guid.NewGuid(), Data = new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero) }, typeof(ToolOutput)) }, typeof(ToolOutput), "KnownType_Read_ToolOutput");

        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(TextItem).FullName, Data = new TextItem("hello").ToString() }, typeof(TextItem), "KnownType_Read_TextItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(CodeItem).FullName, Data = new CodeItem("code").ToString() }, typeof(CodeItem), "KnownType_Read_CodeItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(BinaryItem).FullName, Data = new BinaryItem(new Byte[] { 1, 2, 3 }).ToString() }, typeof(BinaryItem), "KnownType_Read_BinaryItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(MSBuildItem).FullName, Data = new MSBuildItem("build").ToString() }, typeof(MSBuildItem), "KnownType_Read_MSBuildItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(GenericItem).FullName, Data = new GenericItem(new Object[] { "nested", 42 }).ToString() }, typeof(GenericItem), "KnownType_Read_GenericItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(DataSetItem).FullName, Data = new DataSetItem(new DataItem[] { new TextItem("nested") }).ToString() }, typeof(DataSetItem), "KnownType_Read_DataSetItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(MultiMediaItem).FullName, Data = new MultiMediaItem().ToString() }, typeof(MultiMediaItem), "KnownType_Read_MultiMediaItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(GuidReferenceItem).FullName, Data = new GuidReferenceItem(Guid.NewGuid()).ToString() }, typeof(GuidReferenceItem), "KnownType_Read_GuidReferenceItem");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(KeySet).FullName, Data = new KeySet(new SecurityKey[] { new ServiceKey(Bytes(10), Guid.NewGuid()) }).ToString() }, typeof(KeySet), "KnownType_Read_KeySet");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(HostKey).FullName, Data = JsonUtility.ToJsonString(new HostKey(Bytes(1), Guid.NewGuid()), typeof(HostKey)) }, typeof(HostKey), "KnownType_Read_HostKey");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(TokenKey).FullName, Data = JsonUtility.ToJsonString(new TokenKey(new Byte[] { 1, 2, 3 }, Guid.NewGuid(), TokenKeyType.Jwt), typeof(TokenKey)) }, typeof(TokenKey), "KnownType_Read_TokenKey");
        using(var ownercert = CreateCertificate(Guid.NewGuid(), "Owner"))
        {
            yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(OwnerKey).FullName, Data = JsonUtility.ToJsonString(new OwnerKey(ownercert!, Guid.NewGuid()), typeof(OwnerKey)) }, typeof(OwnerKey), "KnownType_Read_OwnerKey");
        }
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(MyHostKey).FullName, Data = JsonUtility.ToJsonString(new MyHostKey(Bytes(2), Guid.NewGuid()), typeof(MyHostKey)) }, typeof(MyHostKey), "KnownType_Read_MyHostKey");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ClientKey).FullName, Data = JsonUtility.ToJsonString(new ClientKey(Bytes(4), Guid.NewGuid()), typeof(ClientKey)) }, typeof(ClientKey), "KnownType_Read_ClientKey");
        using(var managercert = CreateCertificate(Guid.NewGuid(), "Manager"))
        {
            yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ManagerKey).FullName, Data = JsonUtility.ToJsonString(new ManagerKey(managercert!, Guid.NewGuid()), typeof(ManagerKey)) }, typeof(ManagerKey), "KnownType_Read_ManagerKey");
        }
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ServiceKey).FullName, Data = JsonUtility.ToJsonString(new ServiceKey(Bytes(5), Guid.NewGuid()), typeof(ServiceKey)) }, typeof(ServiceKey), "KnownType_Read_ServiceKey");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(CommandKey).FullName, Data = JsonUtility.ToJsonString(new CommandKey(Bytes(6), Guid.NewGuid()), typeof(CommandKey)) }, typeof(CommandKey), "KnownType_Read_CommandKey");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ExecutiveKey).FullName, Data = JsonUtility.ToJsonString(new ExecutiveKey(Bytes(7), Guid.NewGuid()), typeof(ExecutiveKey)) }, typeof(ExecutiveKey), "KnownType_Read_ExecutiveKey");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ServiceRequest).FullName, Data = JsonUtility.ToJsonString(new ServiceRequest(null, "service-request"), typeof(ServiceRequest)) }, typeof(ServiceRequest), "KnownType_Read_ServiceRequest");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(StandardRequest).FullName, Data = JsonUtility.ToJsonString(new StandardRequest("standard-request"), typeof(StandardRequest)) }, typeof(StandardRequest), "KnownType_Read_StandardRequest");
        yield return ReadCase(new ToolValue { Mode = ToolValueMode.Custom, Type = typeof(ManagementRequest).FullName, Data = JsonUtility.ToJsonString(new ManagementRequest("management-request"), typeof(ManagementRequest)) }, typeof(ManagementRequest), "KnownType_Read_ManagementRequest");
    }

    private static TestCaseData ReadCase(ToolValue value , Type expectedType , String name , String? expectedBackType = null)
    {
        return new TestCaseData(value, expectedType, expectedBackType).SetName(name);
    }

    private static TestCaseData WriteCase(Object? value , Type expectedType , ToolValueMode expectedMode , String name)
    {
        return new TestCaseData(value, expectedType, expectedMode).SetName(name);
    }

    private static Byte[] Bytes(Byte seed)
    {
        var data = new Byte[16];
        for(Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return data;
    }
}
