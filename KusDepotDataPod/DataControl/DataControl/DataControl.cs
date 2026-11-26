namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    public String URL { get; private set; } = String.Empty;

    public DataControl()
    {
        try { SetupDiagnostics(); }

        catch ( Exception _ ) { Log.Fatal(_,DataControlFail); Log.CloseAndFlush(); throw; }
    }
}