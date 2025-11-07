namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CommonSecretExam
{
    private X509Certificate2? certA;
    private X509Certificate2? certB;
    private Guid objectId;

    [OneTimeSetUp]
    public void Setup()
    {
        certA = CreateCertificate(Guid.NewGuid(), "CommonSecretsExamA", 2048, 2, null);
        certB = CreateCertificate(Guid.NewGuid(), "CommonSecretsExamB", 2048, 2, null);
        objectId = Guid.NewGuid();
        Check.That(certA).IsNotNull();
        Check.That(certB).IsNotNull();
    }

    /// <summary>
    /// Should create a valid secret with correct version and non-empty raw bytes.
    /// </summary>
    [Test]
    public void CreateV1_Success()
    {
        var secret = CommonSecret.CreateV1(certA, objectId);
        Check.That(secret).IsNotNull();
        var bytes = secret!.GetBytes();
        Check.That(bytes.Length).IsStrictlyGreaterThan(0);
        Check.That(secret.Version).IsEqualTo(1);
        Check.That(CommonSecret.TryValidate(bytes.ToArray(), certA, objectId)).IsTrue();
    }

    /// <summary>
    /// CreateV1 should return null if the certificate is null.
    /// </summary>
    [Test]
    public void CreateV1_NullCertificate_ReturnsNull()
    {
        var secret = CommonSecret.CreateV1(null, objectId);
        Check.That(secret).IsNull();
    }

    /// <summary>
    /// Should fail validation with wrong certificate, wrong objectid, or null/empty secret.
    /// </summary>
    [Test]
    public void TryValidateV1_FailureCases()
    {
        var secret = CommonSecret.CreateV1(certA, objectId);
        Check.That(secret).IsNotNull();

        ValidateAndCheckFailure(() => CommonSecret.TryValidate(secret!.GetBytes().ToArray(), certB, objectId));

        ValidateAndCheckFailure(() => CommonSecret.TryValidate(secret!.GetBytes().ToArray(), certA, Guid.NewGuid()));

        ValidateAndCheckFailure(() => CommonSecret.TryValidate(null, certA, objectId));

        ValidateAndCheckFailure(() => CommonSecret.TryValidate(Array.Empty<Byte>(), certA, objectId));
    }

    /// <summary>
    /// Should fail validation if any part of the secret is tampered with.
    /// </summary>
    [TestCase(0)]   // Version
    [TestCase(2)]   // RSA Key Length
    [TestCase(4)]   // Nonce
    [TestCase(20)]  // Ciphertext
    [TestCase(-2)]  // Tag
    public void TryValidateV1_Tampered_Fails(Int32 offset)
    {
        var secret = CommonSecret.CreateV1(certA, objectId);
        Check.That(secret).IsNotNull();
        var original = secret!.GetBytes().ToArray();
        var tampered = new Byte[original.Length];
        Array.Copy(original, tampered, original.Length);

        Int32 index = offset >= 0 ? offset : tampered.Length + offset;
        if (index >= 0 && index < tampered.Length)
        {
            tampered[index] ^= 0x5A; // Flip some bits
        }

        ValidateAndCheckFailure(() => CommonSecret.TryValidate(tampered, certA, objectId));
    }

    /// <summary>
    /// Version property should return the correct version.
    /// </summary>
    [Test]
    public void Version_Property()
    {
        var secret = CommonSecret.CreateV1(certA, objectId);
        Check.That(secret).IsNotNull();
        Check.That(secret!.Version).IsEqualTo(1);
    }

    private static void ValidateAndCheckFailure(Func<Boolean> validationFunc)
    {
        Boolean caught = false;
        Boolean? result = null;
        try
        {
            result = validationFunc();
        }
        catch (Exception ex)
        {
            caught = ex is CryptographicException;
            if (!caught) throw;
        }
        Check.That(caught || result == false).IsTrue();
    }
}