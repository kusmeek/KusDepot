namespace CommandLine;

public sealed class ToolCommandLine
{
    private static Tool? Tool;

    static void Main(String[] args) { Tool = new Tool(); Tool.RegisterCommand(new MultiplyCommand(),"Multiply"); Tool.Activate(); CoconaLiteApp.Run<ToolCommandLine>(args); }

    [Command("Multiply")]
    public void Multiply([Argument]String[] args)
    {
        Int32? Waits = 0; Int32? WaitLimit = 50;

        List<String> _ = args.ToList(); _.Insert(0,"Multiply"); _.Insert(1,String.Empty); args = _.ToArray();

        Guid? OID = Tool!.ExecuteCommand(args).GetValueOrDefault();

        do { Task.Delay(100).Wait(); Waits++; if(Waits > WaitLimit) { return; } } while(Tool.GetOutputIDs() is null); 

        Console.WriteLine(Tool?.GetOutput(OID!.Value));
    }
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