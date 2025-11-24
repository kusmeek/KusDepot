namespace KusDepot.Cryptography;

internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean DecryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null) { return false; }

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
            UInt64 totalPlain = 0; UInt32 chunkIndex = 0;

            try
            {
                using var gcm = new AesGcm(aesKey,TagSize);
                if(header.HasOriginalLength)
                {
                    Int64 startPos = input.Position;
                    while(true)
                    {
                        Int32 lenRead = input.Read(lengths);
                        if(lenRead == 0) { break; } if(lenRead != ChunkLengthsBlockSize) { return false; }
                        UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                        if(plainLen != cipherLen)                                        { return false; }
                        if(plainLen > (UInt32)chunkSize)                                 { return false; }
                        if(ReadExact(input,cipher,0,(Int32)cipherLen) is false)          { return false; }
                        Int32 tagLenByte = input.ReadByte(); if(tagLenByte != TagSize)   { return false; }
                        if(ReadExact(input,tag) is false)                                { return false; }
                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false)  { return false; }

                        if(BuildPerChunkAad(
                            header.Flags,
                            chunkIndex,
                            plainLen,
                            header.HasOriginalLength,
                            header.OriginalLength,
                            header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                            header.HasAadHash,
                            perChunkAad) is false)
                        { return false; }

                        gcm.Decrypt(nonce,cipher.AsSpan(0,(Int32)cipherLen),tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);

                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                    }

                    if(totalPlain != header.OriginalLength)             { return false; }
                    if(input.CanSeek && input.Position != input.Length) { return false; }
                }
                else
                {
                    while(true)
                    {
                        Int32 b = input.ReadByte(); if(b == -1)        { return false; }
                        if(b == FooterMarker)
                        {
                            Span<Byte> footerRemain = stackalloc Byte[IntSize];
                            if(ReadExact(input,footerRemain) is false) { return false; }
                            UInt32 chunkCountFooter = ReadUInt32BigEndian(footerRemain);
                            if(chunkCountFooter != chunkIndex)         { return false; }
                            if(input.ReadByte() != -1)                 { return false; }
                            break;
                        }

                        lengths[0] = (Byte)b;
                        if(ReadExact(input,lengths[1..ChunkLengthsBlockSize]) is false) { return false; }
                        UInt32 plainLen = ReadUInt32BigEndian(lengths[..IntSize]);
                        UInt32 cipherLen = ReadUInt32BigEndian(lengths.Slice(IntSize,IntSize));
                        if(plainLen != cipherLen)                                       { return false; }
                        if(plainLen > (UInt32)chunkSize)                                { return false; }
                        if(ReadExact(input,cipher,0,(Int32)cipherLen) is false)         { return false; }
                        Int32 tagLenByte = input.ReadByte(); if(tagLenByte != TagSize)  { return false; }
                        if(ReadExact(input,tag) is false)                               { return false; }
                        if(DeriveNonce(aesKey,baseNonceSeed,chunkIndex,nonce) is false) { return false; }
                        if(BuildPerChunkAad(
                            header.Flags,
                            chunkIndex,
                            plainLen,
                            header.HasOriginalLength,
                            header.OriginalLength,
                            header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,
                            header.HasAadHash,
                            perChunkAad) is false)
                        { return false; }

                        gcm.Decrypt(nonce,cipher.AsSpan(0,(Int32)cipherLen),tag,plain.AsSpan(0,(Int32)plainLen),perChunkAad);

                        output.Write(plain,0,(Int32)plainLen);
                        totalPlain += plainLen;
                        chunkIndex++;
                    }
                }
            }
            finally
            {
                ZeroMemory(perChunkAad); ZeroMemory(lengths); ZeroMemory(nonce); ZeroMemory(tag);
                ZeroMemory(cipher); ArrayPool<Byte>.Shared.Return(cipher); ZeroMemory(plain); ArrayPool<Byte>.Shared.Return(plain);
            }

            return true;
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptStreamFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptStreamFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> DecryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null || cancel.IsCancellationRequested) { return false; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null;

        try
        {
            var headerResult = await TryReadAndProcessHeaderAsync(input,certificate,aad,cancel).ConfigureAwait(false); if(headerResult.Success is false) { return false; }

            aesKey = headerResult.AesKey; var header = headerResult.Header; baseNonceSeed = headerResult.BaseNonceSeed; Int32 chunkSize = 1 << header.ChunkSizePower; UInt64 totalPlain = 0;

            var buffer = new BufferBlock<DecryptionChunk>(new DataflowBlockOptions { BoundedCapacity = DataEncryptionConcurrency , CancellationToken = cancel });

            var execOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataEncryptionConcurrency , CancellationToken = cancel };

            var writer = new ActionBlock<DecryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { CancellationToken = cancel });

            var processor = new TransformBlock<DecryptionChunk,DecryptedChunk>(ProcessChunk,execOptions);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkOptions); processor.LinkTo(writer,linkOptions);

            await Task.WhenAll(ReadSourceAsync(),writer.Completion).ConfigureAwait(false);

            if(header.HasOriginalLength)
            {
                if(input.CanSeek && input.Position != input.Length) { return false; }
            }

            return true;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt32 chunkIndex = 0;
                    if(header.HasOriginalLength)
                    {
                        while(totalPlain < header.OriginalLength)
                        {
                            if(cancel.IsCancellationRequested) { break; }
                            UInt32 plainLen; UInt32 cipherLen; Byte[]? lengths = null;
                            try
                            {
                                lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
                                Int32 lenRead = await input.ReadAsync(lengths.AsMemory(0,ChunkLengthsBlockSize),cancel).ConfigureAwait(false);
                                if(lenRead == 0) { break; } if(lenRead != ChunkLengthsBlockSize) { throw new InvalidDataException(); }
                                plainLen = ReadUInt32BigEndian(lengths.AsSpan(0,IntSize));
                                cipherLen = ReadUInt32BigEndian(lengths.AsSpan(IntSize,IntSize));
                            }
                            finally { if(lengths is not null) { ArrayPool<Byte>.Shared.Return(lengths); } }

                            if(plainLen != cipherLen || plainLen > (UInt32)chunkSize) { throw new InvalidDataException(); }
                            Byte[] cipher = ArrayPool<Byte>.Shared.Rent((Int32)cipherLen);
                            Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize); Byte[]? singleByte = null;
                            try
                            {
                                singleByte = ArrayPool<Byte>.Shared.Rent(1);
                                if(await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherLen),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                if(await ReadExactAsync(input,singleByte.AsMemory(0,1),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                if(singleByte[0] != TagSize) { throw new InvalidDataException(); }
                            }
                            finally { if(singleByte is not null) { ArrayPool<Byte>.Shared.Return(singleByte); } }
                            if(await ReadExactAsync(input,tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                            totalPlain += plainLen;
                            await buffer.SendAsync(new DecryptionChunk(cipher,tag,plainLen,cipherLen,chunkIndex++),cancel).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        UInt32 chunkIndexFooter = 0; Byte[]? singleByte = null;
                        try
                        {
                            singleByte = ArrayPool<Byte>.Shared.Rent(1);
                            while(true)
                            {
                                if(cancel.IsCancellationRequested) { break; }
                                Int32 b = await input.ReadAsync(singleByte.AsMemory(0,1),cancel).ConfigureAwait(false);
                                if(b == 0) { break; }
                                if(singleByte[0] == FooterMarker)
                                {
                                    Byte[]? footerRemain = null;
                                    try
                                    {
                                        footerRemain = ArrayPool<Byte>.Shared.Rent(IntSize);
                                        if(await ReadExactAsync(input,footerRemain.AsMemory(0,IntSize),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                        if(ReadUInt32BigEndian(footerRemain.AsSpan(0,IntSize)) != chunkIndexFooter) { throw new InvalidDataException(); }
                                        if((await input.ReadAsync(singleByte.AsMemory(0,1),cancel).ConfigureAwait(false)) != 0) { throw new InvalidDataException(); }
                                        break;
                                    }
                                    finally { if(footerRemain is not null) { ArrayPool<Byte>.Shared.Return(footerRemain); } }
                                }
                                Byte[]? lengths = null; UInt32 plainLen; UInt32 cipherLen;
                                try
                                {
                                    lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);
                                    lengths[0] = singleByte[0];
                                    if(await ReadExactAsync(input,lengths.AsMemory(1,ChunkLengthsBlockSize - 1),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                    plainLen = ReadUInt32BigEndian(lengths.AsSpan(0,IntSize));
                                    cipherLen = ReadUInt32BigEndian(lengths.AsSpan(IntSize,IntSize));
                                }
                                finally { if(lengths is not null) { ArrayPool<Byte>.Shared.Return(lengths); } }
                                if(plainLen != cipherLen || plainLen > (UInt32)chunkSize) { throw new InvalidDataException(); }
                                Byte[] cipher = ArrayPool<Byte>.Shared.Rent((Int32)cipherLen);
                                Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize);
                                if(await ReadExactAsync(input,cipher.AsMemory(0,(Int32)cipherLen),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                if(await ReadExactAsync(input,singleByte.AsMemory(0,1),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                if(singleByte[0] != TagSize) { throw new InvalidDataException(); }
                                if(await ReadExactAsync(input,tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false) is false) { throw new EndOfStreamException(); }
                                await buffer.SendAsync(new DecryptionChunk(cipher,tag,plainLen,cipherLen,chunkIndexFooter++),cancel).ConfigureAwait(false);
                            }
                        }
                        finally { if(singleByte is not null) { ArrayPool<Byte>.Shared.Return(singleByte); } }
                    }
                }
                catch ( Exception _ ) { (buffer as IDataflowBlock).Fault(_); }

                finally { buffer.Complete(); }
            }

            DecryptedChunk ProcessChunk(DecryptionChunk d)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(NonceSize);
                Byte[] perChunkAad = ArrayPool<Byte>.Shared.Rent(PerChunkAadHashSize);
                Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);

                try
                {
                    using var gcm = new AesGcm(aesKey!,TagSize);

                    if(DeriveNonce(aesKey!,baseNonceSeed!,d.ChunkIndex,nonce) is false) { throw new CryptographicException(); }

                    if(BuildPerChunkAad(header.Flags,d.ChunkIndex,d.PlainLength,header.HasOriginalLength,header.OriginalLength,header.HasAadHash ? header.AadHash! : ReadOnlySpan<Byte>.Empty,header.HasAadHash,perChunkAad) is false) { throw new CryptographicException(); }

                    gcm.Decrypt(nonce.AsSpan(0,NonceSize),d.Cipher.AsSpan(0,(Int32)d.CipherLength),d.Tag.AsSpan(0,TagSize),plain.AsSpan(0,(Int32)d.PlainLength),perChunkAad);

                    return new DecryptedChunk(plain,d.PlainLength,d.ChunkIndex);
                }
                finally
                {
                    ArrayPool<Byte>.Shared.Return(d.Cipher); ArrayPool<Byte>.Shared.Return(d.Tag);
                    ArrayPool<Byte>.Shared.Return(nonce); ArrayPool<Byte>.Shared.Return(perChunkAad);
                }
            }

            async Task WriteTargetAsync(DecryptedChunk d)
            {
                try { await output.WriteAsync(d.Plain.AsMemory(0,(Int32)d.PlainLength),cancel).ConfigureAwait(false); }

                finally { ArrayPool<Byte>.Shared.Return(d.Plain); }
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,DecryptStreamAsyncFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptStreamAsyncFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean EncryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default,
        Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null) { return false; }

        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower)                                     { return false; }

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
            UInt64 totalWritten = 0; UInt32 chunkIndex = 0; Int32 read;

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

                if(includeoriginallength) { if(totalWritten != originalLength) { return false; } }
                else { if(WriteFooter(output,chunkIndex) is false) { return false; } }
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

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> EncryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default,
        Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || output is null || output.CanWrite is false || certificate is null || cancel.IsCancellationRequested) { return false; }

        if(chunksizepower < MinChunkSizePower || chunksizepower > MaxChunkSizePower) { return false; }

        Byte[]? aesKey = null; Byte[]? baseNonceSeed = null; Byte[]? aadHash = null;

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

            Int32 chunkSize = 1 << chunksizepower; UInt64 totalWritten = 0;

            var buffer = new BufferBlock<EncryptionChunk>(new DataflowBlockOptions { BoundedCapacity = DataEncryptionConcurrency , CancellationToken = cancel });

            var execOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataEncryptionConcurrency , CancellationToken = cancel };

            var writer = new ActionBlock<EncryptedChunk>(WriteTargetAsync,new ExecutionDataflowBlockOptions { CancellationToken = cancel });

            var processor = new TransformBlock<EncryptionChunk,EncryptedChunk>(ProcessChunk,execOptions);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(processor,linkOptions); processor.LinkTo(writer,linkOptions);

            await Task.WhenAll(ReadSourceAsync(),writer.Completion).ConfigureAwait(false);

            if(includeoriginallength) { if(totalWritten != originalLength) { return false; } }

            else { if(WriteFooter(output,(UInt32)writer.InputCount) is false) { return false; } }

            return true;

            async Task ReadSourceAsync()
            {
                try
                {
                    UInt32 chunkIndex = 0;
                    while(true)
                    {
                        if(cancel.IsCancellationRequested) { break; }
                        Byte[] plain = ArrayPool<Byte>.Shared.Rent(chunkSize);
                        Int32 read = await input.ReadAsync(plain.AsMemory(0,chunkSize),cancel).ConfigureAwait(false);
                        if(read == 0) { ArrayPool<Byte>.Shared.Return(plain); break; }
                        if(includeoriginallength && (totalWritten + (UInt64)read) > originalLength) { ArrayPool<Byte>.Shared.Return(plain); throw new InvalidDataException(); }
                        totalWritten += (UInt64)read;
                        await buffer.SendAsync(new EncryptionChunk(plain,read,chunkIndex++),cancel).ConfigureAwait(false);
                    }
                }
                catch ( Exception _ ) { (buffer as IDataflowBlock).Fault(_); }

                finally { buffer.Complete(); }
            }

            EncryptedChunk ProcessChunk(EncryptionChunk e)
            {
                Byte[] nonce = ArrayPool<Byte>.Shared.Rent(NonceSize);
                Byte[] perChunkAad = ArrayPool<Byte>.Shared.Rent(PerChunkAadHashSize);
                Byte[] cipher = ArrayPool<Byte>.Shared.Rent(chunkSize);
                Byte[] tag = ArrayPool<Byte>.Shared.Rent(TagSize);
                Byte[] lengths = ArrayPool<Byte>.Shared.Rent(ChunkLengthsBlockSize);

                try
                {
                    using var gcm = new AesGcm(aesKey,TagSize);

                    if(DeriveNonce(aesKey,baseNonceSeed,e.ChunkIndex,nonce) is false) { throw new CryptographicException(); }

                    if(BuildPerChunkAad((Byte)((includeoriginallength?Flag_HasOriginalLength:0) | (includeaadhash?Flag_HasAadHash:0)),e.ChunkIndex,(UInt32)e.ReadLength,includeoriginallength,originalLength,includeaadhash ? aadHash!: ReadOnlySpan<Byte>.Empty,includeaadhash,perChunkAad) is false) { throw new CryptographicException(); }

                    gcm.Encrypt(nonce.AsSpan(0,NonceSize),e.Plain.AsSpan(0,e.ReadLength),cipher.AsSpan(0,e.ReadLength),tag.AsSpan(0,TagSize),perChunkAad);

                    WriteUInt32BigEndian(lengths.AsSpan(0,IntSize),(UInt32)e.ReadLength); WriteUInt32BigEndian(lengths.AsSpan(IntSize,IntSize),(UInt32)e.ReadLength);

                    return new EncryptedChunk(cipher,tag,lengths,e.ReadLength,e.ChunkIndex);
                }
                finally
                {
                    ArrayPool<Byte>.Shared.Return(e.Plain);
                    ArrayPool<Byte>.Shared.Return(nonce);
                    ArrayPool<Byte>.Shared.Return(perChunkAad);
                }
            }

            async Task WriteTargetAsync(EncryptedChunk e)
            {
                try
                {
                    await output.WriteAsync(e.Lengths.AsMemory(0,ChunkLengthsBlockSize),cancel).ConfigureAwait(false);
                    await output.WriteAsync(e.Cipher.AsMemory(0,e.CipherLength),cancel).ConfigureAwait(false);
                    await output.WriteAsync(new ReadOnlyMemory<Byte>(new[]{ (Byte)TagSize }),cancel).ConfigureAwait(false);
                    await output.WriteAsync(e.Tag.AsMemory(0,TagSize),cancel).ConfigureAwait(false);
                }
                finally
                {
                    ArrayPool<Byte>.Shared.Return(e.Cipher);
                    ArrayPool<Byte>.Shared.Return(e.Tag);
                    ArrayPool<Byte>.Shared.Return(e.Lengths);
                }
            }
        }
        catch ( CryptographicException _ ) { KusDepotLog.Trace(_,EncryptStreamAsyncFail); return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptStreamAsyncFail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(aesKey); ZeroMemory(baseNonceSeed); ZeroMemory(aadHash); }
    }
}