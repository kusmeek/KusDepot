namespace CommandLine;

public sealed class ToolCommandLine
{
    private static ITool? Tool;

    static async Task Main(String[] args)
    {
        var b = ToolBuilderFactory.CreateBuilder().AutoStart()

        .RegisterCommand<MultiplyCommand>("Multiply");

        Tool = await b.BuildAsync();

        await CoconaLiteApp.RunAsync<ToolCommandLine>(args); }

    [Command("Multiply")]
    public void Multiply([Argument]String[] args)
    {
        if(args is null) { return; }

        Int32? Waits = 0; Int32? WaitLimit = 50;

        Guid? OID = Tool!.ExecuteCommand(new() { Handle = "Multiply" , Arguments = new() { ["x"] = Double.Parse(args[0],CultureInfo.InvariantCulture) , ["y"] = Double.Parse(args[1],CultureInfo.InvariantCulture) } }).GetValueOrDefault();

        do { Task.Delay(100).Wait(); Waits++; if(Waits > WaitLimit) { return; } } while(Tool.GetOutputIDs() is null); 

        Console.WriteLine(Tool?.GetOutput(OID!.Value));
    }
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