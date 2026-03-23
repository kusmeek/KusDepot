namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class OrleansSerializationExams
{
    [Test]
    public void Orleans_Roundtrip_KusDepotCab_DataSetItem()
    {
        var dsi = DataItemGenerator.CreateDataSet(12);
        var cab = Utility.ToKusDepotCab((IDataItem)dsi)!;

        var rt = OrleansSerialization.RoundTrip(cab);

        Assert.That(rt, Is.Not.Null);

        var back = rt.GetDataItem() as DataSetItem;
        Assert.That(back, Is.Not.Null);
        Check.That(back!.Equals(dsi)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_KusDepotCab_GenericItem_With_Embedded_KeySet()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var ks = new KeySet(new SecurityKey[] { new ManagerKey(cm!, Guid.NewGuid()), new ServiceKey(RandomNumberGenerator.GetBytes(32), Guid.NewGuid()) });
        var owner = ks.CreateOwnerKey("ks-owner");
        Assert.That(ks.Lock(owner), Is.True);

        var gi = new GenericItem();
        Assert.That(gi.SetContent(new List<Object> { ks, "payload",42 }), Is.True);

        var cab = Utility.ToKusDepotCab((IDataItem)gi)!;

        var rt = OrleansSerialization.RoundTrip(cab);

        Assert.That(rt, Is.Not.Null);

        var back = rt.GetDataItem() as GenericItem;
        Assert.That(back, Is.Not.Null);

        var content = back!.GetContent();
        Assert.That(content, Is.Not.Null);

        var embedded = content!.OfType<KeySet>().SingleOrDefault();
        Assert.That(embedded, Is.Not.Null);

        Assert.That(embedded!.GetAllKeys(null), Is.Null);
        var got = embedded.GetAllKeys(owner);
        Assert.That(got, Is.Not.Null);
        Assert.That(got!.Values.Any(k => k is ManagerKey), Is.True);
    }

    [Test]
    public void Orleans_Roundtrip_KusDepotCab_Cargo_Nested()
    {
        var dsi = DataItemGenerator.CreateDataSet(3);
        var gi = new GenericItem();
        Assert.That(gi.SetContent(new List<Object> { "x",7 }), Is.True);

        var inner1 = Utility.ToKusDepotCab((IDataItem)dsi)!;
        var inner2 = Utility.ToKusDepotCab((IDataItem)gi)!;

        var outer = new KusDepotCab
        {
            Data = "outer",
            Type = typeof(String).FullName,
            Cargo = new Dictionary<String, KusDepotCab>(),
            Manifest = new Dictionary<String, String>()
        };

        outer.Cargo!["ds"] = inner1;
        outer.Cargo!["gi"] = inner2;
        outer.Manifest!["m1"] = "v1";

        var rt = OrleansSerialization.RoundTrip(outer);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.Cargo, Is.Not.Null);
        Assert.That(rt.Cargo!.Count, Is.EqualTo(2));
        Assert.That(rt.Cargo.ContainsKey("ds"), Is.True);
        Assert.That(rt.Cargo.ContainsKey("gi"), Is.True);

        var backDs = rt.Cargo["ds"].GetDataItem() as DataSetItem;
        Assert.That(backDs, Is.Not.Null);
        Check.That(backDs!.Equals(dsi)).IsTrue();

        var backGi = rt.Cargo["gi"].GetDataItem() as GenericItem;
        Assert.That(backGi, Is.Not.Null);
        var backGiContent = backGi!.GetContent();
        Assert.That(backGiContent, Is.Not.Null);
        Assert.That(backGiContent!.Count, Is.EqualTo(2));

        Assert.That(rt.Manifest, Is.Not.Null);
        Assert.That(rt.Manifest!.ContainsKey("m1"), Is.True);
    }

    [Test]
    public void Orleans_Roundtrip_KusDepotCab_ManyEntries()
    {
        var outer = new KusDepotCab
        {
            Data = "outer",
            Type = typeof(String).FullName,
            Cargo = new Dictionary<String, KusDepotCab>(),
            Manifest = new Dictionary<String, String>()
        };

        for (Int32 i = 0; i < 100; i++)
        {
            var item = DataItemGenerator.CreateDataSet(2 + (i % 5));
            outer.Cargo![$"k{i}"] = Utility.ToKusDepotCab((IDataItem)item)!;
            outer.Manifest![$"m{i}"] = $"v{i}";
        }

        var rt = OrleansSerialization.RoundTrip(outer);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.Cargo, Is.Not.Null);
        Assert.That(rt.Manifest, Is.Not.Null);
        Assert.That(rt.Cargo!.Count, Is.EqualTo(100));
        Assert.That(rt.Manifest!.Count, Is.EqualTo(100));

        foreach (var i in new[] {0,17,53,99 })
        {
            var inner = rt.Cargo![$"k{i}"];
            var back = inner.GetDataItem() as DataSetItem;
            Assert.That(back, Is.Not.Null);
        }
    }

    [Test]
    public void Orleans_Roundtrip_KusDepotCab_LargePayload_DataSetItem()
    {
        var dsi = DataItemGenerator.CreateDataSet(200);
        var cab = Utility.ToKusDepotCab((IDataItem)dsi)!;

        var rt = OrleansSerialization.RoundTrip(cab);

        Assert.That(rt, Is.Not.Null);
        var back = rt.GetDataItem() as DataSetItem;
        Assert.That(back, Is.Not.Null);
        Check.That(back!.Equals(dsi)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_Descriptor()
    {
        var d = new Descriptor
        {
            Application = "AppA",
            ApplicationVersion = "1.2.3",
            Artist = "Art",
            BornOn = DateTime.UtcNow.ToString("o"),
            Commands = new HashSet<CommandDescriptor>
            {
                new(){ Type = "A" , Specifications = "C" , RegisteredHandles = new(){ "B" } },
                new(){ Type = "X" , Specifications = "Z" , RegisteredHandles = new(){ "Y" } }
            },
            ContentStreamed = true,
            DataType = "typeA",
            DistinguishedName = "CN=X",
            FILE = "/tmp/file.bin",
            ID = Guid.NewGuid(),
            LiveStream = false,
            Modified = DateTime.UtcNow.ToString("o"),
            Name = "NameA",
            Notes = new HashSet<String>{"n1","n2"},
            ObjectType = typeof(DataSetItem).FullName,
            Services = new HashSet<ToolDescriptor>{ new(){ Type = "svc" , Specifications = "spec" , ExtendedData = new(){ "v1" } } },
            ServiceVersion = "2.0",
            Size = "1234",
            Tags = new HashSet<String>{"t1","t2"},
            Title = "TitleA",
            Version = "9.9",
            Year = "2025"
        };

        var rt = OrleansSerialization.RoundTrip(d);
        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.Application, Is.EqualTo(d.Application));
        Assert.That(rt.ApplicationVersion, Is.EqualTo(d.ApplicationVersion));
        Assert.That(rt.Artist, Is.EqualTo(d.Artist));
        Assert.That(rt.BornOn, Is.EqualTo(d.BornOn));
        Assert.That(rt.ContentStreamed, Is.EqualTo(d.ContentStreamed));
        Assert.That(rt.DistinguishedName, Is.EqualTo(d.DistinguishedName));
        Assert.That(rt.FILE, Is.EqualTo(d.FILE));
        Assert.That(rt.ID, Is.EqualTo(d.ID));
        Assert.That(rt.LiveStream, Is.EqualTo(d.LiveStream));
        Assert.That(rt.Modified, Is.EqualTo(d.Modified));
        Assert.That(rt.Name, Is.EqualTo(d.Name));
        Assert.That(rt.ObjectType, Is.EqualTo(d.ObjectType));
        Assert.That(rt.ServiceVersion, Is.EqualTo(d.ServiceVersion));
        Assert.That(rt.Size, Is.EqualTo(d.Size));
        Assert.That(rt.Title, Is.EqualTo(d.Title));
        Assert.That(rt.DataType, Is.EqualTo(d.DataType));
        Assert.That(rt.Version, Is.EqualTo(d.Version));
        Assert.That(rt.Year, Is.EqualTo(d.Year));
        Assert.That(rt.Notes, Is.Not.Null);
        Assert.That(rt.Tags, Is.Not.Null);
        Assert.That(rt.Services, Is.Not.Null);
        Assert.That(rt.Commands, Is.Not.Null);
        Assert.That(rt.Notes!.SetEquals(d.Notes!), Is.True);
        Assert.That(rt.Tags!.SetEquals(d.Tags!), Is.True);
        Assert.That(rt.Services!.Select(_ => $"{_.ID}|{_.Type}|{_.Specifications}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet(),
            Is.EqualTo(d.Services!.Select(_ => $"{_.ID}|{_.Type}|{_.Specifications}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet()));
        Assert.That(rt.Commands!.Select(_ => $"{_.Type}|{_.Specifications}|{String.Join(',',_.RegisteredHandles ?? [])}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet(),
            Is.EqualTo(d.Commands!.Select(_ => $"{_.Type}|{_.Specifications}|{String.Join(',',_.RegisteredHandles ?? [])}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet()));
    }

    [Test]
    public void Orleans_Roundtrip_ManagementRequest()
    {
        var request = new ManagementRequest("management-request");

        var rt = OrleansSerialization.RoundTrip(request);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt, Is.InstanceOf<ManagementRequest>());
        Assert.That(rt.Data, Is.EqualTo(request.Data));
    }

    [Test]
    public void Orleans_Roundtrip_ServiceRequest()
    {
        var request = new ServiceRequest(null,"service-request");

        var rt = OrleansSerialization.RoundTrip(request);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt, Is.InstanceOf<ServiceRequest>());
        Assert.That(rt.Data, Is.EqualTo(request.Data));
        Assert.That(rt.Tool, Is.Null);
    }

    [Test]
    public void Orleans_Roundtrip_StandardRequest()
    {
        var request = new StandardRequest("standard-request");

        var rt = OrleansSerialization.RoundTrip(request);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt, Is.InstanceOf<StandardRequest>());
        Assert.That(rt.Data, Is.EqualTo(request.Data));
    }

    [Test]
    public void Orleans_Roundtrip_OwnerKey()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Own");
        var ok = new OwnerKey(cert!, Guid.NewGuid());

        var rt = OrleansSerialization.RoundTrip(ok);
        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.Equals(ok), Is.True);
    }

    [Test]
    public void Orleans_Roundtrip_CommandDescriptor()
    {
        var d = new CommandDescriptor()
        {
            Type = typeof(CommandTest).FullName,
            Specifications = "specifications",
            ExtendedData = new(){ "ext-0" , "ext-1" },
            RegisteredHandles = new(){ "handle-0" , "handle-1" }
        };

        var rt = OrleansSerialization.RoundTrip(d);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.Type, Is.EqualTo(d.Type));
        Assert.That(rt.Specifications, Is.EqualTo(d.Specifications));
        Assert.That(rt.ExtendedData, Is.Not.Null);
        Assert.That(rt.RegisteredHandles, Is.Not.Null);
        Assert.That(rt.ExtendedData!, Is.EqualTo(d.ExtendedData));
        Assert.That(rt.RegisteredHandles!, Is.EqualTo(d.RegisteredHandles));
    }

    [Test]
    public void Orleans_Roundtrip_ToolDescriptor()
    {
        var d = new ToolDescriptor()
        {
            ID = Guid.NewGuid(),
            Type = typeof(Tool).FullName,
            Specifications = "specifications",
            ExtendedData = new(){ "ext-0" , "ext-1" }
        };

        var rt = OrleansSerialization.RoundTrip(d);

        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.ID, Is.EqualTo(d.ID));
        Assert.That(rt.Type, Is.EqualTo(d.Type));
        Assert.That(rt.Specifications, Is.EqualTo(d.Specifications));
        Assert.That(rt.ExtendedData, Is.Not.Null);
        Assert.That(rt.ExtendedData!, Is.EqualTo(d.ExtendedData));
    }

    [Test]
    public void Orleans_Roundtrip_CommandDetails()
    {
        var cmdId = Guid.NewGuid();
        var args = new Dictionary<String,Object?>
        {
            ["foo"] = "bar",
            ["count"] = 42,
            ["id"] = cmdId
        };

        var wf = new CommandWorkflow();
        wf.Sequence = new SortedList<Int32,String>();
        wf.Sequence.Add(0, "init");
        wf.Sequence.Add(1, "exec");
        wf.EventLog = new Dictionary<String,String>();
        wf.EventLog["e1"] = "alpha";
        wf.EventLog["e2"] = "beta";

        var details = new CommandDetails(arguments: args, handle: "run-task", id: Guid.NewGuid(), workflow: wf);
        details.SetArgument("AttachedToolID", Guid.NewGuid());

        var rt = OrleansSerialization.RoundTrip(details);
        Assert.That(rt, Is.Not.Null);
        Assert.That(rt.Handle, Is.EqualTo(details.Handle));
        Assert.That(rt.ID, Is.EqualTo(details.ID));
        Assert.That(rt.Arguments, Is.Not.Null);
        Assert.That(rt.Arguments!.Count, Is.EqualTo(details.Arguments!.Count));
        Assert.That((String)rt.Arguments["foo"]!, Is.EqualTo("bar"));
        Assert.That((Int32)rt.Arguments["count"]!, Is.EqualTo(42));
        Assert.That((Guid)rt.Arguments["id"]!, Is.EqualTo(cmdId));

        Assert.That(rt.Workflow, Is.Not.Null);
        Assert.That(rt.Workflow!.Sequence, Is.Not.Null);
        Assert.That(rt.Workflow.Sequence!.Count, Is.EqualTo(2));
        Assert.That(rt.Workflow.Sequence[0], Is.EqualTo("init"));
        Assert.That(rt.Workflow.Sequence[1], Is.EqualTo("exec"));
        Assert.That(rt.Workflow.EventLog, Is.Not.Null);
        Assert.That(rt.Workflow.EventLog!.Count, Is.EqualTo(2));
        Assert.That(rt.Workflow.EventLog["e1"], Is.EqualTo("alpha"));
        Assert.That(rt.Workflow.EventLog["e2"], Is.EqualTo("beta"));
    }
    [Test]
    public void Orleans_Roundtrip_DataSetItem()
    {
        var dsi = DataItemGenerator.CreateDataSet(12);
        var rt = OrleansSerialization.RoundTrip(dsi);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(dsi)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_GenericItem_With_Embedded_KeySet()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var ks = new KeySet(new SecurityKey[] { new ManagerKey(cm!, Guid.NewGuid()), new ServiceKey(RandomNumberGenerator.GetBytes(32), Guid.NewGuid()) });
        var owner = ks.CreateOwnerKey("ks-owner");
        Assert.That(ks.Lock(owner), Is.True);

        var gi = new GenericItem();
        Assert.That(gi.SetContent(new List<Object> { ks, "payload",42 }), Is.True);

        var rt = OrleansSerialization.RoundTrip(gi);
        Assert.That(rt, Is.Not.Null);

        var content = rt!.GetContent();
        Assert.That(content, Is.Not.Null);
        var embedded = content!.OfType<KeySet>().SingleOrDefault();
        Assert.That(embedded, Is.Not.Null);
        Assert.That(embedded!.GetAllKeys(null), Is.Null);
        var got = embedded.GetAllKeys(owner);
        Assert.That(got, Is.Not.Null);
        Assert.That(got!.Values.Any(k => k is ManagerKey), Is.True);
    }

    [Test]
    public void Orleans_Roundtrip_BinaryItem()
    {
        var bi = DataFactory.CreateBinaryItem(RandomNumberGenerator.GetBytes(64), dllpaths: new[]{"a.dll","b.dll"}, name: "binA", tags: new[]{"t1"});
        Assert.That(bi, Is.Not.Null);
        var rt = OrleansSerialization.RoundTrip(bi!);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(bi)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_CodeItem()
    {
        var ci = DataFactory.CreateCodeItem("Console.WriteLine(\"hi\");", name: "codeA", tags: new[]{DataTypes.CS});
        Assert.That(ci, Is.Not.Null);
        var rt = OrleansSerialization.RoundTrip(ci!);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(ci)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_GuidReferenceItem()
    {
        var id = Guid.NewGuid();
        var gri = DataFactory.CreateGuidReferenceItem(id, name: "guidRef", notes: new[]{"n1"}, tags: new[]{"ref"});
        Assert.That(gri, Is.Not.Null);
        var rt = OrleansSerialization.RoundTrip(gri!);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(gri)).IsTrue();
        Assert.That(rt!.GetContent(), Is.EqualTo(id));
    }

    [Test]
    public void Orleans_Roundtrip_MSBuildItem()
    {
        var msb = DataFactory.CreateMSBuildItem("<Project></Project>", name: "proj", tags: new[]{"msbuild"});
        var r1 = DataFactory.CreateGenericItem(new Object[] { "Req1", 123 }, name: "req1");
        var s1 = DataFactory.CreateMSBuildItem("<Target></Target>", name: "target1", tags: new[]{"msbuild-seq"});
        Assert.That(msb, Is.Not.Null);
        Assert.That(msb!.SetRequirements(new[] { r1 }), Is.True);
        var s = new SortedList<Int32,MSBuildItem> { [0] = s1! };
        Assert.That(msb!.SetSequence(s), Is.True);
        var rt = OrleansSerialization.RoundTrip(msb!);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(msb)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_MultiMediaItem()
    {
        var mm = DataFactory.CreateMultiMediaItem(RandomNumberGenerator.GetBytes(128), artists: new[]{"Artist1","Artist2"}, name: "mediaA", title: "TitleA", year:2025, tags: new[]{"audio"});
        Assert.That(mm, Is.Not.Null);
        var rt = OrleansSerialization.RoundTrip(mm!);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(mm)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_TextItem()
    {
        var ti = DataFactory.CreateTextItem("Hello World", name: "textA", language: "en", tags: new[]{"plain"});
        Assert.That(ti, Is.Not.Null);
        var rt = OrleansSerialization.RoundTrip(ti!);
        Assert.That(rt, Is.Not.Null);
        Check.That(rt!.Equals(ti)).IsTrue();
    }

    [Test]
    public void Orleans_Roundtrip_KeySet()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var ks = new KeySet(new SecurityKey[] { new ManagerKey(cm!, Guid.NewGuid()), new ServiceKey(RandomNumberGenerator.GetBytes(32), Guid.NewGuid()) });
        var owner = ks.CreateOwnerKey("ks-owner");
        Assert.That(ks.Lock(owner), Is.True);

        var rt = OrleansSerialization.RoundTrip(ks);
        Assert.That(rt, Is.Not.Null);

        Assert.That(rt!.GetAllKeys(null), Is.Null);
        var got = rt.GetAllKeys(owner);
        Assert.That(got, Is.Not.Null);
        Assert.That(got!.Values.Any(k => k is ManagerKey), Is.True);
    }

    [Test]
    public void Orleans_Roundtrip_GenericItem_With_ToolValue_Content()
    {
        var payload = new ToolValue
        {
            Mode = ToolValueMode.Parse,
            Type = typeof(Guid).FullName,
            Data = "11111111-1111-1111-1111-111111111111"
        };

        var gi = new GenericItem();
        Assert.That(gi.SetContent(new List<Object> { "toolvalue", payload }), Is.True);

        var rt = OrleansSerialization.RoundTrip(gi);
        Assert.That(rt, Is.Not.Null);

        var content = rt!.GetContent();
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.EqualTo(2));
        Assert.That(content[0], Is.EqualTo("toolvalue"));
        Assert.That(content[1], Is.InstanceOf<ToolValue>());

        var restored = (ToolValue)content[1];
        Assert.That(restored.Mode, Is.EqualTo(ToolValueMode.Parse));
        Assert.That(restored.Type, Is.EqualTo(typeof(Guid).FullName));
        Assert.That(restored.Data, Is.EqualTo("11111111-1111-1111-1111-111111111111"));
    }

    [Test]
    public void Orleans_Roundtrip_GenericItem_With_ToolDataHierarchy_Content()
    {
        Guid id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        DateTimeOffset stamp = new(2026, 03, 14, 00, 00, 00, TimeSpan.Zero);

        var tooldata = new ToolData { Data = new Version(1, 2, 3, 4) };
        var toolinput = new ToolInput { Data = new Uri("https://example.org/input") };
        var tooloutput = new ToolOutput { ID = id, Data = stamp };

        var gi = new GenericItem();
        Assert.That(gi.SetContent(new List<Object> { tooldata, toolinput, tooloutput }), Is.True);

        var rt = OrleansSerialization.RoundTrip(gi);
        Assert.That(rt, Is.Not.Null);

        var content = rt!.GetContent();
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.EqualTo(3));
        Assert.That(content[0], Is.InstanceOf<ToolData>());
        Assert.That(content[1], Is.InstanceOf<ToolInput>());
        Assert.That(content[2], Is.InstanceOf<ToolOutput>());

        var restoredData = (ToolData)content[0];
        var restoredInput = (ToolInput)content[1];
        var restoredOutput = (ToolOutput)content[2];

        Assert.That(restoredData.Data, Is.InstanceOf<Version>());
        Assert.That(((Version)restoredData.Data!).ToString(), Is.EqualTo("1.2.3.4"));

        Assert.That(restoredInput.Data, Is.InstanceOf<Uri>());
        Assert.That(restoredInput.Data!.ToString(), Is.EqualTo("https://example.org/input"));

        Assert.That(restoredOutput.ID, Is.EqualTo(id));
        Assert.That(restoredOutput.Data, Is.InstanceOf<DateTimeOffset>());
        Assert.That((DateTimeOffset)restoredOutput.Data!, Is.EqualTo(stamp));
    }
}

internal static class OrleansSerialization
{
    private static readonly IServiceProvider Provider = BuildProvider();

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSerializer();
        return services.BuildServiceProvider();
    }

    public static Serializer<T> Get<T>() => Provider.GetRequiredService<Serializer<T>>();

    public static Byte[] Serialize<T>(T value) => Get<T>().SerializeToArray(value);

    public static T Deserialize<T>(Byte[] bytes) => Get<T>().Deserialize(new ArraySegment<Byte>(bytes));

    public static T RoundTrip<T>(T value) => Deserialize<T>(Serialize(value));

    public static T? TryDeserialize<T>(Byte[] bytes)
    {
        try { return Deserialize<T>(bytes); }
        catch { return default; }
    }
}
