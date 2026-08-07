namespace KusDepot.Exams.Security;

[TestFixture] [NonParallelizable]
public class ToolManifestRegistryExam
{
    private readonly List<String> schemaids = new();
    private readonly List<Type> boundtypes = new();
    private ManagementKey? unlockkey;

    private sealed class RegistryToolA : Tool {}
    private sealed class RegistryToolB : Tool {}
    private sealed class DerivedToolHost : ToolHost {}
    private sealed class DerivedToolGenericHost : ToolGenericHost {}
    private sealed class DerivedToolWebHost : ToolWebHost {}
    private sealed class DerivedToolAspireHost : ToolAspireHost {}

    private static ToolManifest CreateManifest(String toolschemaid , String version = "1.0") =>
        ToolManifest.Create(toolschemaid,null,version,null,
        [
            new ToolOperationDescriptor { Index = 2 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." },
            new ToolOperationDescriptor { Index = 7 , MethodName = nameof(ITool.GetToolManifest) , Description = "Gets the tool manifest." }
        ]).ComputeManifestHash();

    private static ToolManifest CreateManifest(String toolschemaid , Type tooltype , params ToolOperationDescriptor[] operations) =>
        ToolManifest.Create(toolschemaid,null,"1.0",tooltype.FullName,operations).ComputeManifestHash();

    private String TrackSchemaID()
    {
        String toolschemaid = $"KusDepot.Tests/Registry/{Guid.NewGuid():N}";
        schemaids.Add(toolschemaid);
        return toolschemaid;
    }

    private void TrackBinding(Type tooltype)
    {
        if(boundtypes.Contains(tooltype) is false) { boundtypes.Add(tooltype); }
    }

    private void AssertDerivedToolTypeRegistryBinding(Type tooltype , ToolManifest manifest , params (String MethodName,Int32 Index)[] expectedOperations)
    {
        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(tooltype,manifest.ToolSchemaID)).IsTrue();
        TrackBinding(tooltype);

        Check.That(ToolManifestRegistry.TryGetBoundSchemaID(tooltype,out String boundSchemaID)).IsTrue();
        Check.That(boundSchemaID).IsEqualTo(manifest.ToolSchemaID);

        Check.That(ToolManifestRegistry.TryGetManifest(tooltype,out ToolManifest boundManifest)).IsTrue();
        Check.That(boundManifest.ManifestHash).IsEqualTo(manifest.ManifestHash);

        Check.That(ToolManifestRegistry.TryGetBindings(manifest.ToolSchemaID,out ImmutableArray<Type> bindings)).IsTrue();
        Check.That(bindings.Any(_ => _ == tooltype)).IsTrue();

        foreach((String methodName,Int32 index) in expectedOperations)
        {
            Check.That(ToolManifestRegistry.TryResolveOperation(tooltype,methodName,out Int32 resolvedIndex)).IsTrue();
            Check.That(resolvedIndex).IsEqualTo(index);
        }
    }

