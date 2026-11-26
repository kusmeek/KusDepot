namespace DataPodServices;

internal sealed class SiloProbeHealthState : ILifecycleParticipant<ISiloLifecycle>
{
    private Int32 HasReachedActiveStage;

    private Int32 IsActive; private Int32 IsStopping;

    private Int32 RequiresDataConfigsSnapshot;

    private Int32 HasDataConfigsSnapshot;

    public Boolean IsLive => Volatile.Read(ref IsStopping) == 0;

    public Boolean HasStarted => Volatile.Read(ref HasReachedActiveStage) == 1;

    public Boolean IsReady => Volatile.Read(ref IsActive) == 1 && IsLive && (Volatile.Read(ref RequiresDataConfigsSnapshot) == 0 || Volatile.Read(ref HasDataConfigsSnapshot) == 1);

    public void RequireDataConfigsSnapshot()
    {
        Volatile.Write(ref RequiresDataConfigsSnapshot, 1);
    }

    public void SetDataConfigsSnapshotReady(Boolean isReady)
    {
        Volatile.Write(ref HasDataConfigsSnapshot, isReady ? 1 : 0);
    }

    private void MarkNotReady()
    {
        Volatile.Write(ref IsActive,0);
    }

    public void MarkStopping()
    {
        Volatile.Write(ref IsStopping,1);

        MarkNotReady();
    }

    public void Participate(ISiloLifecycle lifecycle)
    {
        lifecycle.Subscribe(nameof(SiloProbeHealthState),
            ServiceLifecycleStage.Active,

            _ =>
            {
                Volatile.Write(ref HasReachedActiveStage,1);

                Volatile.Write(ref IsActive,1);

                return Task.CompletedTask;
            },

            _ =>
            {
                MarkNotReady();

                return Task.CompletedTask;
            });
    }

}
