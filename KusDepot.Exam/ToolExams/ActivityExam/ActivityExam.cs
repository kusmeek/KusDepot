namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ActivityExam
{
    [Test]
    public void CreateActivity()
    {
        var id = Guid.NewGuid();
        var toolId = Guid.NewGuid();
        var logger = LoggerHelpers.CreateILogger();
        var ctsExt = new CancellationTokenSource();
        var details = new CommandDetails(
            arguments: new Dictionary<String, Object?>
            {
                ["Logger"] = logger,
                ["Timeout"] = TimeSpan.FromMilliseconds(50),
                ["Cancel"] = ctsExt.Token,
                ["AttachedToolID"] = toolId
            },
            handle: "H",
            id: id
        );

        var act = Activity.CreateActivity(details);
        Check.That(act).IsNotNull();
        Check.That(act!.Details).IsSameReferenceAs(details);
        Check.That(act.ID).IsEqualTo(id);
        Check.That(act.Logger).IsSameReferenceAs(logger);
        Check.That(act.Cancel).IsNotNull();
    }

    [Test]
    public void Properties_WriteOnce()
    {
        var act = new Activity();

        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();
        act.ID = g1; act.ID = g2;
        Check.That(act.ID).IsEqualTo(g1);

        act.Handle = "A"; act.Handle = "B";
        Check.That(act.Handle).IsEqualTo("A");

        var logger1 = LoggerHelpers.CreateILogger();
        var logger2 = LoggerHelpers.CreateILogger();
        act.Logger = logger1; act.Logger = logger2;
        Check.That(act.Logger).IsSameReferenceAs(logger1);

        var prog1 = new Progress<Object>(_ => { });
        var prog2 = new Progress<Object>(_ => { });
        act.Progress = prog1; act.Progress = prog2;
        Check.That(act.Progress).IsSameReferenceAs(prog1);

        var t1 = Task.FromResult<Object?>("x");
        var t2 = Task.FromResult<Object?>("y");
        act.Task = t1; act.Task = t2;
        Check.That(act.Task).IsSameReferenceAs(t1);

        var c1 = new CancellationTokenSource();
        var c2 = new CancellationTokenSource();
        act.Cancel = c1; act.Cancel = c2;
        Check.That(act.Cancel).IsSameReferenceAs(c1);
    }

    [Test]
    public void Workflow_Start_and_Stop()
    {
        var id = Guid.NewGuid();
        var toolId = Guid.NewGuid();
        var logger = LoggerHelpers.CreateILogger();
        var details = new CommandDetails(
            arguments: new Dictionary<String, Object?>
            {
                ["Logger"] = logger,
                ["AttachedToolID"] = toolId
            },
            handle: "WFH",
            id: id
        );

        var act = Activity.CreateActivity(details)!;
        var ok1 = act.StartWorkflowAction("X", "D1");
        Check.That(ok1).IsTrue();
        Check.That(details.Workflow).IsNotNull();
        Check.That(details.Workflow!.EventLog).IsNotNull();
        Check.That(details.Workflow!.EventLog!.Count).IsEqualTo(1);
        var key1 = details.Workflow!.EventLog!.Keys.Single();
        var evt1 = CommandWorkflowEvent.Parse(key1);
        Check.That(evt1.DetailsID).IsEqualTo(id);
        Check.That(evt1.ToolID).IsEqualTo(toolId);
        Check.That(evt1.Handle).IsEqualTo("WFH");
        Check.That(evt1.Action).IsEqualTo("Start-X");
        Check.That(details.Workflow!.EventLog![key1]).IsEqualTo("D1");

        var ok2 = act.StopWorkflowAction("X","D2");
        Check.That(ok2).IsTrue();
        Check.That(details.Workflow!.EventLog!.Count).IsEqualTo(2);
        var foundStop = details.Workflow!.EventLog!.Keys.Select(CommandWorkflowEvent.Parse).Any(e => e.Action == "Stop-X");
        Check.That(foundStop).IsTrue();
    }

    [Test]
    public void Workflow_LogWorkflowEvent()
    {
        var id = Guid.NewGuid();
        var toolId = Guid.NewGuid();
        var logger = LoggerHelpers.CreateILogger();
        var details = new CommandDetails(
            arguments: new Dictionary<String, Object?>
            {
                ["Logger"] = logger,
                ["AttachedToolID"] = toolId
            },
            handle: "WFH",
            id: id
        );

        var act = Activity.CreateActivity(details)!;
        var ok = act.LogWorkflowEvent("Custom","D3");
        Check.That(ok).IsTrue();
        Check.That(details.Workflow).IsNotNull();
        Check.That(details.Workflow!.EventLog).IsNotNull();
        Check.That(details.Workflow!.EventLog!.Count).IsEqualTo(1);
        var key = details.Workflow!.EventLog!.Keys.Single();
        var evt = CommandWorkflowEvent.Parse(key);
        Check.That(evt.DetailsID).IsEqualTo(id);
        Check.That(evt.ToolID).IsEqualTo(toolId);
        Check.That(evt.Handle).IsEqualTo("WFH");
        Check.That(evt.Action).IsEqualTo("Custom");
        Check.That(details.Workflow!.EventLog![key]).IsEqualTo("D3");
    }
}