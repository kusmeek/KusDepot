namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class AccessRequestExams
{
    [OneTimeSetUp]
    public void Calibrate() { }

    [Test]
    public void StandardRequest_Roundtrip()
    {
        var subject = Guid.NewGuid().ToString();

        var original = new StandardRequest(subject);

        var json = original.ToString();
        var roundtrip = StandardRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"StandardRequest\"");
        Check.That(json).Contains($"\"Subject\":\"{subject}\"");
        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void ManagementRequest_Roundtrip()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var key = new ManagerKey(cert!, Guid.NewGuid());

        var original = new ManagementRequest(key);

        var json = original.ToString();
        var roundtrip = ManagementRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ManagementRequest\"");
        Check.That(json).Contains("\"Credential\"");
        Check.That(json).Contains("\"Key\":\"");
        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Credential).IsEqualTo(original.Credential);
        Check.That(roundtrip.Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void ServiceRequest_Roundtrip()
    {
        var subject = Guid.NewGuid().ToString();

        var original = new ServiceRequest(null,subject);

        var json = original.ToString();
        var roundtrip = ServiceRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ServiceRequest\"");
        Check.That(json).Contains($"\"Subject\":\"{subject}\"");
        Check.That(json).DoesNotContain("Tool");
        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Subject).IsEqualTo(original.Subject);
        Check.That(roundtrip!.Tool).IsNull();
    }

    [Test]
    public void AccessRequest_ParseAny_Roundtrip_Standard()
    {
        var subject = Guid.NewGuid().ToString();

        var original = new StandardRequest(subject);

        var roundtrip = AccessRequest.Parse(original.ToString());

        Check.That(roundtrip).IsInstanceOf<StandardRequest>();
        Check.That(roundtrip!.Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void AccessRequest_ParseAny_Roundtrip_Management()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var original = new ManagementRequest(new ManagerKey(cert!, Guid.NewGuid()));

        var roundtrip = AccessRequest.Parse(original.ToString());

        Check.That(roundtrip).IsInstanceOf<ManagementRequest>();
        Check.That(roundtrip!.Credential).IsEqualTo(original.Credential);
        Check.That(((ManagementRequest)roundtrip).Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void AccessRequest_ParseAny_Roundtrip_Service()
    {
        var subject = Guid.NewGuid().ToString();

        var original = new ServiceRequest(null,subject);

        var roundtrip = AccessRequest.Parse(original.ToString());

        Check.That(roundtrip).IsInstanceOf<ServiceRequest>();
        Check.That(roundtrip!.Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void AccessRequest_ParseAny_Rejects_HostRequest()
    {
        var request = new HostRequest(null,true) { Subject = Guid.NewGuid().ToString() };

        var roundtrip = AccessRequest.Parse(request.ToString());

        Check.That(roundtrip).IsInstanceOf<HostRequest>();
    }

    [Test]
    public void AccessRequest_KusDepotCab_Roundtrip()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var original = new ManagementRequest(new ManagerKey(cert!, Guid.NewGuid()));

        var cab = original.ToKusDepotCab();
        var json = cab!.ToString();
        var parsed = KusDepotCab.Parse(json)!;
        var roundtrip = parsed.GetAccessRequest();

        Check.That(roundtrip).IsInstanceOf<ManagementRequest>();
        Check.That(roundtrip!.Credential).IsEqualTo(original.Credential);
        Check.That(((ManagementRequest)roundtrip).Subject).IsEqualTo(original.Subject);
    }
}
