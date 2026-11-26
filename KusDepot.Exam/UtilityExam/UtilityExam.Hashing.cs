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
    public void GenerateDeterministicHashCode_FramingAndExactScalarIdentity()
    {
        Check.That(GenerateDeterministicHashCode(new List<String> { "ab", "c" })).IsNotEqualTo(GenerateDeterministicHashCode(new List<String> { "a", "bc" }));
        Check.That(GenerateDeterministicHashCode(1)).IsNotEqualTo(GenerateDeterministicHashCode(1L));
        Check.That(GenerateDeterministicHashCode(1.0m)).IsNotEqualTo(GenerateDeterministicHashCode(1.00m));
        Check.That(GenerateDeterministicHashCode(BitConverter.Int64BitsToDouble(0))).IsNotEqualTo(GenerateDeterministicHashCode(BitConverter.Int64BitsToDouble(unchecked((Int64)0x8000000000000000))));
        Check.That(GenerateDeterministicSHA512(new Object?[] { null })).IsNotEqualTo(GenerateDeterministicSHA512(Array.Empty<Object?>()));
    }

    [Test]
    public void GenerateDeterministicSHA512_CanonicalTypeShapesRemainDistinct()
    {
        Check.That(GenerateDeterministicSHA512(new TypeOuter<Int32>.Left { Value = 7 })).IsNotEqualTo(GenerateDeterministicSHA512(new TypeOuter<Int32>.Right { Value = 7 }));
        Check.That(GenerateDeterministicSHA512(new Type[] { typeof(TypeOuter<Int32>.Left) })).IsNotEqualTo(GenerateDeterministicSHA512(new Type[] { typeof(TypeOuter<Int32>.Right) }));
        Check.That(GenerateDeterministicSHA512(typeof(TypeOuter<Int32>.Nested<String>.Leaf<Guid>))).IsNotEqualTo(GenerateDeterministicSHA512(typeof(TypeOuter<Int32>.Nested<Double>.Leaf<Guid>)));
        Check.That(GenerateDeterministicSHA512(typeof(TypeOuter<Int32>.Nested<String>.Leaf<Guid>))).IsNotEqualTo(GenerateDeterministicSHA512(typeof(TypeOuter<Int32>.Nested<String>.Leaf<Int64>)));
        Check.That(GenerateDeterministicSHA512(typeof(Int32[]))).IsNotEqualTo(GenerateDeterministicSHA512(typeof(Int32).MakeArrayType(1)));
        Check.That(GenerateDeterministicSHA512(typeof(List<>).GetGenericArguments()[0])).IsNotEqualTo(GenerateDeterministicSHA512(typeof(HashSet<>).GetGenericArguments()[0]));
        Check.That(GenerateDeterministicSHA512(typeof(MethodOwnerOne).GetMethod(nameof(MethodOwnerOne.Method))!.GetGenericArguments()[0])).IsNotEqualTo(GenerateDeterministicSHA512(typeof(MethodOwnerTwo).GetMethod(nameof(MethodOwnerTwo.Method))!.GetGenericArguments()[0]));
    }

    [Test]
    public void GenerateDeterministicHashCode_ConcurrentBagSubclassIsOrderInsensitive()
    {
        Check.That(GenerateDeterministicSHA512(new TestConcurrentBag<Int32>(new[] { 1,2 }))).IsEqualTo(GenerateDeterministicSHA512(new TestConcurrentBag<Int32>(new[] { 2,1 })));
        Check.That(GenerateDeterministicSHA512(new ConcurrentQueue<Int32>(new[] { 1,2 }))).IsNotEqualTo(GenerateDeterministicSHA512(new ConcurrentQueue<Int32>(new[] { 2,1 })));
    }

    private class TypeOuter<T>
    {
        public class Left { public Int32 Value { get; set; } }
        public class Right { public Int32 Value { get; set; } }
        public class Nested<U>
        {
            public class Leaf<V> { }
        }
    }

    private class MethodOwnerOne { public void Method<T>() { } }
    private class MethodOwnerTwo { public void Method<T>() { } }

    private sealed class TestConcurrentBag<T>(IEnumerable<T> values) : ConcurrentBag<T>(values);

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

        var cd1 = new ConcurrentDictionary<String, Int32>(new[] { new KeyValuePair<String,Int32>("a",1), new KeyValuePair<String,Int32>("b",2) });
        var cd2 = new ConcurrentDictionary<String, Int32>(new[] { new KeyValuePair<String,Int32>("b",2), new KeyValuePair<String,Int32>("a",1) });
        var cd3 = new ConcurrentDictionary<String, Int32>(new[] { new KeyValuePair<String,Int32>("a",1), new KeyValuePair<String,Int32>("b",3) });
        Check.That(GenerateDeterministicHashCode(cd1)).IsEqualTo(GenerateDeterministicHashCode(cd2));
        Check.That(GenerateDeterministicHashCode(cd1)).IsNotEqualTo(GenerateDeterministicHashCode(cd3));

        var fs1 = new[] { 1, 2, 3 }.ToFrozenSet();
        var fs2 = new[] { 3, 2, 1 }.ToFrozenSet();
        var fs3 = new[] { 1, 2, 4 }.ToFrozenSet();
        Check.That(GenerateDeterministicHashCode(fs1)).IsEqualTo(GenerateDeterministicHashCode(fs2));
        Check.That(GenerateDeterministicHashCode(fs1)).IsNotEqualTo(GenerateDeterministicHashCode(fs3));
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

    [Test]
    public void GenerateDeterministicHashCode_GraphIdentity()
    {
        var sharedChild1 = new GraphNode { Value = 2 };
        var sharedParent1 = new GraphNode { Value = 1 , Left = sharedChild1 , Right = sharedChild1 };

        var sharedChild2 = new GraphNode { Value = 2 };
        var sharedParent2 = new GraphNode { Value = 1 , Left = sharedChild2 , Right = sharedChild2 };

        var duplicatedParent = new GraphNode { Value = 1 , Left = new GraphNode { Value = 2 } , Right = new GraphNode { Value = 2 } };

        Check.That(GenerateDeterministicHashCode(sharedParent1)).IsEqualTo(GenerateDeterministicHashCode(sharedParent2));
        Check.That(GenerateDeterministicHashCode(sharedParent1)).IsNotEqualTo(GenerateDeterministicHashCode(duplicatedParent));
    }

    [Test]
    public void GenerateDeterministicHashCode_GraphIdentity_Cycles()
    {
        var cycle1 = new GraphNode { Value = 1 }; cycle1.Left = cycle1;
        var cycle2 = new GraphNode { Value = 1 }; cycle2.Left = cycle2;
        var acyclic = new GraphNode { Value = 1 , Left = new GraphNode { Value = 1 } };

        Check.That(GenerateDeterministicHashCode(cycle1)).IsEqualTo(GenerateDeterministicHashCode(cycle2));
        Check.That(GenerateDeterministicHashCode(cycle1)).IsNotEqualTo(GenerateDeterministicHashCode(acyclic));
    }

    [Test]
    public void GenerateDeterministicHashCode_GraphIdentity_Aliases()
    {
        var child = new GraphNode { Value = 5 };
        var shared = new GraphNode { Value = 1 , Left = child , Right = child };
        var duplicated = new GraphNode { Value = 1 , Left = new GraphNode { Value = 5 } , Right = new GraphNode { Value = 5 } };
        Check.That(GenerateDeterministicSHA512(shared)).IsNotEqualTo(GenerateDeterministicSHA512(duplicated));
    }

    [Test]
    public async Task GenerateDeterministicHashCodeAsync_GraphIdentity_ParityWithSync()
    {
        var sharedChild = new GraphNode { Value = 3 };
        var sharedParent = new GraphNode { Value = 2 , Left = sharedChild , Right = sharedChild };
        var cycle = new GraphNode { Value = 4 }; cycle.Left = cycle;

        Check.That(await GenerateDeterministicHashCodeAsync(sharedParent).ConfigureAwait(false)).IsEqualTo(GenerateDeterministicHashCode(sharedParent));
        Check.That(await GenerateDeterministicHashCodeAsync(cycle).ConfigureAwait(false)).IsEqualTo(GenerateDeterministicHashCode(cycle));
    }

    private class GraphNode
    {
        public Int32 Value { get; set; }
        public GraphNode? Left { get; set; }
        public GraphNode? Right { get; set; }
    }

    private class TestClassA { public Int32 Prop1 { get; set; } public String? Prop2 { get; set; } }
    private class TestClassB { public Int32 Prop1 { get; set; } public String? Prop2 { get; set; } }

    [Test]
    public void GenericItem_GetHashCode_ContentSensitivity()
    {
        using var isolation = DataIsolation.BeginScope(DataIsolationMode.Disabled);

        var nestedId = Guid.NewGuid();
        var nestedItem1 = new GenericItem(new Object[] { "nested" }); nestedItem1.SetID(nestedId);
        var nestedItem2 = new GenericItem(new Object[] { "nested" }); nestedItem2.SetID(nestedId);
        var nestedItem3 = new GenericItem(new Object[] { "changed" }); nestedItem3.SetID(nestedId);
        var nestedItem4 = new GenericItem(new Object[] { "nested" }); nestedItem4.SetID(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var parent1 = new GenericItem(new Object[] { nestedItem1 }); parent1.SetID(parentId);
        var parent2 = new GenericItem(new Object[] { nestedItem2 }); parent2.SetID(parentId);
        var parent3 = new GenericItem(new Object[] { nestedItem3 }); parent3.SetID(parentId);
        var parent4 = new GenericItem(new Object[] { nestedItem4 }); parent4.SetID(parentId);

        Check.That(parent1.GetHashCode()).IsEqualTo(parent2.GetHashCode());
        Check.That(parent1.Equals(parent3)).IsFalse();
        Check.That(parent1.Equals(parent4)).IsTrue();
        Check.That(parent1.GetHashCode()).IsEqualTo(parent4.GetHashCode());

        var id = Guid.NewGuid();
        var sharedList = new List<Int32> { 1, 2, 3 };
        var listContent1 = new GenericItem(new Object[] { sharedList }); listContent1.SetID(id);
        var listContent2 = new GenericItem(new Object[] { sharedList }); listContent2.SetID(id);
        var listContent3 = new GenericItem(new Object[] { new List<Int32> { 3, 2, 1 } }); listContent3.SetID(id);
        Check.That(listContent1.Equals(listContent2)).IsTrue();
        Check.That(listContent1.GetHashCode()).IsEqualTo(listContent2.GetHashCode());
        Check.That(listContent1.Equals(listContent3)).IsFalse();

        var sharedSet = new HashSet<Int32> { 1, 2, 3 };
        var setContent1 = new GenericItem(new Object[] { sharedSet }); setContent1.SetID(id);
        var setContent2 = new GenericItem(new Object[] { sharedSet }); setContent2.SetID(id);
        var setContent3 = new GenericItem(new Object[] { new HashSet<Int32> { 1, 2, 4 } }); setContent3.SetID(id);
        Check.That(setContent1.Equals(setContent2)).IsTrue();
        Check.That(setContent1.GetHashCode()).IsEqualTo(setContent2.GetHashCode());
        Check.That(setContent1.Equals(setContent3)).IsFalse();

        var deepContent1 = new Object[] { 123, "hello", new List<String> { "a", "b" }, new GenericItem(new Object[] { 456 }) };
        var deepItem1 = new GenericItem(deepContent1); deepItem1.SetID(id);
        var deepItem2 = new GenericItem(deepContent1); deepItem2.SetID(id);
        var deepContent3 = new Object[] { 123, "hello", new List<String> { "a", "c" }, new GenericItem(new Object[] { 456 }) };
        ((GenericItem)deepContent3[3]).SetID(((GenericItem)deepContent1[3]).GetID());
        var deepItem3 = new GenericItem(deepContent3); deepItem3.SetID(id);

        Check.That(deepItem1.Equals(deepItem2)).IsTrue();
        Check.That(deepItem1.GetHashCode()).IsEqualTo(deepItem2.GetHashCode());
        Check.That(deepItem1.Equals(deepItem3)).IsFalse();
        Check.That(deepItem3.GetHashCode()).IsEqualTo(deepItem3.GetHashCode());

        var emptyContentItem1 = new GenericItem(Array.Empty<Object>()); emptyContentItem1.SetID(id);
        var emptyContentItem2 = new GenericItem(Array.Empty<Object>()); emptyContentItem2.SetID(id);
        var nullContentItem = new GenericItem(null); nullContentItem.SetID(id);
        Check.That(emptyContentItem1.Equals(emptyContentItem2)).IsTrue();
        Check.That(emptyContentItem1.GetHashCode()).IsEqualTo(emptyContentItem2.GetHashCode());
        Check.That(emptyContentItem1.Equals(nullContentItem)).IsTrue();
        Check.That(emptyContentItem1.GetHashCode()).IsEqualTo(nullContentItem.GetHashCode());

        var sharedNullableList = new List<Object?> { 3, null };
        var withNulls1 = new GenericItem(new Object?[] { 1, "two", null, sharedNullableList }); withNulls1.SetID(id);
        var withNulls2 = new GenericItem(new Object?[] { 1, "two", null, sharedNullableList }); withNulls2.SetID(id);
        var withNulls3 = new GenericItem(new Object?[] { 1, "two", null, new List<Object?> { 4, null } }); withNulls3.SetID(id);
        Check.That(withNulls1.Equals(withNulls2)).IsTrue();
        Check.That(withNulls1.GetHashCode()).IsEqualTo(withNulls2.GetHashCode());
        Check.That(withNulls1.Equals(withNulls3)).IsFalse();
    }

    [Test]
    public void GenerateDeterministicHashCode_GenericItem()
    {
        var sharedId = Guid.NewGuid();
        var sameIdentity1 = new GenericItem(new Object[] { "content-a" }); sameIdentity1.SetID(sharedId);
        var sameIdentity2 = new GenericItem(new Object[] { "content-b" }); sameIdentity2.SetID(sharedId);
        var differentIdentity = new GenericItem(new Object[] { "content-a" }); differentIdentity.SetID(Guid.NewGuid());

        Check.That(GenerateDeterministicHashCode(sameIdentity1)).IsNotEqualTo(GenerateDeterministicHashCode(sameIdentity2));
        Check.That(GenerateDeterministicHashCode(sameIdentity1)).IsNotEqualTo(GenerateDeterministicHashCode(differentIdentity));

        Object[] graph1 = [sameIdentity1];
        Object[] graph2 = [sameIdentity2];
        Object[] graph3 = [differentIdentity];

        Check.That(GenerateDeterministicHashCode(graph1)).IsNotEqualTo(GenerateDeterministicHashCode(graph2));
        Check.That(GenerateDeterministicHashCode(graph1)).IsNotEqualTo(GenerateDeterministicHashCode(graph3));
    }

    [Test]
    public async Task GenerateDeterministicHashCodeAsync_ParityWithSync()
    {
        var samples = new Object?[]
        {
            null,
            123,
            "async",
            new List<String>{"one","two"},
            new Dictionary<String,Int32>{{"a",1},{"b",2}},
            new HashSet<Int32>{3,2,1},
            new List<Object?>{ new Int32[]{4,5}, "nested", null }
        };

        foreach(var sample in samples)
        {
            var syncHash = GenerateDeterministicHashCode(sample);
            var asyncHash = await GenerateDeterministicHashCodeAsync(sample).ConfigureAwait(false);
            Check.That(asyncHash).IsEqualTo(syncHash);
        }
    }

    [Test]
    public async Task ComputeSHA512Async_ParityWithHashData_AcrossThresholds()
    {
        Byte[] small = Encoding.UTF8.GetBytes("chunked hashing parity");
        Byte[] large = Enumerable.Range(0,10*MiB + (32 * KiB)).Select(i => (Byte)(i % 251)).ToArray();

        Byte[] expectedSmall = SHA512.HashData(small);
        Byte[] expectedLarge = SHA512.HashData(large);

        Byte[] actualSmallArray = await ComputeSHA512Async(small,4 * KiB).ConfigureAwait(false);
        Byte[] actualSmallMemory = await ComputeSHA512Async((ReadOnlyMemory<Byte>)small,4 * KiB).ConfigureAwait(false);
        Byte[] actualLargeArray = await ComputeSHA512Async(large,MiB).ConfigureAwait(false);
        Byte[] actualLargeMemory = await ComputeSHA512Async((ReadOnlyMemory<Byte>)large,MiB).ConfigureAwait(false);

        Check.That(actualSmallArray).IsEqualTo(expectedSmall);
        Check.That(actualSmallMemory).IsEqualTo(expectedSmall);
        Check.That(actualLargeArray).IsEqualTo(expectedLarge);
        Check.That(actualLargeMemory).IsEqualTo(expectedLarge);
    }

    [Test]
    public void GenerateDeterministicSHA512_Null_UsesDeterministicNullMarker()
    {
        Byte[] expected = SHA512.HashData(BitConverter.GetBytes(unchecked((Int32)0xBAADF00D)));

        Check.That(GenerateDeterministicSHA512(null)).IsEqualTo(expected);
    }

    [Test]
    public async Task GenerateDeterministicSHA512Async_ParityWithSync()
    {
        var samples = new Object?[]
        {
            null,
            123,
            "async-sha",
            new List<String>{"one","two"},
            new Dictionary<String,Int32>{{"a",1},{"b",2}},
            new HashSet<Int32>{3,2,1},
            new List<Object?>{ new Int32[]{4,5}, "nested", null }
        };

        foreach(var sample in samples)
        {
            Byte[] syncHash = GenerateDeterministicSHA512(sample);
            Byte[] asyncHash = await GenerateDeterministicSHA512Async(sample).ConfigureAwait(false);
            Check.That(asyncHash).IsEqualTo(syncHash);
        }
    }

    [Test]
    public void GenerateDeterministicSHA512_MetaBaseIntegrity_IgnoresLocalFileBindingsAndContentHashing()
    {
        var contentOnlyLeft = new GenericItem(new Object[] { "content-a" });
        contentOnlyLeft.SetName("Name");
        contentOnlyLeft.SetApplication("App");
        contentOnlyLeft.SetServiceVersion(new Version(9,8,7,6));

        var contentOnlyRight = new GenericItem(new Object[] { "content-b" });
        contentOnlyRight.SetName("Name");
        contentOnlyRight.SetApplication("App");
        contentOnlyRight.SetBornOn(contentOnlyLeft.GetBornOn());
        contentOnlyRight.SetServiceVersion(new Version(9,8,7,6));

        Check.That(contentOnlyLeft.GetMetaBaseIntegrityHash()).IsEqualTo(contentOnlyRight.GetMetaBaseIntegrityHash());

        var left = new GenericItem(new Object[] { "content-a" });
        left.SetName("Name");
        left.SetApplication("App");
        left.SetApplicationVersion(new Version(1,2,3,4));
        left.SetBornOn(DateTimeOffset.Parse("2025-01-01T00:00:00+00:00"));
        left.SetDomainID("domain.example");
        left.SetDistinguishedName("CN=Name,DC=example,DC=org");
        left.SetSecurityDescriptor("O:BAG:BAD:(A;;FA;;;SY)");
        left.SetServiceVersion(new Version(9,8,7,6));
        left.SetVersion(new Version(2,0,0,0));
        left.SetTags(new[] { "one" , "two" });
        left.SetNotes(new[] { "note-a" , "note-b" });
        left.SetFILE("c:\\temp\\content-a.bin");
        left.SetObjectFILE("c:\\temp\\meta-a.kd");

        var nestedLeft = new GenericItem(new Object[] { "nested-a" });
        nestedLeft.SetName("Nested");
        nestedLeft.SetApplication("NestedApp");
        nestedLeft.SetServiceVersion(new Version(3,0));

        left.SetExtension(new Dictionary<String,Object?>
        {
            ["Nested"] = nestedLeft,
            ["Mode"] = "A"
        });

        var right = new GenericItem(new Object[] { "content-b" });
        right.SetName("Name");
        right.SetApplication("App");
        right.SetApplicationVersion(new Version(1,2,3,4));
        right.SetBornOn(DateTimeOffset.Parse("2025-01-01T00:00:00+00:00"));
        right.SetDomainID("domain.example");
        right.SetDistinguishedName("CN=Name,DC=example,DC=org");
        right.SetSecurityDescriptor("O:BAG:BAD:(A;;FA;;;SY)");
        right.SetServiceVersion(new Version(9,8,7,6));
        right.SetVersion(new Version(2,0,0,0));
        right.SetTags(new[] { "two" , "one" });
        right.SetNotes(new[] { "note-b" , "note-a" });
        right.SetFILE("d:\\other\\content-b.bin");
        right.SetObjectFILE("d:\\other\\meta-b.kd");

        var nestedRight = new GenericItem(new Object[] { "nested-a" });
        nestedRight.SetID(nestedLeft.GetID());
        nestedRight.SetName("Nested");
        nestedRight.SetApplication("NestedApp");
        nestedRight.SetBornOn(nestedLeft.GetBornOn());
        nestedRight.SetServiceVersion(new Version(3,0));

        right.SetExtension(new Dictionary<String,Object?>
        {
            ["Nested"] = nestedRight,
            ["Mode"] = "A"
        });

        Check.That(left.GetMetaBaseIntegrityHash()).IsEqualTo(right.GetMetaBaseIntegrityHash());

        right.SetServiceVersion(new Version(9,8,7,7));

        Check.That(left.GetMetaBaseIntegrityHash()).IsNotEqualTo(right.GetMetaBaseIntegrityHash());
    }

    [Test]
    public void GenerateDeterministicSHA512_MetaBaseIntegrity_RootDataItemReferenceUsesContentView()
    {
        Guid rootId = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        FieldInfo contentField = typeof(GenericItem).GetField("Content",BindingFlags.NonPublic|BindingFlags.Instance) ?? throw new InvalidOperationException("Content field not found");

        GenericItem CreateGraph(String childValue)
        {
            GenericItem root = new();
            GenericItem child = new();
            Check.That(root.SetID(rootId)).IsTrue();
            Check.That(child.SetID(childId)).IsTrue();
            Check.That(root.SetBornOn(DateTimeOffset.Parse("2025-01-01T00:00:00+00:00"))).IsTrue();
            Check.That(root.SetExtension(new Dictionary<String,Object?> { ["Child"] = child })).IsTrue();
            contentField.SetValue(root,new List<Object> { child });
            contentField.SetValue(child,new List<Object> { root,childValue });
            return root;
        }

        Byte[] first = CreateGraph("child-a").GetMetaBaseIntegrityHash();
        Byte[] equivalent = CreateGraph("child-a").GetMetaBaseIntegrityHash();
        Byte[] changedContent = CreateGraph("child-b").GetMetaBaseIntegrityHash();

        Check.That(first).IsEqualTo(equivalent);
        Check.That(first).IsNotEqualTo(changedContent);
    }

    [Test]
    public async Task GenerateDeterministicHashCodeAsync_GenericItem()
    {
        var nestedId = Guid.NewGuid();
        var nestedItem1 = new GenericItem(new Object[] { "nested" }); nestedItem1.SetID(nestedId);
        var nestedItem2 = new GenericItem(new Object[] { "nested" }); nestedItem2.SetID(nestedId);
        var nestedItem3 = new GenericItem(new Object[] { "changed" }); nestedItem3.SetID(nestedId);
        var nestedItem4 = new GenericItem(new Object[] { "nested" }); nestedItem4.SetID(Guid.NewGuid());

        var parentId = Guid.NewGuid();
        var parent1 = new GenericItem(new Object[] { nestedItem1 }); parent1.SetID(parentId);
        var parent2 = new GenericItem(new Object[] { nestedItem2 }); parent2.SetID(parentId);
        var parent3 = new GenericItem(new Object[] { nestedItem3 }); parent3.SetID(parentId);
        var parent4 = new GenericItem(new Object[] { nestedItem4 }); parent4.SetID(parentId);

        var hash1 = await GenerateDeterministicHashCodeAsync(parent1).ConfigureAwait(false);
        var hash2 = await GenerateDeterministicHashCodeAsync(parent2).ConfigureAwait(false);
        var hash3 = await GenerateDeterministicHashCodeAsync(parent3).ConfigureAwait(false);
        var hash4 = await GenerateDeterministicHashCodeAsync(parent4).ConfigureAwait(false);

        Check.That(hash1).IsEqualTo(hash2);
        Check.That(hash1).IsNotEqualTo(hash3);
        Check.That(hash1).IsNotEqualTo(hash4);

        var id = Guid.NewGuid();
        var deepContent1 = new Object[] { 123, "hello", new List<String> { "a", "b" }, new GenericItem(new Object[] { 456 }) };
        var deepItem1 = new GenericItem(deepContent1); deepItem1.SetID(id);
        var deepItem2 = new GenericItem(deepContent1); deepItem2.SetID(id);
        var deepContent3 = new Object[] { 123, "hello", new List<String> { "a", "c" }, new GenericItem(new Object[] { 456 }) };
        ((GenericItem)deepContent3[3]).SetID(((GenericItem)deepContent1[3]).GetID());
        var deepItem3 = new GenericItem(deepContent3); deepItem3.SetID(id);

        var asyncDeep1 = await GenerateDeterministicHashCodeAsync(deepItem1).ConfigureAwait(false);
        var asyncDeep2 = await GenerateDeterministicHashCodeAsync(deepItem2).ConfigureAwait(false);
        var asyncDeep3 = await GenerateDeterministicHashCodeAsync(deepItem3).ConfigureAwait(false);

        Check.That(asyncDeep1).IsEqualTo(asyncDeep2);
        Check.That(asyncDeep1).IsNotEqualTo(asyncDeep3);
    }
}
