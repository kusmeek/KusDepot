namespace KusDepot.DataServices;

[TestFixture]
public class SegmentedTransferObjectPrimitivesExam
{
    [Test]
    public async Task StreamEnvelopeRoundTripsMetadataAndPayload()
    {
        GetTransferSegmentRequest metadata = new(){ SessionID = Guid.NewGuid() , ItemID = Guid.NewGuid() , Offset = 12 , Length = 18 };
        Byte[] metadataBytes = JsonUtility.Serialize(metadata);
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(18,0x44);
        using MemoryStream payloadStream = new(payload,false);
        using MemoryStream envelope = new();

        await TransferEnvelope.WriteAsync(envelope,metadataBytes,payloadStream,payload.Length,0,CancellationToken.None);

        envelope.Position = 0;
        TransferEnvelopeHeader? header = await TransferEnvelope.ReadHeaderAsync(envelope,CancellationToken.None);

        Check.That(header.HasValue).IsTrue();
        Check.That(header!.Value.PayloadLength).IsEqualTo(payload.Length);
        Check.That(header.Value.TrailerLength).IsEqualTo(0);

        ReadOnlyMemory<Byte> loadedMetadataBytes = await TransferEnvelope.ReadMetadataBytesAsync(envelope,header.Value,CancellationToken.None);
        GetTransferSegmentRequest? parsedMetadata = JsonUtility.Deserialize<GetTransferSegmentRequest>(loadedMetadataBytes.ToArray());
        using Stream payloadWindow = TransferEnvelope.OpenPayloadStream(envelope,header.Value,leaveopen:true);
        using MemoryStream loadedPayload = new();

        await payloadWindow.CopyToAsync(loadedPayload,CancellationToken.None);

        Check.That(parsedMetadata).IsNotNull();
        Check.That(parsedMetadata!.SessionID).IsEqualTo(metadata.SessionID);
        Check.That(parsedMetadata.ItemID).IsEqualTo(metadata.ItemID);
        Check.That(parsedMetadata.Offset).IsEqualTo(metadata.Offset);
        Check.That(parsedMetadata.Length).IsEqualTo(metadata.Length);
        Check.That(loadedPayload.ToArray().SequenceEqual(payload)).IsTrue();
    }

    [Test]
    public async Task StreamEnvelopeOpensPayloadForNonSeekableInput()
    {
        GetTransferSegmentRequest metadata = new(){ SessionID = Guid.NewGuid() , ItemID = Guid.NewGuid() , Offset = 4 , Length = 12 };
        Byte[] metadataBytes = JsonUtility.Serialize(metadata);
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(12,0x61);
        using MemoryStream payloadStream = new(payload,false);
        using MemoryStream envelope = new();

        await TransferEnvelope.WriteAsync(envelope,metadataBytes,payloadStream,payload.Length,0,CancellationToken.None);

        envelope.Position = 0;
        using Stream nonSeekable = new NonSeekableReadStream(envelope);

        TransferEnvelopeHeader? header = await TransferEnvelope.ReadHeaderAsync(nonSeekable,CancellationToken.None);

        Check.That(header.HasValue).IsTrue();

        ReadOnlyMemory<Byte> loadedMetadataBytes = await TransferEnvelope.ReadMetadataBytesAsync(nonSeekable,header!.Value,CancellationToken.None);
        GetTransferSegmentRequest? parsedMetadata = JsonUtility.Deserialize<GetTransferSegmentRequest>(loadedMetadataBytes.ToArray());
        using Stream payloadWindow = TransferEnvelope.OpenPayloadStream(nonSeekable,header.Value,leaveopen:true);
        using MemoryStream loadedPayload = new();

        await payloadWindow.CopyToAsync(loadedPayload,CancellationToken.None);

        Check.That(parsedMetadata).IsNotNull();
        Check.That(loadedPayload.ToArray().SequenceEqual(payload)).IsTrue();
    }

