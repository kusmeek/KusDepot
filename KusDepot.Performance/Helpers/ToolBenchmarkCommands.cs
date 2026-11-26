namespace KusDepot.Performance.Helpers;

public sealed class NoOpCommand : Command
{
    public NoOpCommand() : base() { }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(!EnabledAllowed(key)) { return null; }

        return null;
    }
}

public sealed class EnabledNoOpCommand : Command
{
    public EnabledNoOpCommand() : base() { }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(!EnabledAllowed(key)) { return null; }

        return null;
    }
}

public sealed class AsyncNoOpCommand : Command
{
    public AsyncNoOpCommand() : base() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(!EnabledAllowed(key)) { return null; }

        return null;
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        if(!EnabledAllowed(key)) { return null; }

        return null;
    }
}
