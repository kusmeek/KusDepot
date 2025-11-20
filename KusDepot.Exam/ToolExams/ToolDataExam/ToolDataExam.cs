namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ToolDataExam
{
    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }
    }

    private static Byte[] Bytes(Byte seed)
    {
        var data = new Byte[32];
        for (Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return data;
    }

    [Test]
    public void ToolData_ToString_Parse_Roundtrip_Primitives()
    {
        var td = new ToolData { Data = new List<Object?> { 123, "abc", true, Guid.NewGuid(), 3.14, 2.71m, DateTime.UtcNow, DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5) } };
        var s = td.ToString();
        var parsed = ToolData.Parse(s);
        Check.That(parsed).IsNotNull();
        var arr = (IEnumerable<Object>)parsed!.Data!;
        Check.That(arr).IsNotNull();
        Check.That(arr.Count()).IsEqualTo(9);
    }

    [Test]
    public void ToolData_ToString_Parse_Roundtrip_Collections_Mixed()
    {
        var payload = new List<Object?>
        {
            new List<String?> { "one", "two" },
            new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            new Dictionary<String,Object?> { ["a"] = 1, ["b"] = "two" },
            new Byte[] { 1,2,3 },
            new Char[] { 'x','y' }
        };
        var td = new ToolData { Data = payload };
        var s = td.ToString();
        var parsed = ToolData.Parse(s);
        Check.That(parsed).IsNotNull();
        var back = (List<Object?>)parsed!.Data!;
        Check.That(back.Count).IsEqualTo(payload.Count);
        Check.That(back[0]).IsInstanceOf<List<String?>>();
        Check.That(back[1]).IsInstanceOf<HashSet<Guid>>();
        Check.That(back[2]).IsInstanceOf<Dictionary<String,Object?>>();
        Check.That(back[3]).IsInstanceOf<Byte[]>();
        Check.That(back[4]).IsInstanceOf<Char[]>();
    }

    [Test]
    public void ToolData_ToString_Parse_Roundtrip_DataItems()
    {
        var ti = new TextItem("hello");
        var bi = new BinaryItem(new Byte[] { 9,8,7 });
        var gi = new GenericItem(new Object[] { "nested", 42 });
        var td = new ToolData { Data = new List<Object?> { ti, bi, gi } };
        var s = td.ToString();
        var parsed = ToolData.Parse(s);
        Check.That(parsed).IsNotNull();
        var arr = ((IEnumerable<Object>)parsed!.Data!).Cast<Object>().ToArray();
        var ti2 = (TextItem)arr[0];
        var bi2 = (BinaryItem)arr[1];
        var gi2 = (GenericItem)arr[2];
        Check.That(ti2.GetContent()).IsEqualTo(ti.GetContent());
        Check.That(bi2.GetContent()!).IsEqualTo(bi.GetContent()!);
        Check.That(gi2.GetContent()).IsNotNull();
    }

    [Test]
    public void ToolData_TryParse_Roundtrip()
    {
        var td = new ToolData { Data = new Dictionary<String,Int32> { ["a"] = 1, ["b"] = 2 } };
        var s = td.ToString();
        var ok = ToolData.TryParse(s, null, out var parsed);
        Check.That(ok).IsTrue();
        Check.That(parsed).IsNotNull();
        var back = (Dictionary<String,Int32>)parsed!.Data!;
        Check.That(back["a"]).IsEqualTo(1);
        Check.That(back["b"]).IsEqualTo(2);
    }

    [Test]
    public void ToolInput_Roundtrip_MixedData()
    {
        var ti = new ToolInput { Data = new List<Object?> { 1, "two", new Byte[] { 1,2 }, new GuidReferenceItem(Guid.NewGuid()) } };
        var s = ti.ToString();
        var parsed = ToolInput.Parse(s);
        Check.That(parsed).IsNotNull();
        var back = (List<Object?>)parsed!.Data!;
        Check.That(back.Count).IsEqualTo(4);
        Check.That(back[3]).IsInstanceOf<GuidReferenceItem>();
    }

    [Test]
    public void ToolOutput_Roundtrip_WithID()
    {
        var id = Guid.NewGuid();
        var to = new ToolOutput { ID = id, Data = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() } };
        var s = to.ToString();
        var parsed = ToolOutput.Parse(s);
        Check.That(parsed).IsNotNull();
        Check.That(parsed!.ID).IsEqualTo(id);
        Check.That(parsed.Data).IsNotNull();
    }

    [Test]
    public void WorkflowExceptionData_Roundtrip_Create()
    {
        try
        {
            throw new InvalidOperationException("boom");
        }
        catch (Exception ex)
        {
            var wd = WorkflowExceptionData.Create(ex);
            var s = wd.ToString();
            var parsed = WorkflowExceptionData.Parse(s);
            Check.That(parsed).IsNotNull();
            Check.That(parsed!.Type).IsEqualTo(wd.Type);
            Check.That(parsed.Message).IsEqualTo(wd.Message);
            Check.That(parsed.StackTrace).IsNotNull();
        }
    }

    [Test]
    public void ToolData_Data_KusDepotCab_Roundtrip_And_Use()
    {
        var ti = new TextItem("inside cab");
        var cab = Utility.ToKusDepotCab((IDataItem)ti)!;
        var td = new ToolData { Data = cab };
        var s = td.ToString();
        var parsed = ToolData.Parse(s);
        Check.That(parsed).IsNotNull();
        var backCab = (KusDepotCab)parsed!.Data!;
        var gotItem = backCab.GetDataItem() as TextItem;
        Check.That(gotItem).IsNotNull();
        Check.That(gotItem!.GetContent()).IsEqualTo("inside cab");
    }

    [Test]
    public void ToolData_Data_SecurityKey_And_KeySet_Roundtrip()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var sk = new ServiceKey(Bytes(10), Guid.NewGuid());
        var mk = new ManagerKey(cm!, Guid.NewGuid());
        var ks = new KeySet(new SecurityKey[] { sk, mk });
        var td = new ToolData { Data = new List<Object?> { sk, mk, ks } };
        var s = td.ToString();
        var parsed = ToolData.Parse(s);
        Check.That(parsed).IsNotNull();
        var arr = ((IEnumerable<Object>)parsed!.Data!).Cast<Object>().ToArray();
        Check.That(arr[0]).IsInstanceOf<ServiceKey>();
        Check.That(arr[1]).IsInstanceOf<ManagerKey>();
        Check.That(arr[2]).IsInstanceOf<KeySet>();
        var backKs = (KeySet)arr[2];
        var all = backKs.GetAllKeys(null);
        Check.That(all).IsNotNull();
        Check.That(all!.Values.Any(k => k is ServiceKey)).IsTrue();
        Check.That(all!.Values.Any(k => k is ManagerKey)).IsTrue();
    }

    [Test]
    public void ToolData_Data_GenericItem_Roundtrip()
    {
        var inner = new GenericItem(new Object[] { "nested", 99 });
        var td = new ToolData { Data = inner };
        var s = td.ToString();
        var parsed = ToolData.Parse(s);
        Check.That(parsed).IsNotNull();
        var gi2 = (GenericItem)parsed!.Data!;
        var content = gi2.GetContent();
        Check.That(content).IsNotNull();
        Check.That(content![0]).IsEqualTo("nested");
        Check.That(content[1]).IsEqualTo(99);
    }
}
