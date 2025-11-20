namespace KusDepot.Exams.Cryptography;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class GeneralDataEnvelopeExam
{
    private static X509Certificate2 CreateEphemeralCert()
    {

        return Utility.CreateCertificate(Guid.NewGuid(),"GeneralDataEnvelopeExam")!;
    }

    private static Byte[] RandomBytes(Int32 len)
    { var b = new Byte[len]; RandomNumberGenerator.Fill(b); return b; }

    private const Int32 ChunkPower = 14;

    /// <summary>
    /// Verifies that a small payload can be encrypted and decrypted in a single chunk using default envelope settings.
    /// Ensures the decrypted output matches the original input, validating basic round-trip integrity.
    /// </summary>
    [Test]
    public void SingleChunk_RoundTrip_Default()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Encoding.UTF8.GetBytes("hello world");
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Ensures that a multi-chunk payload is correctly encrypted and decrypted using default settings.
    /// Validates that all chunks are reassembled to match the original data.
    /// </summary>
    [Test]
    public void MultiChunk_RoundTrip_Default()
    {
        using var cert = CreateEphemeralCert();
        Int32 chunkSize = 1 << ChunkPower;
        Byte[] plain = RandomBytes(chunkSize * 5 + chunkSize / 3);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Checks that encrypting and decrypting an empty payload produces a valid envelope and results in an empty output.
    /// </summary>
    [Test]
    public void ZeroLength_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Array.Empty<Byte>();
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, chunksizepower : ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests envelope operation in footer mode (without original length in the header), verifying round-trip integrity for a large payload.
    /// </summary>
    [Test]
    public void FooterMode_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(70000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, includeoriginallength: false, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Ensures that truncating the envelope in footer mode is detected and decryption fails, demonstrating integrity protection.
    /// </summary>
    [Test]
    public void FooterMode_TruncationDetected()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(50000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, includeoriginallength: false, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        Byte[] truncated = env!.Take(env!.Length - 1).ToArray();
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(truncated, cert);
        Assert.That(dec, Is.Null);
    }

    /// <summary>
    /// Verifies that truncation is detected when the original length is included in the header, and decryption fails as expected.
    /// </summary>
    [Test]
    public void OriginalLength_TruncationDetected()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(80000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, includeoriginallength: true, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        Byte[] truncated = env!.Take(env!.Length - 10).ToArray();
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(truncated, cert);
        Assert.That(dec, Is.Null);
    }

    /// <summary>
    /// Tests AAD hash binding: decryption succeeds only with the correct AAD and fails with incorrect AAD when binding is enabled.
    /// </summary>
    [Test]
    public void AADHash_Binding_SuccessAndFailure()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(30000);
        Byte[] aad = Encoding.UTF8.GetBytes("user-metadata-context");
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, aad, includeaadhash: true, includeoriginallength: true, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec1 = GeneralDataEnvelope.DecryptArray(env, cert, aad);
        Assert.That(dec1, Is.Not.Null);
        Assert.That(dec1, Is.EquivalentTo(plain));
        Byte[] wrong = Encoding.UTF8.GetBytes("wrong-context");
        Byte[]? dec2 = GeneralDataEnvelope.DecryptArray(env, cert, wrong);
        Assert.That(dec2, Is.Null);
        Byte[]? envDefault = GeneralDataEnvelope.EncryptArray(plain, cert, aad, includeaadhash: true , chunksizepower: ChunkPower);
        Assert.That(envDefault, Is.Not.Null);
        Byte[]? decDefaultGood = GeneralDataEnvelope.DecryptArray(envDefault, cert, aad);
        Assert.That(decDefaultGood, Is.Not.Null);
        Assert.That(decDefaultGood, Is.EquivalentTo(plain));
        Byte[]? decDefaultBad = GeneralDataEnvelope.DecryptArray(envDefault, cert, wrong);
        Assert.That(decDefaultBad, Is.Null);
    }

    /// <summary>
    /// Checks that when AAD hash binding is disabled, mismatched AAD is ignored and decryption still succeeds.
    /// </summary>
    [Test]
    public void AAD_NotBound_Ignored()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(25000);
        Byte[] aad1 = Encoding.UTF8.GetBytes("a1");
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, aad1, includeaadhash: false, includeoriginallength: true, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[] aad2 = Encoding.UTF8.GetBytes("different-aad");
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert, aad2);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that modifying a byte in the chunk ciphertext causes decryption to fail, confirming chunk integrity.
    /// </summary>
    [Test]
    public void Tamper_ChunkCipherByte_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(50000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = ParseHeaderLength(env!);
        UInt32 pLen = ReadUInt32(env!, headerLen);
        Int32 firstCipherOffset = headerLen + 8;
        if(pLen > 0)
        {
            env![firstCipherOffset] ^= 0xFF;
        }
        Byte[]? dec = null;
        Boolean caught = false;
        try
        {
            dec = GeneralDataEnvelope.DecryptArray(env, cert);
        }
        catch (Exception ex)
        {
            caught = ex is AuthenticationTagMismatchException;
            if (!caught) throw;
        }
        Assert.That(caught || dec == null);
    }

    /// <summary>
    /// Ensures that reordering encrypted chunks results in decryption failure, protecting against chunk reordering attacks.
    /// </summary>
    [Test]
    public void Tamper_Reorder_Chunks_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes((1<<ChunkPower)*3 + 1234);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        if(!TryReorderFirstTwoChunks(env!, out var tampered)) Assert.Inconclusive("Could not reorder (unexpected layout)");
        Byte[]? dec = null;
        Boolean caught = false;
        try
        {
            dec = GeneralDataEnvelope.DecryptArray(tampered, cert);
        }
        catch (Exception ex)
        {
            caught = ex is AuthenticationTagMismatchException;
            if (!caught) throw;
        }
        Assert.That(caught || dec == null);
    }

    /// <summary>
    /// Verifies that asynchronous buffer-based encryption and decryption round-trip the data correctly.
    /// </summary>
    [Test]
    public async Task Async_RoundTrip_Buffer()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(120000);
        Byte[]? env = await GeneralDataEnvelope.EncryptArrayAsync(plain, cert, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = await GeneralDataEnvelope.DecryptArrayAsync(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Ensures that async stream-based encryption and decryption work as expected for large payloads.
    /// </summary>
    [Test]
    public async Task Async_Stream_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(90000);
        using var input = new MemoryStream(plain);
        using var encrypted = new MemoryStream();
        Assert.That(await GeneralDataEnvelope.EncryptStreamAsync(input, encrypted, cert, chunksizepower: ChunkPower), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(await GeneralDataEnvelope.DecryptStreamAsync(encrypted, output, cert), Is.True);
        Assert.That(output.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Checks that encryption is properly cancelled when a cancellation token is triggered.
    /// </summary>
    [Test]
    public async Task Async_Cancelled_Encrypt()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(500000);
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        using var cts = new CancellationTokenSource();cts.Cancel();
        Boolean ok = await GeneralDataEnvelope.EncryptStreamAsync(input, output, cert, chunksizepower: ChunkPower, cancel:cts.Token);
        Assert.That(ok, Is.False);
    }

    [Test]
    public void Span_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = Encoding.UTF8.GetBytes("hello world span");
        Span<Byte> env = stackalloc Byte[1024];
        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, env, cert, out Int32 written), Is.True);
        Assert.That(written, Is.GreaterThan(0));

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(GeneralDataEnvelope.DecryptSpan(env[..written], dec, cert, out Int32 decwritten), Is.True);
        Assert.That(decwritten, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);
    }

    [Test]
    public void Span_Interop_EncryptSpan_DecryptArray()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = Encoding.UTF8.GetBytes("interop test");
        Span<Byte> envSpan = stackalloc Byte[1024];
        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, envSpan, cert, out Int32 written), Is.True);

        Byte[]? dec = GeneralDataEnvelope.DecryptArray(envSpan[..written].ToArray(), cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain.ToArray()));
    }

    [Test]
    public void Span_Interop_EncryptArray_DecryptSpan()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Encoding.UTF8.GetBytes("interop test 2");
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(GeneralDataEnvelope.DecryptSpan(env, dec, cert, out Int32 written), Is.True);
        Assert.That(written, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);
    }

    [Test]
    public void Span_ZeroLength_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = ReadOnlySpan<Byte>.Empty;
        Span<Byte> env = stackalloc Byte[1024];
        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, env, cert, out Int32 written), Is.True);
        Assert.That(written, Is.GreaterThan(0));

        Span<Byte> dec = stackalloc Byte[1];
        Assert.That(GeneralDataEnvelope.DecryptSpan(env[..written], dec, cert, out Int32 decwritten), Is.True);
        Assert.That(decwritten, Is.EqualTo(0));
    }

    [Test]
    public void Span_AADHash_Binding()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = RandomBytes(100);
        ReadOnlySpan<Byte> aad = Encoding.UTF8.GetBytes("span aad");
        Span<Byte> env = stackalloc Byte[1024];

        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, env, cert, out Int32 written, aad, includeaadhash: true), Is.True);

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(GeneralDataEnvelope.DecryptSpan(env[..written], dec, cert, out Int32 decwritten, aad), Is.True);
        Assert.That(decwritten, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);

        ReadOnlySpan<Byte> wrongAad = Encoding.UTF8.GetBytes("wrong");
        Assert.That(GeneralDataEnvelope.DecryptSpan(env[..written], dec, cert, out _, wrongAad), Is.False);
    }

    [Test]
    public void Span_OutputTooSmall_Fails()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = RandomBytes(100);
        Span<Byte> env = stackalloc Byte[10];
        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, env, cert, out _), Is.False);

        Byte[]? validEnv = GeneralDataEnvelope.EncryptArray(plain.ToArray(), cert);
        Assume.That(validEnv, Is.Not.Null);
        Span<Byte> dec = stackalloc Byte[plain.Length - 1];
        Assert.That(GeneralDataEnvelope.DecryptSpan(validEnv, dec, cert, out _), Is.False);
    }

    [Test]
    public void Span_Tamper_Fails()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = RandomBytes(100);
        Span<Byte> env = stackalloc Byte[1024];
        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, env, cert, out Int32 written), Is.True);

        env[written - 20] ^= 0xFF; 

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(GeneralDataEnvelope.DecryptSpan(env[..written], dec, cert, out _), Is.False);
    }

    [Test]
    public void Span_Interop_EncryptSpan_DecryptStream()
    {
        using var cert = CreateEphemeralCert();
        ReadOnlySpan<Byte> plain = RandomBytes(200);
        Span<Byte> envSpan = stackalloc Byte[1024];
        Assert.That(GeneralDataEnvelope.EncryptSpan(plain, envSpan, cert, out Int32 written), Is.True);

        using var inputStream = new MemoryStream(envSpan[..written].ToArray());
        using var outputStream = new MemoryStream();
        Assert.That(GeneralDataEnvelope.DecryptStream(inputStream, outputStream, cert), Is.True);
        Assert.That(outputStream.ToArray(), Is.EquivalentTo(plain.ToArray()));
    }

    [Test]
    public void Span_Interop_EncryptStream_DecryptSpan()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(250);
        using var inputStream = new MemoryStream(plain);
        using var encryptedStream = new MemoryStream();
        Assert.That(GeneralDataEnvelope.EncryptStream(inputStream, encryptedStream, cert, chunksizepower: ChunkPower), Is.True);
        
        ReadOnlySpan<Byte> env = encryptedStream.ToArray();
        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(GeneralDataEnvelope.DecryptSpan(env, dec, cert, out Int32 written), Is.True);
        Assert.That(written, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);
    }

    private static IEnumerable<TestCaseData> TinyPayloadsSource()
    {
        Int32[] sizes = {0, 1, 2, 3, 4, 5, 7, 8, 9, 10};
        Boolean[] bools = {true, false};
        foreach (var size in sizes)
        foreach (var includeOrig in bools)
        foreach (var includeAadHash in bools)
        {
            yield return new TestCaseData(size, includeOrig, includeAadHash);
        }
    }

    /// <summary>
    /// Tests all combinations of envelope features (original length, AAD hash) for tiny payloads, ensuring correct operation and AAD binding.
    /// </summary>
    [Test, TestCaseSource(nameof(TinyPayloadsSource))]
    public void TinyPayloads_VariousModes(Int32 size, Boolean includeOrig, Boolean includeAadHash)
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(size);
        Byte[] aad = RandomBytes(5);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad, includeAadHash, includeOrig, ChunkPower);
        Assert.That(env, Is.Not.Null, $"env null size={size} orig={includeOrig} hash={includeAadHash}");
        Byte[] aadDecrypt = includeAadHash ? aad : RandomBytes(5);
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert, aadDecrypt);
        if(includeAadHash)
        {
            Assert.That(dec, Is.Not.Null, "Bound AAD should decrypt with correct AAD");
            Assert.That(dec, Is.EquivalentTo(pt));
        }
        else
        {
            Assert.That(dec, Is.Not.Null, "Unbound AAD should ignore mismatch");
            Assert.That(dec, Is.EquivalentTo(pt));
        }
    }

    /// <summary>
    /// Verifies correct behavior with large AAD values, both with and without hash binding.
    /// Ensures that only the correct AAD decrypts when binding is enabled, and that mismatched AAD is ignored when binding is disabled.
    /// </summary>
    [Test]
    public void LargeAAD_HashBindingAndUnbound()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(50000);
        Byte[] largeAad = RandomBytes(2048);
        Byte[]? envBound = GeneralDataEnvelope.EncryptArray(pt, cert, largeAad, includeaadhash: true, includeoriginallength: true, chunksizepower: ChunkPower);
        Assert.That(envBound, Is.Not.Null);
        Byte[]? decOk = GeneralDataEnvelope.DecryptArray(envBound, cert, largeAad);
        Assert.That(decOk, Is.Not.Null);
        Assert.That(decOk, Is.EquivalentTo(pt));
        Byte[] wrong = RandomBytes(2048);
        Byte[]? decFail = GeneralDataEnvelope.DecryptArray(envBound, cert, wrong);
        Assert.That(decFail, Is.Null, "Different AAD must fail when hashed");
        Byte[]? envUnbound = GeneralDataEnvelope.EncryptArray(pt, cert, largeAad, includeaadhash: false, includeoriginallength: true, chunksizepower: ChunkPower);
        Assert.That(envUnbound, Is.Not.Null);
        Byte[] other = RandomBytes(2048);
        Byte[]? decIgnore = GeneralDataEnvelope.DecryptArray(envUnbound, cert, other);
        Assert.That(decIgnore, Is.Not.Null, "Unbound large AAD mismatch should succeed");
        Assert.That(decIgnore, Is.EquivalentTo(pt));
    }

    /// <summary>
    /// Ensures that corruption of the footer marker or chunk count is detected and decryption fails.
    /// </summary>
    [Test]
    public void FooterCorruption_MarkerAndCount()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(60000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad: null, includeaadhash: false, includeoriginallength: false, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[] corruptMarker = (Byte[])env!.Clone();
        if(corruptMarker.Length < 5) Assert.Inconclusive("Envelope too small");
        corruptMarker[^5] ^= 0x0F;
        Assert.That(GeneralDataEnvelope.DecryptArray(corruptMarker, cert), Is.Null, "Corrupt footer marker should fail");
        Byte[] corruptCount = (Byte[])env.Clone();
        UInt32 origCount = (UInt32)((corruptCount[^4] << 24) | (corruptCount[^3] << 16) | (corruptCount[^2] << 8) | corruptCount[^1]);
        UInt32 newCount = origCount + 1;
        corruptCount[^4] = (Byte)((newCount >> 24) & 0xFF);
        corruptCount[^3] = (Byte)((newCount >> 16) & 0xFF);
        corruptCount[^2] = (Byte)((newCount >> 8) & 0xFF);
        corruptCount[^1] = (Byte)(newCount & 0xFF);
        Assert.That(GeneralDataEnvelope.DecryptArray(corruptCount, cert), Is.Null, "Footer chunk count mismatch should fail");
    }

    /// <summary>
    /// Performs randomized round-trip tests with varying payload sizes, chunk sizes, and envelope features.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void RandomRoundTrips()
    {
        using var cert = CreateEphemeralCert();
        Random rnd = new();
        const Int32 iterations = 20;
        for(Int32 i=0;i<iterations;i++)
        {
            Int32 size = rnd.Next(0, 2 * 1024 * 1024);
            Byte[] pt = RandomBytes(size);
            Int32 chunkPow = rnd.Next(12, 26);
            Boolean includeOrig = rnd.Next(0,2)==1;
            Boolean includeAadHash = rnd.Next(0,2)==1;
            Int32 aadLen = rnd.Next(0, 1024);
            Byte[] aad = RandomBytes(aadLen);
            Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad, includeAadHash, includeOrig, chunkPow);
            Assert.That(env, Is.Not.Null, $"Fuzz encrypt failed iter {i}");
            Byte[] aadDecrypt = includeAadHash ? aad : RandomBytes(aadLen);
            Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert, aadDecrypt);
            if(includeAadHash)
            {
                Assert.That(dec, Is.Not.Null, $"Fuzz decrypt (bound AAD) failed iter {i}");
                Assert.That(dec, Is.EquivalentTo(pt));
            }
            else
            {
                Assert.That(dec, Is.Not.Null, $"Fuzz decrypt (unbound AAD mismatch) failed iter {i}");
                Assert.That(dec, Is.EquivalentTo(pt));
            }
        }
    }

    /// <summary>
    /// Checks that tampering with the authentication tag causes decryption to fail, validating tag integrity.
    /// </summary>
    [Test]
    public void Tamper_TagByte_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(50000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = ParseHeaderLength(env!);
        if(env!.Length < headerLen + 8) Assert.Inconclusive();
        UInt32 pLen = ReadUInt32(env, headerLen); UInt32 cLen = ReadUInt32(env, headerLen+4); if(pLen != cLen) Assert.Inconclusive();
        Int32 tagLenOffset = headerLen + 8 + (Int32)cLen; if(env.Length < tagLenOffset + 1) Assert.Inconclusive();
        env[tagLenOffset + 1] ^= 0xAA;
        Byte[]? dec = null;
        Boolean caught = false;
        try
        {
            dec = GeneralDataEnvelope.DecryptArray(env, cert);
        }
        catch (Exception ex)
        {
            caught = ex is AuthenticationTagMismatchException;
            if (!caught) throw;
        }
        Assert.That(caught || dec == null);
    }

    /// <summary>
    /// Tests that truncating the envelope at a tag boundary is detected and decryption fails.
    /// </summary>
    [Test]
    public void Tamper_TruncateAtTagBoundary_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(80000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = ParseHeaderLength(env!);
        UInt32 pLen = ReadUInt32(env!, headerLen); UInt32 cLen = ReadUInt32(env!, headerLen+4); if(pLen != cLen) Assert.Inconclusive();
        Int32 tagLenOffset = headerLen + 8 + (Int32)cLen; if(env!.Length < tagLenOffset + 1 + 16) Assert.Inconclusive();
        Byte[] truncated = env.Take(tagLenOffset + 1).ToArray();
        Assert.That(GeneralDataEnvelope.DecryptArray(truncated, cert), Is.Null);
    }

    /// <summary>
    /// Ensures that a mismatch between plaintext and ciphertext length fields in a chunk is detected and decryption fails.
    /// </summary>
    [Test]
    public void Tamper_LengthFieldMismatch_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(40000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: ChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = ParseHeaderLength(env!);
        if(env!.Length < headerLen + 8) Assert.Inconclusive();
        UInt32 pLen = ReadUInt32(env, headerLen); if(pLen == UInt32.MaxValue) Assert.Inconclusive();
        UInt32 spoofCipherLen = pLen + 1;
        env[headerLen+4] = (Byte)((spoofCipherLen >> 24) & 0xFF);
        env[headerLen+5] = (Byte)((spoofCipherLen >> 16) & 0xFF);
        env[headerLen+6] = (Byte)((spoofCipherLen >> 8) & 0xFF);
        env[headerLen+7] = (Byte)(spoofCipherLen & 0xFF);
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.Null);
    }

    /// <summary>
    /// Confirms that envelopes produced with different AAD (when hash binding is enabled) are distinct and only decrypt with their respective AAD.
    /// </summary>
    [Test]
    public void AAD_Binding_EnvelopesDiffer_ForDifferentAAD()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(60000);
        Byte[] aad1 = Encoding.UTF8.GetBytes("aad-one");
        Byte[] aad2 = Encoding.UTF8.GetBytes("aad-two");
        Byte[]? env1 = GeneralDataEnvelope.EncryptArray(pt, cert, aad1, includeaadhash: true, includeoriginallength: true, chunksizepower: ChunkPower);
        Byte[]? env2 = GeneralDataEnvelope.EncryptArray(pt, cert, aad2, includeaadhash: true, includeoriginallength: true, chunksizepower: ChunkPower);
        Assert.That(env1, Is.Not.Null);
        Assert.That(env2, Is.Not.Null);
        Assert.That(env1!.SequenceEqual(env2!), Is.False, "Different AAD should produce distinct envelopes when hashed");
        Assert.That(GeneralDataEnvelope.DecryptArray(env1, cert, aad1), Is.EqualTo(pt));
        Assert.That(GeneralDataEnvelope.DecryptArray(env2, cert, aad2), Is.EqualTo(pt));
    }

    /// <summary>
    /// Explicit test for very large payloads, verifying stream-based round-trip correctness and spot-checking output.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void LargePayload_StreamRoundTrip()
    {
        using var cert = CreateEphemeralCert();        
        Int32 size = 200 * 1024 * 1024;
        Byte[] pt = RandomBytes(size);
        using var input = new MemoryStream(pt);
        using var encrypted = new MemoryStream();
        Assert.That(GeneralDataEnvelope.EncryptStream(input, encrypted, cert, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: 20), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(GeneralDataEnvelope.DecryptStream(encrypted, output, cert), Is.True);
        Assert.That(output.ToArray().Length, Is.EqualTo(size));
        Byte[] dec = output.GetBuffer().AsSpan(0, size).ToArray();
        Assert.That(dec.AsSpan(0,64).ToArray(), Is.EqualTo(pt.AsSpan(0,64).ToArray()));
        Assert.That(dec.AsSpan(size/2,64).ToArray(), Is.EqualTo(pt.AsSpan(size/2,64).ToArray()));
        Assert.That(dec.AsSpan(size-64,64).ToArray(), Is.EqualTo(pt.AsSpan(size-64,64).ToArray()));
    }

    /// <summary>
    /// Checks that an empty payload is correctly handled in footer mode.
    /// </summary>
    [Test]
    public void ZeroLength_FooterMode_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Array.Empty<Byte>();
        Byte[]? env = GeneralDataEnvelope.EncryptArray(plain, cert, aad : null, includeaadhash : false, includeoriginallength : false, chunksizepower : ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
    }

    private static IEnumerable<TestCaseData> ChunkSizeVariabilitySource()
    {
        Int32[] powers = {12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26};
        Int32[] multipliers = {1, 2, 3};
        foreach (var p in powers)
        {
            Int32 chunk = 1 << p;
            foreach (var mult in multipliers)
            {
                // Exact multiple
                yield return new TestCaseData(p, chunk * mult).SetCategory("Extended");
                // Boundary -1
                if (chunk * mult > 0)
                {
                    yield return new TestCaseData(p, chunk * mult - 1).SetCategory("Extended");
                }
                // Boundary +1
                yield return new TestCaseData(p, chunk * mult + 1).SetCategory("Extended");
            }
        }
    }

    /// <summary>
    /// Tests envelope correctness for a range of chunk sizes and boundary conditions (exact, -1, +1 sizes).
    /// </summary>
    [Test, Explicit, TestCaseSource(nameof(ChunkSizeVariabilitySource))]
    public void ChunkSizeVariability_Boundaries(Int32 power, Int32 size)
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(size);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: power);
        Assert.That(env, Is.Not.Null, $"Encryption failed for chunk power 2^{power}, size {size}");
        Byte[]? dec = GeneralDataEnvelope.DecryptArray(env, cert);
        Assert.That(dec, Is.EqualTo(pt), $"Decryption failed for chunk power 2^{power}, size {size}");
    }

    private static Int32 ParseHeaderLength(Byte[] env)
    {
        if(env.Length < 4) throw new InvalidOperationException();
        UInt16 rsaLen = (UInt16)(env[2] << 8 | env[3]);
        Int32 offset = 4 + rsaLen;
        if(env.Length <= offset) throw new InvalidOperationException();
        Byte chunkPower = env[offset++];
        Byte flags = env[1];
        if((flags & 0x01) != 0) offset += 8;
        if((flags & 0x02) != 0) offset += 64;
        return offset;
    }

    private static UInt32 ReadUInt32(Byte[] buffer, Int32 offset)
        => (UInt32)((buffer[offset] << 24) | (buffer[offset+1] << 16) | (buffer[offset+2] << 8) | buffer[offset+3]);

    private static Boolean TryReorderFirstTwoChunks(Byte[] env, out Byte[] tampered)
    {
        tampered = env;
        try
        {
            Int32 headerLen = ParseHeaderLength(env);
            Int32 pos = headerLen;
            if(env.Length < pos + 8) return false;
            UInt32 p1 = ReadUInt32(env,pos); UInt32 c1 = ReadUInt32(env,pos+4); if(p1 != c1) return false; Int32 chunk1CipherOff = pos + 8; Int32 chunk1TagLenOff;
            chunk1TagLenOff = chunk1CipherOff + (Int32)c1; if(env.Length < chunk1TagLenOff + 1) return false; Int32 tagLen1 = env[chunk1TagLenOff]; if(tagLen1 != 16) return false; Int32 chunk1End = chunk1TagLenOff + 1 + tagLen1;
            if(env.Length < chunk1End + 8) return false;
            Int32 pos2 = chunk1End; UInt32 p2 = ReadUInt32(env,pos2); UInt32 c2 = ReadUInt32(env,pos2+4); if(p2 != c2) return false;
            Int32 chunk2CipherOff = pos2 + 8; Int32 chunk2TagLenOff = chunk2CipherOff + (Int32)c2; if(env.Length < chunk2TagLenOff + 1) return false;
            Int32 tagLen2 = env[chunk2TagLenOff]; if(tagLen2 != 16) return false; Int32 chunk2End = chunk2TagLenOff + 1 + tagLen2;
            Byte[] first = env.Skip(pos).Take(chunk1End - pos).ToArray();
            Byte[] second = env.Skip(pos2).Take(chunk2End - pos2).ToArray();
            tampered = new Byte[env.Length];
            Buffer.BlockCopy(env,0,tampered,0,pos);
            Buffer.BlockCopy(second,0,tampered,pos,second.Length);
            Buffer.BlockCopy(first,0,tampered,pos+second.Length,first.Length);
            Buffer.BlockCopy(env,chunk2End,tampered,pos+second.Length+first.Length,env.Length - chunk2End);
            return true;
        }
        catch { return false; }
    }

    private static X509Certificate2 CreateRsaCert(Int32 keySize)
    {
        return Utility.CreateCertificate(Guid.NewGuid(),"CryptoCoreExam-RSA",keySize)!;
    }

    /// <summary>
    /// Ensures that decryption fails with a different RSA private key than was used for encryption, and succeeds with the correct key.
    /// </summary>
    [Test]
    public void RSA_DifferentPrivateKey_Fails()
    {
        using var certEnc = CreateRsaCert(2048);
        using var certWrong = CreateRsaCert(2048);
        Byte[] pt = RandomBytes(32000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, certEnc, chunksizepower: ChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? decWrong = null;
        Boolean caught = false;
        try
        {
            decWrong = GeneralDataEnvelope.DecryptArray(env, certWrong);
        }
        catch (Exception ex)
        {
            caught = ex is CryptographicException;
            if (!caught) throw;
        }
        Assert.That(caught || decWrong == null);
        Assert.That(GeneralDataEnvelope.DecryptArray(env, certEnc), Is.EqualTo(pt));
    }

    /// <summary>
    /// Verifies that the envelope works with large RSA key sizes (e.g., 8192, 16384 bits).
    /// </summary>
    [TestCase(4096)]
    [TestCase(8192)]
    [TestCase(16384)]
    [Explicit, Category("Extended")]
    public void RSA_LargeKeySizes_Succeed(Int32 keySize)
    {
        using var cert = CreateRsaCert(keySize);
        Byte[] pt = RandomBytes(50000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad : null, includeaadhash : false, includeoriginallength : true, chunksizepower : ChunkPower);
        Assert.That(env, Is.Not.Null, $"Encrypt failed for keySize {keySize}");
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.EqualTo(pt), $"Decrypt failed for keySize {keySize}");
    }

    /// <summary>
    /// Checks that tampering with the version byte in the header is detected and decryption fails.
    /// </summary>
    [Test]
    public void HeaderCorrupt_VersionByte_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(16000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad : null, includeaadhash : false, includeoriginallength : true, chunksizepower : ChunkPower);
        Assert.That(env, Is.Not.Null);
        env![0] ^= 0x01;
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.Null);
    }

    /// <summary>
    /// Ensures that setting reserved flag bits in the header causes decryption to fail.
    /// </summary>
    [Test]
    public void HeaderCorrupt_ReservedFlagBit_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(8000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad : null, includeaadhash : false, includeoriginallength : true, chunksizepower : ChunkPower);
        Assert.That(env, Is.Not.Null);
        env![1] |= 0x04;
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that out-of-range chunk size power values in the header are detected and decryption fails.
    /// </summary>
    [Test]
    public void HeaderCorrupt_ChunkSizePower_OutOfRange_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(10000);
        Byte[]? env = GeneralDataEnvelope.EncryptArray(pt, cert, aad : null, includeaadhash : false, includeoriginallength : true, chunksizepower : 16);
        Assert.That(env, Is.Not.Null);
        UInt16 rsaLen = (UInt16)((env![2] << 8) | env[3]);
        Int32 index = 4 + rsaLen; if(index >= env.Length) Assert.Inconclusive();
        env[index] = 11;
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.Null);
        env[index] = 27;
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.Null);
        env[index] = 20;
        Assert.That(GeneralDataEnvelope.DecryptArray(env, cert), Is.Not.Null);
    }
}