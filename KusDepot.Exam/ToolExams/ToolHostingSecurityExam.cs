namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class ToolHostingSecurityExam
{
    private ITool? _hostTool;
    private ITool? _childTool;

    [SetUp]
    public void Setup()
    {
        _childTool = ToolBuilderFactory.CreateBuilder().Build();

        _hostTool = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(_childTool)
            .Seal().Build();
    }

    private static T? GetProtectedField<T>(Object obj,String fieldName) where T : class
    {
        return obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj) as T;
    }

    [Test]
    public void HostCanManageChildTool()
    {
        var hostingManagerKeys = GetProtectedField<Dictionary<ITool, ManagerKey>>(_hostTool!, "HostingManagerKeys");
        Check.That(hostingManagerKeys).IsNotNull();

        Check.That(_childTool!.GetLocked()).IsTrue();

        Boolean lockResult = _childTool.UnLock(hostingManagerKeys!.GetValueOrDefault(_childTool));

        Check.That(lockResult).IsTrue();
        Check.That(_childTool.GetLocked()).IsFalse();
    }

    [Test]
    public void HostStoresClonedMyHostKey()
    {
        var hostMyHostKeys = GetProtectedField<Dictionary<ITool, MyHostKey>>(_hostTool!, "HostingMyHostKeys");
        Check.That(hostMyHostKeys).IsNotNull();

        var childMyHostKey = GetProtectedField<AccessKey>(_childTool!, "MyHostKey");
        Check.That(childMyHostKey).IsNotNull();

        Check.That(ReferenceEquals(hostMyHostKeys![_childTool!], childMyHostKey)).IsFalse();
    }
}