namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class CommandDetailsExam
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
        Check.That(tool.SetID(toolId)).IsTrue();

        var details = new CommandDetails(handle: "H");
        var ok = details.MakeReady(tool);

        Check.That(ok).IsTrue();
        Check.That(details.ID).IsNotNull();
        Check.That(details.Arguments).IsNotNull();
        Check.That(details.Arguments!.ContainsKey("AttachedToolID")).IsTrue();
        Check.That(details.Arguments["AttachedToolID"]).IsEqualTo(toolId);
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
    public void Web_ToCommandDetails()
    {
        var count = 25;
        var web = new CommandDetailsWeb
        {
            Arguments = new Dictionary<String, String?> { ["X"] = "Y" },
            Handle = "H",
            ID = Guid.NewGuid(),
            Workflow = BuildWorkflow(count)
        };

        var details = web.ToCommandDetails();
        Check.That(details.Workflow).IsNotNull();
        Check.That(details.Workflow!.Details).IsSameReferenceAs(details);
        Check.That(details.Arguments).IsNotNull();
        Check.That(details.Arguments!.ContainsKey("X")).IsTrue();
        Check.That((String?)details.Arguments!["X"]).IsEqualTo("Y");
        Check.That(details.Workflow!.Sequence).IsNotNull();
        Check.That(details.Workflow!.Sequence!.Count).IsEqualTo(count);
        Check.That(details.Workflow!.Sequence![1]).IsEqualTo("S1");
        Check.That(details.Workflow!.Sequence![24]).IsEqualTo("S24");
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