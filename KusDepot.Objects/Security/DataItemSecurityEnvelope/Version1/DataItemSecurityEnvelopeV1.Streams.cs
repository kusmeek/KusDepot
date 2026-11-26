namespace KusDepot.Security.Data;

internal static partial class DataItemSecurityEnvelopeV1
{
    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean DecryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> rootcontext = default)
    {
        if(input is null || output is null || certificate is null || !input.CanRead || !output.CanWrite) { return false; }

        Byte[]? aeskey = null; Byte[]? basenonceseed = null;

        try
        {
            Byte[] headerbuffer = new Byte[FixedHeaderSize];
            if(!ReadExact(input,headerbuffer)) { return false; }
            if(!TryParseHeader(headerbuffer,out Header header)) { return false; }
            if(!TryValidateRootContext(rootcontext,header.RootContextHash.AsSpan())) { return false; }

            if(header.RecipientTableLength > Int32.MaxValue) { return false; }

            Byte[] recipientbuffer = new Byte[(Int32)header.RecipientTableLength];
            if(!ReadExact(input,recipientbuffer)) { return false; }
            if(!TryValidateEnvelopeMetadata(header,recipientbuffer,header.EnvelopeMetadataHash.AsSpan())) { return false; }
            if(!TryParseRecipientTable(recipientbuffer,header.RecipientCount,out ImmutableArray<RecipientEntry> recipients)) { return false; }
            if(!TryResolveRecipientMaterial(recipients,certificate,out aeskey,out basenonceseed) || aeskey is null || basenonceseed is null) { return false; }

            Int32 chunksize = 1 << header.ChunkSizePower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunksize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunksize);
            Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthBlockSize);
            Span<Byte> nonce = stackalloc Byte[12];
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> aad = stackalloc Byte[RootContextHashSize];
            UInt64 chunkindex = 0;
            UInt64 totalplain = 0;

            try
            {
                using var gcm = new AesGcm(aeskey,TagSize);

                while(totalplain < header.OriginalLength)
                {
                    if(!ReadExact(input,lengths.AsSpan(0,ChunkLengthBlockSize))) { return false; }

                    UInt32 plainlength = ReadUInt32BigEndian(lengths.AsSpan(0,4));
                    UInt32 cipherlength = ReadUInt32BigEndian(lengths.AsSpan(4,4));

                    if(plainlength == 0 || plainlength != cipherlength || plainlength > (UInt32)chunksize || plainlength > header.OriginalLength - totalplain) { return false; }
                    if(!ReadExact(input,cipher.AsSpan(0,(Int32)cipherlength))) { return false; }

                    Int32 taglength = input.ReadByte();
                    if(taglength != TagSize) { return false; }
                    if(!ReadExact(input,tag)) { return false; }
                    if(!DeriveNonce(aeskey,basenonceseed,chunkindex,nonce) || !BuildPerChunkAad(header,chunkindex,plainlength,aad)) { return false; }

                    gcm.Decrypt(nonce,cipher.AsSpan(0,(Int32)cipherlength),tag,plain.AsSpan(0,(Int32)plainlength),aad);
                    output.Write(plain,0,(Int32)plainlength);

                    totalplain += plainlength;
                    chunkindex++;
                }

                if(totalplain != header.OriginalLength) { return false; }

                Span<Byte> terminal = stackalloc Byte[TerminalRecordSize];
                if(!ReadExact(input,terminal) || !TryAuthenticateTerminalRecord(terminal,aeskey,basenonceseed,header.EnvelopeMetadataHash,chunkindex,totalplain)) { return false; }

                return input.CanSeek ? input.Position == input.Length : input.ReadByte() == -1;
            }
            finally
            {
                ZeroMemory(plain);
                ArrayPool<Byte>.Shared.Return(plain);
                ZeroMemory(cipher);
                ArrayPool<Byte>.Shared.Return(cipher);
                ZeroMemory(lengths);
                ArrayPool<Byte>.Shared.Return(lengths);
                ZeroMemory(nonce);
                ZeroMemory(tag);
                ZeroMemory(aad);
            }
        }
        catch ( CryptographicException ) { return false; }
        catch ( IOException ) { return false; }
        catch ( InvalidDataException ) { return false; }
        finally
        {
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptStreamAsync"]/*'/>*/
    internal static Task<Boolean> DecryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext = default , CancellationToken cancel = default)
    {
        return DecryptStreamAsyncCore(input,output,certificate,rootcontext,cancel);
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="DecryptStreamAsyncCore"]/*'/>*/
    private static async Task<Boolean> DecryptStreamAsyncCore(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext , CancellationToken cancel)
    {
        if(input is null || output is null || certificate is null || !input.CanRead || !output.CanWrite || cancel.IsCancellationRequested) { return false; }

        Byte[]? aeskey = null; Byte[]? basenonceseed = null;
        var owned = new ConcurrentDictionary<Byte[],Byte>();

        try
        {
            Byte[] headerbuffer = new Byte[FixedHeaderSize];
            if(!await ReadExactAsync(input,headerbuffer.AsMemory(0,FixedHeaderSize),cancel).ConfigureAwait(false)) { return false; }
            if(!TryParseHeader(headerbuffer,out Header header)) { return false; }
            if(!TryValidateRootContext(rootcontext.Span,header.RootContextHash.AsSpan())) { return false; }
            if(header.RecipientTableLength > Int32.MaxValue) { return false; }

            Byte[] recipientbuffer = new Byte[(Int32)header.RecipientTableLength];
            if(!await ReadExactAsync(input,recipientbuffer.AsMemory(0,recipientbuffer.Length),cancel).ConfigureAwait(false)) { return false; }
            if(!TryValidateEnvelopeMetadata(header,recipientbuffer,header.EnvelopeMetadataHash.AsSpan())) { return false; }
            if(!TryParseRecipientTable(recipientbuffer,header.RecipientCount,out ImmutableArray<RecipientEntry> recipients)) { return false; }
            if(!TryResolveRecipientMaterial(recipients,certificate,out aeskey,out basenonceseed) || aeskey is null || basenonceseed is null) { return false; }

            Int32 chunksize = 1 << header.ChunkSizePower; UInt64 totalplain = 0; UInt64 chunkcount = 0;
            Byte[] singlebyte = ArrayPool<Byte>.Shared.Rent(1); Own(singlebyte);

            Int32 concurrency = Math.Max(1,DataEncryptionConcurrency);
            using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            CancellationToken pipelineToken = pipelineCancellation.Token;

            var buffer = new BufferBlock<DecryptionChunk>(new DataflowBlockOptions { BoundedCapacity = concurrency , CancellationToken = pipelineToken });

            var execoptions = new ExecutionDataflowBlockOptions { BoundedCapacity = concurrency , MaxDegreeOfParallelism = concurrency , CancellationToken = pipelineToken };

            var writer = new ActionBlock<DecryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { BoundedCapacity = 1 , CancellationToken = pipelineToken, EnsureOrdered = true });

            var processor = new TransformBlock<DecryptionChunk,DecryptedChunk>(ProcessChunk,execoptions);

            Task cancelOnProcessorFailure = CancelPipelineOnFailure(processor.Completion); Task cancelOnWriterFailure = CancelPipelineOnFailure(writer.Completion);

            var linkoptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkoptions); processor.LinkTo(writer,linkoptions);

            await Task.WhenAll(ReadSourceAsync(),buffer.Completion,processor.Completion,writer.Completion,cancelOnProcessorFailure,cancelOnWriterFailure).ConfigureAwait(false);

            if(totalplain != header.OriginalLength) { return false; }

            Byte[] terminal = new Byte[TerminalRecordSize];
            if(!await ReadExactAsync(input,terminal.AsMemory(),cancel).ConfigureAwait(false) || !TryAuthenticateTerminalRecord(terminal,aeskey,basenonceseed,header.EnvelopeMetadataHash,chunkcount,totalplain)) { return false; }
            if(input.CanSeek) { return input.Position == input.Length; }

            return await input.ReadAsync(singlebyte.AsMemory(0,1),cancel).ConfigureAwait(false) == 0;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt64 chunkindex = 0;

                    while(totalplain < header.OriginalLength)
                    {
                        pipelineToken.ThrowIfCancellationRequested();

                        UInt32 plainlength; UInt32 cipherlength; Byte[]? lengths = null;
                        try
                        {
                            lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthBlockSize); Own(lengths);
                            if(!await ReadExactAsync(input,lengths.AsMemory(0,ChunkLengthBlockSize),pipelineToken).ConfigureAwait(false)) { throw new EndOfStreamException(); }

                            plainlength = ReadUInt32BigEndian(lengths.AsSpan(0,4));
                            cipherlength = ReadUInt32BigEndian(lengths.AsSpan(4,4));
                        }
                        finally { if(lengths is not null) { ReturnOwned(lengths); } }

                        if(plainlength == 0 || plainlength != cipherlength || plainlength > (UInt32)chunksize || plainlength > header.OriginalLength - totalplain) { throw new InvalidDataException(); }

                        Byte[]? cipher = ArrayPool<Byte>.Shared.Rent((Int32)cipherlength); Own(cipher);
                        Byte[]? tag = ArrayPool<Byte>.Shared.Rent(TagSize); Own(tag);
                        try
                        {
                            if(!await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherlength),pipelineToken).ConfigureAwait(false)) { throw new EndOfStreamException(); }
                            if(!await ReadExactAsync(input,singlebyte.AsMemory(0,1),pipelineToken).ConfigureAwait(false) || singlebyte[0] != TagSize) { throw new InvalidDataException(); }
                            if(!await ReadExactAsync(input,tag.AsMemory(0,TagSize),pipelineToken).ConfigureAwait(false)) { throw new EndOfStreamException(); }

                            totalplain += plainlength;
                            if(!await buffer.SendAsync(new DecryptionChunk(cipher!,tag!,plainlength,cipherlength,chunkindex++),pipelineToken).ConfigureAwait(false)) { throw new OperationCanceledException(pipelineToken); }
                            cipher = null; tag = null;
                        }
                        finally
                        {
                            if(cipher is not null) { ReturnOwned(cipher); }
                            if(tag is not null) { ReturnOwned(tag); }
                        }
                    }

                    chunkcount = chunkindex;
                }
                catch ( Exception _ )
                {
                    if(!pipelineCancellation.IsCancellationRequested && !buffer.Completion.IsCompleted) { (buffer as IDataflowBlock).Fault(_); }
                }

                finally { buffer.Complete(); }
            }

            DecryptedChunk ProcessChunk(DecryptionChunk value)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(12); Own(nonce);
                Byte[] aad = ArrayPool<Byte>.Shared.Rent(RootContextHashSize); Own(aad);
                Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunksize); Own(plain);
                Boolean transferred = false;

                try
                {
                    using var gcm = new AesGcm(aeskey,TagSize);
                    if(!DeriveNonce(aeskey,basenonceseed!,value.ChunkIndex,nonce) || !BuildPerChunkAad(header,value.ChunkIndex,value.PlainLength,aad)) { throw new CryptographicException(); }

                    gcm.Decrypt(nonce.AsSpan(0,12),value.Cipher.AsSpan(0,(Int32)value.CipherLength),value.Tag.AsSpan(0,TagSize),plain.AsSpan(0,(Int32)value.PlainLength),aad.AsSpan(0,RootContextHashSize));

                    transferred = true;
                    return new DecryptedChunk(plain,value.PlainLength,value.ChunkIndex);
                }
                finally
                {
                    ReturnOwned(value.Cipher); ReturnOwned(value.Tag); ReturnOwned(nonce); ReturnOwned(aad);
                    if(!transferred) { ReturnOwned(plain); }
                }
            }

            async Task WriteTargetAsync(DecryptedChunk value)
            {
                try { await output.WriteAsync(value.Plain.AsMemory(0,(Int32)value.PlainLength),pipelineToken).ConfigureAwait(false); }
                finally
                {
                    ReturnOwned(value.Plain);
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
        catch ( Exception ) { return false; }
        finally
        {
            foreach(Byte[] value in owned.Keys)
            {
                if(owned.TryRemove(value,out _)) { ZeroMemory(value); ArrayPool<Byte>.Shared.Return(value); }
            }
            ZeroMemory(aeskey); ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean EncryptStream(Stream? input , Stream? output , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlySpan<Byte> rootcontext = default , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || output is null || !input.CanRead || !output.CanWrite || recipients.IsDefaultOrEmpty) { return false; }
        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower || !input.CanSeek) { return false; }

        Byte[] wrappedmaterial = new Byte[WrappedMaterialSize];
        Byte[] aeskey = new Byte[AesKeySize];
        Byte[] basenonceseed = new Byte[BaseNonceSeedSize];

        try
        {
            UInt64 originallength = checked((UInt64)(input.Length - input.Position));
            RandomNumberGenerator.Fill(aeskey);
            RandomNumberGenerator.Fill(basenonceseed);
            aeskey.CopyTo(wrappedmaterial,0);
            basenonceseed.CopyTo(wrappedmaterial,AesKeySize);

            Header header = CreateHeader(recipients,issuer,originallength,(Byte)chunksizepower,rootcontext);
            Byte[] recipientbytes = SerializeRecipientTable(recipients,wrappedmaterial);
            Span<Byte> recipientTableHash = stackalloc Byte[RootContextHashSize];
            Span<Byte> envelopeMetadataHash = stackalloc Byte[EnvelopeMetadataHashSize];
            if(!ComputeRecipientTableHash(recipientbytes,recipientTableHash) || !ComputeEnvelopeMetadataHash(header,recipientTableHash,envelopeMetadataHash)) { return false; }
            Byte[] envelopeMetadataHashBytes = envelopeMetadataHash.ToArray();
            header = new Header(header.VersionByte,header.Flags,header.AlgorithmSuite,header.ChunkSizePower,header.RecipientCount,header.RecipientTableLength,header.OriginalLength,header.RootContextHash,envelopeMetadataHashBytes);
            Byte[] headerbytes = SerializeHeader(header);
            envelopeMetadataHashBytes.CopyTo(headerbytes.AsSpan(18 + RootContextHashSize,EnvelopeMetadataHashSize));

            output.Write(headerbytes,0,headerbytes.Length);
            output.Write(recipientbytes,0,recipientbytes.Length);

            Int32 chunksize = 1 << chunksizepower;
            Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunksize);
            Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunksize);
            Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthBlockSize);
            Span<Byte> nonce = stackalloc Byte[12];
            Span<Byte> tag = stackalloc Byte[TagSize];
            Span<Byte> aad = stackalloc Byte[RootContextHashSize];
            UInt64 totalwritten = 0;
            UInt64 chunkindex = 0;
            Int32 read;

            try
            {
                using var gcm = new AesGcm(aeskey,TagSize);

                while((read = input.Read(plain,0,chunksize)) > 0)
                {
                    if((UInt64)read > originallength - totalwritten) { return false; }
                    if(!DeriveNonce(aeskey,basenonceseed,chunkindex,nonce) || !BuildPerChunkAadCore(header.RootContextHash,envelopeMetadataHashBytes,chunkindex,(UInt32)read,aad)) { return false; }

                    gcm.Encrypt(nonce,plain.AsSpan(0,read),cipher.AsSpan(0,read),tag,aad);
                    WriteUInt32BigEndian(lengths.AsSpan(0,4),(UInt32)read);
                    WriteUInt32BigEndian(lengths.AsSpan(4,4),(UInt32)read);

                    output.Write(lengths,0,ChunkLengthBlockSize);
                    output.Write(cipher,0,read);
                    output.WriteByte(TagSize);
                    output.Write(tag);

                    totalwritten += (UInt64)read;
                    chunkindex++;
                }

                return totalwritten == originallength && WriteTerminalRecord(output,aeskey,basenonceseed,envelopeMetadataHashBytes,chunkindex,totalwritten);
            }
            finally
            {
                ZeroMemory(plain);
                ArrayPool<Byte>.Shared.Return(plain);
                ZeroMemory(cipher);
                ArrayPool<Byte>.Shared.Return(cipher);
                ZeroMemory(lengths);
                ArrayPool<Byte>.Shared.Return(lengths);
                ZeroMemory(nonce);
                ZeroMemory(tag);
                ZeroMemory(aad);
            }
        }
        catch ( CryptographicException ) { return false; }
        catch ( IOException ) { return false; }
        catch ( InvalidDataException ) { return false; }
        finally
        {
            ZeroMemory(wrappedmaterial);
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptStreamAsync"]/*'/>*/
    internal static Task<Boolean> EncryptStreamAsync(Stream? input , Stream? output , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext = default , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        return EncryptStreamAsyncCore(input,output,recipients,issuer,rootcontext,chunksizepower,cancel);
    }

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/method[@name="EncryptStreamAsyncCore"]/*'/>*/
    private static async Task<Boolean> EncryptStreamAsyncCore(Stream? input , Stream? output , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext , Int32 chunksizepower , CancellationToken cancel)
    {
        if(input is null || output is null || !input.CanRead || !output.CanWrite || recipients.IsDefaultOrEmpty || cancel.IsCancellationRequested) { return false; }
        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower || !input.CanSeek) { return false; }

        Byte[]? wrappedmaterial = null; Byte[]? aeskey = null; Byte[]? basenonceseed = null;
        var owned = new ConcurrentDictionary<Byte[],Byte>();

        try
        {
            wrappedmaterial = new Byte[WrappedMaterialSize];
            aeskey = new Byte[AesKeySize];
            basenonceseed = new Byte[BaseNonceSeedSize];

            UInt64 originallength = checked((UInt64)(input.Length - input.Position));
            RandomNumberGenerator.Fill(aeskey);
            RandomNumberGenerator.Fill(basenonceseed);
            aeskey.CopyTo(wrappedmaterial,0);
            basenonceseed.CopyTo(wrappedmaterial,AesKeySize);

            Header header = CreateHeader(recipients,issuer,originallength,(Byte)chunksizepower,rootcontext.Span);
            Byte[] recipientbytes = SerializeRecipientTable(recipients,wrappedmaterial);
            Span<Byte> recipientTableHash = stackalloc Byte[RootContextHashSize];
            Span<Byte> envelopeMetadataHash = stackalloc Byte[EnvelopeMetadataHashSize];
            if(!ComputeRecipientTableHash(recipientbytes,recipientTableHash) || !ComputeEnvelopeMetadataHash(header,recipientTableHash,envelopeMetadataHash)) { return false; }
            Byte[] envelopeMetadataHashBytes = envelopeMetadataHash.ToArray();
            header = new Header(header.VersionByte,header.Flags,header.AlgorithmSuite,header.ChunkSizePower,header.RecipientCount,header.RecipientTableLength,header.OriginalLength,header.RootContextHash,envelopeMetadataHashBytes);
            Byte[] headerbytes = SerializeHeader(header);
            envelopeMetadataHashBytes.CopyTo(headerbytes.AsSpan(18 + RootContextHashSize,EnvelopeMetadataHashSize));

            await output.WriteAsync(headerbytes.AsMemory(0,headerbytes.Length),cancel).ConfigureAwait(false);
            await output.WriteAsync(recipientbytes.AsMemory(0,recipientbytes.Length),cancel).ConfigureAwait(false);

            Int32 chunksize = 1 << chunksizepower;
            UInt64 totalwritten = 0;
            UInt64 chunkcount = 0;
            Byte[] taglength = [TagSize];

            Int32 concurrency = Math.Max(1,DataEncryptionConcurrency);
            using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            CancellationToken pipelineToken = pipelineCancellation.Token;

            var buffer = new BufferBlock<EncryptionChunk>(new DataflowBlockOptions { BoundedCapacity = concurrency , CancellationToken = pipelineToken });

            var execoptions = new ExecutionDataflowBlockOptions { BoundedCapacity = concurrency , MaxDegreeOfParallelism = concurrency , CancellationToken = pipelineToken };

            var writer = new ActionBlock<EncryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { BoundedCapacity = 1 , CancellationToken = pipelineToken, EnsureOrdered = true });

            var processor = new TransformBlock<EncryptionChunk,EncryptedChunk>(ProcessChunk,execoptions);

            Task cancelOnProcessorFailure = CancelPipelineOnFailure(processor.Completion); Task cancelOnWriterFailure = CancelPipelineOnFailure(writer.Completion);

            var linkoptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkoptions); processor.LinkTo(writer,linkoptions);

            await Task.WhenAll(ReadSourceAsync(),buffer.Completion,processor.Completion,writer.Completion,cancelOnProcessorFailure,cancelOnWriterFailure).ConfigureAwait(false);

            if(totalwritten != originallength) { return false; }

            Byte[] terminal = new Byte[TerminalRecordSize];
            if(!WriteTerminalRecord(terminal,0,aeskey,basenonceseed,envelopeMetadataHashBytes,chunkcount,totalwritten)) { return false; }
            await output.WriteAsync(terminal.AsMemory(),cancel).ConfigureAwait(false);
            return true;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt64 chunkindex = 0;

                    while(true)
                    {
                        pipelineToken.ThrowIfCancellationRequested();

                        Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunksize); Own(plain);
                        Int32 read;
                        try
                        {
                            read = await input.ReadAsync(plain.AsMemory(0,chunksize),pipelineToken).ConfigureAwait(false);
                            if(read == 0) { ReturnOwned(plain); break; }

                            if((UInt64)read > originallength - totalwritten) { throw new InvalidDataException(); }

                            totalwritten += (UInt64)read;
                            if(!await buffer.SendAsync(new EncryptionChunk(plain,read,chunkindex++),pipelineToken).ConfigureAwait(false)) { throw new OperationCanceledException(pipelineToken); }
                            plain = null!;
                        }
                        finally { if(plain is not null) { ReturnOwned(plain); } }
                    }

                    chunkcount = chunkindex;
                }
                catch ( Exception _ )
                {
                    if(!pipelineCancellation.IsCancellationRequested && !buffer.Completion.IsCompleted) { (buffer as IDataflowBlock).Fault(_); }
                }
                finally { buffer.Complete(); }
            }

            EncryptedChunk ProcessChunk(EncryptionChunk value)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(12); Own(nonce);
                Byte[] aad = ArrayPool<Byte>.Shared.Rent(RootContextHashSize); Own(aad);
                Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunksize); Own(cipher);
                Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize); Own(tag);
                Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthBlockSize); Own(lengths);
                Boolean transferred = false;

                try
                {
                    using var gcm = new AesGcm(aeskey,TagSize);
                    if(!DeriveNonce(aeskey,basenonceseed!,value.ChunkIndex,nonce) || !BuildPerChunkAadCore(header.RootContextHash,envelopeMetadataHashBytes,value.ChunkIndex,(UInt32)value.ReadLength,aad)) { throw new CryptographicException(); }

                    gcm.Encrypt(nonce.AsSpan(0,12),value.Plain.AsSpan(0,value.ReadLength),cipher.AsSpan(0,value.ReadLength),tag.AsSpan(0,TagSize),aad.AsSpan(0,RootContextHashSize));
                    WriteUInt32BigEndian(lengths.AsSpan(0,4),(UInt32)value.ReadLength);
                    WriteUInt32BigEndian(lengths.AsSpan(4,4),(UInt32)value.ReadLength);

                    transferred = true;
                    return new EncryptedChunk(cipher,tag,lengths,value.ReadLength,value.ChunkIndex);
                }
                finally
                {
                    ReturnOwned(value.Plain); ReturnOwned(nonce); ReturnOwned(aad);
                    if(!transferred) { ReturnOwned(cipher); ReturnOwned(tag); ReturnOwned(lengths); }
                }
            }

            async Task WriteTargetAsync(EncryptedChunk value)
            {
                try
                {
                    await output.WriteAsync(value.Lengths.AsMemory(0,ChunkLengthBlockSize),pipelineToken).ConfigureAwait(false);
                    await output.WriteAsync(value.Cipher.AsMemory(0,value.CipherLength),pipelineToken).ConfigureAwait(false);
                    await output.WriteAsync(taglength.AsMemory(0,1),pipelineToken).ConfigureAwait(false);
                    await output.WriteAsync(value.Tag.AsMemory(0,TagSize),pipelineToken).ConfigureAwait(false);
                }
                finally
                {
                    ReturnOwned(value.Lengths); ReturnOwned(value.Cipher); ReturnOwned(value.Tag);
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
        catch ( Exception ) { return false; }
        finally
        {
            foreach(Byte[] value in owned.Keys)
            {
                if(owned.TryRemove(value,out _)) { ZeroMemory(value); ArrayPool<Byte>.Shared.Return(value); }
            }
            ZeroMemory(wrappedmaterial); ZeroMemory(aeskey); ZeroMemory(basenonceseed);
        }
    }
}