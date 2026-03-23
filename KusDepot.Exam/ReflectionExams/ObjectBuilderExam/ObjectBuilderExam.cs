using System.Runtime.Loader;

namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ObjectBuilderExam
{
    [Test]
    public void Fluent_CreateType_AndWithArgument_Builds()
    {
        ObjectBuilder b = ObjectBuilder.Create(typeof(Version))
            .WithArgument(0,1)
            .WithArgument(1,2);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Version>());
        Assert.That(((Version)b.Value!).ToString(),Is.EqualTo("1.2"));
    }

    [Test]
    public void Build_SetAssemblies_AioAssemblyType_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectBuilder b = new();

        Boolean setAsm = b.Provider.SetAssemblies([ path ]);
        Boolean setType = b.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok = b.Build();

        Assert.That(setAsm,Is.True);
        Assert.That(setType,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.Not.Null);
        Assert.That(b.Value!.GetType().FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void Build_SetContext_WithProviderAssemblies_AioAssemblyType_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        ObjectBuilder b = new();

        Boolean setContext = b.SetContext(c);
        Boolean setAsm = b.Provider.SetAssemblies([ path ]);
        Boolean setType = b.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok = b.Build();

        Assert.That(setContext,Is.True);
        Assert.That(setAsm,Is.True);
        Assert.That(setType,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.Not.Null);
        Assert.That(GetLoadContext(b.Value!.GetType().Assembly),Is.SameAs(c));
    }

    [Test]
    public void Build_WithContext_WithProviderAssemblies_AioAssemblyType_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        AssemblyLoadContext c = CreateCollectibleContext();
        ObjectBuilder b = new ObjectBuilder()
            .WithContext(c)
            .WithType(ReflectionAssemblyProbeHelper.TestTypeFullName);

        Boolean setAsm = b.Provider.SetAssemblies([ path ]);
        Boolean ok = b.Build();

        Assert.That(setAsm,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.Not.Null);
        Assert.That(GetLoadContext(b.Value!.GetType().Assembly),Is.SameAs(c));
    }

    [Test]
    public void Build_WithContext_StringType_IsSameAsTypeofString()
    {
        AssemblyLoadContext c = CreateCollectibleContext();
        ObjectBuilder b = new ObjectBuilder()
            .WithContext(c)
            .WithType(typeof(String).AssemblyQualifiedName)
            .WithArgument(0,'x')
            .WithArgument(1,5);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<String>());
        Assert.That(b.Value!.GetType(),Is.SameAs(typeof(String)));
        Assert.That((String)b.Value,Is.EqualTo("xxxxx"));
    }

    [Test]
    public void Build_ProviderSetAssemblies_AioAssemblyType_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectBuilder b = new();

        Boolean setAsm = b.Provider.SetAssemblies([ path ]);
        Boolean setType = b.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok = b.Build();

        Assert.That(setAsm,Is.True);
        Assert.That(setType,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.Not.Null);
        Assert.That(b.Value!.GetType().FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void Build_ProviderWithAssemblies_AioAssemblyType_Succeeds()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectBuilder b = new ObjectBuilder();
        _ = b.Provider.WithAssemblies([ path ]);
        _ = b.WithType(ReflectionAssemblyProbeHelper.TestTypeFullName);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.Not.Null);
        Assert.That(b.Value!.GetType().FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    [Test]
    public void Fluent_CreateString_AndWithArgument_Builds()
    {
        ObjectBuilder b = ObjectBuilder.Create(typeof(Uri).AssemblyQualifiedName)
            .WithArgument(0,"https://example.org");

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Uri>());
        Assert.That(((Uri)b.Value!).Host,Is.EqualTo("example.org"));
    }

    [Test]
    public void Fluent_WithTypeType_Chain_Builds()
    {
        ObjectBuilder b = new ObjectBuilder()
            .WithType(typeof(Version))
            .WithArgument(0,5)
            .WithArgument(1,6);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Version>());
        Assert.That(((Version)b.Value!).ToString(),Is.EqualTo("5.6"));
    }

    [Test]
    public void Fluent_WithTypeString_Chain_Builds()
    {
        ObjectBuilder b = new ObjectBuilder()
            .WithType(typeof(Uri).AssemblyQualifiedName)
            .WithArgument(0,"https://example.net");

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Uri>());
        Assert.That(((Uri)b.Value!).Host,Is.EqualTo("example.net"));
    }

    [Test]
    public void Build_TypeAndArgs_ExactMatch_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Version));
        _ = b.SetArgument(0,1);
        _ = b.SetArgument(1,2);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Version>());
        Assert.That(((Version)b.Value!).Major,Is.EqualTo(1));
        Assert.That(((Version)b.Value!).Minor,Is.EqualTo(2));
    }

    [Test]
    public void Build_GuidParseStringCtor_Succeeds()
    {
        ObjectBuilder b = new();

        String g = "0f8fad5b-d9cb-469f-a165-70867728950e";

        _ = b.SetType(typeof(Guid));
        _ = b.SetArgument(0,g);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Guid>());
        Assert.That((Guid)b.Value!,Is.EqualTo(Guid.Parse(g,CultureInfo.InvariantCulture)));
    }

    [Test]
    public void Build_DateTimeOffsetCtor_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(DateTimeOffset));
        _ = b.SetArgument(0,2025);
        _ = b.SetArgument(1,4);
        _ = b.SetArgument(2,9);
        _ = b.SetArgument(3,11);
        _ = b.SetArgument(4,22);
        _ = b.SetArgument(5,33);
        _ = b.SetArgument(6,TimeSpan.Zero);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<DateTimeOffset>());
        DateTimeOffset d = (DateTimeOffset)b.Value!;
        Assert.That(d.Year,Is.EqualTo(2025));
        Assert.That(d.Offset,Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void Build_TimeSpanCtor_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(TimeSpan));
        _ = b.SetArgument(0,1);
        _ = b.SetArgument(1,2);
        _ = b.SetArgument(2,3);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<TimeSpan>());
        Assert.That((TimeSpan)b.Value!,Is.EqualTo(new TimeSpan(1,2,3)));
    }

    [Test]
    public void Build_TypeName_Framework_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Uri).AssemblyQualifiedName);
        _ = b.SetArgument(0,"https://example.org");

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Uri>());
        Assert.That(((Uri)b.Value!).Host,Is.EqualTo("example.org"));
    }

    [Test]
    public void Build_ListCtor_WithCapacity_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(List<String>));
        _ = b.SetArgument(0,16);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<List<String>>());
        Assert.That(((List<String>)b.Value!).Capacity,Is.GreaterThanOrEqualTo(16));
    }

    [Test]
    public void Build_DictionaryCtor_WithComparer_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Dictionary<String,Int32>));
        _ = b.SetArgument(0,StringComparer.OrdinalIgnoreCase);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Dictionary<String,Int32>>());

        Dictionary<String,Int32> d = (Dictionary<String,Int32>)b.Value!;

        d["a"] = 1;

        Assert.That(d.ContainsKey("A"),Is.True);
    }

    [Test]
    public void Build_Assignable_InterfaceCtor_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Queue<String>));
        _ = b.SetArgument(0,new List<String>() { "x" , "y" });

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Queue<String>>());
        Assert.That(((Queue<String>)b.Value!).Count,Is.EqualTo(2));
    }

    [Test]
    public void Build_SparseIndexes_UsesRequiredWidth_SucceedsWhenCtorExists()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Tuple<String,String,String>));
        _ = b.SetArgument(0,"a");
        _ = b.SetArgument(2,"c");

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Tuple<String,String,String>>());
        Tuple<String,String,String> t = (Tuple<String,String,String>)b.Value!;
        Assert.That(t.Item1,Is.EqualTo("a"));
        Assert.That(t.Item2,Is.Null);
        Assert.That(t.Item3,Is.EqualTo("c"));
    }

    [Test]
    public void Build_SparseIndexes_FailsWhenNoCtorAtRequiredWidth()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Tuple<String,String>));
        _ = b.SetArgument(0,"a");
        _ = b.SetArgument(2,"c");

        Boolean ok = b.Build();

        Assert.That(ok,Is.False);
        Assert.That(b.Value,Is.Null);
    }

    [Test]
    public void Build_AssignableArgument_Succeeds()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Exception));
        _ = b.SetArgument(0,"Offline");
        _ = b.SetArgument(1,new ArgumentException("inner"));

        Boolean ok = b.Build();

        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Exception>());
        Exception e = (Exception)b.Value!;
        Assert.That(e.Message,Is.EqualTo("Offline"));
        Assert.That(e.InnerException,Is.TypeOf<ArgumentException>());
    }

    [Test]
    public void Build_NullMissingForNonNullableValueType_Fails()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Version));
        _ = b.SetArgument(0,1);

        Boolean ok = b.Build();

        Assert.That(ok,Is.False);
        Assert.That(b.Value,Is.Null);
    }

    [Test]
    public void Build_AmbiguousConstructorSelection_Fails()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(AmbiguousType));
        _ = b.SetArgument(0,123);

        Boolean ok = b.Build();

        Assert.That(ok,Is.False);
        Assert.That(b.Value,Is.Null);
    }

    [Test]
    public void Build_IdempotentAfterSuccess_ReturnsTrueAndKeepsValue()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Version));
        _ = b.SetArgument(0,1);
        _ = b.SetArgument(1,2);

        Boolean ok0 = b.Build();
        Object? v0 = b.Value;
        Boolean ok1 = b.Build();

        Assert.That(ok0,Is.True);
        Assert.That(ok1,Is.True);
        Assert.That(b.Value,Is.SameAs(v0));
    }

    [Test]
    public void Build_WithoutResolvableType_Fails()
    {
        ObjectBuilder b = new();

        _ = b.SetType("Not.Real.BuilderType, Not.Real.Assembly");

        Boolean ok = b.Build();

        Assert.That(ok,Is.False);
        Assert.That(b.Value,Is.Null);
    }

    [Test, TestCaseSource(nameof(GetFrameworkBuilderCases))]
    public void Build_FrameworkTypes(BuilderCase c)
    {
        ObjectBuilder b = new();

        c.Configure(b);

        Boolean ok = b.Build();

        Assert.That(ok,Is.True,c.Name);
        Assert.That(c.Validate(b.Value),Is.True,c.Name);
    }

    private static IEnumerable<TestCaseData> GetFrameworkBuilderCases()
    {
        yield return new TestCaseData(new BuilderCase(
            "DateOnly(yyyy,mm,dd)",
            b => { _ = b.SetType(typeof(DateOnly)); _ = b.SetArgument(0,2024); _ = b.SetArgument(1,2); _ = b.SetArgument(2,29); },
            v => v is DateOnly d && Equals(d.Day,29)));

        yield return new TestCaseData(new BuilderCase(
            "TimeOnly(h,m,s)",
            b => { _ = b.SetType(typeof(TimeOnly)); _ = b.SetArgument(0,23); _ = b.SetArgument(1,59); _ = b.SetArgument(2,58); },
            v => v is TimeOnly t && Equals(t.Hour,23) && Equals(t.Second,58)));

        yield return new TestCaseData(new BuilderCase(
            "DateTime(y,m,d,h,m,s,kind)",
            b => { _ = b.SetType(typeof(DateTime)); _ = b.SetArgument(0,2020); _ = b.SetArgument(1,1); _ = b.SetArgument(2,2); _ = b.SetArgument(3,3); _ = b.SetArgument(4,4); _ = b.SetArgument(5,5); _ = b.SetArgument(6,DateTimeKind.Utc); },
            v => v is DateTime d && Equals(d.Kind,DateTimeKind.Utc) && Equals(d.Year,2020)));

        yield return new TestCaseData(new BuilderCase(
            "Complex(real,imag)",
            b => { _ = b.SetType(typeof(Complex)); _ = b.SetArgument(0,3.5D); _ = b.SetArgument(1,-2.25D); },
            v => v is Complex c && c.Real.Equals(3.5D) && c.Imaginary.Equals(-2.25D)));

        yield return new TestCaseData(new BuilderCase(
            "BigInteger(byte[])",
            b => { _ = b.SetType(typeof(BigInteger)); _ = b.SetArgument(0,new Byte[] { 0x2A }); },
            v => v is BigInteger bi && bi.Equals(new BigInteger(42))));

        yield return new TestCaseData(new BuilderCase(
            "Guid(byte[])",
            b => { _ = b.SetType(typeof(Guid)); _ = b.SetArgument(0,new Byte[] { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 }); },
            v => v is Guid g && g != Guid.Empty));

        yield return new TestCaseData(new BuilderCase(
            "StringBuilder(capacity,max)",
            b => { _ = b.SetType(typeof(StringBuilder)); _ = b.SetArgument(0,5); _ = b.SetArgument(1,20); },
            v => v is StringBuilder s && s.Capacity >= 5 && s.MaxCapacity == 20));

        yield return new TestCaseData(new BuilderCase(
            "MemoryStream(capacity)",
            b => { _ = b.SetType(typeof(MemoryStream)); _ = b.SetArgument(0,128); },
            v => v is MemoryStream m && m.Capacity >= 128 && m.Length == 0));

        yield return new TestCaseData(new BuilderCase(
            "StringReader(string)",
            b => { _ = b.SetType(typeof(StringReader)); _ = b.SetArgument(0,"abc"); },
            v => v is StringReader r && r.ReadToEnd() == "abc"));

        yield return new TestCaseData(new BuilderCase(
            "StringWriter(provider)",
            b => { _ = b.SetType(typeof(StringWriter)); _ = b.SetArgument(0,CultureInfo.InvariantCulture); },
            v => v is StringWriter w && Equals(w.FormatProvider,CultureInfo.InvariantCulture)));

        yield return new TestCaseData(new BuilderCase(
            "Regex(pattern,options,timeout)",
            b => { _ = b.SetType(typeof(Regex)); _ = b.SetArgument(0,"a+b"); _ = b.SetArgument(1,RegexOptions.IgnoreCase); _ = b.SetArgument(2,TimeSpan.FromSeconds(2)); },
            v => v is Regex r && r.IsMatch("AAAb",0)));

        yield return new TestCaseData(new BuilderCase(
            "XmlQualifiedName(local,ns)",
            b => { _ = b.SetType(typeof(XmlQualifiedName)); _ = b.SetArgument(0,"node"); _ = b.SetArgument(1,"urn:test"); },
            v => v is XmlQualifiedName x && x.Name == "node" && x.Namespace == "urn:test"));

        yield return new TestCaseData(new BuilderCase(
            "UriBuilder(uri)",
            b => { _ = b.SetType(typeof(UriBuilder)); _ = b.SetArgument(0,"https://example.org:8080/a?b=c#f"); },
            v => v is UriBuilder u && u.Host == "example.org" && u.Port == 8080));

        yield return new TestCaseData(new BuilderCase(
            "CancellationTokenSource(ms)",
            b => { _ = b.SetType(typeof(CancellationTokenSource)); _ = b.SetArgument(0,1000); },
            v => v is CancellationTokenSource c && !c.IsCancellationRequested));

        yield return new TestCaseData(new BuilderCase(
            "SemaphoreSlim(initial,max)",
            b => { _ = b.SetType(typeof(SemaphoreSlim)); _ = b.SetArgument(0,1); _ = b.SetArgument(1,3); },
            v => v is SemaphoreSlim s && Equals(s.CurrentCount,1)));

        yield return new TestCaseData(new BuilderCase(
            "UTF8Encoding(emitBom,throwOnInvalid)",
            b => { _ = b.SetType(typeof(UTF8Encoding)); _ = b.SetArgument(0,false); _ = b.SetArgument(1,true); },
            v => v is UTF8Encoding u && u.GetPreamble().Length == 0));

        yield return new TestCaseData(new BuilderCase(
            "UnicodeEncoding(be,bom,throw)",
            b => { _ = b.SetType(typeof(UnicodeEncoding)); _ = b.SetArgument(0,true); _ = b.SetArgument(1,true); _ = b.SetArgument(2,true); },
            v => v is UnicodeEncoding u && u.GetPreamble().Length > 0));

        yield return new TestCaseData(new BuilderCase(
            "ArraySegment<byte>(array,offset,count)",
            b => { _ = b.SetType(typeof(ArraySegment<Byte>)); _ = b.SetArgument(0,new Byte[] { 1,2,3,4,5 }); _ = b.SetArgument(1,1); _ = b.SetArgument(2,3); },
            v => v is ArraySegment<Byte> a && Equals(a.Count,3) && a.Array![a.Offset] == 2));

        yield return new TestCaseData(new BuilderCase(
            "KeyValuePair<string,int>(k,v)",
            b => { _ = b.SetType(typeof(KeyValuePair<String,Int32>)); _ = b.SetArgument(0,"k"); _ = b.SetArgument(1,7); },
            v => v is KeyValuePair<String,Int32> kv && kv.Key == "k" && kv.Value == 7));

        yield return new TestCaseData(new BuilderCase(
            "ConcurrentDictionary<string,int>(comparer)",
            b => { _ = b.SetType(typeof(ConcurrentDictionary<String,Int32>)); _ = b.SetArgument(0,StringComparer.OrdinalIgnoreCase); },
            v =>
            {
                if(v is not ConcurrentDictionary<String,Int32> d) { return false; }

                d["a"] = 1; return d.ContainsKey("A");
            }));
    }

    public sealed record BuilderCase(String Name , Action<ObjectBuilder> Configure , Func<Object?,Boolean> Validate);

    private sealed class AmbiguousType
    {
        public AmbiguousType(IComparable value) {}

        public AmbiguousType(IFormattable value) {}
    }

    [Test]
    public void TryResolveType_WithProviderAssemblies_Succeeds_WithoutLockingType()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectBuilder b = new();

        Boolean setAsm = b.Provider.SetAssemblies([ path ]);
        Boolean probe = b.TryResolveType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean setType = b.SetType(typeof(Version));
        Boolean set0 = b.SetArgument(0,1);
        Boolean set1 = b.SetArgument(1,2);
        Boolean ok = b.Build();

        Assert.That(setAsm,Is.True);
        Assert.That(probe,Is.True);
        Assert.That(setType,Is.True);
        Assert.That(set0,Is.True);
        Assert.That(set1,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Version>());
    }

    [Test]
    public void TryResolveType_Failure_DoesNotMutateBuilderState()
    {
        ObjectBuilder b = new();

        Boolean probe = b.TryResolveType("Not.Real.Type, Not.Real.Assembly");
        Boolean setType = b.SetType(typeof(Version));
        Boolean set0 = b.SetArgument(0,2);
        Boolean set1 = b.SetArgument(1,3);
        Boolean ok = b.Build();

        Assert.That(probe,Is.False);
        Assert.That(setType,Is.True);
        Assert.That(set0,Is.True);
        Assert.That(set1,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.TypeOf<Version>());
    }

    [Test]
    public void TryResolveType_WhenTypeLocked_Fails()
    {
        ObjectBuilder b = new();

        Boolean setType = b.SetType(typeof(Version));
        Boolean probe = b.TryResolveType(typeof(Guid).AssemblyQualifiedName);

        Assert.That(setType,Is.True);
        Assert.That(probe,Is.False);
    }

    [Test]
    public void TryResolveType_WhenValueAlreadyBuilt_Fails()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Version));
        _ = b.SetArgument(0,1);
        _ = b.SetArgument(1,2);

        Boolean built = b.Build();
        Boolean probe = b.TryResolveType(typeof(Guid).AssemblyQualifiedName);

        Assert.That(built,Is.True);
        Assert.That(probe,Is.False);
    }

    [Test]
    public void SetContext_WhenTypeLocked_Fails()
    {
        ObjectBuilder b = new();

        Boolean setType = b.SetType(typeof(Version));
        Boolean setContext = b.SetContext(CreateCollectibleContext());

        Assert.That(setType,Is.True);
        Assert.That(setContext,Is.False);
    }

    [Test]
    public void SetContext_WhenValueAlreadyBuilt_Fails()
    {
        ObjectBuilder b = new();

        _ = b.SetType(typeof(Version));
        _ = b.SetArgument(0,1);
        _ = b.SetArgument(1,2);

        Boolean built = b.Build();
        Boolean setContext = b.SetContext(CreateCollectibleContext());

        Assert.That(built,Is.True);
        Assert.That(setContext,Is.False);
    }

    [Test]
    public void Provider_IsExposed_AndCanConfigureTypeResolution()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectBuilder b = new();

        Boolean setAsm = b.Provider.SetAssemblies([ path ]);
        Boolean setType = b.SetType(ReflectionAssemblyProbeHelper.TestTypeFullName);
        Boolean ok = b.Build();

        Assert.That(setAsm,Is.True);
        Assert.That(setType,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(b.Value,Is.Not.Null);
        Assert.That(b.Value!.GetType().FullName,Is.EqualTo(ReflectionAssemblyProbeHelper.TestTypeFullName));
    }

    private static AssemblyLoadContext CreateCollectibleContext()
    {
        return new($"ObjectBuilderExam:{Guid.NewGuid():N}",isCollectible:true);
    }

    private static AssemblyLoadContext? GetLoadContext(Assembly assembly)
    {
        try { return AssemblyLoadContext.GetLoadContext(assembly); }

        catch { return null; }
    }
}
