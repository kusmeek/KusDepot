namespace KusDepot.Exams.DataItems;

// =====================
//  MultiMediaItem-specific Helpers
// =====================
public static class MultiMediaItemTestHelpers
{
    public static void TamperContent(MultiMediaItem item,Byte[] newValue)
    {
        var field = typeof(MultiMediaItem).GetField("Content", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Content field not found");
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
public class MultiMediaItemSecurityContextExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryContent_Valid()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task CheckDataHash_InMemoryContent_Tampered()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);

        Check.That(item.SetContent(content)).IsTrue();
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        MultiMediaItemTestHelpers.TamperContent(item,tampered);
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Valid()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        Check.That(item.SetContent(Array.Empty<Byte>())).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_EncryptedAndDecrypted_Valid()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_InvalidAssertion()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        MultiMediaItemTestHelpers.TamperContent(item,tampered);
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task VerifyData_Content_NoContextProvided()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
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
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_EncryptedData_Tampered()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.ValidateData(context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task ProtectData_UnencryptedContent_EncryptsAndValidates()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new(CreateRandomBytes(32));
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(Array.Empty<Byte>())).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Locked()
    {
        MultiMediaItem item = new(CreateRandomBytes(32));
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_NotEncrypted()
    {
        Byte[] content = CreateRandomBytes(32);
        MultiMediaItem item = new(content);
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String file = Path.GetTempFileName() + ".missing";

        item.SetFILE(file);
        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false)).IsFalse();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_StreamedContent_Locked()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new(CreateRandomBytes(16));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        Byte[] content = CreateRandomBytes(16);
        MultiMediaItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo(content);
    }

    [Test]
    public async Task EncryptData_WrongContext()
    {
        MultiMediaItem item = new(CreateRandomBytes(16));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"TestKey");
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,"WrongKey");

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(wrongContext).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        MultiMediaItem item = new(CreateRandomBytes(16));
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        MultiMediaItemTestHelpers.TamperContent(item,tampered);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_ReplacesExistingFieldAssertion()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullContextOrField()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData(null,context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.SignData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData(null,context).ConfigureAwait(false),Is.False);
        Assert.That(await item.VerifyData("Content",(DataItemSecurityContext?)null).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_StreamedContent_ValidSignature()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new();
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
    public async Task SignData_And_VerifyData_DifferentFields_CanUseDifferentContexts()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(32);
        DataItemSecurityContext context1 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key1");
        DataItemSecurityContext context2 = DataItemSecurityContextExamHelpers.CreateContext(item,"Key2");

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context1).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.SignData("HashCode",context2).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context1).ConfigureAwait(false),Is.True);
        Assert.That(await item.VerifyData("HashCode",context2).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_ValidSignature()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        MultiMediaItem item = new();
        Byte[] content = CreateRandomBytes(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Byte[] tampered = (Byte[])content.Clone();
        tampered[0] ^= 0xFF;
        MultiMediaItemTestHelpers.TamperContent(item,tampered);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"FileRoundtripKey");
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            MultiMediaItem? loadedItem = MultiMediaItem.FromFile(file);
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
        MultiMediaItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"FileRoundtripKey");
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            MultiMediaItem? loadedItem = MultiMediaItem.FromFile(file);
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
        MultiMediaItem item = new(CreateRandomBytes(32));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);

        FieldInfo? secureHashesField = typeof(DataItem).GetField("FieldIntegrityHashes", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureHashesField,Is.Not.Null);
        Assert.That(secureHashesField!.GetValue(item),Is.Null);

        FieldInfo? secureSignaturesField = typeof(DataItem).GetField("FieldAssertions", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(secureSignaturesField,Is.Not.Null);
        Assert.That(secureSignaturesField!.GetValue(item),Is.Null);
    }

    [Test]
    public async Task WipeData_InMemory_Encrypted()
    {
        MultiMediaItem item = new(CreateRandomBytes(32));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
        Assert.That(item.GetDataEncrypted(),Is.False);
        Assert.That(item.GetDataProtectionInfo(),Is.Null);

        FieldInfo? encryptedDataField = typeof(DataItem).GetField("EncryptedData", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(encryptedDataField,Is.Not.Null);
        Assert.That(encryptedDataField!.GetValue(item),Is.Null);
    }

    [Test]
    public async Task WipeData_StreamedContent()
    {
        MultiMediaItem item = new();
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
        MultiMediaItem item = new(content);
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
        MultiMediaItem item = new(content);
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
