namespace KusDepot.Exams;

[TestFixture]
public partial class UtilityExam
{
    [Test]
    public void GenerateDeterministicHashCode_BasicTypes()
    {

        Check.That(GenerateDeterministicHashCode(null)).IsEqualTo(unchecked((Int32)0xBAADF00D));

        Check.That(GenerateDeterministicHashCode(42)).IsEqualTo(GenerateDeterministicHashCode(42));
        Check.That(GenerateDeterministicHashCode(42)).IsNotEqualTo(GenerateDeterministicHashCode(43));

        Check.That(GenerateDeterministicHashCode("hello")).IsEqualTo(GenerateDeterministicHashCode("hello"));
        Check.That(GenerateDeterministicHashCode("hello")).IsNotEqualTo(GenerateDeterministicHashCode("world"));

        var g = Guid.NewGuid();
        Check.That(GenerateDeterministicHashCode(g)).IsEqualTo(GenerateDeterministicHashCode(g));

        Check.That(GenerateDeterministicHashCode(3.14159)).IsEqualTo(GenerateDeterministicHashCode(3.14159));

        Check.That(GenerateDeterministicHashCode(true)).IsNotEqualTo(GenerateDeterministicHashCode(false));
    }

    [Test]
    public void GenerateDeterministicHashCode_ArraysAndCollections()
    {
        var arr1 = new Byte[] { 1, 2, 3, 4 };
        var arr2 = new Byte[] { 1, 2, 3, 4 };
        var arr3 = new Byte[] { 4, 3, 2, 1 };
        Check.That(GenerateDeterministicHashCode(arr1)).IsEqualTo(GenerateDeterministicHashCode(arr2));
        Check.That(GenerateDeterministicHashCode(arr1)).IsNotEqualTo(GenerateDeterministicHashCode(arr3));
        var iarr1 = new Int32[] { 1, 2, 3 };
        var iarr2 = new Int32[] { 1, 2, 3 };
        var iarr3 = new Int32[] { 3, 2, 1 };
        Check.That(GenerateDeterministicHashCode(iarr1)).IsEqualTo(GenerateDeterministicHashCode(iarr2));
        Check.That(GenerateDeterministicHashCode(iarr1)).IsNotEqualTo(GenerateDeterministicHashCode(iarr3));
        var list1 = new List<String> { "a", "b", "c" };
        var list2 = new List<String> { "a", "b", "c" };
        var list3 = new List<String> { "c", "b", "a" };
        Check.That(GenerateDeterministicHashCode(list1)).IsEqualTo(GenerateDeterministicHashCode(list2));
        Check.That(GenerateDeterministicHashCode(list1)).IsNotEqualTo(GenerateDeterministicHashCode(list3));
        var set1 = new HashSet<Int32> { 1, 2, 3 };
        var set2 = new HashSet<Int32> { 3, 2, 1 };
        var set3 = new HashSet<Int32> { 1, 2, 4 };
        Check.That(GenerateDeterministicHashCode(set1)).IsEqualTo(GenerateDeterministicHashCode(set2));
        Check.That(GenerateDeterministicHashCode(set1)).IsNotEqualTo(GenerateDeterministicHashCode(set3));
        var dict1 = new Dictionary<String, Int32> { ["a"] = 1, ["b"] = 2 };
        var dict2 = new Dictionary<String, Int32> { ["b"] = 2, ["a"] = 1 };
        var dict3 = new Dictionary<String, Int32> { ["a"] = 2, ["b"] = 1 };
        Check.That(GenerateDeterministicHashCode(dict1)).IsEqualTo(GenerateDeterministicHashCode(dict2));
        Check.That(GenerateDeterministicHashCode(dict1)).IsNotEqualTo(GenerateDeterministicHashCode(dict3));
    }

    [Test]
    public void GenerateDeterministicHashCode_NestedLists()
    {
        var nested1 = new List<Object> { new Int32[] { 1, 2 }, new List<String> { "x", "y" } };
        var nested2 = new List<Object> { new Int32[] { 1, 2 }, new List<String> { "x", "y" } };
        var nested3 = new List<Object> { new Int32[] { 2, 1 }, new List<String> { "y", "x" } };
        Check.That(GenerateDeterministicHashCode(nested1)).IsEqualTo(GenerateDeterministicHashCode(nested2));
        Check.That(GenerateDeterministicHashCode(nested1)).IsNotEqualTo(GenerateDeterministicHashCode(nested3));
    }

