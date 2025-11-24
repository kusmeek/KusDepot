namespace KusDepot.Exams.DataItems;

// =====================
//  GenericItem-specific Helpers
// =====================
public static class GenericItemTestHelpers
{
    public static void TamperContent(GenericItem item, IEnumerable<Object> newValue)
    {
        var field = typeof(GenericItem).GetField("Content", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Content field not found");
        field.SetValue(item, newValue.ToList());
    }
}

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class GenericItemSecurityExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryContent_Valid()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.CheckDataHash()).IsTrue();
    }

    [Test]
    public async Task CheckDataHash_InMemoryContent_Tampered()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        Check.That(item.SetContent(content)).IsTrue();
        var tampered = new List<Object>(content) { "tampered" };
        GenericItemTestHelpers.TamperContent(item, tampered);
        Check.That(await item.CheckDataHash()).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Valid()
    {
        var item = new GenericItem();
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        try
        {
            Check.That(await item.CheckDataHash()).IsTrue();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Tampered()
    {
        var item = new GenericItem();
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        try
        {
            DataItemSecurityTestHelpers.TamperFile(file);
            Check.That(await item.CheckDataHash()).IsFalse();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public static async Task CheckDataHash_EmptyContent()
    {
        var item = new GenericItem();
        Check.That(item.SetContent(new List<Object>())).IsTrue();
        Check.That(await item.CheckDataHash()).IsFalse();
    }

    // =====================
    //  Signature-Based Data Integrity
    // =====================
    [Test]
    public async Task VerifyData_Content_ValidSignature()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content, key)).IsTrue();
        Check.That(await item.VerifyData("Content", key)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_InvalidSignature()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content, key)).IsTrue();
        var tampered = new List<Object>(content) { "tampered" };
        GenericItemTestHelpers.TamperContent(item, tampered);
        Check.That(await item.VerifyData("Content", key)).IsFalse();
    }

    [Test]
    public async Task VerifyData_Content_NoKeyProvided()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content, key)).IsTrue();
        Check.That(await item.VerifyData("Content", null)).IsFalse();
    }

    // =====================
    //  Encrypted Data Signature
    // =====================
    [Test]
    public static async Task VerifyData_EncryptedData_ValidSignature()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();
    }

    [Test]
    public static async Task VerifyData_EncryptedData_Tampered()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.VerifyData("EncryptedData", key)).IsFalse();
    }

    // =====================
    //  EncryptData/DecryptData/Hash/Info
    // =====================
    [Test]
    public async Task EncryptData_SetsAndClears_DataEncryptInfo()
    {
        var content = new List<Object> { "test content" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        var info = item.GetDataEncryptInfo();
        Check.That(info).IsNotNull();
        Check.That(info!.Serial).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.ThumbPrint).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.PublicKey).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(await item.DecryptData(key)).IsTrue();
        var infoAfter = item.GetDataEncryptInfo();
        Check.That(infoAfter).IsNull();
    }

    // =====================
    //  EncryptData/DecryptData Roundtrip
    // =====================
    [Test]
    public async Task EncryptData_InMemoryContent_RoundTrip()
    {
        var item = new GenericItem();
        var content = new List<Object> { "test", 123 };
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(item.GetDataEncryptInfo()).IsNotNull();
        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(item.GetContent()!.SequenceEqual(content)).IsTrue();
        Check.That(item.GetDataEncryptInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Empty()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(new List<Object>())).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
        Check.That(await item.DecryptData(key)).IsFalse();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Locked()
    {
        var content = new List<Object> { "locked content" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_NotEncrypted()
    {
        var content = new List<Object> { "plain content" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.DecryptData(key)).IsFalse();
        Check.That(item.GetContent()!.SequenceEqual(content)).IsTrue();
    }

    // =====================
    //  Streamed Content Encrypt/Decrypt
    // =====================
    [Test]
    public async Task EncryptData_StreamedContent_RoundTrip()
    {
        var item = new GenericItem();
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");
        try
        {
            Check.That(await item.EncryptData(key, false)).IsTrue();
            Check.That(item.GetDataEncryptInfo()).IsNotNull();
            Check.That(await item.DecryptData(key)).IsTrue();
            Check.That(item.GetDataEncryptInfo()).IsNull();
            var restored = File.ReadAllBytes(file);
            Check.That(restored).IsEqualTo(data);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptData_StreamedContent_MissingFile()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var file = Path.GetTempFileName() + ".missing";
        item.SetFILE(file);
        item.SetContentStreamed(true);
        Check.That(await item.EncryptData(key, false)).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_StreamedContent_Locked()
    {
        var item = new GenericItem();
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");
        try
        {
            Check.That(item.Lock(key)).IsTrue();
            Check.That(await item.EncryptData(key, false)).IsFalse();
            Check.That(item.GetDataEncryptInfo()).IsNull();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptData_StreamedContent_NotEncrypted()
    {
        var item = new GenericItem();
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");
        try
        {
            Check.That(await item.DecryptData(key)).IsFalse();
            var restored = File.ReadAllBytes(file);
            Check.That(restored).IsEqualTo(data);
        }
        finally { File.Delete(file); }
    }

    // =====================
    //  Double Encrypt/Decrypt, Wrong Key, Tamper
    // =====================
    [Test]
    public async Task EncryptData_DoubleEncryption()
    {
        var content = new List<Object> { "double encrypt" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        var content = new List<Object> { "double decrypt" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(await item.DecryptData(key)).IsFalse();
        Check.That(item.GetContent()!.SequenceEqual(content)).IsTrue();
    }

    [Test]
    public async Task EncryptData_WrongKey()
    {
        var content = new List<Object> { "wrong key" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        var wrongKey = item.CreateManagementKey("WrongKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(await item.DecryptData(wrongKey)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        var content = new List<Object> { "tampered" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.DecryptData(key)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    // =====================
    //  Signature/Verification
    // =====================
    [Test]
    public async Task SignData_And_VerifyData_ValidSignature()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("Content", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content", key), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("Content", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        var tampered = new List<Object>(content) { "tampered" };
        GenericItemTestHelpers.TamperContent(item, tampered);
        Assert.That(await item.VerifyData("Content", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_MultipleSignatures_UsesMostRecent()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        var sigKey1 = item.SignData("Content", key);
        Thread.Sleep(10);
        var sigKey2 = item.SignData("Content", key);
        Assert.That(sigKey1, Is.Not.EqualTo(sigKey2));
        Assert.That(await item.VerifyData("Content", key), Is.True);
        var tampered = new List<Object>(content) { "tampered" };
        GenericItemTestHelpers.TamperContent(item, tampered);
        Assert.That(await item.VerifyData("Content", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_InvalidOrMissingHash()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.SignData("Content", key), Is.Null);
        Assert.That(await item.VerifyData("Content", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullKeyOrField()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        Assert.That(item.SignData(null, key), Is.Null);
        Assert.That(item.SignData("Content", null), Is.Null);
        Assert.That(await item.VerifyData(null, key), Is.False);
        Assert.That(await item.VerifyData("Content", null), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContent_ValidSignature()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        try
        {
            item.SetContentStreamed(true);
            var sigKey = item.SignData("Content", key);
            Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
            Assert.That(await item.VerifyData("Content", key), Is.True);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContent_Tampered()
    {
        var item = new GenericItem();
        var data = new Byte[1024];
        RandomNumberGenerator.Create().GetBytes(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");
        try
        {
            item.SetContentStreamed(true);
            var sigKey = item.SignData("Content", key);
            Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
            DataItemSecurityTestHelpers.TamperFile(file);
            Assert.That(await item.VerifyData("Content", key), Is.False);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task SignData_And_VerifyData_MultiCertificate_IndependentSignatures()
    {
        var content = new List<Object> { "test content" };
        var item = new GenericItem(content);
        var key1 = item.CreateManagementKey("Key1");
        var key2 = item.CreateManagementKey("Key2");

        Assert.That(await item.EncryptData(key1, true), Is.True);
        var sigKey2 = item.SignData("EncryptedData", key2);
        Assert.That(sigKey2, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("EncryptedData", key2), Is.True);
        Assert.That(await item.VerifyData("EncryptedData", key1), Is.True);

        Assert.That(await item.DecryptData(key1), Is.True);
        var sigContentKey2 = item.SignData("Content", key2);
        Assert.That(sigContentKey2, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content", key2), Is.True);
        Assert.That(await item.VerifyData("Content", key1), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_ValidSignature()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("HashCode", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode", key), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("HashCode", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        var tampered = new List<Object>(content) { "tampered" };
        GenericItemTestHelpers.TamperContent(item, tampered);
        Assert.That(await item.VerifyData("HashCode", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        var item = new GenericItem();
        var key = item.CreateManagementKey("TestKey");
        var content = new List<Object> { "test", 123 };
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("HashCode", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(item.SetID(Guid.NewGuid()), Is.True);
        Assert.That(await item.VerifyData("HashCode", key), Is.False);
    }

    // =====================
    //  SetContent on Encrypted Item
    // =====================
    [Test]
    public async Task SetContent_Encrypted_Roundtrip()
    {
        var item = new GenericItem();
        var initialContent = new List<Object> { "public class Initial {}" };
        var key = item.CreateManagementKey("TestKey");

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        var newContent = new List<Object> { "public class New {}" };

        Check.That(item.SetContent(newContent, key)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(item.GetContent()!.SequenceEqual(newContent)).IsTrue();
    }

    [Test]
    public async Task SetContent_Encrypted_EmptyContent()
    {
        var item = new GenericItem();
        var content = new List<Object> { "public class ToBeCleared {}" };
        var key = item.CreateManagementKey("TestKey");

        item.SetContent(content);
        await item.EncryptData(key, true);
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(item.SetContent(new List<Object>(), key)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData", key)).IsFalse();

        await item.DecryptData(key);
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task SetContent_Encrypted_NoKey()
    {
        var item = new GenericItem();
        var initialContent = new List<Object> { "public class InitialNoKey {}" };
        var key = item.CreateManagementKey("TestKey");

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        var newContent = new List<Object> { "public class NewNoKey {}" };

        Check.That(item.SetContent(newContent, null)).IsFalse();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();
    }

    // =====================
    //  ClearEncryptedData
    // =====================
    [Test]
    public async Task ClearEncryptedData()
    {
        var item = new GenericItem(new List<Object> { "some content" });
        var key = item.CreateManagementKey("TestKey");

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.GetDataEncrypted()).IsTrue();
        Check.That(item.GetDataEncryptInfo()).IsNotNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();
        Check.That(item.GetContent()).IsNull();

        Check.That(item.ClearEncryptedData(key)).IsTrue();
        Check.That(item.GetDataEncrypted()).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsFalse();
    }

    // =====================
    //  Data Masking
    // =====================

    [Test]
    public async Task MaskData_InMemory_NotEncrypted()
    {
        var content = new List<Object> { "mask-inmem-plain", Guid.NewGuid() };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.GetContent()!.SequenceEqual(content), Is.True);
        Assert.That(((await item.GetDataContent())!.Generic as IEnumerable<Object>)!.SequenceEqual(content), Is.True);
        Assert.That(item.MaskData(true, key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
        Assert.That(await item.GetDataContent(), Is.Null);
        Assert.That(((await item.GetDataContent(key))!.Generic as IEnumerable<Object>)!.SequenceEqual(content), Is.True);
        Assert.That(item.MaskData(false, null), Is.False);
        Assert.That(item.MaskData(false, key), Is.True);
        Assert.That(item.GetContent()!.SequenceEqual(content), Is.True);
        Assert.That(((await item.GetDataContent())!.Generic as IEnumerable<Object>)!.SequenceEqual(content), Is.True);
    }

    [Test]
    public async Task MaskData_InMemory_Encrypted()
    {
        var content = new List<Object> { "mask-inmem-encrypted", Guid.NewGuid() };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(await item.EncryptData(key, false), Is.True);
        Assert.That(item.MaskData(true, key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
        Assert.That(await item.GetDataContent(), Is.Null);
        Assert.That(((await item.GetDataContent(key))!.Generic as IEnumerable<Object>)!.SequenceEqual(content), Is.True);
        Assert.That(item.MaskData(false, null), Is.False);
        Assert.That(item.MaskData(false, key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
        Assert.That(await item.GetDataContent(), Is.Null);
        Assert.That(((await item.GetDataContent(key))!.Generic as IEnumerable<Object>)!.SequenceEqual(content), Is.True);
    }

    [Test]
    public async Task MaskData_Streamed_NotEncrypted()
    {
        var data = new Byte[128];
        RandomNumberGenerator.Create().GetBytes(data);
        var item = new GenericItem();
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");
        try
        {
            Assert.That(item.GetContentStreamed(), Is.True);
            using (var stream = item.GetContentStream())
            {
                Assert.That(stream, Is.Not.Null);
            }
            Assert.That(await item.GetDataContent(), Is.Null);
            Assert.That(await item.GetDataContent(key), Is.Null);
            Assert.That(item.MaskData(true, key), Is.True);
            Assert.That(item.GetContentStream(), Is.Null);
            Assert.That(item.GetContent(), Is.Null);
            Assert.That(await item.GetDataContent(), Is.Null);
            Assert.That(await item.GetDataContent(key), Is.Null);
            Assert.That(item.MaskData(false, null), Is.False);
            Assert.That(item.MaskData(false, key), Is.True);
            using (var stream = item.GetContentStream())
            {
                Assert.That(stream, Is.Not.Null);
            }
            Assert.That(await item.GetDataContent(), Is.Null);
            Assert.That(await item.GetDataContent(key), Is.Null);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task MaskData_Streamed_Encrypted()
    {
        var data = new Byte[128];
        RandomNumberGenerator.Create().GetBytes(data);
        var item = new GenericItem();
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");
        try
        {
            Assert.That(await item.EncryptData(key, false), Is.True);
            var encrypted = File.ReadAllBytes(file);
            Assert.That(encrypted, Is.Not.EqualTo(data));
            Assert.That(item.MaskData(true, key), Is.True);
            Assert.That(item.GetContent(), Is.Null);
            Assert.That(await item.GetDataContent(), Is.Null);
            Assert.That(await item.GetDataContent(key), Is.Null);
            Assert.That(item.MaskData(false, null), Is.False);
            Assert.That(item.MaskData(false, key), Is.True);
            using (var stream = item.GetContentStream())
            {
                Assert.That(stream, Is.Not.Null);
            }
            Assert.That(item.GetContent(), Is.Null);
            Assert.That(await item.GetDataContent(), Is.Null);
            var stillEncrypted = File.ReadAllBytes(file);
            Assert.That(stillEncrypted, Is.Not.EqualTo(data));
            Assert.That(await item.DecryptData(key), Is.True);
            var decrypted = File.ReadAllBytes(file);
            Assert.That(decrypted, Is.EqualTo(data));
            using (var stream = item.GetContentStream())
            {
                Assert.That(stream, Is.Not.Null);
            }
        }
        finally { File.Delete(file); }
    }

    // =====================
    //  File Roundtrip
    // =====================
    [Test]
    public async Task ToFile_FromFile_Roundtrip_Unencrypted()
    {
        var content = new List<Object> { "unencrypted-file-roundtrip", Guid.NewGuid() };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("FileRoundtripKey");
        var file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(item.SignData("Content", key), Is.Not.Null);
            Assert.That(await item.VerifyData("Content", key), Is.True);
            Assert.That(item.ToFile(file), Is.True);

            var loadedItem = GenericItem.FromFile(file);
            Assert.That(loadedItem, Is.Not.Null);

            Assert.That(await loadedItem!.VerifyData("Content", key), Is.True);
            Assert.That(loadedItem.GetContent()!.SequenceEqual(content), Is.True);
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
        var content = new List<Object> { "encrypted-file-roundtrip", Guid.NewGuid() };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("FileRoundtripKey");
        var file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(key, true), Is.True);
            Assert.That(await item.VerifyData("EncryptedData", key), Is.True);
            Assert.That(item.ToFile(file), Is.True);

            var loadedItem = GenericItem.FromFile(file);
            Assert.That(loadedItem, Is.Not.Null);

            Assert.That(loadedItem!.GetDataEncrypted(), Is.True);
            Assert.That(loadedItem.GetContent(), Is.Null);
            Assert.That(loadedItem.GetDataEncryptInfo(), Is.Not.Null);
            Assert.That(await loadedItem.VerifyData("EncryptedData", key), Is.True);

            Assert.That(await loadedItem.DecryptData(key), Is.True);
            Assert.That(loadedItem.GetContent()!.SequenceEqual(content), Is.True);
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
        var textItem = new TextItem("nested text");
        var byteArray = new Byte[] { 1, 2, 3, 4 };
        var string1 = new String("abcd");
        var content = new List<Object> { textItem, byteArray, string1, 123 };

        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.SignData("Content", key), Is.Not.Null);

        Assert.That(await item.ZeroData(key), Is.True);

        Assert.That(item.GetContent(), Is.Null);
        Assert.That(byteArray, Is.EqualTo(new Byte[] { 0, 0, 0, 0 }));
        Assert.That(string1, Is.EqualTo(new Char[] { '\0', '\0', '\0', '\0' }));
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
        var content = new List<Object> { "some content" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(await item.EncryptData(key, true), Is.True);

        Assert.That(await item.ZeroData(key), Is.True);

        Assert.That(item.GetContent(), Is.Null);
        Assert.That(item.GetDataEncrypted(), Is.False);
        Assert.That(item.GetDataEncryptInfo(), Is.Null);

        var encryptedDataField = typeof(DataItem).GetField("EncryptedData", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(encryptedDataField, Is.Not.Null);
        Assert.That(encryptedDataField!.GetValue(item), Is.Null);
    }

    [Test]
    public async Task ZeroData_StreamedContent()
    {
        var item = new GenericItem();
        var data = new Byte[128];
        RandomNumberGenerator.Fill(data);
        var file = DataItemSecurityTestHelpers.SetupStreamedContent(item, data);
        var key = item.CreateManagementKey("TestKey");

        try
        {
            Assert.That(await item.ZeroData(key), Is.True);

            Assert.That(File.Exists(file), Is.False);
            Assert.That(item.GetFILE(), Is.Null);
            Assert.That(item.GetContentStreamed(), Is.False);
        }
        finally
        {
            if (File.Exists(file)) File.Delete(file);
        }
    }

    [Test]
    public async Task ZeroData_Locked_InvalidKey()
    {
        var content = new List<Object> { "some content" };
        var item = new GenericItem(content);
        var validKey = item.CreateManagementKey("ValidKey");
        var invalidKey = new ManagerKey(CreateCertificate(Guid.NewGuid(), "InvalidKey", 2048, 1, null)!);
        Assert.That(item.Lock(validKey), Is.True);

        Assert.That(await item.ZeroData(invalidKey), Is.False);
        Assert.That(item.GetContent(), Is.Not.Null);
        Assert.That(item.GetContent()!.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ZeroData_Locked_ValidKey()
    {
        var content = new List<Object> { "some content" };
        var item = new GenericItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.Lock(key), Is.True);

        Assert.That(await item.ZeroData(key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
    }
}