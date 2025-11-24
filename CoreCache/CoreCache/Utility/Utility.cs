namespace KusDepot.Data;

internal sealed partial class CoreCache
{
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