namespace KusDepot.Exams.Tools;

public partial class CommandDetailsExam
{
    [Test]
    public void MyArguments_Setter_Materializes_Concrete_Collections_And_ToolPayloads()
    {
        Guid outputId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        DateTimeOffset stamp = new(2026, 3, 14, 0, 0, 0, TimeSpan.Zero);

        var dataItems = new HashSet<DataItem> { new TextItem("hello"), new BinaryItem(new Byte[] { 9, 8, 7 }) };

        var details = new CommandDetails(handle: "H", id: Guid.NewGuid(), workflow: new CommandWorkflow())
        {
            MyArguments = new Dictionary<String,ToolValue?>
            {
                ["Version"] = new ToolValue
                {
                    Mode = ToolValueMode.Parse,
                    Type = typeof(Version).FullName,
                    Data = "1.2.3.4"
                },
                ["Uri"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(Uri).FullName,
                    Data = JsonUtility.ToJsonString(new Uri("https://example.org/llm"), typeof(Uri))
                },
                ["ListString"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(List<String>).FullName,
                    Data = JsonUtility.ToJsonString(new List<String> { "one", "two", "three" }, typeof(List<String>))
                },
                ["QueueString"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(Queue<String>).FullName,
                    Data = JsonUtility.ToJsonString(new Queue<String>(new[] { "first", "second" }), typeof(Queue<String>))
                },
                ["DictionaryStringInt32"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(Dictionary<String,Int32>).FullName,
                    Data = JsonUtility.ToJsonString(new Dictionary<String,Int32> { ["A"] = 1, ["B"] = 2 }, typeof(Dictionary<String,Int32>))
                },
                ["HashSetDataItem"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(HashSet<DataItem>).FullName,
                    Data = JsonUtility.ToJsonString(dataItems, typeof(HashSet<DataItem>))
                },
                ["ToolData"] = ToolValueConverter.ToToolValue(new ToolData { Data = new Version(1, 2, 3, 4) }),
                ["ToolInput"] = ToolValueConverter.ToToolValue(new ToolInput { Data = new Uri("https://example.org/input") }),
                ["ToolOutput"] = ToolValueConverter.ToToolValue(new ToolOutput { ID = outputId, Data = stamp }),
                ["Configuration"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(IConfiguration).FullName,
                    Data = "{\"Alpha\":\"One\",\"Section:Beta\":\"Two\"}"
                }
            }
        };

        Check.That(details.Arguments).IsNotNull();
        Check.That(details.Arguments!["Version"]).IsInstanceOf<Version>();
        Check.That(((Version)details.Arguments["Version"]!).ToString()).IsEqualTo("1.2.3.4");

        Check.That(details.Arguments["Uri"]).IsInstanceOf<Uri>();
        Check.That(details.Arguments["Uri"]!.ToString()).IsEqualTo("https://example.org/llm");

        Check.That(details.Arguments["ListString"]).IsInstanceOf<List<String>>();
        Check.That(((List<String>)details.Arguments["ListString"]!).SequenceEqual(new[] { "one", "two", "three" })).IsTrue();

        Check.That(details.Arguments["QueueString"]).IsInstanceOf<Queue<String>>();
        Check.That(((Queue<String>)details.Arguments["QueueString"]!).SequenceEqual(new[] { "first", "second" })).IsTrue();

        Check.That(details.Arguments["DictionaryStringInt32"]).IsInstanceOf<Dictionary<String,Int32>>();
        Dictionary<String,Int32> numbers = (Dictionary<String,Int32>)details.Arguments["DictionaryStringInt32"]!;
        Check.That(numbers["A"]).IsEqualTo(1);
        Check.That(numbers["B"]).IsEqualTo(2);

        Check.That(details.Arguments["HashSetDataItem"]).IsInstanceOf<HashSet<DataItem>>();
        HashSet<DataItem> restoredItems = (HashSet<DataItem>)details.Arguments["HashSetDataItem"]!;
        Check.That(restoredItems.Any(_ => _ is TextItem text && text.GetContent() == "hello")).IsTrue();
        Check.That(restoredItems.Any(_ => _ is BinaryItem binary && binary.GetContent()!.SequenceEqual(new Byte[] { 9, 8, 7 }))).IsTrue();

        Check.That(details.Arguments["ToolData"]).IsInstanceOf<ToolData>();
        Check.That(((ToolData)details.Arguments["ToolData"]!).Data).IsInstanceOf<Version>();

        Check.That(details.Arguments["ToolInput"]).IsInstanceOf<ToolInput>();
        Check.That(((ToolInput)details.Arguments["ToolInput"]!).Data).IsInstanceOf<Uri>();
        Check.That(((ToolInput)details.Arguments["ToolInput"]!).Data!.ToString()).IsEqualTo("https://example.org/input");

        Check.That(details.Arguments["ToolOutput"]).IsInstanceOf<ToolOutput>();
        Check.That(((ToolOutput)details.Arguments["ToolOutput"]!).ID).IsEqualTo(outputId);
        Check.That(((ToolOutput)details.Arguments["ToolOutput"]!).Data).IsInstanceOf<DateTimeOffset>();
        Check.That((DateTimeOffset)((ToolOutput)details.Arguments["ToolOutput"]!).Data!).IsEqualTo(stamp);

        Check.That(details.Arguments["Configuration"]).IsInstanceOf<ConfigurationManager>();
        var config = (IConfiguration)details.Arguments["Configuration"]!;
        Check.That(config["Alpha"]).IsEqualTo("One");
        Check.That(config["Section:Beta"]).IsEqualTo("Two");
    }

