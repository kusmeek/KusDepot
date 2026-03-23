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

        this.HostedServices!.Add(Guid.NewGuid().ToStringInvariant()!,_);

        Check.That(this.UnMaskHostedServices()).IsTrue(); Check.That(this.AddInstance()).IsTrue();

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

    public Tool000(IAccessManager accessmanager) : base(accessmanager) { }

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
        Check.That(this.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

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

public class ActivityRecordTool : Tool
{
    public ActivityRecordTool() : base() { }

    public ActivityRecordTool(IAccessManager? accessmanager = null,
                IEnumerable<DataItem>? data = null,
                Guid? id = null,
                ToolServiceProvider? services = null,
                Dictionary<String,ICommand>? commands = null,
                IConfiguration? configuration = null,
                ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) { }

    public ActivityRecordCode? ExpectedCode { get; set; }

    public Guid? ExpectedActivityID { get; set; }

    public Boolean RemoveActivityCalled { get; private set; }

    public void ResetCalled() { this.RemoveActivityCalled = false; }

    [AccessCheck(ProtectedOperation.RemoveActivity)]
    public override Boolean RemoveActivity(Activity? activity , AccessKey? key = null)
    {
        this.RemoveActivityCalled = true;

        if(activity is not null)
        {
            if(this.ExpectedActivityID.HasValue)
            {
                Check.That(activity.ID).IsEqualTo(this.ExpectedActivityID);
            }

            if(this.ExpectedCode.HasValue)
            {
                Check.That(activity.Record).IsNotNull();

                Check.That(activity.Record!.Code).IsEqualTo(this.ExpectedCode.Value);
            }
        }

        return base.RemoveActivity(activity,key);
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

public sealed class ConstructorArgsBackgroundService : BackgroundService
{
    public ConstructorArgsBackgroundService(Int32 count , String name , Boolean online , Guid id , TimeSpan delay)
    {
        this.Count = count;
        this.Name = name;
        this.Online = online;
        this.Id = id;
        this.Delay = delay;
    }

    public Int32 Count { get; }

    public String Name { get; }

    public Boolean Online { get; }

    public Guid Id { get; }

    public TimeSpan Delay { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}

public sealed class StartFaultService : Tool
{
    public StartFaultService(IAccessManager? accessmanager = null) : base(accessmanager) { this.EnableMyExceptions(); }

    public override Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        return Task.FromException(new InvalidOperationException("StartFaultService"));
    }
}

public sealed class StopFaultService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.FromException(new InvalidOperationException("StopFaultService"));
}

public sealed class StopHangService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.Delay(Timeout.InfiniteTimeSpan,CancellationToken.None);
}

public sealed class StartHangService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.Delay(Timeout.InfiniteTimeSpan,CancellationToken.None);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public sealed class ToolServiceControl : Tool
{
    public ToolServiceControl() : base() { this.EnableMyExceptions(); }

    public Task<Boolean> StartHosted(IHostedService service , CancellationToken cancel = default) => this.StartHostedService(service,cancel);

    public Task<Boolean> StopHosted(IHostedService service , CancellationToken cancel = default) => this.StopHostedService(service,cancel);
}

public sealed class ToolStopFail : Tool000
{
    public ToolStopFail() : base(new TestAccessManager()) { this.EnableMyExceptions(); }

    public override Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        return Task.FromException(new InvalidOperationException("ToolStopFail"));
    }
}

public sealed class ToolHostingOptionsControl : Tool
{
    public ToolHostingOptionsControl(TimeSpan? shutdown = null , TimeSpan? startup = null) : base()
    {
        this.HostingOptions = new ToolHostOptions
        {
            ServicesStartConcurrently = true,
            ServicesStopConcurrently = true,
            StartupTimeout = startup ?? Timeout.InfiniteTimeSpan,
            ShutdownTimeout = shutdown ?? TimeSpan.FromSeconds(120)
        };
    }
}

internal sealed class CommandTest : Command
{
    public CommandTest() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

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

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

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

internal sealed class CommandTestE : Command
{
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(!EnabledAllowed(key)) { return null; } return activity?.ID ?? Guid.CreateVersion7();
    }
}

internal sealed class CommandTestR : Command
{
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        return activity?.ID ?? Guid.CreateVersion7();
    }
}

public sealed class CommandZ : Command
{
    public CommandZ() {}

    public override Boolean Attach(ITool tool , CommandKey? key = null) => throw new NotImplementedException();

    public override Boolean Detach(CommandKey? key = null) => throw new NotImplementedException();

    public override Task<Boolean> Disable(CancellationToken cancel = default , CommandKey? key = null) => throw new NotImplementedException();

    public override Task<Boolean> Enable(CancellationToken cancel = default , CommandKey? key = null) => throw new NotImplementedException();

    public override Guid? Execute(Activity? activity , CommandKey? key = null) => throw new NotImplementedException();
}

