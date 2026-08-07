namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class HashingReadStreamExam
{
    [Test]
    public async Task ReadAsync_Computes_SHA512_For_Read_Bytes()
    {
        Byte[] payload = Enumerable.Range(0,64).Select(i => unchecked((Byte)(0x18 + i))).ToArray();
        using MemoryStream source = new(payload,false);
        using HashingReadStream stream = new(source);
        Byte[] buffer = new Byte[13];
        using MemoryStream copy = new();

        while(true)
        {
            Int32 read = await stream.ReadAsync(buffer,0,buffer.Length,CancellationToken.None);

            if(read == 0) { break; }

            await copy.WriteAsync(buffer.AsMemory(0,read),CancellationToken.None);
        }

        Byte[] hash = stream.GetHashAndReset();

        Assert.That(copy.ToArray(), Is.EqualTo(payload));
        Assert.That(hash, Is.EqualTo(SHA512.HashData(payload)));
    }

    [Test]
    public void ReadByte_Computes_SHA512_For_Read_Bytes()
    {
        Byte[] payload = Enumerable.Range(0,19).Select(i => unchecked((Byte)(0x44 + i))).ToArray();
        using MemoryStream source = new(payload,false);
        using HashingReadStream stream = new(source);
        using MemoryStream copy = new();

        while(true)
        {
            Int32 value = stream.ReadByte();

            if(value < 0) { break; }

            copy.WriteByte((Byte)value);
        }

        Byte[] hash = stream.GetHashAndReset();

        Assert.That(copy.ToArray(), Is.EqualTo(payload));
        Assert.That(hash, Is.EqualTo(SHA512.HashData(payload)));
    }

    [Test]
    public void GetHashAndReset_Throws_When_Called_Twice()
    {
        Byte[] payload = Enumerable.Range(0,8).Select(i => unchecked((Byte)(0x70 + i))).ToArray();
        using MemoryStream source = new(payload,false);
        using HashingReadStream stream = new(source);
        Byte[] buffer = new Byte[4];

        _ = stream.Read(buffer,0,buffer.Length);
        _ = stream.GetHashAndReset();

        Assert.That(() => stream.GetHashAndReset(), Throws.TypeOf<InvalidOperationException>());
    }
}
