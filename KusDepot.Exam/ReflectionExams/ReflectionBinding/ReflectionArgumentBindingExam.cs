namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ReflectionArgumentBindingExam
{
    [Test]
    public void TryCreateInvokeArgs_Array_WithDefaultValue_BindsAndScores()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(NeedsStringAndInt),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "left" ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke!.Length,Is.EqualTo(2));
        Assert.That(invoke[0],Is.EqualTo("left"));
        Assert.That(invoke[1],Is.EqualTo(5));
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_ExplicitNullForNonNullable_Fails()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(NeedsStringAndInt),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "left" , null ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.False);
        Assert.That(invoke,Is.Null);
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_NullAndEmpty_AreEquivalent()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(NeedsStringAndInt),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean okNull = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:(Object?[]?)null,
            allowdefaultvalues:true,
            out Object?[]? invokeNull,
            out Int32 exactNull,
            out Int32 assignableNull);

        Boolean okEmpty = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[],
            allowdefaultvalues:true,
            out Object?[]? invokeEmpty,
            out Int32 exactEmpty,
            out Int32 assignableEmpty);

        Assert.That(okNull,Is.True);
        Assert.That(okEmpty,Is.True);
        Assert.That(invokeNull,Is.Not.Null);
        Assert.That(invokeEmpty,Is.Not.Null);
        Assert.That(invokeNull![0],Is.Null);
        Assert.That(invokeEmpty![0],Is.Null);
        Assert.That(invokeNull[1],Is.EqualTo(5));
        Assert.That(invokeEmpty[1],Is.EqualTo(5));
        Assert.That(exactNull,Is.EqualTo(0));
        Assert.That(assignableNull,Is.EqualTo(0));
        Assert.That(exactEmpty,Is.EqualTo(0));
        Assert.That(assignableEmpty,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_DefaultValueOff_FailsOnMissingNonNullable()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(NeedsStringAndInt),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "left" ],
            allowdefaultvalues:false,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.False);
        Assert.That(invoke,Is.Null);
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Dictionary_ExplicitNullVsMissing_AffectsScoring()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(NeedsNullableAndInt),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Dictionary<Int32,Object?> explicitNull = new() { [0] = null };
        Dictionary<Int32,Object?> missing = [];

        Boolean okNull = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            explicitNull,
            allowdefaultvalues:true,
            out Object?[]? invokeNull,
            out Int32 exactNull,
            out Int32 assignableNull);

        Boolean okMissing = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            missing,
            allowdefaultvalues:true,
            out Object?[]? invokeMissing,
            out Int32 exactMissing,
            out Int32 assignableMissing);

        Assert.That(okNull,Is.True);
        Assert.That(okMissing,Is.True);
        Assert.That(invokeNull,Is.Not.Null);
        Assert.That(invokeMissing,Is.Not.Null);
        Assert.That(invokeNull![0],Is.Null);
        Assert.That(invokeMissing![0],Is.Null);
        Assert.That(invokeNull[1],Is.EqualTo(9));
        Assert.That(invokeMissing[1],Is.EqualTo(9));
        Assert.That(exactNull,Is.EqualTo(0));
        Assert.That(assignableNull,Is.EqualTo(1));
        Assert.That(exactMissing,Is.EqualTo(0));
        Assert.That(assignableMissing,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Dictionary_SparseNonNullableLeading_Fails()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(NeedsIntAndInt),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Dictionary<Int32,Object?> sparse = new() { [1] = 7 };

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            sparse,
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.False);
        Assert.That(invoke,Is.Null);
        Assert.That(exact,Is.EqualTo(0));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_OutParameter_AutoFilled_WhenOmitted()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(TryParseStyle),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "hello" ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke!.Length,Is.EqualTo(2));
        Assert.That(invoke[0],Is.EqualTo("hello"));
        Assert.That(invoke[1],Is.Null);
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_OutParameter_ValueTypeAutoFilled_WhenOmitted()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(IntTryParseStyle),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "42" ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke!.Length,Is.EqualTo(2));
        Assert.That(invoke[0],Is.EqualTo("42"));
        Assert.That(invoke[1],Is.EqualTo(0));
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_OutParameter_ExplicitValue_Accepted()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(TryParseStyle),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "hello" , "placeholder" ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke![1],Is.EqualTo("placeholder"));
        Assert.That(exact,Is.EqualTo(2));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_OutParameter_AutoFilled_ScoresZero()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(TryParseStyle),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "hello" ],
            allowdefaultvalues:true,
            out _,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_RefParameter_Required_FailsWhenOmitted()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(AddToRef),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ 5 ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.False);
        Assert.That(invoke,Is.Null);
    }

    [Test]
    public void TryCreateInvokeArgs_Array_RefParameter_Supplied_BindsAndScores()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(AddToRef),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ 5 , 10 ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke![0],Is.EqualTo(5));
        Assert.That(invoke[1],Is.EqualTo(10));
        Assert.That(exact,Is.EqualTo(2));
        Assert.That(assignable,Is.EqualTo(0));
    }

    [Test]
    public void TryCreateInvokeArgs_Array_MixedRefOut_TrailingOutOmitted_Succeeds()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(MixedRefOut),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ 5 , 10 ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke!.Length,Is.EqualTo(3));
        Assert.That(invoke[0],Is.EqualTo(5));
        Assert.That(invoke[1],Is.EqualTo(10));
        Assert.That(invoke[2],Is.Null);
    }

    [Test]
    public void TryCreateInvokeArgs_Array_MixedRefOut_RefOmitted_Fails()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(MixedRefOut),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ 5 ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out _,
            out _);

        Assert.That(ok,Is.False);
        Assert.That(invoke,Is.Null);
    }

    [Test]
    public void TryCreateInvokeArgs_Array_MultipleOut_AllAutoFilled()
    {
        ParameterInfo[] p = typeof(ReflectionArgumentBindingExam)
            .GetMethod(nameof(MultipleOuts),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters();

        Boolean ok = ReflectionArgumentBinding.TryCreateInvokeArgs(
            p,
            arguments:[ "input" ],
            allowdefaultvalues:true,
            out Object?[]? invoke,
            out Int32 exact,
            out Int32 assignable);

        Assert.That(ok,Is.True);
        Assert.That(invoke,Is.Not.Null);
        Assert.That(invoke!.Length,Is.EqualTo(3));
        Assert.That(invoke[0],Is.EqualTo("input"));
        Assert.That(invoke[1],Is.EqualTo(0));
        Assert.That(invoke[2],Is.Null);
        Assert.That(exact,Is.EqualTo(1));
        Assert.That(assignable,Is.EqualTo(0));
    }

    private static void NeedsStringAndInt(String a , Int32 b = 5) {}

    private static void NeedsNullableAndInt(Int32? a , Int32 b = 9) {}

    private static void NeedsIntAndInt(Int32 a , Int32 b = 9) {}

    private static Boolean TryParseStyle(String input , out String? result) { result = input; return true; }

    private static Boolean IntTryParseStyle(String input , out Int32 result) { result = Int32.Parse(input); return true; }

    private static void AddToRef(Int32 x , ref Int32 y) { y = x + y; }

    private static void MixedRefOut(Int32 x , ref Int32 y , out String? z) { y = x + y; z = y.ToString(); }

    private static void MultipleOuts(String input , out Int32 length , out String? upper) { length = input.Length; upper = input.ToUpper(); }
}
