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
        ClearHashCodeCache(item);
    }

    private static void ClearHashCodeCache(DataItem item)
    {
        var cachedHashCodeField = typeof(DataItem).GetField("CachedHashCode", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("CachedHashCode field not found");
        var hashCodeCachedField = typeof(DataItem).GetField("HashCodeCached", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("HashCodeCached field not found");

        cachedHashCodeField.SetValue(item, default(Int32));
        hashCodeCachedField.SetValue(item, false);
    }
}

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class GuidReferenceItemSecurityContextExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryContent_Valid()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task CheckDataHash_InMemoryContent_Tampered()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();

        Check.That(item.SetContent(content)).IsTrue();
        GuidReferenceItemTestHelpers.TamperContent(item,Guid.NewGuid());
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Valid()
    {
        GuidReferenceItem item = new();

        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContentStreamed()).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Tampered()
    {
        GuidReferenceItem item = new();
        item.SetFILE(Path.GetTempFileName() + ".kdi");

        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContentStreamed()).IsFalse();
    }

    [Test]
    public static async Task CheckDataHash_EmptyContent()
    {
        GuidReferenceItem item = new();
        Check.That(item.SetContent(Guid.Empty)).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    // =====================
    //  Signature-Based Data Integrity
    // =====================
    [Test]
    public async Task VerifyData_Content_ValidAssertion()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_InvalidAssertion()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        GuidReferenceItemTestHelpers.TamperContent(item,Guid.NewGuid());
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task VerifyData_Content_NoContextProvided()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
    }

    // =====================
    //  Encrypted Data Signature
    // =====================
    [Test]
    public async Task VerifyData_EncryptedData_ValidAssertion()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_EncryptedData_Tampered()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsFalse();
    }

    // =====================
    //  ProtectData/UnProtectData Validation
    // =====================
    [Test]
    public async Task ValidateData_Content_UsesCurrentState()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.ValidateData(context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task ProtectData_UnencryptedContent_EncryptsAndValidates()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.ProtectData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetDataEncrypted()).IsTrue();
        Check.That(item.GetDataProtectionInfo()).IsNotNull();
        Check.That(await item.ValidateData(context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task ProtectData_AlreadyEncrypted_TamperedState_FailsValidation()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.ProtectData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task UnProtectData_EncryptedContent_DecryptsAndValidates()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.UnProtectData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetDataEncrypted()).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
        Check.That(await item.ValidateData(context).ConfigureAwait(false)).IsTrue();
    }

    // =====================
    //  EncryptData/DecryptData/Hash/Info
    // =====================
    [Test]
    public async Task EncryptData_SetsAndClears_DataProtectionInfo()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataProtectionInfo? info = item.GetDataProtectionInfo();
        Check.That(info).IsNotNull();
        Check.That(info!.ProtectedByThumbprint).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.Recipients.IsDefaultOrEmpty).IsFalse();
        Check.That(info.ProtectionMode).IsEqualTo(nameof(DataFieldState.EncryptedContent));

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    // =====================
    //  EncryptData/DecryptData Roundtrip
    // =====================
    [Test]
    public async Task EncryptData_InMemoryContent_RoundTrip()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(item.GetDataProtectionInfo()).IsNotNull();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetContent()).IsEqualTo(content);
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Empty()
    {
        GuidReferenceItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(Guid.Empty)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Locked()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_NotEncrypted()
    {
        Guid content = Guid.NewGuid();
        GuidReferenceItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    // =====================
    //  Streamed Content Encrypt/Decrypt
    // =====================
    [Test]
    public async Task EncryptData_StreamedContent_InMemoryRoundTrip()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        Guid expected = item.GetContent()!.Value;

        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetContent()).IsEqualTo(expected);
    }

    [Test]
    public async Task EncryptData_StreamedContent_MissingFile()
    {
        GuidReferenceItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        item.SetFILE(Path.GetTempFileName() + ".missing");

        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_StreamedContent_Locked()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.SetContentStreamed(true,context).ConfigureAwait(false)).IsFalse();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_StreamedContent_NotEncrypted()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
    }

    // =====================
    //  Double Encrypt/Decrypt, Wrong Key, Tamper
    // =====================
    [Test]
    public async Task EncryptData_DoubleEncryption()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        Guid? content = item.GetContent();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    [Test]
    public async Task EncryptData_WrongContext()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"TestKey");
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,"WrongKey");

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(wrongContext).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    // =====================
    //  Signature/Verification
    // =====================
    [Test]
    public async Task SignData_And_VerifyData_ValidSignature()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        GuidReferenceItemTestHelpers.TamperContent(item,Guid.NewGuid());
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_ReplacesExistingFieldAssertion()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context1 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key1");
        DataItemSecurityContext context2 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key2");

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context1).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.SignData("Content",context2).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context2).ConfigureAwait(false),Is.True);
        Assert.That(await item.VerifyData("Content",context1).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_InvalidOrMissingHash()
    {
        GuidReferenceItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullContextOrField()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData(null,context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.SignData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData(null,context).ConfigureAwait(false),Is.False);
        Assert.That(await item.VerifyData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContentRequest_UsesInMemoryContent()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContentRequest_TamperedInMemoryContent()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        GuidReferenceItemTestHelpers.TamperContent(item,Guid.NewGuid());
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_SameField_ReplacesPreviousAssertion()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context1 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key1");
        DataItemSecurityContext context2 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key2");

        Assert.That(await item.EncryptData(context1).ConfigureAwait(false),Is.True);
        Assert.That(await item.SignData("EncryptedData",context2).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("EncryptedData",context2).ConfigureAwait(false),Is.True);
        Assert.That(await item.VerifyData("EncryptedData",context1).ConfigureAwait(false),Is.False);

        Assert.That(await item.DecryptData(context1).ConfigureAwait(false),Is.True);
        Assert.That(await item.SignData("Content",context2).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context2).ConfigureAwait(false),Is.True);
        Assert.That(await item.VerifyData("Content",context1).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_ValidSignature()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        GuidReferenceItemTestHelpers.TamperContent(item,Guid.NewGuid());
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(item.SetID(Guid.NewGuid()),Is.True);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    // =====================
    //  SetContent on Encrypted Item
    // =====================
    [Test]
    public async Task SetContent_Encrypted_Roundtrip()
    {
        GuidReferenceItem item = new();
        Guid initialContent = Guid.NewGuid();
        Guid newContent = Guid.NewGuid();
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(newContent,context)).IsTrue();
        Check.That(item.GetContent()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetContent()).IsEqualTo(newContent);
    }

    [Test]
    public async Task SetContent_Encrypted_EmptyContent()
    {
        GuidReferenceItem item = new();
        Guid content = Guid.NewGuid();
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(Guid.Empty,context)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsFalse();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task SetContent_Encrypted_NoKey()
    {
        GuidReferenceItem item = new();
        Guid initialContent = Guid.NewGuid();
        Guid newContent = Guid.NewGuid();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(newContent,security:null)).IsFalse();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    // =====================
    //  SetNext/SetPrevious/SetUndirectedLinks on Encrypted Item
    // =====================
    [Test]
    public async Task SetNext_Encrypted_Roundtrip()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        ManagementKey? key = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);
        GuidReferenceItem nextItem = new(Guid.NewGuid());

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetNext()).IsNull();
        Check.That(item.SetNext(nextItem,context)).IsTrue();
        Check.That(item.GetNext()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetNext()).IsEqualTo(nextItem);
    }

    [Test]
    public async Task SetNext_Encrypted_NoKey()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        GuidReferenceItem nextItem = new(Guid.NewGuid());

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.SetNext(nextItem,security:null)).IsFalse();
    }

    [Test]
    public async Task SetPrevious_Encrypted_Roundtrip()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        ManagementKey? key = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);
        GuidReferenceItem previousItem = new(Guid.NewGuid());

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetPrevious()).IsNull();
        Check.That(item.SetPrevious(previousItem,context)).IsTrue();
        Check.That(item.GetPrevious()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetPrevious()).IsEqualTo(previousItem);
    }

    [Test]
    public async Task SetPrevious_Encrypted_NoKey()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        GuidReferenceItem previousItem = new(Guid.NewGuid());

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.SetPrevious(previousItem,security:null)).IsFalse();
    }

    [Test]
    public async Task SetUndirectedLinks_Encrypted_Roundtrip()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        ManagementKey? key = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);
        GuidReferenceItem[] links = [new(Guid.NewGuid()), new(Guid.NewGuid())];

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetUndirectedLinks()).IsNull();
        Check.That(item.SetUndirectedLinks(links,context)).IsTrue();
        Check.That(item.GetUndirectedLinks()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        HashSet<GuidReferenceItem>? decryptedLinks = item.GetUndirectedLinks();
        Check.That(decryptedLinks).IsNotNull();
        Check.That(decryptedLinks).HasSize(links.Length);
        Check.That(links.All(l => decryptedLinks!.Any(dl => dl.Equals(l)))).IsTrue();
    }

    [Test]
    public async Task SetUndirectedLinks_Encrypted_NoKey()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        GuidReferenceItem[] links = [new(Guid.NewGuid())];

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.SetUndirectedLinks(links,security:null)).IsFalse();
    }

    // =====================
    //  WipeData
    // =====================
    [Test]
    public async Task WipeData_InMemory_NotEncrypted()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        item.SetNext(new GuidReferenceItem(Guid.NewGuid()));
        item.SetPrevious(new GuidReferenceItem(Guid.NewGuid()));
        item.SetUndirectedLinks([new GuidReferenceItem(Guid.NewGuid())]);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
        Assert.That(item.GetNext(),Is.Null);
        Assert.That(item.GetPrevious(),Is.Null);
        Assert.That(item.GetUndirectedLinks(),Is.Null);
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
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);

        var encryptedDataField = typeof(DataItem).GetField("EncryptedData", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(encryptedDataField,Is.Not.Null);
        Assert.That(encryptedDataField!.GetValue(item),Is.Null);
    }

    [Test]
    public async Task WipeData_StreamedContent()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContentStreamed(),Is.False);
    }

    [Test]
    public async Task WipeData_Locked_InvalidKey()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        ManagementKey? validKey = item.CreateManagementKey("ValidKey");
        ManagementKey invalidKey = new ManagerKey(CreateCertificate(Guid.NewGuid(),"InvalidKey",2048,1,null)!);

        Assert.That(item.Lock(validKey),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,invalidKey)).ConfigureAwait(false),Is.False);
        Assert.That(item.GetContent(),Is.Not.Null);
    }

    [Test]
    public async Task WipeData_Locked_ValidKey()
    {
        GuidReferenceItem item = new(Guid.NewGuid());
        ManagementKey? key = item.CreateManagementKey("TestKey");

        Assert.That(item.Lock(key),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,key!)).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
    }
}