    [Test]
    public void GenerateDeterministicHashCode_Fallback()
    {
        var anon1 = new { X = 1, Y = "foo" };
        var anon2 = new { X = 1, Y = "foo" };
        var anon3 = new { X = 2, Y = "foo" };
        var anon4 = new { Y = "foo", X = 1 };

        Check.That(GenerateDeterministicHashCode(anon1)).IsEqualTo(GenerateDeterministicHashCode(anon2));
        Check.That(GenerateDeterministicHashCode(anon1)).IsNotEqualTo(GenerateDeterministicHashCode(anon3));
        Check.That(GenerateDeterministicHashCode(anon1)).IsNotEqualTo(GenerateDeterministicHashCode(anon4));
    }

    [Test]
    public void GenerateDeterministicHashCode_CollectionOrderSensitivity()
    {
        var list1 = new List<Int32> { 1, 2, 3 };
        var list2 = new List<Int32> { 3, 2, 1 };
        Check.That(GenerateDeterministicHashCode(list1)).IsNotEqualTo(GenerateDeterministicHashCode(list2));

        var arr1 = new[] { "a", "b", "c" };
        var arr2 = new[] { "c", "b", "a" };
        Check.That(GenerateDeterministicHashCode(arr1)).IsNotEqualTo(GenerateDeterministicHashCode(arr2));
    }

    [Test]
    public void GenerateDeterministicHashCode_CollectionOrderInsensitivity()
    {
        var set1 = new HashSet<Int32> { 1, 2, 3 };
        var set2 = new HashSet<Int32> { 3, 2, 1 };
        var set3 = new HashSet<Int32> { 1, 2, 4 };
        Check.That(GenerateDeterministicHashCode(set1)).IsEqualTo(GenerateDeterministicHashCode(set2));
        Check.That(GenerateDeterministicHashCode(set1)).IsNotEqualTo(GenerateDeterministicHashCode(set3));

        var dict1 = new Dictionary<String, Int32> { { "a", 1 }, { "b", 2 } };
        var dict2 = new Dictionary<String, Int32> { { "b", 2 }, { "a", 1 } };
        var dict3 = new Dictionary<String, Int32> { { "a", 1 }, { "b", 3 } };
        Check.That(GenerateDeterministicHashCode(dict1)).IsEqualTo(GenerateDeterministicHashCode(dict2));
        Check.That(GenerateDeterministicHashCode(dict1)).IsNotEqualTo(GenerateDeterministicHashCode(dict3));
    }

    [Test]
    public void GenerateDeterministicHashCode_AdvancedFallback()
    {
        var objA1 = new TestClassA { Prop1 = 1, Prop2 = "Hello" };
        var objA2 = new TestClassA { Prop1 = 1, Prop2 = "Hello" };
        var objA3 = new TestClassA { Prop1 = 2, Prop2 = "Hello" };
        var objB1 = new TestClassB { Prop1 = 1, Prop2 = "Hello" };

        Check.That(GenerateDeterministicHashCode(objA1)).IsEqualTo(GenerateDeterministicHashCode(objA2));
        Check.That(GenerateDeterministicHashCode(objA1)).IsNotEqualTo(GenerateDeterministicHashCode(objA3));
        Check.That(GenerateDeterministicHashCode(objA1)).IsNotEqualTo(GenerateDeterministicHashCode(objB1));
    }

    [Test]
    public void GenerateDeterministicHashCode_EdgeCases()
    {
        Check.That(GenerateDeterministicHashCode(UInt64.MaxValue)).IsEqualTo(GenerateDeterministicHashCode(UInt64.MaxValue));
        Check.That(GenerateDeterministicHashCode(UInt64.MaxValue)).IsNotEqualTo(GenerateDeterministicHashCode(UInt64.MinValue));

        var nestedWithNulls = new List<Object?> { 1, "two", null, new List<Object?> { 3, null } };
        var nestedWithNulls2 = new List<Object?> { 1, "two", null, new List<Object?> { 3, null } };
        Check.That(GenerateDeterministicHashCode(nestedWithNulls)).IsEqualTo(GenerateDeterministicHashCode(nestedWithNulls2));

        Check.That(GenerateDeterministicHashCode(new List<Int32>())).IsNotEqualTo(GenerateDeterministicHashCode(new List<String>()));
        Check.That(GenerateDeterministicHashCode(new Dictionary<Int32,Int32>())).IsNotEqualTo(GenerateDeterministicHashCode(new List<Int32>()));
    }

