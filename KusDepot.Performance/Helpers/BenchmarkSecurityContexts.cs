namespace KusDepot.Performance.Benchmarks;

internal static class BenchmarkSecurityContexts
{
    internal static DataItemSecurityContext Create(DataItem item , ManagementKey key)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(key);

        Guid? id = item.GetID();
        if(id is null || id == Guid.Empty) { throw new InvalidOperationException("Benchmark item requires an ID before building a security context."); }

        X509Certificate2 certificate = Utility.DeserializeCertificate(key.Key) ?? throw new InvalidOperationException("Unable to deserialize management key certificate for benchmark context.");

        return DataItemSecurityContextFactory
            .ForObject(id.Value,certificate,"Benchmark")
            .Build();
    }
}