    [Test]
    public void JsonUtility_Parse_Materializes_CommandDetails_Input()
    {
        Guid outputId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        DateTimeOffset stamp = new(2026, 4, 1, 12, 30, 0, TimeSpan.Zero);

        var source = new CommandDetails(handle: "H", id: Guid.NewGuid(), workflow: new CommandWorkflow())
        {
            MyArguments = new Dictionary<String,ToolValue?>
            {
                ["Version"] = new ToolValue
                {
                    Mode = ToolValueMode.Parse,
                    Type = typeof(Version).FullName,
                    Data = "9.8.7.6"
                },
                ["QueueString"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(Queue<String>).FullName,
                    Data = JsonUtility.ToJsonString(new Queue<String>(new[] { "alpha", "beta" }), typeof(Queue<String>))
                },
                ["DictionaryStringInt32"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(Dictionary<String,Int32>).FullName,
                    Data = JsonUtility.ToJsonString(new Dictionary<String,Int32> { ["X"] = 10, ["Y"] = 20 }, typeof(Dictionary<String,Int32>))
                },
                ["HashSetDataItem"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(HashSet<DataItem>).FullName,
                    Data = JsonUtility.ToJsonString(new HashSet<DataItem> { new TextItem("world"), new GuidReferenceItem(Guid.Parse("88888888-8888-8888-8888-888888888888")) }, typeof(HashSet<DataItem>))
                },
                ["ToolInput"] = ToolValueConverter.ToToolValue(new ToolInput { Data = new Uri("https://example.org/json") }),
                ["ToolOutput"] = ToolValueConverter.ToToolValue(new ToolOutput { ID = outputId, Data = stamp })
            }
        };

        String json = JsonUtility.ToJsonString(source, typeof(CommandDetails));
        CommandDetails? roundtrip = JsonUtility.Parse<CommandDetails>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Arguments).IsNotNull();
        Check.That(roundtrip.Arguments!["Version"]).IsInstanceOf<Version>();
        Check.That(((Version)roundtrip.Arguments["Version"]!).ToString()).IsEqualTo("9.8.7.6");

        Check.That(roundtrip.Arguments["QueueString"]).IsInstanceOf<Queue<String>>();
        Check.That(((Queue<String>)roundtrip.Arguments["QueueString"]!).SequenceEqual(new[] { "alpha", "beta" })).IsTrue();

        Check.That(roundtrip.Arguments["DictionaryStringInt32"]).IsInstanceOf<Dictionary<String,Int32>>();
        Dictionary<String,Int32> numbers = (Dictionary<String,Int32>)roundtrip.Arguments["DictionaryStringInt32"]!;
        Check.That(numbers["X"]).IsEqualTo(10);
        Check.That(numbers["Y"]).IsEqualTo(20);

        Check.That(roundtrip.Arguments["HashSetDataItem"]).IsInstanceOf<HashSet<DataItem>>();
        HashSet<DataItem> restoredItems = (HashSet<DataItem>)roundtrip.Arguments["HashSetDataItem"]!;
        Check.That(restoredItems.Any(_ => _ is TextItem text && text.GetContent() == "world")).IsTrue();
        Check.That(restoredItems.Any(_ => _ is GuidReferenceItem reference && reference.GetContent() == Guid.Parse("88888888-8888-8888-8888-888888888888"))).IsTrue();

        Check.That(roundtrip.Arguments["ToolInput"]).IsInstanceOf<ToolInput>();
        Check.That(((ToolInput)roundtrip.Arguments["ToolInput"]!).Data).IsInstanceOf<Uri>();
        Check.That(((ToolInput)roundtrip.Arguments["ToolInput"]!).Data!.ToString()).IsEqualTo("https://example.org/json");

        Check.That(roundtrip.Arguments["ToolOutput"]).IsInstanceOf<ToolOutput>();
        Check.That(((ToolOutput)roundtrip.Arguments["ToolOutput"]!).ID).IsEqualTo(outputId);
        Check.That(((ToolOutput)roundtrip.Arguments["ToolOutput"]!).Data).IsInstanceOf<DateTimeOffset>();
        Check.That((DateTimeOffset)((ToolOutput)roundtrip.Arguments["ToolOutput"]!).Data!).IsEqualTo(stamp);

        Check.That(roundtrip.MyArguments).IsNotNull();
        Check.That(roundtrip.MyArguments!["QueueString"]!.Type).IsEqualTo(typeof(Queue<String>).FullName);
        Check.That(roundtrip.MyArguments["DictionaryStringInt32"]!.Type).IsEqualTo(typeof(Dictionary<String,Int32>).FullName);
        Check.That(roundtrip.MyArguments["HashSetDataItem"]!.Type).IsEqualTo(typeof(HashSet<DataItem>).FullName);
        Check.That(roundtrip.Workflow).IsNotNull();
        Check.That(roundtrip.Workflow!.Details).IsSameReferenceAs(roundtrip);
    }
}
