namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class AccessRequestExams
{
    [OneTimeSetUp]
    public void Calibrate() { }

    [Test]
    public void StandardRequest_Roundtrip()
    {
        var data = Guid.NewGuid().ToString();

        var original = new StandardRequest(data);

        var json = original.ToString();
        var roundtrip = StandardRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"StandardRequest\"");
        Check.That(json).Contains($"\"Data\":\"{data}\"");
        Check.That(roundtrip).IsNotNull();
        Check.That(original.Data!.AsSpan().SequenceEqual(roundtrip!.Data!.AsSpan())).IsTrue();
    }

    [Test]
    public void ManagementRequest_Roundtrip()
    {
        var data = Guid.NewGuid().ToString();

        var original = new ManagementRequest(data);

        var json = original.ToString();
        var roundtrip = ManagementRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ManagementRequest\"");
        Check.That(json).Contains($"\"Data\":\"{data}\"");
        Check.That(roundtrip).IsNotNull();
        Check.That(original.Data!.AsSpan().SequenceEqual(roundtrip!.Data!.AsSpan())).IsTrue();
    }

    [Test]
    public void ServiceRequest_Roundtrip()
    {
        var data = Guid.NewGuid().ToString();

        var original = new ServiceRequest(null,data);

        var json = original.ToString();
        var roundtrip = ServiceRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ServiceRequest\"");
        Check.That(json).Contains($"\"Data\":\"{data}\"");
        Check.That(json).DoesNotContain("Tool");
        Check.That(roundtrip).IsNotNull();
        Check.That(original.Data!.AsSpan().SequenceEqual(roundtrip!.Data!.AsSpan())).IsTrue();
        Check.That(roundtrip!.Tool).IsNull();
    }

    [Test]
    public void AccessRequest_ParseAny_Roundtrip_Standard()
    {
        var data = Guid.NewGuid().ToString();

        var original = new StandardRequest(data);

        var roundtrip = AccessRequest.Parse(original.ToString());

        Check.That(roundtrip).IsInstanceOf<StandardRequest>();
        Check.That(original.Data!.AsSpan().SequenceEqual(roundtrip!.Data!.AsSpan())).IsTrue();
    }

    [Test]
    public void AccessRequest_ParseAny_Roundtrip_Management()
    {
        var data = Guid.NewGuid().ToString();

        var original = new ManagementRequest(data);

        var roundtrip = AccessRequest.Parse(original.ToString());

        Check.That(roundtrip).IsInstanceOf<ManagementRequest>();
        Check.That(original.Data!.AsSpan().SequenceEqual(roundtrip!.Data!.AsSpan())).IsTrue();
    }

    [Test]
    public void AccessRequest_ParseAny_Roundtrip_Service()
    {
        var data = Guid.NewGuid().ToString();

        var original = new ServiceRequest(null,data);

        var roundtrip = AccessRequest.Parse(original.ToString());

        Check.That(roundtrip).IsInstanceOf<ServiceRequest>();
        Check.That(original.Data!.AsSpan().SequenceEqual(roundtrip!.Data!.AsSpan())).IsTrue();
    }

    [Test]
    public void AccessRequest_ParseAny_Rejects_HostRequest()
    {
        var request = new HostRequest(null,true) { Data = Guid.NewGuid().ToString() };

        var roundtrip = AccessRequest.Parse(request.ToString());

        Check.That(roundtrip).IsInstanceOf<HostRequest>();
    }

    [Test]
    public void AccessRequest_KusDepotCab_Roundtrip()
    {
        var original = new ManagementRequest(Guid.NewGuid().ToString());

        var cab = original.ToKusDepotCab();
        var json = cab!.ToString();
        var parsed = KusDepotCab.Parse(json)!;
        var roundtrip = parsed.GetAccessRequest();

        Check.That(roundtrip).IsInstanceOf<ManagementRequest>();
        Check.That(roundtrip!.Data).IsEqualTo(original.Data);
    }
}
