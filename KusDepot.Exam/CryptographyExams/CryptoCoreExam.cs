namespace KusDepot.Exams.Cryptography;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CryptoCoreExam
{
    private static X509Certificate2 CreateEphemeralCert()
    {
        return CreateCertificate(Guid.NewGuid(), "CryptoCoreExam")!;
    }

    private static Byte[] RandomBytes(Int32 len)
    { var b = new Byte[len]; RandomNumberGenerator.Fill(b); return b; }

    /// <summary>
    /// Verifies that encrypting and decrypting an empty array produces a valid envelope and results in an empty output.
    /// </summary>
    [Test]
    public void Array_EncryptDecrypt_Empty()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Array.Empty<Byte>();
        Byte[]? env = CryptoCore.Encrypt(plain, cert);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = CryptoCore.Decrypt(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a small array, ensuring output matches input.
    /// </summary>
    [Test]
    public void Array_EncryptDecrypt_Small()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Encoding.UTF8.GetBytes("hello world");
        Byte[]? env = CryptoCore.Encrypt(plain, cert);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = CryptoCore.Decrypt(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a large array, ensuring output matches input.
    /// </summary>
    [Test]
    public void Array_EncryptDecrypt_Large()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(128_000);
        Byte[]? env = CryptoCore.Encrypt(plain, cert);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = CryptoCore.Decrypt(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that encrypting and decrypting an empty stream produces a valid envelope and results in an empty output.
    /// </summary>
    [Test]
    public void Stream_EncryptDecrypt_Empty()
    {
        using var cert = CreateEphemeralCert();
        using var input = new MemoryStream(Array.Empty<Byte>());
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Encrypt(input, output, cert), Is.True);
        output.Position = 0;
        using var decrypted = new MemoryStream();
        Assert.That(CryptoCore.Decrypt(output, decrypted, cert), Is.True);
        Assert.That(decrypted.ToArray().Length, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a small stream, ensuring output matches input.
    /// </summary>
    [Test]
    public void Stream_EncryptDecrypt_Small()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = Encoding.UTF8.GetBytes("stream test");
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Encrypt(input, output, cert), Is.True);
        output.Position = 0;
        using var decrypted = new MemoryStream();
        Assert.That(CryptoCore.Decrypt(output, decrypted, cert), Is.True);
        Assert.That(decrypted.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies round-trip encryption and decryption for a large stream, ensuring output matches input.
    /// </summary>
    [Test]
    public void Stream_EncryptDecrypt_Large()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(256_000);
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Encrypt(input, output, cert), Is.True);
        output.Position = 0;
        using var decrypted = new MemoryStream();
        Assert.That(CryptoCore.Decrypt(output, decrypted, cert), Is.True);
        Assert.That(decrypted.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Tests round-trip encryption and decryption for a variety of chunk sizes and array lengths, ensuring correctness across boundaries.
    /// </summary>
    [Test, Explicit, Category("Extended")]
    public void ChunkSizeVariations_ArrayRoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Int32[] powers = {12,18,22,26};
        foreach (Int32 p in powers)
        {
            Int32 chunk = 1 << p;
            Int32[] sizes = {0,1,chunk-1,chunk,chunk+1, 2*chunk-1, 2*chunk, 2*chunk+1};
            foreach(Int32 size in sizes.Distinct().Where(s=>s>=0))
            {
                Byte[] pt = RandomBytes(size);
                Byte[]? env = CryptoCore.Encrypt(pt, cert, chunksizepower: p);
                Assert.That(env, Is.Not.Null, $"env null size={size} p={p}");
                Byte[]? dec = CryptoCore.Decrypt(env, cert);
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
        using var cert = CreateEphemeralCert();
        foreach(Int32 p in new[]{12,18,22,26})
        {
            Int32 chunk = 1 << p;
            Byte[] pt = RandomBytes(chunk + chunk/3);
            using var input = new MemoryStream(pt);
            using var encrypted = new MemoryStream();
            Assert.That(CryptoCore.Encrypt(input, encrypted, cert, chunksizepower: p), Is.True, $"encrypt fail p={p}");
            encrypted.Position = 0;
            using var output = new MemoryStream();
            Assert.That(CryptoCore.Decrypt(encrypted, output, cert), Is.True, $"decrypt fail p={p}");
            Assert.That(output.ToArray(), Is.EquivalentTo(pt), $"roundtrip mismatch p={p}");
        }
    }

    /// <summary>
    /// Tests AAD binding: decryption only succeeds with the correct AAD when hash binding is enabled, and fails with incorrect AAD.
    /// </summary>
    [Test]
    public void AAD_Bound_Hash_SuccessAndFailure()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(60_000);
        Byte[] aad = Encoding.UTF8.GetBytes("context-binding-1");
        Byte[]? env = CryptoCore.Encrypt(pt, cert, aad);
        Assert.That(env, Is.Not.Null);
        Assert.That(CryptoCore.Decrypt(env, cert, aad), Is.EqualTo(pt));
        Byte[] wrong = Encoding.UTF8.GetBytes("context-binding-2");
        Assert.That(CryptoCore.Decrypt(env, cert, wrong), Is.Null);
    }

    /// <summary>
    /// Tests that when AAD hash binding is disabled, mismatched AAD is ignored and decryption still succeeds.
    /// </summary>
    [Test]
    public void AAD_Unbound_Ignored_MismatchStillSucceeds()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(45_000);
        Byte[] aad = RandomBytes(32);
        Byte[]? env = CryptoCore.Encrypt(pt, cert, aad, includeaadhash: false);
        Assert.That(env, Is.Not.Null);
        Byte[] different = RandomBytes(32);
        Byte[]? dec = CryptoCore.Decrypt(env, cert, different);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(pt));
    }

    /// <summary>
    /// Tests AAD binding with large AAD values, ensuring only the correct AAD decrypts when binding is enabled, and mismatched AAD is ignored when binding is disabled.
    /// </summary>
    [Test]
    public void AAD_Large_BoundAndUnbound()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(80_000);
        Byte[] largeAad = RandomBytes(4096);
        Byte[]? bound = CryptoCore.Encrypt(pt, cert, largeAad);
        Assert.That(bound, Is.Not.Null);
        Assert.That(CryptoCore.Decrypt(bound, cert, largeAad), Is.EqualTo(pt));
        Assert.That(CryptoCore.Decrypt(bound, cert, RandomBytes(4096)), Is.Null);
        Byte[]? unbound = CryptoCore.Encrypt(pt, cert, largeAad, includeaadhash: false);
        Assert.That(unbound, Is.Not.Null);
        Assert.That(CryptoCore.Decrypt(unbound, cert, RandomBytes(4096)), Is.EqualTo(pt));
    }

    /// <summary>
    /// Tests AAD binding and unbound behavior with empty AAD values, ensuring correct and incorrect AAD handling.
    /// </summary>
    [Test]
    public void AAD_Empty_BoundAndUnbound()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(10_000);
        Byte[] empty = Array.Empty<Byte>();
        Byte[]? envEmptyBound = CryptoCore.Encrypt(pt, cert, empty);
        Assert.That(envEmptyBound, Is.Not.Null);
        Assert.That(CryptoCore.Decrypt(envEmptyBound, cert, empty), Is.EqualTo(pt));
        Assert.That(CryptoCore.Decrypt(envEmptyBound, cert, new Byte[]{0x00}), Is.Null);
        Byte[]? envEmptyUnbound = CryptoCore.Encrypt(pt, cert, empty, includeaadhash: false);
        Assert.That(envEmptyUnbound, Is.Not.Null);
        Assert.That(CryptoCore.Decrypt(envEmptyUnbound, cert, new Byte[]{0x00}), Is.EqualTo(pt));
    }

    /// <summary>
    /// Verifies that tampering with a ciphertext byte causes decryption to fail, demonstrating envelope integrity protection.
    /// </summary>
    [Test]
    public void Tamper_CiphertextByte_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(50_000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert);
        Assert.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        if(env!.Length > headerLen + 8) env[headerLen + 8] ^= 0xFF;
        Byte[]? dec = null;
        Boolean caught = false;
        try
        {
            dec = CryptoCore.Decrypt(env, cert);
        }
        catch (Exception ex)
        {
            caught = ex is AuthenticationTagMismatchException;
            if (!caught) throw;
        }
        Assert.That(caught || dec == null);
    }

    /// <summary>
    /// Verifies that tampering with a tag byte causes decryption to fail, demonstrating tag integrity protection.
    /// </summary>
    [Test]
    public void Tamper_TagByte_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(50_000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert);
        Assert.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        UInt32 pLen = ReadUInt32(env!, headerLen);
        Int32 tagOffset = headerLen + 8 + (Int32)pLen + 1;
        if(env!.Length > tagOffset) env[tagOffset] ^= 0xAA;
        Byte[]? dec = null;
        Boolean caught = false;
        try
        {
            dec = CryptoCore.Decrypt(env, cert);
        }
        catch (Exception ex)
        {
            caught = ex is AuthenticationTagMismatchException;
            if (!caught) throw;
        }
        Assert.That(caught || dec == null);
    }

    /// <summary>
    /// Verifies that a mismatch between plaintext and ciphertext length fields in a chunk is detected and decryption fails.
    /// </summary>
    [Test]
    public void Tamper_LengthFieldMismatch_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(40_000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert);
        Assert.That(env, Is.Not.Null);
        Int32 headerLen = GetHeaderLength(env!);
        UInt32 pLen = ReadUInt32(env!, headerLen);
        UInt32 wrongLen = pLen + 1;
        env![headerLen+4] = (Byte)((wrongLen >> 24) & 0xFF);
        env![headerLen+5] = (Byte)((wrongLen >> 16) & 0xFF);
        env![headerLen+6] = (Byte)((wrongLen >> 8) & 0xFF);
        env![headerLen+7] = (Byte)(wrongLen & 0xFF);
        Assert.That(CryptoCore.Decrypt(env, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that tampering with the header version byte causes decryption to fail, demonstrating header integrity protection.
    /// </summary>
    [Test]
    public void Tamper_HeaderByte_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(20_000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert);
        Assert.That(env, Is.Not.Null);
        env![0] ^= 0x01;
        Assert.That(CryptoCore.Decrypt(env, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that tampering with the footer marker causes decryption to fail, demonstrating footer integrity protection.
    /// </summary>
    [Test]
    public void Tamper_FooterMarker_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(60_000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert, includeoriginallength: false);
        Assert.That(env, Is.Not.Null);
        if(env!.Length > 5) env[^5] ^= 0x0F;
        Assert.That(CryptoCore.Decrypt(env, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that decryption returns null when the envelope is null.
    /// </summary>
    [Test]
    public void Decrypt_NullEnvelope_ReturnsNull()
    {
        using var cert = CreateEphemeralCert();
        Assert.That(CryptoCore.Decrypt(null, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that decryption returns null when the certificate is null.
    /// </summary>
    [Test]
    public void Decrypt_NullCert_ReturnsNull()
    {
        Byte[] pt = RandomBytes(100);
        Byte[]? env = CryptoCore.Encrypt(pt, CreateEphemeralCert());
        Assert.That(CryptoCore.Decrypt(env, null), Is.Null);
    }

    /// <summary>
    /// Verifies that encryption returns null when the plaintext is null.
    /// </summary>
    [Test]
    public void Encrypt_NullPlaintext_ReturnsNull()
    {
        using var cert = CreateEphemeralCert();
        Assert.That(CryptoCore.Encrypt(null, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that encryption returns null when the certificate is null.
    /// </summary>
    [Test]
    public void Encrypt_NullCert_ReturnsNull()
    {
        Byte[] pt = RandomBytes(100);
        Assert.That(CryptoCore.Encrypt(pt, null), Is.Null);
    }

    /// <summary>
    /// Verifies that stream encryption returns false when the input stream is null.
    /// </summary>
    [Test]
    public void Stream_Encrypt_NullInput_ReturnsFalse()
    {
        using var cert = CreateEphemeralCert();
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Encrypt(null, output, cert), Is.False);
    }

    /// <summary>
    /// Verifies that stream encryption returns false when the output stream is null.
    /// </summary>
    [Test]
    public void Stream_Encrypt_NullOutput_ReturnsFalse()
    {
        using var cert = CreateEphemeralCert();
        using var input = new MemoryStream(RandomBytes(100));
        Assert.That(CryptoCore.Encrypt(input, null, cert), Is.False);
    }

    /// <summary>
    /// Verifies that stream encryption returns false when the certificate is null.
    /// </summary>
    [Test]
    public void Stream_Encrypt_NullCert_ReturnsFalse()
    {
        using var input = new MemoryStream(RandomBytes(100));
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Encrypt(input, output, null), Is.False);
    }

    /// <summary>
    /// Verifies that stream decryption returns false when the input stream is null.
    /// </summary>
    [Test]
    public void Stream_Decrypt_NullInput_ReturnsFalse()
    {
        using var cert = CreateEphemeralCert();
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Decrypt(null, output, cert), Is.False);
    }

    /// <summary>
    /// Verifies that stream decryption returns false when the output stream is null.
    /// </summary>
    [Test]
    public void Stream_Decrypt_NullOutput_ReturnsFalse()
    {
        using var cert = CreateEphemeralCert();
        using var input = new MemoryStream(RandomBytes(100));
        Assert.That(CryptoCore.Decrypt(input, null, cert), Is.False);
    }

    /// <summary>
    /// Verifies that stream decryption returns false when the certificate is null.
    /// </summary>
    [Test]
    public void Stream_Decrypt_NullCert_ReturnsFalse()
    {
        using var input = new MemoryStream(RandomBytes(100));
        using var output = new MemoryStream();
        Assert.That(CryptoCore.Decrypt(input, output, null), Is.False);
    }

    /// <summary>
    /// Verifies that tampering with reserved flag bits in the header causes decryption to fail.
    /// </summary>
    [Test]
    public void HeaderCorrupt_ReservedFlagBit_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(8000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert);
        Assert.That(env, Is.Not.Null);
        env![1] |= 0x04; // set reserved bit (bit2)
        Assert.That(CryptoCore.Decrypt(env, cert), Is.Null);
    }

    /// <summary>
    /// Verifies that out-of-range chunk size power values in the header are detected and decryption fails.
    /// </summary>
    [Test]
    public void HeaderCorrupt_ChunkSizePower_OutOfRange_Fails()
    {
        using var cert = CreateEphemeralCert();
        Byte[] pt = RandomBytes(10000);
        Byte[]? env = CryptoCore.Encrypt(pt, cert, chunksizepower: 16);
        Assert.That(env, Is.Not.Null);
        UInt16 rsaLen = (UInt16)((env![2] << 8) | env[3]);
        Int32 index = 4 + rsaLen;
        env[index] = 11;
        Assert.That(CryptoCore.Decrypt(env, cert), Is.Null);
        env[index] = 27;
        Assert.That(CryptoCore.Decrypt(env, cert), Is.Null);
    }

    /// <summary>
    /// Performs randomized round-trip tests with varying payload sizes, chunk sizes, and envelope features to ensure robustness.
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
            Int32 chunkPow = rnd.Next(12, 21);
            Boolean includeOrig = rnd.Next(0,2)==1;
            Boolean includeAadHash = rnd.Next(0,2)==1;
            Int32 aadLen = rnd.Next(0, 1024);
            Byte[] aad = RandomBytes(aadLen);
            Byte[]? env = CryptoCore.Encrypt(pt, cert, aad, includeAadHash, includeOrig, chunkPow);
            Assert.That(env, Is.Not.Null, $"Fuzz encrypt failed iter {i}");
            Byte[] aadDecrypt = includeAadHash ? aad : RandomBytes(aadLen);
            Byte[]? dec = CryptoCore.Decrypt(env, cert, aadDecrypt);
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
    /// Verifies that asynchronous array-based encryption and decryption round-trip the data correctly.
    /// </summary>
    [Test]
    public async Task Async_Array_EncryptDecrypt_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(120_000);
        Byte[]? env = await CryptoCore.EncryptAsync(plain, cert);
        Assert.That(env, Is.Not.Null);
        Byte[]? dec = await CryptoCore.DecryptAsync(env, cert);
        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that asynchronous stream-based encryption and decryption work as expected for large payloads.
    /// </summary>
    [Test]
    public async Task Async_Stream_EncryptDecrypt_RoundTrip()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(90_000);
        using var input = new MemoryStream(plain);
        using var encrypted = new MemoryStream();
        Assert.That(await CryptoCore.EncryptAsync(input, encrypted, cert), Is.True);
        encrypted.Position = 0;
        using var output = new MemoryStream();
        Assert.That(await CryptoCore.DecryptAsync(encrypted, output, cert), Is.True);
        Assert.That(output.ToArray(), Is.EquivalentTo(plain));
    }

    /// <summary>
    /// Verifies that async encryption returns false when the operation is cancelled via a cancellation token.
    /// </summary>
    [Test]
    public async Task Async_Encrypt_Cancelled_ReturnsFalse()
    {
        using var cert = CreateEphemeralCert();
        Byte[] plain = RandomBytes(500_000);
        using var input = new MemoryStream(plain);
        using var output = new MemoryStream();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Boolean ok = await CryptoCore.EncryptAsync(input, output, cert, cancel:cts.Token);
        Assert.That(ok, Is.False);
    }

    private static Int32 GetHeaderLength(Byte[] env)
    {
        if(env.Length < 4) throw new InvalidOperationException();
        UInt16 rsaLen = (UInt16)(env[2] << 8 | env[3]);
        Int32 offset = 4 + rsaLen;
        Byte chunkPower = env[offset++];
        Byte flags = env[1];
        if((flags & 0x01) != 0) offset += 8;
        if((flags & 0x02) != 0) offset += 64;
        return offset;
    }

    private static UInt32 ReadUInt32(Byte[] buffer, Int32 offset)
        => (UInt32)((buffer[offset] << 24) | (buffer[offset+1] << 16) | (buffer[offset+2] << 8) | buffer[offset+3]);
}