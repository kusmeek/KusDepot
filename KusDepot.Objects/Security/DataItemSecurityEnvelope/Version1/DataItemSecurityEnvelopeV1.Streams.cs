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
            UInt32 chunkindex = 0;
            UInt64 totalplain = 0;

            try
            {
                using var gcm = new AesGcm(aeskey,TagSize);

                while(totalplain < header.OriginalLength)
                {
                    if(!ReadExact(input,lengths.AsSpan(0,ChunkLengthBlockSize))) { return false; }

                    UInt32 plainlength = ReadUInt32BigEndian(lengths.AsSpan(0,4));
                    UInt32 cipherlength = ReadUInt32BigEndian(lengths.AsSpan(4,4));

                    if(plainlength != cipherlength || plainlength > (UInt32)chunksize) { return false; }
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

                return totalplain == header.OriginalLength && (!input.CanSeek || input.Position == input.Length);
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

            Int32 chunksize = 1 << header.ChunkSizePower; UInt64 totalplain = 0;

            var buffer = new BufferBlock<DecryptionChunk>(new DataflowBlockOptions { BoundedCapacity = DataEncryptionConcurrency , CancellationToken = cancel });

            var execoptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataEncryptionConcurrency , CancellationToken = cancel };

            var writer = new ActionBlock<DecryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { CancellationToken = cancel, EnsureOrdered = true });

            var processor = new TransformBlock<DecryptionChunk,DecryptedChunk>(ProcessChunk,execoptions);

            var linkoptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkoptions); processor.LinkTo(writer,linkoptions);

            await Task.WhenAll(ReadSourceAsync(),writer.Completion).ConfigureAwait(false);

            return totalplain == header.OriginalLength && (!input.CanSeek || input.Position == input.Length);

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt32 chunkindex = 0;
                    Byte[]? lengths = null;
                    Byte[]? tag = null;

                    while(totalplain < header.OriginalLength)
                    {
                        cancel.ThrowIfCancellationRequested();

                        lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthBlockSize);
                        if(!await ReadExactAsync(input,lengths.AsMemory(0,ChunkLengthBlockSize),cancel).ConfigureAwait(false)) { throw new EndOfStreamException(); }

                        UInt32 plainlength = ReadUInt32BigEndian(lengths.AsSpan(0,4));
                        UInt32 cipherlength = ReadUInt32BigEndian(lengths.AsSpan(4,4));
                        if(plainlength != cipherlength || plainlength > (UInt32)chunksize) { throw new InvalidDataException(); }

                        Byte[] cipher = ArrayPool<Byte>.Shared.Rent((Int32)cipherlength);
                        if(!await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherlength),cancel).ConfigureAwait(false)) { throw new EndOfStreamException(); }

                        Byte[] taglengthbuffer = new Byte[1];
                        if(!await ReadExactAsync(input,taglengthbuffer.AsMemory(0,1),cancel).ConfigureAwait(false) || taglengthbuffer[0] != TagSize) { throw new InvalidDataException(); }

                        tag = ArrayPool<Byte>.Shared.Rent(TagSize);
                        if(!await ReadExactAsync(input,tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false)) { throw new EndOfStreamException(); }

                        totalplain += plainlength;
                        await buffer.SendAsync(new DecryptionChunk(cipher,tag,plainlength,cipherlength,chunkindex++),cancel).ConfigureAwait(false);

                        ArrayPool<Byte>.Shared.Return(lengths); lengths = null; tag = null;
                    }
                }
                catch ( Exception _ ) { (buffer as IDataflowBlock).Fault(_); }

                finally { buffer.Complete(); }
            }

            DecryptedChunk ProcessChunk(DecryptionChunk value)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(12);
                Byte[] aad = ArrayPool<Byte>.Shared.Rent(RootContextHashSize);
                Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunksize);

                try
                {
                    using var gcm = new AesGcm(aeskey,TagSize);
                    if(!DeriveNonce(aeskey,basenonceseed!,value.ChunkIndex,nonce) || !BuildPerChunkAad(header,value.ChunkIndex,value.PlainLength,aad)) { throw new CryptographicException(); }

                    gcm.Decrypt(nonce.AsSpan(0,12),value.Cipher.AsSpan(0,(Int32)value.CipherLength),value.Tag.AsSpan(0,TagSize),plain.AsSpan(0,(Int32)value.PlainLength),aad.AsSpan(0,RootContextHashSize));

                    return new DecryptedChunk(plain,value.PlainLength,value.ChunkIndex);
                }
                finally
                {
                    ZeroMemory(value.Cipher);
                    ArrayPool<Byte>.Shared.Return(value.Cipher);
                    ZeroMemory(value.Tag);
                    ArrayPool<Byte>.Shared.Return(value.Tag);
                    ZeroMemory(nonce);
                    ArrayPool<Byte>.Shared.Return(nonce);
                    ZeroMemory(aad);
                    ArrayPool<Byte>.Shared.Return(aad);
                }
            }

            async Task WriteTargetAsync(DecryptedChunk value)
            {
                try { await output.WriteAsync(value.Plain.AsMemory(0,(Int32)value.PlainLength),cancel).ConfigureAwait(false); }
                finally
                {
                    ZeroMemory(value.Plain);
                    ArrayPool<Byte>.Shared.Return(value.Plain);
                }
            }
        }
        catch ( CryptographicException ) { return false; }
        catch ( IOException ) { return false; }
        catch ( InvalidDataException ) { return false; }
        catch ( OperationCanceledException ) { return false; }
        finally
        {
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
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
            UInt32 chunkindex = 0;
            Int32 read;

            try
            {
                using var gcm = new AesGcm(aeskey,TagSize);

                while((read = input.Read(plain,0,chunksize)) > 0)
                {
                    if((totalwritten + (UInt64)read) > originallength) { return false; }
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

                return totalwritten == originallength;
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
            UInt32 chunkcount = 0;
            Byte[] taglength = [TagSize];

            var buffer = new BufferBlock<EncryptionChunk>(new DataflowBlockOptions { BoundedCapacity = DataEncryptionConcurrency , CancellationToken = cancel });

            var execoptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataEncryptionConcurrency , CancellationToken = cancel };

            var writer = new ActionBlock<EncryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { CancellationToken = cancel, EnsureOrdered = true });

            var processor = new TransformBlock<EncryptionChunk,EncryptedChunk>(ProcessChunk,execoptions);

            var linkoptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkoptions); processor.LinkTo(writer,linkoptions);

            await Task.WhenAll(ReadSourceAsync(),writer.Completion).ConfigureAwait(false);

            return totalwritten == originallength;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt32 chunkindex = 0;

                    while(true)
                    {
                        cancel.ThrowIfCancellationRequested();

                        Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunksize);
                        Int32 read = await input.ReadAsync(plain.AsMemory(0,chunksize),cancel).ConfigureAwait(false);
                        if(read == 0)
                        {
                            ZeroMemory(plain);
                            ArrayPool<Byte>.Shared.Return(plain);
                            break;
                        }

                        if((totalwritten + (UInt64)read) > originallength)
                        {
                            ZeroMemory(plain);
                            ArrayPool<Byte>.Shared.Return(plain);
                            throw new InvalidDataException();
                        }

                        totalwritten += (UInt64)read;
                        await buffer.SendAsync(new EncryptionChunk(plain,read,chunkindex++),cancel).ConfigureAwait(false);
                    }

                    chunkcount = chunkindex;
                }
                catch ( Exception _ ) { (buffer as IDataflowBlock).Fault(_); }
                finally { buffer.Complete(); }
            }

            EncryptedChunk ProcessChunk(EncryptionChunk value)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(12);
                Byte[] aad = ArrayPool<Byte>.Shared.Rent(RootContextHashSize);
                Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunksize);
                Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize);
                Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthBlockSize);

                try
                {
                    using var gcm = new AesGcm(aeskey,TagSize);
                    if(!DeriveNonce(aeskey,basenonceseed!,value.ChunkIndex,nonce) || !BuildPerChunkAadCore(header.RootContextHash,envelopeMetadataHashBytes,value.ChunkIndex,(UInt32)value.ReadLength,aad)) { throw new CryptographicException(); }

                    gcm.Encrypt(nonce.AsSpan(0,12),value.Plain.AsSpan(0,value.ReadLength),cipher.AsSpan(0,value.ReadLength),tag.AsSpan(0,TagSize),aad.AsSpan(0,RootContextHashSize));
                    WriteUInt32BigEndian(lengths.AsSpan(0,4),(UInt32)value.ReadLength);
                    WriteUInt32BigEndian(lengths.AsSpan(4,4),(UInt32)value.ReadLength);

                    return new EncryptedChunk(cipher,tag,lengths,value.ReadLength,value.ChunkIndex);
                }
                finally
                {
                    ZeroMemory(value.Plain);
                    ArrayPool<Byte>.Shared.Return(value.Plain);
                    ZeroMemory(nonce);
                    ArrayPool<Byte>.Shared.Return(nonce);
                    ZeroMemory(aad);
                    ArrayPool<Byte>.Shared.Return(aad);
                }
            }

            async Task WriteTargetAsync(EncryptedChunk value)
            {
                try
                {
                    await output.WriteAsync(value.Lengths.AsMemory(0,ChunkLengthBlockSize),cancel).ConfigureAwait(false);
                    await output.WriteAsync(value.Cipher.AsMemory(0,value.CipherLength),cancel).ConfigureAwait(false);
                    await output.WriteAsync(taglength.AsMemory(0,1),cancel).ConfigureAwait(false);
                    await output.WriteAsync(value.Tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false);
                }
                finally
                {
                    ZeroMemory(value.Lengths);
                    ArrayPool<Byte>.Shared.Return(value.Lengths);
                    ZeroMemory(value.Cipher);
                    ArrayPool<Byte>.Shared.Return(value.Cipher);
                    ZeroMemory(value.Tag);
                    ArrayPool<Byte>.Shared.Return(value.Tag);
                }
            }
        }
        catch ( CryptographicException ) { return false; }
        catch ( IOException ) { return false; }
        catch ( InvalidDataException ) { return false; }
        catch ( OperationCanceledException ) { return false; }
        finally
        {
            ZeroMemory(wrappedmaterial);
            ZeroMemory(aeskey);
            ZeroMemory(basenonceseed);
        }
    }
}