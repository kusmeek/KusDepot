namespace KusDepot.Exams.DataItems;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class GenericItemContentExam
{
    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }
    }

    private static void SetSameID(params GenericItem[] items)
    {
        var id = Guid.NewGuid();
        foreach (var gi in items) gi.SetID(id);
    }

    [Test]
    public void Hash_Primitives_And_CommonTypes()
    {
        var a1 = new GenericItem(new Object[] { 123, "abc", true, Guid.Parse("11111111-1111-1111-1111-111111111111"), 1.23, 4.56m, new BigInteger(999999), TimeSpan.FromMinutes(5), new DateTime(2024, 01, 02, 03, 04, 05, DateTimeKind.Utc), new DateTimeOffset(2024, 01, 02, 03, 04, 05, TimeSpan.Zero) });
        var a2 = new GenericItem(new Object[] { 123, "abc", true, Guid.Parse("11111111-1111-1111-1111-111111111111"), 1.23, 4.56m, new BigInteger(999999), TimeSpan.FromMinutes(5), new DateTime(2024, 01, 02, 03, 04, 05, DateTimeKind.Utc), new DateTimeOffset(2024, 01, 02, 03, 04, 05, TimeSpan.Zero) });
        var b = new GenericItem(new Object[] { 124, "abd", false, Guid.Parse("22222222-2222-2222-2222-222222222222"), 1.24, 4.57m, new BigInteger(1000000), TimeSpan.FromMinutes(6), new DateTime(2024, 01, 02, 03, 04, 06, DateTimeKind.Utc), new DateTimeOffset(2024, 01, 02, 03, 04, 06, TimeSpan.Zero) });
        SetSameID(a1, a2, b);
        Check.That(a1.GetHashCode()).IsEqualTo(a2.GetHashCode());
        Check.That(a1.GetHashCode()).IsNotEqualTo(b.GetHashCode());
    }

    [Test]
    public void Hash_Uri_Version_IntPtr_UIntPtr()
    {
        var gi1 = new GenericItem(new Object[] { new Uri("https://example.com/x"), new Version(1,2,3,4), new IntPtr(1234), new UIntPtr(5678) });
        var gi2 = new GenericItem(new Object[] { new Uri("https://example.com/x"), new Version(1,2,3,4), new IntPtr(1234), new UIntPtr(5678) });
        var gi3 = new GenericItem(new Object[] { new Uri("https://example.com/y"), new Version(2,3,4,5), new IntPtr(9999), new UIntPtr(42) });
        SetSameID(gi1, gi2, gi3);
        Check.That(gi1.GetHashCode()).IsEqualTo(gi2.GetHashCode());
        Check.That(gi1.GetHashCode()).IsNotEqualTo(gi3.GetHashCode());
    }

    [Test]
    public void Hash_Arrays_And_Lists()
    {
        var bytes = new Byte[] { 1,2,3,4,5 };
        var chars = new Char[] { 'a','b','c' };
        var list1 = new List<Int32> { 1,2,3 };
        var list2 = new List<Int32> { 1,2,3 };
        var list3 = new List<Int32> { 3,2,1 };
        var gi1 = new GenericItem(new Object[] { bytes, chars, list1 });
        var gi2 = new GenericItem(new Object[] { (Byte[])bytes.Clone(), (Char[])chars.Clone(), list2 });
        var gi3 = new GenericItem(new Object[] { new Byte[] { 1,2,3,4 }, new Char[] { 'a','b','c','d' }, list3 });
        SetSameID(gi1, gi2, gi3);
        Check.That(gi1.GetHashCode()).IsEqualTo(gi2.GetHashCode());
        Check.That(gi1.GetHashCode()).IsNotEqualTo(gi3.GetHashCode());
    }

    [Test]
    public void Hash_Sets_And_Dictionaries_OrderIndependent()
    {
        var set1 = new HashSet<Int32> { 1,2,3 };
        var set2 = new HashSet<Int32> { 3,2,1 };
        var dict1 = new Dictionary<String,Int32> { ["a"] = 1, ["b"] = 2 };
        var dict2 = new Dictionary<String,Int32> { ["b"] = 2, ["a"] = 1 };
        var dict3 = new Dictionary<String,Int32> { ["a"] = 1, ["b"] = 3 };
        var gi1 = new GenericItem(new Object[] { set1, dict1 });
        var gi2 = new GenericItem(new Object[] { set2, dict2 });
        var gi3 = new GenericItem(new Object[] { set2, dict3 });
        SetSameID(gi1, gi2, gi3);
        Check.That(gi1.GetHashCode()).IsEqualTo(gi2.GetHashCode());
        Check.That(gi1.GetHashCode()).IsNotEqualTo(gi3.GetHashCode());
    }

    [Test]
    public void Hash_ValueTuple_And_KeyValuePair()
    {
        var t1 = (A: 1, B: "x");
        var t2 = (A: 1, B: "x");
        var t3 = (A: 2, B: "y");
        var kv1 = new KeyValuePair<String,Int32>("k", 7);
        var kv2 = new KeyValuePair<String,Int32>("k", 7);
        var kv3 = new KeyValuePair<String,Int32>("k", 8);
        var gi1 = new GenericItem(new Object[] { t1, kv1 });
        var gi2 = new GenericItem(new Object[] { t2, kv2 });
        var gi3 = new GenericItem(new Object[] { t3, kv3 });
        SetSameID(gi1, gi2, gi3);
        Check.That(gi1.GetHashCode()).IsEqualTo(gi2.GetHashCode());
        Check.That(gi1.GetHashCode()).IsNotEqualTo(gi3.GetHashCode());
    }

    [Test]
    public void Serialization_Roundtrip_MixedContent()
    {
        var content = new Object[]
        {
            123, "abc", true,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            new Byte[]{9,8,7}, new Char[]{'x','y'},
            new DateTime(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc),
            new DateTimeOffset(2024, 01, 01, 00, 00, 00, TimeSpan.Zero),
            TimeSpan.FromSeconds(30), 3.14, 2.71m,
            new BigInteger(1234567890),
            new Uri("https://example.com"),
            new Version(1,0,0,0),
            new Int32[] { 4,5,6 },
            new List<String> { "one", "two" },
            new HashSet<Guid> { Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc") },
            DataItemGenerator.CreateDataSet(2)
        };
        var gi = new GenericItem(content);
        var str = gi.ToString();
        var back = GenericItem.Parse(str);
        Check.That(back).IsNotNull();
        var restored = back!.GetContent();
        Check.That(restored).IsNotNull();
        Check.That(restored!.Count).IsEqualTo(content.Length);

        Check.That(restored[0]).IsInstanceOf<Int32>();
        Check.That(restored[1]).IsInstanceOf<String>();
        Check.That(restored[4]).IsInstanceOf<Byte[]>();
        Check.That(restored[5]).IsInstanceOf<Char[]>();
        Check.That(restored[12]).IsInstanceOf<Uri>();
        Check.That(restored[13]).IsInstanceOf<Version>();
        Check.That(restored[14]).IsInstanceOf<Int32[]>();
        Check.That(restored[15]).IsInstanceOf<List<String>>();
        Check.That(restored[16]).IsInstanceOf<HashSet<Guid>>();
        Check.That(restored[17]).IsInstanceOf<DataSetItem>();

        Check.That((Int32)restored[0]).IsEqualTo(123);
        Check.That((String)restored[1]).IsEqualTo("abc");
        Check.That(((Byte[])restored[4]).SequenceEqual(new Byte[]{9,8,7})).IsTrue();
        Check.That(((Char[])restored[5]).SequenceEqual(new Char[]{'x','y'})).IsTrue();
        Check.That(((Int32[])restored[14]).SequenceEqual(new Int32[]{4,5,6})).IsTrue();
        Check.That(((List<String>)restored[15]).SequenceEqual(new[]{"one","two"})).IsTrue();
        var set = (HashSet<Guid>)restored[16];
        Check.That(set.Contains(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"))).IsTrue();
        Check.That(set.Contains(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"))).IsTrue();
    }

    [Test]
    public void Serialization_Roundtrip_Nested_GenericItem()
    {
        var child = new GenericItem(new Object[] { "child", 7 });
        var parent = new GenericItem(new Object[] { "parent", child });
        var s = parent.ToString();
        var back = GenericItem.Parse(s);
        Check.That(back).IsNotNull();
        var content = back!.GetContent();
        Check.That(content).IsNotNull();
        Check.That(content![1]).IsInstanceOf<GenericItem>();
        var restoredChild = (GenericItem)content[1];
        var rc = restoredChild.GetContent();
        Check.That(rc).IsNotNull();
        Check.That(rc![0]).IsEqualTo("child");
        Check.That(rc[1]).IsEqualTo(7);
    }
}
