using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<HashSet<StorageSilo>?> GetStorageSilos(String token , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Logger.Error(GetSilosAuth); SetErr(__); return null; }

            if(await Security.IsAdmin(token,cancel) is false) { Logger.Error(GetSilosAuth); SetErr(__); return null; }

            await this.ReadStateAsync();

            if(State.Silos is not null && State.Silos.Count > 0) { Logger.Information(GetSilosSuccess); SetOk(__); return State.Silos; }

            Logger.Error(GetSilosEmpty); SetErr(__); return null;
        }
        catch ( Exception _ ) { Logger.Error(_,GetSilosFail); return null; }
    }
}