    [Test]
    public void TransferSegmentFooterSerializesAndDeserializes()
    {
        TransferSegmentFooter footer = new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = Guid.NewGuid(),
            RequestedOffset = 32,
            RequestedLength = 64,
            ReturnedOffset = 48,
            ReturnedLength = 16,
            SegmentSHA512 = SegmentedTransferExamHelpers.CreatePayload(64,0x55),
            StateVersion = 7,
        };

        Byte[] serialized = JsonUtility.Serialize(footer);
        TransferSegmentFooter? deserialized = JsonUtility.Deserialize<TransferSegmentFooter>(serialized);

        Check.That(deserialized).IsNotNull();
        Check.That(deserialized!.SessionID).IsEqualTo(footer.SessionID);
        Check.That(deserialized.ItemID).IsEqualTo(footer.ItemID);
        Check.That(deserialized.RequestedOffset).IsEqualTo(footer.RequestedOffset);
        Check.That(deserialized.RequestedLength).IsEqualTo(footer.RequestedLength);
        Check.That(deserialized.ReturnedOffset).IsEqualTo(footer.ReturnedOffset);
        Check.That(deserialized.ReturnedLength).IsEqualTo(footer.ReturnedLength);
        Check.That(deserialized.StateVersion).IsEqualTo(footer.StateVersion);
        Check.That(deserialized.SegmentSHA512.SequenceEqual(footer.SegmentSHA512)).IsTrue();
    }

    [Test]
    public async Task StreamEnvelopeRoundTripsPayloadAndTrailer()
    {
        Byte[] trailerBytes = JsonUtility.Serialize(new TransferSegmentFooter
        {
            SessionID = Guid.NewGuid(),
            ItemID = Guid.NewGuid(),
            RequestedOffset = 10,
            RequestedLength = 20,
            ReturnedOffset = 10,
            ReturnedLength = 20,
            SegmentSHA512 = SegmentedTransferExamHelpers.CreatePayload(64,0x33),
            StateVersion = 3,
        });
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(20,0x72);
        using MemoryStream payloadStream = new(payload,false);
        using MemoryStream envelope = new();

        await TransferEnvelope.WriteWithTrailer(envelope,trailerBytes,payloadStream,payload.Length,CancellationToken.None);

        envelope.Position = 0;
        TransferEnvelopeHeader? header = await TransferEnvelope.ReadHeaderAsync(envelope,CancellationToken.None);

        Check.That(header.HasValue).IsTrue();
        Check.That(header!.Value.MetadataLength).IsEqualTo(0);
        Check.That(header.Value.TrailerLength).IsEqualTo(trailerBytes.Length);

        using Stream payloadWindow = TransferEnvelope.OpenPayloadStream(envelope,header.Value,leaveopen:true);
        using MemoryStream loadedPayload = new();

        await payloadWindow.CopyToAsync(loadedPayload,CancellationToken.None);

        ReadOnlyMemory<Byte> loadedTrailerBytes = await TransferEnvelope.ReadTrailerBytesAsync(envelope,header.Value,CancellationToken.None);

        Check.That(loadedPayload.ToArray().SequenceEqual(payload)).IsTrue();
        Check.That(loadedTrailerBytes.ToArray().SequenceEqual(trailerBytes)).IsTrue();
    }

    private sealed class NonSeekableReadStream(Stream inner) : Stream
    {
        private readonly Stream Inner = inner;

        public override Boolean CanRead => this.Inner.CanRead;
        public override Boolean CanSeek => false;
        public override Boolean CanWrite => false;
        public override Int64 Length => throw new NotSupportedException();
        public override Int64 Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => this.Inner.Flush();
        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => this.Inner.Read(buffer,offset,count);
        public override Int32 Read(Span<Byte> buffer) => this.Inner.Read(buffer);
        public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancellationToken) => this.Inner.ReadAsync(buffer,offset,count,cancellationToken);
        public override ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancellationToken = default) => this.Inner.ReadAsync(buffer,cancellationToken);
        public override Int64 Seek(Int64 offset , SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(Int64 value) => throw new NotSupportedException();
        public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();
    }
}
