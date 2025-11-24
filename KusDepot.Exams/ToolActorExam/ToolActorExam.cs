namespace KusDepot.FabricExams.Actors;

[TestFixture] [Parallelizable(ParallelScope.Self)]
public class ToolActorExam
{
    private IToolActor? Proxy;
    private ActorId? ActorId;
    private AccessKey? Key;
    private Guid? Oid;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.ActorId = new(Guid.NewGuid()); this.Proxy = ActorProxy.Create<IToolActor>(ActorId,ServiceLocators.ToolActorService);
    }

    [OneTimeTearDown]
    public void Complete()
    {
        ActorServiceProxy.Create(ServiceLocators.ToolActorService,ActorId!).DeleteActorAsync(ActorId,CancellationToken.None).GetAwaiter().GetResult();
    }

    [Test] [Order(1)]
    public async Task RequestAccess()
    {
        Key = (await this.Proxy!.RequestAccess(new StandardRequest("RequestAccess"))) as ClientKey;

        Check.That(Key).IsNotNull();
    }

    [Test] [Order(2)]
    public async Task ExecuteCommand()
    {
        Oid = await this.Proxy!.ExecuteCommand(new CommandDetails().SetHandle("CommandTest"),Key);

        Check.That(Oid).HasAValue().Which.HasDifferentValueThan(Guid.Empty);
    }

    [Test] [Order(3)]
    public async Task ExecuteCommandCab()
    {
        Oid = await this.Proxy!.ExecuteCommandCab(new CommandDetails().SetHandle("CommandTest").ToKusDepotCab(),Key);

        Check.That(Oid).HasAValue().Which.HasDifferentValueThan(Guid.Empty);
    }

    [Test] [Order(4)]
    public async Task GetOutput()
    {
        Oid = await this.Proxy!.ExecuteCommand(new CommandDetails().SetHandle("CommandTest"),Key);

        Check.That(await this.Proxy!.GetOutput(Oid,Key)).IsNotNull().And.IsInstanceOf<Guid>().Which.HasDifferentValueThan(Guid.Empty);
    }

    [Test] [Order(5)]
    public async Task RevokeAccess()
    {
        Check.That(await this.Proxy!.RevokeAccess(Key)).IsTrue();
    }
}