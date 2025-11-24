namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    private static Int64 ComputeLockKey(String catalog)
    {
        return BitConverter.ToInt64(SHA256.HashData(catalog.ToByteArrayFromUTF16String()),0);
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

    private String BuildConnectionString(String catalog)
    {
        String? cstring = this.Config?[$"ConnectionStrings:{catalog}"];

        if(String.IsNullOrWhiteSpace(cstring)) { cstring = this.Config?["ConnectionStrings:Default"]; }
        if(String.IsNullOrWhiteSpace(cstring) is false) { return cstring!; }

        var host = this.Config?["Server:Address"] ?? "127.0.0.1";
        var port = this.Config?["Server:Port"] ?? "5432";
        var user = this.Config?["Server:PGUSER"];
        var pass = this.Config?["Server:PGPASSWORD"];

        return $"Host={host};Port={port};Database={catalog};Username={user};Password={pass};Pooling=true;Maximum Pool Size=50";
    }
}