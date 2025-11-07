namespace KusDepot.Data;

public sealed partial class DataControl
{
    private static void DeleteBlobActor(ActorId id)
    {
        new Thread( () =>
        {
            try { ActorServiceProxy.Create(ServiceLocators.BlobService,id).DeleteActorAsync(id,CancellationToken.None).GetAwaiter().GetResult(); } catch {}
        })
        .Start();
    }

    private String GetURL() { return $"https://{this.Context.NodeContext.IPAddressOrFQDN}:{this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port}"; }

    private Int32 GetPort() { if(Int32.TryParse(this.URL.Split(':').ElementAt(2),out Int32 p)) { return p; } return this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port; }

    private static String GetToken(HttpContext context) => context.Request.Headers.Authorization.ToString().StartsWith("Bearer ",Ordinal) is false ? String.Empty : context.Request.Headers.Authorization.ToString()[7..];
}