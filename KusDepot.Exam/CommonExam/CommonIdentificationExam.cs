namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CommonIdentificationExam
{
    private X509Certificate2? certificateA;
    private X509Certificate2? certificateB;

    [OneTimeSetUp]
    public void Setup()
    {
        certificateA = CreateCertificate(Guid.NewGuid(),"CommonIdentificationA",2048,2,null);
        certificateB = CreateCertificate(Guid.NewGuid(),"CommonIdentificationB",2048,2,null);
    }

    [Test]
    public void TryCreate_And_TryRead_Roundtrip()
    {
        DateTimeOffset createdat = UtcSecondNow();
        CommonIdentificationData data = CommonIdentificationData.Create(
            issuerobjectid: Guid.NewGuid(),
            purpose: "common.identification/test",
            thumbprint: certificateA!.Thumbprint!,
            subjectobjectid: Guid.NewGuid(),
            createdat: createdat,
            notbefore: createdat,
            expiresat: createdat.AddMinutes(5));

        Boolean created = CommonIdentificationFactory.TryCreate(data,certificateA,out Byte[]? identification);
        Boolean read = CommonIdentificationFactory.TryRead(identification!,out CommonIdentification? artifact);

        Check.That(created).IsTrue();
        Check.That(read).IsTrue();
        Check.That(artifact).IsNotNull();
        Check.That(artifact!.Signature.Length).IsStrictlyGreaterThan(0);
        Check.That(artifact.Data).IsEqualTo(data);
    }

    [Test]
    public void TryVerify_WithCertificate_Succeeds()
    {
        DateTimeOffset createdat = UtcSecondNow();
        CommonIdentificationData data = CommonIdentificationData.Create(
            issuerobjectid: Guid.NewGuid(),
            purpose: "common.identification/verify",
            thumbprint: certificateA!.Thumbprint!,
            createdat: createdat,
            notbefore: createdat,
            expiresat: createdat.AddMinutes(5));
        Check.That(CommonIdentificationFactory.TryCreate(data,certificateA,out Byte[]? identification)).IsTrue();

        Boolean verified = CommonIdentificationVerifier.TryVerify(identification!,certificateA,createdat.AddMinutes(1),out CommonIdentificationData verifieddata);

        Check.That(verified).IsTrue();
        Check.That(verifieddata).IsEqualTo(data);
    }

    [Test]
    public void TryVerify_WithResolver_Succeeds()
    {
        Guid issuerobjectid = Guid.NewGuid();
        DateTimeOffset createdat = UtcSecondNow();
        CommonIdentificationData data = CommonIdentificationData.Create(
            issuerobjectid: issuerobjectid,
            purpose: "common.identification/resolver",
            thumbprint: certificateA!.Thumbprint!,
            createdat: createdat,
            notbefore: createdat,
            expiresat: createdat.AddMinutes(5));
        Check.That(CommonIdentificationFactory.TryCreate(data,certificateA,out Byte[]? identification)).IsTrue();

        Boolean verified = CommonIdentificationVerifier.TryVerify(identification!,id => id == issuerobjectid ? certificateA : null,createdat.AddMinutes(1),out CommonIdentificationData verifieddata);

        Check.That(verified).IsTrue();
        Check.That(verifieddata).IsEqualTo(data);
    }

    [Test]
    public void TryVerify_Fails_When_NotYetValid()
    {
        DateTimeOffset createdat = UtcSecondNow();
        CommonIdentificationData data = CommonIdentificationData.Create(
            issuerobjectid: Guid.NewGuid(),
            purpose: "common.identification/not-before",
            thumbprint: certificateA!.Thumbprint!,
            createdat: createdat,
            notbefore: createdat.AddMinutes(5));
        Check.That(CommonIdentificationFactory.TryCreate(data,certificateA,out Byte[]? identification)).IsTrue();

        Boolean verified = CommonIdentificationVerifier.TryVerify(identification!,certificateA,createdat.AddMinutes(1),out _);

        Check.That(verified).IsFalse();
    }

    [Test]
    public void TryVerify_Fails_After_Expiration_And_Succeeds_Without_Expiration()
    {
        DateTimeOffset createdat = UtcSecondNow();
        CommonIdentificationData expiringdata = CommonIdentificationData.Create(
            issuerobjectid: Guid.NewGuid(),
            purpose: "common.identification/expiry",
            thumbprint: certificateA!.Thumbprint!,
            createdat: createdat,
            notbefore: createdat,
            expiresat: createdat.AddMinutes(1));
        CommonIdentificationData nonexpiringdata = CommonIdentificationData.Create(
            issuerobjectid: Guid.NewGuid(),
            purpose: "common.identification/no-expiry",
            thumbprint: certificateA!.Thumbprint!,
            createdat: createdat,
            notbefore: createdat,
            expiresat: null);
        Check.That(CommonIdentificationFactory.TryCreate(expiringdata,certificateA,out Byte[]? expiringidentification)).IsTrue();
        Check.That(CommonIdentificationFactory.TryCreate(nonexpiringdata,certificateA,out Byte[]? nonexpiringidentification)).IsTrue();

        Boolean expiredverified = CommonIdentificationVerifier.TryVerify(expiringidentification!,certificateA,createdat.AddMinutes(2),out _);
        Boolean nonexpiredverified = CommonIdentificationVerifier.TryVerify(nonexpiringidentification!,certificateA,createdat.AddMinutes(2),out CommonIdentificationData verifieddata);
        Boolean wrongcertificateverified = CommonIdentificationVerifier.TryVerify(nonexpiringidentification!,certificateB,createdat.AddMinutes(2),out _);

        Check.That(expiredverified).IsFalse();
        Check.That(nonexpiredverified).IsTrue();
        Check.That(verifieddata).IsEqualTo(nonexpiringdata);
        Check.That(wrongcertificateverified).IsFalse();
    }

    private static DateTimeOffset UtcSecondNow()
    {
        return DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }
}
