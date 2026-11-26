namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolSecurityAsyncExam
{
    private ManagerKey? KeyM;

    private OwnerKey? KeyO;

    [OneTimeSetUp]
    public void Calibrate()
    {
        KeyM = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Management"))!);
        KeyO = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Owner"))!);
    }

    [Test]
    public async Task RequestAccessAsync_And_RevokeAccessAsync()
    {
        Tool tool = new();

        Check.That(tool.TakeOwnership(KeyO)).IsTrue();

        ClientKey? clientKey = await tool.RequestAccessAsync(new StandardRequest("RequestAccessAsync")).ConfigureAwait(false) as ClientKey;
        Check.That(clientKey).IsNotNull();

        Check.That(await tool.RevokeAccessAsync(new ClientKey(Array.Empty<Byte>())).ConfigureAwait(false)).IsFalse();
        Check.That(await tool.RevokeAccessAsync(clientKey).ConfigureAwait(false)).IsTrue();
    }
}
