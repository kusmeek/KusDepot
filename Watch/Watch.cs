namespace Watch;

[StatePersistence(StatePersistence.Persisted)]
internal sealed class Watch : Actor , IRemindable , IWatch
{
    private DateTimeOffset? Started; private DateTimeOffset? Stopped;

    public Watch(ActorService service,ActorId id) : base(service,id) { }

    public Task<TimeSpan?> GetElapsed()
    {
        if(this.Started is null) { return Task.FromResult<TimeSpan?>(null); }

        return Task.FromResult(this.Stopped.HasValue ? this.Stopped - this.Started : DateTimeOffset.Now - this.Started);
    }

    public Task<DateTimeOffset?> GetStarted() { return Task.FromResult(this.Started); }

    public Task<DateTimeOffset?> GetStopped() { return Task.FromResult(this.Stopped); }

    public Task<DateTimeOffset> GetTime() { return Task.FromResult(DateTimeOffset.Now); }

    protected override Task OnActivateAsync()
    {
        return Task.Run(() => { this.RegisterReminderAsync("Watch",Array.Empty<Byte>(),TimeSpan.FromMilliseconds(0),TimeSpan.FromHours(1)); });
    }

    public Task ReceiveReminderAsync(String n , Byte[] s , TimeSpan d , TimeSpan p) { return this.UnregisterReminderAsync(this.GetReminder("Watch")); }

    public Task<Boolean> Reset() { this.Started = null; this.Stopped = null; return Task.FromResult(true); }

    public Task<Boolean> Start() { this.Started = DateTimeOffset.Now; return Task.FromResult(true); }

    public Task<Boolean> Stop() { this.Stopped = DateTimeOffset.Now; return Task.FromResult(true); }
}