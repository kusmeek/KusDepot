namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class KusDepotCabExam
{
    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }
    }

    private static KusDepotCab MakeCab(DataItem item) => Utility.ToKusDepotCab((IDataItem)item)!;
    private static KusDepotCab MakeCab(SecurityKey key) => Utility.ToKusDepotCab(key)!;

    private static Byte[] Bytes(Byte seed)
    {
        var data = new Byte[32];
        for (Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return data;
    }

    private static GenericItem MakeGenericWith(params Object[] content)
    {
        var gi = new GenericItem();
        Check.That(gi.SetContent(content.ToList())).IsTrue();
        return gi;
    }

    [Test]
    public void Roundtrip_TextItem()
    {
        var ti = new TextItem("hello world");
        var cab = MakeCab(ti);
        var parsed = KusDepotCab.Parse(cab.ToString());
        Check.That(parsed).IsNotNull();
        var back = parsed!.GetDataItem() as TextItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetContent()).IsEqualTo(ti.GetContent());
    }

    [Test]
    public void Roundtrip_CodeItem()
    {
        var ci = new CodeItem("int x = 1;");
        var cab = MakeCab(ci);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as CodeItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetContent()).IsEqualTo(ci.GetContent());
    }

    [Test]
    public void Roundtrip_BinaryItem()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var bi = new BinaryItem(bytes);
        var cab = MakeCab(bi);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as BinaryItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetContent()!).IsEqualTo(bytes);
    }

    [Test]
    public void Roundtrip_MultiMediaItem()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var mi = new MultiMediaItem(bytes);
        var cab = MakeCab(mi);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as MultiMediaItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetContent()!).IsEqualTo(bytes);
    }

    [Test]
    public void Roundtrip_MSBuildItem()
    {
        var mbi = new MSBuildItem();
        var cab = MakeCab(mbi);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as MSBuildItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetID()).IsEqualTo(mbi.GetID());
    }

    [Test]
    public void Roundtrip_DataSetItem()
    {
        var dsi = new DataSetItem();
        var cab = MakeCab(dsi);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as DataSetItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetID()).IsEqualTo(dsi.GetID());
    }

    [Test]
    public void Roundtrip_GuidReferenceItem()
    {
        var guid = Guid.NewGuid();
        var gri = new GuidReferenceItem(guid);
        var cab = MakeCab(gri);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as GuidReferenceItem;
        Check.That(back).IsNotNull();
        Check.That(back!.GetContent()).IsEqualTo(guid);
    }

    [Test]
    public void Roundtrip_GenericItem_With_Embedded_KeySet()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var ks = new KeySet(new SecurityKey[]{ new ManagerKey(cm!, Guid.NewGuid()), new ServiceKey(RandomNumberGenerator.GetBytes(32), Guid.NewGuid()) });
        var owner = ks.CreateOwnerKey("ks-owner");
        Check.That(ks.Lock(owner)).IsTrue();

        var gi = MakeGenericWith(ks, "payload", 42);
        var cab = MakeCab(gi);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetDataItem() as GenericItem;
        Check.That(back).IsNotNull();
        var content = back!.GetContent();
        Check.That(content).IsNotNull();
        var embedded = content!.OfType<KeySet>().SingleOrDefault();
        Check.That(embedded).IsNotNull();
        Check.That(embedded!.GetAllKeys(null)).IsNull();
        var got = embedded.GetAllKeys(owner);
        Check.That(got).IsNotNull();
        Check.That(got!.Values.Any(k => k is ManagerKey)).IsTrue();
    }

    [Test]
    public void Roundtrip_SecurityKey_HostKey()
    {
        var id = Guid.NewGuid();
        var k = new HostKey(Bytes(1), id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as HostKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_MyHostKey()
    {
        var id = Guid.NewGuid();
        var k = new MyHostKey(Bytes(2), id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as MyHostKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_ClientKey()
    {
        var id = Guid.NewGuid();
        var k = new ClientKey(Bytes(3), id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as ClientKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_ServiceKey()
    {
        var id = Guid.NewGuid();
        var k = new ServiceKey(Bytes(4), id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as ServiceKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_CommandKey()
    {
        var id = Guid.NewGuid();
        var k = new CommandKey(Bytes(5), id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as CommandKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_ExecutiveKey()
    {
        var id = Guid.NewGuid();
        var k = new ExecutiveKey(Bytes(6), id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as ExecutiveKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_ManagerKey()
    {
        var id = Guid.NewGuid();
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var k = new ManagerKey(cert!, id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as ManagerKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_SecurityKey_OwnerKey()
    {
        var id = Guid.NewGuid();
        using var cert = CreateCertificate(Guid.NewGuid(), "Own");
        var k = new OwnerKey(cert!, id);
        var cab = MakeCab(k);
        var parsed = KusDepotCab.Parse(cab.ToString());
        var back = parsed!.GetSecurityKey() as OwnerKey;
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(parsed.GetObject<SecurityKey>()).IsEqualTo(k);
    }

    [Test]
    public void Roundtrip_KeySet_Unlocked_AllTypes()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        using var co = CreateCertificate(Guid.NewGuid(), "Own");
        var keys = new SecurityKey[]
        {
            new ManagerKey(cm!, Guid.NewGuid()),
            new OwnerKey(co!, Guid.NewGuid()),
            new HostKey(Bytes(10), Guid.NewGuid()),
            new MyHostKey(Bytes(11), Guid.NewGuid()),
            new ClientKey(Bytes(12), Guid.NewGuid()),
            new ServiceKey(Bytes(13), Guid.NewGuid()),
            new CommandKey(Bytes(14), Guid.NewGuid()),
            new ExecutiveKey(Bytes(15), Guid.NewGuid())
        };
        var ks = new KeySet(keys);

        var cab = ks.ToKusDepotCab();
        var parsed = KusDepotCab.Parse(cab!.ToString());
        Check.That(parsed).IsNotNull();
        var back = parsed!.GetKeySet();
        Check.That(back).IsNotNull();
        var all = back!.GetAllKeys(null);
        Check.That(all).IsNotNull();
        Check.That(all!.Count).IsEqualTo(keys.Length);
        foreach (var k in keys)
        {
            Check.That(all.Values.Any(x => x.Equals(k) && x.GetType() == k.GetType())).IsTrue();
        }

        Check.That(parsed.GetObject<KeySet>()).IsNotNull();
    }

    [Test]
    public void Roundtrip_KeySet_Locked_Gating()
    {
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var ks = new KeySet(new SecurityKey[]
        {
            new ManagerKey(cm!, Guid.NewGuid()),
            new ServiceKey(Bytes(21), Guid.NewGuid())
        });
        var owner = ks.CreateOwnerKey("ks-owner");
        Check.That(ks.Lock(owner)).IsTrue();

        var cab = ks.ToKusDepotCab();
        var parsed = KusDepotCab.Parse(cab!.ToString());
        Check.That(parsed).IsNotNull();
        var back = parsed!.GetKeySet();
        Check.That(back).IsNotNull();
        Check.That(back!.GetAllKeys(null)).IsNull();
        var all = back.GetAllKeys(owner);
        Check.That(all).IsNotNull();
        Check.That(all!.Any(k => k.Value is ManagerKey)).IsTrue();
        Check.That(all!.Any(k => k.Value is ServiceKey)).IsTrue();

        Check.That(parsed.GetObject<KeySet>()).IsNotNull();
    }

    [Test]
    public async Task Roundtrip_Complex_Nested_Encrypted_Signed_Verified()
    {
        var originalKeys = KusDepot.Exams.DataItems.KeySetTestHelpers.MakeKeys(5);
        var ks = new KeySet();
        var ksOwner = ks.CreateOwnerKey("ks-owner");
        var ksSigner = ks.CreateManagementKey("ks-signer");

        Assert.That(ks.AddKeys(originalKeys, ksOwner, true), Is.True);
        Assert.That(await ks.EncryptData(ksOwner, true), Is.True);
        Assert.That(await ks.VerifyData("EncryptedData", ksOwner), Is.True);

        var innerGI = new GenericItem();
        innerGI.SetContent(new List<Object> { ks });
        var innerGIOwner = innerGI.CreateManagementKey("innerGI-owner");
        var innerGISigner = innerGI.CreateManagementKey("innerGI-signer");

        Assert.That(innerGI.SignData("Content", innerGISigner), Is.Not.Null);
        Assert.That(await innerGI.EncryptData(innerGIOwner, true), Is.True);
        Assert.That(await innerGI.VerifyData("EncryptedData", innerGIOwner), Is.True);

        var outerGI = new GenericItem();
        outerGI.SetContent(new List<Object> { innerGI });
        var outerGISigner = outerGI.CreateManagementKey("outerGI-signer");

        Assert.That(outerGI.SignData("Content", outerGISigner), Is.Not.Null);
        Assert.That(await outerGI.VerifyData("Content", outerGISigner), Is.True);

        var cab = Utility.ToKusDepotCab(outerGI);
        Assert.That(cab, Is.Not.Null);

        var cabString = cab!.ToString();
        var parsedCab = KusDepotCab.Parse(cabString);
        Assert.That(parsedCab, Is.Not.Null);

        var extractedOuterGI = parsedCab!.GetDataItem() as GenericItem;
        Assert.That(extractedOuterGI, Is.Not.Null);

        Assert.That(await extractedOuterGI!.VerifyData("Content", outerGISigner), Is.True);

        var extractedInnerGI = extractedOuterGI.GetContent()!.ToList().OfType<GenericItem>().Single();
        Assert.That(extractedInnerGI, Is.Not.Null);

        Assert.That(await extractedInnerGI.VerifyData("EncryptedData", innerGIOwner), Is.True);
        Assert.That(await extractedInnerGI.DecryptData(innerGIOwner), Is.True);
        Assert.That(await extractedInnerGI.VerifyData("Content", innerGISigner), Is.True);

        var extractedKS = extractedInnerGI.GetContent()!.ToList().OfType<KeySet>().Single();
        Assert.That(extractedKS, Is.Not.Null);

        Assert.That(await extractedKS.VerifyData("EncryptedData", ksOwner), Is.True);
        Assert.That(await extractedKS.DecryptData(ksOwner), Is.True);
        var finalKeys = extractedKS.GetAllKeys();
        Assert.That(finalKeys, Is.Not.Null);
        Assert.That(finalKeys!.Count, Is.EqualTo(originalKeys.Count));

        foreach (var originalKey in originalKeys)
        {
            Assert.That(finalKeys.Values.Any(k => k.Equals(originalKey) && k.GetType() == originalKey.GetType()), Is.True);
        }
    }
}