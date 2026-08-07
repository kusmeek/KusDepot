namespace KusDepot.Exams.DataItems;

// =====================
//  MSBuildItem-specific Helpers
// =====================
public static class MSBuildItemTestHelpers
{
    public static void TamperContent(MSBuildItem item,String newValue)
    {
        var field = typeof(MSBuildItem).GetField("Content", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Content field not found");
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
public class MSBuildItemSecurityContextExam
{
    // =====================
    //  Hash-Only Data Integrity
    // =====================
    [Test]
    public async Task CheckDataHash_InMemoryContent_Valid()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task CheckDataHash_InMemoryContent_Tampered()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);

        Check.That(item.SetContent(content)).IsTrue();
        MSBuildItemTestHelpers.TamperContent(item,content + "tampered");
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_Valid()
    {
        MSBuildItem item = new();
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
        MSBuildItem item = new();
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
        MSBuildItem item = new();
        Check.That(item.SetContent(String.Empty)).IsTrue();
        Check.That(await item.CheckDataHash().ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_StreamedContent_EncryptedAndDecrypted_Valid()
    {
        MSBuildItem item = new();
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
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_Content_InvalidAssertion()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        MSBuildItemTestHelpers.TamperContent(item,content + "tampered");
        Check.That(await item.VerifyData("Content",context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task VerifyData_Content_NoContextProvided()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task VerifyData_EncryptedData_Tampered()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.SignData("Content",context).ConfigureAwait(false)).IsNotNull();
        Check.That(await item.ValidateData(context).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task ProtectData_UnencryptedContent_EncryptsAndValidates()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        DataItemSecurityTestHelpers.TamperEncryptedData(item);
        Check.That(await item.ProtectData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task UnProtectData_EncryptedContent_DecryptsAndValidates()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new("test content");
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
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(String.Empty)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_Locked()
    {
        MSBuildItem item = new("locked content");
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.Lock(key)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetDataProtectionInfo()).IsNull();
    }

    [Test]
    public async Task EncryptData_InMemoryContent_NotEncrypted()
    {
        MSBuildItem item = new("plain content");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo("plain content");
    }

    // =====================
    //  Streamed Content Encrypt/Decrypt
    // =====================
    [Test]
    public async Task EncryptData_StreamedContent_RoundTrip()
    {
        MSBuildItem item = new();
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
        MSBuildItem item = new();
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
        MSBuildItem item = new();
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
        MSBuildItem item = new();
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
        MSBuildItem item = new("double encrypt");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsFalse();
    }

    [Test]
    public async Task EncryptData_DoubleDecryption()
    {
        MSBuildItem item = new("double decrypt");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsEqualTo("double decrypt");
    }

    [Test]
    public async Task EncryptData_WrongContext()
    {
        MSBuildItem item = new("wrong key");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"TestKey");
        DataItemSecurityContext wrongContext = DataItemSecurityContextExamHelpers.CreateContext(item,"WrongKey");

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.DecryptData(wrongContext).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task EncryptData_TamperedEncryptedData()
    {
        MSBuildItem item = new("tampered");
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
        MSBuildItem item = new();
        String initialContent = "public class Initial {}";
        String newContent = "public class New {}";
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
        MSBuildItem item = new();
        String content = "public class ToBeCleared {}";
        (ManagementKey key, DataItemSecurityContext context) = DataItemSecurityContextExamHelpers.CreateContextPair(item);

        Check.That(item.SetContent(content)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(String.Empty,context)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsFalse();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsFalse();
        Check.That(item.GetContent()).IsNull();
    }

    [Test]
    public async Task SetContent_Encrypted_NoKey()
    {
        MSBuildItem item = new();
        String initialContent = "public class InitialNoKey {}";
        String newContent = "public class NewNoKey {}";
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Check.That(item.SetContent(initialContent)).IsTrue();
        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(item.SetContent(newContent,security:null)).IsFalse();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();
    }

    // =====================
    //  SetRequirements/SetSequence on Encrypted Item
    // =====================
    [Test]
    public async Task SetRequirements_Encrypted_Roundtrip()
    {
        MSBuildItem item = new("Initial Content");
        ManagementKey? key = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);
        HashSet<DataItem> requirements = [new TextItem("Requirement 1")];

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetRequirements()).IsNull();
        Check.That(item.SetRequirements(requirements,context)).IsTrue();
        Check.That(item.GetRequirements()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetRequirements()).IsEquivalentTo(requirements);
    }

    [Test]
    public async Task SetRequirements_Encrypted_NoKey()
    {
        MSBuildItem item = new("Initial Content");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        HashSet<DataItem> requirements = [new TextItem("Requirement 1")];

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.SetRequirements(requirements,security:null)).IsFalse();
    }

    [Test]
    public async Task SetSequence_Encrypted_Roundtrip()
    {
        MSBuildItem item = new("Initial Content");
        ManagementKey? key = item.CreateManagementKey("TestKey");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,key!);
        SortedList<Int32,MSBuildItem> sequence = new() { { 0, new MSBuildItem("Sequence 1") } };

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetSequence()).IsNull();
        Check.That(item.SetSequence(sequence,context)).IsTrue();
        Check.That(item.GetSequence()).IsNull();
        Check.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false)).IsTrue();

        Check.That(await item.DecryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.GetSequence()!.Values.First().GetContent()).IsEqualTo(sequence.Values.First().GetContent());
    }

    [Test]
    public async Task SetSequence_Encrypted_NoKey()
    {
        MSBuildItem item = new("Initial Content");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        SortedList<Int32,MSBuildItem> sequence = new() { { 0, new MSBuildItem("Sequence 1") } };

        Check.That(await item.EncryptData(context).ConfigureAwait(false)).IsTrue();
        Check.That(item.SetSequence(sequence,security:null)).IsFalse();
    }

    // =====================
    //  Signature/Verification
    // =====================
    [Test]
    public async Task SignData_And_VerifyData_ValidSignature()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_TamperedContent()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        MSBuildItemTestHelpers.TamperContent(item,content + "tampered");
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_ReplacesExistingFieldAssertion()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_NullContextOrField()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        MSBuildItem item = new();
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
        MSBuildItem item = new();
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
        MSBuildItem item = new("test content");
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
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_TamperedContent()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetContent(content),Is.True);
        Assert.That(await item.SignData("HashCode",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        MSBuildItemTestHelpers.TamperContent(item,content + "tampered");
        Assert.That(await item.VerifyData("HashCode",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_HashCode_AlteredID()
    {
        MSBuildItem item = new();
        String content = TestCaseDataGenerator.GenerateUnicodeString(1024);
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
        String content = "<Project Sdk=\"Microsoft.NET.Sdk\"></Project>";
        MSBuildItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"FileRoundtripKey");
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            MSBuildItem? loadedItem = MSBuildItem.FromFile(file);
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
        String content = "<Project Sdk=\"Microsoft.NET.Sdk\"></Project>";
        MSBuildItem item = new(content);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,"FileRoundtripKey");
        String file = Path.GetTempFileName() + ".kdi";

        try
        {
            Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(item.ToFile(file),Is.True);

            MSBuildItem? loadedItem = MSBuildItem.FromFile(file);
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
        TextItem requirementItem = new("requirement");
        MSBuildItem sequenceItem = new("sequence step");
        MSBuildItem item = new("content");
        item.SetRequirements([requirementItem]);
        item.SetSequence(new Dictionary<Int32,MSBuildItem> { { 1, sequenceItem } });
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null);
        Assert.That(await item.WipeData(context).ConfigureAwait(false),Is.True);
        Assert.That(item.GetContent(),Is.Null);
        Assert.That(item.GetRequirements(),Is.Null);
        Assert.That(item.GetSequence(),Is.Null);

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
        MSBuildItem item = new("some content");
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
        MSBuildItem item = new();
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
        MSBuildItem item = new("some content");
        ManagementKey? validKey = item.CreateManagementKey("ValidKey");
        ManagementKey invalidKey = new ManagerKey(CreateCertificate(Guid.NewGuid(),"InvalidKey",2048,1,null)!);

        Assert.That(item.Lock(validKey),Is.True);
        Assert.That(await item.WipeData(DataItemSecurityContextExamHelpers.CreateContext(item,invalidKey)).ConfigureAwait(false),Is.False);
        Assert.That(item.GetContent(),Is.Not.Null);
    }

    [Test]
    public async Task WipeData_Locked_ValidKey()
    {
        MSBuildItem item = new("some content");
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
