namespace ToolCmdlet;

[Cmdlet(VerbsLifecycle.Invoke,"Tool")]
public sealed class ToolCmdlet : PSCmdlet
{
    private Guid? OID;
    private Tool? Tool;
    private Int32? Waits = 0;
    private Int32? WaitLimit = 50;
    
    [Parameter(Position = 0)]
    public Object[]? Params{get;set;}

    protected override void BeginProcessing()
    {
        this.Tool = new Tool(); this.Tool.RegisterCommand(new MultiplyCommand(),"Multiply"); this.Tool.Activate();

        this.OID = this.Tool.ExecuteCommand(this.Params).GetValueOrDefault();

        do { Task.Delay(100).Wait(); this.Waits++; if(this.Waits > this.WaitLimit) { return; } } while(this.Tool.GetOutputIDs() is null); 
    }

    protected override void EndProcessing() { this.WriteObject(this.Tool?.GetOutput(this.OID!.Value)); }
}

internal sealed class MultiplyCommand : Command
{
    ITool? AttachedTool; List<Activity> Activities = new List<Activity>(); Boolean Enabled;

    public override Boolean Attach(ITool tool) { this.AttachedTool = tool; return true; }

    public override Boolean Detach() { this.AttachedTool = null; return true; }

    public override Boolean Disable() { this.Enabled = false; return true; }

    public override Boolean Enable() { this.Enabled = true; return true; }

    public override Activity? ExecuteAsync(Object[]? parameter)
    {
        try
        {
            if( parameter is null || !this.Enabled ) { throw new InvalidOperationException(); }

            Guid? id = parameter[1] as Guid?; id = id is null || id.Equals(Guid.Empty) ? Guid.NewGuid() : id;

            if( !Double.TryParse(parameter[2].ToString()!,out Double x) || !Double.TryParse(parameter[3].ToString()!,out Double y) ) { throw new InvalidOperationException(); }

            Activity a = new Activity(); a.ID = id; a.Task = Task.Run<Object?>( () => { return this.Multiply(a,x,y); } ); this.Activities.Add(a); return a;
        }
        catch ( Exception ) { this.AttachedTool?.AddOutput(parameter?[1] as Guid? ?? Guid.NewGuid(),null); return null; }
    }

    private Double? Multiply(Activity a , Double x , Double y) { Double result = x * y; this.AttachedTool?.AddOutput(a.ID ?? Guid.NewGuid(),result); this.CleanUp(a); return result; }

    private void CleanUp(Activity a) { this.Activities.Remove(a); this.AttachedTool?.RemoveActivity(a); }

    public override Dictionary<String , String> GetSpecifications()
    {
        throw new NotImplementedException();
    }
}