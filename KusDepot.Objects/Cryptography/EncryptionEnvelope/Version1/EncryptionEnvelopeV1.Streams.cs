namespace KusDepot.Cryptography;

internal static partial class EncryptionEnvelopeV1
{
    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean DecryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null) { return false; }

        if(output is MemoryStream memoryOutput && TryGetRemainingMemory(input,out ReadOnlyMemory<Byte> decryptInput))
        {
            Byte[]? decrypted = DecryptArrayCore(decryptInput.Span,certificate,aad); if(decrypted is null) { return false; }

            memoryOutput.Write(decrypted,0,decrypted.Length);

            return true;
        }

        Span<Byte> aesKey = stackalloc Byte[AesKeySize]; Span<Byte> baseNonceSeed = stackalloc Byte[BaseNonceSeedSize];

        try
        {
            if(TryReadAndProcessHeader(input,certificate,aad,out var header,aesKey,baseNonceSeed) is false) { return false; }

            Int32 chunkSize = 1 << header.ChunkSizePower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> nonce = stackalloc Byte[NonceSize];
            Span<Byte> lengths = stackalloc Byte[ChunkLengthsBlockSize];
            Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];
            UInt64 totalPlain = 0; UInt64 chunkIndex = 0;

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                while(true)
                {
                    if(header.HasOriginalLength && totalPlain == header.OriginalLength) { break; }
                    Int32 first = input.ReadByte(); if(first == -1) { return false; }
                    if(first == TerminalMarker)
                    {
                        if(header.HasOriginalLength) { return false; }
                        Span<Byte> terminalRecordBuffer = stackalloc Byte[TerminalRecordSize]; terminalRecordBuffer[0] = (Byte)first;
                        if(ReadExact(input,terminalRecordBuffer[1..]) is false || input.ReadByte() != -1 || !TryAuthenticateTerminalRecord(terminalRecordBuffer,header,aesKey,baseNonceSeed,chunkIndex,totalPlain)) { return false; }
                        return true;
                    }

                    lengths[0] = (Byte)first;
                    if(ReadExact(input,lengths[1..]) is false) { return false; }
                    UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                    UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                    if(plainLen == 0 || plainLen != cipherLen || plainLen > (UInt32)chunkSize) { return false; }
                    if(header.HasOriginalLength && (UInt64)plainLen > header.OriginalLength - totalPlain) { return false; }
                    if(ReadExact(input,cipher,0,(Int32)cipherLen) is false) { return false; }
                    Int32 tagLenByte = input.ReadByte(); if(tagLenByte != TagSize || ReadExact(input,tag) is false) { return false; }
                    if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false || BuildPerChunkAad(header.Flags,chunkIndex,plainLen,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { return false; }

                    gcm.Decrypt(nonce,cipher.AsSpan(0,(Int32)cipherLen),tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);
                    output.Write(plain,0,(Int32)plainLen);
                    totalPlain += plainLen;
                    chunkIndex++;
                }

                Span<Byte> terminal = stackalloc Byte[TerminalRecordSize];
                if(ReadExact(input,terminal) is false || input.ReadByte() != -1 || !TryAuthenticateTerminalRecord(terminal,header,aesKey,baseNonceSeed,chunkIndex,totalPlain)) { return false; }
                return true;
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(lengths); ZeroMemory(nonce); ZeroMemory(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptStreamFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptStreamFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="DecryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> DecryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null || cancel.IsCancellationRequested) { return false; }

        if(output is MemoryStream memoryOutput && TryGetRemainingMemory(input,out ReadOnlyMemory<Byte> decryptInput))
        {
            Byte[]? decrypted = DecryptArrayAsyncCore(decryptInput,certificate,aad,cancel); if(decrypted is null) { return false; }
            try
            {
                await memoryOutput.WriteAsync(decrypted.AsMemory(),cancel).ConfigureAwait(false);

                return true;
            }
            finally { ZeroMemory(decrypted); }
        }

        Byte[]? keyMaterial = null;
        var owned = new ConcurrentDictionary<Byte[],Byte>();

        try
        {
            var headerResult = await TryReadAndProcessHeaderAsync(input,certificate,aad,cancel).ConfigureAwait(false); if(headerResult.Success is false) { return false; }

            keyMaterial = headerResult.KeyMaterial; var header = headerResult.Header; Int32 chunkSize = 1 << header.ChunkSizePower; UInt64 totalPlain = 0;
            Byte[] singleByte = ArrayPool<Byte>.Shared.Rent(1); Own(singleByte);
            Byte[] terminalRecord = ArrayPool<Byte>.Shared.Rent(TerminalRecordSize); Own(terminalRecord);
            Boolean terminalSeen = false;
            UInt64 observedChunkCount = 0;
            Exception? readerFailure = null;

            Int32 concurrency = Math.Max(1,DataEncryptionConcurrency);
            using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            CancellationToken pipelineToken = pipelineCancellation.Token;

            var buffer = new BufferBlock<DecryptionChunk>(new DataflowBlockOptions { BoundedCapacity = concurrency , CancellationToken = pipelineToken });

            var execOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = concurrency , MaxDegreeOfParallelism = concurrency , CancellationToken = pipelineToken };

            var writer = new ActionBlock<DecryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { BoundedCapacity = 1 , CancellationToken = pipelineToken });

            var processor = new TransformBlock<DecryptionChunk,DecryptedChunk>(ProcessChunk,execOptions);

            Task cancelOnProcessorFailure = CancelPipelineOnFailure(processor.Completion); Task cancelOnWriterFailure = CancelPipelineOnFailure(writer.Completion);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkOptions); processor.LinkTo(writer,linkOptions);

            try
            {
                await Task.WhenAll(ReadSourceAsync(),buffer.Completion,processor.Completion,writer.Completion,cancelOnProcessorFailure,cancelOnWriterFailure).ConfigureAwait(false);
            }
            catch(Exception _)
            {
                KusDepotLog.Trace(_,DecryptStreamAsyncFail);
                return false;
            }

            if(readerFailure is not null) { return false; }
            if(!terminalSeen || !TryAuthenticateTerminalRecord(terminalRecord.AsSpan(0,TerminalRecordSize),header,keyMaterial.AsSpan(0,AesKeySize),keyMaterial.AsSpan(AesKeySize,BaseNonceSeedSize),observedChunkCount,totalPlain)) { return false; }
            if(await input.ReadAsync(singleByte.AsMemory(0,1),cancel).ConfigureAwait(false) != 0) { return false; }

            return true;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt64 localChunkIndex = 0;
                    while(!pipelineToken.IsCancellationRequested && (!header.HasOriginalLength || totalPlain < header.OriginalLength))
                    {
                        Int32 firstRead = await input.ReadAsync(singleByte.AsMemory(0,1),pipelineToken).ConfigureAwait(false);
                        if(firstRead == 0) { throw new EndOfStreamException(); }
                        if(singleByte[0] == TerminalMarker)
                        {
                            if(header.HasOriginalLength) { throw new InvalidDataException(); }
                            terminalRecord[0] = singleByte[0];
                            if(await ReadExactAsync(input,terminalRecord.AsMemory(1,TerminalRecordSize - 1),pipelineToken).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                            terminalSeen = true;
                            break;
                        }
                        Byte[]? lengths = null;
                        Byte[]? cipher = null;
                        Byte[]? tag = null;
                        try
                        {
                            lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize); Own(lengths);
                            lengths[0] = singleByte[0];
                            if(await ReadExactAsync(input,lengths.AsMemory(1,ChunkLengthsBlockSize - 1),pipelineToken).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                            UInt32 plainLen = ReadUInt32BigEndian(lengths.AsSpan(0,IntSize));
                            UInt32 cipherLen = ReadUInt32BigEndian(lengths.AsSpan(IntSize,IntSize));
                            if(plainLen == 0 || plainLen != cipherLen || plainLen > (UInt32)chunkSize || (header.HasOriginalLength && (UInt64)plainLen > header.OriginalLength - totalPlain)) { throw new InvalidDataException(); }

                            cipher = ArrayPool<Byte>.Shared.Rent((Int32)cipherLen); Own(cipher);
                            tag = ArrayPool<Byte>.Shared.Rent(TagSize); Own(tag);
                            if(await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherLen),pipelineToken).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                            if(await ReadExactAsync(input,singleByte.AsMemory(0,1),pipelineToken).ConfigureAwait(false) is false || singleByte[0] != TagSize) { throw new InvalidDataException(); }
                            if(await ReadExactAsync(input,tag.AsMemory(0,TagSize),pipelineToken).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }

                            totalPlain += plainLen;
                            if(await buffer.SendAsync(new DecryptionChunk(cipher,tag,plainLen,cipherLen,localChunkIndex++),pipelineToken).ConfigureAwait(false) is false) { throw new OperationCanceledException(pipelineToken); }
                            cipher = null;
                            tag = null;
                        }
                        finally
                        {
                            if(lengths is not null) { ReturnOwned(lengths); }
                            if(cipher is not null) { ReturnOwned(cipher); }
                            if(tag is not null) { ReturnOwned(tag); }
                        }
                    }

                    observedChunkCount = localChunkIndex;
                    if(!pipelineToken.IsCancellationRequested && !terminalSeen)
                    {
                        if(await ReadExactAsync(input,terminalRecord.AsMemory(0,TerminalRecordSize),pipelineToken).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                        terminalSeen = true;
                    }
                }
                catch ( Exception _ )
                {
                    readerFailure = _;
                    if(!pipelineCancellation.IsCancellationRequested && !buffer.Completion.IsCompleted) { (buffer as IDataflowBlock).Fault(_); }
                }

                finally { buffer.Complete(); }
            }

            DecryptedChunk ProcessChunk(DecryptionChunk d)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(NonceSize);
                Byte[] perChunkAad = ArrayPool<Byte>.Shared.Rent(PerChunkAadHashSize);
                Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
                Own(nonce); Own(perChunkAad); Own(plain);
                Boolean transferred = false;

                try
                {
                    using var gcm = new AesGcm(keyMaterial!.AsSpan(0,AesKeySize),TagSize);

                    if(DeriveNonce(keyMaterial.AsSpan(0,AesKeySize),keyMaterial.AsSpan(AesKeySize,BaseNonceSeedSize),d.ChunkIndex,nonce) is false) { throw new CryptographicException(); }

                    if(BuildPerChunkAad(header.Flags,d.ChunkIndex,d.PlainLength,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { throw new CryptographicException(); }

                    gcm.Decrypt(nonce.AsSpan(0,NonceSize),d.Cipher.AsSpan(0,(Int32)d.CipherLength),d.Tag.AsSpan(0,TagSize),plain.AsSpan(0,(Int32)d.PlainLength),perChunkAad.AsSpan(0,PerChunkAadHashSize));

                    transferred = true; return new DecryptedChunk(plain,d.PlainLength,d.ChunkIndex);
                }
                finally
                {
                    ReturnOwned(d.Cipher); ReturnOwned(d.Tag); ReturnOwned(nonce); ReturnOwned(perChunkAad);
                    if(!transferred) { ReturnOwned(plain); }
                }
            }

            async Task WriteTargetAsync(DecryptedChunk d)
            {
                try { await output.WriteAsync(d.Plain.AsMemory(0,(Int32)d.PlainLength),pipelineToken).ConfigureAwait(false); }

                finally { ReturnOwned(d.Plain); }
            }

            void Own(Byte[] value) { owned.TryAdd(value,0); }
            Task CancelPipelineOnFailure(Task completion)
            {
                return completion.ContinueWith(completed =>
                {
                    if(completed.IsFaulted || completed.IsCanceled) { pipelineCancellation.Cancel(); }
                },CancellationToken.None,TaskContinuationOptions.ExecuteSynchronously,TaskScheduler.Default);
            }
            void ReturnOwned(Byte[] value)
            {
                if(owned.TryRemove(value,out _)) { ZeroMemory(value); ArrayPool<Byte>.Shared.Return(value); }
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptStreamAsyncFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptStreamAsyncFail); if(NoExceptions) { return false; } throw; }

        finally { foreach(Byte[] value in owned.Keys) { if(owned.TryRemove(value,out _)) { ZeroMemory(value); ArrayPool<Byte>.Shared.Return(value); } } ZeroMemory(keyMaterial); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean EncryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default,
        Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null) { return false; }

        if(output is MemoryStream memoryOutput && TryGetRemainingMemory(input,out ReadOnlyMemory<Byte> encryptInput))
        {
            Byte[]? encrypted = EncryptArrayCore(encryptInput.Span,certificate,aad,includeaadhash,includeoriginallength,chunksizepower); if(encrypted is null) { return false; }

            memoryOutput.Write(encrypted,0,encrypted.Length);

            return true;
        }

        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return false; }

        Span<Byte> aadHash = stackalloc Byte[AadHashSize]; Span<Byte> wrappedMaterial = stackalloc Byte[WrappedMaterialSize];

        Span<Byte> aesKey = stackalloc Byte[AesKeySize]; Span<Byte> baseNonceSeed = stackalloc Byte[BaseNonceSeedSize];

        Span<Byte> rsaWrappedKey = Span<Byte>.Empty;

        try
        {
            Int64? origLen = null;
            if(includeoriginallength)
            {
                if(input.CanSeek is false) { return false; }
                origLen = input.Length - input.Position;
                if(origLen < 0) { return false; }
            }
            UInt64 originalLength = includeoriginallength ? (UInt64)origLen!.Value : 0UL;

            if(includeaadhash)
            {
                if(SHA512.TryHashData(aad,aadHash,out Int32 written) is false || written != AadHashSize) { return false; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);

            aesKey.CopyTo(wrappedMaterial); baseNonceSeed.CopyTo(wrappedMaterial.Slice(AesKeySize,BaseNonceSeedSize));

            using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null) { return false; }
            rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
            ZeroMemory(wrappedMaterial);

            if(WriteHeader(
                output,
                rsaWrappedKey,
                aadHash,
                includeaadhash,
                originalLength,
                includeoriginallength,
                (Byte)chunksizepower) is false)
            { return false; }
            ZeroMemory(rsaWrappedKey);

            Int32 chunkSize = 1 << chunksizepower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> nonce = stackalloc Byte[NonceSize];
            Span<Byte> lengths = stackalloc Byte[ChunkLengthsBlockSize];
            Span<Byte> perChunkAad = stackalloc Byte[PerChunkAadHashSize];
            UInt64 totalWritten = 0; UInt64 chunkIndex = 0; Int32 read;

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                while((read = input.Read(plain,0,chunkSize)) > 0)
                {
                    if(includeoriginallength && (totalWritten + (UInt64)read) > originalLength) { return false; }
                    if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false)             { return false; }
                    if(BuildPerChunkAad(
                        (Byte)((includeoriginallength?Flag_HasOriginalLength:0) | (includeaadhash?Flag_HasAadHash:0)),
                        chunkIndex,
                        (UInt32)read,
                        includeoriginallength,
                        originalLength,
                        includeaadhash ? aadHash : ReadOnlySpan<Byte>.Empty,
                        includeaadhash,
                        perChunkAad) is false)
                    { return false; }

                    gcm.Encrypt(nonce,plain.AsSpan(0,read),cipher.AsSpan(0,read),tag,perChunkAad);
                    WriteUInt32BigEndian(lengths[..IntSize],(UInt32)read);
                    WriteUInt32BigEndian(lengths.Slice(IntSize,IntSize),(UInt32)read);
                    output.Write(lengths);
                    output.Write(cipher,0,read);
                    output.WriteByte(TagSize);
                    output.Write(tag);
                    totalWritten += (UInt64)read;
                    chunkIndex++;
                }

                if(includeoriginallength && totalWritten != originalLength) { return false; }
                Header terminalHeader = new()
                {
                    VersionByte = Version,
                    Flags = (Byte)((includeoriginallength ? Flag_HasOriginalLength : 0) | (includeaadhash ? Flag_HasAadHash : 0)),
                    ChunkSizePower = (Byte)chunksizepower,
                    HasOriginalLength = includeoriginallength,
                    HasAadHash = includeaadhash,
                    OriginalLength = originalLength,
                    AadHash = null
                };
                if(!WriteTerminalRecordStream(output,terminalHeader,includeaadhash ? aadHash : ReadOnlySpan<Byte>.Empty,aesKey,baseNonceSeed,chunkIndex,totalWritten)) { return false; }
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(lengths); ZeroMemory(nonce); ZeroMemory(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }

            return true;
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,EncryptStreamFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptStreamFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); ZeroMemory(rsaWrappedKey); ZeroMemory(wrappedMaterial); }
    }

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/method[@name="EncryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> EncryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default,
        Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null || cancel.IsCancellationRequested) { return false; }

        if(output is MemoryStream memoryOutput && TryGetRemainingMemory(input,out ReadOnlyMemory<Byte> encryptInput))
        {
            Byte[]? encrypted = EncryptArrayAsyncCore(encryptInput,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel); if(encrypted is null) { return false; }
            try
            {
                await memoryOutput.WriteAsync(encrypted.AsMemory(),cancel).ConfigureAwait(false);
                return true;
            }
            finally { ZeroMemory(encrypted); }
        }

        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return false; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null; Byte[]? aadHash = null;
        var owned = new ConcurrentDictionary<Byte[],Byte>();

        try
        {
            aesKey = new Byte[AesKeySize]; baseNonceSeed = new Byte[BaseNonceSeedSize];

            Int64? origLen = null;
            if(includeoriginallength)
            {
                if(input.CanSeek is false) { return false; }
                origLen = input.Length - input.Position; if(origLen < 0) { return false; }
            }
            UInt64 originalLength = includeoriginallength ? (UInt64)origLen!.Value : 0UL;

            if(includeaadhash)
            {
                aadHash = new Byte[AadHashSize]; if(SHA512.TryHashData(aad.Span,aadHash,out Int32 written) is false || written != AadHashSize) { return false; }
            }

            RandomNumberGenerator.Fill(aesKey); RandomNumberGenerator.Fill(baseNonceSeed);

            using(var rsa = certificate.GetRSAPublicKey())
            {
                if(rsa is null) { return false; }
                Byte[] wrappedMaterial = new Byte[WrappedMaterialSize];
                aesKey.CopyTo(wrappedMaterial.AsSpan(0,AesKeySize));
                baseNonceSeed.CopyTo(wrappedMaterial.AsSpan(AesKeySize,BaseNonceSeedSize));
                Byte[] rsaWrappedKey = rsa.Encrypt(wrappedMaterial,RSAEncryptionPadding.OaepSHA512);
                ZeroMemory(wrappedMaterial);
                if(WriteHeader(output,rsaWrappedKey,aadHash,includeaadhash,originalLength,includeoriginallength,(Byte)chunksizepower) is false) { return false; }
                ZeroMemory(rsaWrappedKey);
            }

            Int32 chunkSize = 1 << chunksizepower; UInt64 totalWritten = 0; UInt64 chunkCount = 0;

            Int32 concurrency = Math.Max(1,DataEncryptionConcurrency);
            using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            CancellationToken pipelineToken = pipelineCancellation.Token;

            var buffer = new BufferBlock<EncryptionChunk>(new DataflowBlockOptions { BoundedCapacity = concurrency , CancellationToken = pipelineToken });

            var execOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = concurrency , MaxDegreeOfParallelism = concurrency , CancellationToken = pipelineToken };

            var writer = new ActionBlock<EncryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { BoundedCapacity = 1 , CancellationToken = pipelineToken });

            var processor = new TransformBlock<EncryptionChunk,EncryptedChunk>(ProcessChunk,execOptions);

            Task cancelOnProcessorFailure = CancelPipelineOnFailure(processor.Completion); Task cancelOnWriterFailure = CancelPipelineOnFailure(writer.Completion);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkOptions); processor.LinkTo(writer,linkOptions);

            await Task.WhenAll(ReadSourceAsync(),buffer.Completion,processor.Completion,writer.Completion,cancelOnProcessorFailure,cancelOnWriterFailure).ConfigureAwait(false);

            if(includeoriginallength && totalWritten != originalLength) { return false; }
            Header terminalHeader = new()
            {
                VersionByte = Version,
                Flags = (Byte)((includeoriginallength ? Flag_HasOriginalLength : 0) | (includeaadhash ? Flag_HasAadHash : 0)),
                ChunkSizePower = (Byte)chunksizepower,
                HasOriginalLength = includeoriginallength,
                HasAadHash = includeaadhash,
                OriginalLength = originalLength,
                AadHash = null
            };
            if(!WriteTerminalRecordStream(output,terminalHeader,includeaadhash ? aadHash.AsSpan() : ReadOnlySpan<Byte>.Empty,aesKey,baseNonceSeed,chunkCount,totalWritten)) { return false; }

            return true;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt64 chunkIndex = 0;
                    while(true)
                    {
                        if(pipelineToken.IsCancellationRequested) { break; }
                        Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize); Own(plain);
                        Int32 read = await input.ReadAsync(plain.AsMemory(0,chunkSize),pipelineToken).ConfigureAwait(false);
                        if(read == 0) { ReturnOwned(plain); break; }
                        if(includeoriginallength && (totalWritten + (UInt64)read) > originalLength) { ReturnOwned(plain); throw new InvalidDataException(); }
                        totalWritten += (UInt64)read;
                        Boolean accepted = await buffer.SendAsync(new EncryptionChunk(plain,read,chunkIndex++),pipelineToken).ConfigureAwait(false);
                        if(!accepted) { throw new OperationCanceledException(pipelineToken); }
                    }
                    chunkCount = chunkIndex;
                }
                catch ( Exception _ )
                {
                    if(!pipelineCancellation.IsCancellationRequested && !buffer.Completion.IsCompleted) { (buffer as IDataflowBlock).Fault(_); }
                }

                finally { buffer.Complete(); }
            }

            EncryptedChunk ProcessChunk(EncryptionChunk e)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(NonceSize);
                Byte[] perChunkAad = ArrayPool<Byte>.Shared.Rent(PerChunkAadHashSize);
                Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
                Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize);
                Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
                Own(nonce); Own(perChunkAad); Own(cipher); Own(tag); Own(lengths);
                Boolean transferred = false;

                try
                {
                    using var gcm = new AesGcm(aesKey,TagSize);

                    if(DeriveNonce(aesKey,baseNonceSeed,e.ChunkIndex,nonce) is false) { throw new CryptographicException(); }

                    if(BuildPerChunkAad((Byte)((includeoriginallength?Flag_HasOriginalLength:0) | (includeaadhash?Flag_HasAadHash:0)),e.ChunkIndex,(UInt32)e.ReadLength,includeoriginallength,originalLength,includeaadhash ? aadHash!: ReadOnlySpan<Byte>.Empty,includeaadhash,perChunkAad) is false) { throw new CryptographicException(); }

                    gcm.Encrypt(nonce.AsSpan(0,NonceSize),e.Plain.AsSpan(0,e.ReadLength),cipher.AsSpan(0,e.ReadLength),tag.AsSpan(0,TagSize),perChunkAad.AsSpan(0,PerChunkAadHashSize));

                    WriteUInt32BigEndian(lengths.AsSpan(0,IntSize),(UInt32)e.ReadLength); WriteUInt32BigEndian(lengths.AsSpan(IntSize,IntSize),(UInt32)e.ReadLength);

                    transferred = true; return new EncryptedChunk(cipher,tag,lengths,e.ReadLength,e.ChunkIndex);
                }
                finally
                {
                    ReturnOwned(e.Plain); ReturnOwned(nonce); ReturnOwned(perChunkAad);
                    if(!transferred)
                    {
                        ReturnOwned(cipher); ReturnOwned(tag); ReturnOwned(lengths);
                    }
                }
            }

            async Task WriteTargetAsync(EncryptedChunk e)
            {
                try
                {
                    await output.WriteAsync(e.Lengths.AsMemory(0,ChunkLengthsBlockSize),pipelineToken).ConfigureAwait(false);
                    await output.WriteAsync(e.Cipher.AsMemory(0,e.CipherLength),pipelineToken).ConfigureAwait(false);
                    await output.WriteAsync(new ReadOnlyMemory<Byte>(new[]{ (Byte)TagSize }),pipelineToken).ConfigureAwait(false);
                    await output.WriteAsync(e.Tag.AsMemory(0,TagSize),pipelineToken).ConfigureAwait(false);
                }
                finally
                {
                    ReturnOwned(e.Cipher); ReturnOwned(e.Tag); ReturnOwned(e.Lengths);
                }
            }

            void Own(Byte[] value) { owned.TryAdd(value,0); }
            Task CancelPipelineOnFailure(Task completion)
            {
                return completion.ContinueWith(completed =>
                {
                    if(completed.IsFaulted || completed.IsCanceled) { pipelineCancellation.Cancel(); }
                },CancellationToken.None,TaskContinuationOptions.ExecuteSynchronously,TaskScheduler.Default);
            }
            void ReturnOwned(Byte[] value)
            {
                if(owned.TryRemove(value,out _)) { ZeroMemory(value); ArrayPool<Byte>.Shared.Return(value); }
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,EncryptStreamAsyncFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptStreamAsyncFail); if(NoExceptions) { return false; } throw; }

        finally { foreach(Byte[] value in owned.Keys) { if(owned.TryRemove(value,out _)) { ZeroMemory(value); ArrayPool<Byte>.Shared.Return(value); } } ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); }
    }
}