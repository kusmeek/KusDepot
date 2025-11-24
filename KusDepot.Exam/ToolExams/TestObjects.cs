namespace KusDepot.Exams;

public class ToolHost1 : Tool
{
    public ToolHost1() : base() { this.EnableMyExceptions(); }

    public ToolHost1(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        await base.StartedAsync(cancel);

        Check.That(this.HostedServices!.Count).IsEqualTo(1);
    }
}

public class ToolHost2 : Tool
{
    public ToolHost2() : base() { }

    public ToolHost2(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        await base.StartedAsync(cancel);

        Check.That(this.HostedServices!.Count).IsEqualTo(2);
    }
}

public class ToolHost3 : Tool
{
    public ToolHost3() : base()
    {
        this.EnableMyExceptions();

        var _ = new Tool003();

        this.ResolveHostedServices();

        this.HostedServices!.Add(_);

        Check.That(this.MaskHostedServices(false)).IsTrue(); Check.That(this.AddInstance()).IsTrue();

        Check.That(this.IsHosting(_)).IsTrue();

        Check.That(Tool.GetHost(_)).IsSameReferenceAs(this);

        _.Dispose(); Check.That(this.IsHosting(_)).IsFalse();
    }

    public ToolHost3(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }
}

public class Tool000 : Tool
{
    public Tool000() : base() { }

    public Tool000(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }
}

public class Tool001 : Tool
{
    public Tool001() : base() { this.EnableMyExceptions(); }

    public Tool001(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }

    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await base.StartingAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);
    }

    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);

        await base.StartAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await base.StartedAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await base.StoppingAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);
    }

    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);

        await base.StopAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        await base.StoppedAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(Settings.NoExceptions).IsFalse();
    }
}

public class Tool002 : Tool001
{
    public Tool002() : base() { }

    public Tool002(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        await base.StartedAsync(cancel,key);

        Check.That(Tool.GetHost(this)!.GetType()).IsEqualTo(typeof(ToolHost));
    }
}

public class Tool003 : Tool
{
    public Tool003() : base() { }

    public Tool003(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { }
}

public class Tool004 : Tool
{
    public Tool004() : base() { this.EnableMyExceptions(); }

    public Tool004(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }

    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await base.StartingAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);
    }

    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);

        await base.StartAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await base.StartedAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await base.StoppingAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);
    }

    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);

        await base.StopAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        await base.StoppedAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(Settings.NoExceptions).IsFalse();
    }
}

public class Tool005 : Tool
{
    public Tool005() : base() { this.EnableMyExceptions(); }

    public Tool005(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { this.EnableMyExceptions(); }

    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await base.StartingAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);
    }

    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);

        await base.StartAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await base.StartedAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await base.StoppingAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);
    }

    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);

        await base.StopAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        await base.StoppedAsync(cancel,key);

        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(Settings.NoExceptions).IsFalse();
    }
}

public class BackgroundServce006 : BackgroundService
{
    public BackgroundServce006() : base() { }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Check.That(Tool.GetHost(this)!.IsHosting(this)).IsTrue();

        return Task.CompletedTask;
    }
}

internal sealed class CommandTest : Command
{
    public CommandTest() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a);

            Double? x = a.Details.GetArgument<Double?>("x"); Double? y = a.Details.GetArgument<Double?>("y");

            if(x is null || y is null) { AddOutput(a.ID); return null; }

            this.Multiply(a,x.Value,y.Value); return a.ID;
        }
        catch { AddOutput(a?.ID); CleanUp(a); return null; }

        finally { CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    Double? x = a.Details.GetArgument<Double?>("x"); Double? y = a.Details.GetArgument<Double?>("y");

                    if(x is null || y is null) { AddOutput(a.ID); return null; } this.Multiply(a,x.Value,y.Value);

                    await Task.CompletedTask; return true;
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch { AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch { AddOutput(a?.ID); CleanUp(a); return null; }
    }

    private void Multiply(Activity a , Double x , Double y) { Double result = x * y; AddOutput(a.ID,result); }
}

internal sealed class CommandTestR : Command
{
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        return activity?.ID ?? Guid.CreateVersion7();
    }
}

public sealed class CommandZ : Command
{
    public CommandZ() {}

    public override Boolean Attach(ITool tool , AccessKey? key = null) => throw new NotImplementedException();

    public override Boolean Detach(AccessKey? key = null) => throw new NotImplementedException();

    public override Task<Boolean> Disable(CancellationToken cancel = default , AccessKey? key = null) => throw new NotImplementedException();

    public override Task<Boolean> Enable(CancellationToken cancel = default , AccessKey? key = null) => throw new NotImplementedException();

    public override Guid? Execute(Activity? activity , AccessKey? key = null) => throw new NotImplementedException();
}