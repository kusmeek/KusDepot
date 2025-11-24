namespace KusDepot.Data;

internal sealed partial class Management
{
    private static void DeleteBlobActor(ActorId id)
    {
        new Thread( () =>
        {
            try { ActorServiceProxy.Create(ServiceLocators.BlobService,id).DeleteActorAsync(id,CancellationToken.None).GetAwaiter().GetResult(); } catch {}
        })
        .Start();
    }

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