    private static OwnerKey CreateManagementKey() => new OwnerKey(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"RegistryLock"))!);

    [TearDown]
    public void Cleanup()
    {
        if(unlockkey is not null) { _ = ToolManifestRegistry.UnLock(unlockkey); unlockkey = null; }

        foreach(Type tooltype in boundtypes)
        {
            _ = ToolManifestRegistry.TryUnBind(tooltype);
        }

        foreach(String toolschemaid in schemaids)
        {
            _ = ToolManifestRegistry.TryUnRegister(toolschemaid);
        }

        boundtypes.Clear(); schemaids.Clear();
    }

    [Test]
    public void TryRegister_Registers_Manifest_And_Rejects_DuplicateSchema()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsFalse();
        Check.That(ToolManifestRegistry.TryGetManifest(toolschemaid,out ToolManifest registered)).IsTrue();
        Check.That(registered.ToolSchemaID).IsEqualTo(toolschemaid);
    }

    [Test]
    public void TryBind_And_HelperLookups_Work()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolA),toolschemaid)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolB),toolschemaid)).IsTrue();
        TrackBinding(typeof(RegistryToolA)); TrackBinding(typeof(RegistryToolB));

        Check.That(ToolManifestRegistry.TryGetBoundSchemaID(typeof(RegistryToolA),out String boundSchemaID)).IsTrue();
        Check.That(boundSchemaID).IsEqualTo(toolschemaid);

        Check.That(ToolManifestRegistry.TryGetBindings(toolschemaid,out ImmutableArray<Type> tooltypes)).IsTrue();
        Check.That(tooltypes.Any(_ => _ == typeof(RegistryToolA))).IsTrue();
        Check.That(tooltypes.Any(_ => _ == typeof(RegistryToolB))).IsTrue();

        Check.That(ToolManifestRegistry.TryGetCurrentManifest(typeof(RegistryToolA),out ToolManifest currentManifest)).IsTrue();
        Check.That(currentManifest.ToolSchemaID).IsEqualTo(toolschemaid);

        Check.That(ToolManifestRegistry.TryGetManifest(typeof(RegistryToolB),out ToolManifest boundManifest)).IsTrue();
        Check.That(boundManifest.ManifestHash).IsEqualTo(manifest.ManifestHash);
    }

    [Test]
    public void TryResolveOperation_Resolves_BySchemaID_And_Type()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolA),toolschemaid)).IsTrue();
        TrackBinding(typeof(RegistryToolA));

        Check.That(ToolManifestRegistry.TryResolveOperation(toolschemaid,nameof(ITool.GetToolDescriptor),out Int32 schemaIndex)).IsTrue();
        Check.That(schemaIndex).IsEqualTo(2);

        Check.That(ToolManifestRegistry.TryResolveOperation(typeof(RegistryToolA),nameof(ITool.GetToolManifest),out Int32 typeIndex)).IsTrue();
        Check.That(typeIndex).IsEqualTo(7);
    }

    [Test]
    public void TryRegister_Allows_RepeatedDescriptors_With_SharedIndex()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = ToolManifest.Create(toolschemaid,null,"1.0",typeof(RegistryToolA).FullName,
        [
            new ToolOperationDescriptor { Index = 4096 , MethodName = "ReadPulse" , Description = "Reads the pulse." },
            new ToolOperationDescriptor { Index = 4096 , MethodName = "ReadPulseAsync" , Description = "Reads the pulse." }
        ]).ComputeManifestHash();

        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolA),toolschemaid)).IsTrue();
        TrackBinding(typeof(RegistryToolA));

        Check.That(ToolManifestRegistry.TryResolveOperation(toolschemaid,"ReadPulse",out Int32 syncIndex)).IsTrue();
        Check.That(syncIndex).IsEqualTo(4096);
        Check.That(ToolManifestRegistry.TryResolveOperation(typeof(RegistryToolA),"ReadPulseAsync",out Int32 asyncIndex)).IsTrue();
        Check.That(asyncIndex).IsEqualTo(4096);
    }

    [Test]
    public void TryRegisterAndBind_GenericManifestHelper_Registers_And_Binds()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.TryRegisterAndBind<RegistryToolA>(manifest)).IsTrue();
        TrackBinding(typeof(RegistryToolA));

        Check.That(ToolManifestRegistry.TryGetManifest(toolschemaid,out ToolManifest registered)).IsTrue();
        Check.That(registered.ToolSchemaID).IsEqualTo(toolschemaid);
        Check.That(ToolManifestRegistry.TryGetBoundSchemaID(typeof(RegistryToolA),out String boundSchemaID)).IsTrue();
        Check.That(boundSchemaID).IsEqualTo(toolschemaid);
    }

    [Test]
    public void TryBind_DerivedToolHost_Type_Works_With_ToolManifestRegistry()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(
            toolschemaid,
            typeof(DerivedToolHost),
            new ToolOperationDescriptor { Index = 17 , MethodName = nameof(ToolHost.GetServices) , Description = "Gets host services." });

        AssertDerivedToolTypeRegistryBinding(
            typeof(DerivedToolHost),
            manifest,
            (nameof(ToolHost.GetServices),17));
    }

    [Test]
    public void TryBind_DerivedToolGenericHost_Type_Works_With_ToolManifestRegistry()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(
            toolschemaid,
            typeof(DerivedToolGenericHost),
            new ToolOperationDescriptor { Index = 17 , MethodName = nameof(ToolHost.GetServices) , Description = "Gets host services." },
            new ToolOperationDescriptor { Index = 18 , MethodName = nameof(ToolGenericHost.GetManagedApplication) , Description = "Gets the managed generic host application." });

        AssertDerivedToolTypeRegistryBinding(
            typeof(DerivedToolGenericHost),
            manifest,
            (nameof(ToolHost.GetServices),17),
            (nameof(ToolGenericHost.GetManagedApplication),18));
    }

    [Test]
    public void TryBind_DerivedToolWebHost_Type_Works_With_ToolManifestRegistry()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(
            toolschemaid,
            typeof(DerivedToolWebHost),
            new ToolOperationDescriptor { Index = 17 , MethodName = nameof(ToolHost.GetServices) , Description = "Gets host services." },
            new ToolOperationDescriptor { Index = 19 , MethodName = nameof(ToolWebHost.GetManagedApplication) , Description = "Gets the managed web host application." });

        AssertDerivedToolTypeRegistryBinding(
            typeof(DerivedToolWebHost),
            manifest,
            (nameof(ToolHost.GetServices),17),
            (nameof(ToolWebHost.GetManagedApplication),19));
    }

    [Test]
    public void TryBind_DerivedToolAspireHost_Type_Works_With_ToolManifestRegistry()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(
            toolschemaid,
            typeof(DerivedToolAspireHost),
            new ToolOperationDescriptor { Index = 17 , MethodName = nameof(ToolHost.GetServices) , Description = "Gets host services." },
            new ToolOperationDescriptor { Index = 20 , MethodName = nameof(ToolAspireHost.GetManagedApplication) , Description = "Gets the managed Aspire host application." });

        AssertDerivedToolTypeRegistryBinding(
            typeof(DerivedToolAspireHost),
            manifest,
            (nameof(ToolHost.GetServices),17),
            (nameof(ToolAspireHost.GetManagedApplication),20));
    }

    [Test]
    public void TryUnBind_Removes_TypeBinding()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolA),toolschemaid)).IsTrue();

        Check.That(ToolManifestRegistry.TryUnBind(typeof(RegistryToolA))).IsTrue();
        Check.That(ToolManifestRegistry.TryGetBoundSchemaID(typeof(RegistryToolA),out _)).IsFalse();
    }

    [Test]
    public void TryUnRegister_Removes_Manifest_And_Bindings()
    {
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolA),toolschemaid)).IsTrue();

        Check.That(ToolManifestRegistry.TryUnRegister(toolschemaid)).IsTrue();
        Check.That(ToolManifestRegistry.TryGetManifest(toolschemaid,out _)).IsFalse();
        Check.That(ToolManifestRegistry.TryGetBoundSchemaID(typeof(RegistryToolA),out _)).IsFalse();

        schemaids.Remove(toolschemaid);
    }

    [Test]
    public void Lock_And_UnLock_Gate_Writes()
    {
        unlockkey = CreateManagementKey();
        String toolschemaid = TrackSchemaID();
        ToolManifest manifest = CreateManifest(toolschemaid);

        Check.That(ToolManifestRegistry.GetLocked()).IsFalse();
        Check.That(ToolManifestRegistry.Lock(unlockkey)).IsTrue();
        Check.That(ToolManifestRegistry.GetLocked()).IsTrue();
        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsFalse();
        Check.That(ToolManifestRegistry.TryBind(typeof(RegistryToolA),toolschemaid)).IsFalse();
        Check.That(ToolManifestRegistry.UnLock(unlockkey)).IsTrue();
        unlockkey = null;
        Check.That(ToolManifestRegistry.GetLocked()).IsFalse();
        Check.That(ToolManifestRegistry.TryRegister(manifest)).IsTrue();
    }
}
