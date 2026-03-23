namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class TypeProviderExam
{
    [Test]
    public void Fluent_Create_AndTryResolve_Succeeds()
    {
        TypeProvider p = TypeProvider.Create(typeof(Guid).AssemblyQualifiedName);

        Boolean ok = p.TryResolve();

        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void Fluent_WithType_AndTryResolve_Succeeds()
    {
        TypeProvider p = new TypeProvider().WithType(typeof(DateTimeOffset).FullName);

        Boolean ok = p.TryResolve();

        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(DateTimeOffset)));
    }

    [Test]
    public void TryResolve_FullName_FrameworkType_Succeeds()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(StringBuilder).FullName);

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value,Is.EqualTo(typeof(StringBuilder)));
    }

    [Test]
    public void TryResolve_AssemblyQualified_FrameworkType_Succeeds()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(Guid).AssemblyQualifiedName);

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void Resolve_ReturnsType_WhenResolvable()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(DateTimeOffset).FullName);

        Type? t = p.Resolve();

        Assert.That(set,Is.True);
        Assert.That(t,Is.Not.Null);
        Assert.That(t,Is.EqualTo(typeof(DateTimeOffset)));
    }

    [Test]
    public void TryResolve_WhenValueAlreadySet_FailsFast()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(Guid).AssemblyQualifiedName);
        Boolean ok0 = p.TryResolve();
        Boolean ok1 = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok0,Is.True);
        Assert.That(ok1,Is.False);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void Resolve_WhenValueAlreadySet_ReturnsCachedType()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(StringBuilder).AssemblyQualifiedName);
        Boolean ok = p.TryResolve();
        Type? t = p.Resolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(t,Is.EqualTo(typeof(StringBuilder)));
        Assert.That(p.Value,Is.EqualTo(typeof(StringBuilder)));
    }

    [Test]
    public void TryResolve_WithWrongTypeCase_Fails()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(Guid).FullName!.ToLowerInvariant());

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithAssemblySimpleName_AndWrongCase_Fails()
    {
        TypeProvider p = new();

        Boolean set = p.SetType($"{typeof(Guid).FullName}, {typeof(Guid).Assembly.GetName().Name!.ToLowerInvariant()}");

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithAssemblySimpleName_Succeeds_WhenExact()
    {
        TypeProvider p = new();

        AssemblyName a = typeof(Guid).Assembly.GetName();
        Boolean set = p.SetType($"{typeof(Guid).FullName}, {a.Name}");

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void TryResolve_WithWrongVersion_Fails()
    {
        TypeProvider p = new();

        AssemblyName a = typeof(Guid).Assembly.GetName();
        Version v = a.Version ?? new Version(1,0,0,0);
        Version w = new(v.Major + 1,v.Minor,v.Build < 0 ? 0 : v.Build,v.Revision < 0 ? 0 : v.Revision);

        Boolean set = p.SetType($"{typeof(Guid).FullName}, {a.Name}, Version={w}");

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithExactVersion_Succeeds()
    {
        TypeProvider p = new();

        AssemblyName a = typeof(Guid).Assembly.GetName();
        Boolean set = p.SetType($"{typeof(Guid).FullName}, {a.Name}, Version={a.Version}");

        Boolean ok = p.TryResolve();

        Assert.That(set,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void TryResolve_WithProbePath_AndAssemblyQualifiedName_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

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
    public void TryResolve_WithProbePath_AndWrongAssemblyCase_Fails()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setName = p.SetType($"{ReflectionAssemblyProbeHelper.TestTypeFullName}, {ReflectionAssemblyProbeHelper.TestAssemblySimpleName.ToLowerInvariant()}");
        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithProbePath_AndWrongAssemblyVersion_Fails()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        Assert.That(TryGetAssemblyName(path,out AssemblyName? a),Is.True);
        Assert.That(a,Is.Not.Null);

        Version v = a!.Version ?? new Version(1,0,0,0);
        Version w = new(v.Major + 1,v.Minor,v.Build < 0 ? 0 : v.Build,v.Revision < 0 ? 0 : v.Revision);

        TypeProvider p = new();

        Boolean setName = p.SetType($"{ReflectionAssemblyProbeHelper.TestTypeFullName}, {a.Name}, Version={w}");
        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithProbePath_AndFullName_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void Fluent_WithAssemblies_Chain_WithAioHelperType_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new TypeProvider()
            .WithType(ReflectionAssemblyProbeHelper.TestTypeFullName)
            .WithAssemblies([ path ]);

        Boolean ok = p.TryResolve();

        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void Clear_ClearsNameAndValue()
    {
        TypeProvider p = new();

        Boolean set = p.SetType(typeof(StringBuilder).AssemblyQualifiedName);
        Boolean resolved = p.TryResolve();
        Boolean clear = p.Clear();

        Assert.That(set,Is.True);
        Assert.That(resolved,Is.True);
        Assert.That(clear,Is.True);
        Assert.That(p.Name,Is.Null);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void Clear_PreservesDirectories_ForSubsequentResolve()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean set0 = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok0 = p.TryResolve();
        Boolean clear = p.Clear();
        Boolean set1 = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok1 = p.TryResolve();

        Assert.That(setAsm,Is.True);
        Assert.That(set0,Is.True);
        Assert.That(ok0,Is.True);
        Assert.That(clear,Is.True);
        Assert.That(set1,Is.True);
        Assert.That(ok1,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void Unload_ClearsValue_And_PreservesConfiguration()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setAsm = p.SetAssemblies([ path ]);
        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok0 = p.TryResolve();
        Boolean unload = p.Unload();
        Boolean ok1 = p.TryResolve();

        Assert.That(setAsm,Is.True);
        Assert.That(setName,Is.True);
        Assert.That(ok0,Is.True);
        Assert.That(unload,Is.True);
        Assert.That(p.Name,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
        Assert.That(ok1,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void TryResolve_WithConfiguredAssemblyName_Succeeds_WhenTypeNameOnly()
    {
        TypeProvider p = new();

        AssemblyName a = typeof(Guid).Assembly.GetName();
        Boolean setName = p.SetType(typeof(Guid).FullName);
        Boolean setAsm = p.SetAssemblyNames([ a ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void TryResolve_WithConfiguredAssemblyName_Succeeds_WhenAnyConfiguredIdentityMatches()
    {
        TypeProvider p = new();

        AssemblyName a0 = typeof(Guid).Assembly.GetName();
        AssemblyName a1 = new("Different.Assembly");
        Boolean setName = p.SetType(typeof(Guid).FullName);
        Boolean setAsm = p.SetAssemblyNames([ a0 , a1 ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void TryResolve_WithConfiguredAssemblyName_DoesNotOverride_NameIdentity()
    {
        TypeProvider p = new();

        AssemblyName wrong = typeof(StringBuilder).Assembly.GetName();
        Boolean setName = p.SetType(typeof(Guid).AssemblyQualifiedName);
        Boolean setAsm = p.SetAssemblyNames([ wrong ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void TryResolve_WithConfiguredAssemblyName_FindsMatch_WithoutBeingBlockedByOthers()
    {
        TypeProvider p = new();

        AssemblyName a0 = typeof(Guid).Assembly.GetName();
        AssemblyName a1 = new("Different.Assembly");
        Boolean setName = p.SetType(typeof(Guid).FullName);
        Boolean setAsm = p.SetAssemblyNames([ a0 , a1 ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void TryResolve_WithDirectory_AndFullName_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        String? directory = Path.GetDirectoryName(path);

        Assert.That(String.IsNullOrWhiteSpace(directory),Is.False);

        TypeProvider p = new();

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
    public void TryResolve_WithDirectory_AndWrongAssemblyCase_Fails()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        String? directory = Path.GetDirectoryName(path);

        Assert.That(String.IsNullOrWhiteSpace(directory),Is.False);

        TypeProvider p = new();

        Boolean setName = p.SetType($"{ReflectionAssemblyProbeHelper.TestTypeFullName}, {ReflectionAssemblyProbeHelper.TestAssemblySimpleName.ToLowerInvariant()}");
        Boolean setDir = p.SetDirectories([ directory! ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setDir,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void TryResolve_WithAssemblyStream_AndAssemblyQualifiedName_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setName = p.SetType(ReflectionAssemblyProbeHelper.TestAssemblyQualifiedTypeName);
        Boolean setAsm = p.SetAssemblies([ CreateAssemblyStream(path) ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void TryResolve_WithAssemblyStream_AndWrongAssemblyCase_Fails()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setName = p.SetType($"{ReflectionAssemblyProbeHelper.TestTypeFullName}, {ReflectionAssemblyProbeHelper.TestAssemblySimpleName.ToLowerInvariant()}");
        Boolean setAsm = p.SetAssemblies([ CreateAssemblyStream(path) ]);
        Boolean ok = p.TryResolve();

        Assert.That(setName,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.False);
        Assert.That(p.Value,Is.Null);
    }

    [Test]
    public void Clear_PreservesAssemblyStreams_ForSubsequentResolve()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        TypeProvider p = new();

        Boolean setAsm = p.SetAssemblies([ CreateAssemblyStream(path) ]);
        Boolean set0 = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok0 = p.TryResolve();
        Boolean clear = p.Clear();
        Boolean set1 = p.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok1 = p.TryResolve();

        Assert.That(setAsm,Is.True);
        Assert.That(set0,Is.True);
        Assert.That(ok0,Is.True);
        Assert.That(clear,Is.True);
        Assert.That(set1,Is.True);
        Assert.That(ok1,Is.True);
        Assert.That(p.Value,Is.Not.Null);
        Assert.That(p.Value!.FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void SetContext_WithProbePath_LoadsIntoSuppliedContext()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = new();

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
    public void WithContext_PreservesSuppliedContext_ForSubsequentResolve()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = WithContext(new TypeProvider(),c)
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
    public void SetContext_WithNull_Fails()
    {
        TypeProvider p = new();

        Boolean ok = p.SetContext(null);

        Assert.That(ok,Is.False);
    }

    [Test]
    public void WithContext_ReturnsCurrentProvider()
    {
        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = new();

        TypeProvider result = WithContext(p,c);

        Assert.That(result,Is.SameAs(p));
    }

    [Test]
    public void SetContext_WhenValueAlreadySet_Fails()
    {
        TypeProvider p = new();

        Boolean setName = p.SetType(typeof(Guid).AssemblyQualifiedName);
        Boolean resolved = p.TryResolve();
        Boolean setContext = SetContext(p,CreateCollectibleContext());

        Assert.That(setName,Is.True);
        Assert.That(resolved,Is.True);
        Assert.That(setContext,Is.False);
    }

    [Test]
    public void Unload_WithSuppliedContext_FailsFast()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        TypeProvider p = new();

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

    private static Boolean TryGetAssemblyName(String path , out AssemblyName? assemblyname)
    {
        assemblyname = null;

        try { assemblyname = AssemblyName.GetAssemblyName(path); return assemblyname is not null; }

        catch { assemblyname = null; return false; }
    }

    private static MemoryStream CreateAssemblyStream(String path)
    {
        Byte[] b = File.ReadAllBytes(path);

        return new MemoryStream(b,writable:false);
    }

    private static Boolean SetContext(TypeProvider provider , AssemblyLoadContext context)
    {
        try
        {
            MethodInfo? m = typeof(TypeProvider).GetMethod("SetContext",BindingFlags.Public | BindingFlags.Instance);

            return m?.Invoke(provider,[ context ]) as Boolean? ?? false;
        }
        catch { return false; }
    }

    private static TypeProvider WithContext(TypeProvider provider , AssemblyLoadContext context)
    {
        try
        {
            MethodInfo? m = typeof(TypeProvider).GetMethod("WithContext",BindingFlags.Public | BindingFlags.Instance);

            return m?.Invoke(provider,[ context ]) as TypeProvider ?? provider;
        }
        catch { return provider; }
    }

    private static AssemblyLoadContext CreateCollectibleContext()
    {
        return new($"TypeProviderExam:{Guid.NewGuid():N}",isCollectible:true);
    }

    private static AssemblyLoadContext? GetLoadContext(Assembly assembly)
    {
        try
        {
            return AssemblyLoadContext.GetLoadContext(assembly);
        }
        catch { return null; }
    }
}