internal sealed class ActivityRecordSuccessCommand : Command
{
    public ActivityRecordSuccessCommand() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            AddActivity(a); return a.ID;
        }
        catch { AddOutput(a.ID); return null; }

        finally { CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    await Task.CompletedTask; return true;
                }
                catch { AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch { return null; }
    }
}

internal sealed class ActivityRecordCanceledCommand : Command
{
    public ActivityRecordCanceledCommand() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        throw new NotSupportedException("Use ExecuteAsync for cancellation tests.");
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    await Task.Delay(Timeout.InfiniteTimeSpan,a.Details.GetCancel()).ConfigureAwait(false);

                    return true;
                }
                catch { AddOutput(a.ID); throw; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch { throw; }
    }
}

internal sealed class ActivityRecordFaultCommand : Command
{
    public ActivityRecordFaultCommand() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            AddActivity(a);

            throw new InvalidOperationException("Sync fault.");
        }
        catch { AddOutput(a?.ID); throw; }

        finally { CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    await Task.CompletedTask; throw new InvalidOperationException("Async fault.");
                }
                catch { AddOutput(a.ID); throw; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch { throw; }
    }
}

internal sealed class ActivityRecordTimedOutCommand : Command
{
    public ActivityRecordTimedOutCommand() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        throw new NotSupportedException("Use ExecuteAsync for timeout tests.");
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    await Task.Delay(Timeout.InfiniteTimeSpan,a.Cancel!.Token).ConfigureAwait(false);

                    return true;
                }
                catch { AddOutput(a.ID); throw; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch { throw; }
    }
}

internal sealed class FreeModeCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public FreeModeCommand()
    {
        this.ExecutionMode.AllowAsynchronousFreeMode();
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity, CommandKey? key = null)
    {
        LastActivity = activity; if (activity is null) { return null; }

        try
        {
            await Task.Delay(4000, activity.Details!.GetCancel()).ConfigureAwait(false);
            SetSuccess(activity);
            return activity.ID;
        }
        finally { this.CleanUpFreeMode(activity); }
    }
}

internal sealed class StateCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public StateCommand() { this.ExecutionMode.AllowBothFreeMode(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a); if(a.FreeMode) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { if(a.FreeMode) { this.CleanUpFreeMode(a); } else { this.CleanUp(a); } }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        this.LastActivity = a;

        try
        {
            AddActivity(a); await Task.Delay(TimeSpan.FromSeconds(20)).ConfigureAwait(false);

            if(a.FreeMode is false) { Check.That(a.State == ActivityState.Running).IsTrue(); }

            if(AddOutput(a.ID,true) is true) { SetSuccess(a); return true; }

            AddOutput(a.ID); return false;
        }
        catch ( Exception _ )
        {
            AddOutput(a.ID); SetFaulted(a,_.Message); return false;
        }
    }
}

internal sealed class FailingCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public FailingCommand() { this.ExecutionMode.AllowBothFreeMode(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a); if(a.FreeMode) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        this.LastActivity = a;

        try
        {
            AddActivity(a);

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            SetFailed(a); return false;
        }
        catch ( Exception _ )
        {
            AddOutput(a.ID); SetFaulted(a,_.Message); return false;
        }
        finally { this.CleanUpFreeMode(a); }
    }
}

internal sealed class FaultingCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public FaultingCommand() { this.ExecutionMode.AllowBothFreeMode(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a); if(a.FreeMode) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        this.LastActivity = a;

        try
        {
            AddActivity(a);

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            throw new OperationFailedException("FaultingCommand simulated failure");
        }
        catch ( Exception _ )
        {
            AddOutput(a.ID); SetFaulted(a,_.Message); return false;
        }
        finally { this.CleanUpFreeMode(a); }
    }
}

internal sealed class NonCheckingCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public NonCheckingCommand() { this.ExecutionMode.AllowBothFreeMode(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a); if(a.FreeMode) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        this.LastActivity = a;

        try
        {
            AddActivity(a);

            while(true)
            {
                try { await Task.Delay(1000).ConfigureAwait(false); } catch { }
            }
        }
        catch ( Exception _ )
        {
            AddOutput(a.ID); SetFaulted(a,_.Message); return false;
        }
        finally { this.CleanUpFreeMode(a); }
    }
}

internal sealed class CheckingCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public CheckingCommand() { this.ExecutionMode.AllowBothFreeMode(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a); if(a.FreeMode) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        this.LastActivity = a;

        try
        {
            AddActivity(a);

            while(true)
            {
                try { await Task.Delay(1000).ConfigureAwait(false); } catch { break; }

                if(a.Details.GetCancel().IsCancellationRequested) { throw new OperationCanceledException(String.Empty); }
            }

            AddOutput(a.ID,true); return true;
        }
        catch ( OperationCanceledException )
        {
            AddOutput(a.ID); SetCanceled(a); return false;
        }
        catch ( Exception _ )
        {
            AddOutput(a.ID); SetFaulted(a,_.Message); return false;
        }
        finally { this.CleanUpFreeMode(a); }
    }
}

