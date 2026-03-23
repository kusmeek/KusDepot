namespace KusDepot.Exams;

public partial class TypeProviderExam
{
    [Test]
    public void TryResolve_Isolated_WithFrameworkType_AndNoProbeSources_Fails()
    {
        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean set = p.SetType(typeof(Guid).AssemblyQualifiedName);
        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_Isolated_WithFrameworkTypeFullName_AndNoProbeSources_Fails()
    {
        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean set = p.SetType(typeof(Guid).FullName);
        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_Isolated_WithNoProbeSources_AndNoContext_Fails()
    {
        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean set = p.SetType(ReflectionAssemblyProbeHelper.TestAssemblyQualifiedTypeName);
        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithProbePath_AndAssemblyQualifiedName_Succeeds_InIsolatedMode()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestAssemblyQualifiedTypeName);
        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
        Assert.That(p.Value,Is.Not.EqualTo(Type.GetType(ReflectionAssemblyProbeHelper.TestAssemblyQualifiedTypeName,false,false)));
    }

    [Test]
    public void TryResolve_Isolated_WithDirectory_AndFullName_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        String? directory = Path.GetDirectoryName(path);

        Assert.That(String.IsNullOrWhiteSpace(directory),Is.False);

        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean setDir = p.SetDirectories([ directory! ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setDir,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void TryResolve_Isolated_WithAssemblyStream_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestAssemblyQualifiedTypeName);
        Boolean setAsm = p.SetAssemblies([ CreateAssemblyStream(path) ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
        Assert.That(p.Value,Is.Not.EqualTo(Type.GetType(ReflectionAssemblyProbeHelper.TestAssemblyQualifiedTypeName,false,false)));
    }

    [Test]
    public void SetContext_WithProbePath_LoadsIntoSuppliedContext_InIsolatedMode()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean setContext = SetContext(p,c);
        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean ok = p.TryResolve();

        Assert.That(setContext,Is.True);
        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(GetLoadContext(p.Value!.Assembly),Is.SameAs(c));
    }

    [Test]
    public void WithContext_PreservesSuppliedContext_ForSubsequentResolve_InIsolatedMode()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = WithContext(new TypeProvider(TypeResolutionMode.Isolated),c)
            .WithAssemblies([ path ]);

        Boolean set0 = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok0 = p.TryResolve();
        Boolean clear = p.Clear();
        Boolean set1 = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok1 = p.TryResolve();

        Assert.That(set0,Is.True);
        Assert.That(ok0,Is.True);
        Assert.That(clear,Is.True);
        Assert.That(set1,Is.True);
        Assert.That(ok1,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(GetLoadContext(p.Value!.Assembly),Is.SameAs(c));
    }

    [Test]
    public void Unload_WithSuppliedContext_FailsFast_InIsolatedMode()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = new(TypeResolutionMode.Isolated);

        Boolean setContext = SetContext(p,c);
        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean ok = p.TryResolve();
        Boolean unload = p.Unload();

        Assert.That(setContext,Is.True);
        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(unload,Is.False);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(GetLoadContext(p.Value!.Assembly),Is.SameAs(c));
    }
}
