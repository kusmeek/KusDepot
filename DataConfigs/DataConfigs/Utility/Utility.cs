namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    private static void DeleteSecureActor(ActorId id)
    {
        new Thread( () =>
        {
            try { ActorServiceProxy.Create(ServiceLocators.SecureService,id).DeleteActorAsync(id,CancellationToken.None).GetAwaiter().GetResult(); } catch {}
        })
        .Start();
    }

    private String GetActorID()
    {
        switch(this.Id.Kind)
        {
            case ActorIdKind.Guid:   return this.Id.GetGuidId().ToString("N").ToUpperInvariant();
            case ActorIdKind.Long:   return this.Id.GetLongId().ToStringInvariant()!;
            case ActorIdKind.String: return this.Id.GetStringId();
            default:                 return String.Empty;
        }
    }
}