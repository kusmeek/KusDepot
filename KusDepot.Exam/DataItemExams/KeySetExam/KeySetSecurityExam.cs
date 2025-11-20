namespace KusDepot.Exams.DataItems;

// =====================
//  KeySet-specific Helpers
// =====================
public static class KeySetTestHelpers
{
    public static void TamperAllKeys(KeySet item, IEnumerable<SecurityKey> newValue)
    {
        var field = typeof(KeySet).GetField("AllKeys", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("AllKeys field not found");
        var dictionary = newValue.ToDictionary(k => k.ID!.Value.ToString(), k => k, StringComparer.OrdinalIgnoreCase);
        field.SetValue(item, dictionary);
    }

    public static List<SecurityKey> MakeKeys(Int32 count = 2)
    {
        var list = new List<SecurityKey>(count);
        var random = new Random();

        Func<Byte[]> randomBytes = () => {
            var buffer = new Byte[32];
            random.NextBytes(buffer);
            return buffer;
        };

        var keyFactories = new List<Func<SecurityKey>>
        {
            () => { var cert = CreateCertificate(Guid.NewGuid(), "Manager"); return new ManagerKey(SerializeCertificate(cert)!, Guid.NewGuid(), cert!.Thumbprint); },
            () => { var cert = CreateCertificate(Guid.NewGuid(), "Owner"); return new OwnerKey(SerializeCertificate(cert)!, Guid.NewGuid(), cert!.Thumbprint); },
            () => new HostKey(randomBytes(), Guid.NewGuid()),
            () => new MyHostKey(randomBytes(), Guid.NewGuid()),
            () => new ClientKey(randomBytes(), Guid.NewGuid()),
            () => new ServiceKey(randomBytes(), Guid.NewGuid()),
            () => new CommandKey(randomBytes(), Guid.NewGuid()),
            () => new ExecutiveKey(randomBytes(), Guid.NewGuid())
        };

        for (var i = 0; i < count; i++)
        {
            list.Add(keyFactories[random.Next(keyFactories.Count)]());
        }
        return list;
    }
}

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class KeySetSecurityExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryKeys_Valid()
    {
        var item = new KeySet();
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, null, sign: true), Is.True);
        Assert.That(await item.CheckDataHash(), Is.True);
    }

    [Test]
    public async Task CheckDataHash_InMemoryKeys_Tampered()
    {
        var item = new KeySet();
        var keys = KeySetTestHelpers.MakeKeys(12);
        foreach (var k in keys) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        var tampered = KeySetTestHelpers.MakeKeys();
        KeySetTestHelpers.TamperAllKeys(item, tampered);
        Assert.That(await item.CheckDataHash(), Is.False);
    }

    // =====================
    //  Signature-Based Data Integrity
    // =====================
    [Test]
    public async Task VerifyData_Content_ValidSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        Assert.That(await item.VerifyData("Content", mgr), Is.True);
    }

    [Test]
    public async Task VerifyData_Content_InvalidSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        foreach (var k in keys) Assert.That(item.AddKey(k, null, mgr, sign: true), Is.True);
        var tampered = KeySetTestHelpers.MakeKeys();
        KeySetTestHelpers.TamperAllKeys(item, tampered);
        Assert.That(await item.VerifyData("Content", mgr), Is.False);
    }

    [Test]
    public async Task VerifyData_Content_NoKeyProvided()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        Assert.That(await item.VerifyData("Content", null), Is.False);
    }

    // =====================
    //  Encrypted Data Signature
    // =====================
    [Test]
    public static async Task VerifyData_EncryptedData_ValidSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
    }

    [Test]
    public static async Task VerifyData_EncryptedData_Tampered()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        foreach (var k in keys) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.False);
    }

    // =====================
    //  EncryptData/DecryptData/Hash/Info
    // =====================
    [Test]
    public async Task EncryptData_SetsAndClears_DataEncryptInfo()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        var info = item.GetDataEncryptInfo();
        Check.That(info).IsNotNull();
        Check.That(info!.Serial).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.ThumbPrint).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.PublicKey).IsNotNull().And.Not.Equals(String.Empty);
        Assert.That(await item.DecryptData(mgr), Is.True);
        var infoAfter = item.GetDataEncryptInfo();
        Assert.That(infoAfter, Is.Null);
    }

    // =====================
    //  EncryptData/DecryptData Roundtrip
    // =====================
    [Test]
    public async Task EncryptData_InMemoryKeys_RoundTrip()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncryptInfo(), Is.Not.Null);
        var snapshot = item.GetAllKeys(mgr);
        Assert.That(snapshot, Is.Not.Null);
        Assert.That(snapshot!.Count, Is.EqualTo(keys.Count));
        foreach (var k in keys) Assert.That(snapshot.Values.Any(r => r.Equals(k)), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.True);

        Assert.That(await item.DecryptData(mgr), Is.True);
        var restored = item.GetAllKeys();
        Assert.That(restored, Is.Not.Null);
        Assert.That(restored!.Count, Is.EqualTo(keys.Count));
        foreach (var k in keys) Assert.That(restored.Values.Any(r => r.Equals(k)), Is.True);
        Assert.That(item.GetDataEncryptInfo(), Is.Null);
    }

    [Test]
    public async Task EncryptData_InMemoryKeys_Empty()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(item.GetDataEncryptInfo(), Is.Not.Null);
        Assert.That(await item.DecryptData(mgr), Is.True);
    }

    [Test]
    public async Task EncryptData_InMemoryKeys_Locked()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        foreach (var k in KeySetTestHelpers.MakeKeys(12)) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        Assert.That(item.Lock(mgr), Is.True);
        Assert.That(await item.EncryptData(mgr, false), Is.True);
        Assert.That(item.GetDataEncryptInfo(), Is.Not.Null);
    }

    [Test]
    public async Task EncryptData_InMemoryKeys_NotEncrypted()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        Assert.That(await item.DecryptData(mgr), Is.False);
        Assert.That(item.GetAllKeys(), Is.Not.Null);
    }

    // =====================
    //  Double Encrypt/Decrypt, Wrong Key, Tamper
    // =====================
    [Test]
    public async Task EncryptData_DoubleEncryption()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(), null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(await item.EncryptData(mgr, false), Is.False);
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(await item.DecryptData(mgr), Is.True);
        Assert.That(await item.DecryptData(mgr), Is.False);
        var restored = item.GetAllKeys();
        Assert.That(restored, Is.Not.Null);
    }

    [Test]
    public async Task EncryptData_WrongKey()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var wrong = item.CreateManagementKey("WrongKey");
        foreach (var k in KeySetTestHelpers.MakeKeys(12)) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(await item.DecryptData(wrong), Is.False);
        Assert.That(item.GetAllKeys(), Is.Null);
        var snapshot = item.GetAllKeys(mgr);
        Assert.That(snapshot, Is.Not.Null);
        Assert.That(snapshot!.Count, Is.EqualTo(12));
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Assert.That(await item.DecryptData(mgr), Is.False);
        Assert.That(item.GetAllKeys(), Is.Null);
    }

    // =====================
    //  ClearEncryptedData
    // =====================
    [Test]
    public async Task ClearEncryptedData()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);

        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(item.GetDataEncryptInfo(), Is.Not.Null);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);

        Assert.That(item.ClearEncryptedData(mgr), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(item.GetDataEncryptInfo(), Is.Null);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.False);
    }

    // =====================
    //  Signature/Verification
    // =====================
    [Test]
    public async Task SignData_And_VerifyData_ValidSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        var sigKey = item.SignData("Content", mgr);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content", mgr), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        foreach (var k in KeySetTestHelpers.MakeKeys(12)) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        var sigKey = item.SignData("Content", mgr);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        KeySetTestHelpers.TamperAllKeys(item, KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("Content", mgr), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_MultipleSignatures_UsesMostRecent()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        var sigKey1 = item.SignData("Content", mgr);
        Thread.Sleep(10);
        var sigKey2 = item.SignData("Content", mgr);
        Assert.That(sigKey1, Is.Not.EqualTo(sigKey2));
        Assert.That(await item.VerifyData("Content", mgr), Is.True);
        KeySetTestHelpers.TamperAllKeys(item, KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("Content", mgr), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_InvalidOrMissingHash()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.SignData("Content", mgr), Is.Null);
        Assert.That(await item.VerifyData("Content", mgr), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullKeyOrField()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(), null, sign: true), Is.True);
        Assert.That(item.SignData(null, mgr), Is.Null);
        Assert.That(item.SignData("Content", null), Is.Null);
        Assert.That(await item.VerifyData(null, mgr), Is.False);
        Assert.That(await item.VerifyData("Content", null), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_ValidSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        var sigKey = item.SignData("HashCode", mgr);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode", mgr), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        foreach (var k in KeySetTestHelpers.MakeKeys(12)) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        var sigKey = item.SignData("HashCode", mgr);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        KeySetTestHelpers.TamperAllKeys(item, KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("HashCode", mgr), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12), null, sign: true), Is.True);
        var sigKey = item.SignData("HashCode", mgr);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(item.SetID(Guid.NewGuid()), Is.True);
        Assert.That(await item.VerifyData("HashCode", mgr), Is.False);
    }

    // =====================
    //  Data Masking
    // =====================
    [Test]
    public void MaskData_InMemory_NotEncrypted()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, null, sign: true), Is.True);
        var current = item.GetAllKeys();
        Assert.That(current, Is.Not.Null);
        Assert.That(current!.Count, Is.EqualTo(keys.Count));
        Assert.That(item.MaskData(true, mgr), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.MaskData(false, null), Is.False);
        Assert.That(item.MaskData(false, mgr), Is.True);
        var unmasked = item.GetAllKeys();
        Assert.That(unmasked, Is.Not.Null);
        Assert.That(unmasked!.Count, Is.EqualTo(keys.Count));
    }

    [Test]
    public async Task MaskData_InMemory_Encrypted()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        foreach (var k in keys) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, false), Is.True);
        Assert.That(item.MaskData(true, mgr), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        var snapshot = item.GetAllKeys(mgr);
        Assert.That(snapshot, Is.Not.Null);
        Assert.That(item.MaskData(false, null), Is.False);
        Assert.That(item.MaskData(false, mgr), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(await item.DecryptData(mgr), Is.True);
        var restored = item.GetAllKeys();
        Assert.That(restored, Is.Not.Null);
        Assert.That(restored!.Count, Is.EqualTo(keys.Count));
    }

    [Test]
    public async Task GetAllKeys_Encrypted_ReturnsSnapshot_And_DoesNotMutate()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, false), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        var snapshot = item.GetAllKeys(mgr);
        Assert.That(snapshot, Is.Not.Null);
        Assert.That(snapshot!.Count, Is.EqualTo(keys.Count));
        foreach (var k in keys) Assert.That(snapshot.Values.Any(r => r.Equals(k)), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(item.GetDataEncryptInfo(), Is.Not.Null);
    }

    [Test]
    public async Task GetAllKeys_Encrypted_WithoutManager_ReturnsNull()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        foreach (var k in KeySetTestHelpers.MakeKeys()) Assert.That(item.AddKey(k, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, false), Is.True);
        Assert.That(item.GetAllKeys(null), Is.Null);
    }

    [Test]
    public async Task GetDataContent_AlwaysNull()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(), null, sign: true), Is.True);
        Assert.That(await item.GetDataContent(), Is.Null);
        Assert.That(await item.GetDataContent(mgr), Is.Null);
        Assert.That(await item.EncryptData(mgr, false), Is.True);
        Assert.That(await item.GetDataContent(), Is.Null);
        Assert.That(await item.GetDataContent(mgr), Is.Null);
    }

    // =====================
    //  Encrypted mutation behavior
    // =====================
    [Test]
    public async Task Encrypted_AddRemove_WithManager_MaintainsEncryption_AndIntegrity()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var initial = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(initial, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);

        var toAdd = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.AddKey(toAdd, null, mgr, sign: true), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
        var snap1 = item.GetAllKeys(mgr);
        Assert.That(snap1!.Values.Any(k => k.Equals(toAdd)), Is.True);

        Assert.That(item.RemoveKey(toAdd.ID!.Value.ToString(), mgr, sign: true), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
        var snap2 = item.GetAllKeys(mgr);
        Assert.That(snap2!.Values.Any(k => k.Equals(toAdd)), Is.False);
    }

    [Test]
    public async Task Encrypted_PartialDuplicateAndMissing()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var id = Guid.NewGuid();
        var a = new ManagerKey(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Test"))!, id);
        Assert.That(item.AddKey(a, null, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);

        var duplicateA = new ManagerKey(a.Key!, a.ID!.Value, a.Thumbprint);
        var newB = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.AddKeys(new[]{ duplicateA, newB }, mgr, sign: true), Is.False);
        var snapAdd = (item.GetAllKeys(mgr))!;
        Assert.That(snapAdd.Values.Any(k => k.Equals(a)), Is.True);
        Assert.That(snapAdd.Values.Any(k => k.Equals(newB)), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);

        var missing = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.RemoveKeys(new[]{ newB, missing }, mgr, sign: true), Is.False);
        var snapRem = (item.GetAllKeys(mgr))!;
        Assert.That(snapRem.Values.Any(k => k.Equals(newB)), Is.False);
        Assert.That(snapRem.Values.Any(k => k.Equals(a)), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
    }

    [Test]
    public async Task LockedAndEncrypted_AddRemove_WithManager_MaintainsStateAndIntegrity()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var initial = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(initial, null, sign: true), Is.True);

        Assert.That(item.TakeOwnership(mgr), Is.True);
        Assert.That(item.Lock(mgr), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);

        Assert.That(item.GetLocked(), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);

        var toAdd = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.AddKey(toAdd, null, mgr, sign: true), Is.True);

        Assert.That(item.GetLocked(), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
        var snap1 = item.GetAllKeys(mgr);
        Assert.That(snap1!.Values.Any(k => k.Equals(toAdd)), Is.True);

        Assert.That(item.RemoveKey(toAdd.ID!.Value.ToString(), mgr, sign: true), Is.True);

        Assert.That(item.GetLocked(), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
        var snap2 = item.GetAllKeys(mgr);
        Assert.That(snap2!.Values.Any(k => k.Equals(toAdd)), Is.False);
    }

    // =====================
    //  Authorization
    // =====================
    [Test]
    public void GetAllKeys_Locked_ManagerFails()
    {
        var item = new KeySet();
        var owner = item.CreateOwnerKey("Owner");
        var manager = item.CreateManagementKey("Manager");
        var keys = KeySetTestHelpers.MakeKeys(2);
        Assert.That(item.AddKeys(keys, manager, true), Is.True);
        Assert.That(item.Lock(owner), Is.True);
        Assert.That(item.GetAllKeys(manager), Is.Null);
        Assert.That(item.GetAllKeys(owner), Is.Not.Null);
    }

    // =====================
    //  Ownership
    // =====================
    [Test]
    public void CreateOwnerKey_CreatesAndTakesOwnership()
    {
        var item = new KeySet();
        var owner = item.CreateOwnerKey("TestOwner");
        Assert.That(owner, Is.Not.Null);
        Assert.That(item.CheckOwner(owner), Is.True);
    }

    [Test]
    public void TakeOwnership_And_CheckOwner_And_ReleaseOwnership_Cycle()
    {
        var item = new KeySet();
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.CheckOwner(key), Is.False);
        Assert.That(item.TakeOwnership(key), Is.True);
        Assert.That(item.CheckOwner(key), Is.True);
        var wrongKey = item.CreateManagementKey("WrongKey");
        Assert.That(item.CheckOwner(wrongKey), Is.False);
        Assert.That(item.TakeOwnership(wrongKey), Is.False);
        Assert.That(item.ReleaseOwnership(wrongKey), Is.False);
        Assert.That(item.ReleaseOwnership(key), Is.True);
        Assert.That(item.CheckOwner(key), Is.False);
    }

    [Test]
    public void TakeOwnership_FailsWhenLocked()
    {
        var item = new KeySet();
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.Lock(key), Is.True);
        Assert.That(item.TakeOwnership(key), Is.False);
    }

    [Test]
    public void ReleaseOwnership_FailsWithoutOwnership()
    {
        var item = new KeySet();
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.ReleaseOwnership(key), Is.False);
    }

    [Test]
    public void CreateOwnerKey_FailsWhenOwned()
    {
        var item = new KeySet();
        var owner1 = item.CreateOwnerKey("Owner1");
        Assert.That(owner1, Is.Not.Null);
        var owner2 = item.CreateOwnerKey("Owner2");
        Assert.That(owner2, Is.Null);
    }

    // =====================
    //  ClearKeys
    // =====================
    [Test]
    public async Task ClearKeys_Unencrypted_ClearsAllKeysAndInvalidatesSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        Assert.That(item.GetAllKeys(), Is.Not.Null);
        Assert.That((item.GetAllKeys())!.Count, Is.EqualTo(12));
        Assert.That(await item.VerifyData("Content", mgr), Is.True);
        Assert.That(item.ClearKeys(mgr), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(await item.VerifyData("Content", mgr), Is.False);
    }

    [Test]
    public async Task ClearKeys_Encrypted_ClearsEncryptedDataAndInvalidatesSignature()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.True);
        Assert.That(item.ClearKeys(mgr), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(item.GetDataEncryptInfo(), Is.Null);
        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(await item.VerifyData("EncryptedData", mgr), Is.False);
    }

    [Test]
    public void ClearKeys_Locked_Authorization()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var wrongMgr = item.CreateManagementKey("WrongKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        Assert.That(item.TakeOwnership(mgr), Is.True);
        Assert.That(item.Lock(mgr), Is.True);
        Assert.That((item.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        Assert.That(item.ClearKeys(null), Is.False);
        Assert.That((item.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        Assert.That(item.ClearKeys(wrongMgr), Is.False);
        Assert.That((item.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        Assert.That(item.ClearKeys(mgr), Is.True);
        Assert.That(item.GetAllKeys(mgr), Is.Null);
    }

    [Test]
    public async Task ClearKeys_Locked_Encrypted_Authorization()
    {
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var wrongMgr = item.CreateManagementKey("WrongKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        Assert.That(item.TakeOwnership(mgr), Is.True);
        Assert.That(item.Lock(mgr), Is.True);
        Assert.That(await item.EncryptData(mgr, true), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That((item.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        Assert.That(item.ClearKeys(null), Is.False);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That((item.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        Assert.That(item.ClearKeys(wrongMgr), Is.False);
        Assert.That(item.GetDataEncrypted(), Is.True);
        Assert.That((item.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        Assert.That(item.ClearKeys(mgr), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(item.GetAllKeys(mgr), Is.Null);
    }

    // =====================
    //  File Roundtrip
    // =====================
    [Test]
    public async Task ToFile_FromFile_Roundtrip_Unencrypted()
    {
        var keys = KeySetTestHelpers.MakeKeys(12);
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var key = item.CreateManagementKey("FileRoundtripKey");
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        var file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(item.SignData("Content", key), Is.Not.Null);
            Assert.That(await item.VerifyData("Content", key), Is.True);
            Assert.That(item.ToFile(file), Is.True);

            var loadedItem = KeySet.FromFile(file);
            Assert.That(loadedItem, Is.Not.Null);

            Assert.That(await loadedItem!.VerifyData("Content", key), Is.True);
            Assert.That((loadedItem.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
            Assert.That(loadedItem.GetDataEncrypted(), Is.False);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Test]
    public async Task ToFile_FromFile_Roundtrip_Encrypted()
    {
        var keys = KeySetTestHelpers.MakeKeys(12);
        var item = new KeySet();
        var mgr = item.CreateManagementKey("TestKey");
        var key = item.CreateManagementKey("FileRoundtripKey");
        Assert.That(item.AddKeys(keys, mgr, sign: true), Is.True);
        var file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(key, true), Is.True);
            Assert.That(await item.VerifyData("EncryptedData", key), Is.True);
            Assert.That(item.ToFile(file), Is.True);

            var loadedItem = KeySet.FromFile(file);
            Assert.That(loadedItem, Is.Not.Null);

            Assert.That(loadedItem!.GetDataEncrypted(), Is.True);
            Assert.That(loadedItem.GetAllKeys(), Is.Null);
            Assert.That(loadedItem.GetDataEncryptInfo(), Is.Not.Null);
            Assert.That(await loadedItem.VerifyData("EncryptedData", key), Is.True);

            Assert.That(await loadedItem.DecryptData(key), Is.True);
            Assert.That((loadedItem.GetAllKeys(mgr))!.Count, Is.EqualTo(12));
        }
        finally
        {
            File.Delete(file);
        }
    }

    // =====================
    //  ZeroData
    // =====================

    [Test]
    public async Task ZeroData_InMemory_NotEncrypted()
    {
        var item = new KeySet();
        var owner = item.CreateOwnerKey("TestOwner");
        Assert.That(owner, Is.Not.Null);
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, owner, true), Is.True);
        Assert.That(item.SignData("Content", owner), Is.Not.Null);

        Assert.That(await item.ZeroData(owner), Is.True);

        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.CheckOwner(owner), Is.True);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(item.GetDataEncryptInfo(), Is.Null);
        
        var secureHashesField = typeof(DataItem).GetField("SecureHashes", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureHashesField, Is.Not.Null);
        Assert.That(secureHashesField!.GetValue(item), Is.Null);

        var secureSignaturesField = typeof(DataItem).GetField("SecureSignatures", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureSignaturesField, Is.Not.Null);
        Assert.That(secureSignaturesField!.GetValue(item), Is.Null);
    }

    [Test]
    public async Task ZeroData_InMemory_Encrypted()
    {
        var item = new KeySet();
        var owner = item.CreateOwnerKey("TestOwner");
        Assert.That(owner, Is.Not.Null);
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, owner, true), Is.True);
        Assert.That(await item.EncryptData(owner, true), Is.True);

        Assert.That(await item.ZeroData(owner), Is.True);

        Assert.That(item.GetAllKeys(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(item.GetDataEncryptInfo(), Is.Null);

        var encryptedDataField = typeof(DataItem).GetField("EncryptedData", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(encryptedDataField, Is.Not.Null);
        Assert.That(encryptedDataField!.GetValue(item), Is.Null);
    }

    [Test]
    public void ZeroData_StreamedContent() {}

    [Test]
    public async Task ZeroData_Locked_InvalidKey()
    {
        var item = new KeySet();
        var owner = item.CreateOwnerKey("TestOwner");
        Assert.That(owner, Is.Not.Null);
        var invalidKey = item.CreateManagementKey("InvalidKey");
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, owner, true), Is.True);
        Assert.That(item.Lock(owner), Is.True);

        Assert.That(await item.ZeroData(invalidKey), Is.False);
        Assert.That(item.GetAllKeys(owner), Is.Not.Null);
        Assert.That((item.GetAllKeys(owner))!.Count, Is.EqualTo(12));
    }

    [Test]
    public async Task ZeroData_Locked_ValidKey()
    {
        var item = new KeySet();
        var owner = item.CreateOwnerKey("TestOwner");
        Assert.That(owner, Is.Not.Null);
        var keys = KeySetTestHelpers.MakeKeys(12);
        Assert.That(item.AddKeys(keys, owner, true), Is.True);
        Assert.That(item.Lock(owner), Is.True);

        Assert.That(await item.ZeroData(owner), Is.True);
        Assert.That(item.GetAllKeys(owner), Is.Null);
    }
}