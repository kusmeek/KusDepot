namespace KusDepot.Dapr;

public interface IManager
{
    Task Manage(CancellationToken cancel = default);
}