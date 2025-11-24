namespace KusDepot.Test;

#pragma warning disable CS1591
public sealed class CalculateCommand : Command
{
    public CalculateCommand() : base() { ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(this.Enabled is false) { return null; }

        try
        {
            try
            {
                AddActivity(a);

                String? op = a.Details?.GetArgument<String>("Operation");

                Double? x = a.Details?.GetArgument<Double?>("x"); Double? y = a.Details?.GetArgument<Double?>("y");

                if(op is null || x is null || y is null) { AddOutput(a.ID); return null; }

                switch(op)
                {
                    case "Add": this.Add(a,x.Value,y.Value); break;

                    case "Subtract": this.Subtract(a,x.Value,y.Value); break;

                    case "Multiply": this.Multiply(a,x.Value,y.Value); break;

                    case "Divide": this.Divide(a,x.Value,y.Value); break;

                    default: AddOutput(a.ID); return null;
                }

                return a.ID;
            }
            catch { AddOutput(a.ID); return null; }

            finally { this.CleanUp(a); }
        }
        catch { AddOutput(a?.ID); CleanUp(a); return null; }
    }

    ///<inheritdoc/>
    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(this.Enabled is false) { return null; }

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    String? op = a.Details?.GetArgument<String>("Operation");

                    Double? x = a.Details?.GetArgument<Double?>("x"); Double? y = a.Details?.GetArgument<Double?>("y");

                    if(op is null || x is null || y is null) { AddOutput(a.ID); return false; }

                    switch(op)
                    {
                        case "Add": this.Add(a,x.Value,y.Value); return true;

                        case "Subtract": this.Subtract(a,x.Value,y.Value); return true;

                        case "Multiply": this.Multiply(a,x.Value,y.Value); return true;

                        case "Divide": this.Divide(a,x.Value,y.Value); return true;

                        default: AddOutput(a.ID); await Task.CompletedTask; return false;
                    }
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch { AddOutput(a.ID); return false; }

                finally { this.CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch { AddOutput(a?.ID); CleanUp(a); return null; }
    }

    private void Add(Activity a , Double x , Double y) { Double result = x + y; AddOutput(a.ID,result); }

    private void Divide(Activity a , Double x , Double y) { Double result = x / y; AddOutput(a.ID,result); }

    private void Multiply(Activity a , Double x , Double y) { Double result = x * y; AddOutput(a.ID,result); }

    private void Subtract(Activity a , Double x , Double y) { Double result = x - y; AddOutput(a.ID,result); }
}