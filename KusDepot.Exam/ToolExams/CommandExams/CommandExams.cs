using CliWrap.Buffered;

namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.Children)]
public partial class CommandExams
{
    [Test]
    public async Task ExecuteCommand()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _11 = new() { Handle = "Multiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _12 = new() { Handle = "Multiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new CommandTest();

        Check.That(_0.ExecuteCommand(new(){})).IsNull();
        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(_0.ExecuteCommand(new(){})).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(await _0.EnableAllCommands()).IsTrue();
        Check.That(_0.ExecuteCommand(new(){})).IsNull();
        Check.That(_0.ExecuteCommand(_11)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);
        Check.That(await _0.Deactivate()).IsTrue();
        Check.That(_0.ExecuteCommand(_12)).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommand(_12)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);
        Check.That(_0.GetOutput(_4)).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteCommandAsync()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _11 = new() { Handle = "Multiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _12 = new() { Handle = "Multiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new CommandTest();

        Check.That(await _0.ExecuteCommandAsync(new(){})).IsNull();
        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.ExecuteCommandAsync(new(){})).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(await _0.EnableAllCommands()).IsTrue();
        Check.That(await _0.ExecuteCommandAsync(new(){})).IsNull();
        Check.That(await _0.ExecuteCommandAsync(_11)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);
        Check.That(await _0.Deactivate()).IsTrue();
        Check.That(await _0.ExecuteCommandAsync(_12)).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(await _0.ExecuteCommandAsync(_12)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);
        Check.That(await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteCommandAsync_FreeMode()
    {
        var tool = new Tool();
        var cmd = new FreeModeCommand();

        Check.That(await tool.RegisterCommand("FREE",cmd)).IsTrue();

        var details = CommandDetails.Create().SetHandle("FREE").SetFreeMode(true);

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();
        Guid? id = await tool.ExecuteCommandAsync(details, CancellationToken.None);
        Check.That(id).IsNotNull();

        await Task.Delay(10000);

        var act = cmd.LastActivity;
        Check.That(act).IsNotNull();
        Check.That(act!.Record).IsNotNull();
        Check.That(act.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
    }

    [Test]
    public async Task ExecuteCommandDelegator()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _21 = new() { Handle = "AltMultiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _22 = new() { Handle = "LinkMultiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new();
        CommandDelegator _7 = new("Multiply");
        CommandDelegator _9 = new("AltMultiply");
        CommandDelegator _11 = new("NextMultiply");

        Check.That(_0.ExecuteCommand(new(){})).IsNull();
        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.RegisterCommand("AltMultiply",_7)).IsTrue();
        Check.That(await _0.RegisterCommand("NextMultiply",_9)).IsTrue();
        Check.That(await _0.RegisterCommand("LinkMultiply",_11)).IsTrue();
        Check.That(_0.ExecuteCommand(new(){})).IsNull();
        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();
        Check.That(_0.ExecuteCommand(new(){})).IsNull();
        Check.That(_0.ExecuteCommand(_21)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);
        Check.That(await _0.Deactivate()).IsTrue();
        Check.That(_0.ExecuteCommand(_22)).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommand(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteCommandDelegatorAsync()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _21 = new() { Handle = "AltMultiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _22 = new() { Handle = "LinkMultiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new();
        CommandDelegator _7 = new("Multiply");
        CommandDelegator _9 = new("AltMultiply");
        CommandDelegator _11 = new("NextMultiply");

        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.RegisterCommand("AltMultiply",_7)).IsTrue();
        Check.That(await _0.RegisterCommand("NextMultiply",_9)).IsTrue();
        Check.That(await _0.RegisterCommand("LinkMultiply",_11)).IsTrue();
        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(await _0.ExecuteCommandAsync(_21)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(await _0.ExecuteCommandAsync(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteCommandDelegatorCab()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _21 = new() { Handle = "AltMultiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _22 = new() { Handle = "LinkMultiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new();
        CommandDelegator _7 = new("Multiply");
        CommandDelegator _9 = new("AltMultiply");
        CommandDelegator _11 = new("NextMultiply");
        
        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.RegisterCommand("AltMultiply",_7)).IsTrue();
        Check.That(await _0.RegisterCommand("NextMultiply",_9)).IsTrue();
        Check.That(await _0.RegisterCommand("LinkMultiply",_11)).IsTrue();
        Check.That(_0.ExecuteCommandCab(new())).IsNull();
        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();
        Check.That(_0.ExecuteCommandCab(new(){})).IsNull();
        Check.That(_0.ExecuteCommandCab(_21.ToKusDepotCab())).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);
        Check.That(await _0.Deactivate()).IsTrue();
        Check.That(_0.ExecuteCommandCab(_22.ToKusDepotCab())).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommandCab(_22.ToKusDepotCab())).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteCommandDelegatorCabAsync()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _21 = new() { Handle = "AltMultiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _22 = new() { Handle = "LinkMultiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new();
        CommandDelegator _7 = new("Multiply");
        CommandDelegator _9 = new("AltMultiply");
        CommandDelegator _11 = new("NextMultiply");
 
        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.RegisterCommand("AltMultiply",_7)).IsTrue();
        Check.That(await _0.RegisterCommand("NextMultiply",_9)).IsTrue();
        Check.That(await _0.RegisterCommand("LinkMultiply",_11)).IsTrue();
        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(await _0.ExecuteCommandCabAsync(_21.ToKusDepotCab())).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(await _0.ExecuteCommandCabAsync(_22.ToKusDepotCab())).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteCommandSync_ActivityRecord_Success()
    {
        ActivityRecordTool _0 = new ActivityRecordTool(); Guid _4 = Guid.NewGuid();

        _0.ExpectedCode = ActivityRecordCode.Success; _0.ExpectedActivityID = _4;

        ActivityRecordSuccessCommand _5 = new ActivityRecordSuccessCommand();

        Check.That(await _0.RegisterCommand("Success",_5)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        CommandDetails _11 = CommandDetails.Create("Success").SetID(_4);

        Check.That(_0.ExecuteCommand(_11)).IsEqualTo(_4);

        Check.That(_0.RemoveActivityCalled).IsTrue();
    }

    [Test]
    public async Task ExecuteCommandAsync_ActivityRecord_Success()
    {
        ActivityRecordTool _0 = new ActivityRecordTool(); Guid _4 = Guid.NewGuid();

        _0.ExpectedCode = ActivityRecordCode.Success; _0.ExpectedActivityID = _4;

        ActivityRecordSuccessCommand _5 = new ActivityRecordSuccessCommand();

        Check.That(await _0.RegisterCommand("SuccessAsync",_5)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        CommandDetails _11 = CommandDetails.Create("SuccessAsync").SetID(_4);

        Check.That(await _0.ExecuteCommandAsync(_11)).IsEqualTo(_4);

        Check.That(_0.RemoveActivityCalled).IsTrue();
    }

    [Test]
    public async Task ExecuteCommandAsync_ActivityRecord_Canceled()
    {
        ActivityRecordTool _0 = new ActivityRecordTool(); Guid _4 = Guid.NewGuid();

        _0.ExpectedCode = ActivityRecordCode.Canceled; _0.ExpectedActivityID = _4;

        ActivityRecordCanceledCommand _5 = new ActivityRecordCanceledCommand();

        Check.That(await _0.RegisterCommand("CanceledAsync",_5)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        using CancellationTokenSource cts1 = new(TimeSpan.FromSeconds(12));

        CommandDetails _11 = CommandDetails.Create("CanceledAsync").SetID(_4).SetCancel(cts1.Token);

        Guid? r1 = null;

        Check.ThatCode(async () => r1 = await _0.ExecuteCommandAsync(_11,cts1.Token)).DoesNotThrow();

        Check.That(r1).IsNull();

        await Task.Delay(TimeSpan.FromSeconds(3));

        Check.That(_0.RemoveActivityCalled).IsTrue(); _0.ResetCalled(); Check.That(_0.RemoveActivityCalled).IsFalse();

        Check.That(_0.EnableMyExceptions()).IsTrue();

        using CancellationTokenSource cts2 = new(TimeSpan.FromSeconds(10));

        CommandDetails _12 = CommandDetails.Create("CanceledAsync").SetID(_4).SetCancel(cts2.Token);

        Check.ThatCode(async () => await _0.ExecuteCommandAsync(_12,cts2.Token)).Throws<OperationCanceledException>();

        await Task.Delay(TimeSpan.FromSeconds(3));

        Check.That(_0.RemoveActivityCalled).IsFalse();
    }

    [Test]
    public async Task ExecuteCommandSync_ActivityRecord_Faulted()
    {
        ActivityRecordTool _0 = new ActivityRecordTool(); Guid _4 = Guid.NewGuid();

        _0.ExpectedCode = ActivityRecordCode.Faulted; _0.ExpectedActivityID = _4;

        ActivityRecordFaultCommand _5 = new ActivityRecordFaultCommand();

        Check.That(await _0.RegisterCommand("Fault",_5)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        CommandDetails _11 = CommandDetails.Create("Fault").SetID(_4);

        Guid? r1 = null;

        Check.ThatCode(() => r1 = _0.ExecuteCommand(_11)).DoesNotThrow();

        Check.That(r1).IsNull();

        Check.That(_0.RemoveActivityCalled).IsTrue(); _0.ResetCalled(); Check.That(_0.RemoveActivityCalled).IsFalse();

        Check.That(_0.EnableMyExceptions()).IsTrue();

        Check.ThatCode(() => _0.ExecuteCommand(_11)).Throws<InvalidOperationException>();

        Check.That(_0.RemoveActivityCalled).IsFalse();
    }

    [Test]
    public async Task ExecuteCommandAsync_ActivityRecord_Faulted()
    {
        ActivityRecordTool _0 = new ActivityRecordTool(); Guid _4 = Guid.NewGuid();

        _0.ExpectedCode = ActivityRecordCode.Faulted; _0.ExpectedActivityID = _4;

        ActivityRecordFaultCommand _5 = new ActivityRecordFaultCommand();

        Check.That(await _0.RegisterCommand("FaultAsync",_5)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        CommandDetails _11 = CommandDetails.Create("FaultAsync").SetID(_4);

        Guid? r1 = null;

        Check.ThatCode(async () => r1 = await _0.ExecuteCommandAsync(_11)).DoesNotThrow();

        Check.That(r1).IsNull();

        Check.That(_0.RemoveActivityCalled).IsTrue(); _0.ResetCalled(); Check.That(_0.RemoveActivityCalled).IsFalse();

        Check.That(_0.EnableMyExceptions()).IsTrue();

        Check.ThatCode(async () => await _0.ExecuteCommandAsync(_11)).Throws<InvalidOperationException>();

        Check.That(_0.RemoveActivityCalled).IsFalse();
    }

    [Test]
    public async Task ExecuteCommandAsync_ActivityRecord_TimedOut()
    {
        ActivityRecordTool _0 = new ActivityRecordTool(); Guid _4 = Guid.NewGuid();

        _0.ExpectedCode = ActivityRecordCode.TimedOut; _0.ExpectedActivityID = _4;

        ActivityRecordTimedOutCommand _5 = new ActivityRecordTimedOutCommand();

        Check.That(await _0.RegisterCommand("TimedOutAsync",_5)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        CommandDetails _11 = CommandDetails.Create("TimedOutAsync").SetID(_4).SetTimeout(TimeSpan.FromSeconds(10));

        Guid? r1 = null;

        Check.ThatCode(async () => r1 = await _0.ExecuteCommandAsync(_11)).DoesNotThrow();

        Check.That(r1).IsNull(); await Task.Delay(TimeSpan.FromSeconds(3));

        Check.That(_0.RemoveActivityCalled).IsTrue();
        
        _0.ResetCalled(); Check.That(_0.RemoveActivityCalled).IsFalse();

        Check.That(_0.EnableMyExceptions()).IsTrue();

        CommandDetails _12 = CommandDetails.Create("TimedOutAsync").SetID(_4).SetTimeout(TimeSpan.FromSeconds(10));

        Check.ThatCode(async () => await _0.ExecuteCommandAsync(_12)).Throws<OperationCanceledException>();

        Check.That(_0.RemoveActivityCalled).IsFalse();
    }

    [Test]
    public async Task ExecuteConsoleDelegator()
    {
        CliWrap.Command concmd = Cli.Wrap("systeminfo");

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("ConsoleDelegator",new ConsoleDelegator(concmd))

            .RegisterCommand("BufferedConsoleDelegator",new ConsoleDelegator(concmd,buffered:true))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        var id1 = _.ExecuteCommand(new() { Handle = "ConsoleDelegator" } )!.Value;

        var r1 = await _.GetOutputAsync(id1,timeout:TimeSpan.FromSeconds(60)) as CommandResult; Check.That(r1).IsNotNull();

        Check.That(r1!.IsSuccess).IsTrue();

        var id2 = _.ExecuteCommand(new() { Handle = "BufferedConsoleDelegator" } )!.Value;

        var r2 = await _.GetOutputAsync(id2,timeout:TimeSpan.FromSeconds(60)) as CommandResult; Check.That(r2).IsNotNull();

        Check.That(r2!.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ExecuteConsoleDelegatorAsync()
    {
        CliWrap.Command concmd = Cli.Wrap("systeminfo");

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("ConsoleDelegator",new ConsoleDelegator(concmd))

            .RegisterCommand("BufferedConsoleDelegator",new ConsoleDelegator(concmd,buffered:true))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        var id1 = (await _.ExecuteCommandAsync(new() { Handle = "ConsoleDelegator" } ))!.Value;

        var r1 = await _.GetOutputAsync(id1,timeout:TimeSpan.FromSeconds(60)) as CommandResult; Check.That(r1).IsNotNull();

        Check.That(r1!.IsSuccess).IsTrue();

        var id2 = (await _.ExecuteCommandAsync(new() { Handle = "BufferedConsoleDelegator" } ))!.Value;

        var r2 = await _.GetOutputAsync(id2,timeout:TimeSpan.FromSeconds(60)) as CommandResult; Check.That(r2).IsNotNull();

        Check.That(r2!.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ExecuteConsoleDelegator_FreeMode_ActivityLifecycle()
    {
        CliWrap.Command concmd = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 2; Write-Output freemode-lifecycle\"");

        Tool tool = new();

        Check.That(await tool.RegisterCommand("ConsoleDelegatorLifecycle",new ConsoleDelegator(concmd,buffered:true))).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("ConsoleDelegatorLifecycle").SetFreeMode(true);

        Guid? id = await tool.ExecuteCommandAsync(details);

        Check.That(id).IsNotNull();

        await Task.Delay(50);

        var activitiesNow = tool.GetActivities();

        Check.That(activitiesNow).IsNotNull();
        Check.That(activitiesNow!.Any(a => a.ID == id)).IsTrue();

        await Task.Delay(TimeSpan.FromSeconds(8));

        var activitiesLater = tool.GetActivities();

        Check.That(activitiesLater).IsNull();

        var output = await tool.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(2));
        Check.That(output).IsInstanceOf<BufferedCommandResult>();

        var bcr = (BufferedCommandResult)output!;
        Check.That(bcr.IsSuccess).IsTrue();
        Check.That(bcr.StandardOutput.Contains("freemode-lifecycle",StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task ExecuteConsoleDelegatorBuffered_FreeMode()
    {
        CliWrap.Command concmd = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 1; Write-Output console-buffer-freemode\"");

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("BufferedConsoleDelegatorFreeMode",new ConsoleDelegator(concmd,buffered:true))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        CommandDetails d = CommandDetails.Create("BufferedConsoleDelegatorFreeMode").SetFreeMode(true);

        Guid? id = await _.ExecuteCommandAsync(d);

        Check.That(id).IsNotNull();

        Object? early = await _.GetOutputAsync(id,timeout:TimeSpan.FromMilliseconds(200));

        Check.That(early).IsNull();

        Object? result = await _.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(10));

        Check.That(result).IsInstanceOf<BufferedCommandResult>();

        BufferedCommandResult cr = (BufferedCommandResult)result!;

        Check.That(cr.IsSuccess).IsTrue();

        Check.That(cr.StandardOutput.Contains("console-buffer-freemode",StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task ExecuteConsoleDelegatorStreamedAsync()
    {
        CliWrap.Command concmd = Cli.Wrap("cmd").WithArguments("/c \"echo line1 & echo line2 & exit 0\"");

        List<String> _1 = new();

        ConsoleCommandObserver _2 = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = e =>
            {
                _1.Add(e.Text); return Task.CompletedTask;
            },

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("StreamedConsoleDelegator",new ConsoleDelegator(concmd,streamed:true,buffered:null,observer:_2))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        Guid? _3 = await _.ExecuteCommandAsync(new() { Handle = "StreamedConsoleDelegator" });

        Check.That(_3).IsNotNull();

        Object? _4 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromSeconds(30));

        Check.That(_4).IsInstanceOf<CommandResult>();

        CommandResult _5 = (CommandResult)_4!;

        Check.That(_5.IsSuccess).IsTrue();

        Check.That(_1.Count != 0).IsTrue();

        Check.That(_1.Any(l => l.Contains("line1",StringComparison.OrdinalIgnoreCase))).IsTrue();

        Check.That(_1.Any(l => l.Contains("line2",StringComparison.OrdinalIgnoreCase))).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegatorStreamedAsync()
    {
        CliWrap.Command _c = Cli.Wrap("cmd").WithArguments("/c \"echo binary-test & exit 0\"");

        List<Byte> _1 = new();

        BinaryCommandObserver _2 = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = async s =>
            {
                Byte[] _b = new Byte[8192];

                while(true)
                {
                    Int32 _r = await s.ReadAsync(_b).ConfigureAwait(false);

                    if(_r is 0) { break; }

                    _1.AddRange(_b);
                }
            },

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("StreamedBinaryConsoleDelegator",new BinaryConsoleDelegator(_c,observer:_2))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        Guid? _3 = await _.ExecuteCommandAsync(new() { Handle = "StreamedBinaryConsoleDelegator" });

        Check.That(_3).IsNotNull();

        Object? _4 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromSeconds(30));

        Check.That(_4).IsInstanceOf<CommandResult>();

        CommandResult _5 = (CommandResult)_4!;

        Check.That(_5.IsSuccess).IsTrue();

        Check.That(_1.Count != 0).IsTrue();

        String _s = System.Text.Encoding.Default.GetString(_1.ToArray());

        Check.That(_s.Contains("binary-test",StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegatorBufferedAsync()
    {
        CliWrap.Command _c = Cli.Wrap("cmd").WithArguments("/c \"echo binary-test & exit 0\"");

        BinaryCommandObserver _2 = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = _ => Task.CompletedTask,

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("BufferedBinaryConsoleDelegator",new BinaryConsoleDelegator(_c,observer:_2,buffered:true))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        Guid? _3 = await _.ExecuteCommandAsync(new() { Handle = "BufferedBinaryConsoleDelegator" });

        Check.That(_3).IsNotNull();

        Object? _4 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromSeconds(30));

        Check.That(_4).IsInstanceOf<BinaryCommandResult>();

        BinaryCommandResult _5 = (BinaryCommandResult)_4!;

        Check.That(_5.IsSuccess).IsTrue();

        Check.That(_5.StandardOutputBytes.Length != 0).IsTrue();

        String _s = System.Text.Encoding.Default.GetString(_5.StandardOutputBytes);

        Check.That(_s.Contains("binary-test",StringComparison.OrdinalIgnoreCase)).IsTrue();

        Check.That(_5.StandardErrorBytes.Length == 0).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegatorBuffered_FreeMode()
    {
        CliWrap.Command _c = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 1; Write-Output binary-buffer-freemode\"");

        BinaryCommandObserver _2 = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = _ => Task.CompletedTask,

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("BufferedBinaryConsoleDelegatorFreeMode",new BinaryConsoleDelegator(_c,observer:_2,buffered:true))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        CommandDetails d = CommandDetails.Create("BufferedBinaryConsoleDelegatorFreeMode").SetFreeMode(true);

        Guid? id = await _.ExecuteCommandAsync(d);

        Check.That(id).IsNotNull();

        Object? early = await _.GetOutputAsync(id,timeout:TimeSpan.FromMilliseconds(200));

        Check.That(early).IsNull();

        Object? result = await _.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(10));

        Check.That(result).IsInstanceOf<BinaryCommandResult>();

        BinaryCommandResult br = (BinaryCommandResult)result!;

        Check.That(br.IsSuccess).IsTrue();

        Check.That(br.StandardOutputBytes.Length != 0).IsTrue();

        String s = System.Text.Encoding.Default.GetString(br.StandardOutputBytes);

        Check.That(s.Contains("binary-buffer-freemode",StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_FreeMode_ActivityLifecycle()
    {
        CliWrap.Command concmd = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 2; Write-Output binary-freemode-lifecycle\"");

        BinaryCommandObserver observer = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = _ => Task.CompletedTask,

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        Tool tool = new();

        Check.That(await tool.RegisterCommand("BinaryConsoleDelegatorLifecycle",new BinaryConsoleDelegator(concmd,buffered:true,observer:observer))).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("BinaryConsoleDelegatorLifecycle").SetFreeMode(true);

        Guid? id = await tool.ExecuteCommandAsync(details);

        Check.That(id).IsNotNull();

        await Task.Delay(50);

        var activitiesNow = tool.GetActivities();

        Check.That(activitiesNow!.Any(a => a.ID == id)).IsTrue();

        await Task.Delay(TimeSpan.FromSeconds(12));

        var activitiesLater = tool.GetActivities(); Check.That(activitiesLater).IsNull();

        var output = await tool.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(2));
        Check.That(output).IsInstanceOf<BinaryCommandResult>();

        var bcr = (BinaryCommandResult)output!;
        Check.That(bcr.IsSuccess).IsTrue();
        Check.That(bcr.StandardOutputBytes.Length != 0).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_FreeModeAsync()
    {
        CliWrap.Command _c = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 3; Write-Output binary-freemode\"");

        List<Byte> _1 = new();

        BinaryCommandObserver _2 = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = async s =>
            {
                Byte[] _b = new Byte[8192];

                while(true)
                {
                    Int32 _r = await s.ReadAsync(_b).ConfigureAwait(false);

                    if(_r is 0) { break; }

                    _1.AddRange(_b.AsSpan(0,_r).ToArray());
                }
            },

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand(
                "FreeModeBinaryConsoleDelegator",
                new BinaryConsoleDelegator(_c,observer:_2,buffered:false))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        Guid? _3 = await _.ExecuteCommandAsync(new CommandDetails { Handle = "FreeModeBinaryConsoleDelegator" }.SetFreeMode(true));

        Check.That(_3).IsNotNull();

        Object? _4 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromMilliseconds(200));

        Check.That(_4).IsNull();

        Object? _5 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromSeconds(20));

        Check.That(_5).IsInstanceOf<CommandResult>();

        CommandResult _6 = (CommandResult)_5!;

        Check.That(_6.IsSuccess).IsTrue();

        Check.That(_1.Count != 0).IsTrue();

        String _s = System.Text.Encoding.Default.GetString(_1.ToArray());

        Check.That(_s.Contains("binary-freemode",StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task ExecuteConsoleDelegatorStreamed_FreeModeAsync()
    {
        CliWrap.Command concmd = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 3; Write-Output freemode-line\"");

        List<String> _1 = new();

        ConsoleCommandObserver _2 = new()
        {
            OnStarted = _ => Task.CompletedTask,

            OnStdOut = e =>
            {
                _1.Add(e.Text); return Task.CompletedTask;
            },

            OnStdErr = _ => Task.CompletedTask,

            OnExited = _ => Task.CompletedTask
        };

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("StreamedConsoleDelegatorFreeMode",new ConsoleDelegator(concmd,streamed:true,buffered:null,observer:_2))

            .Build();

        Check.That(await _.Activate()).IsTrue(); Check.That(await _.EnableAllCommands()).IsTrue();

        Guid? _3 = await _.ExecuteCommandAsync(new CommandDetails { Handle = "StreamedConsoleDelegatorFreeMode" }.SetFreeMode(true));

        Check.That(_3).IsNotNull();

        Object? _4 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromMilliseconds(200));

        Check.That(_4).IsNull();

        Object? _5 = await _.GetOutputAsync(_3,timeout:TimeSpan.FromSeconds(10));

        Check.That(_5).IsInstanceOf<CommandResult>();

        CommandResult _6 = (CommandResult)_5!;

        Check.That(_6.IsSuccess).IsTrue();

        Check.That(_1.Count != 0).IsTrue();

        Check.That(_1.Any(l => l.Contains("freemode-line",StringComparison.OrdinalIgnoreCase))).IsTrue();
    }

    [Test]
    public async Task ExecuteDynamicDelegator()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();
        
        CommandDetails _2 = CommandDetails.Create("DynamicDelegator").SetID(_4).SetFreeMode(true).PreserveActivity(true);

        DynamicDelegator _d = new(executor);

        Check.That(await _0.RegisterCommand("DynamicDelegator",_d)).IsTrue();

        Check.That(_0.ExecuteCommand(new(){})).IsNull();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(await _0.ExecuteCommandAsync(_2)).IsEqualTo(_4);

        var r = await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12),interval:1) as String;

        Check.That(_0.GetActivities()!.Any(a => a.ID == _4)).IsTrue();

        Check.That(String.Equals(r,"Lambda Complex")).IsTrue();
    }

    [Test]
    public async Task ExecuteDynamicDelegatorAsync()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _2 = new() { Handle = "DynamicDelegator" , ID = _4 };

        DynamicDelegator _d = new(executor);

        Check.That(await _0.RegisterCommand("DynamicDelegator",_d)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(await _0.ExecuteCommandAsync(_2)).IsEqualTo(_4);

        var r = await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12),interval:1) as String;

        Check.That(String.Equals(r,"Lambda Complex")).IsTrue();
    }

    [Test]
    public async Task ExecuteDynamicDelegator_SyncCtor()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _2 = new() { Handle = "DynamicDelegatorSync" , ID = _4 };

        DynamicDelegator _d = new(syncExecutor);

        Check.That(await _0.RegisterCommand("DynamicDelegatorSync",_d)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(_0.ExecuteCommand(_2)).IsEqualTo(_4);

        var r = await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12),interval:1) as String;

        Check.That(String.Equals(r,"Lambda Complex")).IsTrue();
    }

    [Test]
    public async Task ExecuteDynamicDelegator_TaskCtor()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _2 = new() { Handle = "DynamicDelegatorTask" , ID = _4 };

        DynamicDelegator _d = new(taskExecutor);

        Check.That(await _0.RegisterCommand("DynamicDelegatorTask",_d)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(await _0.ExecuteCommandAsync(_2)).IsEqualTo(_4);

        var r = await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12),interval:1) as String;

        Check.That(String.Equals(r,"Lambda Complex")).IsTrue();
    }

    Func<ITool,Activity,AccessKey,Task<Activity?>> taskExecutor = (tool,a,k) =>
    {
        String o = "Lambda Complex"; tool.AddOutput(a.ID,o);

        return Task.FromResult<Activity?>(a);
    };

    Func<ITool,Activity,AccessKey,ValueTask<Activity?>> executor = (tool,a,k) =>
    {
        String o = "Lambda Complex"; tool.AddOutput(a.ID,o);

        return ValueTask.FromResult<Activity?>(a);
    };

    Func<ITool,Activity,AccessKey,Activity?> syncExecutor = (tool,a,k) =>
    {
        String o = "Lambda Complex"; tool.AddOutput(a.ID,o);

        return a;
    };

    [Test]
    public async Task ExecuteToolDelegator()
    {
        Tool _0 = new(); Guid _4 = Guid.NewGuid(); Tool _10 = new();

        CommandDetails _21 = new() { Handle = "AltMultiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _22 = new() { Handle = "LinkMultiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new();
        CommandDelegator _7 = new("Multiply");
        CommandDelegator _9 = new("AltMultiply");
        CommandDelegator _11 = new("NextMultiply");

        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.RegisterCommand("AltMultiply",_7)).IsTrue();
        Check.That(await _0.RegisterCommand("NextMultiply",_9)).IsTrue();
        Check.That(await _0.RegisterCommand("LinkMultiply",_11)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(_0.ExecuteCommand(_21)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(_0.ExecuteCommand(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);

        Check.That(await _10.RegisterCommand("LinkMultiply",new ToolDelegator(_0))).IsTrue();

        Check.That(await _10.Activate()).IsTrue(); Check.That(await _10.EnableAllCommands()).IsTrue();

        Check.That(_10.ExecuteCommand(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _10.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteToolDelegatorAsync()
    {
        Tool _0 = new(); Guid _4 = Guid.NewGuid(); Tool _10 = new();

        CommandDetails _21 = new() { Handle = "AltMultiply" , ID = Guid.Empty , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        CommandDetails _22 = new() { Handle = "LinkMultiply" , ID = _4 , Arguments = new() { ["x"] = 483.660 , ["y"] = 660.483 } };
        Double _3 = 483.660*660.483;
        CommandTest _5 = new();
        CommandDelegator _7 = new("Multiply");
        CommandDelegator _9 = new("AltMultiply");
        CommandDelegator _11 = new("NextMultiply");

        Check.That(await _0.RegisterCommand("Multiply",_5)).IsTrue();
        Check.That(await _0.RegisterCommand("AltMultiply",_7)).IsTrue();
        Check.That(await _0.RegisterCommand("NextMultiply",_9)).IsTrue();
        Check.That(await _0.RegisterCommand("LinkMultiply",_11)).IsTrue();

        Check.That(await _0.Activate()).IsTrue(); Check.That(await _0.EnableAllCommands()).IsTrue();

        Check.That(await _0.ExecuteCommandAsync(_21)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(await _0.ExecuteCommandAsync(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);

        Check.That(await _10.RegisterCommand("LinkMultiply",new ToolDelegator(_0))).IsTrue();

        Check.That(await _10.Activate()).IsTrue(); Check.That(await _10.EnableAllCommands()).IsTrue();

        Check.That(await _10.ExecuteCommandAsync(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _10.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task GetCommand_ByHandle_ReturnsCommand_AndMissingNull()
    {
        Tool tool = new Tool();
        CommandTestE cmd = new();

        Check.That(await tool.RegisterCommand("GetCmd",cmd)).IsTrue();

        Check.That(tool.GetCommand("GetCmd")).IsSameReferenceAs(cmd);
        Check.That(tool.GetCommand("missing")).IsNull();
        Check.That(tool.GetCommand(String.Empty)).IsNull();
    }
}