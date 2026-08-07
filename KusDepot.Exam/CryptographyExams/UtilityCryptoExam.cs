namespace KusDepot.Exams.Cryptography;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public partial class UtilityCryptoExam
{
    private const Int32 EnvelopeChunkPower = 14;

    private static Byte[] RandomBytes(Int32 size)
    {
        Byte[] bytes = new Byte[size];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }

    private X509Certificate2? CertA;

    [OneTimeSetUp]
    public void Calibrate()
    {
        CertA = CreateExamCert("UtilityCryptoExamA");
        Check.That(CertA).IsNotNull();
    }

    private static X509Certificate2 CreateExamCert(String name, Int32 keySize = 2048)
    {
        return CreateCertificate(Guid.NewGuid(), name, keySize, 2, null)!;
    }

    private static void AssertDecryptFailsOrThrows<TException>(Func<Byte[]?> decrypt)
        where TException : Exception
    {
        Byte[]? dec = null;
        Boolean caught = false;
        try
        {
            dec = decrypt();
        }
        catch (Exception ex)
        {
            caught = ex is TException;
            if (!caught) throw;
        }

        Assert.That(caught || dec == null);
    }

    private static Int32 GetHeaderLength(Byte[] env)
    {
        if(env.Length < 4) throw new InvalidOperationException();
        UInt16 rsaLen = (UInt16)(env[2] << 8 | env[3]);
        Int32 offset = 4 + rsaLen;
        if(env.Length <= offset) throw new InvalidOperationException();
        offset++;
        Byte flags = env[1];
        if((flags & 0x01) != 0) offset += 8;
        if((flags & 0x02) != 0) offset += 64;
        return offset;
    }

    private static UInt32 ReadUInt32(Byte[] buffer, Int32 offset)
        => (UInt32)((buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3]);

    private static Boolean TryReorderFirstTwoChunks(Byte[] env, out Byte[] tampered)
    {
        tampered = env;
        try
        {
            Int32 headerLen = GetHeaderLength(env);
            Int32 pos = headerLen;
            if(env.Length < pos + 8) return false;

            UInt32 p1 = ReadUInt32(env, pos);
            UInt32 c1 = ReadUInt32(env, pos + 4);
            if(p1 != c1) return false;

            Int32 chunk1CipherOff = pos + 8;
            Int32 chunk1TagLenOff = chunk1CipherOff + (Int32)c1;
            if(env.Length < chunk1TagLenOff + 1) return false;

            Int32 tagLen1 = env[chunk1TagLenOff];
            if(tagLen1 != 16) return false;

            Int32 chunk1End = chunk1TagLenOff + 1 + tagLen1;
            if(env.Length < chunk1End + 8) return false;

            Int32 pos2 = chunk1End;
            UInt32 p2 = ReadUInt32(env, pos2);
            UInt32 c2 = ReadUInt32(env, pos2 + 4);
            if(p2 != c2) return false;

            Int32 chunk2CipherOff = pos2 + 8;
            Int32 chunk2TagLenOff = chunk2CipherOff + (Int32)c2;
            if(env.Length < chunk2TagLenOff + 1) return false;

            Int32 tagLen2 = env[chunk2TagLenOff];
            if(tagLen2 != 16) return false;

            Int32 chunk2End = chunk2TagLenOff + 1 + tagLen2;
            Byte[] first = env.Skip(pos).Take(chunk1End - pos).ToArray();
            Byte[] second = env.Skip(pos2).Take(chunk2End - pos2).ToArray();

            tampered = new Byte[env.Length];
            Buffer.BlockCopy(env, 0, tampered, 0, pos);
            Buffer.BlockCopy(second, 0, tampered, pos, second.Length);
            Buffer.BlockCopy(first, 0, tampered, pos + second.Length, first.Length);
            Buffer.BlockCopy(env, chunk2End, tampered, pos + second.Length + first.Length, env.Length - chunk2End);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifies that encrypting and decrypting an empty array produces a valid envelope and results in an empty output.
    /// </summary>
    [Test]
    public void Array_EncryptDecrypt_Empty()
    {
        Byte[] plain = Array.Empty<Byte>();
        Byte[]? env = plain.Encrypt(CertA!);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a small array, ensuring output matches input.
    /// </summary>
    [Test]
    public void Array_EncryptDecrypt_Small()
    {
        Byte[] plain = Encoding.UTF8.GetBytes("hello world");
        Byte[]? env = plain.Encrypt(CertA!);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a large array, ensuring output matches input.
    /// </summary>
    [Test]
    public void Array_EncryptDecrypt_Large()
    {
        Byte[] plain = RandomBytes(128_000);
        Byte[]? env = plain.Encrypt(CertA!);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that encrypting and decrypting an empty stream produces a valid envelope and results in an empty output.
    /// </summary>
    [Test]
    public void Stream_EncryptDecrypt_Empty()
    {
        using var input = new MemoryStream(Array.Empty<Byte>());
        using var output = new MemoryStream();
        Assert.That(Encrypt(input, output, CertA!), Is.True);
        output.Position = 0;
        using var decrypted = new MemoryStream();
        Assert.That(Decrypt(output, decrypted, CertA!), Is.True);
        Assert.That(decrypted.ToArray().Length, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a small stream, ensuring output matches input.
    /// </summary>
    [Test]
    public void Stream_EncryptDecrypt_Small()
    {
        Byte[] plain = Encoding.UTF8.GetBytes("stream test");
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        Assert.That(Encrypt(input, output, CertA!), Is.True);
        output.Position = 0;
        using var decrypted = new MemoryStream();
        Assert.That(Decrypt(output, decrypted, CertA!), Is.True);
        Assert.That(decrypted.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a large stream, ensuring output matches input.
    /// </summary>
    [Test]
    public void Stream_EncryptDecrypt_Large()
    {
        Byte[] plain = RandomBytes(256_000);
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        Assert.That(Encrypt(input, output, CertA!), Is.True);
        output.Position = 0;
        using var decrypted = new MemoryStream();
        Assert.That(Decrypt(output, decrypted, CertA!), Is.True);
        Assert.That(decrypted.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Tests round-trip encryption and decryption for a variety of chunk sizes and array lengths, ensuring correctness across boundaries.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void ChunkSizeVariations_ArrayRoundTrip()
    {
        using var cert = CreateExamCert("UtilityCryptoExam.CryptoCore");
        Int32[] powers = {12,18,22,26};
        foreach (Int32 p in powers)
        {
            Int32 chunk = 1 << p;
            Int32[] sizes = {0,1,chunk - 1,chunk,chunk + 1,2 * chunk - 1,2 * chunk,2 * chunk + 1};
            foreach(Int32 size in sizes.Distinct().Where(s => s >= 0))
            {
                Byte[] pt = RandomBytes(size);
                Byte[]? env = pt.Encrypt(cert, chunksizepower: p);
                Assert.That(env, Is.Not.Null, $"env null size={size} p={p}");
                Byte[]? dec = env.Decrypt(cert);
                Assert.That(dec, Is.Not.Null, $"dec null size={size} p={p}");
                Assert.That(dec, Is.EquivalentTo(pt), $"mismatch size={size} p={p}");
            }
        }
    }

    /// <summary>
    /// Tests round-trip encryption and decryption for selected chunk sizes using streams, ensuring correctness across chunk boundaries.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void ChunkSize_Stream_SelectedPowers()
    {
        using var cert = CreateExamCert("UtilityCryptoExam.CryptoCore");
        foreach(Int32 p in new[]{12,18,22,26})
        {
            Int32 chunk = 1 << p;
            Byte[] pt = RandomBytes(chunk + chunk / 3);
            using var input = new MemoryStream(pt);
            using var encrypted = new MemoryStream();
            Assert.That(Encrypt(input, encrypted, cert, chunksizepower: p), Is.True, $"encrypt fail p={p}");
            encrypted.Position = 0;
            using var output = new MemoryStream();
            Assert.That(Decrypt(encrypted, output, cert), Is.True, $"decrypt fail p={p}");
            Assert.That(output.ToArray(), Is.EquivalentTo(pt), $"roundtrip mismatch p={p}");
        }
    }

    /// <summary>
    /// Tests AAD binding: decryption only succeeds with the correct AAD when hash binding is enabled, and fails with incorrect AAD.
    /// </summary>
    [Test]
    public void AAD_Bound_Hash_SuccessAndFailure()
    {
        Byte[] pt = RandomBytes(60_000);
        Byte[] aad = Encoding.UTF8.GetBytes("context-binding-1");
        Byte[]? env = pt.Encrypt(CertA!, aad);
        Assert.That(env, Is.Not.Null);
        Assert.That(env.Decrypt(CertA!, aad), Is.EqualTo(pt));
        Byte[] wrong = Encoding.UTF8.GetBytes("context-binding-2");
        Assert.That(env.Decrypt(CertA!, wrong), Is.Null);
    }

    /// <summary>
    /// Tests that when AAD hash binding is disabled, mismatched AAD is ignored and decryption still succeeds.
    /// </summary>
    [Test]
    public void AAD_Unbound_Ignored_MismatchStillSucceeds()
    {
        Byte[] pt = RandomBytes(45_000);
        Byte[] aad = RandomBytes(32);
        Byte[]? env = pt.Encrypt(CertA!, aad, includeaadhash: false);
        Assert.That(env, Is.Not.Null);
        Byte[] different = RandomBytes(32);
        Byte[]? dec = env.Decrypt(CertA!, different);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(pt));
    }

    /// <summary>
    /// Tests AAD binding with large AAD values, ensuring only the correct AAD decrypts when binding is enabled, and mismatched AAD is ignored when binding is disabled.
    /// </summary>
    [Test]
    public void AAD_Large_BoundAndUnbound()
    {
        Byte[] pt = RandomBytes(80_000);
        Byte[] largeAad = RandomBytes(4096);
        Byte[]? bound = pt.Encrypt(CertA!, largeAad);
        Assert.That(bound, Is.Not.Null);
        Assert.That(bound.Decrypt(CertA!, largeAad), Is.EqualTo(pt));
        Assert.That(bound.Decrypt(CertA!, RandomBytes(4096)), Is.Null);
        Byte[]? unbound = pt.Encrypt(CertA!, largeAad, includeaadhash: false);
        Assert.That(unbound, Is.Not.Null);
        Assert.That(unbound.Decrypt(CertA!, RandomBytes(4096)), Is.EqualTo(pt));
    }

    /// <summary>
    /// Tests AAD binding and unbound behavior with empty AAD values, ensuring correct and incorrect AAD handling.
    /// </summary>
    [Test]
    public void AAD_Empty_BoundAndUnbound()
    {
        Byte[] pt = RandomBytes(10_000);
        Byte[] empty = Array.Empty<Byte>();
        Byte[]? envEmptyBound = pt.Encrypt(CertA!, empty);
        Assert.That(envEmptyBound, Is.Not.Null);
        Assert.That(envEmptyBound.Decrypt(CertA!, empty), Is.EqualTo(pt));
        Assert.That(envEmptyBound.Decrypt(CertA!, new Byte[]{0x00}), Is.Null);
        Byte[]? envEmptyUnbound = pt.Encrypt(CertA!, empty, includeaadhash: false);
        Assert.That(envEmptyUnbound, Is.Not.Null);
        Assert.That(envEmptyUnbound.Decrypt(CertA!, new Byte[]{0x00}), Is.EqualTo(pt));
    }

    /// <summary>
    /// Verifies that tampering with a ciphertext byte causes decryption to fail, demonstrating envelope integrity protection.
    /// </summary>
    [Test]
    public void Tamper_CiphertextByte_Fails()
    {
        Byte[] pt = RandomBytes(50_000);
        Byte[]? env = pt.Encrypt(CertA!);
        Assert.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        if(env!.Length > headerLen + 8) env[headerLen + 8] ^= 0xFF;
        AssertDecryptFailsOrThrows<AuthenticationTagMismatchException>(() => env.Decrypt(CertA!));
    }

    /// <summary>
    /// Verifies that tampering with the header version byte causes decryption to fail, demonstrating header integrity protection.
    /// </summary>
    [Test]
    public void Tamper_HeaderByte_Fails()
    {
        Byte[] pt = RandomBytes(20_000);
        Byte[]? env = pt.Encrypt(CertA!);
        Assert.That(env, Is.Not.Null);
        env![0] ^= 0x01;
        Assert.That(env.Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Verifies that tampering with the footer marker causes decryption to fail, demonstrating footer integrity protection.
    /// </summary>
    [Test]
    public void Tamper_FooterMarker_Fails()
    {
        Byte[] pt = RandomBytes(60_000);
        Byte[]? env = pt.Encrypt(CertA!, includeoriginallength: false);
        Assert.That(env, Is.Not.Null);
        if(env!.Length > 5) env[^5] ^= 0x0F;
        Assert.That(env.Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Verifies that decryption returns null when the envelope is null.
    /// </summary>
    [Test]
    public void Decrypt_NullEnvelope_ReturnsNull()
    {
        Assert.That(((Byte[]?)null).Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Verifies that decryption returns null when the certificate is null.
    /// </summary>
    [Test]
    public void Decrypt_NullCert_ReturnsNull()
    {
        using var cert = CreateExamCert("UtilityCryptoExam.CryptoCore");
        Byte[] pt = RandomBytes(100);
        Byte[]? env = pt.Encrypt(cert);
        Assert.That(env.Decrypt(null), Is.Null);
    }

    /// <summary>
    /// Verifies that encryption returns null when the plaintext is null.
    /// </summary>
    [Test]
    public void Encrypt_NullPlaintext_ReturnsNull()
    {
        Byte[]? plain = null;
        Assert.That(plain.Encrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Verifies that encryption returns null when the certificate is null.
    /// </summary>
    [Test]
    public void Encrypt_NullCert_ReturnsNull()
    {
        Byte[] pt = RandomBytes(100);
        Assert.That(pt.Encrypt(null), Is.Null);
    }

    /// <summary>
    /// Verifies that stream encryption returns false when the input stream is null.
    /// </summary>
    [Test]
    public void Stream_Encrypt_NullInput_ReturnsFalse()
    {
        using var output = new MemoryStream();
        Assert.That(Encrypt((Stream?)null, output, CertA!), Is.False);
    }

    /// <summary>
    /// Verifies that stream encryption returns false when the output stream is null.
    /// </summary>
    [Test]
    public void Stream_Encrypt_NullOutput_ReturnsFalse()
    {
        using var input = new MemoryStream(RandomBytes(100));
        Assert.That(Encrypt(input, (Stream?)null, CertA!), Is.False);
    }

    /// <summary>
    /// Verifies that stream encryption returns false when the certificate is null.
    /// </summary>
    [Test]
    public void Stream_Encrypt_NullCert_ReturnsFalse()
    {
        using var input = new MemoryStream(RandomBytes(100));
        using var output = new MemoryStream();
        Assert.That(Encrypt(input, output, null), Is.False);
    }

    /// <summary>
    /// Verifies that stream decryption returns false when the input stream is null.
    /// </summary>
    [Test]
    public void Stream_Decrypt_NullInput_ReturnsFalse()
    {
        using var output = new MemoryStream();
        Assert.That(Decrypt((Stream?)null, output, CertA!), Is.False);
    }

    /// <summary>
    /// Verifies that stream decryption returns false when the output stream is null.
    /// </summary>
    [Test]
    public void Stream_Decrypt_NullOutput_ReturnsFalse()
    {
        using var input = new MemoryStream(RandomBytes(100));
        Assert.That(Decrypt(input, (Stream?)null, CertA!), Is.False);
    }

    /// <summary>
    /// Verifies that stream decryption returns false when the certificate is null.
    /// </summary>
    [Test]
    public void Stream_Decrypt_NullCert_ReturnsFalse()
    {
        using var input = new MemoryStream(RandomBytes(100));
        using var output = new MemoryStream();
        Assert.That(Decrypt(input, output, null), Is.False);
    }

    /// <summary>
    /// Verifies that asynchronous array-based encryption and decryption round-trip the data correctly.
    /// </summary>
    [Test]
    public async Task Async_Array_EncryptDecrypt_RoundTrip()
    {
        Byte[] plain = RandomBytes(120_000);
        Byte[]? env = await plain.EncryptAsync(CertA!);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = await env.DecryptAsync(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that asynchronous stream-based encryption and decryption work as expected for large payloads.
    /// </summary>
    [Test]
    public async Task Async_Stream_EncryptDecrypt_RoundTrip()
    {
        Byte[] plain = RandomBytes(90_000);
        using var input = new MemoryStream(plain);
        using var encrypted = new MemoryStream();
        Assert.That(await EncryptAsync(input, encrypted, CertA!), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(await DecryptAsync(encrypted, output, CertA!), Is.True);
        Assert.That(output.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that async encryption returns false when the operation is cancelled via a cancellation token.
    /// </summary>
    [Test]
    public async Task Async_Encrypt_Cancelled_ReturnsFalse()
    {
        Byte[] plain = RandomBytes(500_000);
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Boolean ok = await EncryptAsync(input, output, CertA!, cancel: cts.Token);
        Assert.That(ok, Is.False);
    }

    /// <summary>
    /// Verifies that a small payload can be encrypted and decrypted in a single chunk using default envelope settings.
    /// Ensures the decrypted output matches the original input, validating basic round-trip integrity.
    /// </summary>
    [Test]
    public void SingleChunk_RoundTrip_Default()
    {
        Byte[] plain = Encoding.UTF8.GetBytes("hello world");
        Byte[]? env = plain.Encrypt(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
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
        Int32 chunkSize = 1 << EnvelopeChunkPower;
        Byte[] plain = RandomBytes(chunkSize * 5 + chunkSize / 3);
        Byte[]? env = plain.Encrypt(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Checks that encrypting and decrypting an empty payload produces a valid envelope and results in an empty output.
    /// </summary>
    [Test]
    public void ZeroLength_RoundTrip()
    {
        Byte[] plain = Array.Empty<Byte>();
        Byte[]? env = plain.Encrypt(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests envelope operation in footer mode (without original length in the header), verifying round-trip integrity for a large payload.
    /// </summary>
    [Test]
    public void FooterMode_RoundTrip()
    {
        Byte[] plain = RandomBytes(70000);
        Byte[]? env = plain.Encrypt(CertA!, includeoriginallength: false, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Ensures that truncating the envelope in footer mode is detected and decryption fails, demonstrating integrity protection.
    /// </summary>
    [Test]
    public void FooterMode_TruncationDetected()
    {
        Byte[] plain = RandomBytes(50000);
        Byte[]? env = plain.Encrypt(CertA!, includeoriginallength: false, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        Byte[] truncated = env!.Take(env!.Length - 1).ToArray();
        Byte[]? dec = truncated.Decrypt(CertA!);
        Assert.That(dec, Is.Null);
    }

    /// <summary>
    /// Verifies that truncation is detected when the original length is included in the header, and decryption fails as expected.
    /// </summary>
    [Test]
    public void OriginalLength_TruncationDetected()
    {
        Byte[] plain = RandomBytes(80000);
        Byte[]? env = plain.Encrypt(CertA!, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        Byte[] truncated = env!.Take(env!.Length - 10).ToArray();
        Byte[]? dec = truncated.Decrypt(CertA!);
        Assert.That(dec, Is.Null);
    }

    /// <summary>
    /// Tests AAD hash binding: decryption succeeds only with the correct AAD and fails with incorrect AAD when binding is enabled.
    /// </summary>
    [Test]
    public void AADHash_Binding_SuccessAndFailure()
    {
        Byte[] plain = RandomBytes(30000);
        Byte[] aad = Encoding.UTF8.GetBytes("user-metadata-context");
        Byte[]? env = plain.Encrypt(CertA!, aad, includeaadhash: true, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec1 = env.Decrypt(CertA!, aad);
        Assert.That(dec1, Is.Not.Null);
        Assert.That(dec1, Is.EquivalentTo(plain));
        Byte[] wrong = Encoding.UTF8.GetBytes("wrong-context");
        Byte[]? dec2 = env.Decrypt(CertA!, wrong);
        Assert.That(dec2, Is.Null);
        Byte[]? envDefault = plain.Encrypt(CertA!, aad, includeaadhash: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(envDefault, Is.Not.Null);
        Byte[]? decDefaultGood = envDefault.Decrypt(CertA!, aad);
        Assert.That(decDefaultGood, Is.Not.Null);
        Assert.That(decDefaultGood, Is.EquivalentTo(plain));
        Byte[]? decDefaultBad = envDefault.Decrypt(CertA!, wrong);
        Assert.That(decDefaultBad, Is.Null);
    }

    /// <summary>
    /// Checks that when AAD hash binding is disabled, mismatched AAD is ignored and decryption still succeeds.
    /// </summary>
    [Test]
    public void AAD_NotBound_Ignored()
    {
        Byte[] plain = RandomBytes(25000);
        Byte[] aad1 = Encoding.UTF8.GetBytes("a1");
        Byte[]? env = plain.Encrypt(CertA!, aad1, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[] aad2 = Encoding.UTF8.GetBytes("different-aad");
        Byte[]? dec = env.Decrypt(CertA!, aad2);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that modifying a byte in the chunk ciphertext causes decryption to fail, confirming chunk integrity.
    /// </summary>
    [Test]
    public void Tamper_ChunkCipherByte_Fails()
    {
        Byte[] plain = RandomBytes(50000);
        Byte[]? env = plain.Encrypt(CertA!, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        UInt32 pLen = ReadUInt32(env!, headerLen);
        Int32 firstCipherOffset = headerLen + 8;
        if(pLen > 0)
        {
            env![firstCipherOffset] ^= 0xFF;
        }
        AssertDecryptFailsOrThrows<AuthenticationTagMismatchException>(() => env.Decrypt(CertA!));
    }

    /// <summary>
    /// Ensures that reordering encrypted chunks results in decryption failure, protecting against chunk reordering attacks.
    /// </summary>
    [Test]
    public void Tamper_Reorder_Chunks_Fails()
    {
        Byte[] plain = RandomBytes((1 << EnvelopeChunkPower) * 3 + 1234);
        Byte[]? env = plain.Encrypt(CertA!, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        if(!TryReorderFirstTwoChunks(env!, out Byte[] tampered)) Assert.Inconclusive("Could not reorder (unexpected layout)");
        AssertDecryptFailsOrThrows<AuthenticationTagMismatchException>(() => tampered.Decrypt(CertA!));
    }

    /// <summary>
    /// Verifies that asynchronous buffer-based encryption and decryption round-trip the data correctly.
    /// </summary>
    [Test]
    public async Task Async_RoundTrip_Buffer()
    {
        Byte[] plain = RandomBytes(120000);
        Byte[]? env = await plain.EncryptAsync(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = await env.DecryptAsync(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Ensures that async stream-based encryption and decryption work as expected for large payloads.
    /// </summary>
    [Test]
    public async Task Async_Stream_RoundTrip()
    {
        Byte[] plain = RandomBytes(90000);
        using var input = new MemoryStream(plain);
        using var encrypted = new MemoryStream();
        Assert.That(await EncryptAsync(input, encrypted, CertA!, chunksizepower: EnvelopeChunkPower), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(await DecryptAsync(encrypted, output, CertA!), Is.True);
        Assert.That(output.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Checks that encryption is properly cancelled when a cancellation token is triggered.
    /// </summary>
    [Test]
    public async Task Async_Cancelled_Encrypt()
    {
        Byte[] plain = RandomBytes(500000);
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Boolean ok = await EncryptAsync(input, output, CertA!, chunksizepower: EnvelopeChunkPower, cancel: cts.Token);
        Assert.That(ok, Is.False);
    }

    /// <summary>
    /// Verifies that UTF-16 text can be encrypted to a buffer and decrypted back to the original string.
    /// </summary>
    [Test]
    public void Utf16_Buffer_RoundTrip()
    {
        String plain = new('x', 50_000);
        Byte[]? env = plain.EncryptUtf16(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.ToUTF16StringFromByteArray(), Is.EqualTo(plain));
    }

    /// <summary>
    /// Verifies that UTF-16 text can be encrypted directly to a stream and decrypted back to the original string.
    /// </summary>
    [Test]
    public void Utf16_Stream_RoundTrip()
    {
        String plain = new('y', 40_000);
        using var encrypted = new MemoryStream();
        Assert.That(EncryptUtf16(plain, encrypted, CertA!, chunksizepower: EnvelopeChunkPower), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(Decrypt(encrypted, output, CertA!), Is.True);
        Assert.That(output.ToArray().ToUTF16StringFromByteArray(), Is.EqualTo(plain));
    }

    /// <summary>
    /// Verifies that asynchronous UTF-16 buffer encryption round-trips the original string.
    /// </summary>
    [Test]
    public async Task Async_Utf16_Buffer_RoundTrip()
    {
        String plain = new('z', 60_000);
        Byte[]? env = await plain.EncryptUtf16Async(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = await env.DecryptAsync(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.ToUTF16StringFromByteArray(), Is.EqualTo(plain));
    }

    /// <summary>
    /// Verifies that asynchronous UTF-16 stream encryption round-trips the original string.
    /// </summary>
    [Test]
    public async Task Async_Utf16_Stream_RoundTrip()
    {
        String plain = new('w', 45_000);
        using var encrypted = new MemoryStream();
        Assert.That(await EncryptUtf16Async(plain, encrypted, CertA!, chunksizepower: EnvelopeChunkPower), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(await DecryptAsync(encrypted, output, CertA!), Is.True);
        Assert.That(output.ToArray().ToUTF16StringFromByteArray(), Is.EqualTo(plain));
    }

    [Test]
    public void Span_RoundTrip()
    {
        ReadOnlySpan<Byte> plain = Encoding.UTF8.GetBytes("hello world span");
        Span<Byte> env = stackalloc Byte[1024];
        Assert.That(Encrypt(plain, env, CertA!, out Int32 written), Is.True);
        Assert.That(written, Is.GreaterThan(0));

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(Decrypt(env[..written], dec, CertA!, out Int32 decwritten), Is.True);
        Assert.That(decwritten, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);
    }

    [Test]
    public void Span_Interop_EncryptSpan_DecryptArray()
    {
        ReadOnlySpan<Byte> plain = Encoding.UTF8.GetBytes("interop test");
        Span<Byte> envSpan = stackalloc Byte[1024];
        Assert.That(Encrypt(plain, envSpan, CertA!, out Int32 written), Is.True);

        Byte[]? dec = envSpan[..written].ToArray().Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain.ToArray()));
    }

    [Test]
    public void Span_Interop_EncryptArray_DecryptSpan()
    {
        Byte[] plain = Encoding.UTF8.GetBytes("interop test 2");
        Byte[]? env = plain.Encrypt(CertA!, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(Decrypt(env, dec, CertA!, out Int32 written), Is.True);
        Assert.That(written, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);
    }

    [Test]
    public void Span_ZeroLength_RoundTrip()
    {
        ReadOnlySpan<Byte> plain = ReadOnlySpan<Byte>.Empty;
        Span<Byte> env = stackalloc Byte[1024];
        Assert.That(Encrypt(plain, env, CertA!, out Int32 written), Is.True);
        Assert.That(written, Is.GreaterThan(0));

        Span<Byte> dec = stackalloc Byte[1];
        Assert.That(Decrypt(env[..written], dec, CertA!, out Int32 decwritten), Is.True);
        Assert.That(decwritten, Is.EqualTo(0));
    }

    [Test]
    public void Span_AADHash_Binding()
    {
        ReadOnlySpan<Byte> plain = RandomBytes(100);
        ReadOnlySpan<Byte> aad = Encoding.UTF8.GetBytes("span aad");
        Span<Byte> env = stackalloc Byte[1024];

        Assert.That(Encrypt(plain, env, CertA!, out Int32 written, aad, includeaadhash: true), Is.True);

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(Decrypt(env[..written], dec, CertA!, out Int32 decwritten, aad), Is.True);
        Assert.That(decwritten, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);

        ReadOnlySpan<Byte> wrongAad = Encoding.UTF8.GetBytes("wrong");
        Assert.That(Decrypt(env[..written], dec, CertA!, out _, wrongAad), Is.False);
    }

    [Test]
    public void Span_OutputTooSmall_Fails()
    {
        ReadOnlySpan<Byte> plain = RandomBytes(100);
        Span<Byte> env = stackalloc Byte[10];
        Assert.That(Encrypt(plain, env, CertA!, out _), Is.False);

        Byte[]? validEnv = plain.ToArray().Encrypt(CertA!);
        Assume.That(validEnv, Is.Not.Null);
        Span<Byte> dec = stackalloc Byte[plain.Length - 1];
        Assert.That(Decrypt(validEnv, dec, CertA!, out _), Is.False);
    }

    [Test]
    public void Span_Tamper_Fails()
    {
        ReadOnlySpan<Byte> plain = RandomBytes(100);
        Span<Byte> env = stackalloc Byte[1024];
        Assert.That(Encrypt(plain, env, CertA!, out Int32 written), Is.True);

        env[written - 20] ^= 0xFF;

        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(Decrypt(env[..written], dec, CertA!, out _), Is.False);
    }

    [Test]
    public void Span_Interop_EncryptSpan_DecryptStream()
    {
        ReadOnlySpan<Byte> plain = RandomBytes(200);
        Span<Byte> envSpan = stackalloc Byte[1024];
        Assert.That(Encrypt(plain, envSpan, CertA!, out Int32 written), Is.True);

        using var inputStream = new MemoryStream(envSpan[..written].ToArray());
        using var outputStream = new MemoryStream();
        Assert.That(Decrypt(inputStream, outputStream, CertA!), Is.True);
        Assert.That(outputStream.ToArray(), Is.EquivalentTo(plain.ToArray()));
    }

    [Test]
    public void Span_Interop_EncryptStream_DecryptSpan()
    {
        Byte[] plain = RandomBytes(250);
        using var inputStream = new MemoryStream(plain);
        using var encryptedStream = new MemoryStream();
        Assert.That(Encrypt(inputStream, encryptedStream, CertA!, chunksizepower: EnvelopeChunkPower), Is.True);

        ReadOnlySpan<Byte> env = encryptedStream.ToArray();
        Span<Byte> dec = stackalloc Byte[plain.Length];
        Assert.That(Decrypt(env, dec, CertA!, out Int32 written), Is.True);
        Assert.That(written, Is.EqualTo(plain.Length));
        Assert.That(dec.SequenceEqual(plain), Is.True);
    }

    private static IEnumerable<TestCaseData> TinyPayloadsSource()
    {
        Int32[] sizes = {0, 1, 2, 3, 4, 5, 7, 8, 9, 10};
        Boolean[] bools = {true, false};
        foreach (Int32 size in sizes)
        foreach (Boolean includeOrig in bools)
        foreach (Boolean includeAadHash in bools)
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
        Byte[] pt = RandomBytes(size);
        Byte[] aad = RandomBytes(5);
        Byte[]? env = pt.Encrypt(CertA!, aad, includeAadHash, includeOrig, EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null, $"env null size={size} orig={includeOrig} hash={includeAadHash}");
        Byte[] aadDecrypt = includeAadHash ? aad : RandomBytes(5);
        Byte[]? dec = env.Decrypt(CertA!, aadDecrypt);
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
        Byte[] pt = RandomBytes(50000);
        Byte[] largeAad = RandomBytes(2048);
        Byte[]? envBound = pt.Encrypt(CertA!, largeAad, includeaadhash: true, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(envBound, Is.Not.Null);
        Byte[]? decOk = envBound.Decrypt(CertA!, largeAad);
        Assert.That(decOk, Is.Not.Null);
        Assert.That(decOk, Is.EquivalentTo(pt));
        Byte[] wrong = RandomBytes(2048);
        Byte[]? decFail = envBound.Decrypt(CertA!, wrong);
        Assert.That(decFail, Is.Null, "Different AAD must fail when hashed");
        Byte[]? envUnbound = pt.Encrypt(CertA!, largeAad, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(envUnbound, Is.Not.Null);
        Byte[] other = RandomBytes(2048);
        Byte[]? decIgnore = envUnbound.Decrypt(CertA!, other);
        Assert.That(decIgnore, Is.Not.Null, "Unbound large AAD mismatch should succeed");
        Assert.That(decIgnore, Is.EquivalentTo(pt));
    }

    /// <summary>
    /// Ensures that corruption of the footer marker or chunk count is detected and decryption fails.
    /// </summary>
    [Test]
    public void FooterCorruption_MarkerAndCount()
    {
        Byte[] pt = RandomBytes(60000);
        Byte[]? env = pt.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: false, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);

        Byte[] corruptMarker = (Byte[])env!.Clone();
        if(corruptMarker.Length < 5) Assert.Inconclusive("Envelope too small");
        corruptMarker[^5] ^= 0x0F;
        Assert.That(corruptMarker.Decrypt(CertA!), Is.Null, "Corrupt footer marker should fail");

        Byte[] corruptCount = (Byte[])env.Clone();
        UInt32 origCount = (UInt32)((corruptCount[^4] << 24) | (corruptCount[^3] << 16) | (corruptCount[^2] << 8) | corruptCount[^1]);
        UInt32 newCount = origCount + 1;
        corruptCount[^4] = (Byte)((newCount >> 24) & 0xFF);
        corruptCount[^3] = (Byte)((newCount >> 16) & 0xFF);
        corruptCount[^2] = (Byte)((newCount >> 8) & 0xFF);
        corruptCount[^1] = (Byte)(newCount & 0xFF);
        Assert.That(corruptCount.Decrypt(CertA!), Is.Null, "Footer chunk count mismatch should fail");
    }

    /// <summary>
    /// Performs randomized round-trip tests with varying payload sizes, chunk sizes, and envelope features.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void RandomRoundTrips()
    {
        Random rnd = new();
        const Int32 iterations = 20;
        for(Int32 i = 0; i < iterations; i++)
        {
            Int32 size = rnd.Next(0, 2 * 1024 * 1024);
            Byte[] pt = RandomBytes(size);
            Int32 chunkPow = rnd.Next(12, 26);
            Boolean includeOrig = rnd.Next(0, 2) == 1;
            Boolean includeAadHash = rnd.Next(0, 2) == 1;
            Int32 aadLen = rnd.Next(0, 1024);
            Byte[] aad = RandomBytes(aadLen);
            Byte[]? env = pt.Encrypt(CertA!, aad, includeAadHash, includeOrig, chunkPow);
            Assert.That(env, Is.Not.Null, $"Fuzz encrypt failed iter {i}");
            Byte[] aadDecrypt = includeAadHash ? aad : RandomBytes(aadLen);
            Byte[]? dec = env.Decrypt(CertA!, aadDecrypt);
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
        Byte[] plain = RandomBytes(50000);
        Byte[]? env = plain.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        if(env!.Length < headerLen + 8) Assert.Inconclusive();
        UInt32 pLen = ReadUInt32(env, headerLen);
        UInt32 cLen = ReadUInt32(env, headerLen + 4);
        if(pLen != cLen) Assert.Inconclusive();
        Int32 tagLenOffset = headerLen + 8 + (Int32)cLen;
        if(env.Length < tagLenOffset + 1) Assert.Inconclusive();
        env[tagLenOffset + 1] ^= 0xAA;
        AssertDecryptFailsOrThrows<AuthenticationTagMismatchException>(() => env.Decrypt(CertA!));
    }

    /// <summary>
    /// Tests that truncating the envelope at a tag boundary is detected and decryption fails.
    /// </summary>
    [Test]
    public void Tamper_TruncateAtTagBoundary_Fails()
    {
        Byte[] plain = RandomBytes(80000);
        Byte[]? env = plain.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        UInt32 pLen = ReadUInt32(env!, headerLen);
        UInt32 cLen = ReadUInt32(env!, headerLen + 4);
        if(pLen != cLen) Assert.Inconclusive();
        Int32 tagLenOffset = headerLen + 8 + (Int32)cLen;
        if(env!.Length < tagLenOffset + 1 + 16) Assert.Inconclusive();
        Byte[] truncated = env.Take(tagLenOffset + 1).ToArray();
        Assert.That(truncated.Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Ensures that a mismatch between plaintext and ciphertext length fields in a chunk is detected and decryption fails.
    /// </summary>
    [Test]
    public void Tamper_LengthFieldMismatch_Fails()
    {
        Byte[] plain = RandomBytes(40000);
        Byte[]? env = plain.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assume.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        if(env!.Length < headerLen + 8) Assert.Inconclusive();
        UInt32 pLen = ReadUInt32(env, headerLen);
        if(pLen == UInt32.MaxValue) Assert.Inconclusive();
        UInt32 spoofCipherLen = pLen + 1;
        env[headerLen + 4] = (Byte)((spoofCipherLen >> 24) & 0xFF);
        env[headerLen + 5] = (Byte)((spoofCipherLen >> 16) & 0xFF);
        env[headerLen + 6] = (Byte)((spoofCipherLen >> 8) & 0xFF);
        env[headerLen + 7] = (Byte)(spoofCipherLen & 0xFF);
        Assert.That(env.Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Confirms that envelopes produced with different AAD (when hash binding is enabled) are distinct and only decrypt with their respective AAD.
    /// </summary>
    [Test]
    public void AAD_Binding_EnvelopesDiffer_ForDifferentAAD()
    {
        Byte[] pt = RandomBytes(60000);
        Byte[] aad1 = Encoding.UTF8.GetBytes("aad-one");
        Byte[] aad2 = Encoding.UTF8.GetBytes("aad-two");
        Byte[]? env1 = pt.Encrypt(CertA!, aad1, includeaadhash: true, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Byte[]? env2 = pt.Encrypt(CertA!, aad2, includeaadhash: true, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(env1, Is.Not.Null);
        Assert.That(env2, Is.Not.Null);
        Assert.That(env1!.SequenceEqual(env2!), Is.False, "Different AAD should produce distinct envelopes when hashed");
        Assert.That(env1.Decrypt(CertA!, aad1), Is.EqualTo(pt));
        Assert.That(env2.Decrypt(CertA!, aad2), Is.EqualTo(pt));
    }

    /// <summary>
    /// Explicit test for very large payloads, verifying stream-based round-trip correctness and spot-checking output.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void LargePayload_StreamRoundTrip()
    {
        Int32 size = 200 * 1024 * 1024;
        Byte[] pt = RandomBytes(size);
        using var input = new MemoryStream(pt);
        using var encrypted = new MemoryStream();
        Assert.That(Encrypt(input, encrypted, CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: 20), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(Decrypt(encrypted, output, CertA!), Is.True);
        Assert.That(output.ToArray().Length, Is.EqualTo(size));
        Byte[] dec = output.GetBuffer().AsSpan(0, size).ToArray();
        Assert.That(dec.AsSpan(0, 64).ToArray(), Is.EqualTo(pt.AsSpan(0, 64).ToArray()));
        Assert.That(dec.AsSpan(size / 2, 64).ToArray(), Is.EqualTo(pt.AsSpan(size / 2, 64).ToArray()));
        Assert.That(dec.AsSpan(size - 64, 64).ToArray(), Is.EqualTo(pt.AsSpan(size - 64, 64).ToArray()));
    }

    /// <summary>
    /// Checks that an empty payload is correctly handled in footer mode.
    /// </summary>
    [Test]
    public void ZeroLength_FooterMode_RoundTrip()
    {
        Byte[] plain = Array.Empty<Byte>();
        Byte[]? env = plain.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: false, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
    }

    private static IEnumerable<TestCaseData> ChunkSizeVariabilitySource()
    {
        Int32[] powers = {12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26};
        Int32[] multipliers = {1, 2, 3};
        foreach (Int32 p in powers)
        {
            Int32 chunk = 1 << p;
            foreach (Int32 mult in multipliers)
            {
                yield return new TestCaseData(p, chunk * mult).SetCategory("Extended");
                if (chunk * mult > 0)
                {
                    yield return new TestCaseData(p, chunk * mult - 1).SetCategory("Extended");
                }
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
        Byte[] pt = RandomBytes(size);
        Byte[]? env = pt.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: power);
        Assert.That(env, Is.Not.Null, $"Encryption failed for chunk power 2^{power}, size {size}");
        Byte[]? dec = env.Decrypt(CertA!);
        Assert.That(dec, Is.EqualTo(pt), $"Decryption failed for chunk power 2^{power}, size {size}");
    }

    /// <summary>
    /// Ensures that decryption fails with a different RSA private key than was used for encryption, and succeeds with the correct key.
    /// </summary>
    [Test]
    public void RSA_DifferentPrivateKey_Fails()
    {
        using var certEnc = CreateExamCert("UtilityCryptoExam.GeneralDataEnvelope", 2048);
        using var certWrong = CreateExamCert("UtilityCryptoExam.GeneralDataEnvelope", 2048);
        Byte[] pt = RandomBytes(32000);
        Byte[]? env = pt.Encrypt(certEnc, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        AssertDecryptFailsOrThrows<CryptographicException>(() => env.Decrypt(certWrong));
        Assert.That(env.Decrypt(certEnc), Is.EqualTo(pt));
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
        using var cert = CreateExamCert("UtilityCryptoExam.GeneralDataEnvelope", keySize);
        Byte[] pt = RandomBytes(50000);
        Byte[]? env = pt.Encrypt(cert, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null, $"Encrypt failed for keySize {keySize}");
        Assert.That(env.Decrypt(cert), Is.EqualTo(pt), $"Decrypt failed for keySize {keySize}");
    }

    /// <summary>
    /// Checks that tampering with the version byte in the header is detected and decryption fails.
    /// </summary>
    [Test]
    public void HeaderCorrupt_VersionByte_Fails()
    {
        Byte[] pt = RandomBytes(16000);
        Byte[]? env = pt.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        env![0] ^= 0x01;
        Assert.That(env.Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Ensures that setting reserved flag bits in the header causes decryption to fail.
    /// </summary>
    [Test]
    public void HeaderCorrupt_ReservedFlagBit_Fails()
    {
        Byte[] pt = RandomBytes(8000);
        Byte[]? env = pt.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: EnvelopeChunkPower);
        Assert.That(env, Is.Not.Null);
        env![1] |= 0x04;
        Assert.That(env.Decrypt(CertA!), Is.Null);
    }

    /// <summary>
    /// Verifies that out-of-range chunk size power values in the header are detected and decryption fails.
    /// </summary>
    [Test]
    public void HeaderCorrupt_ChunkSizePower_OutOfRange_Fails()
    {
        Byte[] pt = RandomBytes(10000);
        Byte[]? env = pt.Encrypt(CertA!, aad: null, includeaadhash: false, includeoriginallength: true, chunksizepower: 16);
        Assert.That(env, Is.Not.Null);
        UInt16 rsaLen = (UInt16)((env![2] << 8) | env[3]);
        Int32 index = 4 + rsaLen;
        if(index >= env.Length) Assert.Inconclusive();
        env[index] = 11;
        Assert.That(env.Decrypt(CertA!), Is.Null);
        env[index] = 27;
        Assert.That(env.Decrypt(CertA!), Is.Null);
        env[index] = 20;
        Assert.That(env.Decrypt(CertA!), Is.Not.Null);
    }
}
