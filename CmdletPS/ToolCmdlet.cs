namespace ToolCmdlet;

[Cmdlet(VerbsLifecycle.Invoke,"Tool")]
public sealed class ToolCmdlet : PSCmdlet
{
    private Guid? OID;
    private ITool? Tool;
    private Int32? Waits = 0;
    private Int32? WaitLimit = 50;

    [Parameter(Position = 0)]
    public Object[]? Params{get;set;}

    protected override void BeginProcessing()
    {
        var b = ToolBuilderFactory.CreateBuilder().AutoStart()

        .RegisterCommand<MultiplyCommand>("Multiply");

        this.Tool = b.Build();

        if(Params is null ) { return; }

        String? h = Params.ElementAt(0) as String;
        Double.TryParse(Params.ElementAt(2).ToString(),null,out Double x);
        Double.TryParse(Params.ElementAt(3).ToString(),null,out Double y);
        Guid i = Guid.TryParse(Params.ElementAt(1)?.ToString(),null,out i) ? i : Guid.Empty;

        this.OID = this.Tool.ExecuteCommand(new() { Handle = h , ID = i , Arguments = new() { ["x"] = x , ["y"] = y } }).GetValueOrDefault();

        do { Task.Delay(100).Wait(); this.Waits++; if(this.Waits > this.WaitLimit) { return; } } while(this.Tool.GetOutputIDs() is null); 
    }

    protected override void EndProcessing() { this.WriteObject(this.Tool?.GetOutput(this.OID!.Value)); }
}

internal sealed class MultiplyCommand : Command
{
    public MultiplyCommand() : base() { ExecutionMode.AllowSynchronousOnly(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            try
            {
                AddActivity(a);

                var x = a.Details?.GetArgument<Double?>("x"); var y = a.Details?.GetArgument<Double?>("y"); if(x is null || y is null) { AddOutput(a.ID); return null; }

                this.Multiply(a,x.Value,y.Value); return a.ID;
            }
            catch { AddOutput(a.ID); return null; }

            finally { this.CleanUp(a); }
        }
        catch { AddOutput(a?.ID); CleanUp(a); return null; }
    }

    private void Multiply(Activity a , Double x , Double y) { Double r = x * y; this.AttachedTool?.AddOutput(a.ID ?? Guid.NewGuid(),r,Key); }
}