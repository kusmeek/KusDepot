namespace KusDepot.Test;

public sealed class MathCommand : Command
{
    public MathCommand() : base() { ExecutionMode.AllowBoth(); }

    public override CommandDescriptor? GetCommandDescriptor(CommandKey? key = null)
    {
        if(Locked && AccessCheck(key) is false) { return null; }

        return new CommandDescriptor
        {
            Type = GetType().FullName,
            Specifications = "Arithmetic command supporting Add, Subtract, Multiply, and Divide operations via Activity arguments Operation, x, and y.",
            ExtendedData = ["Argument: Operation (String) — Add, Subtract, Multiply, Divide", "Argument: x (Double)", "Argument: y (Double)", "Output: Double result keyed by Activity ID"]
        };
    }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            try
            {
                AddActivity(a);

                String? op = a.Details?.GetArgument<String>("Operation");
                Double? x = a.Details?.GetArgument<Double?>("x");
                Double? y = a.Details?.GetArgument<Double?>("y");

                if(op is null || x is null || y is null) { SetFailed(a); AddOutput(a.ID); return null; }

                Double result = op switch
                {
                    "Add"      => x.Value + y.Value,
                    "Subtract" => x.Value - y.Value,
                    "Multiply" => x.Value * y.Value,
                    "Divide"   => x.Value / y.Value,
                    _          => Double.NaN
                };

                if(Double.IsNaN(result)) { SetFailed(a); AddOutput(a.ID); return null; }

                AddOutput(a.ID,result); SetSuccess(a); return a.ID;
            }
            catch ( Exception _ ) { SetFaulted(a,_); AddOutput(a.ID); return null; }

            finally { CleanUp(a); }
        }
        catch ( Exception _ ) { SetFaulted(a,_); AddOutput(a?.ID); CleanUp(a); return null; }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a);

            if(a.FreeMode is true) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception _ ) { SetFaulted(a,_); AddOutput(a?.ID); CleanUp(a); return null; }

        finally { CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        try
        {
            AddActivity(a);

            String? op = a.Details?.GetArgument<String>("Operation");
            Double? x = a.Details?.GetArgument<Double?>("x");
            Double? y = a.Details?.GetArgument<Double?>("y");

            if(op is null || x is null || y is null) { SetFailed(a); AddOutput(a.ID); return false; }

            Double result = op switch
            {
                "Add"      => x.Value + y.Value,
                "Subtract" => x.Value - y.Value,
                "Multiply" => x.Value * y.Value,
                "Divide"   => x.Value / y.Value,
                _          => Double.NaN
            };

            if(Double.IsNaN(result)) { SetFailed(a); AddOutput(a.ID); return false; }

            AddOutput(a.ID,result); SetSuccess(a); return true;
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID); return false; }

        catch ( Exception _ ) { SetFaulted(a,_); AddOutput(a.ID); return false; }

        finally { CleanUpFreeMode(a); await Task.CompletedTask; }
    }
}
