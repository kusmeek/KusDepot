namespace KusDepot.DataServices;

public static class StreamTransferExamHelpers
{
    public sealed class StreamTransferExamAppend : IDisposable
    {
        public PutStreamTransferSegmentRequest Request { get; init; } = new();
        public MemoryStream Payload { get; init; } = new();

        public void Dispose()
        {
            this.Payload.Dispose();
        }
    }

    public static DataStreamTransferManifest CreateManifest(Byte[]? objectPayload = null , Byte[]? streamHash = null , Int64 appendedLength = 0 , Guid? itemId = null , Guid? sessionId = null)
    {
        Byte[] payload = objectPayload ?? CreatePayload(32,0x11);

        return new()
        {
            SessionID = sessionId ?? Guid.NewGuid(),
            ItemID = itemId ?? Guid.NewGuid(),
            SourceSessionID = null,
            ObjectInfo = new DataItemTransferObjectInfo(){ ObjectType = typeof(DataStreamItem).FullName , ContentStreamed = true },
            ObjectPayload = payload,
            ObjectSHA512 = SHA512.HashData(payload),
            SegmentSizePolicy = new DataItemTransferSegmentSizePolicy(){ MinSegmentBytes = 1 , PreferredSegmentBytes = 16 , MaxSegmentBytes = 64 },
            AppendedLength = appendedLength,
            StateVersion = 0,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.StreamUpload,
            StreamSHA512 = streamHash ?? Array.Empty<Byte>(),
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    public static Byte[] CreatePayload(Int32 length , Byte seed)
    {
        Byte[] payload = new Byte[length];

        for(Int32 i = 0; i < payload.Length; i++) { payload[i] = unchecked((Byte)(seed + i)); }

        return payload;
    }

    public static MemoryStream CreatePayloadStream(Byte[] payload)
    {
        return new(payload,false);
    }

    public static IStreamTransferStorage CreateStorage(String rootPath)
    {
        return new StreamTransferStorage(Options.Create(new TransferStorageOptions(){ RootPath = rootPath }));
    }

    public static OpenStreamTransferRequest CreateOpenRequest(Byte[]? objectPayload = null , Guid? itemId = null)
    {
        Byte[] payload = objectPayload ?? CreatePayload(32,0x21);

        return new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = itemId ?? Guid.NewGuid(),
            ObjectPayload = payload,
            ObjectSHA512 = SHA512.HashData(payload),
            ObjectInfo = new DataItemTransferObjectInfo(){ ObjectType = typeof(DataStreamItem).FullName , ContentStreamed = true },
            RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy(){ MinSegmentBytes = 1 , PreferredSegmentBytes = 16 , MaxSegmentBytes = 64 },
        };
    }

    public static OpenFollowStreamTransferRequest CreateFollowRequest(OpenStreamTransferRequest source , Guid? followerSessionId = null)
    {
        return new()
        {
            SessionID = followerSessionId ?? Guid.NewGuid(),
            ItemID = source.ItemID,
            SourceSessionID = source.SessionID,
        };
    }

    public static PutStreamTransferSegmentRequest CreateAppendRequest(Guid sessionId , Guid itemId , Int64 offset , Byte[] payload , Int64? expectedStateVersion = 0 , Byte[]? streamHash = null)
    {
        return new()
        {
            SessionID = sessionId,
            ItemID = itemId,
            Offset = offset,
            Length = payload.LongLength,
            ExpectedStateVersion = expectedStateVersion,
            SegmentSHA512 = SHA512.HashData(payload),
            StreamSHA512 = streamHash ?? Array.Empty<Byte>(),
            RequestID = Guid.NewGuid(),
        };
    }

    public static StreamTransferExamAppend CreateAppend(Guid sessionId , Guid itemId , Int64 offset , Byte[] payload , Int64? expectedStateVersion = 0 , Byte[]? streamHash = null)
    {
        return new()
        {
            Request = CreateAppendRequest(sessionId,itemId,offset,payload,expectedStateVersion,streamHash),
            Payload = CreatePayloadStream(payload),
        };
    }

    public static GetStreamTransferSegmentRequest CreateReadRequest(Guid sessionId , Guid itemId , Int64 offset , Int64 length)
    {
        return new()
        {
            SessionID = sessionId,
            ItemID = itemId,
            Offset = offset,
            Length = length,
        };
    }

    public static ReOpenStreamTransferRequest CreateReOpenRequest(OpenStreamTransferRequest source , Byte[]? streamHash = null)
    {
        return new()
        {
            SessionID = source.SessionID,
            ItemID = source.ItemID,
            ObjectSHA512 = source.ObjectSHA512,
            StreamSHA512 = streamHash ?? Array.Empty<Byte>(),
        };
    }

    public static ReOpenFollowStreamTransferRequest CreateReOpenFollowRequest(OpenFollowStreamTransferRequest follower)
    {
        return new()
        {
            SessionID = follower.SessionID,
            ItemID = follower.ItemID,
            SourceSessionID = follower.SourceSessionID,
        };
    }

    public static CompleteStreamTransferRequest CreateCompleteRequest(OpenStreamTransferRequest source , Int64 expectedStateVersion , Int64 finalStreamLength , Byte[]? finalStreamHash = null)
    {
        return new()
        {
            SessionID = source.SessionID,
            ItemID = source.ItemID,
            ExpectedStateVersion = expectedStateVersion,
            FinalObjectInfo = source.ObjectInfo,
            FinalObjectPayload = source.ObjectPayload,
            FinalObjectSHA512 = source.ObjectSHA512,
            FinalStreamLength = finalStreamLength,
            FinalStreamSHA512 = finalStreamHash ?? Array.Empty<Byte>(),
        };
    }
}