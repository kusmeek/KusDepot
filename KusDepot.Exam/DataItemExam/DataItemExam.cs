namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class DataItemExam
{
    private ManagerKey? KeyM;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }
        KeyM = new( Utility.SerializeCertificate(new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );
        if(KeyM is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Constructor_InitializesFields()
    {
        var item = new DataItemTest();
        Check.That(item.GetID()).IsNotNull();
        Check.That(item.GetLocked()).IsFalse();
        Check.That(item.GetDataMasked()).IsFalse();
        Check.That(item.GetDataEncrypted()).IsFalse();
        Check.That(item.GetDataEncryptInfo()).IsNull();
        Check.That(item.GetContentStreamed()).IsFalse();
    }

    [Test]
    public void Clone_Roundtrip()
    {
        var item = new DataItemTest();
        var clone = item.Clone();
        Check.That(clone).IsNotNull();
        Check.That(clone).IsInstanceOf<DataItemTest>();
        Check.That(clone!.GetID()).IsEqualTo(item.GetID());
        Check.That(clone.GetType()).IsEqualTo(item.GetType());
    }

    [Test]
    public void ToString_Parse_Roundtrip()
    {
        var item = new DataItemTest();
        var str = item.ToString();
        var parsed = DataItemTest.Parse<DataItemTest>(str,null);
        Check.That(parsed).IsNotNull();
        Check.That(parsed!.GetID()).IsEqualTo(item.GetID());
    }

    [Test]
    public void ToFile_FromFile_Roundtrip()
    {
        var item = new DataItemTest();
        var path = Path.GetTempFileName()+".kdi";
        try {
            File.Delete(path);
            Check.That(item.ToFile(path)).IsTrue();
            var loaded = DataItemTest.FromFile<DataItemTest>(path);
            Check.That(loaded).IsNotNull();
            Check.That(loaded!.GetID()).IsEqualTo(item.GetID());
        } finally { if(File.Exists(path)) File.Delete(path); }
    }

    [Test]
    public void CompareTo_And_Equality()
    {
        var a = new DataItemTest();
        var b = new DataItemTest();
        Check.That(a.CompareTo(a)).IsEqualTo(0);
        Check.That(a.Equals(a)).IsTrue();
        Check.That(a.Equals((Object)a)).IsTrue();
        Check.That(a.Equals((Object?)null)).IsFalse();
        Check.That(a!.Equals(b)).IsFalse();
        Check.That(a.GetHashCode()).IsNotZero();
        Check.That(a.CompareTo(b)).IsNotEqualTo(0);
    }

    [Test]
    public void SetType_GetType()
    {
        var item = new DataItemTest();
        Check.That(item.SetType("BIN")).IsFalse();
    }

    [Test]
    public void SetContentStreamed_GetContentStreamed()
    {
        var item = new DataItemTest();
        Check.That(item.GetContentStreamed()).IsFalse();
        Check.That(item.SetContentStreamed(true)).IsFalse();
        Check.That(item.SetContentStreamed(false)).IsTrue();
    }

    [Test]
    public void Lock_And_UnLock()
    {
        var item = new DataItemTest();
        Check.That(item.GetLocked()).IsFalse();
        Check.That(item.Lock(KeyM)).IsTrue();
        Check.That(item.GetLocked()).IsTrue();
        Check.That(item.UnLock(KeyM)).IsTrue();
        Check.That(item.GetLocked()).IsFalse();
    }

    [Test]
    public void MaskData_NoSecrets()
    {
        var item = new DataItemTest();
        Check.That(item.MaskData(true,KeyM)).IsFalse();
        Check.That(item.GetDataMasked()).IsFalse();
    }

    [Test]
    public async Task EncryptData_DefaultIsFalse()
    {
        var item = new DataItemTest();
        var result = await item.EncryptData();
        Check.That(result).IsFalse();
    }

    [Test]
    public async Task GetDataContent_ReturnsNull()
    {
        var item = new DataItemTest();
        Check.That(await item.GetDataContent()).IsNull();
    }

    [Test]
    public void Null_And_Invalid_Inputs()
    {
        var item = new DataItemTest();
        Check.That(item.SetType(null)).IsFalse();
        Check.That(item.SetType(String.Empty)).IsFalse();
        Check.That(item.SetContentStreamed(false)).IsTrue();
        Check.That(item.SetContentStreamed(true)).IsFalse();
    }

    [Test]
    public async Task CheckDataHash_ReturnsFalse()
    {
        var item = new DataItemTest();
        Check.That(await item.CheckDataHash()).IsFalse();
    }

    [Test]
    public async Task VerifyData_ReturnsFalse()
    {
        var item = new DataItemTest();
        Check.That(await item.VerifyData("field", KeyM)).IsFalse();
        Check.That(await item.VerifyData(null, KeyM)).IsFalse();
        Check.That(await item.VerifyData("field", null)).IsFalse();
    }

    [Test]
    public void SignData_ReturnsNull()
    {
        var item = new DataItemTest();
        Check.That(item.SignData("field", KeyM)).IsNull();
        Check.That(item.SignData(null, KeyM)).IsNull();
        Check.That(item.SignData("field", null)).IsNull();
    }
}