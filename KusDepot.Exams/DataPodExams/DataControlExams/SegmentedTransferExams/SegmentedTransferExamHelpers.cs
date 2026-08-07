namespace KusDepot.DataServices;

public static class SegmentedTransferExamHelpers
{
    public sealed class TransferSegmentExamPayload : IDisposable
    {
        public PutTransferSegmentRequest Request { get; init; } = new();
        public MemoryStream Payload { get; init; } = new();

        public void Dispose()
        {
            this.Payload.Dispose();
        }
    }

    public static DataItemTransferManifest CreateManifest(Int64 streamLength = 128)
    {
        Byte[] objectPayload = CreatePayload(32,0x01);
        Byte[] streamPayload = CreatePayload((Int32)Math.Min(streamLength,64),0x02);

        return new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = Guid.NewGuid(),
            SourceSessionID = null,
            ObjectSHA512 = SHA512.HashData(objectPayload),
            StreamSHA512 = SHA512.HashData(streamPayload),
            StreamLength = streamLength,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.Upload,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
            ObjectInfo = new DataItemTransferObjectInfo(){ ObjectType = typeof(BinaryItem).FullName , ContentStreamed = true },
        };
    }

    public static Byte[] CreatePayload(Int32 length,Byte seed)
    {
        Byte[] payload = new Byte[length];

        for(Int32 i = 0; i < payload.Length; i++) { payload[i] = unchecked((Byte)(seed + i)); }

        return payload;
    }

    public static MemoryStream CreatePayloadStream(Byte[] payload)
    {
        return new(payload,false);
    }

    public static ISegmentedTransferStorage CreateStorage(String rootPath)
    {
        return new SegmentedTransferStorage(Options.Create(new TransferStorageOptions() { RootPath = rootPath }));
    }

    public static OpenUploadTransferRequest CreateOpenUploadRequest(Int64 streamLength = 128, Byte objectSeed = 0x11, Byte streamSeed = 0x22, Guid? itemId = null)
    {
        Byte[] objectPayload = CreatePayload(32,objectSeed);
        Byte[] streamPayload = CreatePayload(checked((Int32)streamLength),streamSeed);

        return new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = itemId ?? Guid.NewGuid(),
            ObjectPayload = objectPayload,
            ObjectSHA512 = SHA512.HashData(objectPayload),
            StreamSHA512 = SHA512.HashData(streamPayload),
            StreamLength = streamLength,
            ObjectInfo = new DataItemTransferObjectInfo(){ ObjectType = typeof(BinaryItem).FullName , ContentStreamed = true },
        };
    }

    public static PutTransferSegmentRequest CreatePutTransferSegmentRequest(OpenUploadTransferRequest request, Int64 offset, Byte[] payload, Int64 expectedStateVersion = 0)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            Offset = offset,
            Length = payload.LongLength,
            ExpectedStateVersion = expectedStateVersion,
            SegmentSHA512 = SHA512.HashData(payload),
            StreamSHA512 = request.StreamSHA512,
            RequestID = Guid.NewGuid(),
        };
    }

    public static TransferSegmentExamPayload CreatePutTransferSegment(OpenUploadTransferRequest request, Int64 offset, Byte[] payload, Int64 expectedStateVersion = 0)
    {
        return new()
        {
            Request = CreatePutTransferSegmentRequest(request,offset,payload,expectedStateVersion),
            Payload = CreatePayloadStream(payload),
        };
    }
}