    [Test]
    public void GenerateDeterministicHashCode_PrimitiveTypes()
    {
        var ts1 = TimeSpan.FromSeconds(12345);
        var ts2 = TimeSpan.FromSeconds(12345);
        var ts3 = TimeSpan.FromSeconds(12346);
        Check.That(GenerateDeterministicHashCode(ts1)).IsEqualTo(GenerateDeterministicHashCode(ts2));
        Check.That(GenerateDeterministicHashCode(ts1)).IsNotEqualTo(GenerateDeterministicHashCode(ts3));

        var v1 = new Version(1, 2);
        var v2 = Version.Parse("1.2");
        var v3 = new Version(1, 2, 3, 4);
        Check.That(GenerateDeterministicHashCode(v1)).IsEqualTo(GenerateDeterministicHashCode(v2));
        Check.That(GenerateDeterministicHashCode(v1)).IsNotEqualTo(GenerateDeterministicHashCode(v3));

        var ua1 = new Uri("https://example.com/a/b?x=1");
        var ua2 = new Uri("https://example.com/a/b?x=1");
        var ua3 = new Uri("https://example.com/a/b?x=2");
        Check.That(GenerateDeterministicHashCode(ua1)).IsEqualTo(GenerateDeterministicHashCode(ua2));
        Check.That(GenerateDeterministicHashCode(ua1)).IsNotEqualTo(GenerateDeterministicHashCode(ua3));
        var ur1 = new Uri("relative/path", UriKind.Relative);
        var ur2 = new Uri("relative/path", UriKind.Relative);
        var ur3 = new Uri("relative/other", UriKind.Relative);
        Check.That(GenerateDeterministicHashCode(ur1)).IsEqualTo(GenerateDeterministicHashCode(ur2));
        Check.That(GenerateDeterministicHashCode(ur1)).IsNotEqualTo(GenerateDeterministicHashCode(ur3));

        var big1 = BigInteger.Parse("123456789012345678901234567890");
        var big2 = BigInteger.Parse("123456789012345678901234567890");
        var big3 = BigInteger.Parse("123456789012345678901234567891");
        Check.That(GenerateDeterministicHashCode(big1)).IsEqualTo(GenerateDeterministicHashCode(big2));
        Check.That(GenerateDeterministicHashCode(big1)).IsNotEqualTo(GenerateDeterministicHashCode(big3));

        Half h1 = (Half)1.5f;
        Half h2 = (Half)1.5f;
        Half h3 = (Half)2.0f;
        Check.That(GenerateDeterministicHashCode(h1)).IsEqualTo(GenerateDeterministicHashCode(h2));
        Check.That(GenerateDeterministicHashCode(h1)).IsNotEqualTo(GenerateDeterministicHashCode(h3));

        IntPtr ip1 = new IntPtr(123456);
        IntPtr ip2 = new IntPtr(123456);
        IntPtr ip3 = new IntPtr(654321);
        Check.That(GenerateDeterministicHashCode(ip1)).IsEqualTo(GenerateDeterministicHashCode(ip2));
        Check.That(GenerateDeterministicHashCode(ip1)).IsNotEqualTo(GenerateDeterministicHashCode(ip3));

        UIntPtr up1 = new UIntPtr(1234u);
        UIntPtr up2 = new UIntPtr(1234u);
        UIntPtr up3 = new UIntPtr(4321u);
        Check.That(GenerateDeterministicHashCode(up1)).IsEqualTo(GenerateDeterministicHashCode(up2));
        Check.That(GenerateDeterministicHashCode(up1)).IsNotEqualTo(GenerateDeterministicHashCode(up3));
    }

    private class TestClassA { public Int32 Prop1 { get; set; } public String? Prop2 { get; set; } }
    private class TestClassB { public Int32 Prop1 { get; set; } public String? Prop2 { get; set; } }

