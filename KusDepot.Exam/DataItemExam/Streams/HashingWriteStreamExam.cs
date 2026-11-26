namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class HashingWriteStreamExam
{
    [Test]
    public async Task WriteAsync_Computes_SHA512_For_Written_Bytes()
    {
        Byte[] payload = Enumerable.Range(0,64).Select(i => unchecked((Byte)(0x20 + i))).ToArray();
        using MemoryStream destination = new();
        using HashingWriteStream stream = new(destination);

        await stream.WriteAsync(payload,0,payload.Length,CancellationToken.None);
        await stream.FlushAsync(CancellationToken.None);

        Byte[] hash = stream.GetHashAndReset();

        Assert.That(destination.ToArray(), Is.EqualTo(payload));
        Assert.That(hash, Is.EqualTo(SHA512.HashData(payload)));
    }

    [Test]
    public void WriteByte_Computes_SHA512_For_Written_Bytes()
    {
        Byte[] payload = Enumerable.Range(0,17).Select(i => unchecked((Byte)(0x40 + i))).ToArray();
        using MemoryStream destination = new();
        using HashingWriteStream stream = new(destination);

        foreach(Byte value in payload)
        {
            stream.WriteByte(value);
        }

        Byte[] hash = stream.GetHashAndReset();

        Assert.That(destination.ToArray(), Is.EqualTo(payload));
        Assert.That(hash, Is.EqualTo(SHA512.HashData(payload)));
    }

    [Test]
    public void GetHashAndReset_Throws_When_Called_Twice()
    {
        using MemoryStream destination = new();
        using HashingWriteStream stream = new(destination);

        stream.WriteByte(0x7A);
        _ = stream.GetHashAndReset();

        Assert.That(() => stream.GetHashAndReset(), Throws.TypeOf<InvalidOperationException>());
    }
}
