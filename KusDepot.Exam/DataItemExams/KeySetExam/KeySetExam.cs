namespace KusDepot.Exams.DataItems;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class KeySetExam
{
    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }
    }

    private static ServiceKey CreateServiceKey(Byte seed, Guid id)
    {
        var data = new Byte[32];
        for (Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return new ServiceKey(data, id);
    }

    private static Byte[] Bytes(Byte seed)
    {
        var data = new Byte[32];
        for (Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return data;
    }

    private static HostKey CreateHostKey(Byte seed, Guid id) => new(Bytes(seed), id);
    private static MyHostKey CreateMyHostKey(Byte seed, Guid id) => new(Bytes(seed), id);
    private static ClientKey CreateClientKey(Byte seed, Guid id) => new(Bytes(seed), id);
    private static CommandKey CreateCommandKey(Byte seed, Guid id) => new(Bytes(seed), id);
    private static ExecutiveKey CreateExecutiveKey(Byte seed, Guid id) => new(Bytes(seed), id);

    private static SecurityKey[] CreateUniqueKeys(Int32 count)
    {
        if (count > 8) throw new ArgumentOutOfRangeException(nameof(count), "Cannot create more than 8 unique key types.");
        
        using var cert1 = CreateCertificate(Guid.NewGuid(), "Manager");
        using var cert2 = CreateCertificate(Guid.NewGuid(), "Owner");

        var allKeys = new SecurityKey[]
        {
            new ManagerKey(cert1!, Guid.NewGuid()),
            new OwnerKey(cert2!, Guid.NewGuid()),
            CreateHostKey(1, Guid.NewGuid()),
            CreateMyHostKey(2, Guid.NewGuid()),
            CreateClientKey(3, Guid.NewGuid()),
            CreateServiceKey(4, Guid.NewGuid()),
            CreateCommandKey(5, Guid.NewGuid()),
            CreateExecutiveKey(6, Guid.NewGuid())
        };

        return allKeys.Take(count).ToArray();
    }

    [Test]
    public void Constructor_Initialize_Keys_Unlocked()
    {
        var keys = CreateUniqueKeys(8);
        var ks = new KeySet(keys);
        var all = (ks.GetAllKeys())!;
        Check.That(all.Count).IsEqualTo(keys.Length);
        foreach (var k in keys)
        {
            Check.That(all.Values.Any(a => a.Equals(k) && a.GetType() == k.GetType())).IsTrue();
        }
    }

    [Test]
    public void Constructor()
    {
        var keys = CreateUniqueKeys(2);
        using var cm = CreateCertificate(Guid.NewGuid(), "Mgr");
        var owner = new OwnerKey(cm!, Guid.NewGuid());
        var ks = new KeySet(keys);
        Check.That(ks.TakeOwnership(owner)).IsTrue();
        Check.That(ks.Lock(owner)).IsTrue();
        Check.That(ks.GetAllKeys(null)).IsNull();
        Check.That(ks.GetAllKeys(owner)).IsNotNull();
    }

    [Test]
    public void AddKeys_Null_And_Empty()
    {
        var ks = new KeySet();
        Check.That(ks.AddKeys(null, null)).IsFalse();
        var ok = ks.AddKeys(Array.Empty<SecurityKey>(), null);
        Check.That(ok).IsTrue();
        Check.That(ks.GetAllKeys(null)).IsNull();
    }

    [Test]
    public void AddKeys_Mixed_ReturnsFalseWhenAnyDuplicate()
    {
        var ks = new KeySet();
        var id = Guid.NewGuid();
        var k1 = CreateServiceKey(21, id);
        var k2 = CreateHostKey(22, Guid.NewGuid());
        var k3 = CreateServiceKey(21, id);
        var ok = ks.AddKeys(new SecurityKey[] { k1, k2, k3 }, null);
        Check.That(ok).IsFalse();
        var all = (ks.GetAllKeys(null))!;
        Check.That(all.Count).IsEqualTo(2);
        Check.That(all.Values.Any(a => a.Equals(k1))).IsTrue();
        Check.That(all.Values.Any(a => a.Equals(k2))).IsTrue();
    }

    [Test]
    public void AddKeys_Locked_Gating()
    {
        var ks = new KeySet();
        var owner = ks.CreateOwnerKey("mgr");
        Check.That(ks.Lock(owner)).IsTrue();
        var list = CreateUniqueKeys(2);

        Check.That(ks.AddKeys(list, null)).IsFalse();
        Check.That(ks.GetAllKeys(owner)).IsNull();

        using var wrong = CreateCertificate(Guid.NewGuid(), "wrong");
        var wrongMgr = new ManagerKey(wrong!);
        Check.That(ks.AddKeys(list, wrongMgr)).IsFalse();
        Check.That(ks.GetAllKeys(owner)).IsNull();

        Check.That(ks.AddKeys(list, owner)).IsTrue();
        var all = (ks.GetAllKeys(owner))!;
        Check.That(all.Count).IsEqualTo(2);
    }

    [Test]
    public void RemoveKeys_Null_And_Empty()
    {
        var ks = new KeySet();
        var keys = CreateUniqueKeys(2);
        Check.That(ks.AddKeys(keys, null)).IsTrue();
        var okNull = ks.RemoveKeys(null, null);
        Check.That(okNull).IsFalse();
        var okEmpty = ks.RemoveKeys(Array.Empty<SecurityKey>(), null);
        Check.That(okEmpty).IsTrue();
        var all = (ks.GetAllKeys(null))!;
        Check.That(all.Count).IsEqualTo(2);
    }

    [Test]
    public void RemoveKeys_PartialAndReturnValue()
    {
        var ks = new KeySet();
        var keys = CreateUniqueKeys(2);
        var a = keys[0];
        var b = keys[1];
        Check.That(ks.AddKeys(new SecurityKey[]{a,b}, null)).IsTrue();
        var nonExisting = CreateServiceKey(53, Guid.NewGuid());
        var ok = ks.RemoveKeys(new SecurityKey[]{a,nonExisting}, null);
        Check.That(ok).IsFalse();
        var left = (ks.GetAllKeys(null))!;
        Check.That(left.Count).IsEqualTo(1);
        Check.That(left.Values.Any(k => k.Equals(b))).IsTrue();
    }

    [Test]
    public void RemoveKeys_All_Removes_ToEmpty()
    {
        var ks = new KeySet();
        var keys = CreateUniqueKeys(2);
        Check.That(ks.AddKeys(keys, null)).IsTrue();
        var ok = ks.RemoveKeys(keys, null);
        Check.That(ok).IsTrue();
        Check.That(ks.GetAllKeys(null)).IsNull();
    }

    [Test]
    public void RemoveKeys_Locked_Gating()
    {
        var ks = new KeySet();
        var mgr = ks.CreateOwnerKey("mgr");
        var keys = CreateUniqueKeys(2);
        var a = keys[0];
        var b = keys[1];
        Check.That(ks.AddKeys(new SecurityKey[]{a,b}, null)).IsTrue();
        Check.That(ks.Lock(mgr)).IsTrue();

        using var wrong = CreateCertificate(Guid.NewGuid(), "wrong");
        var wrongMgr = new ManagerKey(wrong!);
        Check.That(ks.RemoveKeys(new SecurityKey[]{a,b}, null)).IsFalse();
        Check.That(ks.RemoveKey(a.ID!.Value.ToString(), wrongMgr)).IsFalse();
        Check.That(ks.RemoveKey(a.ID!.Value.ToString(), mgr)).IsTrue();
        var left = (ks.GetAllKeys(mgr))!;
        Check.That(left.Count).IsEqualTo(1);
        Check.That(left.Values.Any(k => k.Equals(b))).IsTrue();
    }

    [Test]
    public void AddKey_NullKey_ReturnsFalse()
    {
        var ks = new KeySet();
        Check.That(ks.AddKey(null, null)).IsFalse();
    }

    [Test]
    public void AddKey_Unlocked_Adds_And_DuplicateHandling()
    {
        var ks = new KeySet();
        var id1 = Guid.NewGuid();
        var sk1 = CreateServiceKey(1, id1);
        var sk1dup = CreateServiceKey(1, id1);

        Check.That(ks.AddKey(sk1, null)).IsTrue();
        Check.That(ks.AddKey(sk1dup, null)).IsFalse();

        var id2 = Guid.NewGuid();
        using var cert = CreateCertificate(Guid.NewGuid(), "Test");
        var mk1 = new ManagerKey(cert!, id2);
        var mk2 = new ManagerKey(cert!, id2);

        Check.That(ks.AddKey(mk1, null)).IsTrue();
        Check.That(ks.AddKey(mk2, null)).IsFalse();
    }

    [Test]
    public void AddKey_AllConcreteTypes_Unlocked()
    {
        var ks = new KeySet();
        var keys = CreateUniqueKeys(8);

        foreach (var k in keys) Check.That(ks.AddKey(k, null)).IsTrue();

        var all = ks.GetAllKeys(null);
        Check.That(all).IsNotNull();
        Check.That(all!.Count).IsEqualTo(keys.Length);
        foreach (var k in keys)
        {
            Check.That(all.Values.Any(a => a.Equals(k) && a.GetType() == k.GetType())).IsTrue();
        }
    }

    [Test]
    public void DuplicateHandling_EachConcreteType()
    {
        var ks = new KeySet();
        using var certMgr = CreateCertificate(Guid.NewGuid(), "Mgr");
        using var certOwn = CreateCertificate(Guid.NewGuid(), "Own");

        var id1 = Guid.NewGuid();
        Check.That(ks.AddKey(CreateHostKey(20, id1), null)).IsTrue();
        Check.That(ks.AddKey(CreateHostKey(20, id1), null)).IsFalse();
        var id2 = Guid.NewGuid();
        Check.That(ks.AddKey(CreateMyHostKey(21, id2), null)).IsTrue();
        Check.That(ks.AddKey(CreateMyHostKey(21, id2), null)).IsFalse();
        var id3 = Guid.NewGuid();
        Check.That(ks.AddKey(CreateClientKey(22, id3), null)).IsTrue();
        Check.That(ks.AddKey(CreateClientKey(22, id3), null)).IsFalse();
        var id4 = Guid.NewGuid();
        Check.That(ks.AddKey(CreateServiceKey(23, id4), null)).IsTrue();
        Check.That(ks.AddKey(CreateServiceKey(23, id4), null)).IsFalse();
        var id5 = Guid.NewGuid();
        Check.That(ks.AddKey(CreateCommandKey(24, id5), null)).IsTrue();
        Check.That(ks.AddKey(CreateCommandKey(24, id5), null)).IsFalse();
        var id6 = Guid.NewGuid();
        Check.That(ks.AddKey(CreateExecutiveKey(25, id6), null)).IsTrue();
        Check.That(ks.AddKey(CreateExecutiveKey(25, id6), null)).IsFalse();

        var id7 = Guid.NewGuid();
        var mk1 = new ManagerKey(certMgr!, id7);
        var mk2 = new ManagerKey(certMgr!, id7);
        var id8 = Guid.NewGuid();
        var ok1 = new OwnerKey(certOwn!, id8);
        var ok2 = new OwnerKey(certOwn!, id8);
        Check.That(ks.AddKey(mk1, null)).IsTrue();
        Check.That(ks.AddKey(mk2, null)).IsFalse();
        Check.That(ks.AddKey(ok1, null)).IsTrue();
        Check.That(ks.AddKey(ok2, null)).IsFalse();

        var pfx = SerializeCertificate(certOwn);
        var okBytes = new OwnerKey(pfx!, id8, null);
        Check.That(ks.AddKey(okBytes, null)).IsFalse();
    }

    [Test]
    public void Remove_EqualInstance_EachConcreteType()
    {
        var ks = new KeySet();
        var items = CreateUniqueKeys(7);

        foreach (var it in items) Check.That(ks.AddKey(it, null)).IsTrue();

        foreach (var eq in items) Check.That(ks.RemoveKey(eq.ID!.Value.ToString(), null)).IsTrue();
        Check.That(ks.GetAllKeys(null)).IsNull();
    }

    [Test]
    public void Locked_Gating_With_Manager_And_Owner()
    {
        var ks = new KeySet();
        var manager = ks.CreateManagementKey("mgr");
        var owner = ks.CreateOwnerKey("owner");

        Check.That(ks.RegisterManager(manager)).IsTrue();
        Check.That(ks.Lock(owner)).IsTrue();

        var k = CreateClientKey(41, Guid.NewGuid());
        Check.That(ks.AddKey(k, null)).IsFalse();
        Check.That(ks.AddKey(k, null, manager)).IsTrue();
        var s2 = CreateServiceKey(42, Guid.NewGuid());
        Check.That(ks.AddKey(s2, null, owner)).IsTrue();

        Check.That(ks.GetAllKeys(null)).IsNull();
        Check.That(ks.GetAllKeys(manager)).IsNull();
        Check.That(ks.GetAllKeys(owner)).IsNotNull();
    }

    [Test]
    public void AddKey_CrossType_SameID_Rejected()
    {
        var ks = new KeySet();
        var id = Guid.NewGuid();
        var sk = CreateServiceKey(7, id);
        using var cert = CreateCertificate(Guid.NewGuid(), "Test");
        var mk = new ManagerKey(cert!, id);

        Check.That(ks.AddKey(sk, null)).IsTrue();
        Check.That(ks.AddKey(mk, null)).IsFalse();

        var all = ks.GetAllKeys(null);
        Check.That(all).IsNotNull();
        Check.That(all!.Count).IsEqualTo(1);
    }

    [Test]
    public void GetAllKeys_Unlocked_NoKeys_ReturnsNull()
    {
        var ks = new KeySet();
        Check.That(ks.GetAllKeys(null)).IsNull();
    }

    [Test]
    public void GetAllKeys_Unlocked_WithKeys_ReturnsClones()
    {
        var ks = new KeySet();
        var keys = CreateUniqueKeys(2);
        var sk = keys[0];
        var mk = keys[1];
        Check.That(ks.AddKey(sk, null)).IsTrue();
        Check.That(ks.AddKey(mk, null)).IsTrue();

        var originals = new List<SecurityKey> { sk, mk };
        var clones = ks.GetAllKeys(null);
        Check.That(clones).IsNotNull();
        Check.That(clones!.Count).IsEqualTo(originals.Count);

        foreach (var o in originals)
        {
            var found = clones.Values.SingleOrDefault(c => c.Equals(o));
            Check.That(found).IsNotNull();
            Check.That(ReferenceEquals(found, o)).IsFalse();
            Check.That(found!.GetType()).IsEqualTo(o.GetType());
        }
    }

    [Test]
    public void GetAllKeys_Locked_Gating()
    {
        var ks = new KeySet();
        var owner = ks.CreateOwnerKey("owner");
        Check.That(owner).IsNotNull();
        Check.That(ks.Lock(owner)).IsTrue();

        var sk = CreateServiceKey(9, Guid.NewGuid());
        Check.That(ks.AddKey(sk, null, owner)).IsTrue();

        Check.That(ks.GetAllKeys(null)).IsNull();

        using var wrongCert = CreateCertificate(Guid.NewGuid(), "Wrong");
        var wrongMgr = new ManagerKey(wrongCert!);
        Check.That(ks.GetAllKeys(wrongMgr)).IsNull();

        var result = ks.GetAllKeys(owner);
        Check.That(result).IsNotNull();
        Check.That(result!.Values.Any(k => k.Equals(sk))).IsTrue();
    }

    [Test]
    public void GetAllKeys_ReturnedSet_Independent()
    {
        var ks = new KeySet();
        var keys = CreateUniqueKeys(2);
        var a = keys[0];
        var b = keys[1];
        Check.That(ks.AddKey(a, null)).IsTrue();
        Check.That(ks.AddKey(b, null)).IsTrue();

        var r1 = (ks.GetAllKeys(null))!;
        Check.That(r1.Count).IsEqualTo(2);
        var toRemove = r1.First();
        r1.Remove(toRemove.Key);
        var r2 = (ks.GetAllKeys(null))!;
        Check.That(r2.Count).IsEqualTo(2);
    }

    [Test]
    public void RemoveKey_Unlocked_Scenarios()
    {
        var ks = new KeySet();
        var id = Guid.NewGuid();
        var sk = CreateServiceKey(5, id);
        Check.That(ks.AddKey(sk, null)).IsTrue();

        Check.That(ks.RemoveKey(sk.ID!.Value.ToString(), null)).IsTrue();
        Check.That(ks.GetAllKeys(null)).IsNull();

        var sk2 = CreateServiceKey(5, Guid.NewGuid());
        Check.That(ks.AddKey(sk2, null)).IsTrue();
        var equal = CreateServiceKey(5, sk2.ID.Value);
        Check.That(ks.RemoveKey(equal.ID!.Value.ToString(), null)).IsTrue();

        var sk3 = CreateServiceKey(6, Guid.NewGuid());
        Check.That(ks.RemoveKey(sk3.ID!.Value.ToString(), null)).IsFalse();
    }

    [Test]
    public void RemoveKey_Locked_Gating_And_LastItem_Nulls_AllKeys()
    {
        var ks = new KeySet();
        var mgr = ks.CreateOwnerKey("mgr");
        Check.That(mgr).IsNotNull();
        Check.That(ks.Lock(mgr)).IsTrue();

        var keys = CreateUniqueKeys(2);
        var a = keys[0];
        var b = keys[1];
        Check.That(ks.AddKey(a, null, mgr)).IsTrue();
        Check.That(ks.AddKey(b, null, mgr)).IsTrue();

        using var wrongCert = CreateCertificate(Guid.NewGuid(), "Wrong");
        var wrongMgr = new ManagerKey(wrongCert!);
        Check.That(ks.RemoveKey(a.ID!.Value.ToString(), null)).IsFalse();
        Check.That(ks.RemoveKey(a.ID!.Value.ToString(), wrongMgr)).IsFalse();

        Check.That(ks.RemoveKey(a.ID!.Value.ToString(), mgr)).IsTrue();
        var left = ks.GetAllKeys(mgr);
        Check.That(left).IsNotNull();
        Check.That(left!.Count).IsEqualTo(1);

        Check.That(ks.RemoveKey(b.ID!.Value.ToString(), mgr)).IsTrue();
        Check.That(ks.GetAllKeys(mgr)).IsNull();
    }

    [Test]
    public void ToString_Parse_Roundtrip_Unlocked_And_Locked_AllTypes()
    {
        var ks1 = new KeySet();
        var keys = CreateUniqueKeys(8);
        foreach (var k in keys) Check.That(ks1.AddKey(k, null)).IsTrue();
        var str1 = ks1.ToString();
        var parsed1 = KeySet.Parse(str1);
        Check.That(parsed1).IsNotNull();
        var p1 = (parsed1!.GetAllKeys(null))!;
        Check.That(p1.Count).IsEqualTo(keys.Length);
        foreach (var k in keys) Check.That(p1.Values.Any(x => x.Equals(k) && x.GetType() == k.GetType())).IsTrue();

        var ks2 = new KeySet();
        var owner = ks2.CreateOwnerKey("owner");
        Check.That(ks2.Lock(owner)).IsTrue();
        var s2a = CreateServiceKey(70, Guid.NewGuid());
        var h2a = CreateHostKey(71, Guid.NewGuid());
        Check.That(ks2.AddKey(s2a, null, owner)).IsTrue();
        Check.That(ks2.AddKey(h2a, null, owner)).IsTrue();
        var str2 = ks2.ToString();
        var parsed2 = KeySet.Parse(str2);
        Check.That(parsed2).IsNotNull();
        Check.That(parsed2!.GetAllKeys(null)).IsNull();
        var p2 = (parsed2.GetAllKeys(owner))!;
        Check.That(p2.Values.Any(k => k.Equals(s2a))).IsTrue();
        Check.That(p2.Values.Any(k => k.Equals(h2a))).IsTrue();
    }

    [Test]
    public void TryParse_Roundtrip()
    {
        var ks = new KeySet();
        var s = CreateServiceKey(50, Guid.NewGuid());
        Check.That(ks.AddKey(s, null)).IsTrue();
        var text = ks.ToString();
        var ok = KeySet.TryParse(text, null, out var parsed);
        Check.That(ok).IsTrue();
        Check.That(parsed).IsNotNull();
        var keys = parsed!.GetAllKeys(null);
        Check.That(keys).IsNotNull();
        Check.That(keys!.Values.Any(k => k.Equals(s))).IsTrue();
    }

    [Test]
    public async Task GenericItem_Stores_Encrypted_Roundtrip_And_Unlocks_Embedded_KeySet()
    {
        var keys = CreateUniqueKeys(8);
        var ks = new KeySet(keys);
        var owner = ks.CreateOwnerKey("ks-owner");
        Check.That(owner).IsNotNull();
        Check.That(ks.Lock(owner)).IsTrue();
        Check.That(ks.GetAllKeys(null)).IsNull();

        var gi = new GenericItem();
        var content = new List<Object> { ks };
        Check.That(gi.SetContent(content)).IsTrue();
        Check.That(await gi.EncryptData(owner,true)).IsTrue();
        var serialized = gi.ToString();
        Check.That(serialized).IsNotNull().And.Not.Equals(String.Empty);

        GenericItem? round;
        Check.That(GenericItem.TryParse(serialized, null, out round)).IsTrue();
        Check.That(round).IsNotNull();
        var gi2 = round!;
        Check.That(await gi2.DecryptData(owner)).IsTrue();

        var restored = gi2.GetContent() as IEnumerable<Object>;
        Check.That(restored).IsNotNull();
        var embedded = restored!.OfType<KeySet>().SingleOrDefault();
        Check.That(embedded).IsNotNull();

        Check.That(embedded!.GetAllKeys(null)).IsNull();
        var got = embedded.GetAllKeys(owner);
        Check.That(got).IsNotNull();
        Check.That(got!.Count).IsEqualTo(keys.Length);
        foreach (var k in keys)
        {
            Check.That(got.Values.Any(x => x.Equals(k) && x.GetType() == k.GetType())).IsTrue();
        }
    }

    [Test]
    public void AddNotesAndTags_And_Remove()
    {
        var ks = new KeySet();
        var notes1 = new HashSet<String> { "AddNotesExam" };
        var notes2 = new HashSet<String> { "Pass",String.Empty };
        var tags1 = new HashSet<String> { "AddTagsExam" };
        var tags2 = new HashSet<String> { "Pass",String.Empty };

        Check.That(ks.AddNotes(null)).IsFalse();
        Check.That(ks.GetNotes()).IsNull();
        Check.That(ks.AddNotes(notes1)).IsTrue();
        Check.That(ks.GetNotes()).Contains(notes1);
        Check.That(ks.AddNotes(notes2)).IsTrue();
        Check.That(ks.GetNotes()).Contains(notes1.Union(notes2));

        Check.That(ks.AddTags(null)).IsFalse();
        Check.That(ks.GetTags()).IsNull();
        Check.That(ks.AddTags(tags1)).IsTrue();
        Check.That(ks.GetTags()).Contains(tags1);
        Check.That(ks.AddTags(tags2)).IsTrue();
        Check.That(ks.GetTags()).Contains(tags1.Union(tags2));

        Check.That(ks.RemoveNote(null)).IsFalse();
        Check.That(ks.RemoveTag(null)).IsFalse();
        foreach (var n in notes1.Union(notes2)) Check.That(ks.RemoveNote(n)).IsTrue();
        foreach (var t in tags1.Union(tags2)) Check.That(ks.RemoveTag(t)).IsTrue();
        Check.That(ks.GetNotes()).IsNull();
        Check.That(ks.GetTags()).IsNull();
    }

    [Test]
    public void Equality_And_ID()
    {
        var a = new KeySet();
        var b = new KeySet();

        Check.That(b.SetID(Guid.Empty)).IsTrue();
        Check.That(b.GetID()).IsNull();
        Check.That(new KeySet() == null).IsFalse();
        Check.That(new KeySet() != null).IsTrue();
        Check.That(b.SetID(a.GetID())).IsTrue();
        Check.That(a == b).IsTrue();
        Check.That(a.Equals((Object)b)).IsTrue();
        Check.That(a != b).IsFalse();

        Check.That(((Int32?)a.GetHashCode())).HasAValue();
    }

    [Test]
    public void BasicProperties_And_LockGating()
    {
        var ks = new KeySet();
        var app = "KeySetApp";
        var ver = new Version("1.2.3.4");
        var dn = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";
        var name = "KS-Name";
        var domainId = "Domain-1";
        var born = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);
        var svcVer = new Version("0.1.0.1");

        Check.That(ks.SetApplication(app)).IsTrue();
        Check.That(ks.SetApplicationVersion(ver)).IsTrue();
        Check.That(ks.SetDistinguishedName(dn)).IsTrue();
        Check.That(ks.SetName(name)).IsTrue();
        Check.That(ks.SetDomainID(domainId)).IsTrue();
        Check.That(ks.SetBornOn(born)).IsTrue();
        Check.That(ks.SetServiceVersion(svcVer)).IsTrue();
        Check.That(ks.SetVersion(ver)).IsTrue();

        Check.That(ks.GetApplication()).IsEqualTo(app);
        Check.That(ks.GetApplicationVersion()).IsNotNull();
        Check.That(ks.GetDistinguishedName()).IsEqualTo(dn);
        Check.That(ks.GetName()).IsEqualTo(name);
        Check.That(ks.GetDomainID()).IsEqualTo(domainId);
        Check.That(ks.GetBornOn()).Equals(born);
        Check.That(ks.GetServiceVersion()!.ToString()).IsEqualTo(svcVer.ToString());
        Check.That(ks.GetVersion()!.ToString()).IsEqualTo(ver.ToString());

        var mgr = ks.CreateManagementKey("mgr");
        Check.That(ks.Lock(mgr)).IsTrue();
        Check.That(ks.SetDomainID(domainId)).IsFalse();
        var modified = DateTimeOffset.Now;
        Check.That(ks.SetModified(modified)).IsFalse();
    }

    [Test]
    public void Certificates_And_Links_Gating()
    {
        var ks = new KeySet();
        var certs = new Dictionary<String,String> { {"0","6131f9c300030000000f"}, {"1","611e23c200030000000e"} };

        var links = new Dictionary<String, GuidReferenceItem>();
        var g1 = new GuidReferenceItem(Guid.NewGuid());
        var g2 = new GuidReferenceItem(Guid.NewGuid());
        links.Add("g1", g1); links.Add("g2", g2);
        Check.That(ks.SetLinks(links)).IsTrue();

        var mgr = ks.CreateManagementKey("mgr");
        Check.That(ks.Lock(mgr)).IsTrue();

        Check.That(ks.GetLinks()).HasSize(2);
        Check.That(ks.GetLinks()!["g1"]).IsEqualTo(g1);
        Check.That(ks.GetLinks()!["g2"]).IsEqualTo(g2);

        Check.That(ks.SetLinks(new Dictionary<String, GuidReferenceItem>())).IsFalse();

        Check.That(ks.UnLock(mgr)).IsTrue();
        Check.That(ks.SetLinks(new Dictionary<String, GuidReferenceItem>())).IsTrue();
        Check.That(ks.GetLinks()).IsNull();
    }

    [Test]
    public void Extension_FILE_GPS_Locator_SecurityDescriptor()
    {
        var ks = new KeySet();

        var ext = new Dictionary<String, Object?>();
        String s = "Pass"; String ok = "OK"; Func<String> fn = () => ok;
        ext["1"] = s; ext["2"] = fn;
        Check.That(ks.SetExtension(ext)).IsTrue();
        Check.That((String?)ks.GetExtension()!["1"]).IsEqualTo(s);
        Check.That(((Func<String>?)ks.GetExtension()!["2"])!.DynamicInvoke()).IsEqualTo(ok);

        var file = "file://not-a-real-path";
        Check.That(ks.SetFILE(file)).IsTrue();
        Check.That(ks.GetFILE()).IsEqualTo(file);
        Check.That(ks.SetFILE(String.Empty)).IsTrue();
        Check.That(ks.GetFILE()).IsNull();

        var sd = "D:(A;;FA;;;SY)";
        Check.That(ks.SetSecurityDescriptor(null)).IsFalse();
        Check.That(ks.SetSecurityDescriptor(sd)).IsTrue();
        Check.That(ks.GetSecurityDescriptor()).IsEqualTo(sd);
    }

    [Test]
    public void Descriptor_Composes_From_Fields()
    {
        var ks = new KeySet();
        var s = "MetaDesc";
        var v = new Version("1.2.3.4");
        var d = DateTimeOffset.Now;
        var notes = new HashSet<String>{"Get","Descriptor","Notes"};
        var tags  = new HashSet<String>{"Get","Descriptor","Tags"};

        Check.That(ks.SetApplication(s)).IsTrue();
        Check.That(ks.SetApplicationVersion(v)).IsTrue();
        Check.That(ks.SetDistinguishedName(s)).IsTrue();
        Check.That(ks.SetModified(d)).IsTrue();
        Check.That(ks.SetName(s)).IsTrue();
        Check.That(ks.AddNotes(notes)).IsTrue();
        Check.That(ks.SetServiceVersion(v)).IsTrue();
        Check.That(ks.AddTags(tags)).IsTrue();
        Check.That(ks.SetVersion(v)).IsTrue();

        var desc = ks.GetDescriptor();
        Check.That(desc).IsNotNull();
        Check.That(desc!.Application).IsEqualTo(s);
        Check.That(desc.ApplicationVersion).IsEqualTo(v.ToString());
        Check.That(desc.DistinguishedName).IsEqualTo(s);
        Check.That(desc.ID).Equals(ks.GetID());
        Check.That(desc.Name).IsEqualTo(s);
        Check.That(desc.Notes).Contains(notes);
        Check.That(desc.ServiceVersion).IsEqualTo(v.ToString());
        Check.That(desc.Tags).Contains(tags);
        Check.That(desc.Version).IsEqualTo(v.ToString());

        Check.That(ks.SetID(Guid.Empty)).IsTrue();
        Check.That(ks.GetID()).IsNull();
        Check.That(ks.GetDescriptor()).IsNull();
    }

    [Test]
    public void Lock_State_And_UnLock_With_Wrong_Key()
    {
        var ks = new KeySet();
        Check.That(ks.GetLocked()).IsFalse();
        var mgr = ks.CreateManagementKey("mgr");
        Check.That(ks.Lock(mgr)).IsTrue();
        Check.That(ks.GetLocked()).IsTrue();

        using var wrong = CreateCertificate(Guid.NewGuid(), "debug");
        var wrongMgr = new ManagerKey(wrong!);
        Check.That(ks.UnLock(wrongMgr)).IsFalse();
        Check.That(ks.GetLocked()).IsTrue();
        Check.That(ks.UnLock(mgr)).IsTrue();
        Check.That(ks.GetLocked()).IsFalse();
    }

    [Test]
    public void Clone_Roundtrip()
    {
        var items = CreateUniqueKeys(7);

        var ks = new KeySet(items);
        var clone = ks.Clone();
        Check.That(clone).IsNotNull();
        Check.That(clone).IsInstanceOf<KeySet>();
        Check.That(clone!.GetID()).IsEqualTo(ks.GetID());
        Check.That(clone.GetType()).IsEqualTo(ks.GetType());
    }

    [Test]
    public void ToString_Parse_Roundtrip()
    {
        var items = CreateUniqueKeys(7);

        var ks = new KeySet(items);
        var str = ks.ToString();
        var parsed = KeySet.Parse(str);
        Check.That(parsed).IsNotNull();
        Check.That(parsed!.GetID()).IsEqualTo(ks.GetID());
    }

    [Test]
    public void ToFile_FromFile_Roundtrip()
    {
        var items = CreateUniqueKeys(7);

        var ks = new KeySet(items);
        var path = Path.GetTempFileName()+".kdi";
        try
        {
            if(File.Exists(path)) File.Delete(path);
            Check.That(ks.ToFile(path)).IsTrue();
            var loaded = KeySet.FromFile(path);
            Check.That(loaded).IsNotNull();
            Check.That(loaded!.GetID()).IsEqualTo(ks.GetID());
        }
        finally
        {
            if(File.Exists(path)) File.Delete(path);
        }
    }

    [Test]
    public void GetKey_ReturnsClone()
    {
        var ks = new KeySet();
        var key = CreateServiceKey(1, Guid.NewGuid());
        Check.That(ks.AddKey(key, null)).IsTrue();

        var retrieved = ks.GetKey(key.ID!.Value.ToString());
        Check.That(retrieved).IsNotNull();
        Check.That(retrieved).IsEqualTo(key);
        Check.That(ReferenceEquals(retrieved, key)).IsFalse();
    }

    [Test]
    public void GetKey_NotFound_ReturnsNull()
    {
        var ks = new KeySet();
        Check.That(ks.GetKey("non-existent-key")).IsNull();
    }

    [Test]
    public void GetKey_Locked_Gating()
    {
        var ks = new KeySet();
        var key = CreateServiceKey(1, Guid.NewGuid());
        var owner = ks.CreateOwnerKey("owner");
        Check.That(ks.Lock(owner)).IsTrue();
        Check.That(ks.AddKey(key, null, owner)).IsTrue();

        Check.That(ks.GetKey(key.ID!.Value.ToString(), null)).IsNull();
        
        using var wrongCert = CreateCertificate(Guid.NewGuid(), "Wrong");
        var wrongMgr = new ManagerKey(wrongCert!);
        Check.That(ks.GetKey(key.ID!.Value.ToString(), wrongMgr)).IsNull();

        var retrieved = ks.GetKey(key.ID!.Value.ToString(), owner);
        Check.That(retrieved).IsNotNull();
        Check.That(retrieved).IsEqualTo(key);
    }

    [Test]
    public async Task AddKey_And_GetKey_ByName_When_Locked_And_Encrypted()
    {
        var ks = new KeySet();
        var owner = ks.CreateOwnerKey("mgr");
        Check.That(owner).IsNotNull();
        Check.That(ks.Lock(owner)).IsTrue();
        Check.That(await ks.EncryptData(owner,true)).IsTrue();

        var keys = CreateUniqueKeys(8);

        foreach (var key in keys)
        {
            var friendlyName = $"friendly-{key.GetType().Name}";
            Check.That(ks.AddKey(key, friendlyName, owner)).IsTrue();
        }

        foreach (var key in keys)
        {
            var friendlyName = $"friendly-{key.GetType().Name}";
            var retrieved = ks.GetKey(friendlyName, owner);
            Check.That(retrieved).IsNotNull();
            Check.That(retrieved).IsEqualTo(key);
            Check.That(retrieved!.GetType()).IsEqualTo(key.GetType());
        }
    }
}