    [Test]
    public void GenerateDeterministicHashCode_GenericItem()
    {
        var nestedId = Guid.NewGuid();
        var nestedItem1 = new GenericItem(new Object[] { "nested" }); nestedItem1.SetID(nestedId);
        var nestedItem2 = new GenericItem(new Object[] { "nested" }); nestedItem2.SetID(nestedId);
        var nestedItem3 = new GenericItem(new Object[] { "changed" }); nestedItem3.SetID(nestedId);
        var nestedItem4 = new GenericItem(new Object[] { "nested" });

        var parentId = Guid.NewGuid();
        var parent1 = new GenericItem(new Object[] { nestedItem1 }); parent1.SetID(parentId);
        var parent2 = new GenericItem(new Object[] { nestedItem2 }); parent2.SetID(parentId);
        var parent3 = new GenericItem(new Object[] { nestedItem3 }); parent3.SetID(parentId);
        var parent4 = new GenericItem(new Object[] { nestedItem4 }); parent4.SetID(parentId);

        Check.That(parent1.GetHashCode()).IsEqualTo(parent2.GetHashCode());
        Check.That(parent1.GetHashCode()).IsNotEqualTo(parent3.GetHashCode());
        Check.That(parent1.GetHashCode()).IsNotEqualTo(parent4.GetHashCode());

        var id = Guid.NewGuid();
        var listContent1 = new GenericItem(new Object[] { new List<Int32> { 1, 2, 3 } }); listContent1.SetID(id);
        var listContent2 = new GenericItem(new Object[] { new List<Int32> { 1, 2, 3 } }); listContent2.SetID(id);
        var listContent3 = new GenericItem(new Object[] { new List<Int32> { 3, 2, 1 } }); listContent3.SetID(id);
        Check.That(listContent1.GetHashCode()).IsEqualTo(listContent2.GetHashCode());
        Check.That(listContent1.GetHashCode()).IsNotEqualTo(listContent3.GetHashCode());

        var setContent1 = new GenericItem(new Object[] { new HashSet<Int32> { 1, 2, 3 } }); setContent1.SetID(id);
        var setContent2 = new GenericItem(new Object[] { new HashSet<Int32> { 3, 2, 1 } }); setContent2.SetID(id);
        var setContent3 = new GenericItem(new Object[] { new HashSet<Int32> { 1, 2, 4 } }); setContent3.SetID(id);
        Check.That(setContent1.GetHashCode()).IsEqualTo(setContent2.GetHashCode());
        Check.That(setContent1.GetHashCode()).IsNotEqualTo(setContent3.GetHashCode());

        var deepContent1 = new Object[] { 123, "hello", new List<String> { "a", "b" }, new GenericItem(new Object[] { 456 }) };
        var deepItem1 = new GenericItem(deepContent1); deepItem1.SetID(id);
        var deepItem2 = new GenericItem(deepContent1); deepItem2.SetID(id);
        var deepContent3 = new Object[] { 123, "hello", new List<String> { "a", "c" }, new GenericItem(new Object[] { 456 }) };
        var deepItem3 = new GenericItem(deepContent3); deepItem3.SetID(id);

        Check.That(deepItem1.GetHashCode()).IsEqualTo(deepItem2.GetHashCode());
        Check.That(deepItem1.GetHashCode()).IsNotEqualTo(deepItem3.GetHashCode());
        Check.That(deepItem3.GetHashCode()).IsEqualTo(deepItem3.GetHashCode());

        var emptyContentItem1 = new GenericItem(Array.Empty<Object>()); emptyContentItem1.SetID(id);
        var emptyContentItem2 = new GenericItem(Array.Empty<Object>()); emptyContentItem2.SetID(id);
        var nullContentItem = new GenericItem(null); nullContentItem.SetID(id);
        Check.That(emptyContentItem1.GetHashCode()).IsEqualTo(emptyContentItem2.GetHashCode());
        Check.That(emptyContentItem1.GetHashCode()).IsEqualTo(nullContentItem.GetHashCode());

        var withNulls1 = new GenericItem(new Object?[] { 1, "two", null, new List<Object?> { 3, null } }); withNulls1.SetID(id);
        var withNulls2 = new GenericItem(new Object?[] { 1, "two", null, new List<Object?> { 3, null } }); withNulls2.SetID(id);
        var withNulls3 = new GenericItem(new Object?[] { 1, "two", null, new List<Object?> { 4, null } }); withNulls3.SetID(id);
        Check.That(withNulls1.GetHashCode()).IsEqualTo(withNulls2.GetHashCode());
        Check.That(withNulls1.GetHashCode()).IsNotEqualTo(withNulls3.GetHashCode());
    }
}
