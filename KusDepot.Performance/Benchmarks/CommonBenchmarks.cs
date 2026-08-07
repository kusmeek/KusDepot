namespace KusDepot.Performance.Benchmarks;

[MemoryDiagnoser]

public class CommonManagerCheckBenchmarks
{
    private X509Certificate2 Certificate = null!;
    private ManagerKey ManagerKey = null!;
    private TextItem CheckItem = null!;

    [GlobalSetup]
    public void Setup()
    {
        Certificate = BenchmarkData.CreateTestCertificate();
        ManagerKey = new ManagerKey(Certificate,Guid.NewGuid());
        CheckItem = BenchmarkData.CreateTextItem("CommonManagerBenchmark");

        if(!CheckItem.RegisterManager(ManagerKey))
        {
            throw new InvalidOperationException();
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Certificate?.Dispose();
    }

    [Benchmark(Description = "Common CheckManager")]
    public Boolean CheckManager()
    {
        return CheckItem.CheckManager(ManagerKey);
    }
}

[MemoryDiagnoser]
[SimpleJob(iterationCount:100)]

public class CommonBenchmarks
{
    private X509Certificate2 Certificate = null!;
    private ManagerKey ManagerKey = null!;
    private TextItem LockItem = null!;
    private TextItem RegisterItem = null!;
    private TextItem UnlockItem = null!;
    private TextItem UnregisterItem = null!;

    [GlobalSetup]
    public void Setup()
    {
        Certificate = BenchmarkData.CreateTestCertificate();
        ManagerKey = new ManagerKey(Certificate,Guid.NewGuid());
    }

    [IterationSetup(Target = nameof(RegisterManager))]
    public void SetupRegisterManager()
    {
        RegisterItem = BenchmarkData.CreateTextItem("CommonManagerBenchmark");
    }

    [IterationSetup(Target = nameof(Lock))]
    public void SetupLock()
    {
        LockItem = BenchmarkData.CreateTextItem("CommonManagerBenchmark");

        if(!LockItem.RegisterManager(ManagerKey))
        {
            throw new InvalidOperationException();
        }
    }

    [IterationSetup(Target = nameof(UnLock))]
    public void SetupUnlock()
    {
        UnlockItem = BenchmarkData.CreateTextItem("CommonManagerBenchmark");

        if(!UnlockItem.RegisterManager(ManagerKey) || !UnlockItem.Lock(ManagerKey))
        {
            throw new InvalidOperationException();
        }
    }

    [IterationSetup(Target = nameof(UnRegisterManager))]
    public void SetupUnregisterManager()
    {
        UnregisterItem = BenchmarkData.CreateTextItem("CommonManagerBenchmark");

        if(!UnregisterItem.RegisterManager(ManagerKey))
        {
            throw new InvalidOperationException();
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Certificate?.Dispose();
    }

    [Benchmark(Description = "Common RegisterManager" , Baseline = true)]
    public Boolean RegisterManager()
    {
        return RegisterItem.RegisterManager(ManagerKey);
    }

    [Benchmark(Description = "Common Lock")]
    public Boolean Lock()
    {
        return LockItem.Lock(ManagerKey);
    }

    [Benchmark(Description = "Common UnLock")]
    public Boolean UnLock()
    {
        return UnlockItem.UnLock(ManagerKey);
    }

    [Benchmark(Description = "Common UnRegisterManager")]
    public Boolean UnRegisterManager()
    {
        return UnregisterItem.UnRegisterManager(ManagerKey);
    }
}
