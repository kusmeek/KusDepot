namespace KusDepot.DaprActors;

internal static class DaprActorStrings
{
    public const String LogDirectory       = @"C:\KusDepotLogs\DaprToolActor\";

    public const String DaprToolActorFail  = @"DaprToolActor Failed {@ActorID}";
    public const String ExecutingCommand   = @"{@ActorID} Executing Handle {@Handle} ID {@ID}";
    public const String ExecuteCommandFail = @"ExecuteCommand Failed";
    public const String GetOutputStart     = @"{@ActorID} GetOutput ID {@ID}";
    public const String GetOutputFail      = @"{@ActorID} GetOutput Failed {@ID}";
    public const String OnActivate         = @"Activating ID {@ID}";
    public const String OnActivateFail     = @"OnActivate Failed {@ID}";
    public const String OnDeactivate       = @"Deactivating ID {@ID}";
    public const String OnDeactivateFail   = @"OnDeactivate Failed {@ID}";
    public const String RequestAccessStart = @"{@ActorID} RequestAccess";
    public const String RequestAccessFail  = @"RequestAccess Failed {@ID}";
    public const String RevokeAccessStart  = @"{@ActorID} RevokeAccess";
    public const String RevokeAccessFail   = @"RevokeAccess Failed {@ID}";
}