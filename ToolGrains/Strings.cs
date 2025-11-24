namespace KusDepot.ToolGrains;

internal static class ToolSiloStrings
{
    public const String LogDirectory       = @"C:\KusDepotLogs\ToolGrains\";

    public const String ExecutingCommand   = @"{@GrainPrimaryKey} Executing Handle {@Handle} ID {@ID}";
    public const String ExecuteCommandFail = @"ExecuteCommand Failed";
    public const String GetOutputStart     = @"{@GrainPrimaryKey} GetOutput ID {@ID}";
    public const String GetOutputFail      = @"{@GrainPrimaryKey} GetOutput Failed {@ID}";
    public const String HostProcessExit    = @"ToolSilo Host Process Exiting {@PID}";
    public const String OnActivate         = @"Activating {@GrainPrimaryKey}";
    public const String OnActivateFail     = @"OnActivate Failed {@ID}";
    public const String OnDeactivate       = @"Deactivating {@GrainPrimaryKey} {@ReasonCode} {@ReasonDescription} {@ReasonException}";
    public const String OnDeactivateFail   = @"OnDeactivate Failed {@ID} {@ReasonCode} {@ReasonDescription} {@ReasonException}";
    public const String RegisterSuccess    = @"ToolSilo Service Registration Succeeded {@PID}";
    public const String RequestAccessStart = @"{@GrainPrimaryKey} RequestAccess";
    public const String RequestAccessFail  = @"RequestAccess Failed {@ID}";
    public const String RevokeAccessStart  = @"{@GrainPrimaryKey} RevokeAccess";
    public const String RevokeAccessFail   = @"RevokeAccess Failed {@ID}";
    public const String StartUpFail        = @"ToolSilo Service StartUp Failed";
    public const String ToolGrainFail      = @"ToolGrain Service Failed {@GrainPrimaryKey}";
    public const String ToolSiloClosing    = @"ToolSilo Service Closing";
    public const String ToolSiloFail       = @"ToolSilo Service Failed";
    public const String ToolSiloStarted    = @"ToolSilo Server Started";
    public const String ToolSiloStopped    = @"ToolSilo Server Stopped";
}