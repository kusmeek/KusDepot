namespace KusDepot.Performance.Benchmarks;

[MemoryDiagnoser]

public class DataItemBenchmarks
{
    private BinaryItem EncryptionItem = null!;
    private BinaryItem DecryptionItem = null!;
    private ManagementKey ManagementKey = null!;
    private ManagementKey EncryptionManagementKey = null!;
    private ManagementKey DecryptionManagementKey = null!;
    private DataItemSecurityContext SecurityContext = null!;
    private DataItemSecurityContext EncryptionSecurityContext = null!;
    private DataItemSecurityContext DecryptionSecurityContext = null!;
    private BinaryItem SignedBinaryItem = null!;

    public Int32 ContentSize = BenchmarkData.Size_16MB;

    [GlobalSetup]
    public void Setup()
    {
        SignedBinaryItem = BenchmarkData.CreateBinaryItem(BenchmarkData.CreatePayload(ContentSize));
        ManagementKey = SignedBinaryItem.CreateManagementKey("Benchmark")!;
        SecurityContext = BenchmarkSecurityContexts.Create(SignedBinaryItem,ManagementKey);
        SignedBinaryItem.SetContent(SignedBinaryItem.GetContent()! , SecurityContext);
    }

    [IterationSetup(Target = nameof(BinaryItem_EncryptData))]
    public void SetupEncryption()
    {
        EncryptionItem = BenchmarkData.CreateBinaryItem(BenchmarkData.CreatePayload(ContentSize));
        EncryptionManagementKey = EncryptionItem.CreateManagementKey("Benchmark")!;
        EncryptionSecurityContext = BenchmarkSecurityContexts.Create(EncryptionItem,EncryptionManagementKey);
    }

    [IterationSetup(Target = nameof(BinaryItem_DecryptData))]
    public void SetupDecryption()
    {
        DecryptionItem = BenchmarkData.CreateBinaryItem(BenchmarkData.CreatePayload(ContentSize));
        DecryptionManagementKey = DecryptionItem.CreateManagementKey("Benchmark")!;
        DecryptionSecurityContext = BenchmarkSecurityContexts.Create(DecryptionItem,DecryptionManagementKey);

        _ = DecryptionItem.EncryptData(DecryptionSecurityContext).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    [Benchmark(Description = "BinaryItem SignData")]
    public async Task<String?> BinaryItem_SignData()
    {
        return await SignedBinaryItem.SignData("Content" , SecurityContext).ConfigureAwait(false);
    }

    [Benchmark(Description = "BinaryItem VerifyData")]
    public async Task<Boolean> BinaryItem_VerifyData()
    {
        return await SignedBinaryItem.VerifyData("Content" , SecurityContext).ConfigureAwait(false);
    }

    [Benchmark(Description = "BinaryItem EncryptData")]
    public async Task<Boolean> BinaryItem_EncryptData()
    {
        return await EncryptionItem.EncryptData(EncryptionSecurityContext).ConfigureAwait(false);
    }

    [Benchmark(Description = "BinaryItem DecryptData")]
    public async Task<Boolean> BinaryItem_DecryptData()
    {
        return await DecryptionItem.DecryptData(DecryptionSecurityContext).ConfigureAwait(false);
    }
}
