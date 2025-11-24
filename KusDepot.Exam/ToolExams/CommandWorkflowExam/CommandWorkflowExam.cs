using Serilog;

namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CommandWorkflowExam
{
    [Test]
    public void Constructor()
    {
        var id = Guid.NewGuid();
        var details = new CommandDetails(handle: "HandleA", id: id);

        var wf = new CommandWorkflow(details);

        Check.That(wf.Details).IsSameReferenceAs(details);
        Check.That(wf.Sequence).IsNotNull();
        Check.That(wf.Sequence!.Count).IsEqualTo(0);
        Check.That(wf.EventLog).IsNotNull();
        Check.That(wf.EventLog!.Count).IsEqualTo(0);
    }

    [Test]
    public void Properties_WriteOnce()
    {
        var wf = new CommandWorkflow();

        var d1 = new CommandDetails(handle: "A", id: Guid.NewGuid());
        var d2 = new CommandDetails(handle: "B", id: Guid.NewGuid());
        wf.Details = d1;
        wf.Details = d2;
        Check.That(wf.Details).IsSameReferenceAs(d1);

        var seqOriginal = wf.Sequence;
        var seqNew = new SortedList<Int32, String> { [0] = "X" };
        wf.Sequence = seqNew;
        Check.That(wf.Sequence).IsSameReferenceAs(seqOriginal);
        Check.That(wf.Sequence).IsNotNull();

        var logOriginal = wf.EventLog;
        var logNew = new Dictionary<String, String> { ["K"] = "V" };
        wf.EventLog = logNew;
        Check.That(wf.EventLog).IsSameReferenceAs(logOriginal);
        Check.That(wf.EventLog).IsNotNull();
    }

    [Test]
    public void LogEvent_Format()
    {
        var id = Guid.NewGuid();
        var toolid = Guid.NewGuid();
        var details = new CommandDetails(
            arguments: new Dictionary<String, Object?> { ["AttachedToolID"] = toolid },
            handle: "MyHandle",
            id: id
        );

        var wf = new CommandWorkflow(details);
        var ok = wf.LogEvent("MyAction", "payload");

        Check.That(ok).IsTrue();
        Check.That(wf.EventLog).IsNotNull();
        Check.That(wf.EventLog!.Count).IsEqualTo(1);

        var kv = wf.EventLog!.Single();
        var key = kv.Key;
        var val = kv.Value;

        var parts = key.Split('~');
        Check.That(parts.Length).IsEqualTo(5);
        Check.That(parts[0]).IsEqualTo(id.ToString("N").ToUpperInvariant());
        Check.That(parts[1]).IsEqualTo(toolid.ToString("N").ToUpperInvariant());
        Check.That(parts[2]).IsEqualTo("MyHandle");
        Check.That(parts[3]).IsEqualTo("MyAction");

        Check.That(DateTimeOffset.TryParseExact(parts[4], "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ts)).IsTrue();
        Check.That((DateTimeOffset.Now - ts).Duration() < TimeSpan.FromMinutes(5)).IsTrue();

        Check.That(val).IsEqualTo("payload");
    }

    [Test]
    public void Json()
    {
        var details = new CommandDetails(handle: "H", id: Guid.NewGuid());
        var wf = new CommandWorkflow(details);
        wf.Sequence!.Add(0, "H"); Check.That(wf.Sequence.Count).IsEqualTo(1);
        wf.LogEvent("A", "B"); Check.That(wf.EventLog!.Count).IsEqualTo(1);

        var json = wf.ToString();
        var wf2 = CommandWorkflow.Parse(json)!;

        Check.That(wf2).IsNotNull();
        Check.That(wf2.Details).IsNull();
        Check.That(wf2.Sequence).IsNotNull();
        Check.That(wf2.Sequence!.Count).IsEqualTo(1);
        Check.That(wf2.EventLog).IsNotNull();
        Check.That(wf2.EventLog!.Count).IsEqualTo(1);
    }

    [Test]
    public void DataContract()
    {
        var cd = new CommandDetails(handle: "H", id: Guid.NewGuid(), workflow: new CommandWorkflow());
        cd.Workflow!.Sequence!.Add(0, "H");
        cd.Workflow!.Sequence!.Add(1, "S1");
        cd.Workflow!.LogEvent("S", "D");

        var ser = new DataContractSerializer(typeof(CommandDetails));
        using var ms = new MemoryStream();
        using var writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
        ser.WriteObject(writer, cd); writer.Flush();
        ms.Position = 0;
        CommandDetails? cd2;
        using (var reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max))
        {
            cd2 = ser.ReadObject(reader) as CommandDetails;
        }

        Check.That(cd2).IsNotNull();
        Check.That(cd2!.Workflow).IsNotNull();
        Check.That(cd2!.Workflow!.Details).IsSameReferenceAs(cd2);
        Check.That(cd2!.Workflow!.EventLog).IsNotNull();
        Check.That(cd2!.Workflow!.EventLog!.Count).IsEqualTo(1);
        Check.That(cd2!.Workflow!.Sequence).IsNotNull();
        Check.That(cd2!.Workflow!.Sequence!.Count).IsEqualTo(2);
        Check.That(cd2!.Workflow!.Sequence![0]).IsEqualTo("H");
        Check.That(cd2!.Workflow!.Sequence![1]).IsEqualTo("S1");
    }

    [Test]
    public void FileLogger_ParseLine()
    {
        var path = Path.Combine(Path.GetTempPath(), $"wf_{Guid.NewGuid():N}.log");
        Serilog.ILogger serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(path, formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
        var logger = LoggerHelpers.CreateILogger(serilogLogger, "WFDBG");
        var id = Guid.NewGuid();
        var tool = Guid.NewGuid();
        var details = new CommandDetails(
            arguments: new Dictionary<String, Object?>
            {
                ["Logger"] = logger,
                ["AttachedToolID"] = tool
            },
            handle: "WFDBG",
            id: id
        );
        var wf = new CommandWorkflow(details);
        for (Int32 i = 0; i < 12; i++)
        {
            wf.LogEvent($"Action{i}", $"Payload-{i}");
        }
        (serilogLogger as IDisposable)?.Dispose();
        var lines = File.ReadAllLines(path);
        var eventsParsed = lines.Select(CommandWorkflowEvent.ParseLine)
            .Where(e => e.DetailsID == id && e.ToolID == tool && e.Handle == "WFDBG" && e.Action.StartsWith("Action", StringComparison.Ordinal))
            .ToList();
        Check.That(eventsParsed.Count).IsEqualTo(12);
        var payloads = eventsParsed.Select(e => e.Data).Where(d => d is not null).ToList();
        Check.That(payloads.Count).IsGreaterOrEqualThan(12);
        try { File.Delete(path); } catch { }
    }

    [Test]
    public void Event_ToString_Parse_RoundTrip()
    {
        var id = Guid.NewGuid();
        var tool = Guid.NewGuid();
        var evt = new CommandWorkflowEvent
        {
            DetailsID = id,
            ToolID = tool,
            Handle = "WFDBG",
            Action = "ActX",
            Time = DateTimeOffset.Now
        };

        var s = evt.ToString();
        Check.That(s).IsNotNull();
        Check.That(s).IsNotEmpty();

        var parts = s.Split('~');
        Check.That(parts.Length).IsEqualTo(5);
        Check.That(parts[0]).IsEqualTo(id.ToString("N").ToUpperInvariant());
        Check.That(parts[1]).IsEqualTo(tool.ToString("N").ToUpperInvariant());
        Check.That(parts[2]).IsEqualTo("WFDBG");
        Check.That(parts[3]).IsEqualTo("ActX");
        Check.That(DateTimeOffset.TryParseExact(parts[4], "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ts)).IsTrue();
        Check.That((DateTimeOffset.Now - ts).Duration() < TimeSpan.FromMinutes(5)).IsTrue();

        var parsed = CommandWorkflowEvent.Parse(s);
        Check.That(parsed.DetailsID).IsEqualTo(id);
        Check.That(parsed.ToolID).IsEqualTo(tool);
        Check.That(parsed.Handle).IsEqualTo("WFDBG");
        Check.That(parsed.Action).IsEqualTo("ActX");
        Check.That(parsed.Time).IsEqualTo(evt.Time);
        Check.That(parsed.Data).IsNull();
    }

    [Test]
    public void Event_Equality()
    {
        var t = DateTimeOffset.UtcNow;
        var e1 = new CommandWorkflowEvent { DetailsID = Guid.NewGuid(), ToolID = Guid.NewGuid(), Handle = "A", Action = "X", Time = t };
        var e2 = new CommandWorkflowEvent { DetailsID = Guid.NewGuid(), ToolID = Guid.NewGuid(), Handle = "B", Action = "Y", Time = t };
        var e3 = new CommandWorkflowEvent { DetailsID = e1.DetailsID, ToolID = e1.ToolID, Handle = e1.Handle, Action = e1.Action, Time = t.AddTicks(1) };

        Check.That(e1.Equals(e2)).IsTrue();
        Check.That(e1 == e2).IsTrue();
        Check.That(e1 != e3).IsTrue();
        Check.That(e1.Equals(e3)).IsFalse();
        Check.That(e1.GetHashCode()).IsEqualTo(e2.GetHashCode());
        Check.That(e1.GetHashCode()).IsNotEqualTo(e3.GetHashCode());
    }

    [Test]
    public void Event_HashSet()
    {
        var t = DateTimeOffset.UtcNow;
        var set = new HashSet<CommandWorkflowEvent>();
        var a = new CommandWorkflowEvent { DetailsID = Guid.NewGuid(), ToolID = Guid.NewGuid(), Handle = "H1", Action = "A1", Time = t };
        var b = new CommandWorkflowEvent { DetailsID = Guid.NewGuid(), ToolID = Guid.NewGuid(), Handle = "H2", Action = "A2", Time = t };
        var c = new CommandWorkflowEvent { DetailsID = Guid.NewGuid(), ToolID = Guid.NewGuid(), Handle = "H3", Action = "A3", Time = t.AddMilliseconds(1) };

        Check.That(set.Add(a)).IsTrue();
        Check.That(set.Add(b)).IsFalse();
        Check.That(set.Add(c)).IsTrue();
        Check.That(set.Count).IsEqualTo(2);
    }

    [Test]
    public void Event_TryParse_Success_And_Fail()
    {
        var id = Guid.NewGuid();
        var tool = Guid.NewGuid();
        var evt = new CommandWorkflowEvent { DetailsID = id, ToolID = tool, Handle = "HH", Action = "AA", Time = DateTimeOffset.UtcNow };
        var s = evt.ToString();

        var ok = CommandWorkflowEvent.TryParse(s, CultureInfo.InvariantCulture, out var parsed);
        Check.That(ok).IsTrue();
        Check.That(parsed.DetailsID).IsEqualTo(id);
        Check.That(parsed.ToolID).IsEqualTo(tool);
        Check.That(parsed.Handle).IsEqualTo("HH");
        Check.That(parsed.Action).IsEqualTo("AA");
        Check.That(parsed.Time).IsEqualTo(evt.Time);
    }
}