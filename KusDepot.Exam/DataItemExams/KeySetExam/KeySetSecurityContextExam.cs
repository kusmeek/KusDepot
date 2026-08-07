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
        ClearHashCodeCache(item);
    }

    private static void ClearHashCodeCache(DataItem item)
    {
        var cachedHashCodeField = typeof(DataItem).GetField("CachedHashCode", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("CachedHashCode field not found");
        var hashCodeCachedField = typeof(DataItem).GetField("HashCodeCached", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("HashCodeCached field not found");

        cachedHashCodeField.SetValue(item, default(Int32));
        hashCodeCachedField.SetValue(item, false);
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
            () => { var cert = CreateCertificate(Guid.NewGuid(), "Manager"); return new ManagerKey(SerializeCertificate(cert)!, Guid.NewGuid()); },
            () => { var cert = CreateCertificate(Guid.NewGuid(), "Owner"); return new OwnerKey(SerializeCertificate(cert)!, Guid.NewGuid()); },
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
public class KeySetSecurityContextExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryKeys_Valid()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);

        Assert.That(item.AddKeys(keys,security:null,sign:true),Is.True);
        Assert.That(await item.CheckDataHash().ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task CheckDataHash_InMemoryKeys_Tampered()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);

        foreach(SecurityKey key in keys) { Assert.That(item.AddKey(key,null,security:null,sign:true),Is.True); }
        KeySetTestHelpers.TamperAllKeys(item,KeySetTestHelpers.MakeKeys());
        Assert.That(await item.CheckDataHash().ConfigureAwait(false),Is.False);
    }

    // =====================
    //  Signature-Based Data Integrity
    // =====================
    [Test]
    public async Task VerifyData_Content_ValidAssertion()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:false),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task VerifyData_Content_InvalidAssertion()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:false),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        KeySetTestHelpers.TamperAllKeys(item,KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task VerifyData_Content_NoContextProvided()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:false),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
    }

    // =====================
    //  Encrypted Data Signature
    // =====================
    [Test]
    public async Task VerifyData_EncryptedData_ValidAssertion()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:false),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task VerifyData_EncryptedData_Tampered()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:false),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.False);
    }

    // =====================
    //  ProtectData/UnProtectData Validation
    // =====================
    [Test]
    public async Task ValidateData_Content_UsesCurrentState()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:true),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
        Assert.That(await item.ValidateData(context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task ProtectData_UnencryptedContent_EncryptsAndValidates()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:true),Is.True);
        Assert.That(await item.ProtectData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(item.GetDataProtectionInfo(),Is.Not.Null);
        Assert.That(await item.ValidateData(context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task ProtectData_AlreadyEncrypted_TamperedState_FailsValidation()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Assert.That(await item.ProtectData(context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task UnProtectData_EncryptedContent_DecryptsAndValidates()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(keys,security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.UnProtectData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.False);
        IReadOnlyDictionary<String,SecurityKey>? restored = item.GetAllKeys();
        Assert.That(restored,Is.Not.Null);
        Assert.That(restored!.Count,Is.EqualTo(keys.Count));
        foreach(SecurityKey securityKey in keys) { Assert.That(restored.Values.Any(r => r.Equals(securityKey)),Is.True); }
        Assert.That(await item.ValidateData(context).ConfigureAwait(false),Is.True);
    }

    // =====================
    //  EncryptData/DecryptData/Hash/Info
    // =====================
    [Test]
    public async Task EncryptData_SetsAndClears_DataProtectionInfo()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);

        DataProtectionInfo? info = item.GetDataProtectionInfo();
        Assert.That(info,Is.Not.Null);
        Assert.That(info!.ProtectedByThumbprint,Is.Not.Null.And.Not.Empty);
        Assert.That(info.Recipients.IsDefaultOrEmpty,Is.False);
        Assert.That(info.ProtectionMode,Is.EqualTo(nameof(DataFieldState.EncryptedContent)));

        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);
    }

    // =====================
    //  EncryptData/DecryptData Roundtrip
    // =====================
    [Test]
    public async Task EncryptData_InMemoryKeys_RoundTrip()
    {
        KeySet item = new();
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Assert.That(item.AddKeys(keys,security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.GetDataProtectionInfo(),Is.Not.Null);

        IReadOnlyDictionary<String,SecurityKey>? snapshot = item.GetAllKeys(context);
        Assert.That(snapshot,Is.Not.Null);
        Assert.That(snapshot!.Count,Is.EqualTo(keys.Count));
        foreach(SecurityKey securityKey in keys) { Assert.That(snapshot.Values.Any(r => r.Equals(securityKey)),Is.True); }
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.True);

        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.True);
        IReadOnlyDictionary<String,SecurityKey>? restored = item.GetAllKeys();
        Assert.That(restored,Is.Not.Null);
        Assert.That(restored!.Count,Is.EqualTo(keys.Count));
        foreach(SecurityKey securityKey in keys) { Assert.That(restored.Values.Any(r => r.Equals(securityKey)),Is.True); }
        Assert.That(item.GetDataProtectionInfo(),Is.Null);
    }

    [Test]
    public async Task GetKeys_Encrypted_Filtered()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        ServiceKey a = new(RandomNumberGenerator.GetBytes(32),Guid.NewGuid());
        HostKey b = new(RandomNumberGenerator.GetBytes(32),Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKey(a,"Alpha",security:null,true),Is.True);
        Assert.That(item.AddKey(b,"Beta",security:null,true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);

        IReadOnlyDictionary<String,SecurityKey>? result = item.GetKeys(["Beta","Missing"],context);

        Assert.That(result,Is.Not.Null);
        Assert.That(result!.Count,Is.EqualTo(1));
        Assert.That(result.ContainsKey("Beta"),Is.True);
        Assert.That(result["Beta"].Equals(b),Is.True);
    }

    [Test]
    public async Task GetKeys_Encrypted_Null_GetsAll()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKey(new ServiceKey(RandomNumberGenerator.GetBytes(32),Guid.NewGuid()),"Alpha",security:null,true),Is.True);
        Assert.That(item.AddKey(new HostKey(RandomNumberGenerator.GetBytes(32),Guid.NewGuid()),"Beta",security:null,true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);

        Assert.That(item.GetKeys(),Is.Null);

        IReadOnlyDictionary<String,SecurityKey>? result = item.GetKeys(null,context);

        Assert.That(result,Is.Not.Null);
        Assert.That(result!.Count,Is.EqualTo(2));
        Assert.That(result.Keys,Has.Member("Alpha"));
        Assert.That(result.Keys,Has.Member("Beta"));
    }

    [Test]
    public async Task RemoveKeys_ByName_Encrypted_RoundTrip()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        ServiceKey a = new(RandomNumberGenerator.GetBytes(32),Guid.NewGuid());
        HostKey b = new(RandomNumberGenerator.GetBytes(32),Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKey(a,"Alpha",security:null,true),Is.True);
        Assert.That(item.AddKey(b,"Beta",security:null,true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);

        Assert.That(item.RemoveKeys(["Alpha"],context,true),Is.True);

        IReadOnlyDictionary<String,SecurityKey>? result = item.GetKeys(null,context);

        Assert.That(result,Is.Not.Null);
        Assert.That(result!.Count,Is.EqualTo(1));
        Assert.That(result.ContainsKey("Beta"),Is.True);
    }

    [Test]
    public async Task EncryptData_InMemoryKeys_Empty()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataProtectionInfo(),Is.Not.Null);
        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task EncryptData_InMemoryKeys_Locked()
    {
        KeySet item = new();
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        foreach(SecurityKey securityKey in KeySetTestHelpers.MakeKeys(12)) { Assert.That(item.AddKey(securityKey,null,security:null,sign:true),Is.True); }
        Assert.That(item.Lock(key),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataProtectionInfo(),Is.Not.Null);
    }

    [Test]
    public async Task EncryptData_InMemoryKeys_NotEncrypted()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.False);
        Assert.That(item.GetAllKeys(),Is.Not.Null);
    }

    // =====================
    //  Double Encrypt/Decrypt, Wrong Key, Tamper
    // =====================
    [Test]
    public async Task EncryptData_DoubleEncryption()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(),security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.False);
        Assert.That(item.GetAllKeys(),Is.Not.Null);
    }

    [Test]
    public async Task EncryptData_WrongContext()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"TestKey");
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,"WrongKey");

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.DecryptData(wrongContext).ConfigureAwait(false),Is.False);
        Assert.That(item.GetAllKeys(),Is.Null);
        IReadOnlyDictionary<String,SecurityKey>? snapshot = item.GetAllKeys(context);
        Assert.That(snapshot,Is.Not.Null);
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.False);
        Assert.That(item.GetAllKeys(),Is.Null);
    }

    // =====================
    //  Signature/Verification
    // =====================
    [Test]
    public async Task SignData_And_VerifyData_ValidSignature()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        foreach(SecurityKey key in KeySetTestHelpers.MakeKeys(12)) { Assert.That(item.AddKey(key,null,security:null,sign:true),Is.True); }
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        KeySetTestHelpers.TamperAllKeys(item,KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_ReplacesExistingFieldAssertion()
    {
        KeySet item = new();
        DataItemSecurityContext context1 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key1");
        DataItemSecurityContext context2 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key2");

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.SignData("Content",context1).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.SignData("Content",context2).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context2).ConfigureAwait(false),Is.True);
        Assert.That(await item.VerifyData("Content",context1).ConfigureAwait(false),Is.False);
        KeySetTestHelpers.TamperAllKeys(item,KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("Content",context2).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_InvalidOrMissingHash()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullContextOrField()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(),security:null,sign:true),Is.True);
        Assert.That(await item.SignData(null,context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.SignData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData(null,context).ConfigureAwait(false),Is.False);
        Assert.That(await item.VerifyData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_ValidSignature()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        foreach(SecurityKey key in KeySetTestHelpers.MakeKeys(12)) { Assert.That(item.AddKey(key,null,security:null,sign:true),Is.True); }
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        KeySetTestHelpers.TamperAllKeys(item,KeySetTestHelpers.MakeKeys());
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        KeySet item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.AddKeys(KeySetTestHelpers.MakeKeys(12),security:null,sign:true),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(item.SetID(Guid.NewGuid()),Is.True);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.False);
    }

    // =====================
    //  Encrypted mutation behavior
    // =====================
    [Test]
    public async Task Encrypted_AddRemove_WithManager_MaintainsEncryption_AndIntegrity()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);
        List<SecurityKey> initial = KeySetTestHelpers.MakeKeys(12);

        Assert.That(item.AddKeys(initial,security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);

        SecurityKey toAdd = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.AddKey(toAdd,null,context,sign:true),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
        IReadOnlyDictionary<String,SecurityKey>? snap1 = item.GetAllKeys(context);
        Assert.That(snap1!.Values.Any(k => k.Equals(toAdd)),Is.True);

        Assert.That(item.RemoveKey(toAdd.ID!.Value.ToString(),context,sign:true),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
        IReadOnlyDictionary<String,SecurityKey>? snap2 = item.GetAllKeys(context);
        Assert.That(snap2!.Values.Any(k => k.Equals(toAdd)),Is.False);
    }

    [Test]
    public async Task Encrypted_PartialDuplicateAndMissing()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);
        Guid id = Guid.NewGuid();
        ManagerKey a = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Test"))!,id);

        Assert.That(item.AddKey(a,null,security:null,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);

        ManagerKey duplicateA = new(a.Key!,a.ID!.Value);
        SecurityKey newB = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.AddKeys([duplicateA,newB],context,sign:true),Is.False);
        IReadOnlyDictionary<String,SecurityKey> snapAdd = item.GetAllKeys(context)!;
        Assert.That(snapAdd.Values.Any(k => k.Equals(a)),Is.True);
        Assert.That(snapAdd.Values.Any(k => k.Equals(newB)),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);

        SecurityKey missing = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.RemoveKeys([newB,missing],context,sign:true),Is.False);
        IReadOnlyDictionary<String,SecurityKey> snapRem = item.GetAllKeys(context)!;
        Assert.That(snapRem.Values.Any(k => k.Equals(newB)),Is.False);
        Assert.That(snapRem.Values.Any(k => k.Equals(a)),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task LockedAndEncrypted_AddRemove_WithManager_MaintainsStateAndIntegrity()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);
        List<SecurityKey> initial = KeySetTestHelpers.MakeKeys(12);

        Assert.That(item.AddKeys(initial,security:null,sign:true),Is.True);
        Assert.That(item.TakeOwnership(mgr),Is.True);
        Assert.That(item.Lock(mgr),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);

        Assert.That(item.GetLocked(),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);

        SecurityKey toAdd = KeySetTestHelpers.MakeKeys(1).Single();
        Assert.That(item.AddKey(toAdd,null,context,sign:true),Is.True);

        Assert.That(item.GetLocked(),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
        IReadOnlyDictionary<String,SecurityKey>? snap1 = item.GetAllKeys(context);
        Assert.That(snap1!.Values.Any(k => k.Equals(toAdd)),Is.True);

        Assert.That(item.RemoveKey(toAdd.ID!.Value.ToString(),context,sign:true),Is.True);

        Assert.That(item.GetLocked(),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
        IReadOnlyDictionary<String,SecurityKey>? snap2 = item.GetAllKeys(context);
        Assert.That(snap2!.Values.Any(k => k.Equals(toAdd)),Is.False);
    }

    // =====================
    //  Authorization
    // =====================
    [Test]
    public void GetAllKeys_Locked_ManagerFails()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("Owner");
        ManagementKey? manager = item.CreateManagementKey("Manager");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(2);

        Assert.That(item.AddKeys(keys,DataItemSecurityContextExamHelpers.CreateContext(item,manager!),true),Is.True);
        Assert.That(item.Lock(owner),Is.True);
        Assert.That(item.GetAllKeys(DataItemSecurityContextExamHelpers.CreateContext(item,manager!)),Is.Null);
        Assert.That(item.GetAllKeys(DataItemSecurityContextExamHelpers.CreateContext(item,owner!)),Is.Not.Null);
    }

    // =====================
    //  Ownership
    // =====================
    [Test]
    public void CreateOwnerKey_CreatesAndTakesOwnership()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("TestOwner");

        Assert.That(owner,Is.Not.Null);
        Assert.That(item.CheckOwner(owner),Is.True);
    }

    [Test]
    public void TakeOwnership_And_CheckOwner_And_ReleaseOwnership_Cycle()
    {
        KeySet item = new();
        ManagementKey? key = item.CreateManagementKey("TestKey");

        Assert.That(item.CheckOwner(key),Is.False);
        Assert.That(item.TakeOwnership(key),Is.True);
        Assert.That(item.CheckOwner(key),Is.True);
        ManagementKey? wrongKey = item.CreateManagementKey("WrongKey");
        Assert.That(item.CheckOwner(wrongKey),Is.False);
        Assert.That(item.TakeOwnership(wrongKey),Is.False);
        Assert.That(item.ReleaseOwnership(wrongKey),Is.False);
        Assert.That(item.ReleaseOwnership(key),Is.True);
        Assert.That(item.CheckOwner(key),Is.False);
    }

    [Test]
    public void TakeOwnership_FailsWhenLocked()
    {
        KeySet item = new();
        ManagementKey? key = item.CreateManagementKey("TestKey");

        Assert.That(item.Lock(key),Is.True);
        Assert.That(item.TakeOwnership(key),Is.False);
    }

    [Test]
    public void ReleaseOwnership_FailsWithoutOwnership()
    {
        KeySet item = new();
        ManagementKey? key = item.CreateManagementKey("TestKey");

        Assert.That(item.ReleaseOwnership(key),Is.False);
    }

    [Test]
    public void CreateOwnerKey_FailsWhenOwned()
    {
        KeySet item = new();
        ManagementKey? owner1 = item.CreateOwnerKey("Owner1");
        ManagementKey? owner2 = item.CreateOwnerKey("Owner2");

        Assert.That(owner1,Is.Not.Null);
        Assert.That(owner2,Is.Null);
    }

    // =====================
    //  ClearKeys
    // =====================
    [Test]
    public async Task ClearKeys_Unencrypted_ClearsAllKeysAndInvalidatesSignature()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKeys(keys,context,sign:true),Is.True);
        Assert.That(item.GetAllKeys(),Is.Not.Null);
        Assert.That(item.GetAllKeys()!.Count,Is.EqualTo(12));
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
        Assert.That(item.ClearKeys(context),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task ClearKeys_Encrypted_ClearsEncryptedDataAndInvalidatesSignature()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKeys(keys,context,sign:true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
        Assert.That(item.ClearKeys(context),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public void ClearKeys_Locked_Authorization()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        ManagementKey? wrongMgr = item.CreateManagementKey("WrongKey");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKeys(keys,context,sign:true),Is.True);
        Assert.That(item.TakeOwnership(mgr),Is.True);
        Assert.That(item.Lock(mgr),Is.True);
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,wrongMgr!);
        Assert.That(item.GetAllKeys(context)!.Count,Is.EqualTo(12));
        Assert.That(item.ClearKeys((DataItemSecurityContext?)null),Is.False);
        Assert.That(item.GetAllKeys(context)!.Count,Is.EqualTo(12));
        Assert.That(item.ClearKeys(wrongContext),Is.False);
        Assert.That(item.GetAllKeys(context)!.Count,Is.EqualTo(12));
        Assert.That(item.ClearKeys(context),Is.True);
        Assert.That(item.GetAllKeys(context),Is.Null);
    }

    [Test]
    public async Task ClearKeys_Locked_Encrypted_Authorization()
    {
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        ManagementKey? wrongMgr = item.CreateManagementKey("WrongKey");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,mgr!);

        Assert.That(item.AddKeys(keys,context,sign:true),Is.True);
        Assert.That(item.TakeOwnership(mgr),Is.True);
        Assert.That(item.Lock(mgr),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.True);
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,wrongMgr!);
        Assert.That(item.GetAllKeys(context)!.Count,Is.EqualTo(12));
        Assert.That(item.ClearKeys((DataItemSecurityContext?)null),Is.False);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(item.GetAllKeys(context)!.Count,Is.EqualTo(12));
        Assert.That(item.ClearKeys(wrongContext),Is.False);
        Assert.That(item.GetDataEncrypted(),Is.True);
        Assert.That(item.GetAllKeys(context)!.Count,Is.EqualTo(12));
        Assert.That(item.ClearKeys(context),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetAllKeys(context),Is.Null);
    }

    // =====================
    //  File Roundtrip
    // =====================
    [Test]
    public async Task ToFile_FromFile_Roundtrip_Unencrypted()
    {
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        ManagementKey? key = item.CreateManagementKey("FileRoundtripKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);

        Assert.That(item.AddKeys(keys,context,sign:true),Is.True);
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            KeySet? loadedItem = KeySet.FromFile(file);
            Assert.That(loadedItem,Is.Not.Null);
            Assert.That(await loadedItem!.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(loadedItem.GetAllKeys(context)!.Count,Is.EqualTo(12));
            Assert.That(loadedItem.GetDataEncrypted(),Is.False);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Test]
    public async Task ToFile_FromFile_Roundtrip_Encrypted()
    {
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        KeySet item = new();
        ManagementKey? mgr = item.CreateManagementKey("TestKey");
        ManagementKey? key = item.CreateManagementKey("FileRoundtripKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);

        Assert.That(item.AddKeys(keys,context,sign:true),Is.True);
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            KeySet? loadedItem = KeySet.FromFile(file);
            Assert.That(loadedItem,Is.Not.Null);
            Assert.That(loadedItem!.GetDataEncrypted(),Is.True);
            Assert.That(loadedItem.GetAllKeys(),Is.Null);
            Assert.That(loadedItem.GetDataProtectionInfo(),Is.Not.Null);
            Assert.That(await loadedItem.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(await loadedItem.DecryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(loadedItem.GetAllKeys(context)!.Count,Is.EqualTo(12));
        }
        finally
        {
            File.Delete(file);
        }
    }

    // =====================
    //  WipeData
    // =====================
    [Test]
    public async Task WipeData_InMemory_NotEncrypted()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("TestOwner");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,owner!);

        Assert.That(owner,Is.Not.Null);
        Assert.That(item.AddKeys(keys,DataItemSecurityContextExamHelpers.CreateContext(item,owner!),true),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,owner!)).ConfigureAwait(false),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.CheckOwner(owner),Is.True);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);

        var secureHashesField = typeof(DataItem).GetField("FieldIntegrityHashes", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureHashesField,Is.Not.Null);
        Assert.That(secureHashesField!.GetValue(item),Is.Null);

        var secureSignaturesField = typeof(DataItem).GetField("FieldAssertions", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureSignaturesField,Is.Not.Null);
        Assert.That(secureSignaturesField!.GetValue(item),Is.Null);
    }

    [Test]
    public async Task WipeData_InMemory_Encrypted()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("TestOwner");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,owner!);

        Assert.That(owner,Is.Not.Null);
        Assert.That(item.AddKeys(keys,DataItemSecurityContextExamHelpers.CreateContext(item,owner!),true),Is.True);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,owner!)).ConfigureAwait(false),Is.True);
        Assert.That(item.GetAllKeys(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);

        var encryptedDataField = typeof(DataItem).GetField("EncryptedData", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(encryptedDataField,Is.Not.Null);
        Assert.That(encryptedDataField!.GetValue(item),Is.Null);
    }

    [Test]
    public async Task WipeData_StreamedContent()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("TestOwner");

        Assert.That(owner,Is.Not.Null);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,owner!)).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContentStreamed(),Is.False);
    }

    [Test]
    public async Task WipeData_Locked_InvalidKey()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("TestOwner");
        ManagementKey? invalidKey = item.CreateManagementKey("InvalidKey");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);

        Assert.That(owner,Is.Not.Null);
        Assert.That(item.AddKeys(keys,DataItemSecurityContextExamHelpers.CreateContext(item,owner!),true),Is.True);
        Assert.That(item.Lock(owner),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,invalidKey!)).ConfigureAwait(false),Is.False);
        DataItemSecurityContext ownerContext = DataItemSecurityContextExamHelpers.CreateContext(item,owner!);
        Assert.That(item.GetAllKeys(ownerContext),Is.Not.Null);
        Assert.That(item.GetAllKeys(ownerContext)!.Count,Is.EqualTo(12));
    }

    [Test]
    public async Task WipeData_Locked_ValidKey()
    {
        KeySet item = new();
        ManagementKey? owner = item.CreateOwnerKey("TestOwner");
        List<SecurityKey> keys = KeySetTestHelpers.MakeKeys(12);

        Assert.That(owner,Is.Not.Null);
        Assert.That(item.AddKeys(keys,DataItemSecurityContextExamHelpers.CreateContext(item,owner!),true),Is.True);
        Assert.That(item.Lock(owner),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,owner!)).ConfigureAwait(false),Is.True);
        Assert.That(item.GetAllKeys(DataItemSecurityContextExamHelpers.CreateContext(item,owner!)),Is.Null);
    }
}
