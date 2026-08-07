namespace KusDepot.Performance.Helpers;

public static class BenchmarkData
{
    public static Byte[] CreatePayload(Int32 size)
    {
        Byte[] data = new Byte[size];

        RandomNumberGenerator.Fill(data);

        return data;
    }

    public static X509Certificate2 CreateTestCertificate(Int32 rsakeysize = 4096)
    {
        return Utility.CreateCertificate(Guid.NewGuid(),"CN=KusDepot-Benchmark")!;
    }

    public static TextItem CreateTextItem(String? content = null)
    {
        return new TextItem(content ?? "Benchmark content" , null , Guid.NewGuid() , "BenchmarkItem" , ["note1","note2"] , ["tag1","tag2"] , null , "en-US");
    }

    public static BinaryItem CreateBinaryItem(Byte[]? content = null)
    {
        return new BinaryItem(content ?? CreatePayload(1024) , null , null , Guid.NewGuid() , "BinaryBenchmark" , ["note1"] , ["tag1"] , null);
    }

    public const Int32 Size_1KB   = 1024;
    public const Int32 Size_64KB  = 64 * 1024;
    public const Int32 Size_1MB   = 1024 * 1024;
    public const Int32 Size_16MB  = 16 * 1024 * 1024;
}
