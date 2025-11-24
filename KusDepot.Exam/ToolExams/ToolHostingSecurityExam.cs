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
        var hostingManagerKey = GetProtectedField<ManagerKey>(_hostTool!, "HostingManagerKey");
        Check.That(hostingManagerKey).IsNotNull();

        Check.That(_childTool!.GetLocked()).IsTrue();

        Boolean lockResult = _childTool.UnLock(hostingManagerKey);

        Check.That(lockResult).IsTrue();
        Check.That(_childTool.GetLocked()).IsFalse();
    }

    [Test]
    public void ChildToolCannotManageHost()
    {
        var childsKeyForHost = GetProtectedField<AccessKey>(_childTool!, "MyHostKey");
        Check.That(childsKeyForHost).IsNotNull();

        Check.That(_hostTool!.GetLocked()).IsTrue();

        Boolean lockResult = _hostTool.UnLock(new ManagerKey(childsKeyForHost!.Key));

        Check.That(lockResult).IsFalse();
        Check.That(_hostTool.GetLocked()).IsTrue();
    }
}