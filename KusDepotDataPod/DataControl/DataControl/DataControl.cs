namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    public String URL { get; private set; }

    public DataControl()
    {
        try { this.URL = null!; SetURL(); SetupDiagnostics(); SetupTelemetry(); }

        catch ( Exception _ ) { Log.Fatal(_,DataControlFail); Log.CloseAndFlush(); throw; }
    }
}