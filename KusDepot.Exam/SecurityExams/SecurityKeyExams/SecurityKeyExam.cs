namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class SecurityKeyExam
{
    private X509Certificate2? Cert;
    private Byte[]? KeyData1;
    private Byte[]? KeyData2;
    private Guid Id1;
    private Guid Id2;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        using var rsa = RSA.Create(4096);
        var req = new CertificateRequest("CN=KeyTests", rsa, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);
        Cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

        Id1 = Guid.NewGuid();
        Id2 = Guid.NewGuid();

        KeyData1 = RandomNumberGenerator.GetBytes(32);
        KeyData2 = RandomNumberGenerator.GetBytes(32);

        if(Cert is null || KeyData1 is null || KeyData2 is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Constructor_InitializesFields()
    {
        var mk = new ManagerKey(Cert!, Id1);
        var sk = new ServiceKey(KeyData1!, Id1);

        Check.That(mk.ID).IsNotNull();
        Check.That(mk.Key).IsNotNull();
        Check.That(sk.ID).IsNotNull();
        Check.That(sk.Key).IsNotNull();
    }

    [Test]
    public void Clone_Roundtrip_ManagerKey_And_ServiceKey()
    {
        var mk = new ManagerKey(Cert!, Id1);
        var sk = new ServiceKey(KeyData1!, Id1);

        var mkClone = mk.Copy();
        var skClone = sk.Copy();

        Check.That(mkClone).IsNotNull().And.IsInstanceOf<ManagerKey>();
        Check.That(skClone).IsNotNull().And.IsInstanceOf<ServiceKey>();

        Check.That(mkClone!.ID).IsEqualTo(mk.ID);
        Check.That(skClone!.ID).IsEqualTo(sk.ID);

        Check.That(mkClone.Key).IsEqualTo(mk.Key);
        Check.That(skClone.Key).IsEqualTo(sk.Key);
    }

    [Test]
    public void ToString_Parse_Roundtrip_For_Derived()
    {
        var mk = new ManagerKey(Cert!, Id1);
        var sk = new ServiceKey(KeyData1!, Id1);

        var mkStr = mk.ToString();
        var skStr = sk.ToString();

        var anyMk = SecurityKey.Parse(mkStr);
        var anySk = SecurityKey.Parse(skStr);

        var genMk = SecurityKey.Parse<ManagerKey>(mkStr);
        var genSk = SecurityKey.Parse<ServiceKey>(skStr);

        Check.That(anyMk).IsNotNull().And.IsInstanceOf<ManagerKey>();
        Check.That(anySk).IsNotNull().And.IsInstanceOf<ServiceKey>();
        Check.That(genMk).IsNotNull().And.IsInstanceOf<ManagerKey>();
        Check.That(genSk).IsNotNull().And.IsInstanceOf<ServiceKey>();

        Check.That(genMk!.ID).IsEqualTo(mk.ID);
        Check.That(genSk!.ID).IsEqualTo(sk.ID);
        Check.That(genMk.Key).IsEqualTo(mk.Key);
        Check.That(genSk.Key).IsEqualTo(sk.Key);
    }

    [Test]
    public void TryParse_And_InvalidInputs()
    {
        var sk = new ServiceKey(KeyData1!, Id1);
        var s = sk.ToString();

        SecurityKey? result;
        var ok1 = SecurityKey.TryParse(s, null, out result);
        Check.That(ok1).IsTrue();
        Check.That(result).IsNotNull().And.IsInstanceOf<ServiceKey>();

        ServiceKey? parsedSk;
        var ok2 = SecurityKey.TryParse<ServiceKey>(s, null, out parsedSk);
        Check.That(ok2).IsTrue();
        Check.That(parsedSk).IsNotNull();
        Check.That(parsedSk!.ID).IsEqualTo(sk.ID);

        Check.That(SecurityKey.Parse(String.Empty)).IsNull();
        SecurityKey? parsedNull;
        var ok3 = SecurityKey.TryParse(null, null, out parsedNull);
        Check.That(ok3).IsFalse();
    }

    [Test]
    public void ParseAny_RoutesToDerivedTypes()
    {
        var mk = new ManagerKey(Cert!, Id1);
        var sk = new ServiceKey(KeyData1!, Id1);

        var p1 = SecurityKey.Parse(mk.ToString());
        var p2 = SecurityKey.Parse(sk.ToString());

        Check.That(p1).IsNotNull().And.IsInstanceOf<ManagerKey>();
        Check.That(p2).IsNotNull().And.IsInstanceOf<ServiceKey>();
    }

    [Test]
    public void Equals_And_GetHashCode()
    {
        var mk1 = new ManagerKey(Cert!, Id1);
        var mk2_same = new ManagerKey(Cert!, Id1);
        var mk3_diffId = new ManagerKey(Cert!, Id2);

        var sk1 = new ServiceKey(KeyData1!, Id1);
        var sk2_same = new ServiceKey(KeyData1!.ToArray(), Id1);
        var sk3_diffKey = new ServiceKey(KeyData2!, Id1);
        var sk4_diffId = new ServiceKey(KeyData1!, Id2);

        Check.That(mk1.Equals(mk2_same)).IsTrue();
        Check.That(mk1.GetHashCode()).IsEqualTo(mk2_same.GetHashCode());
        Check.That(mk1.Equals(mk3_diffId)).IsFalse();

        Check.That(sk1.Equals(sk2_same)).IsTrue();
        Check.That(sk1.GetHashCode()).IsEqualTo(sk2_same.GetHashCode());
        Check.That(sk1.Equals(sk3_diffKey)).IsFalse();
        Check.That(sk1.Equals(sk4_diffId)).IsFalse();

        Check.That(mk1.Equals((SecurityKey)sk1)).IsFalse();
        Check.That(mk1.Equals((Object?)null)).IsFalse();
        Object asObj = mk2_same;
        Check.That(mk1!.Equals(asObj)).IsTrue();
    }

    [Test]
    public void Key_DefensiveCopy_ManagerKey_And_ServiceKey()
    {
        var mk = new ManagerKey(Cert!, Id1);
        var sk = new ServiceKey(KeyData1!, Id1);

        var mkOriginal = mk.Key!;
        var skOriginal = sk.Key!;

        var mkCopy = mk.Key!;
        var skCopy = sk.Key!;

        if(mkCopy.Length > 0) mkCopy[0] ^= 0xFF;
        if(skCopy.Length > 0) skCopy[0] ^= 0xFF;

        Check.That(mk.Key).IsEqualTo(mkOriginal);
        Check.That(sk.Key).IsEqualTo(skOriginal);
        Check.That(mkCopy).IsNotEqualTo(mk.Key);
        Check.That(skCopy).IsNotEqualTo(sk.Key);
    }

    [Test]
    public void CanSign_Property_Returns_Correct_Value()
    {
        var managerKeyWithPrivate = new ManagerKey(Cert!, Id1);
        var ownerKeyWithPrivate = new OwnerKey(Cert!, Id1);

        var publicCertBytes = Cert!.Export(X509ContentType.Cert);
        using var publicCert = X509CertificateLoader.LoadCertificate(publicCertBytes);

        var managerKeyWithoutPrivate = new ManagerKey(publicCert, Id2);
        var ownerKeyWithoutPrivate = new OwnerKey(publicCert, Id2);

        Check.That(managerKeyWithPrivate.CanSign).IsTrue();
        Check.That(ownerKeyWithPrivate.CanSign).IsTrue();

        Check.That(managerKeyWithoutPrivate.CanSign).IsFalse();
        Check.That(ownerKeyWithoutPrivate.CanSign).IsFalse();
    }

    [Test]
    public void ToFile_FromFile_Roundtrip_AllSecurityKeyTypes()
    {
        static Boolean RoundtripToFile<TResult>(TResult key) where TResult : SecurityKey
        {
            var path = Path.GetTempFileName() + ".key";

            try
            {
                if(key.ToFile(path) is false) { return false; }

                var loaded = SecurityKey.FromFile<TResult>(path);

                Check.That(key.Equals(loaded)).IsTrue();

                return true;
            }
            finally
            {
                try { if(File.Exists(path)) { File.Delete(path); } } catch {}
            }
        }

        Check.That(RoundtripToFile(new OwnerKey(Cert!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new ManagerKey(Cert!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new ClientKey(KeyData1!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new CommandKey(KeyData1!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new ExecutiveKey(KeyData1!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new HostKey(KeyData1!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new MyHostKey(KeyData1!, Id1))).IsTrue();
        Check.That(RoundtripToFile(new ServiceKey(KeyData1!, Id1))).IsTrue();
    }

    [Test]
    public void Serialize_Deserialize_Roundtrip_ManagerKey_And_ServiceKey()
    {
        var mk = new ManagerKey(Cert!, Id1);
        var sk = new ServiceKey(KeyData1!, Id1);

        var mkBytes = mk.Serialize();
        var skBytes = sk.Serialize();

        Assert.That(mkBytes, Is.Not.Null.And.Not.Empty);
        Assert.That(skBytes, Is.Not.Null.And.Not.Empty);

        var mkBack = SecurityKey.Deserialize<ManagerKey>(mkBytes);
        var skBack = SecurityKey.Deserialize<ServiceKey>(skBytes);

        Check.That(mkBack).IsNotNull().And.IsInstanceOf<ManagerKey>();
        Check.That(skBack).IsNotNull().And.IsInstanceOf<ServiceKey>();

        Check.That(mkBack!.ID).IsEqualTo(mk.ID);
        Check.That(skBack!.ID).IsEqualTo(sk.ID);
        Check.That(mkBack.Key).IsEqualTo(mk.Key);
        Check.That(skBack.Key).IsEqualTo(sk.Key);
    }

    private static void TestClearKeyForType<T>(Func<T> keyFactory , String keyName) where T : SecurityKey
    {
        var key = keyFactory();
        var keyBefore = key.Key;
        Assert.That(keyBefore, Is.Not.Null.And.Not.Empty);

        var result = key.ClearKey();
        Assert.That(result, Is.True);

        var keyAfter = key.Key;
        Assert.That(keyAfter, Is.Not.Null);
        Assert.That(keyAfter, Is.Empty);
    }

    [Test]
    public void With_Copy_Equals_For_All_SecurityKey_Types()
    {
        static void CheckWithEqualsCopy<TResult>(TResult key) where TResult : SecurityKey
        {
            var copy = key.Copy<TResult>();
            SecurityKey withCopy = key switch
            {
                OwnerKey value     => (SecurityKey)(value with { }),
                ManagerKey value   => (SecurityKey)(value with { }),
                ClientKey value    => (SecurityKey)(value with { }),
                CommandKey value   => (SecurityKey)(value with { }),
                ExecutiveKey value => (SecurityKey)(value with { }),
                HostKey value      => (SecurityKey)(value with { }),
                MyHostKey value    => (SecurityKey)(value with { }),
                ServiceKey value   => (SecurityKey)(value with { }),
                TokenKey value     => (SecurityKey)(value with { }),
                _                  => throw new InvalidOperationException($"Unsupported key type: {key.GetType().FullName}")
            };

            Check.That(copy).IsNotNull();
            Check.That(withCopy).IsEqualTo(copy);
            Check.That(withCopy.GetHashCode()).IsEqualTo(copy!.GetHashCode());
        }

        CheckWithEqualsCopy(new OwnerKey(Cert!, Id1));
        CheckWithEqualsCopy(new ManagerKey(Cert!, Id1));
        CheckWithEqualsCopy(new ClientKey(KeyData1!, Id1));
        CheckWithEqualsCopy(new CommandKey(KeyData1!, Id1));
        CheckWithEqualsCopy(new ExecutiveKey(KeyData1!, Id1));
        CheckWithEqualsCopy(new HostKey(KeyData1!, Id1));
        CheckWithEqualsCopy(new MyHostKey(KeyData1!, Id1));
        CheckWithEqualsCopy(new ServiceKey(KeyData1!, Id1));
        CheckWithEqualsCopy(new TokenKey(KeyData1!, Id1, TokenKeyType.Jwt));
    }

    [Test]
    public void ClearKey_SetsKeyToZeros()
    {
        TestClearKeyForType(() => new OwnerKey(Cert!, Id1), "OwnerKey");
        TestClearKeyForType(() => new ManagerKey(Cert!, Id1), "ManagerKey");
        TestClearKeyForType(() => new ClientKey(KeyData1!, Id1), "ClientKey");
        TestClearKeyForType(() => new CommandKey(KeyData1!, Id1), "CommandKey");
        TestClearKeyForType(() => new ExecutiveKey(KeyData1!, Id1), "ExecutiveKey");
        TestClearKeyForType(() => new HostKey(KeyData1!, Id1), "HostKey");
        TestClearKeyForType(() => new MyHostKey(KeyData1!, Id1), "MyHostKey");
        TestClearKeyForType(() => new ServiceKey(KeyData1!, Id1), "ServiceKey");
    }
}