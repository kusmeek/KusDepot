namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class UtilityExam
{
    [Test]
    public void Compress()
    {
        Byte[] b = new Byte[42974109]; RandomNumberGenerator.Create().GetBytes(b);

        Byte[]? c = b.Compress();

        Check.That(c!.Decompress()!.SequenceEqual(b)).IsTrue();
    }

    [Test]
    public void Decompress()
    {
        Byte[] b = new Byte[42974109]; RandomNumberGenerator.Create().GetBytes(b);

        Byte[]? c = b.Compress();

        Check.That(c!.Decompress()!.SequenceEqual(b)).IsTrue();
    }

    [Test]
    public void Decrypt()
    {
        CertificateRequest _r = new CertificateRequest("CN=SelfSigned",RSA.Create(4096), HashAlgorithmName.SHA512, RSASignaturePadding.Pss);

        X509Certificate2 _c = _r.CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1));

        Byte[] _0 = new Byte[90491804]; RandomNumberGenerator.Create().GetBytes(_0);

        Byte[]? _1 = Utility.Encrypt(_0,_c); Byte[]? _2 = Utility.Decrypt(_1,_c);

        Check.That(_1).IsNotNull(); Check.That(_2).IsNotNull();

        Check.That(_0.SequenceEqual(_1!)).IsFalse();

        Check.That(_0.SequenceEqual(_2!)).IsTrue();
    }

    [Test]
    public void Encrypt()
    {
        CertificateRequest _r = new CertificateRequest("CN=SelfSigned",RSA.Create(4096), HashAlgorithmName.SHA512, RSASignaturePadding.Pss);

        X509Certificate2 _c = _r.CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1));

        Byte[] _0 = new Byte[90460184]; RandomNumberGenerator.Create().GetBytes(_0);

        Byte[]? _1 = Utility.Encrypt(_0,_c); Byte[]? _2 = Utility.Decrypt(_1,_c);

        Check.That(_1).IsNotNull(); Check.That(_2).IsNotNull();

        Check.That(_0.SequenceEqual(_1!)).IsFalse();

        Check.That(_0.SequenceEqual(_2!)).IsTrue();
    }

    [Test]
    public void ToBase64FromByteArray()
    {
        Byte[] _0 = new Byte[5427184]; RandomNumberGenerator.Create().GetBytes(_0);

        String _1 = Convert.ToBase64String(_0);

        String _2 = _0.ToBase64FromByteArray();

        Byte[] _3 = _1.ToByteArrayFromBase64();

        Check.That(String.Equals(_1,_2,StringComparison.Ordinal)).IsTrue();

        Check.That(_0.SequenceEqual(_3)).IsTrue();
    }

    [Test]
    public void ToByteArrayFromBase64()
    {
        Byte[] _0 = new Byte[5427184]; RandomNumberGenerator.Create().GetBytes(_0);

        String _1 = Convert.ToBase64String(_0);

        String _2 = _0.ToBase64FromByteArray();

        Byte[] _3 = _1.ToByteArrayFromBase64();

        Check.That(String.Equals(_1,_2,StringComparison.Ordinal)).IsTrue();

        Check.That(_0.SequenceEqual(_3)).IsTrue();
    }

    [Test]
    public void ToByteArrayFromUTF16String()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(5427184);

        Byte[] _1 = _0.ToByteArrayFromUTF16String();

        String _2 = Encoding.Unicode.GetString(_1);

        Byte[] _3 = Encoding.Unicode.GetBytes(_2);

        String _4 = _3.ToUTF16StringFromByteArray();

        Check.That(String.Equals(_0,_2,StringComparison.Ordinal)).IsTrue();

        Check.That(_1.SequenceEqual(_3)).IsTrue();

        Check.That(String.Equals(_0,_4,StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public void ToUTF16StringFromByteArray()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(5427184);

        Byte[] _1 = _0.ToByteArrayFromUTF16String();

        String _2 = Encoding.Unicode.GetString(_1);

        Byte[] _3 = Encoding.Unicode.GetBytes(_2);

        String _4 = _3.ToUTF16StringFromByteArray();

        Check.That(String.Equals(_0,_2,StringComparison.Ordinal)).IsTrue();

        Check.That(_1.SequenceEqual(_3)).IsTrue();

        Check.That(String.Equals(_0,_4,StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public void TryGetStringFromByteArray()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(5427184);

        Byte[] _1 = _0.ToByteArrayFromUTF16String();

        String _2 = _1.TryGetStringFromByteArray();

        Check.That(String.Equals(_0,_2,StringComparison.Ordinal)).IsTrue();

        Check.That(((Byte[]?)null).TryGetStringFromByteArray()).Equals(String.Empty);
    }
}