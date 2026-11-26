using KusDepot.CatalogDb;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    private String? ActorID;

    private String GetActorID() => this.ActorID ??= this.GetPrimaryKeyString();

    private static Int64 ComputeLockKey(String catalog)
    {
        return BitConverter.ToInt64(SHA256.HashData(catalog.ToByteArrayFromUTF16String()),0);
    }

    private CatalogDBContext CreateContext(String catalog)
    {
        return this.ctxfactory.Create(this.dataSources.GetDataSource(this.dataSources.BuildConnectionString(catalog)));
    }
}