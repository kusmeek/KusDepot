namespace KusDepot.DataServices;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class CompositeReadStreamExam
{
    [Test]
    public async Task ReadAsync_Concatenates_Lazy_Source_Parts_Without_Buffering_Whole_Result()
    {
        Int32 opens = 0;
        CompositeReadStreamPart[] parts =
        [
            new() { Length = 3 , OpenAsync = _ => { opens++; return Task.FromResult<Stream?>(new MemoryStream([1,2,3],false)); } },
            new() { Length = 2 , OpenAsync = _ => { opens++; return Task.FromResult<Stream?>(new MemoryStream([4,5],false)); } },
        ];

        await using CompositeReadStream stream = new(parts);
        using MemoryStream destination = new();
        await stream.CopyToAsync(destination);

        Assert.That(destination.ToArray(), Is.EqualTo(new Byte[] { 1,2,3,4,5 }));
        Assert.That(stream.Length, Is.EqualTo(5));
        Assert.That(stream.Position, Is.EqualTo(5));
        Assert.That(opens, Is.EqualTo(2));
    }

    [Test]
    public async Task ReadAsync_Skips_Source_Offset_And_Exposes_Only_Part_Length()
    {
        CompositeReadStreamPart[] parts =
        [
            new() { SourceOffset = 2 , Length = 3 , OpenAsync = _ => Task.FromResult<Stream?>(new MemoryStream([0,1,2,3,4,5],false)) },
        ];

        await using CompositeReadStream stream = new(parts);
        Byte[] buffer = new Byte[8];
        Int32 read = await stream.ReadAsync(buffer);

        Assert.That(read, Is.EqualTo(3));
        Assert.That(buffer[..3], Is.EqualTo(new Byte[] { 2,3,4 }));
        Assert.That(await stream.ReadAsync(buffer), Is.EqualTo(0));
    }

    [Test]
    public void ReadAsync_Throws_When_A_Source_Is_Short()
    {
        CompositeReadStreamPart[] parts =
        [
            new() { Length = 4 , OpenAsync = _ => Task.FromResult<Stream?>(new MemoryStream([1,2,3],false)) },
        ];

        using CompositeReadStream stream = new(parts);

        Assert.That(async () => await stream.CopyToAsync(Stream.Null), Throws.TypeOf<EndOfStreamException>());
    }

    [Test]
    public async Task DisposeAsync_Disposes_Current_Source()
    {
        TrackingStream source = new([1,2]);
        await using(CompositeReadStream stream = new(
            [new CompositeReadStreamPart { Length = 2 , OpenAsync = _ => Task.FromResult<Stream?>(source) }]))
        {
            Assert.That(await stream.ReadAsync(new Byte[1]), Is.EqualTo(1));
        }

        Assert.That(source.Disposed, Is.True);
    }

    private sealed class TrackingStream : MemoryStream
    {
        public TrackingStream(Byte[] data) : base(data,false) { }

        public Boolean Disposed { get; private set; }

        protected override void Dispose(Boolean disposing)
        {
            this.Disposed = true;
            base.Dispose(disposing);
        }
    }
}
