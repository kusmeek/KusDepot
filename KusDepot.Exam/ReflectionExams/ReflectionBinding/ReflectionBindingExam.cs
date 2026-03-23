namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ReflectionBindingExam
{
    [Test]
    public void IsCompatible_ExactMatch_ReturnsTrueAndExact()
    {
        Boolean ok = ReflectionBinding.IsCompatible(typeof(Int32),5,out Boolean exact);

        Assert.That(ok,Is.True);
        Assert.That(exact,Is.True);
    }

    [Test]
    public void IsCompatible_AssignableMatch_ReturnsTrueAndNotExact()
    {
        Boolean ok = ReflectionBinding.IsCompatible(typeof(IFormattable),5,out Boolean exact);

        Assert.That(ok,Is.True);
        Assert.That(exact,Is.False);
    }

    [Test]
    public void IsCompatible_InterfaceHierarchy_StringToIEnumerableChar_ReturnsTrue()
    {
        Boolean ok = ReflectionBinding.IsCompatible(typeof(IEnumerable<Char>),"abc",out Boolean exact);

        Assert.That(ok,Is.True);
        Assert.That(exact,Is.False);
    }

    [Test]
    public void IsCompatible_BaseHierarchy_DerivedToBase_ReturnsTrue()
    {
        Boolean ok = ReflectionBinding.IsCompatible(typeof(Exception),new ArgumentException("x"),out Boolean exact);

        Assert.That(ok,Is.True);
        Assert.That(exact,Is.False);
    }

    [Test]
    public void IsCompatible_NullAgainstReferenceType_ReturnsTrue()
    {
        Boolean ok = ReflectionBinding.IsCompatible(typeof(String),null,out Boolean exact);

        Assert.That(ok,Is.True);
        Assert.That(exact,Is.False);
    }

    [Test]
    public void IsCompatible_NullAgainstNonNullableValueType_ReturnsFalse()
    {
        Boolean ok = ReflectionBinding.IsCompatible(typeof(Int32),null,out Boolean exact);

        Assert.That(ok,Is.False);
        Assert.That(exact,Is.False);
    }

    [Test]
    public void CanAssignNull()
    {
        Assert.That(ReflectionBinding.CanAssignNull(typeof(String)),Is.True);
        Assert.That(ReflectionBinding.CanAssignNull(typeof(Int32?)),Is.True);
        Assert.That(ReflectionBinding.CanAssignNull(typeof(Int32)),Is.False);
    }

    [Test]
    public void GetBindableType_NormalType_ReturnsSameType()
    {
        Type t = ReflectionBinding.GetBindableType(typeof(Int32));

        Assert.That(t,Is.EqualTo(typeof(Int32)));
    }

    [Test]
    public void GetBindableType_ByRefType_ReturnsElementType()
    {
        Type byref = typeof(Int32).MakeByRefType();

        Type t = ReflectionBinding.GetBindableType(byref);

        Assert.That(t,Is.EqualTo(typeof(Int32)));
    }

    [Test]
    public void GetBindableType_ReferenceByRefType_ReturnsElementType()
    {
        Type byref = typeof(String).MakeByRefType();

        Type t = ReflectionBinding.GetBindableType(byref);

        Assert.That(t,Is.EqualTo(typeof(String)));
    }

    [Test]
    public void IsOutParameter_OutParameter_ReturnsTrue()
    {
        ParameterInfo p = typeof(ReflectionBindingExam)
            .GetMethod(nameof(MethodWithOut),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[1];

        Assert.That(ReflectionBinding.IsOutParameter(p),Is.True);
    }

    [Test]
    public void IsOutParameter_RefParameter_ReturnsFalse()
    {
        ParameterInfo p = typeof(ReflectionBindingExam)
            .GetMethod(nameof(MethodWithRef),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[1];

        Assert.That(ReflectionBinding.IsOutParameter(p),Is.False);
    }

    [Test]
    public void IsOutParameter_NormalParameter_ReturnsFalse()
    {
        ParameterInfo p = typeof(ReflectionBindingExam)
            .GetMethod(nameof(MethodWithOut),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[0];

        Assert.That(ReflectionBinding.IsOutParameter(p),Is.False);
    }

    [Test]
    public void IsRefParameter_RefParameter_ReturnsTrue()
    {
        ParameterInfo p = typeof(ReflectionBindingExam)
            .GetMethod(nameof(MethodWithRef),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[1];

        Assert.That(ReflectionBinding.IsRefParameter(p),Is.True);
    }

    [Test]
    public void IsRefParameter_OutParameter_ReturnsFalse()
    {
        ParameterInfo p = typeof(ReflectionBindingExam)
            .GetMethod(nameof(MethodWithOut),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[1];

        Assert.That(ReflectionBinding.IsRefParameter(p),Is.False);
    }

    [Test]
    public void IsRefParameter_NormalParameter_ReturnsFalse()
    {
        ParameterInfo p = typeof(ReflectionBindingExam)
            .GetMethod(nameof(MethodWithRef),BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[0];

        Assert.That(ReflectionBinding.IsRefParameter(p),Is.False);
    }

    [Test]
    public void TryAutoFillOutParameter_ReferenceType_ReturnsNullValue()
    {
        Boolean ok = ReflectionBinding.TryAutoFillOutParameter(typeof(String),out Object? v);

        Assert.That(ok,Is.True);
        Assert.That(v,Is.Null);
    }

    [Test]
    public void TryAutoFillOutParameter_NullableValueType_ReturnsNullValue()
    {
        Boolean ok = ReflectionBinding.TryAutoFillOutParameter(typeof(Int32?),out Object? v);

        Assert.That(ok,Is.True);
        Assert.That(v,Is.Null);
    }

    [Test]
    public void TryAutoFillOutParameter_NonNullableValueType_ReturnsDefault()
    {
        Boolean ok = ReflectionBinding.TryAutoFillOutParameter(typeof(Int32),out Object? v);

        Assert.That(ok,Is.True);
        Assert.That(v,Is.EqualTo(0));
    }

    [Test]
    public void TryAutoFillOutParameter_BoolValueType_ReturnsDefaultFalse()
    {
        Boolean ok = ReflectionBinding.TryAutoFillOutParameter(typeof(Boolean),out Object? v);

        Assert.That(ok,Is.True);
        Assert.That(v,Is.EqualTo(false));
    }

    [Test]
    public void TryAutoFillOutParameter_GuidValueType_ReturnsDefaultGuid()
    {
        Boolean ok = ReflectionBinding.TryAutoFillOutParameter(typeof(Guid),out Object? v);

        Assert.That(ok,Is.True);
        Assert.That(v,Is.EqualTo(Guid.Empty));
    }

    private static void MethodWithOut(Int32 x , out Int32 y) { y = x + 1; }

    private static void MethodWithRef(Int32 x , ref Int32 y) { y = x + y; }
}
