namespace KusDepot.Exams;

[TestFixture]
public partial class UtilityExam
{
    [Test]
    public void Compress_Null_Empty()
    {
        Byte[]? n = null;
        Check.That(n.Compress()).IsNull();
        Byte[] empty = Array.Empty<Byte>();
        Check.That(empty.Compress()).IsNotNull().And.HasSize(0);
        Check.That(empty.Decompress()).IsNotNull().And.HasSize(0);
    }

    [Test]
    public void Compress_RoundTrip_Small()
    {
        Byte[] data = RandomBytes(256);
        Byte[]? comp = data.Compress();
        Check.That(comp).IsNotNull();
        Byte[]? decomp = comp.Decompress();
        Check.That(decomp).IsNotNull();
        Check.That(decomp!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void Compress_RoundTrip_Medium()
    {
        Byte[] data = Encoding.UTF8.GetBytes(new String('A', 50_000));
        Byte[]? comp = data.Compress();
        Check.That(comp).IsNotNull();
        Check.That(comp!.Length).IsStrictlyLessThan(data.Length);
        Byte[]? decomp = comp.Decompress();
        Check.That(decomp).IsNotNull();
        Check.That(decomp!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void Compress_RoundTrip_Incompressible()
    {
        Byte[] data = RandomBytes(64_000);
        Byte[]? comp = data.Compress();
        Check.That(comp).IsNotNull();
        Byte[]? decomp = comp.Decompress();
        Check.That(decomp).IsNotNull();
        Check.That(decomp!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void Stream_Compress_Decompress()
    {
        Byte[] data = Encoding.UTF8.GetBytes(new String('B', 200_000));
        using MemoryStream input = new(data, writable:false);
        using MemoryStream compressed = new();
        Boolean ok = Compress(input, compressed);
        Check.That(ok).IsTrue();
        Byte[] compBytes = compressed.ToArray();
        Check.That(compBytes.Length).IsStrictlyLessThan(data.Length);
        using MemoryStream compIn = new(compBytes, writable:false);
        using MemoryStream output = new();
        Boolean dok = Decompress(compIn, output);
        Check.That(dok).IsTrue();
        Check.That(output.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public async Task Stream_CompressAsync_DecompressAsync()
    {
        Byte[] data = RandomBytes(300_000);
        using MemoryStream input = new(data, writable:false);
        using MemoryStream compressed = new();
        Boolean ok = await CompressAsync(input, compressed);
        Check.That(ok).IsTrue();
        using MemoryStream compIn = new(compressed.ToArray(), writable:false);
        using MemoryStream output = new();
        Boolean dok = await DecompressAsync(compIn, output);
        Check.That(dok).IsTrue();
        Check.That(output.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public async Task CompressAsync_Cancellation()
    {
        Byte[] data = RandomBytes(500_000);
        using MemoryStream input = new(data, writable:false);
        using MemoryStream output = new();
        using CancellationTokenSource cts = new();
        cts.Cancel();
        Boolean ok = await CompressAsync(input, output, cts.Token);
        Check.That(ok).IsFalse();
    }

    [Test]
    public async Task DecompressAsync_Cancellation()
    {
        Byte[] data = Encoding.UTF8.GetBytes(new String('C', 150_000));
        using MemoryStream input = new(data, writable:false);
        using MemoryStream comp = new();
        Check.That(Compress(input, comp)).IsTrue();
        using MemoryStream compIn = new(comp.ToArray(), writable:false);
        using MemoryStream output = new();
        using CancellationTokenSource cts = new(); cts.Cancel();
        Boolean ok = await DecompressAsync(compIn, output, cts.Token);
        Check.That(ok).IsFalse();
    }
}
