namespace KusDepot.Exams.Cryptography;

internal static class DataItemSecurityEnvelopeV1TestHelpers
{
    internal const Int32 FixedHeaderSize = 146;
    internal const Int32 RecipientTableLengthOffset = 6;
    internal const Int32 ChunkSizePowerOffset = 3;
    internal const Int32 OriginalLengthOffset = 10;
    internal const Int32 RootContextHashOffset = 18;
    internal const Int32 HashSize = 64;
    internal const Int32 EnvelopeMetadataHashOffset = RootContextHashOffset + HashSize;
    internal const Int32 DefaultChunkSizePower = 20;
    private const Int32 ChunkLengthBlockSize = 8;
    private const Int32 TagSizeLengthFieldSize = 1;
    private const Int32 TagSize = 16;

    private static readonly Type EnvelopeType = typeof(DataItem).Assembly.GetType("KusDepot.Security.Data.DataItemSecurityEnvelope",throwOnError:true)!;
    private static readonly MethodInfo EncryptArrayAsyncMethod = EnvelopeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(_=>_.Name == "EncryptArrayAsync");
    private static readonly MethodInfo DecryptArrayAsyncMethod = EnvelopeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(_=>_.Name == "DecryptArrayAsync");
    private static readonly MethodInfo EncryptStreamAsyncMethod = EnvelopeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(_=>_.Name == "EncryptStreamAsync");
    private static readonly MethodInfo DecryptStreamAsyncMethod = EnvelopeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(_=>_.Name == "DecryptStreamAsync");
    private static readonly MethodInfo GetEncryptedCapacityMethod = EnvelopeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(_=>_.Name == "GetEncryptedCapacity");

    internal sealed class EnvelopeParty : IDisposable
    {
        internal EnvelopeParty(Guid objectId , X509Certificate2 certificate , String subject)
        {
            ObjectId = objectId;
            Certificate = certificate;
            Object = new DataSecurityObject(objectId,certificate,subject);
            Recipient = new DataSecurityRecipient(certificate,objectId);
        }

        internal Guid ObjectId { get; }
        internal X509Certificate2 Certificate { get; }
        internal DataSecurityObject Object { get; }
        internal DataSecurityRecipient Recipient { get; }

        public void Dispose() => Certificate.Dispose();
    }

    internal static EnvelopeParty CreateParty(String subject)
    {
        Guid objectId = Guid.NewGuid();
        X509Certificate2 certificate = CreateCertificate(objectId,subject,2048,2,HashAlgorithmName.SHA512) ?? throw new InvalidOperationException("Unable to create test certificate.");
        return new EnvelopeParty(objectId,certificate,subject);
    }

    internal static Byte[] BuildRootContext(Guid dataItemId , DataFieldState fieldState = DataFieldState.EncryptedContent)
    {
        Byte[] context = new Byte[16 + sizeof(Int32)];
        dataItemId.TryWriteBytes(context.AsSpan(0,16));
        BinaryPrimitives.WriteInt32BigEndian(context.AsSpan(16,sizeof(Int32)),(Int32)fieldState);
        return context;
    }

    internal static async Task<Byte[]?> EncryptArrayAsync(Byte[] input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , Byte[] rootContext , Int32 chunkSizePower = DefaultChunkSizePower)
    {
        Task<Byte[]?> task = (Task<Byte[]?>)EncryptArrayAsyncMethod.Invoke(null,[input,recipients,issuer,new ReadOnlyMemory<Byte>(rootContext),chunkSizePower,CancellationToken.None])!;
        return await task.ConfigureAwait(false);
    }

    internal static async Task<Byte[]?> DecryptArrayAsync(Byte[] input , X509Certificate2 certificate , Byte[] rootContext)
    {
        Task<Byte[]?> task = (Task<Byte[]?>)DecryptArrayAsyncMethod.Invoke(null,[input,certificate,new ReadOnlyMemory<Byte>(rootContext),CancellationToken.None])!;
        return await task.ConfigureAwait(false);
    }

    internal static async Task<Boolean> EncryptStreamAsync(Stream input , Stream output , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , Byte[] rootContext , Int32 chunkSizePower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        Task<Boolean> task = (Task<Boolean>)EncryptStreamAsyncMethod.Invoke(null,[input,output,recipients,issuer,new ReadOnlyMemory<Byte>(rootContext),chunkSizePower,cancel])!;
        return await task.ConfigureAwait(false);
    }

    internal static async Task<Boolean> DecryptStreamAsync(Stream input , Stream output , X509Certificate2 certificate , Byte[] rootContext , CancellationToken cancel = default)
    {
        Task<Boolean> task = (Task<Boolean>)DecryptStreamAsyncMethod.Invoke(null,[input,output,certificate,new ReadOnlyMemory<Byte>(rootContext),cancel])!;
        return await task.ConfigureAwait(false);
    }

    internal static Stream CreateNonSeekableReadStream(Byte[] input) => new NonSeekableReadStream(input);

    internal static Int32? GetEncryptedCapacity(ImmutableArray<DataSecurityRecipient> recipients , Int64 plaintextLength , Int32 chunkSizePower = DefaultChunkSizePower)
        => (Int32?)GetEncryptedCapacityMethod.Invoke(null,[recipients,plaintextLength,chunkSizePower]);

