using KusDepot.Data;

namespace KusDepot.DataServices;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class BoundedReadStreamExam
{
    [Test]
    public void Read_Limits_Output_To_Configured_Window()
    {
        Byte[] payload = Enumerable.Range(0,32).Select(i => unchecked((Byte)(0x10 + i))).ToArray();
        using MemoryStream source = new(payload,false);
        using BoundedReadStream stream = new(source,6,9,leaveopen:true);
        Byte[] buffer = new Byte[16];

        Int32 firstRead = stream.Read(buffer,0,4);
        Int32 secondRead = stream.Read(buffer,4,12);
        Int32 thirdRead = stream.Read(buffer,0,buffer.Length);

        Assert.That(firstRead, Is.EqualTo(4));
        Assert.That(secondRead, Is.EqualTo(5));
        Assert.That(thirdRead, Is.EqualTo(0));
        Assert.That(buffer[..9], Is.EqualTo(payload.Skip(6).Take(9).ToArray()));
        Assert.That(stream.Position, Is.EqualTo(9));
        Assert.That(source.Position, Is.EqualTo(15));
    }

    [Test]
    public async Task ReadAsync_And_Seek_Respect_Window_Bounds()
    {
        Byte[] payload = Enumerable.Range(0,40).Select(i => unchecked((Byte)(0x30 + i))).ToArray();
        using MemoryStream source = new(payload,false);
        using BoundedReadStream stream = new(source,10,12,leaveopen:true);
        Byte[] firstBuffer = new Byte[8];
        Byte[] secondBuffer = new Byte[8];

        Int32 firstRead = await stream.ReadAsync(firstBuffer.AsMemory(0,firstBuffer.Length),CancellationToken.None);
        Int64 position = stream.Seek(-3,SeekOrigin.End);
        Int32 secondRead = await stream.ReadAsync(secondBuffer.AsMemory(0,secondBuffer.Length),CancellationToken.None);

        Assert.That(firstRead, Is.EqualTo(8));
        Assert.That(firstBuffer, Is.EqualTo(payload.Skip(10).Take(8).ToArray()));
        Assert.That(position, Is.EqualTo(9));
        Assert.That(secondRead, Is.EqualTo(3));
        Assert.That(secondBuffer[..3], Is.EqualTo(payload.Skip(19).Take(3).ToArray()));
    }

    [Test]
    public void Dispose_With_LeaveOpen_True_Does_Not_Dispose_Source()
    {
        Byte[] payload = Enumerable.Range(0,8).Select(i => unchecked((Byte)(0x50 + i))).ToArray();
        using MemoryStream source = new(payload,false);

        using(BoundedReadStream stream = new(source,1,4,leaveopen:true))
        {
            Assert.That(stream.ReadByte(), Is.EqualTo(payload[1]));
        }

        Assert.That(() => source.ReadByte(), Throws.Nothing);
    }
}
