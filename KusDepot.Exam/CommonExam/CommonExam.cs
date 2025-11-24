namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class CommonExam
{
    private ManagerKey? KeyM; private ManagerKey? KeyX;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Management"))!);
        KeyX = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"debug"))!);

        if(KeyM is null || KeyX is null) { throw new InvalidOperationException(); }
    }

    private static List<Byte[]>? GetSecrets(Common obj)
    {
        return typeof(Common).GetField("Secrets",BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(obj) as List<Byte[]>;
    }

    private sealed class ByteArrayComparer : IEqualityComparer<Byte[]>
    {
        public Boolean Equals(Byte[]? x,Byte[]? y)
        {
            if (ReferenceEquals(x,y)) return true;
            if (x is null || y is null) return false;
            if (x.Length != y.Length) return false;
            return x.AsSpan().SequenceEqual(y);
        }
        public Int32 GetHashCode(Byte[] obj)
        {
            if (obj == null) return 0;
            var hash = new HashCode();
            hash.AddBytes(obj.AsSpan());
            return hash.ToHashCode();
        }
    }

    [Test]
    public void CheckManager()
    {
        CommonTest _ = new();

        Check.That(_.CheckManager(KeyM)).IsFalse();

        ManagerKey? KeyM2 = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM2).IsNotNull(); Check.That(_.CheckManager(KeyM2)).IsTrue();

        Check.That(_.RegisterManager(KeyM)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.Lock(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();
    }

    [Test]
    public void Constructor()
    {
        CommonTest _1 = new();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void CreateManagementKey()
    {
        CommonTest _ = new();

        Check.That(_.CheckManager(KeyM)).IsFalse();

        ManagerKey? KeyM2 = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM2).IsNotNull(); Check.That(_.CheckManager(KeyM2)).IsTrue();

        Check.That(_.RegisterManager(KeyM)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.Lock(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();
    }

    [Test]
    public void DisableMyExceptions()
    {
        CommonTest _ = new();

        Check.That(_.ExceptionsEnabled).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsFalse();
    }

    [Test]
    public void EnableMyExceptions()
    {
        CommonTest _ = new();

        Check.That(_.ExceptionsEnabled).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsFalse();
    }

    [Test]
    public void opEquality()
    {
        CommonTest _0 = new CommonTest();
        CommonTest _1 = new CommonTest();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new CommonTest() == null).IsFalse();
        Check.That(new CommonTest() == _1).IsFalse();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0 == _1).IsTrue();
    }

    [Test]
    public void opInequality()
    {
        CommonTest _0 = new CommonTest();
        CommonTest _1 = new CommonTest();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new CommonTest() != null).IsTrue();
        Check.That(new CommonTest() != _1).IsTrue();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0 != _1).IsFalse();
    }

    [Test]
    public void EqualsObject()
    {
        CommonTest _ = new();

        Check.That((Object)new CommonTest().Equals(null)).IsEqualTo(false);
        Check.That((Object)new CommonTest().Equals(new Object())).IsEqualTo(false);
        Check.That((Object)_.Equals(_)).IsEqualTo(true);
    }

    [Test]
    public void EqualsInterface()
    {
        CommonTest _0 = new CommonTest();
        CommonTest _1 = new CommonTest();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new CommonTest().Equals(null)).IsFalse();
        Check.That(new CommonTest().Equals(new Object())).IsFalse();
        Check.That(new CommonTest().Equals(_1)).IsFalse();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void GetID()
    {
        CommonTest _0 = new CommonTest();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.GetID()).Not.Equals(_1);
        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void GetLocked()
    {
        CommonTest _0 = new CommonTest();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyX)).IsFalse();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test]
    public void Initialize()
    {
        CommonTest _0 = new CommonTest();

        Check.That(_0.GetID()).HasAValue();
        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.Initialize()).IsTrue();
        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        CommonTest _0 = new();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyX)).IsFalse();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.SetID(Guid.NewGuid())).IsEqualTo(false);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();
    }

    [Test]
    public void MyExceptionsEnabled()
    {
        CommonTest _ = new();

        Check.That(_.MyExceptionsEnabled()).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.MyExceptionsEnabled()).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.MyExceptionsEnabled()).IsFalse();
    }

    [Test]
    public void RegisterManager()
    {
        CommonTest _ = new();

        Check.That(_.CheckManager(KeyM)).IsFalse();

        ManagerKey? KeyM2 = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM2).IsNotNull(); Check.That(_.CheckManager(KeyM2)).IsTrue();

        Check.That(_.RegisterManager(KeyM)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.Lock(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();
    }

    [Test]
    public void SetID()
    {
        CommonTest _0 = new();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void UnLock()
    {
        CommonTest _0 = new();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyX)).IsFalse();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.SetID(Guid.NewGuid())).IsEqualTo(false);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();
    }

    [Test]
    public void UnRegisterManager()
    {
        CommonTest _ = new();

        Check.That(_.CheckManager(KeyM)).IsFalse();

        ManagerKey? KeyM2 = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM2).IsNotNull(); Check.That(_.CheckManager(KeyM2)).IsTrue();

        Check.That(_.RegisterManager(KeyM)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.UnRegisterManager(KeyM2)).IsTrue();

        Check.That(_.CheckManager(KeyM2)).IsFalse();
    }

    [Test]
    public void Secret_CheckRemove_And_Uniqueness()
    {
        CommonTest obj = new(); List<ManagerKey> keys = new();
        for(Int32 i =0;i<5;i++) { var mk = obj.CreateManagementKey($"K{i}") as ManagerKey; Check.That(mk).IsNotNull(); keys.Add(mk!); }
        List<Byte[]>? secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull(); Check.That(secrets!.Count).IsEqualTo(5);
        Check.That(secrets.Count).IsEqualTo(secrets.Distinct(new ByteArrayComparer()).Count());
        Check.That(obj.UnRegisterManager(keys[0])).IsTrue();
        List<Byte[]>? secretsAfter = GetSecrets(obj);
        Check.That(secretsAfter).IsNotNull(); Check.That(secretsAfter!.Count).IsEqualTo(4);
        Check.That(obj.CheckManager(keys[0])).IsFalse();
        Check.That(obj.CheckManager(keys[1])).IsTrue();
    }

    [Test]
    public void CreateManagementKey_WithCertificate_Success()
    {
        CommonTest obj = new();
        using var cert = CreateCertificate(Guid.NewGuid(),"Test");

        ManagerKey? mk = obj.CreateManagementKey(cert) as ManagerKey;
        Check.That(mk).IsNotNull();
        Check.That(obj.CheckManager(mk)).IsTrue();
        List<Byte[]>? secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull();
        Check.That(secrets!.Count).IsEqualTo(1);
    }

    [Test]
    public void RegisterManager_WithCertificate_Success()
    {
        CommonTest obj = new();
        using var cert = CreateCertificate(Guid.NewGuid(),"Test");

        ManagerKey? mk = obj.RegisterManager(cert) as ManagerKey;
        Check.That(mk).IsNotNull();
        Check.That(obj.CheckManager(mk)).IsTrue();
        List<Byte[]>? secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull();
        Check.That(secrets!.Count).IsEqualTo(1);
    }

    [Test]
    public void RegisterManager_WithCertificate_Then_UnRegister_Removes()
    {
        CommonTest obj = new();
        using var cert = CreateCertificate(Guid.NewGuid(),"Test");

        ManagerKey? mk = obj.RegisterManager(cert) as ManagerKey; Check.That(mk).IsNotNull();
        Boolean removed = obj.UnRegisterManager(mk);
        Check.That(removed).IsTrue();
        Check.That(obj.CheckManager(mk)).IsFalse();
        List<Byte[]>? secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull();
        Check.That(secrets!.Count).IsEqualTo(0);
    }

    [Test]
    public void DestroySecrets()
    {
        CommonTest obj = new();
        Check.That(obj.RegisterManager(KeyM)).IsTrue();
        List<Byte[]>? secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull();
        Check.That(secrets!.Count).IsEqualTo(1);

        var destroySecretsMethod = typeof(Common).GetMethod("DestroySecrets", BindingFlags.NonPublic | BindingFlags.Instance);
        Check.That(destroySecretsMethod).IsNotNull();

        var resultFalse = (Boolean)destroySecretsMethod!.Invoke(obj, new Object?[] { KeyX })!;
        Check.That(resultFalse).IsFalse();
        secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull();
        Check.That(secrets!.Count).IsEqualTo(1);
        Check.That(obj.CheckManager(KeyM)).IsTrue();

        var resultTrue = (Boolean)destroySecretsMethod!.Invoke(obj, new Object?[] { KeyM })!;
        Check.That(resultTrue).IsTrue();
        secrets = GetSecrets(obj);
        Check.That(secrets).IsNotNull();
        Check.That(secrets!.Count).IsEqualTo(0);
        Check.That(obj.CheckManager(KeyM)).IsFalse();
    }
}