    internal static Byte[] TamperRootContextHash(Byte[] envelope) => CloneAndFlip(envelope,RootContextHashOffset);

    internal static Byte[] TamperEnvelopeMetadataHash(Byte[] envelope) => CloneAndFlip(envelope,EnvelopeMetadataHashOffset);

    internal static Byte[] TamperRecipientTable(Byte[] envelope)
    {
        Int32 recipientTableLength = checked((Int32)BinaryPrimitives.ReadUInt32BigEndian(envelope.AsSpan(RecipientTableLengthOffset,4)));
        Assert.That(recipientTableLength, Is.GreaterThan(0));
        return CloneAndFlip(envelope,FixedHeaderSize);
    }

    internal static Byte[] TamperCiphertext(Byte[] envelope)
    {
        Int32 recipientTableLength = checked((Int32)BinaryPrimitives.ReadUInt32BigEndian(envelope.AsSpan(RecipientTableLengthOffset,4)));
        Int32 bodyStart = FixedHeaderSize + recipientTableLength;
        Assert.That(envelope.Length, Is.GreaterThan(bodyStart + 8));
        return CloneAndFlip(envelope,bodyStart + 8);
    }

    internal static Byte[] TamperTerminalRecord(Byte[] envelope , Int32 relativeOffset)
    {
        Int32 terminalOffset = GetTerminalRecordOffset(envelope);
        return CloneAndFlip(envelope,checked(terminalOffset + relativeOffset));
    }

    internal static Byte[] RemoveTerminalRecord(Byte[] envelope)
    {
        Byte[] truncated = (Byte[])envelope.Clone();
        Array.Resize(ref truncated,truncated.Length - 33);
        return truncated;
    }

    internal static Byte[] ForgeEmptyEnvelopeWithoutTerminal(Byte[] envelope)
    {
        Int32 recipientTableLength = checked((Int32)BinaryPrimitives.ReadUInt32BigEndian(envelope.AsSpan(RecipientTableLengthOffset,4)));
        Int32 bodyOffset = checked(FixedHeaderSize + recipientTableLength);
        Byte[] forged = envelope[..bodyOffset];
        BinaryPrimitives.WriteUInt64BigEndian(forged.AsSpan(OriginalLengthOffset,sizeof(UInt64)),0);

        Byte[] recipientTableHash = SHA512.HashData(forged.AsSpan(FixedHeaderSize,recipientTableLength));
        Byte[] metadata = new Byte[18 + HashSize + HashSize];
        forged.AsSpan(0,18).CopyTo(metadata);
        forged.AsSpan(RootContextHashOffset,HashSize).CopyTo(metadata.AsSpan(18,HashSize));
        recipientTableHash.CopyTo(metadata,18 + HashSize);
        SHA512.HashData(metadata).CopyTo(forged,EnvelopeMetadataHashOffset);

        return forged;
    }

    internal static Byte[] SetVersion(Byte[] envelope , Byte version)
    {
        Byte[] tampered = (Byte[])envelope.Clone();
        tampered[0] = version;
        return tampered;
    }

    private static Int32 GetTerminalRecordOffset(Byte[] envelope)
    {
        Int32 recipientTableLength = checked((Int32)BinaryPrimitives.ReadUInt32BigEndian(envelope.AsSpan(RecipientTableLengthOffset,4)));
        UInt64 originalLength = BinaryPrimitives.ReadUInt64BigEndian(envelope.AsSpan(OriginalLengthOffset,sizeof(UInt64)));
        Int32 chunkSizePower = envelope[ChunkSizePowerOffset];
        UInt64 chunkSize = 1UL << chunkSizePower;
        UInt64 chunkCount = originalLength == 0 ? 0 : ((originalLength - 1) / chunkSize) + 1;
        UInt64 chunkRecordOverhead = ChunkLengthBlockSize + TagSizeLengthFieldSize + TagSize;
        return checked((Int32)((UInt64)FixedHeaderSize + (UInt64)recipientTableLength + originalLength + chunkCount * chunkRecordOverhead));
    }

    private static Byte[] CloneAndFlip(Byte[] envelope , Int32 offset)
    {
        Byte[] tampered = (Byte[])envelope.Clone();
        tampered[offset] ^= 0x5A;
        return tampered;
    }

    private sealed class NonSeekableReadStream : Stream
    {
        private readonly MemoryStream Source;

        internal NonSeekableReadStream(Byte[] input) { this.Source = new MemoryStream(input,false); }

        public override Boolean CanRead => this.Source.CanRead;
        public override Boolean CanSeek => false;
        public override Boolean CanWrite => false;
        public override Int64 Length => this.Source.Length;
        public override Int64 Position
        {
            get => this.Source.Position;
            set => throw new NotSupportedException();
        }

        public override void Flush() { }
        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => this.Source.Read(buffer,offset,count);
        public override ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default) => this.Source.ReadAsync(buffer,cancel);
        public override Int64 Seek(Int64 offset , SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(Int64 value) => throw new NotSupportedException();
        public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();
        public override void Write(ReadOnlySpan<Byte> buffer) => throw new NotSupportedException();
        public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default) => throw new NotSupportedException();

        protected override void Dispose(Boolean disposing)
        {
            if(disposing) { this.Source.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