internal sealed class TimeOutCommand : Command
{
    public Activity? LastActivity { get; private set; }

    public TimeOutCommand() { this.ExecutionMode.AllowBothFreeMode(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a); if(a.FreeMode) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception ) { AddOutput(a.ID); return null; }

        finally { this.CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        this.LastActivity = a;

        try
        {
            AddActivity(a);

            while(true)
            {
                try { await Task.Delay(1000).ConfigureAwait(false); } catch { }

                if(a.IsTimedOut()) { SetTimedOut(a); return false; }
            }
        }
        catch ( OperationCanceledException )
        {
            AddOutput(a.ID); SetCanceled(a); return false;
        }
        catch ( Exception _ )
        {
            AddOutput(a.ID); SetFaulted(a,_.Message); return false;
        }
        finally { this.CleanUpFreeMode(a); }
    }
}

internal sealed class CommandEnableFail : Command
{
    public CommandEnableFail() : base() { }

    public override Task<Boolean> Enable(CancellationToken cancel = default , CommandKey? key = null)
    {
        return Task.FromResult(false);
    }
}

internal sealed class CommandDisableFail : Command
{
    public CommandDisableFail() : base() { }

    public override Task<Boolean> Disable(CancellationToken cancel = default , CommandKey? key = null)
    {
        return Task.FromResult(false);
    }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(!EnabledAllowed(key)) { return null; } return activity?.ID ?? Guid.CreateVersion7();
    }
}

public sealed class CommandAttachFail : Command
{
    public override Boolean Attach(ITool tool , CommandKey? key = null) => false;
}

public sealed class CommandLockFail : Command
{
    public override Boolean Lock(ManagementKey? key) => false;
}

public sealed class CommandAttachFault : Command
{
    public override Boolean Attach(ITool tool , CommandKey? key = null) => throw new InvalidOperationException("Attach fault");
}

public sealed class CommandEnableFault : Command
{
    public override Task<Boolean> Enable(CancellationToken cancel = default , CommandKey? key = null)
        => throw new InvalidOperationException("Enable fault");
}

public sealed class CommandLockFault : Command
{
    public override Boolean Lock(ManagementKey? key) => throw new InvalidOperationException("Lock fault");
}

public sealed class CommandDetachFault : Command
{
    public override Boolean Detach(CommandKey? key = null) => throw new InvalidOperationException("Detach fault");
}

public class SecurityCommand : Command
{
    public Boolean ExecutedSuccessfully { get; private set; }

    public override Guid? Execute(Activity? activity = null , CommandKey? key = null)
    {
        if(AccessCheck(key) is false)
        {
            ExecutedSuccessfully = false;
            return null;
        }

        ExecutedSuccessfully = true;
        return activity!.ID;
    }

    public void Reset() { ExecutedSuccessfully = false; }
}

public class TestAccessManager : AccessManager
{
    public TestAccessManager(ITool? tool = null , Microsoft.Extensions.Logging.ILogger? logger = null , X509Certificate2? certificate = null) : base(tool,logger,certificate){}

    public TestAccessManager() : base(){}

    public override AccessKey? RequestAccess(AccessRequest? request = null)
    {
        try
        {
            if(request is null || Tool is null) { return null; }

            if(request is ManagementRequest)
            {
                ManagementKey? k = ManagementKey.Parse((request as ManagementRequest)!.Data,null);

                if(Tool.CheckManager(k) is true || Tool.CheckOwner(k) is true) { return GenerateExecutiveKey(); }

                k = new OwnerKey(DeserializeCertificate((request as ManagementRequest)!.Data.ToByteArrayFromBase64())!);

                if(Tool.CheckOwner(k) is true) { return GenerateExecutiveKey(); }

                k = new ManagerKey(DeserializeCertificate((request as ManagementRequest)!.Data.ToByteArrayFromBase64())!);

                if(Tool.CheckManager(k) is true) { return GenerateExecutiveKey(); }
            }

            //if(Tool.GetLocked() is not false) { return null; }

            switch(request)
            {
                case HostRequest:
                {
                    if((request as HostRequest)!.External) { return GenerateHostKey(); }

                    if((request as HostRequest)!.Host!.IsHosting(Tool,Tool) ) { return GenerateHostKey(); }

                    return null;
                }

                case ServiceRequest:
                {
                    if(ReferenceEquals(Tool,(request as ServiceRequest)!.Tool)) { return GenerateExecutiveKey(); }

                    return GenerateServiceKey((request as ServiceRequest)!.Tool);
                }

                case StandardRequest: { return GenerateClientKey(); }

                default: { return null; }
            }
        }
        catch { return null; }
    }
}