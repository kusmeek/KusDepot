namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ObjectParserExam
{
    [Test]
    public void Provider_SetAssemblies_AioAssemblyPath_IsAccepted()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectParser p = new();

        Boolean ok = p.Provider.SetAssemblies([ path ]);

        Assert.That(ok,Is.True);
    }

    [Test]
    public void Fluent_Provider_WithAssemblies_AioAssemblyPath_IsAccepted()
    {
        Assert.That(ReflectionAssemblyProbeHelper.TryGetTestAssemblyPath(out String path),Is.True,
            "Test Assembly Not Found");

        ObjectParser p = new(); _ = p.Provider.WithAssemblies([ path ]);

        Assert.That(p,Is.Not.Null);
    }

    [Test]
    public void Create_AndParse_Succeeds()
    {
        ObjectParser p = ObjectParser.Create(typeof(Int32).AssemblyQualifiedName)
            .WithData("456")
            .WithFormatter(CultureInfo.InvariantCulture);

        Object? v = p.Parse();

        Assert.That(v,Is.TypeOf<Int32>());
        Assert.That((Int32)v!,Is.EqualTo(456));
    }

    [Test]
    public void Provider_SetType_AndTryParse_Succeeds()
    {
        ObjectParser p = new(); _ = p.Provider.SetType(typeof(Int32).AssemblyQualifiedName);

        _ = p.SetData("88");
        _ = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<Int32>());
        Assert.That((Int32)p.Value!,Is.EqualTo(88));
    }

    [Test]
    public void Parse_ReturnsParsedValue_WhenResolvable()
    {
        ObjectParser p = new();

        _ = p.Provider.SetType(typeof(Int32).AssemblyQualifiedName);
        _ = p.SetData("123");
        _ = p.SetFormatter(CultureInfo.InvariantCulture);

        Object? v = p.Parse();

        Assert.That(v,Is.TypeOf<Int32>());
        Assert.That((Int32)v!,Is.EqualTo(123));
        Assert.That(p.Value,Is.EqualTo(v));
    }

    [Test]
    public void TryParse_TryParseSignature_FrameworkType_Succeeds()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(Int32).AssemblyQualifiedName);
        Boolean setData = p.SetData("123");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<Int32>());
        Assert.That((Int32)p.Value!,Is.EqualTo(123));
    }

    [Test]
    public void TryParse_TryParseOnlyType_Succeeds()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(TryParseOnlyType).AssemblyQualifiedName);
        Boolean setData = p.SetData("77");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<TryParseOnlyType>());
        Assert.That(((TryParseOnlyType)p.Value!).Value,Is.EqualTo(77));
    }

    [Test]
    public void TryParse_ParseFallback_TypeWithOnlyParse_Succeeds()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(ParseOnlyType).AssemblyQualifiedName);
        Boolean setData = p.SetData("42");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<ParseOnlyType>());
        Assert.That(((ParseOnlyType)p.Value!).Value,Is.EqualTo(42));
    }

    [Test]
    public void TryParse_PrefersTryParse_WhenBothExist()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(BothType).AssemblyQualifiedName);
        Boolean setData = p.SetData("x");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<BothType>());
        Assert.That(((BothType)p.Value!).Source,Is.EqualTo("TryParse"));
    }

    [Test]
    public void TryParse_WithContext_Int32_Succeeds_AndReturnsSystemInt32()
    {
        ObjectParser p = new ObjectParser()
            .WithContext(CreateCollectibleContext())
            .WithData("123")
            .WithFormatter(CultureInfo.InvariantCulture);

        Boolean setType = p.Provider.SetType(typeof(Int32).AssemblyQualifiedName);
        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<Int32>());
        Assert.That(p.Value!.GetType(),Is.SameAs(typeof(Int32)));
    }

    [Test]
    public void SetContext_WhenParsed_Fails()
    {
        ObjectParser p = new();

        _ = p.Provider.SetType(typeof(Int32).AssemblyQualifiedName);
        _ = p.SetData("5");
        _ = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean parsed = p.TryParse();
        Boolean setContext = p.SetContext(CreateCollectibleContext());

        Assert.That(parsed,Is.True);
        Assert.That(setContext,Is.False);
    }

    [Test]
    public void TryParse_TryParseStringOutOnly_Succeeds()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(TryParseStringOnlyType).AssemblyQualifiedName);
        Boolean setData = p.SetData("91");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<TryParseStringOnlyType>());
        Assert.That(((TryParseStringOnlyType)p.Value!).Value,Is.EqualTo(91));
    }

    [Test]
    public void TryParse_ParseStringOnlyFallback_Succeeds()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(ParseStringOnlyType).AssemblyQualifiedName);
        Boolean setData = p.SetData("84");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<ParseStringOnlyType>());
        Assert.That(((ParseStringOnlyType)p.Value!).Value,Is.EqualTo(84));
    }

    [Test]
    public void TryParse_PrefersFormatterAwareTryParse_WhenBothTryParseOverloadsExist()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(BothTryParseFormsType).AssemblyQualifiedName);
        Boolean setData = p.SetData("x");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<BothTryParseFormsType>());
        Assert.That(((BothTryParseFormsType)p.Value!).Source,Is.EqualTo("TryParseWithFormatter"));
    }

    [Test]
    public void TryParse_Version_Succeeds()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(Version).AssemblyQualifiedName);
        Boolean setData = p.SetData("1.2.3.4");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<Version>());
        Assert.That(((Version)p.Value!).ToString(),Is.EqualTo("1.2.3.4"));
    }

    [Test]
    public void CanParse_Type_Int32_IsTrue()
    {
        Assert.That(ObjectParser.CanParse(typeof(Int32)),Is.True);
    }

    [Test]
    public void CanParse_Type_Version_IsTrue()
    {
        Assert.That(ObjectParser.CanParse(typeof(Version)),Is.True);
    }

    [Test]
    public void CanParse_Type_Uri_IsFalse()
    {
        Assert.That(ObjectParser.CanParse(typeof(Uri)),Is.False);
    }

    [Test]
    public void CanParse_Type_Null_IsFalse()
    {
        Assert.That(ObjectParser.CanParse((Type?)null),Is.False);
    }

    [Test]
    public void CanParse_Name_Int32_IsTrue()
    {
        Assert.That(ObjectParser.CanParse(typeof(Int32).AssemblyQualifiedName),Is.True);
    }

    [Test]
    public void CanParse_Name_Uri_IsFalse()
    {
        Assert.That(ObjectParser.CanParse(typeof(Uri).AssemblyQualifiedName),Is.False);
    }

    [Test]
    public void TryParse_String_Succeeds_AndReturnsInput()
    {
        ObjectParser p = new();

        Boolean setType = p.Provider.SetType(typeof(String).AssemblyQualifiedName);
        Boolean setData = p.SetData("hello");
        Boolean setFormat = p.SetFormatter(CultureInfo.InvariantCulture);

        Boolean ok = p.TryParse();

        Assert.That(setType,Is.True);
        Assert.That(setData,Is.True);
        Assert.That(setFormat,Is.True);
        Assert.That(ok,Is.True);
        Assert.That(p.Value,Is.TypeOf<String>());
        Assert.That((String)p.Value!,Is.EqualTo("hello"));
    }

    [Test]
    public void Parse_String_Succeeds_AndReturnsInput()
    {
        ObjectParser p = ObjectParser.Create(typeof(String).AssemblyQualifiedName)
            .WithData("payload")
            .WithFormatter(CultureInfo.InvariantCulture);

        Object? v = p.Parse();

        Assert.That(v,Is.TypeOf<String>());
        Assert.That((String)v!,Is.EqualTo("payload"));
    }

    [Test]
    public void CanParse_Type_String_IsTrue()
    {
        Assert.That(ObjectParser.CanParse(typeof(String)),Is.True);
    }

    [Test]
    public void CanParse_Name_String_IsTrue()
    {
        Assert.That(ObjectParser.CanParse(typeof(String).AssemblyQualifiedName),Is.True);
    }

    private sealed class TryParseOnlyType
    {
        public Int32 Value { get; }

        private TryParseOnlyType(Int32 value)
        {
            Value = value;
        }

        public static Boolean TryParse(String input , IFormatProvider formatter , out TryParseOnlyType value)
        {
            if(Int32.TryParse(input,NumberStyles.Integer,formatter,out Int32 v))
            {
                value = new TryParseOnlyType(v); return true;
            }

            value = null!; return false;
        }
    }

    private sealed class ParseOnlyType
    {
        public Int32 Value { get; }

        private ParseOnlyType(Int32 value)
        {
            Value = value;
        }

        public static ParseOnlyType Parse(String input , IFormatProvider formatter)
        {
            Int32 v = Int32.Parse(input,formatter); return new ParseOnlyType(v);
        }
    }

    private sealed class BothType
    {
        public String Source { get; }

        private BothType(String source)
        {
            Source = source;
        }

        public static Boolean TryParse(String input , IFormatProvider formatter , out BothType value)
        {
            value = new BothType("TryParse"); return true;
        }

        public static BothType Parse(String input , IFormatProvider formatter)
        {
            return new BothType("Parse");
        }
    }

    private sealed class TryParseStringOnlyType
    {
        public Int32 Value { get; }

        private TryParseStringOnlyType(Int32 value)
        {
            Value = value;
        }

        public static Boolean TryParse(String input , out TryParseStringOnlyType value)
        {
            if(Int32.TryParse(input,out Int32 v))
            {
                value = new TryParseStringOnlyType(v); return true;
            }

            value = null!; return false;
        }
    }

    private sealed class ParseStringOnlyType
    {
        public Int32 Value { get; }

        private ParseStringOnlyType(Int32 value)
        {
            Value = value;
        }

        public static ParseStringOnlyType Parse(String input)
        {
            Int32 v = Int32.Parse(input,CultureInfo.InvariantCulture); return new ParseStringOnlyType(v);
        }
    }

    private sealed class BothTryParseFormsType
    {
        public String Source { get; }

        private BothTryParseFormsType(String source)
        {
            Source = source;
        }

        public static Boolean TryParse(String input , IFormatProvider formatter , out BothTryParseFormsType value)
        {
            value = new BothTryParseFormsType("TryParseWithFormatter"); return true;
        }

        public static Boolean TryParse(String input , out BothTryParseFormsType value)
        {
            value = new BothTryParseFormsType("TryParseStringOnly"); return true;
        }
    }

    private static AssemblyLoadContext CreateCollectibleContext()
    {
        return new($"ObjectParserExam:{Guid.NewGuid():N}",isCollectible:true);
    }
}
