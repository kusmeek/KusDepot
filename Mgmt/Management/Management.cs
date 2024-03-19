namespace KusDepot.Data;

[StatePersistence(StatePersistence.None)]
internal sealed partial class Management : Actor , IManagement
{
    public Management(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    protected override Task OnActivateAsync() { Log.Information(Activated); return Task.FromResult(true); }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }
}