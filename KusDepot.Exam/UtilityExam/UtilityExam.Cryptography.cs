namespace KusDepot.Exams;

[TestFixture]
public partial class UtilityExam
{
    [Test]
    public void CertificateCreation()
    {
        var c4096 = CreateCertificate(Guid.NewGuid(),"Cert4096",4096,5,null);
        var c3072 = CreateCertificate(Guid.NewGuid(),"Cert3072",3072,3,null);
        var cBad  = CreateCertificate(Guid.NewGuid(),"Bad",1025,1,null);
        Check.That(c4096).IsNotNull();
        Check.That(c3072).IsNotNull();
        Check.That(cBad).IsNull();
        Check.That(c4096!.GetRSAPublicKey()!.KeySize).IsEqualTo(4096);
        Check.That(c3072!.GetRSAPublicKey()!.KeySize).IsEqualTo(3072);
    }

    [Test]
    public void EncryptDecrypt_ByteArrays_VariousSizes()
    {
        foreach(Int32 size in new[]{0,1,32,1024, 256*1024})
        {
            Byte[] data = RandomBytes(size);
            Byte[]? enc = data.Encrypt(CertA);
            Check.That(enc).IsNotNull();
            Check.That(enc!.SequenceEqual(data)).IsFalse();
            Byte[]? dec = enc.Decrypt(CertA);
            Check.That(dec).IsNotNull();
            Check.That(dec!.SequenceEqual(data)).IsTrue();
        }
    }

