using Serilog;

namespace KusDepot.Dapr;

public class ManageJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            context.JobDetail.JobDataMap.TryGetValue("Manager",out Object? m);

            if(m is String manager) { await DaprSupervisor.Managers[manager].Manage(); }
        }
        catch ( Exception _ ) { Log.Error(_,JobExecutionFail); if(NoExceptions) throw; }
    }
}