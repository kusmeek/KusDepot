namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class CommandDetailsExam
{
    [Test]
    public void Constructor()
    {
        CommandDetails _ = new()
        {
            Arguments = new()
            {
                ["Int"] = 123,
                ["String"] = "Test",
                ["List"] = new List<Object?> {1,"two",3.0 },
                ["Stack"] = new Stack<Object?>(new Object?[] {1,"two",3.0}),
                ["Queue"] = new Queue<Object?>(new Object?[] {1,"two",3.0}),
                ["Dictionary"] = new Dictionary<String,Object?> { ["KeyM"] = "Value" }
            },
            Handle = "HandleTest",
            ID = Guid.NewGuid()
        };

        DataContractSerializer _s = new DataContractSerializer(typeof(CommandDetails));

        using MemoryStream _m = new MemoryStream(); _s.WriteObject(_m, _); _m.Position = 0;

        CommandDetails? __ = _s.ReadObject(_m) as CommandDetails;

        Check.That(_.Handle).IsEqualTo(__!.Handle);
        Check.That(_.ID).IsEqualTo(__.ID);

        Check.That(( Int32 )_!.Arguments!["Int"]!).IsEqualTo(( Int32 )__!.Arguments!["Int"]!);
        Check.That(( String? )_.Arguments["String"]).IsEqualTo(( String? )__.Arguments["String"]);

        List<Object?>? _ol = (List<Object?>?)_.Arguments["List"];
        List<Object?>? _dl = (List<Object?>?)__.Arguments["List"];
        Check.That(_ol).ContainsExactly(_dl);

        Stack<Object?>? _os = (Stack<Object?>?)_.Arguments["Stack"];
        Stack<Object?>? _ds = (Stack<Object?>?)__.Arguments["Stack"];
        Check.That(_os!.ElementAt(0)).IsEqualTo(_ds!.ElementAt(0));
        Check.That(_os!.ElementAt(1)).IsEqualTo(_ds!.ElementAt(1));
        Check.That(_os!.ElementAt(2)).IsEqualTo(_ds!.ElementAt(2));

        Queue<Object?>? _oq = (Queue<Object?>?)_.Arguments["Queue"];
        Queue<Object?>? _dq = (Queue<Object?>?)__.Arguments["Queue"];
        Check.That(_oq!.ElementAt(0)).IsEqualTo(_dq!.ElementAt(0));
        Check.That(_oq!.ElementAt(1)).IsEqualTo(_dq!.ElementAt(1));
        Check.That(_oq!.ElementAt(2)).IsEqualTo(_dq!.ElementAt(2));

        Dictionary<String,Object?>? _od = (Dictionary<String,Object?>?)_.Arguments["Dictionary"];
        Dictionary<String,Object?>? _dd = (Dictionary<String,Object?>?)__.Arguments["Dictionary"];
        Check.That(_od!["KeyM"]).IsEqualTo(_dd!["KeyM"]);
    }

    [Test]
    public void Create()
    {
        String? handle = "TestHandle";
        Guid? id = Guid.NewGuid();
        Dictionary<String,Object?> arguments = new() { { "TestKey" , "TestValue" } };

        CommandDetails details = CommandDetails.Create(handle,arguments,id);
        Check.That(details.Handle).IsEqualTo(handle);
        Check.That(details.ID).IsEqualTo(id);
        Check.That(details.Arguments).ContainsPair("TestKey","TestValue");

        CommandDetails detailsHandle = CommandDetails.Create(handle: handle);
        Check.That(detailsHandle.Handle).IsEqualTo(handle);
        Check.That(detailsHandle.ID).IsNull();
        Check.That(detailsHandle.Arguments).IsNull();

        CommandDetails detailsId = CommandDetails.Create(id: id);
        Check.That(detailsId.Handle).IsNull();
        Check.That(detailsId.ID).IsEqualTo(id);
        Check.That(detailsId.Arguments).IsNull();

        CommandDetails detailsArguments = CommandDetails.Create(arguments: arguments);
        Check.That(detailsArguments.Handle).IsNull();
        Check.That(detailsArguments.ID).IsNull();
        Check.That(detailsArguments.Arguments).ContainsPair("TestKey","TestValue");

        CommandDetails detailsDefault = CommandDetails.Create();
        Check.That(detailsDefault.Handle).IsNull();
        Check.That(detailsDefault.ID).IsNull();
        Check.That(detailsDefault.Arguments).IsNull();

        var wf = BuildWorkflow(10);
        CommandDetails detailsWorkflow = CommandDetails.Create(handle: "H", workflow: wf);
        Check.That(detailsWorkflow.Workflow).IsNotNull();
        Check.That(detailsWorkflow.Workflow!.Details).IsSameReferenceAs(detailsWorkflow);
        Check.That(detailsWorkflow.Workflow!.Sequence).IsNotNull();
        Check.That(detailsWorkflow.Workflow!.Sequence!.Count).IsEqualTo(10);
    }

    [Test]
    public void Properties_WriteOnce()
    {
        var d = new CommandDetails();

        var g1 = Guid.NewGuid(); var g2 = Guid.NewGuid();
        d.ID = g1; d.ID = g2; Check.That(d.ID).IsEqualTo(g1);

        d.Handle = "A"; d.Handle = "B"; Check.That(d.Handle).IsEqualTo("A");

        var a1 = new Dictionary<String, Object?> { ["k"] = "v" };
        var a2 = new Dictionary<String, Object?> { ["k2"] = "v2" };
        d.Arguments = a1; d.Arguments = a2; Check.That(d.Arguments).IsSameReferenceAs(a1);

        var w1 = new CommandWorkflow(); var w2 = new CommandWorkflow();
        d.Workflow = w1; d.Workflow = w2; Check.That(d.Workflow).IsSameReferenceAs(w1);
    }

    [Test]
    public void Constructor_With_Workflow()
    {
        var details = new CommandDetails(handle: "H", id: Guid.NewGuid(), workflow: new CommandWorkflow());
        Check.That(details.Workflow).IsNotNull();
        Check.That(details.Workflow!.Details).IsSameReferenceAs(details);
    }

    [Test]
    public void MakeReady()
    {
        var tool = new Tool();
        var toolId = tool.GetID();

        var details = new CommandDetails(handle: "H");
        var ok = details.MakeReady(tool);

        Check.That(ok).IsTrue();
        Check.That(details.ID).IsNotNull();
        Check.That(details.Arguments).IsNotNull();
        Check.That(details.Arguments!.ContainsKey("!AttachedToolID")).IsTrue();
        Check.That(details.Arguments["!AttachedToolID"]).IsEqualTo(toolId);
    }

    [Test]
    public void ToString_Parse()
    {
        var details = new CommandDetails(handle: "H", id: Guid.NewGuid(), workflow: new CommandWorkflow());
        details.Workflow!.Sequence!.Add(0, "H");
        details.Workflow!.LogEvent("S", "D");

        var s = details.ToString();
        var d2 = CommandDetails.Parse(s, null)!;

        Check.That(d2).IsNotNull();
        Check.That(d2.Workflow).IsNotNull();
        Check.That(d2.Workflow!.Details).IsSameReferenceAs(d2);
        Check.That(d2.Workflow!.EventLog).IsNotNull();
        Check.That(d2.Workflow!.EventLog!.Count).IsEqualTo(1);
        Check.That(d2.Workflow!.Sequence).IsNotNull();
        Check.That(d2.Workflow!.Sequence!.Count).IsEqualTo(1);
        Check.That(d2.Workflow!.Sequence![0]).IsEqualTo("H");
    }

    [Test]
    public void KusDepotCab_Serialization()
    {
        var id = Guid.NewGuid();
        var wf = BuildWorkflow(200);
        wf.LogEvent("Action1", "Data1");
        var details = new CommandDetails(
            arguments: new Dictionary<String,Object?> { ["X"] = "Y" },
            handle: "H",
            id: id,
            workflow: wf
        );

        var cab = details.ToKusDepotCab();
        Check.That(cab).IsNotNull();
        Check.That(cab!.Type).IsEqualTo(typeof(CommandDetails).FullName);
        Check.That(cab.Data).IsNotNull();

        var json = cab.ToString();
        var cab2 = KusDepotCab.Parse(json)!;
        Check.That(cab2).IsNotNull();

        var restored = cab2.GetObject<CommandDetails>();
        Check.That(restored).IsNotNull();
        Check.That(restored!.Handle).IsEqualTo("H");
        Check.That(restored.ID).IsEqualTo(id);
        Check.That(restored.Arguments).IsNotNull();
        Check.That((String?)restored.Arguments!["X"]).IsEqualTo("Y");
        Check.That(restored.Workflow).IsNotNull();
        Check.That(restored.Workflow!.Details).IsSameReferenceAs(restored);
        Check.That(restored.Workflow!.Sequence).IsNotNull();
        Check.That(restored.Workflow!.Sequence!.Count).IsEqualTo(200);
        Check.That(restored.Workflow!.EventLog).IsNotNull();
        Check.That(restored.Workflow!.EventLog!.Count).IsEqualTo(1);
    }

    [Test]
    public void MyArguments_Getter_Projects_Framework_Types_To_ToolValues()
    {
        var details = new CommandDetails(
            arguments: new Dictionary<String,Object?>
            {
                ["Version"] = new Version(1, 2, 3, 4),
                ["Uri"] = new Uri("https://example.org/"),
                ["Builder"] = new StringBuilder("builder-text"),
                ["Configuration"] = new ConfigurationManager().AddInMemoryCollection(new Dictionary<String,String?>
                {
                    ["Alpha"] = "One",
                    ["Section:Beta"] = "Two"
                }).Build(),
                ["Unhandled"] = new Object()
            },
            handle: "H",
            id: Guid.NewGuid(),
            workflow: new CommandWorkflow());

        Dictionary<String,ToolValue?>? projection = details.MyArguments;

        Check.That(projection).IsNotNull();
        Check.That(projection!["Version"]).IsNotNull();
        Check.That(projection["Version"]!.Mode).IsEqualTo(ToolValueMode.Parse);
        Check.That(projection["Version"]!.Type).IsEqualTo(typeof(Version).FullName);

        Check.That(projection["Uri"]).IsNotNull();
        Check.That(projection["Uri"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(projection["Uri"]!.Data).IsEqualTo("\"https://example.org/\"");

        Check.That(projection["Builder"]).IsNotNull();
        Check.That(projection["Builder"]!.Mode).IsEqualTo(ToolValueMode.Custom);

        Check.That(projection["Configuration"]).IsNotNull();
        Check.That(projection["Configuration"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(projection["Configuration"]!.Type).IsEqualTo(typeof(IConfiguration).FullName);
        Check.That(projection["Configuration"]!.Data).Contains("\"Alpha\":\"One\"");

        Check.That(projection["Unhandled"]).IsNotNull();
        Check.That(projection["Unhandled"]!.Mode).IsEqualTo(ToolValueMode.Unhandled);
        Check.That(projection["Unhandled"]!.Type).IsEqualTo(typeof(Object).FullName);
    }

    [Test]
    public void MyArguments_Setter_Materializes_Framework_Types_From_ToolValues()
    {
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
                    Mode = ToolValueMode.Build,
                    Type = typeof(Uri).FullName,
                    Arguments =
                    [
                        new ToolValueArgument
                        {
                            Index = 0,
                            Mode = ToolValueMode.Parse,
                            Type = typeof(String).FullName,
                            Data = "https://example.org/"
                        }
                    ]
                },
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
        Check.That(details.Arguments["Uri"]!.ToString()).IsEqualTo("https://example.org/");

        Check.That(details.Arguments["Configuration"]).IsInstanceOf<ConfigurationManager>();
        var config = (IConfiguration)details.Arguments["Configuration"]!;
        Check.That(config["Alpha"]).IsEqualTo("One");
        Check.That(config["Section:Beta"]).IsEqualTo("Two");
    }

    [Test]
    public void MyArguments_Setter_Preserves_OneShot_Runtime_Assignment()
    {
        var original = new Dictionary<String,Object?> { ["Existing"] = 123 };

        var details = new CommandDetails(arguments: original)
        {
            MyArguments = new Dictionary<String,ToolValue?>
            {
                ["Version"] = new ToolValue
                {
                    Mode = ToolValueMode.Parse,
                    Type = typeof(Version).FullName,
                    Data = "1.2.3.4"
                }
            }
        };

        Check.That(details.Arguments).IsSameReferenceAs(original);
        Check.That(details.Arguments!.ContainsKey("Existing")).IsTrue();
        Check.That(details.Arguments.ContainsKey("Version")).IsFalse();
    }

    [Test]
    public void ToString_Parse_Roundtrip_Preserves_MyArguments_Framework_Types()
    {
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
                    Mode = ToolValueMode.Build,
                    Type = typeof(Uri).FullName,
                    Arguments =
                    [
                        new ToolValueArgument
                        {
                            Index = 0,
                            Mode = ToolValueMode.Parse,
                            Type = typeof(String).FullName,
                            Data = "https://example.org/"
                        }
                    ]
                }
            }
        };

        var s = details.ToString();
        var d2 = CommandDetails.Parse(s, null);

        Check.That(d2).IsNotNull();
        Check.That(d2!.Arguments).IsNotNull();
        Check.That(d2.Arguments!["Version"]).IsInstanceOf<Version>();
        Check.That(((Version)d2.Arguments["Version"]!).ToString()).IsEqualTo("1.2.3.4");
        Check.That(d2.Arguments["Uri"]).IsInstanceOf<Uri>();
        Check.That(d2.Arguments["Uri"]!.ToString()).IsEqualTo("https://example.org/");
        Check.That(d2.MyArguments).IsNotNull();
        Check.That(d2.MyArguments!["Version"]!.Mode).IsEqualTo(ToolValueMode.Parse);
        Check.That(d2.MyArguments["Uri"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(d2.Workflow).IsNotNull();
        Check.That(d2.Workflow!.Details).IsSameReferenceAs(d2);
    }

    [Test]
    public void MyArguments_Getter_Projects_Tool_Transport_Types()
    {
        var rawToolValue = new ToolValue
        {
            Mode = ToolValueMode.Custom,
            Type = typeof(IConfiguration).FullName,
            Data = "{\"Alpha\":\"One\"}"
        };

        var details = new CommandDetails(
            arguments: new Dictionary<String,Object?>
            {
                ["ToolData"] = new ToolData { Data = new Version(1, 2, 3, 4) },
                ["ToolInput"] = new ToolInput { Data = new Uri("https://example.org/") },
                ["ToolOutput"] = new ToolOutput { ID = Guid.Parse("55555555-5555-5555-5555-555555555555"), Data = new StringBuilder("builder-text") },
                ["ToolValue"] = rawToolValue
            },
            handle: "H",
            id: Guid.NewGuid(),
            workflow: new CommandWorkflow());

        Dictionary<String,ToolValue?>? projection = details.MyArguments;

        Check.That(projection).IsNotNull();

        Check.That(projection!["ToolData"]).IsNotNull();
        Check.That(projection["ToolData"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(projection["ToolData"]!.Type).IsEqualTo(typeof(ToolData).FullName);

        Check.That(projection["ToolInput"]).IsNotNull();
        Check.That(projection["ToolInput"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(projection["ToolInput"]!.Type).IsEqualTo(typeof(ToolInput).FullName);

        Check.That(projection["ToolOutput"]).IsNotNull();
        Check.That(projection["ToolOutput"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(projection["ToolOutput"]!.Type).IsEqualTo(typeof(ToolOutput).FullName);

        Check.That(projection["ToolValue"]).IsSameReferenceAs(rawToolValue);
        Check.That(projection["ToolValue"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(projection["ToolValue"]!.Data).IsEqualTo("{\"Alpha\":\"One\"}");
    }

    [Test]
    public void ToString_Parse_Roundtrip_Preserves_Tool_Transport_Arguments()
    {
        var details = new CommandDetails(
            arguments: new Dictionary<String,Object?>
            {
                ["ToolData"] = new ToolData { Data = new Version(1, 2, 3, 4) },
                ["ToolInput"] = new ToolInput { Data = new Uri("https://example.org/") },
                ["ToolOutput"] = new ToolOutput { ID = Guid.Parse("55555555-5555-5555-5555-555555555555"), Data = new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero) }
            },
            handle: "H",
            id: Guid.NewGuid(),
            workflow: new CommandWorkflow());

        var s = details.ToString();
        var d2 = CommandDetails.Parse(s, null);

        Check.That(d2).IsNotNull();
        Check.That(d2!.Arguments).IsNotNull();
        Check.That(d2.Arguments!["ToolData"]).IsInstanceOf<ToolData>();
        Check.That(((ToolData)d2.Arguments["ToolData"]!).Data).IsInstanceOf<Version>();
        Check.That(((Version)((ToolData)d2.Arguments["ToolData"]!).Data!).ToString()).IsEqualTo("1.2.3.4");

        Check.That(d2.Arguments["ToolInput"]).IsInstanceOf<ToolInput>();
        Check.That(((ToolInput)d2.Arguments["ToolInput"]!).Data).IsInstanceOf<Uri>();
        Check.That(((ToolInput)d2.Arguments["ToolInput"]!).Data!.ToString()).IsEqualTo("https://example.org/");

        Check.That(d2.Arguments["ToolOutput"]).IsInstanceOf<ToolOutput>();
        Check.That(((ToolOutput)d2.Arguments["ToolOutput"]!).Data).IsInstanceOf<DateTimeOffset>();
        Check.That((DateTimeOffset)((ToolOutput)d2.Arguments["ToolOutput"]!).Data!).IsEqualTo(new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero));

        Check.That(d2.MyArguments).IsNotNull();
        Check.That(d2.MyArguments!["ToolData"]!.Type).IsEqualTo(typeof(ToolData).FullName);
        Check.That(d2.MyArguments["ToolInput"]!.Type).IsEqualTo(typeof(ToolInput).FullName);
        Check.That(d2.MyArguments["ToolOutput"]!.Type).IsEqualTo(typeof(ToolOutput).FullName);
    }

    [Test]
    public void JsonUtility_Roundtrip_Preserves_MyArguments_Framework_Types()
    {
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
                    Mode = ToolValueMode.Build,
                    Type = typeof(Uri).FullName,
                    Arguments =
                    [
                        new ToolValueArgument
                        {
                            Index = 0,
                            Mode = ToolValueMode.Parse,
                            Type = typeof(String).FullName,
                            Data = "https://example.org/"
                        }
                    ]
                },
                ["Configuration"] = new ToolValue
                {
                    Mode = ToolValueMode.Custom,
                    Type = typeof(IConfiguration).FullName,
                    Data = "{\"Alpha\":\"One\",\"Section:Beta\":\"Two\"}"
                }
            }
        };

        var json = JsonUtility.ToJsonString(details, typeof(CommandDetails));
        var roundtrip = JsonUtility.Parse<CommandDetails>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Arguments).IsNotNull();
        Check.That(roundtrip.Arguments!["Version"]).IsInstanceOf<Version>();
        Check.That(((Version)roundtrip.Arguments["Version"]!).ToString()).IsEqualTo("1.2.3.4");
        Check.That(roundtrip.Arguments["Uri"]).IsInstanceOf<Uri>();
        Check.That(roundtrip.Arguments["Uri"]!.ToString()).IsEqualTo("https://example.org/");
        Check.That(roundtrip.Arguments["Configuration"]).IsInstanceOf<ConfigurationManager>();
        var config = (IConfiguration)roundtrip.Arguments["Configuration"]!;
        Check.That(config["Alpha"]).IsEqualTo("One");
        Check.That(config["Section:Beta"]).IsEqualTo("Two");
        Check.That(roundtrip.MyArguments).IsNotNull();
        Check.That(roundtrip.Workflow).IsNotNull();
        Check.That(roundtrip.Workflow!.Details).IsSameReferenceAs(roundtrip);
    }

    [Test]
    public void JsonUtility_Roundtrip_Preserves_Tool_Transport_Arguments()
    {
        var rawToolValue = new ToolValue
        {
            Mode = ToolValueMode.Custom,
            Type = typeof(IConfiguration).FullName,
            Data = "{\"Alpha\":\"One\"}"
        };

        var details = new CommandDetails(
            arguments: new Dictionary<String,Object?>
            {
                ["ToolData"] = new ToolData { Data = new Version(1, 2, 3, 4) },
                ["ToolInput"] = new ToolInput { Data = new Uri("https://example.org/") },
                ["ToolOutput"] = new ToolOutput { ID = Guid.Parse("55555555-5555-5555-5555-555555555555"), Data = new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero) },
                ["ToolValue"] = rawToolValue
            },
            handle: "H",
            id: Guid.NewGuid(),
            workflow: new CommandWorkflow());

        var json = JsonUtility.ToJsonString(details, typeof(CommandDetails));
        var roundtrip = JsonUtility.Parse<CommandDetails>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Arguments).IsNotNull();
        Check.That(roundtrip.Arguments!["ToolData"]).IsInstanceOf<ToolData>();
        Check.That(((ToolData)roundtrip.Arguments["ToolData"]!).Data).IsInstanceOf<Version>();
        Check.That(((Version)((ToolData)roundtrip.Arguments["ToolData"]!).Data!).ToString()).IsEqualTo("1.2.3.4");
        Check.That(roundtrip.Arguments["ToolInput"]).IsInstanceOf<ToolInput>();
        Check.That(((ToolInput)roundtrip.Arguments["ToolInput"]!).Data).IsInstanceOf<Uri>();
        Check.That(((ToolInput)roundtrip.Arguments["ToolInput"]!).Data!.ToString()).IsEqualTo("https://example.org/");
        Check.That(roundtrip.Arguments["ToolOutput"]).IsInstanceOf<ToolOutput>();
        Check.That(((ToolOutput)roundtrip.Arguments["ToolOutput"]!).Data).IsInstanceOf<DateTimeOffset>();
        Check.That((DateTimeOffset)((ToolOutput)roundtrip.Arguments["ToolOutput"]!).Data!).IsEqualTo(new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero));
        Check.That(roundtrip.Arguments["ToolValue"]).IsInstanceOf<ConfigurationManager>();
        var config = (IConfiguration)roundtrip.Arguments["ToolValue"]!;
        Check.That(config["Alpha"]).IsEqualTo("One");
        Check.That(roundtrip.MyArguments).IsNotNull();
        Check.That(roundtrip.MyArguments!["ToolData"]!.Type).IsEqualTo(typeof(ToolData).FullName);
        Check.That(roundtrip.MyArguments["ToolInput"]!.Type).IsEqualTo(typeof(ToolInput).FullName);
        Check.That(roundtrip.MyArguments["ToolOutput"]!.Type).IsEqualTo(typeof(ToolOutput).FullName);
        Check.That(roundtrip.MyArguments["ToolValue"]!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(roundtrip.MyArguments["ToolValue"]!.Type).IsEqualTo(typeof(IConfiguration).FullName);
        Check.That(roundtrip.Workflow).IsNotNull();
        Check.That(roundtrip.Workflow!.Details).IsSameReferenceAs(roundtrip);
    }

    private static CommandWorkflow BuildWorkflow(Int32 count)
    {
        var wf = new CommandWorkflow();
        wf.Sequence ??= new SortedList<Int32, String>(); if(count <= 0) return wf;
        wf.Sequence[0] = String.Concat(Enumerable.Range(0, count).Select(i => i.ToString()));
        for (Int32 i = 1; i < count; i++)
        {
            wf.Sequence[i] = $"S{i}";
        }
        return wf;
    }
}