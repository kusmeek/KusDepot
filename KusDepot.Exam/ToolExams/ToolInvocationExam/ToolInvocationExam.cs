namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolInvocationExam
{
    [Test]
    public async Task Invoke_ITool_RegisterAndExecuteCommand()
    {
        using Tool tool = new();
        ITool t = tool;

        ObjectInvoker i = ObjectInvoker.Create(t);

        Check.That(await i.InvokeAsync("Activate")).IsEqualTo(true);
        Check.That(await i.InvokeAsync("EnableAllCommands")).IsEqualTo(true);
        Check.That(await i.InvokeAsync("RegisterCommand",[ "Ping" , new CommandTestE() , null , true ])).IsEqualTo(true);

        Guid id = Guid.NewGuid();

        CommandDetails d = new() { Handle = "Ping" , ID = id };

        Check.That(i.Invoke("ExecuteCommand",d)).IsEqualTo((Guid?)id);
    }

    [Test]
    public async Task Invoke_ITool_ExecuteCommandAsync()
    {
        using Tool tool = new();
        ITool t = tool;

        ObjectInvoker i = ObjectInvoker.Create(t);

        Check.That(await i.InvokeAsync("Activate")).IsEqualTo(true);
        Check.That(await i.InvokeAsync("EnableAllCommands")).IsEqualTo(true);
        Check.That(await i.InvokeAsync("RegisterCommand",[ "MultiplyAsync" , new CommandTest() , null , true ])).IsEqualTo(true);

        Guid id = Guid.NewGuid();

        CommandDetails d = new()
        {
            Handle = "MultiplyAsync",
            ID = id,
            Arguments = new() { ["x"] = 2.0D , ["y"] = 3.0D }
        };

        Check.That(await i.InvokeAsync("ExecuteCommandAsync",[ d ])).IsEqualTo((Guid?)id);
        Check.That(i.Invoke("GetOutput",[ id ])).IsEqualTo(6.0D);
    }

    [Test]
    public async Task Invoke_ITool_AddStartAndRemoveHostedService()
    {
        using Tool host = new();
        using BackgroundServce006 child = new();

        ITool h = host;

        ObjectInvoker i = ObjectInvoker.Create(h);

        Check.That(await i.InvokeAsync("StartHostAsync")).IsEqualTo(true);
        Check.That(i.Invoke("UnMaskHostedServices")).IsEqualTo(true);

        Check.That(await i.InvokeAsync("AddHostedService",[ child , null , null , null , true ])).IsEqualTo(true);
        Check.That(host.IsHosting(child)).IsTrue();

        Check.That(await i.InvokeAsync("RemoveHostedService",[ child , true ])).IsEqualTo(true);
        Check.That(host.IsHosting(child)).IsFalse();
    }
}
