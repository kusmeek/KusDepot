namespace KusDepot.Exams.Cryptography;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class EncryptionEnvelopeV1CompatibilityExam
{
    private const String FixtureRoot = "KusDepot.Exam.CryptographyExams.EncryptionEnvelope.V1.Fixtures.";
    private const String PfxPassword = "compat-test";

    private X509Certificate2? Cert;
    private X509Certificate2? WrongCert;

    [OneTimeSetUp]
    public void Calibrate()
    {
        Byte[] pfx = LoadFixture("compat.pfx");
        Cert = X509CertificateLoader.LoadPkcs12(pfx,PfxPassword,X509KeyStorageFlags.Exportable);
        Assert.That(Cert, Is.Not.Null);

        WrongCert = CreateCertificate(Guid.NewGuid(),"EncryptionEnvelopeV1.WrongCert",2048,2,null);
        Assert.That(WrongCert, Is.Not.Null);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Cert?.Dispose();
        WrongCert?.Dispose();
    }

    private static Byte[] LoadFixture(String name)
    {
        String resource = FixtureRoot + name;
        using Stream? s = typeof(EncryptionEnvelopeV1CompatibilityExam).Assembly.GetManifestResourceStream(resource);
        Assert.That(s, Is.Not.Null,$"Missing embedded resource: {resource}");
        using MemoryStream m = new(); s!.CopyTo(m);
        return m.ToArray();
    }

    /// <summary>
    /// Verifies that a permanently stored empty-payload V1 envelope decrypts to an empty result.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_Empty_Succeeds()
    {
        Byte[] env = LoadFixture("empty.env.bin");
        Byte[] expected = LoadFixture("empty.plain.bin");

        Byte[]? dec = Utility.Decrypt(env,Cert!);

        Assert.That(dec, Is.Not.Null);
        Assert.That(dec!.Length, Is.EqualTo(0));
        Assert.That(dec, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that a permanently stored small-payload V1 envelope decrypts to the original plaintext.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_Small_Succeeds()
    {
        Byte[] env = LoadFixture("small.env.bin");
        Byte[] expected = LoadFixture("small.plain.bin");

        Byte[]? dec = Utility.Decrypt(env,Cert!);

        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that a permanently stored multi-chunk V1 envelope decrypts to the original plaintext.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_MultiChunk_Succeeds()
    {
        Byte[] env = LoadFixture("multichunk.env.bin");
        Byte[] expected = LoadFixture("multichunk.plain.bin");

        Byte[]? dec = Utility.Decrypt(env,Cert!);

        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that a permanently stored V1 envelope without original length decrypts to the original plaintext.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_NoLength_Succeeds()
    {
        Byte[] env = LoadFixture("nolength.env.bin");
        Byte[] expected = LoadFixture("nolength.plain.bin");

        Byte[]? dec = Utility.Decrypt(env,Cert!);

        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that a permanently stored AAD-bound V1 envelope decrypts successfully with the correct AAD.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_AadBound_Succeeds()
    {
        Byte[] env = LoadFixture("aad.env.bin");
        Byte[] expected = LoadFixture("aad.plain.bin");
        Byte[] aad = LoadFixture("aad.aad.bin");

        Byte[]? dec = Utility.Decrypt(env,Cert!,aad);

        Assert.That(dec, Is.Not.Null);
        Assert.That(dec, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that a permanently stored AAD-bound V1 envelope fails to decrypt with incorrect AAD.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_AadBound_WrongAad_Fails()
    {
        Byte[] env = LoadFixture("aad.env.bin");
        Byte[] wrongAad = Encoding.UTF8.GetBytes("wrong-aad-value");

        Byte[]? dec = null;
        Boolean caught = false;

        try { dec = Utility.Decrypt(env,Cert!,wrongAad); }
        catch { caught = true; }

        Assert.That(caught || dec is null);
    }

    /// <summary>
    /// Verifies that a permanently stored V1 envelope fails to decrypt with the wrong certificate.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_WrongCert_Fails()
    {
        Byte[] env = LoadFixture("small.env.bin");

        Byte[]? dec = null;
        Boolean caught = false;

        try { dec = Utility.Decrypt(env,WrongCert!); }
        catch { caught = true; }

        Assert.That(caught || dec is null);
    }

    /// <summary>
    /// Verifies that the first byte of all permanently stored V1 envelopes is the V1 version marker.
    /// </summary>
    [TestCase("empty.env.bin")]
    [TestCase("small.env.bin")]
    [TestCase("multichunk.env.bin")]
    [TestCase("nolength.env.bin")]
    [TestCase("aad.env.bin")]
    public void V1_DecryptFixture_VersionByte_IsV1(String fixture)
    {
        Byte[] env = LoadFixture(fixture);
        Assert.That(env[0], Is.EqualTo((Byte)0x01));
    }

    /// <summary>
    /// Verifies that the permanently stored V1 envelope decrypts correctly through the stream path.
    /// </summary>
    [Test]
    public void V1_DecryptFixture_Small_Stream_Succeeds()
    {
        Byte[] env = LoadFixture("small.env.bin");
        Byte[] expected = LoadFixture("small.plain.bin");

        using MemoryStream input = new(env);
        using MemoryStream output = new();
        Boolean result = Utility.Decrypt(input,output,Cert!);

        Assert.That(result, Is.True);
        Assert.That(output.ToArray(), Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that the permanently stored V1 envelope decrypts correctly through the async stream path.
    /// </summary>
    [Test]
    public async Task V1_DecryptFixture_Small_StreamAsync_Succeeds()
    {
        Byte[] env = LoadFixture("small.env.bin");
        Byte[] expected = LoadFixture("small.plain.bin");

        await using MemoryStream input = new(env);
        await using MemoryStream output = new();
        Boolean result = await Utility.DecryptAsync(input,output,Cert!);

        Assert.That(result, Is.True);
        Assert.That(output.ToArray(), Is.EqualTo(expected));
    }
}
