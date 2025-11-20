namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.None)]
public class CommandExams
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
        Check.That(await _0.ExecuteCommandAsync(new(){})).IsNull();
        Check.That(await _0.ExecuteCommandAsync(_11)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);
        Check.That(await _0.Deactivate()).IsTrue();
        Check.That(await _0.ExecuteCommandAsync(_12)).IsNull();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(await _0.ExecuteCommandAsync(_12)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);
        Check.That(await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
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
        Check.That(await _0.Activate()).IsTrue();
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
        Check.That(await _0.Activate()).IsTrue();

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
        Check.That(await _0.Activate()).IsTrue();
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
        Check.That(await _0.Activate()).IsTrue();

        Check.That(await _0.ExecuteCommandCabAsync(_21.ToKusDepotCab())).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(await _0.ExecuteCommandCabAsync(_22.ToKusDepotCab())).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }

    [Test]
    public async Task ExecuteConsoleDelegator()
    {
        CliWrap.Command concmd = Cli.Wrap("systeminfo");

        ITool _ = ToolBuilderFactory.CreateBuilder()

            .RegisterCommand("ConsoleDelegator",new ConsoleDelegator(concmd))

            .RegisterCommand("BufferedConsoleDelegator",new ConsoleDelegator(concmd,true))

            .Build();

        Check.That( await _.Activate() ).IsTrue();

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

            .RegisterCommand("BufferedConsoleDelegator",new ConsoleDelegator(concmd,true))

            .Build();

        Check.That( await _.Activate() ).IsTrue();

        var id1 = (await _.ExecuteCommandAsync(new() { Handle = "ConsoleDelegator" } ))!.Value;

        var r1 = await _.GetOutputAsync(id1,timeout:TimeSpan.FromSeconds(60)) as CommandResult; Check.That(r1).IsNotNull();

        Check.That(r1!.IsSuccess).IsTrue();

        var id2 = (await _.ExecuteCommandAsync(new() { Handle = "BufferedConsoleDelegator" } ))!.Value;

        var r2 = await _.GetOutputAsync(id2,timeout:TimeSpan.FromSeconds(60)) as CommandResult; Check.That(r2).IsNotNull();

        Check.That(r2!.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ExecuteDynamicDelegator()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _2 = new() { Handle = "DynamicDelegator" , ID = _4 };

        DynamicDelegator _d = new(executor);

        Check.That(await _0.RegisterCommand("DynamicDelegator",_d)).IsTrue();

        Check.That(_0.ExecuteCommand(new(){})).IsNull();

        Check.That(await _0.Activate()).IsTrue();

        Check.That(_0.ExecuteCommand(_2)).IsEqualTo(_4);

        var r = await _0.GetOutputAsync(_4,timeout:TimeSpan.FromSeconds(12),interval:1) as String;

        Check.That(String.Equals(r,"Lambda Complex")).IsTrue();
    }

    [Test]
    public async Task ExecuteDynamicDelegatorAsync()
    {
        Tool _0 = new Tool(); Guid _4 = Guid.NewGuid();

        CommandDetails _2 = new() { Handle = "DynamicDelegator" , ID = _4 };

        DynamicDelegator _d = new(executor);

        Check.That(await _0.RegisterCommand("DynamicDelegator",_d)).IsTrue();

        Check.That(await _0.Activate()).IsTrue();

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

        Check.That(await _0.Activate()).IsTrue();

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

        Check.That(await _0.Activate()).IsTrue();

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

        Check.That(await _0.Activate()).IsTrue();

        Check.That(_0.ExecuteCommand(_21)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(_0.ExecuteCommand(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);

        Check.That(await _10.RegisterCommand("LinkMultiply",new ToolDelegator(_0))).IsTrue();

        Check.That(await _10.Activate()).IsTrue();

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

        Check.That(await _0.Activate()).IsTrue();

        Check.That(await _0.ExecuteCommandAsync(_21)).IsInstanceOf<Guid?>().And.IsNotNull().And.IsNotEqualTo<Guid?>(Guid.Empty);

        Check.That(await _0.ExecuteCommandAsync(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _0.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);

        Check.That(await _10.RegisterCommand("LinkMultiply",new ToolDelegator(_0))).IsTrue();

        Check.That(await _10.Activate()).IsTrue();

        Check.That(await _10.ExecuteCommandAsync(_22)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);

        Check.That(await _10.GetRemoveOutputAsync(_4,timeout:TimeSpan.FromSeconds(12))).IsEqualTo(_3);
    }
}