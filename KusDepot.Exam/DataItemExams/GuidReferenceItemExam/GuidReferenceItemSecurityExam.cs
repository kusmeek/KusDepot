namespace KusDepot.Exams.DataItems;

// =====================
//  GuidReferenceItem-specific Helpers
// =====================
public static class GuidReferenceItemTestHelpers
{
    public static void TamperContent(GuidReferenceItem item, Guid newValue)
    {
        var field = typeof(GuidReferenceItem).GetField("Content", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Content field not found");
        field.SetValue(item, newValue);
    }
}

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class GuidReferenceItemSecurityExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryContent_Valid()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.CheckDataHash()).IsTrue();
    }

    [Test]
    public async Task CheckDataHash_InMemoryContent_Tampered()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        Check.That(item.SetContent(content)).IsTrue();
        var tampered = Guid.NewGuid();
        GuidReferenceItemTestHelpers.TamperContent(item, tampered);
        Check.That(await item.CheckDataHash()).IsFalse();
    }

    [Test]
    public void CheckDataHash_StreamedContent_Valid() {}

    [Test]
    public void CheckDataHash_StreamedContent_Tampered() {}

    [Test]
    public static async Task CheckDataHash_EmptyContent()
    {
        var item = new GuidReferenceItem();
        Check.That(item.SetContent(Guid.Empty)).IsTrue();
        Check.That(await item.CheckDataHash()).IsFalse();
    }

    // =====================
    //  Signature-Based Data Integrity
    // =====================
    [Test]
    public async Task VerifyData_Content_ValidSignature()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content, key)).IsTrue();
        Check.That(await item.VerifyData("Content", key)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_InvalidSignature()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content, key)).IsTrue();
        var tampered = Guid.NewGuid();
        GuidReferenceItemTestHelpers.TamperContent(item, tampered);
        Check.That(await item.VerifyData("Content", key)).IsFalse();
    }

    [Test]
    public async Task VerifyData_Content_NoKeyProvided()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
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
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();
    }

    [Test]
    public static async Task VerifyData_EncryptedData_Tampered()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
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
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
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
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(item.GetDataEncryptInfo()).IsNotNull();
        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(item.GetContent()).IsEqualTo(content);
        Check.That(item.GetDataEncryptInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Empty()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.SetContent(Guid.Empty)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
        Check.That(await item.DecryptData(key)).IsFalse();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Locked()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_NotEncrypted()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.DecryptData(key)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    // =====================
    //  Streamed Content Encrypt/Decrypt
    // =====================
    [Test]
    public void EncryptData_StreamedContent_RoundTrip() {}

    [Test]
    public void EncryptData_StreamedContent_MissingFile() {}

    [Test]
    public void EncryptData_StreamedContent_Locked() {}

    [Test]
    public void EncryptData_StreamedContent_NotEncrypted() {}

    // =====================
    //  Double Encrypt/Decrypt, Wrong Key, Tamper
    // =====================
    [Test]
    public async Task EncryptData_DoubleEncryption()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(await item.EncryptData(key, false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(await item.DecryptData(key)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    [Test]
    public async Task EncryptData_WrongKey()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        var wrongKey = item.CreateManagementKey("WrongKey");
        Check.That(await item.EncryptData(key, false)).IsTrue();
        Check.That(await item.DecryptData(wrongKey)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
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
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("Content", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content", key), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("Content", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        var tampered = Guid.NewGuid();
        GuidReferenceItemTestHelpers.TamperContent(item, tampered);
        Assert.That(await item.VerifyData("Content", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_MultipleSignatures_UsesMostRecent()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
        Assert.That(item.SetContent(content), Is.True);
        var sigKey1 = item.SignData("Content", key);
        Thread.Sleep(10);
        var sigKey2 = item.SignData("Content", key);
        Assert.That(sigKey1, Is.Not.EqualTo(sigKey2));
        Assert.That(await item.VerifyData("Content", key), Is.True);
        var tampered = Guid.NewGuid();
        GuidReferenceItemTestHelpers.TamperContent(item, tampered);
        Assert.That(await item.VerifyData("Content", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_InvalidOrMissingHash()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.SignData("Content", key), Is.Null);
        Assert.That(await item.VerifyData("Content", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullKeyOrField()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
        Assert.That(item.SetContent(content), Is.True);
        Assert.That(item.SignData(null, key), Is.Null);
        Assert.That(item.SignData("Content", null), Is.Null);
        Assert.That(await item.VerifyData(null, key), Is.False);
        Assert.That(await item.VerifyData("Content", null), Is.False);
    }

    [Test]
    public void SignData_And_VerifyData_StreamedContent_ValidSignature() {}

    [Test]
    public void SignData_And_VerifyData_StreamedContent_Tampered() {}

    [Test]
    public async Task SignData_And_VerifyData_MultiCertificate_IndependentSignatures()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
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
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("HashCode", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode", key), Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
        Assert.That(item.SetContent(content), Is.True);
        var sigKey = item.SignData("HashCode", key);
        Assert.That(sigKey, Is.Not.Null.And.Not.Empty);
        GuidReferenceItemTestHelpers.TamperContent(item, Guid.NewGuid());
        Assert.That(await item.VerifyData("HashCode", key), Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        var item = new GuidReferenceItem();
        var key = item.CreateManagementKey("TestKey");
        var content = Guid.NewGuid();
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
        var item = new GuidReferenceItem();
        var initialContent = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        var newContent = Guid.NewGuid();

        Check.That(item.SetContent(newContent, key)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(item.GetContent()).IsEqualTo(newContent);
    }

    [Test]
    public async Task SetContent_Encrypted_EmptyContent()
    {
        var item = new GuidReferenceItem();
        var content = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");

        item.SetContent(content);
        await item.EncryptData(key, true);
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(item.SetContent(Guid.Empty, key)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData", key)).IsFalse();

        await item.DecryptData(key);
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task SetContent_Encrypted_NoKey()
    {
        var item = new GuidReferenceItem();
        var initialContent = Guid.NewGuid();
        var key = item.CreateManagementKey("TestKey");

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        var newContent = Guid.NewGuid();

        Check.That(item.SetContent(newContent, null)).IsFalse();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();
    }

    // =====================
    //  ClearEncryptedData
    // =====================
    [Test]
    public async Task ClearEncryptedData()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
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
    //  SetNext/SetPrevious/SetUndirectedLinks on Encrypted Item
    // =====================
    [Test]
    public async Task SetNext_Encrypted_Roundtrip()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        var nextItem = new GuidReferenceItem(Guid.NewGuid());

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.GetNext()).IsNull();

        Check.That(item.SetNext(nextItem, key)).IsTrue();
        Check.That(item.GetNext()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(item.GetNext()).IsEqualTo(nextItem);
    }

    [Test]
    public async Task SetNext_Encrypted_NoKey()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        var nextItem = new GuidReferenceItem(Guid.NewGuid());

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.SetNext(nextItem, null)).IsFalse();
    }

    [Test]
    public async Task SetPrevious_Encrypted_Roundtrip()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        var previousItem = new GuidReferenceItem(Guid.NewGuid());

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.GetPrevious()).IsNull();

        Check.That(item.SetPrevious(previousItem, key)).IsTrue();
        Check.That(item.GetPrevious()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(await item.DecryptData(key)).IsTrue();
        Check.That(item.GetPrevious()).IsEqualTo(previousItem);
    }

    [Test]
    public async Task SetPrevious_Encrypted_NoKey()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        var previousItem = new GuidReferenceItem(Guid.NewGuid());

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.SetPrevious(previousItem, null)).IsFalse();
    }

    [Test]
    public async Task SetUndirectedLinks_Encrypted_Roundtrip()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        var links = new[] { new GuidReferenceItem(Guid.NewGuid()), new GuidReferenceItem(Guid.NewGuid()) };

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.GetUndirectedLinks()).IsNull();

        Check.That(item.SetUndirectedLinks(links, key)).IsTrue();
        Check.That(item.GetUndirectedLinks()).IsNull();
        Check.That(await item.VerifyData("EncryptedData", key)).IsTrue();

        Check.That(await item.DecryptData(key)).IsTrue();
        var decryptedLinks = item.GetUndirectedLinks();
        Check.That(decryptedLinks).IsNotNull();
        Check.That(decryptedLinks).HasSize(links.Length);
        Check.That(links.All(l => decryptedLinks!.Any(dl => dl.Equals(l)))).IsTrue();
    }

    [Test]
    public async Task SetUndirectedLinks_Encrypted_NoKey()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        var links = new[] { new GuidReferenceItem(Guid.NewGuid()) };

        Check.That(await item.EncryptData(key, true)).IsTrue();
        Check.That(item.SetUndirectedLinks(links, null)).IsFalse();
    }

    // =====================
    //  Data Masking
    // =====================

    [Test]
    public async Task MaskData_InMemory_NotEncrypted()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.GetContent(), Is.EqualTo(content));
        var dataContent = await item.GetDataContent();
        Assert.That(dataContent, Is.Not.Null);
        Assert.That(dataContent!.GuidReference, Is.Not.Null);
        Assert.That(dataContent.GuidReference!.ContainsKey("Content"), Is.True);
        Assert.That(dataContent.GuidReference["Content"], Is.EqualTo(content));
        Assert.That(item.MaskData(true, key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
        Assert.That(await item.GetDataContent(), Is.Null);
        var unmasked = await item.GetDataContent(key);
        Assert.That(unmasked, Is.Not.Null);
        Assert.That(unmasked!.GuidReference, Is.Not.Null);
        Assert.That(unmasked.GuidReference!.ContainsKey("Content"), Is.True);
        Assert.That(unmasked.GuidReference["Content"], Is.EqualTo(content));
        Assert.That(item.MaskData(false, null), Is.False);
        Assert.That(item.MaskData(false, key), Is.True);
        Assert.That(item.GetContent(), Is.EqualTo(content));
        var dataContent2 = await item.GetDataContent();
        Assert.That(dataContent2, Is.Not.Null);
        Assert.That(dataContent2!.GuidReference, Is.Not.Null);
        Assert.That(dataContent2.GuidReference!.ContainsKey("Content"), Is.True);
        Assert.That(dataContent2.GuidReference["Content"], Is.EqualTo(content));
    }

    [Test]
    public async Task MaskData_InMemory_Encrypted()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("TestKey");
        Assert.That(await item.EncryptData(key, false), Is.True);
        Assert.That(item.MaskData(true, key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
        Assert.That(await item.GetDataContent(), Is.Null);
        var unmasked = await item.GetDataContent(key);
        Assert.That(unmasked, Is.Not.Null);
        Assert.That(unmasked!.GuidReference, Is.Not.Null);
        Assert.That(unmasked.GuidReference!.ContainsKey("Content"), Is.True);
        Assert.That(unmasked.GuidReference["Content"], Is.EqualTo(content));
        Assert.That(item.MaskData(false, null), Is.False);
        Assert.That(item.MaskData(false, key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
        Assert.That(await item.GetDataContent(), Is.Null);
        var unmasked2 = await item.GetDataContent(key);
        Assert.That(unmasked2, Is.Not.Null);
        Assert.That(unmasked2!.GuidReference, Is.Not.Null);
        Assert.That(unmasked2.GuidReference!.ContainsKey("Content"), Is.True);
        Assert.That(unmasked2.GuidReference["Content"], Is.EqualTo(content));
    }

    [Test]
    public void MaskData_Streamed_NotEncrypted() {}

    [Test]
    public void MaskData_Streamed_Encrypted() {}

    // =====================
    //  File Roundtrip
    // =====================
    [Test]
    public async Task ToFile_FromFile_Roundtrip_Unencrypted()
    {
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("FileRoundtripKey");
        var file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(item.SignData("Content", key), Is.Not.Null);
            Assert.That(await item.VerifyData("Content", key), Is.True);
            Assert.That(item.ToFile(file), Is.True);

            var loadedItem = GuidReferenceItem.FromFile(file);
            Assert.That(loadedItem, Is.Not.Null);

            Assert.That(await loadedItem!.VerifyData("Content", key), Is.True);
            Assert.That(loadedItem.GetContent(), Is.EqualTo(content));
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
        var content = Guid.NewGuid();
        var item = new GuidReferenceItem(content);
        var key = item.CreateManagementKey("FileRoundtripKey");
        var file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(key, true), Is.True);
            Assert.That(await item.VerifyData("EncryptedData", key), Is.True);
            Assert.That(item.ToFile(file), Is.True);

            var loadedItem = GuidReferenceItem.FromFile(file);
            Assert.That(loadedItem, Is.Not.Null);

            Assert.That(loadedItem!.GetDataEncrypted(), Is.True);
            Assert.That(loadedItem.GetContent(), Is.Null);
            Assert.That(loadedItem.GetDataEncryptInfo(), Is.Not.Null);
            Assert.That(await loadedItem.VerifyData("EncryptedData", key), Is.True);

            Assert.That(await loadedItem.DecryptData(key), Is.True);
            Assert.That(loadedItem.GetContent(), Is.EqualTo(content));
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
        var item = new GuidReferenceItem(Guid.NewGuid());
        item.SetNext(new GuidReferenceItem(Guid.NewGuid()));
        item.SetPrevious(new GuidReferenceItem(Guid.NewGuid()));
        item.SetUndirectedLinks(new[] { new GuidReferenceItem(Guid.NewGuid()) });
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.SignData("Content", key), Is.Not.Null);

        Assert.That(await item.ZeroData(key), Is.True);

        Assert.That(item.GetContent(), Is.Null);
        Assert.That(item.GetNext(), Is.Null);
        Assert.That(item.GetPrevious(), Is.Null);
        Assert.That(item.GetUndirectedLinks(), Is.Null);
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
        var item = new GuidReferenceItem(Guid.NewGuid());
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
    public void ZeroData_StreamedContent() {}

    [Test]
    public async Task ZeroData_Locked_InvalidKey()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var validKey = item.CreateManagementKey("ValidKey");
        var invalidKey = new ManagerKey(CreateCertificate(Guid.NewGuid(), "InvalidKey", 2048, 1, null)!);
        Assert.That(item.Lock(validKey), Is.True);

        Assert.That(await item.ZeroData(invalidKey), Is.False);
        Assert.That(item.GetContent(), Is.Not.Null);
    }

    [Test]
    public async Task ZeroData_Locked_ValidKey()
    {
        var item = new GuidReferenceItem(Guid.NewGuid());
        var key = item.CreateManagementKey("TestKey");
        Assert.That(item.Lock(key), Is.True);

        Assert.That(await item.ZeroData(key), Is.True);
        Assert.That(item.GetContent(), Is.Null);
    }
}