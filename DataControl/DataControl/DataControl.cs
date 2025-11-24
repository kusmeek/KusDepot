namespace KusDepot.Data;

public sealed partial class DataControl
{
    public readonly String URL;

    private readonly StatelessServiceContext Context;

    public DataControl(StatelessServiceContext context)
    {
        try { this.Context = context; SetupDiagnostics(); this.URL = GetURL(); }

        catch ( Exception _ ) { Log.Fatal(_,DataControlFail); Log.CloseAndFlush(); throw; }
    }
}