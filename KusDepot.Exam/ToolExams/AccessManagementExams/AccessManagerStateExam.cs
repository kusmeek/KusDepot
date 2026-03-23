namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.None)]
internal class AccessManagerStateExam
{
    private AccessManagerState? State;
    private String? Subject;
    private AccessKeyToken Token;

    [OneTimeSetUp]
    public void SetUp()
    {
        this.Subject = "test-subject";

        this.Token = new AccessKeyToken(SHA512.HashData(Encoding.UTF8.GetBytes(this.Subject)));

        this.State = new AccessManagerState
        {
            ID = Guid.NewGuid(),
            AccessKeys = new Dictionary<String,HashSet<AccessKeyToken>>
            {
                { this.Subject, new HashSet<AccessKeyToken> { this.Token } }
            },
            Certificate = SerializeCertificate(CreateCertificate(Guid.NewGuid(),"AccessManagerStateExamCert"))
        };
    }

    [Test] [Order(1)]
    public void SerializeAndDeserialize()
    {
        Byte[]? serialized = this.State!.Serialize();
        Check.That(serialized).IsNotNull().And.Not.IsEmpty();

        AccessManagerState? deserialized = AccessManagerState.Deserialize(serialized);
        Check.That(deserialized).IsNotNull();

        Check.That(deserialized!.ID).IsEqualTo(this.State.ID);
        Check.That(deserialized.Certificate).IsNotNull().And.ContainsExactly(this.State.Certificate);
        Check.That(deserialized.AccessKeys).IsNotNull();
        Check.That(deserialized.AccessKeys!.Keys).Contains(this.Subject);
        Check.That(deserialized.AccessKeys[this.Subject!].SetEquals(new[] { this.Token })).IsTrue();
    }

    [Test] [Order(2)]
    public void Clear()
    {
        this.State!.Clear();

        Check.That(this.State.ID).IsEqualTo(Guid.Empty);
        Check.That(this.State.Certificate!.All(b => b == 0)).IsTrue();
        Check.That(this.State.AccessKeys).IsNull();
    }
}
