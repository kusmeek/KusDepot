namespace KusDepot.Exams.DataItems;

// =====================
//  BinaryItem-specific Helpers
// =====================
public static class BinaryItemTestHelpers
{
    public static void TamperContent(BinaryItem item,Byte[] newValue)
    {
        var field = typeof(BinaryItem).GetField("Content", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Content field not found");
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
public class BinaryItemSecurityContextExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryContent_Valid()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task CheckDataHash_InMemoryContent_Tampered()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);

        Check.That(item.SetContent(content)).IsTrue();
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        BinaryItemTestHelpers.TamperContent(item,tampered);
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Valid()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);

        try
        {
            Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Tampered()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);

        try
        {
            DataItemSecurityTestHelpers.TamperFile(file);
            Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public static async Task CheckDataHash_EmptyContent()
    {
        BinaryItem item = new();
        Check.That(item.SetContent(Array.Empty<Byte>())).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_EncryptedAndDecrypted_Valid()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
            Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
            Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
            Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
        }
        finally { File.Delete(file); }
    }

    // =====================
    //  Signature-Based Data Integrity
    // =====================
    [Test]
    public async Task VerifyData_Content_ValidAssertion()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_ValidAssertion_WithDistinctSigningIdentity()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        ManagementKey owner = item.CreateManagementKey("Owner") ?? throw new InvalidOperationException("Unable to create owner key.");
        ManagementKey signer = item.CreateManagementKey("Signer") ?? throw new InvalidOperationException("Unable to create signer key.");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,owner,signer,displayName:"Owner");

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_InvalidAssertion()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        BinaryItemTestHelpers.TamperContent(item,tampered);
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task VerifyData_Content_NoContextProvided()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content").ConfigureAwait(false)).IsFalse();
    }

    // =====================
    //  Encrypted Data Signature
    // =====================
    [Test]
    public async Task VerifyData_EncryptedData_ValidAssertion()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_EncryptedData_ValidAssertion_WithDistinctSigningIdentity()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        ManagementKey owner = item.CreateManagementKey("Owner") ?? throw new InvalidOperationException("Unable to create owner key.");
        ManagementKey signer = item.CreateManagementKey("Signer") ?? throw new InvalidOperationException("Unable to create signer key.");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,owner,signer,displayName:"Owner");

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetDataProtectionInfo()!.ProtectedByObjectId).IsEqualTo(context.LocalObject.ObjectId);
        Check.That(item.GetDataProtectionInfo()!.Assertions[0].IssuerObjectId).IsEqualTo(context.SigningObject.ObjectId);
    }

    [Test]
    public async Task VerifyData_EncryptedData_Tampered()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task VerifyData_EncryptedData_StreamedContent_ValidAssertion()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
            Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task VerifyData_EncryptedData_StreamedContent_Tampered()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
            DataItemSecurityTestHelpers.TamperFile(file);
            Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsFalse();
        }
        finally { File.Delete(file); }
    }

    // =====================
    //  ProtectData/UnProtectData Validation
    // =====================
    [Test]
    public async Task ValidateData_Content_UsesCurrentState()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.ValidateData(context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task ProtectData_UnencryptedContent_EncryptsAndValidates()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
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
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.ProtectData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task UnProtectData_EncryptedContent_DecryptsAndValidates()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
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
        BinaryItem item = new(CreateRandomBytes(32));
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
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
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
        BinaryItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(Array.Empty<Byte>())).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Locked()
    {
        BinaryItem item = new(CreateRandomBytes(32));
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_NotEncrypted()
    {
        Byte[] content = CreateRandomBytes(32);
        BinaryItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    // =====================
    //  Streamed Content Encrypt/Decrypt
    // =====================
    [Test]
    public async Task EncryptData_StreamedContent_RoundTrip()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
            Check.That(item.GetDataProtectionInfo()).IsNotNull();
            Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
            Check.That(item.GetDataProtectionInfo()).IsNull();
            Check.That(File.ReadAllBytes(file)).IsEqualTo(data);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptData_StreamedContent_MissingFile()
    {
        BinaryItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String file = Path.GetTempFileName() + ".missing";

        item.SetFILE(file);
        Check.That(await item.SetContentStreamed(true).ConfigureAwait(false)).IsFalse();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_StreamedContent_Locked()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        try
        {
            Check.That(item.Lock(key)).IsTrue();
            Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
            Check.That(item.GetDataProtectionInfo()).IsNull();
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptData_StreamedContent_NotEncrypted()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
            Check.That(File.ReadAllBytes(file)).IsEqualTo(data);
        }
        finally { File.Delete(file); }
    }

    // =====================
    //  Double Encrypt/Decrypt, Wrong Key, Tamper
    // =====================
    [Test]
    public async Task EncryptData_DoubleEncryption()
    {
        BinaryItem item = new(CreateRandomBytes(16));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        Byte[] content = CreateRandomBytes(16);
        BinaryItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    [Test]
    public async Task EncryptData_WrongContext()
    {
        BinaryItem item = new(CreateRandomBytes(16));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"TestKey");
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,"WrongKey");

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(wrongContext).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        BinaryItem item = new(CreateRandomBytes(16));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    // =====================
    //  SetContent on Encrypted Item
    // =====================
    [Test]
    public async Task SetContent_Encrypted_Roundtrip()
    {
        BinaryItem item = new();
        Byte[] initialContent = CreateRandomBytes(128);
        Byte[] newContent = CreateRandomBytes(256);
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
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(128);
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(Array.Empty<Byte>(),context)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsFalse();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task SetContent_Encrypted_NoKey()
    {
        BinaryItem item = new();
        Byte[] initialContent = CreateRandomBytes(128);
        Byte[] newContent = CreateRandomBytes(256);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(newContent,security:null)).IsFalse();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    // =====================
    //  Signature/Verification
    // =====================
    [Test]
    public async Task SignData_And_VerifyData_ValidSignature()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        BinaryItemTestHelpers.TamperContent(item,tampered);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_ReplacesExistingFieldAssertion()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
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
        BinaryItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullContextOrField()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData(null,context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.SignData("Content").ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData(null,context).ConfigureAwait(false),Is.False);
        Assert.That(await item.VerifyData("Content").ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContent_ValidSignature()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContent_Tampered()
    {
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(1024);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
            DataItemSecurityTestHelpers.TamperFile(file);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task SignData_And_VerifyData_SameField_ReplacesPreviousAssertion()
    {
        BinaryItem item = new(CreateRandomBytes(32));
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
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        BinaryItemTestHelpers.TamperContent(item,tampered);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        BinaryItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(item.SetID(Guid.NewGuid()),Is.True);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    // =====================
    //  File Roundtrip
    // =====================
    [Test]
    public async Task ToFile_FromFile_Roundtrip_Unencrypted()
    {
        Byte[] content = CreateRandomBytes(128);
        BinaryItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"FileRoundtripKey");
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            BinaryItem? loadedItem = BinaryItem.FromFile(file);
            Assert.That(loadedItem,Is.Not.Null);
            Assert.That(await loadedItem!.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(loadedItem.GetContent(),Is.EqualTo(content));
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
        Byte[] content = CreateRandomBytes(128);
        BinaryItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"FileRoundtripKey");
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            BinaryItem? loadedItem = BinaryItem.FromFile(file);
            Assert.That(loadedItem,Is.Not.Null);
            Assert.That(loadedItem!.GetDataEncrypted(),Is.True);
            Assert.That(loadedItem.GetContent(),Is.Null);
            Assert.That(loadedItem.GetDataProtectionInfo(),Is.Not.Null);
            Assert.That(await loadedItem.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(await loadedItem.DecryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(loadedItem.GetContent(),Is.EqualTo(content));
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
        BinaryItem item = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin"))!;
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        Byte[] content = item.GetContent()!;

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);

        var secureHashesField = typeof(DataItem).GetField("FieldIntegrityHashes", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureHashesField,Is.Not.Null);
        Assert.That(secureHashesField!.GetValue(item),Is.Null);

        var secureSignaturesField = typeof(DataItem).GetField("FieldAssertions", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureSignaturesField,Is.Not.Null);
        Assert.That(secureSignaturesField!.GetValue(item),Is.Null);

        var commandsField = typeof(BinaryItem).GetField("Commands", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(commandsField,Is.Not.Null);
        Assert.That(commandsField!.GetValue(item),Is.Null);

        var servicesField = typeof(BinaryItem).GetField("Services", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(servicesField,Is.Not.Null);
        Assert.That(servicesField!.GetValue(item),Is.Null);
    }

    [Test]
    public async Task WipeData_InMemory_Encrypted()
    {
        BinaryItem item = new(CreateRandomBytes(32));
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
        BinaryItem item = new();
        Byte[] data = CreateRandomBytes(128);
        String file = await DataItemSecurityTestHelpers.SetupStreamedContent(item,data).ConfigureAwait(false);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
            Assert.That(File.Exists(file),Is.False);
            Assert.That(item.GetFILE(),Is.Null);
            Assert.That(item.GetContentStreamed(),Is.False);
        }
        finally
        {
            if(File.Exists(file)) { File.Delete(file); }
        }
    }

    [Test]
    public async Task WipeData_Locked_InvalidKey()
    {
        Byte[] content = CreateRandomBytes(32);
        BinaryItem item = new(content);
        ManagementKey? validKey = item.CreateManagementKey("ValidKey");
        ManagementKey invalidKey = new ManagerKey(CreateCertificate(Guid.NewGuid(),"InvalidKey",2048,1,null)!);

        Assert.That(item.Lock(validKey),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,invalidKey)).ConfigureAwait(false),Is.False);
        Assert.That(item.GetContent(),Is.EqualTo(content));
    }

    [Test]
    public async Task WipeData_Locked_ValidKey()
    {
        Byte[] content = CreateRandomBytes(32);
        BinaryItem item = new(content);
        ManagementKey? key = item.CreateManagementKey("TestKey");

        Assert.That(item.Lock(key),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,key!)).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
    }

    private static Byte[] CreateRandomBytes(Int32 length)
    {
        Byte[] data = new Byte[length];
        RandomNumberGenerator.Create().GetBytes(data);
        return data;
    }
}