    [Test]
    public async Task EncryptDecrypt_ByteArrays_Async()
    {
        Byte[] data = RandomBytes(128*1024);
        Byte[]? enc = await data.EncryptAsync(CertA);
        Check.That(enc).IsNotNull();
        Byte[]? dec = await enc!.DecryptAsync(CertA);
        Check.That(dec).IsNotNull();
        Check.That(dec!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void Encrypt_WrongCertificate_Fails()
    {
        Byte[] data = RandomBytes(512);
        Byte[]? enc = data.Encrypt(CertA);
        Check.That(enc).IsNotNull();
        Byte[]? decWrong = enc!.Decrypt(CertB);
        Check.That(decWrong).IsNull();
    }

    [Test]
    public void Encrypt_Randomness()
    {
        Byte[] data = RandomBytes(4096);
        Byte[]? c1 = data.Encrypt(CertA);
        Byte[]? c2 = data.Encrypt(CertA);
        Check.That(c1).IsNotNull(); Check.That(c2).IsNotNull();
        Check.That(c1!.SequenceEqual(c2!)).IsFalse();
    }

    [Test]
    public void EncryptDecrypt_Stream_WithAAD()
    {
        Byte[] data = RandomBytes(90_000); Byte[] aad = RandomBytes(16);
        using MemoryStream input = new(data, writable:false); using MemoryStream encOut = new();
        Boolean eok = Encrypt(input,encOut,CertA,aad); Check.That(eok).IsTrue();
        using MemoryStream decIn = new(encOut.ToArray(), writable:false); using MemoryStream decOut = new();
        Boolean dok = Decrypt(decIn,decOut,CertA,aad); Check.That(dok).IsTrue();
        Check.That(decOut.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public async Task EncryptDecrypt_Stream_WithAAD_MismatchFails()
    {
        Byte[] data = RandomBytes(32_000); Byte[] aadGood = RandomBytes(32); Byte[] aadBad = RandomBytes(32);
        using MemoryStream input = new(data, writable:false); using MemoryStream encOut = new();
        Check.That(await EncryptAsync(input,encOut,CertA,aadGood,includeaadhash:true)).IsTrue();
        using MemoryStream decIn = new(encOut.ToArray(), writable:false); using MemoryStream decOut = new();
        Boolean dok = await DecryptAsync(decIn,decOut,CertA,aadBad);
        Check.That(dok).IsFalse();
        using MemoryStream input2 = new(data, writable:false); using MemoryStream encOut2 = new();
        Check.That(await EncryptAsync(input2,encOut2,CertA)).IsTrue();
        using MemoryStream decIn2 = new(encOut2.ToArray(), writable:false); using MemoryStream decOut2 = new();
        Boolean dok2 = await DecryptAsync(decIn2,decOut2,CertA);
        Check.That(dok2).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_TamperDetection()
    {
        Byte[] data = RandomBytes(5000);
        Byte[]? enc = data.Encrypt(CertA); Check.That(enc).IsNotNull();
        Byte[] tampered = (Byte[])enc!.Clone(); Int32 idx = tampered.Length/2; tampered[idx] ^= 0xAA;
        Byte[]? dec = tampered.Decrypt(CertA);
        Check.That(dec).IsNull();
    }

    [Test]
    public void EncryptDecrypt_NullArgs()
    {
        Byte[] sample = RandomBytes(32);
        Check.That(((Byte[]?)null).Encrypt(CertA)).IsNull();
        Check.That(sample.Encrypt(null)).IsNull();
        Check.That(((Byte[]?)null).Decrypt(CertA)).IsNull();
    }

    [Test]
    public void EncryptDecrypt_RsaWrappedKey_Uniqueness()
    {
        HashSet<String> wrappedKeyBlobs = new();
        Byte[] data = RandomBytes(64);
        for (Int32 i = 0; i < 30; i++)
        {
            Byte[]? enc = data.Encrypt(CertA);
            Check.That(enc).IsNotNull();
            // Header: [0]=version, [1]=flags, [2-3]=rsaLen (big-endian), [4...]=rsaWrappedKey
            Int32 rsaLen = (enc![2] << 8) | enc[3];
            String wrappedKey = Convert.ToBase64String(enc, 4, rsaLen);
            Check.That(wrappedKeyBlobs.Contains(wrappedKey)).IsFalse();
            wrappedKeyBlobs.Add(wrappedKey);
        }
    }

    [Test]
    public void CompressThenEncrypt_RoundTrip()
    {
        Byte[] data = Encoding.UTF8.GetBytes(new String('Z',100_000));
        Byte[]? comp = data.Compress(); Check.That(comp).IsNotNull();
        Byte[]? enc = comp!.Encrypt(CertA); Check.That(enc).IsNotNull();
        Byte[]? dec = enc!.Decrypt(CertA); Check.That(dec).IsNotNull();
        Byte[]? decomp = dec!.Decompress(); Check.That(decomp).IsNotNull();
        Check.That(decomp!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public async Task CompressThenEncrypt_RoundTrip_Async()
    {
        Byte[] data = RandomBytes(220_000);
        Byte[]? comp = await data.CompressAsync(); Check.That(comp).IsNotNull();
        Byte[]? enc = await comp!.EncryptAsync(CertA); Check.That(enc).IsNotNull();
        Byte[]? dec = await enc!.DecryptAsync(CertA); Check.That(dec).IsNotNull();
        Byte[]? decomp = await dec!.DecompressAsync(); Check.That(decomp).IsNotNull();
        Check.That(decomp!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void Stream_Compress_Encrypt_Decrypt_Decompress()
    {
        Byte[] data = Encoding.UTF8.GetBytes(new String('Q',180_000)); Byte[] aad = RandomBytes(24);
        using MemoryStream rawIn = new(data, writable:false);
        using MemoryStream compOut = new();
        Check.That(Compress(rawIn, compOut)).IsTrue();
        using MemoryStream compIn = new(compOut.ToArray(), writable:false);
        using MemoryStream encOut = new();
        Check.That(Encrypt(compIn, encOut, CertA, aad)).IsTrue();
        using MemoryStream encIn = new(encOut.ToArray(), writable:false);
        using MemoryStream decOut = new();
        Check.That(Decrypt(encIn, decOut, CertA, aad)).IsTrue();
        using MemoryStream decCompIn = new(decOut.ToArray(), writable:false);
        using MemoryStream finalOut = new();
        Check.That(Decompress(decCompIn, finalOut)).IsTrue();
        Check.That(finalOut.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public async Task Stream_Compress_Encrypt_Decrypt_Decompress_Async()
    {
        Byte[] data = RandomBytes(250_000); Byte[] aad = RandomBytes(16);
        using MemoryStream rawIn = new(data, writable:false);
        using MemoryStream compOut = new();
        Check.That(await CompressAsync(rawIn, compOut)).IsTrue();
        using MemoryStream compIn = new(compOut.ToArray(), writable:false);
        using MemoryStream encOut = new();
        Check.That(await EncryptAsync(compIn, encOut, CertA, aad)).IsTrue();
        using MemoryStream encIn = new(encOut.ToArray(), writable:false);
        using MemoryStream decOut = new();
        Check.That(await DecryptAsync(encIn, decOut, CertA, aad)).IsTrue();
        using MemoryStream decCompIn = new(decOut.ToArray(), writable:false);
        using MemoryStream finalOut = new();
        Check.That(await DecompressAsync(decCompIn, finalOut)).IsTrue();
        Check.That(finalOut.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void CompressEncrypt_TamperDetection()
    {
        Byte[] data = Encoding.UTF8.GetBytes(new String('R', 60_000));
        Byte[]? comp = data.Compress(); Check.That(comp).IsNotNull();
        Byte[]? enc = comp!.Encrypt(CertA); Check.That(enc).IsNotNull();
        Byte[] tampered = (Byte[])enc!.Clone(); tampered[^10] ^= 0x55;
        Byte[]? dec = tampered.Decrypt(CertA);
        Check.That(dec).IsNull();
    }

    [Test]
    public async Task EncryptDecrypt_StreamAsync_WithAAD()
    {
        Byte[] data = RandomBytes(600_000); Byte[] aad = RandomBytes(24);
        using MemoryStream input = new(data, writable:false); using MemoryStream encOut = new();
        Boolean eok = await EncryptAsync(input,encOut,CertA,aad); Check.That(eok).IsTrue();
        using MemoryStream decIn = new(encOut.ToArray(), writable:false); using MemoryStream decOut = new();
        Boolean dok = await DecryptAsync(decIn,decOut,CertA,aad); Check.That(dok).IsTrue();
        Check.That(decOut.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_LargePayload_ByteArray()
    {
        Byte[] data = RandomBytes(2_000_000);
        Byte[]? enc = data.Encrypt(CertA); Check.That(enc).IsNotNull();
        Byte[]? dec = enc!.Decrypt(CertA); Check.That(dec).IsNotNull();
        Check.That(dec!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_LargePayload_Stream()
    {
        Byte[] data = RandomBytes(3_000_000);
        using MemoryStream input = new(data, writable:false); using MemoryStream encOut = new();
        Check.That(Encrypt(input, encOut, CertA)).IsTrue();
        using MemoryStream decIn = new(encOut.ToArray(), writable:false); using MemoryStream decOut = new();
        Check.That(Decrypt(decIn, decOut, CertA)).IsTrue();
        Check.That(decOut.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public async Task EncryptDecrypt_LargePayload_StreamAsync()
    {
        Byte[] data = RandomBytes(40_000_000);
        using MemoryStream input = new(data, writable:false); using MemoryStream encOut = new();
        Check.That(await EncryptAsync(input, encOut, CertA)).IsTrue();
        using MemoryStream decIn = new(encOut.ToArray(), writable:false); using MemoryStream decOut = new();
        Boolean ok = await DecryptAsync(decIn, decOut, CertA); Check.That(ok).IsTrue();
        Check.That(decOut.ToArray().SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_AAD_EmptyVsPresent_Fails()
    {
        Byte[] data = RandomBytes(40_000); Byte[] aad = RandomBytes(16);
        using (MemoryStream input1 = new(data, writable: false))
        using (MemoryStream enc1 = new())
        {
            Check.That(Encrypt(input1, enc1, CertA)).IsTrue();
            using MemoryStream enc1In = new(enc1.ToArray(), writable: false); using MemoryStream out1 = new();
            Check.That(Decrypt(enc1In, out1, CertA)).IsTrue();
        }

        using (MemoryStream input2 = new(data, writable: false))
        using (MemoryStream enc2 = new())
        {
            Check.That(Encrypt(input2, enc2, CertA)).IsTrue();
            using MemoryStream enc2In = new(enc2.ToArray(), writable: false); using MemoryStream out2 = new();
            Check.That(Decrypt(enc2In, out2, CertA, aad)).IsFalse();
        }

        using (MemoryStream input3 = new(data, writable: false))
        using (MemoryStream enc3 = new())
        {
            Check.That(Encrypt(input3, enc3, CertA, aad)).IsTrue();
            using MemoryStream enc3In = new(enc3.ToArray(), writable: false); using MemoryStream out3 = new();
            Check.That(Decrypt(enc3In, out3, CertA, aad)).IsTrue();
        }

        using (MemoryStream input4 = new(data, writable: false))
        using (MemoryStream enc4 = new())
        {
            Check.That(Encrypt(input4, enc4, CertA, aad)).IsTrue();
            using MemoryStream enc4In = new(enc4.ToArray(), writable: false); using MemoryStream out4 = new();
            Check.That(Decrypt(enc4In, out4, CertA, Array.Empty<Byte>())).IsFalse();
        }

        Byte[] aad2 = RandomBytes(16);
        using (MemoryStream input5 = new(data, writable: false))
        using (MemoryStream enc5 = new())
        {
            Check.That(Encrypt(input5, enc5, CertA, aad)).IsTrue();
            using MemoryStream enc5In = new(enc5.ToArray(), writable: false); using MemoryStream out5 = new();
            Check.That(Decrypt(enc5In, out5, CertA, aad2)).IsFalse();
        }

        using (MemoryStream input7 = new(data, writable: false))
        using (MemoryStream enc7 = new())
        {
            Check.That(Encrypt(input7, enc7, CertA)).IsTrue();
            using MemoryStream enc7In = new(enc7.ToArray(), writable: false); using MemoryStream out7 = new();
            Check.That(Decrypt(enc7In, out7, CertA, Array.Empty<Byte>())).IsTrue();
        }

        using (MemoryStream input8 = new(data, writable: false))
        using (MemoryStream enc8 = new())
        {
            Check.That(Encrypt(input8, enc8, CertA, null)).IsTrue();
            using MemoryStream enc8In = new(enc8.ToArray(), writable: false); using MemoryStream out8 = new();
            Check.That(Decrypt(enc8In, out8, CertA, null)).IsTrue();
            enc8In.Seek(0, SeekOrigin.Begin);
            Check.That(Decrypt(enc8In, out8, CertA, Array.Empty<Byte>())).IsTrue();
        }
    }

    [Test]
    public async Task EncryptDecrypt_Stream_Cancellation()
    {
        Byte[] data = RandomBytes(120_000); Byte[] aad = RandomBytes(8);
        using MemoryStream input = new(data, writable:false); using MemoryStream encOut = new();
        using CancellationTokenSource cts = new(); cts.Cancel();
        Check.That(await EncryptAsync(input, encOut, CertA, aad)).IsTrue();
        using MemoryStream decIn = new(encOut.ToArray(), writable:false); using MemoryStream decOut = new();
        Boolean dok = await DecryptAsync(decIn, decOut, CertA, aad, cts.Token);
        Check.That(dok).IsFalse();
    }

    [Test]
    public void EncryptDecrypt_ByteArray_WithAAD()
    {
        Byte[] data = RandomBytes(4096); Byte[] aad = RandomBytes(16);
        Byte[]? enc = data.Encrypt(CertA, aad); Check.That(enc).IsNotNull();
        Byte[]? dec = enc!.Decrypt(CertA, aad); Check.That(dec).IsNotNull();
        Check.That(dec!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_ByteArray_AAD_MismatchFails()
    {
        Byte[] data = RandomBytes(2048); Byte[] aadGood = RandomBytes(24); Byte[] aadBad = RandomBytes(24);
        Byte[]? enc = data.Encrypt(CertA, aadGood, includeaadhash : true); Check.That(enc).IsNotNull();
        Byte[]? decBad = enc!.Decrypt(CertA, aadBad); Check.That(decBad).IsNull();
        Byte[]? decGood = enc!.Decrypt(CertA, aadGood); Check.That(decGood).IsNotNull();
        Check.That(decGood!.SequenceEqual(data)).IsTrue();

        Byte[]? encDefault = data.Encrypt(CertA, aadGood);
        Check.That(encDefault).IsNotNull();
        Byte[]? decDefaultGood = encDefault!.Decrypt(CertA, aadGood);
        Check.That(decDefaultGood).IsNotNull();
        Check.That(decDefaultGood!.SequenceEqual(data)).IsTrue();
        Byte[]? decDefaultBad = encDefault!.Decrypt(CertA, aadBad);
        Check.That(decDefaultBad).IsNull();
    }

    [Test]
    public void EncryptDecrypt_ByteArray_AAD_EmptyVsPresent_Fails()
    {
        Byte[] data = RandomBytes(40_000); Byte[] aad = RandomBytes(16);

        Byte[]? enc1 = data.Encrypt(CertA);
        Check.That(enc1).IsNotNull();
        Byte[]? dec1 = enc1!.Decrypt(CertA);
        Check.That(dec1).IsNotNull();
        Check.That(dec1!.SequenceEqual(data)).IsTrue();

        Byte[]? enc2 = data.Encrypt(CertA);
        Check.That(enc2).IsNotNull();
        Byte[]? dec2 = enc2!.Decrypt(CertA, aad);
        Check.That(dec2).IsNull();        

        Byte[]? enc3 = data.Encrypt(CertA, aad);
        Check.That(enc3).IsNotNull();
        Byte[]? dec3 = enc3!.Decrypt(CertA, aad);
        Check.That(dec3).IsNotNull();
        Check.That(dec3!.SequenceEqual(data)).IsTrue();

        Byte[]? enc4 = data.Encrypt(CertA, aad);
        Check.That(enc4).IsNotNull();
        Byte[]? dec4 = enc4!.Decrypt(CertA, Array.Empty<Byte>());
        Check.That(dec4).IsNull();

        Byte[] aad2 = RandomBytes(16);
        Byte[]? enc5 = data.Encrypt(CertA, aad);
        Check.That(enc5).IsNotNull();
        Byte[]? dec5 = enc5!.Decrypt(CertA, aad2);
        Check.That(dec5).IsNull();

        Byte[]? enc6 = data.Encrypt(CertA);
        Check.That(enc6).IsNotNull();
        Byte[]? dec6 = enc6!.Decrypt(CertA, Array.Empty<Byte>());
        Check.That(dec3).IsNotNull();
        Check.That(dec6!.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_Span_RoundTrip()
    {
        ReadOnlySpan<Byte> data = RandomBytes(1024);
        Span<Byte> enc = stackalloc Byte[2048];
        Check.That(Encrypt(data, enc, CertA, out Int32 written)).IsTrue();
        Check.That(written).IsStrictlyGreaterThan(data.Length);

        Span<Byte> dec = stackalloc Byte[data.Length];
        Check.That(Decrypt(enc[..written], dec, CertA, out Int32 decWritten)).IsTrue();
        Check.That(decWritten).IsEqualTo(data.Length);
        Check.That(dec.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_Span_AAD_MismatchFails()
    {
        ReadOnlySpan<Byte> data = RandomBytes(512);
        ReadOnlySpan<Byte> aadGood = RandomBytes(32);
        ReadOnlySpan<Byte> aadBad = RandomBytes(32);
        Span<Byte> enc = stackalloc Byte[1024];

        Check.That(Encrypt(data, enc, CertA, out Int32 written, aadGood, includeaadhash: true)).IsTrue();

        Span<Byte> dec = stackalloc Byte[data.Length];
        Check.That(Decrypt(enc[..written], dec, CertA, out _, aadBad)).IsFalse();
        Check.That(Decrypt(enc[..written], dec, CertA, out Int32 decWritten, aadGood)).IsTrue();
        Check.That(decWritten).IsEqualTo(data.Length);
        Check.That(dec.SequenceEqual(data)).IsTrue();
    }

    [Test]
    public void EncryptDecrypt_Span_TamperDetection()
    {
        ReadOnlySpan<Byte> data = RandomBytes(256);
        Span<Byte> enc = stackalloc Byte[1024];
        Check.That(Encrypt(data, enc, CertA, out Int32 written)).IsTrue();

        enc[written / 2] ^= 0xFF;

        Span<Byte> dec = stackalloc Byte[data.Length];
        Check.That(Decrypt(enc[..written], dec, CertA, out _)).IsFalse();
    }

    [Test]
    public void SignHash_And_VerifyHash_RoundTrip()
    {
        var cert = CreateCertificate(Guid.NewGuid(), "SignVerifyTest", 2048, 2, null);
        Check.That(cert).IsNotNull();
        var data = Encoding.UTF8.GetBytes("Test data for signing");
        var hash = SHA512.HashData(data);
        var signature = SignHash(cert, hash);
        Check.That(signature).IsNotNull();
        var valid = VerifyHash(cert, hash, signature);
        Check.That(valid).IsTrue();
    }

    [Test]
    public void SignHash_VerifyHash_Fails_On_Modified_Data()
    {
        var cert = CreateCertificate(Guid.NewGuid(), "SignVerifyTestFail", 2048, 2, null);
        Check.That(cert).IsNotNull();
        var data = Encoding.UTF8.GetBytes("Original data");
        var hash = SHA512.HashData(data);
        var signature = SignHash(cert, hash);
        Check.That(signature).IsNotNull();
        var tamperedHash = (Byte[])hash.Clone();
        tamperedHash[0] ^= 0xFF;
        var valid = VerifyHash(cert, tamperedHash, signature);
        Check.That(valid).IsFalse();
    }

    [Test]
    public void SignHash_VerifyHash_Fails_On_Modified_Signature()
    {
        var cert = CreateCertificate(Guid.NewGuid(), "SignVerifyTestFail2", 2048, 2, null);
        Check.That(cert).IsNotNull();
        var data = Encoding.UTF8.GetBytes("Original data");
        var hash = SHA512.HashData(data);
        var signature = SignHash(cert, hash);
        Check.That(signature).IsNotNull();
        var tamperedSignature = (Byte[])signature!.Clone();
        tamperedSignature[0] ^= 0xFF;
        var valid = VerifyHash(cert, hash, tamperedSignature);
        Check.That(valid).IsFalse();
    }

    [Test]
    public void SignHash_VerifyHash_Null_Arguments()
    {
        var cert = CreateCertificate(Guid.NewGuid(), "SignVerifyTestNull", 2048, 2, null);
        var data = Encoding.UTF8.GetBytes("Null test");
        var hash = SHA512.HashData(data);
        Check.That(SignHash((X509Certificate2?)null, hash)).IsNull();
        Check.That(SignHash(cert, null)).IsNull();
        Check.That(VerifyHash((X509Certificate2?)null, hash, new Byte[1])).IsFalse();
        Check.That(VerifyHash(cert, null, new Byte[1])).IsFalse();
        Check.That(VerifyHash(cert, hash, null)).IsFalse();
    }

    [Test]
    public void ZeroMemory_Stream()
    {
        Byte[] data = RandomBytes(1024);
        using MemoryStream ms = new MemoryStream();
        ms.Write(data, 0, data.Length);
        long originalPosition = ms.Position;

        Check.That(ZeroMemory(ms)).IsTrue();

        Check.That(ms.Position).IsEqualTo(originalPosition);

        Byte[] clearedData = new Byte[data.Length];
        ms.Seek(0, SeekOrigin.Begin);
        ms.Read(clearedData, 0, clearedData.Length);

        Check.That(clearedData.All(b => b == 0)).IsTrue();
    }
}
