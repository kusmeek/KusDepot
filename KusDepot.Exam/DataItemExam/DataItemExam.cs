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
        Check.That(item.GetDataEncrypted()).IsFalse();
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
            Check.That(loaded!.GetObjectFILE()).IsEqualTo(item.GetObjectFILE());
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
    public void SetDataType_GetDataType()
    {
        var item = new DataItemTest();
        Check.That(item.SetDataType("BIN")).IsFalse();
    }

    [Test]
    public async Task SetContentStreamed_UpdatesFileSizeCachedBehavior()
    {
        var path1 = Path.GetTempFileName();
        var path2 = Path.GetTempFileName();

        try
        {
            File.WriteAllText(path1,"Alpha",Encoding.Unicode);
            File.WriteAllText(path2,"Beta",Encoding.Unicode);

            var item = new TextItem("Hello",path1);
            var field = typeof(MetaBase).GetField("FileSizeCached",BindingFlags.Instance|BindingFlags.NonPublic);

            Check.That(field).IsNotNull();
            Check.That(field!.GetValue(item)).IsNull();

            Check.That(await item.SetContentStreamed(true,security:null)).IsTrue();
            Check.That(field.GetValue(item)).IsEqualTo(new FileInfo(path1).Length);

            Check.That(item.SetFILE(path1)).IsTrue();
            Check.That(field.GetValue(item)).IsEqualTo(new FileInfo(path1).Length);

            Check.That(item.SetFILE(path2)).IsTrue();
            Check.That(field.GetValue(item)).IsNull();

            Check.That(await item.SetContentStreamed(true,security:null)).IsTrue();
            Check.That(field.GetValue(item)).IsEqualTo(new FileInfo(path2).Length);

            Check.That(await item.SetContentStreamed(false,security:null)).IsTrue();
            Check.That(field.GetValue(item)).IsNull();
        }
        finally
        {
            if(File.Exists(path1)) { File.Delete(path1); }
            if(File.Exists(path2)) { File.Delete(path2); }
        }
    }

    [Test]
    public async Task SetContentStreamed_GetContentStreamed()
    {
        var item = new DataItemTest();
        Check.That(item.GetContentStreamed()).IsFalse();
        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null)).IsFalse();
        Check.That(await item.SetContentStreamed(false,(DataItemSecurityContext?)null)).IsTrue();
    }

    [Test]
    public void Lock_And_UnLock()
    {
        var item = new DataItemTest();
        Check.That(item.GetLocked()).IsFalse();
        Check.That(item.RegisterManager(KeyM)).IsTrue();
        Check.That(item.Lock(KeyM)).IsTrue();
        Check.That(item.GetLocked()).IsTrue();
        Check.That(item.UnLock(KeyM)).IsTrue();
        Check.That(item.GetLocked()).IsFalse();
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
        Check.That(await item.GetDataContent((DataItemSecurityContext?)null)).IsNull();
    }

    [Test]
    public async Task Null_And_Invalid_Inputs()
    {
        var item = new DataItemTest();
        Check.That(item.SetDataType(null)).IsFalse();
        Check.That(item.SetDataType(String.Empty)).IsFalse();
        Check.That(await item.SetContentStreamed(false,(DataItemSecurityContext?)null)).IsTrue();
        Check.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null)).IsFalse();
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
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,KeyM!);
        Check.That(await item.VerifyData("field", context)).IsFalse();
        Check.That(await item.VerifyData(null, context)).IsFalse();
        Check.That(await item.VerifyData("field", (DataItemSecurityContext?)null)).IsFalse();
    }

    [Test]
    public async Task SignData_ReturnsNull()
    {
        var item = new DataItemTest();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item,KeyM!);
        Check.That(await item.SignData("field", context)).IsNull();
        Check.That(await item.SignData(null, context)).IsNull();
        Check.That(await item.SignData("field", (DataItemSecurityContext?)null)).IsNull();
    }
}