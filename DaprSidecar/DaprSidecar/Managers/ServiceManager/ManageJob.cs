using Serilog;

namespace KusDepot.Dapr;

public class ManageJob : IJob
{
    public async ValueTask Execute(IJobExecutionContext context , CancellationToken cancel)
    {
        try
        {
            context.JobDetail.JobDataMap.TryGetValue("Manager",out Object? m);

            if(m is String manager) { await DaprSupervisor.Managers[manager].Manage(cancel); }
        }
        catch ( OperationCanceledException ) { throw; }

        catch ( Exception _ ) { Log.Error(_,JobExecutionFail); throw; }
    }
}