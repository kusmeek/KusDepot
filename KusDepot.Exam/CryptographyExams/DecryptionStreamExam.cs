namespace KusDepot.Exams.Cryptography;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DecryptionStreamExam
{
    private static Byte[] RandomBytes(Int32 size)
    {
        Byte[] bytes = new Byte[size];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }

    /// <summary>
    /// Verifies that CanRead delegates to the inner stream.
    /// </summary>
    [Test]
    public void CanRead_DelegatesFromInner()
    {
        using var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        using var ds = new DecryptionStream(inner,0xAA);
        Assert.That(ds.CanRead, Is.True);
    }

    /// <summary>
    /// Verifies that CanSeek delegates to the inner stream when seekable.
    /// </summary>
    [Test]
    public void CanSeek_TrueWhenInnerSeekable()
    {
        using var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        using var ds = new DecryptionStream(inner,0xAA);
        Assert.That(ds.CanSeek, Is.True);
    }

    /// <summary>
    /// Verifies that CanWrite always returns false.
    /// </summary>
    [Test]
    public void CanWrite_AlwaysFalse()
    {
        using var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        using var ds = new DecryptionStream(inner,0xAA);
        Assert.That(ds.CanWrite, Is.False);
    }

    /// <summary>
    /// Verifies that Length equals one prefix byte plus the remaining inner stream length from origin.
    /// </summary>
    [Test]
    public void Length_EqualsPrefixPlusInnerRemainder()
    {
        Byte[] data = new Byte[]{ 10,20,30,40,50 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Assert.That(ds.Length, Is.EqualTo(1 + data.Length));
    }

    /// <summary>
    /// Verifies that Length accounts for a non-zero inner origin when the inner stream is partially consumed before wrapping.
    /// </summary>
    [Test]
    public void Length_AccountsForNonZeroInnerOrigin()
    {
        Byte[] data = new Byte[]{ 10,20,30,40,50 };
        using var inner = new MemoryStream(data);
        inner.Position = 2;
        using var ds = new DecryptionStream(inner,0x01);
        Assert.That(ds.Length, Is.EqualTo(1 + (data.Length - 2)));
    }

    /// <summary>
    /// Verifies that Position starts at zero after construction.
    /// </summary>
    [Test]
    public void Position_StartsAtZero()
    {
        using var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        using var ds = new DecryptionStream(inner,0xAA);
        Assert.That(ds.Position, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies that Position advances correctly after reading through the prefix and into the inner stream.
    /// </summary>
    [Test]
    public void Position_AdvancesAfterRead()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] buf = new Byte[2];
        ds.Read(buf,0,2);
        Assert.That(ds.Position, Is.EqualTo(2));
    }

    /// <summary>
    /// Verifies that setting Position within the prefix range resets the inner stream to origin.
    /// </summary>
    [Test]
    public void PositionSet_WithinPrefix_ResetsInnerToOrigin()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] buf = new Byte[4];
        ds.Read(buf,0,4);
        Assert.That(ds.Position, Is.EqualTo(4));
        ds.Position = 1;
        Assert.That(ds.Position, Is.EqualTo(1));
        Assert.That(inner.Position, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies that setting Position beyond the prefix advances the inner stream accordingly.
    /// </summary>
    [Test]
    public void PositionSet_BeyondPrefix_AdvancesInner()
    {
        Byte[] data = new Byte[]{ 10,20,30,40,50 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        ds.Position = 3;
        Assert.That(ds.Position, Is.EqualTo(3));
        Assert.That(inner.Position, Is.EqualTo(2));
    }

    /// <summary>
    /// Verifies that setting Position to a negative value throws ArgumentOutOfRangeException.
    /// </summary>
    [Test]
    public void PositionSet_Negative_Throws()
    {
        using var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        using var ds = new DecryptionStream(inner,0x01);
        Assert.Throws<ArgumentOutOfRangeException>(() => ds.Position = -1);
    }

    /// <summary>
    /// Verifies that setting Position beyond Length throws ArgumentOutOfRangeException.
    /// </summary>
    [Test]
    public void PositionSet_BeyondLength_Throws()
    {
        using var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        using var ds = new DecryptionStream(inner,0x01);
        Assert.Throws<ArgumentOutOfRangeException>(() => ds.Position = ds.Length + 1);
    }

    /// <summary>
    /// Verifies that a synchronous read replays the prefix byte first, then continues from the inner stream.
    /// </summary>
    [Test]
    public void Read_ReplaysPrefixThenInner()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] buf = new Byte[4];
        Int32 read = ds.Read(buf,0,4);
        Assert.That(read, Is.EqualTo(4));
        Assert.That(buf, Is.EqualTo(new Byte[]{ 0x01,10,20,30 }));
    }

    /// <summary>
    /// Verifies that reading only the prefix byte returns only the prefix without consuming the inner stream.
    /// </summary>
    [Test]
    public void Read_OnlyPrefix_DoesNotConsumeInner()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] buf = new Byte[1];
        Int32 read = ds.Read(buf,0,1);
        Assert.That(read, Is.EqualTo(1));
        Assert.That(buf, Is.EqualTo(new Byte[]{ 0x01 }));
        Assert.That(inner.Position, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies that a read crossing the prefix-inner boundary in a single call returns the correct combined result.
    /// </summary>
    [Test]
    public void Read_CrossesBoundary_ReturnsCombined()
    {
        Byte[] data = new Byte[]{ 10,20 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0xAA);
        Byte[] buf = new Byte[3];
        Int32 read = ds.Read(buf,0,3);
        Assert.That(read, Is.EqualTo(3));
        Assert.That(buf, Is.EqualTo(new Byte[]{ 0xAA,10,20 }));
    }

    /// <summary>
    /// Verifies that reading after the prefix is fully consumed returns only inner stream bytes.
    /// </summary>
    [Test]
    public void Read_AfterPrefixConsumed_ReadsOnlyInner()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] skip = new Byte[1]; ds.Read(skip,0,1);
        Byte[] buf = new Byte[3];
        Int32 read = ds.Read(buf,0,3);
        Assert.That(read, Is.EqualTo(3));
        Assert.That(buf, Is.EqualTo(data));
    }

    /// <summary>
    /// Verifies that reading past the end of the logical stream returns the available bytes without error.
    /// </summary>
    [Test]
    public void Read_PastEnd_ReturnsAvailable()
    {
        Byte[] data = new Byte[]{ 10 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] buf = new Byte[10];
        Int32 read = ds.Read(buf,0,10);
        Assert.That(read, Is.EqualTo(2));
        Assert.That(buf[0], Is.EqualTo(0x01));
        Assert.That(buf[1], Is.EqualTo(10));
    }

    /// <summary>
    /// Verifies that an asynchronous read replays the prefix byte first, then continues from the inner stream.
    /// </summary>
    [Test]
    public async Task ReadAsync_ReplaysPrefixThenInner()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Byte[] buf = new Byte[4];
        Int32 read = await ds.ReadAsync(buf.AsMemory(0,4));
        Assert.That(read, Is.EqualTo(4));
        Assert.That(buf, Is.EqualTo(new Byte[]{ 0x01,10,20,30 }));
    }

    /// <summary>
    /// Verifies that an asynchronous read crossing the prefix-inner boundary returns the correct combined result.
    /// </summary>
    [Test]
    public async Task ReadAsync_CrossesBoundary_ReturnsCombined()
    {
        Byte[] data = new Byte[]{ 10,20 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0xAA);
        Byte[] buf = new Byte[3];
        Int32 read = await ds.ReadAsync(buf.AsMemory(0,3));
        Assert.That(read, Is.EqualTo(3));
        Assert.That(buf, Is.EqualTo(new Byte[]{ 0xAA,10,20 }));
    }

    /// <summary>
    /// Verifies Seek with SeekOrigin.Begin positions at the absolute logical offset.
    /// </summary>
    [Test]
    public void Seek_Begin_PositionsCorrectly()
    {
        Byte[] data = new Byte[]{ 10,20,30,40 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        ds.Read(new Byte[5],0,5);
        Int64 pos = ds.Seek(1,SeekOrigin.Begin);
        Assert.That(pos, Is.EqualTo(1));
        Byte[] buf = new Byte[1]; ds.Read(buf,0,1);
        Assert.That(buf[0], Is.EqualTo(10));
    }

    /// <summary>
    /// Verifies Seek with SeekOrigin.Current moves relative to the current logical position.
    /// </summary>
    [Test]
    public void Seek_Current_MovesRelative()
    {
        Byte[] data = new Byte[]{ 10,20,30,40 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        ds.Read(new Byte[2],0,2);
        Int64 pos = ds.Seek(1,SeekOrigin.Current);
        Assert.That(pos, Is.EqualTo(3));
        Byte[] buf = new Byte[1]; ds.Read(buf,0,1);
        Assert.That(buf[0], Is.EqualTo(30));
    }

    /// <summary>
    /// Verifies Seek with SeekOrigin.End positions relative to the logical stream length.
    /// </summary>
    [Test]
    public void Seek_End_PositionsFromLength()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        Int64 pos = ds.Seek(-1,SeekOrigin.End);
        Assert.That(pos, Is.EqualTo(3));
        Byte[] buf = new Byte[1]; ds.Read(buf,0,1);
        Assert.That(buf[0], Is.EqualTo(30));
    }

    /// <summary>
    /// Verifies that Seek back to the start after reading inner stream bytes replays the prefix byte again.
    /// </summary>
    [Test]
    public void Seek_BackToPrefix_ReplaysCorrectly()
    {
        Byte[] data = new Byte[]{ 10,20,30 };
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0xAA);
        ds.Read(new Byte[4],0,4);
        ds.Seek(0,SeekOrigin.Begin);
        Byte[] buf = new Byte[4];
        Int32 read = ds.Read(buf,0,4);
        Assert.That(read, Is.EqualTo(4));
        Assert.That(buf, Is.EqualTo(new Byte[]{ 0xAA,10,20,30 }));
    }

    /// <summary>
    /// Verifies that Write throws NotSupportedException.
    /// </summary>
    [Test]
    public void Write_ThrowsNotSupported()
    {
        using var inner = new MemoryStream(new Byte[]{ 1 });
        using var ds = new DecryptionStream(inner,0x01);
        Assert.Throws<NotSupportedException>(() => ds.Write(new Byte[]{ 0xFF },0,1));
    }

    /// <summary>
    /// Verifies that SetLength throws NotSupportedException.
    /// </summary>
    [Test]
    public void SetLength_ThrowsNotSupported()
    {
        using var inner = new MemoryStream(new Byte[]{ 1 });
        using var ds = new DecryptionStream(inner,0x01);
        Assert.Throws<NotSupportedException>(() => ds.SetLength(100));
    }

    /// <summary>
    /// Verifies that a null inner stream argument throws ArgumentNullException.
    /// </summary>
    [Test]
    public void Constructor_NullInner_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DecryptionStream(null!,0x01));
    }

    /// <summary>
    /// Verifies that disposing the wrapper does not dispose the inner stream.
    /// </summary>
    [Test]
    public void Dispose_DoesNotDisposeInner()
    {
        var inner = new MemoryStream(new Byte[]{ 1,2,3 });
        var ds = new DecryptionStream(inner,0x01);
        ds.Dispose();
        Assert.That(inner.CanRead, Is.True);
        inner.Dispose();
    }

    /// <summary>
    /// Verifies full read of a large payload with a single-byte prefix produces the correct combined output.
    /// </summary>
    [Test]
    public void Read_LargePayload_MatchesCombined()
    {
        Byte[] data = RandomBytes(128_000);
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        using var result = new MemoryStream();
        ds.CopyTo(result);
        Byte[] combined = result.ToArray();
        Assert.That(combined.Length, Is.EqualTo(1 + data.Length));
        Assert.That(combined[0], Is.EqualTo(0x01));
        Assert.That(combined.AsSpan(1).SequenceEqual(data), Is.True);
    }

    /// <summary>
    /// Verifies full async read of a large payload with a single-byte prefix produces the correct combined output.
    /// </summary>
    [Test]
    public async Task ReadAsync_LargePayload_MatchesCombined()
    {
        Byte[] data = RandomBytes(128_000);
        using var inner = new MemoryStream(data);
        using var ds = new DecryptionStream(inner,0x01);
        using var result = new MemoryStream();
        await ds.CopyToAsync(result);
        Byte[] combined = result.ToArray();
        Assert.That(combined.Length, Is.EqualTo(1 + data.Length));
        Assert.That(combined[0], Is.EqualTo(0x01));
        Assert.That(combined.AsSpan(1).SequenceEqual(data), Is.True);
    }
}
