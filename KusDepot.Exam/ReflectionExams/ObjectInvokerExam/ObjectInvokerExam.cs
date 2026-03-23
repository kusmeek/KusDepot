namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ObjectInvokerExam
{
    [Test]
    public void Create_WithInstance_SetsValue()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Assert.That(i.Value,Is.SameAs(t));
    }

    [Test]
    public void InvokeStatic_WithType_ReturnsValue()
    {
        ObjectInvoker i = new ObjectInvoker().WithType(typeof(StaticOnlyTarget));

        Object? v = i.InvokeStatic("StaticSum",2,3);

        Assert.That(v,Is.EqualTo(5));
    }

    [Test]
    public void InvokeStatic_WithoutType_ReturnsNull()
    {
        ObjectInvoker i = new();

        Object? v = i.InvokeStatic("StaticSum",2,3);

        Assert.That(v,Is.Null);
    }

    [Test]
    public void InvokeStatic_ExcludesInstanceMethods_WhenNamesConflict()
    {
        ObjectInvoker i = new ObjectInvoker().WithType(typeof(DerivedWithStaticBase));

        Object? v = i.InvokeStatic("Pick",7);

        Assert.That(v,Is.EqualTo("base-static"));
    }

    [Test]
    public async Task InvokeStaticAsync_TaskOfT_ReturnsResult()
    {
        ObjectInvoker i = new ObjectInvoker().WithType(typeof(StaticAsyncTarget));

        Object? v = await i.InvokeStaticAsync("StaticSumAsync",[ 7 , 8 ]);

        Assert.That(v,Is.TypeOf<Int32>());
        Assert.That((Int32)v!,Is.EqualTo(15));
    }

    [Test]
    public async Task InvokeStaticAsync_ValueTaskOfT_ReturnsResult()
    {
        ObjectInvoker i = new ObjectInvoker().WithType(typeof(StaticAsyncTarget));

        Object? v = await i.InvokeStaticAsync("StaticNameAsync");

        Assert.That(v,Is.EqualTo("static-async-name"));
    }

    [Test]
    public void SetInstance_WithNull_ReturnsFalse()
    {
        ObjectInvoker i = new();

        Boolean ok = i.SetInstance(null);

        Assert.That(ok,Is.False);
        Assert.That(i.Value,Is.Null);
    }

    [Test]
    public void WithInstance_Chains_AndSetsValue()
    {
        SampleTarget t = new();

        ObjectInvoker i = new ObjectInvoker().WithInstance(t);

        Assert.That(i.Value,Is.SameAs(t));
    }

    [Test]
    public void WithType_AndAllowStaticMembers_EnablesStaticInvocation()
    {
        ObjectInvoker i = new ObjectInvoker()
            .WithType(typeof(StaticOnlyTarget))
            .WithAllowStaticMembers();

        Object? v = i.Invoke("StaticSum",2,3);

        Assert.That(v,Is.EqualTo(5));
    }

    [Test]
    public void GetProperty_StaticProperty_RequiresAllowStaticMembers()
    {
        ObjectInvoker i0 = new ObjectInvoker().WithType(typeof(StaticOnlyTarget));
        ObjectInvoker i1 = new ObjectInvoker().WithType(typeof(StaticOnlyTarget)).WithAllowStaticMembers();

        Object? v0 = i0.GetProperty("StaticName");
        Object? v1 = i1.GetProperty("StaticName");

        Assert.That(v0,Is.Null);
        Assert.That(v1,Is.EqualTo("static-name"));
    }

    [Test]
    public void Invoke_PrefersInstanceOverStatic_WhenStaticEnabled()
    {
        DerivedWithStaticBase t = new();

        ObjectInvoker i = new ObjectInvoker()
            .WithInstance(t)
            .WithAllowStaticMembers();

        Object? v = i.Invoke("Pick",7);

        Assert.That(v,Is.EqualTo("instance"));
    }

    [Test]
    public void GetProperty_ReadableProperty_ReturnsValue()
    {
        SampleTarget t = new() { Name = "alpha" };

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.GetProperty("Name");

        Assert.That(v,Is.EqualTo("alpha"));
    }

    [Test]
    public void GetProperty_MissingProperty_ReturnsNull()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.GetProperty("Nope");

        Assert.That(v,Is.Null);
    }

    [Test]
    public void SetProperty_WritableProperty_Succeeds()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Boolean ok = i.SetProperty("Name","beta");

        Assert.That(ok,Is.True);
        Assert.That(t.Name,Is.EqualTo("beta"));
    }

    [Test]
    public void SetProperty_IncompatibleValue_Fails()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Boolean ok = i.SetProperty("Number","not-an-int");

        Assert.That(ok,Is.False);
        Assert.That(t.Number,Is.EqualTo(0));
    }

    [Test]
    public void SetProperty_ReadOnlyProperty_Fails()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Boolean ok = i.SetProperty("ReadOnlyLabel","x");

        Assert.That(ok,Is.False);
        Assert.That(t.ReadOnlyLabel,Is.EqualTo("readonly"));
    }

    [Test]
    public void Invoke_SyncMethod_ReturnsValue()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Sum",2,3);

        Assert.That(v,Is.TypeOf<Int32>());
        Assert.That((Int32)v!,Is.EqualTo(5));
    }

    [Test]
    public void Invoke_UsesDefaultParameter_WhenMissingArg()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("JoinDefault","left");

        Assert.That(v,Is.EqualTo("left:right"));
    }

    [Test]
    public void Invoke_ParamsArray_PacksAdditionalArguments()
    {
        ParamsTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Join","root","a","b","c");

        Assert.That(v,Is.EqualTo("root:a,b,c"));
    }

    [Test]
    public void Invoke_ParamsArray_AcceptsExplicitArray()
    {
        ParamsTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Join","root",new String[] { "x" , "y" });

        Assert.That(v,Is.EqualTo("root:x,y"));
    }

    [Test]
    public void Invoke_Overload_ExactPreferredOverAssignable()
    {
        OverloadTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Pick",5);

        Assert.That(v,Is.EqualTo("int"));
    }

    [Test]
    public void Invoke_Overload_Ambiguous_ReturnsNull()
    {
        AmbiguousInvokeTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Pick",5);

        Assert.That(v,Is.Null);
    }

    [Test]
    public async Task InvokeAsync_Task_ReturnsNullOnCompletion()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = await i.InvokeAsync("MarkAsync");

        Assert.That(v,Is.Null);
        Assert.That(t.Marked,Is.True);
    }

    [Test]
    public async Task InvokeAsync_TaskOfT_ReturnsResult()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = await i.InvokeAsync("SumAsync",[ 7 , 8 ]);

        Assert.That(v,Is.TypeOf<Int32>());
        Assert.That((Int32)v!,Is.EqualTo(15));
    }

    [Test]
    public async Task InvokeAsync_ValueTask_ReturnsNullOnCompletion()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = await i.InvokeAsync("MarkValueTaskAsync");

        Assert.That(v,Is.Null);
        Assert.That(t.MarkedValueTask,Is.True);
    }

    [Test]
    public async Task InvokeAsync_ValueTaskOfT_ReturnsResult()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = await i.InvokeAsync("NameAsync");

        Assert.That(v,Is.EqualTo("async-name"));
    }

    [Test]
    public async Task InvokeAsync_NonAsyncMethod_ReturnsSyncValue()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = await i.InvokeAsync("Sum",[ 4 , 6 ]);

        Assert.That(v,Is.TypeOf<Int32>());
        Assert.That((Int32)v!,Is.EqualTo(10));
    }

    [Test]
    public void Framework_StringBuilder_PropertyAndMethod_Works()
    {
        StringBuilder sb = new();

        ObjectInvoker i = ObjectInvoker.Create(sb);

        Object? r0 = i.Invoke("Append","abc");
        Object? len = i.GetProperty("Length");
        Object? s = i.Invoke("ToString");

        Assert.That(r0,Is.TypeOf<StringBuilder>());
        Assert.That(len,Is.EqualTo(3));
        Assert.That(s,Is.EqualTo("abc"));
    }

    [Test]
    public void Framework_List_AddAndCount_Works()
    {
        List<Int32> list = [];

        ObjectInvoker i = ObjectInvoker.Create(list);

        Object? r0 = i.Invoke("Add",5);
        Object? r1 = i.Invoke("Add",7);
        Object? count = i.GetProperty("Count");

        Assert.That(r0,Is.Null);
        Assert.That(r1,Is.Null);
        Assert.That(count,Is.EqualTo(2));
    }

    [Test]
    public void Framework_Dictionary_Add_Works()
    {
        Dictionary<String,Int32> d = [];

        ObjectInvoker i = ObjectInvoker.Create(d);

        Object? r = i.Invoke("Add","k",9);

        Assert.That(r,Is.Null);
        Assert.That(d["k"],Is.EqualTo(9));
    }

    [Test]
    public void Framework_UriBuilder_SetProperty_AndToString_Works()
    {
        UriBuilder u = new("https://example.org");

        ObjectInvoker i = ObjectInvoker.Create(u);

        Boolean ok = i.SetProperty("Host","example.net");
        Object? s = i.Invoke("ToString");

        Assert.That(ok,Is.True);
        Assert.That(s,Is.TypeOf<String>());
        Assert.That(((String)s!).Contains("example.net"),Is.True);
    }

    [Test]
    public async Task Framework_CancellationTokenSource_InvokeAsync_Cancel_Works()
    {
        CancellationTokenSource cts = new();

        ObjectInvoker i = ObjectInvoker.Create(cts);

        Object? r = await i.InvokeAsync("CancelAsync");

        Assert.That(r,Is.Null);
        Assert.That(cts.IsCancellationRequested,Is.True);
    }

    [Test]
    public void SetInstance_WhenAlreadySet_Fails_AndKeepsOriginal()
    {
        SampleTarget t0 = new() { Name = "first" };
        SampleTarget t1 = new() { Name = "second" };

        ObjectInvoker i = new();

        Boolean ok0 = i.SetInstance(t0);
        Boolean ok1 = i.SetInstance(t1);

        Assert.That(ok0,Is.True);
        Assert.That(ok1,Is.False);
        Assert.That(i.Value,Is.SameAs(t0));
    }

    [Test]
    public void SetType_WhenInstanceAlreadySet_Fails()
    {
        SampleTarget t = new();

        ObjectInvoker i = new();

        Boolean ok0 = i.SetInstance(t);
        Boolean ok1 = i.SetType(typeof(StaticOnlyTarget));

        Assert.That(ok0,Is.True);
        Assert.That(ok1,Is.False);
        Assert.That(i.Type,Is.EqualTo(typeof(SampleTarget)));
    }

    [Test]
    public void CreateGeneric_WithAllowStaticMembers_InvokesStaticMethod()
    {
        ObjectInvoker i = ObjectInvoker.Create<StaticOnlyTarget>().WithAllowStaticMembers();

        Object? v = i.Invoke("StaticSum",3,4);

        Assert.That(v,Is.EqualTo(7));
    }

    [Test]
    public void GetField_InstanceField_ReturnsValue()
    {
        FieldTarget t = new() { Code = "alpha" };

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.GetField("Code");

        Assert.That(v,Is.EqualTo("alpha"));
    }

    [Test]
    public void SetField_InstanceField_Succeeds()
    {
        FieldTarget t = new() { Code = "init" };

        ObjectInvoker i = ObjectInvoker.Create(t);

        Boolean ok = i.SetField("Code","beta");

        Assert.That(ok,Is.True);
        Assert.That(t.Code,Is.EqualTo("beta"));
    }

    [Test]
    public void SetField_ReadOnlyOrConst_Fails()
    {
        FieldTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Boolean okReadonly = i.SetField("ReadOnlyCode","x");
        Boolean okConst = i.SetField("ConstCode","y");

        Assert.That(okReadonly,Is.False);
        Assert.That(okConst,Is.False);
        Assert.That(t.ReadOnlyCode,Is.EqualTo("readonly-field"));
        Assert.That(FieldTarget.ConstCode,Is.EqualTo("const-field"));
    }

    [Test]
    public void GetField_StaticField_RequiresAllowStaticMembers()
    {
        ObjectInvoker i0 = new ObjectInvoker().WithType(typeof(FieldTarget));
        ObjectInvoker i1 = new ObjectInvoker().WithType(typeof(FieldTarget)).WithAllowStaticMembers();

        Object? v0 = i0.GetField("StaticCode");
        Object? v1 = i1.GetField("StaticCode");

        Assert.That(v0,Is.Null);
        Assert.That(v1,Is.EqualTo("static-field"));
    }

    [Test]
    public void SetField_StaticField_RequiresAllowStaticMembers()
    {
        FieldTarget.StaticCode = "static-field";

        ObjectInvoker i0 = new ObjectInvoker().WithType(typeof(FieldTarget));
        ObjectInvoker i1 = new ObjectInvoker().WithType(typeof(FieldTarget)).WithAllowStaticMembers();

        Boolean ok0 = i0.SetField("StaticCode","x");
        Boolean ok1 = i1.SetField("StaticCode","z");

        Assert.That(ok0,Is.False);
        Assert.That(ok1,Is.True);
        Assert.That(FieldTarget.StaticCode,Is.EqualTo("z"));
    }

    private sealed class FieldTarget
    {
        public String Code = String.Empty;

        public readonly String ReadOnlyCode = "readonly-field";

        public const String ConstCode = "const-field";

        public static String StaticCode = "static-field";
    }

    private sealed class SampleTarget
    {
        public String Name { get; set; } = String.Empty;

        public Int32 Number { get; set; }

        public String ReadOnlyLabel => "readonly";

        public Int32 Sum(Int32 a , Int32 b) => a + b;

        public String JoinDefault(String a , String b = "right") => $"{a}:{b}";
    }

    private sealed class ParamsTarget
    {
        public String Join(String root , params String[] parts)
        {
            return $"{root}:{String.Join(',',parts)}";
        }
    }

    private class BaseWithStaticPick
    {
        public static String Pick(Int32 value) => "base-static";
    }

    private sealed class DerivedWithStaticBase : BaseWithStaticPick
    {
        public new String Pick(Int32 value) => "instance";
    }

    private sealed class StaticOnlyTarget
    {
        public static String StaticName { get; } = "static-name";

        public static Int32 StaticSum(Int32 a , Int32 b) => a + b;
    }

    private sealed class StaticAsyncTarget
    {
        public static async Task<Int32> StaticSumAsync(Int32 a , Int32 b)
        {
            await Task.Yield(); return a + b;
        }

        public static async ValueTask<String> StaticNameAsync()
        {
            await Task.Yield(); return "static-async-name";
        }
    }

    private sealed class OverloadTarget
    {
        public String Pick(Int32 value) => "int";

        public String Pick(IFormattable value) => "iformattable";
    }

    private sealed class AmbiguousInvokeTarget
    {
        public String Pick(IComparable value) => "icomparable";

        public String Pick(IFormattable value) => "iformattable";
    }

    private sealed class AsyncTarget
    {
        public Boolean Marked { get; private set; }

        public Boolean MarkedValueTask { get; private set; }

        public async Task MarkAsync()
        {
            await Task.Yield(); Marked = true;
        }

        public async Task<Int32> SumAsync(Int32 a , Int32 b)
        {
            await Task.Yield(); return a + b;
        }

        public async ValueTask MarkValueTaskAsync()
        {
            await Task.Yield(); MarkedValueTask = true;
        }

        public async ValueTask<String> NameAsync()
        {
            await Task.Yield(); return "async-name";
        }
    }

    // --- out/ref test targets ---

    private sealed class OutTarget
    {
        public Boolean TryGetValue(String key , out String? value)
        {
            if(String.Equals(key,"found",StringComparison.Ordinal)) { value = "result"; return true; }

            value = null; return false;
        }

        public Boolean TryGetInt(String input , out Int32 value)
        {
            return Int32.TryParse(input,out value);
        }

        public void MultiOut(String input , out Int32 length , out String upper)
        {
            length = input.Length; upper = input.ToUpper();
        }
    }

    private sealed class RefTarget
    {
        public void Increment(ref Int32 value) { value++; }

        public void SwapNames(ref String a , ref String b) { (a,b) = (b,a); }

        public Int32 AddToRef(Int32 x , ref Int32 y) { y = x + y; return y; }
    }

    private sealed class MixedRefOutTarget
    {
        public Boolean TryTransform(ref Int32 value , out String? text)
        {
            text = value.ToString(); value *= 2; return true;
        }
    }

    private sealed class OutOverloadTarget
    {
        public String Pick(Int32 x) => "normal";

        public String Pick(Int32 x , out Int32 y) { y = x * 2; return "out"; }
    }

    private sealed class RefVsByValTarget
    {
        public String Pick(Int32 x) => "byval";

        public String Pick(ref Int32 x) { x++; return "byref"; }
    }

    private sealed class TaskOutTarget
    {
        public Task<Boolean> TryGetWithTask(String key , out String? value)
        {
            if(String.Equals(key,"found",StringComparison.Ordinal)) { value = "task-result"; return Task.FromResult(true); }

            value = null; return Task.FromResult(false);
        }
    }

    private sealed class ParamsWithRefTarget
    {
        public String Combine(ref Int32 count , params String[] items)
        {
            count += items.Length; return String.Join(",",items);
        }
    }

    // --- InvokeDetailed tests ---

    [Test]
    public void InvokeDetailed_OutParameter_ReturnsUpdatedArgs()
    {
        OutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("TryGetValue","found");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(true));
        Assert.That(r.Arguments.Length,Is.EqualTo(2));
        Assert.That(r.Arguments[0],Is.EqualTo("found"));
        Assert.That(r.Arguments[1],Is.EqualTo("result"));
        Assert.That(r.HasByRefArguments,Is.True);
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 1 }));
    }

    [Test]
    public void InvokeDetailed_OutParameter_NotFound_ReturnsNullOut()
    {
        OutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("TryGetValue","missing");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(false));
        Assert.That(r.Arguments[1],Is.Null);
    }

    [Test]
    public void InvokeDetailed_OutParameter_ValueType_ReturnsUpdatedValue()
    {
        OutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("TryGetInt","42");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(true));
        Assert.That(r.Arguments[1],Is.EqualTo(42));
    }

    [Test]
    public void InvokeDetailed_MultipleOutParameters_AllReturned()
    {
        OutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("MultiOut","hello");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.Arguments.Length,Is.EqualTo(3));
        Assert.That(r.Arguments[0],Is.EqualTo("hello"));
        Assert.That(r.Arguments[1],Is.EqualTo(5));
        Assert.That(r.Arguments[2],Is.EqualTo("HELLO"));
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 1 , 2 }));
    }

    [Test]
    public void InvokeDetailed_RefParameter_ReturnsUpdatedValue()
    {
        RefTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("Increment",10);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.Arguments[0],Is.EqualTo(11));
        Assert.That(r.HasByRefArguments,Is.True);
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void InvokeDetailed_TwoRefParameters_BothUpdated()
    {
        RefTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("SwapNames","alpha","beta");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.Arguments[0],Is.EqualTo("beta"));
        Assert.That(r.Arguments[1],Is.EqualTo("alpha"));
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 0 , 1 }));
    }

    [Test]
    public void InvokeDetailed_RefParameter_WithReturnValue()
    {
        RefTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("AddToRef",3,7);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(10));
        Assert.That(r.Arguments[0],Is.EqualTo(3));
        Assert.That(r.Arguments[1],Is.EqualTo(10));
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 1 }));
    }

    [Test]
    public void InvokeDetailed_MixedRefOut_BothUpdated()
    {
        MixedRefOutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("TryTransform",5);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(true));
        Assert.That(r.Arguments[0],Is.EqualTo(10));
        Assert.That(r.Arguments[1],Is.EqualTo("5"));
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 0 , 1 }));
    }

    [Test]
    public void InvokeDetailed_NoByRef_EmptyByRefIndices()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("Sum",2,3);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(5));
        Assert.That(r.HasByRefArguments,Is.False);
        Assert.That(r.ByRefIndices,Is.Empty);
    }

    [Test]
    public void InvokeDetailed_MethodInfo_IsPopulated()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("Sum",2,3);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.Method,Is.Not.Null);
        Assert.That(r.Method.Name,Is.EqualTo("Sum"));
    }

    [Test]
    public void InvokeDetailed_InvalidName_ReturnsNull()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("NonExistentMethod",1);

        Assert.That(r,Is.Null);
    }

    // --- InvokeDetailedAsync tests ---

    [Test]
    public async Task InvokeDetailedAsync_OutParameter_ReturnsUpdatedArgs()
    {
        TaskOutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = await i.InvokeDetailedAsync("TryGetWithTask",[ "found" ]);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(true));
        Assert.That(r.Arguments[1],Is.EqualTo("task-result"));
        Assert.That(r.HasByRefArguments,Is.True);
    }

    [Test]
    public async Task InvokeDetailedAsync_NonAsync_ReturnsResult()
    {
        SampleTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = await i.InvokeDetailedAsync("Sum",[ 4 , 6 ]);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(10));
        Assert.That(r.HasByRefArguments,Is.False);
    }

    [Test]
    public async Task InvokeDetailedAsync_TaskVoid_ReturnsNullReturnValue()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = await i.InvokeDetailedAsync("MarkAsync");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.Null);
        Assert.That(t.Marked,Is.True);
    }

    [Test]
    public async Task InvokeDetailedAsync_TaskOfT_UnwrapsResult()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = await i.InvokeDetailedAsync("SumAsync",[ 7 , 8 ]);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(15));
    }

    [Test]
    public async Task InvokeDetailedAsync_ValueTaskOfT_UnwrapsResult()
    {
        AsyncTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = await i.InvokeDetailedAsync("NameAsync");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo("async-name"));
    }

    // --- existing Invoke/InvokeAsync with out/ref ---

    [Test]
    public void Invoke_OutMethod_ReturnsReturnValueOnly()
    {
        OutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("TryGetValue","found");

        Assert.That(v,Is.EqualTo(true));
    }

    [Test]
    public void Invoke_RefMethod_ReturnsReturnValue()
    {
        RefTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("AddToRef",3,7);

        Assert.That(v,Is.EqualTo(10));
    }

    [Test]
    public async Task InvokeAsync_OutMethod_ReturnsReturnValue()
    {
        TaskOutTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = await i.InvokeAsync("TryGetWithTask",[ "found" ]);

        Assert.That(v,Is.EqualTo(true));
    }

    // --- overload resolution edge cases with out/ref ---

    [Test]
    public void Invoke_OverloadWithAndWithoutOut_PrefersFewerParams()
    {
        OutOverloadTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Pick",5);

        Assert.That(v,Is.EqualTo("normal"));
    }

    [Test]
    public void InvokeDetailed_OverloadWithAndWithoutOut_ExplicitOutArg_SelectsOutOverload()
    {
        OutOverloadTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("Pick",5,0);

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo("out"));
        Assert.That(r.Arguments[1],Is.EqualTo(10));
    }

    [Test]
    public void Invoke_RefVsByVal_Ambiguous_ReturnsNull()
    {
        RefVsByValTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        Object? v = i.Invoke("Pick",5);

        Assert.That(v,Is.Null);
    }

    // --- params + ref ---

    [Test]
    public void InvokeDetailed_ParamsWithRef_RefUpdated_ParamsPacked()
    {
        ParamsWithRefTarget t = new();

        ObjectInvoker i = ObjectInvoker.Create(t);

        InvocationResult? r = i.InvokeDetailed("Combine",0,"a","b","c");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo("a,b,c"));
        Assert.That(r.Arguments[0],Is.EqualTo(3));
        Assert.That(r.ByRefIndices,Is.EqualTo(new[] { 0 }));
    }

    // --- Framework type integration with InvokeDetailed ---

    [Test]
    public void InvokeDetailed_Framework_DictionaryTryGetValue_Works()
    {
        Dictionary<String,Int32> d = new() { ["key"] = 42 };

        ObjectInvoker i = ObjectInvoker.Create(d);

        InvocationResult? r = i.InvokeDetailed("TryGetValue","key");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(true));
        Assert.That(r.Arguments[1],Is.EqualTo(42));
        Assert.That(r.HasByRefArguments,Is.True);
    }

    [Test]
    public void InvokeDetailed_Framework_DictionaryTryGetValue_MissingKey()
    {
        Dictionary<String,Int32> d = new() { ["key"] = 42 };

        ObjectInvoker i = ObjectInvoker.Create(d);

        InvocationResult? r = i.InvokeDetailed("TryGetValue","missing");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(false));
        Assert.That(r.Arguments[1],Is.EqualTo(0));
    }

    [Test]
    public void InvokeDetailed_Framework_IntTryParse_Works()
    {
        ObjectInvoker i = ObjectInvoker.Create<Int32>().WithAllowStaticMembers();

        InvocationResult? r = i.InvokeDetailed("TryParse","123");

        Assert.That(r,Is.Not.Null);
        Assert.That(r!.ReturnValue,Is.EqualTo(true));
        Assert.That(r.Arguments[1],Is.EqualTo(123));
    }
}
