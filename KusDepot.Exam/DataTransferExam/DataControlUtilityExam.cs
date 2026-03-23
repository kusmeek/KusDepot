using KusDepot.Data;

namespace KusDepot.Exams;

[TestFixture] [Parallelizable]
public class DataControlUtilityExam
{
    [Test]
    public void MakeDataControlUpload()
    {
        Byte[] b = new Byte[660900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlUpload? _u = DataControlUtility.MakeDataControlUpload(_);

        Check.That(_u.Verify()).IsTrue();
    }

    [Test]
    public async Task MakeDataControlUploadAsync()
    {
        Byte[] b = new Byte[660900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlUpload? s = DataControlUtility.MakeDataControlUpload(_);

        DataControlUpload? a = await DataControlUtility.MakeDataControlUploadAsync(_,default);

        CheckEqual(s,a);

        Check.That(await a.VerifyAsync()).IsTrue();
    }

    [Test]
    public void MakeDataControlDownload()
    {
        Byte[] b = new Byte[660900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlDownload? _d = DataControlUtility.MakeDataControlDownload(_.ToString());

        Check.That(_d.Verify()).IsTrue();
    }

    [Test]
    public async Task MakeDataControlDownloadAsync()
    {
        Byte[] b = new Byte[660900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlDownload? s = DataControlUtility.MakeDataControlDownload(_.ToString());

        DataControlDownload? a = await DataControlUtility.MakeDataControlDownloadAsync(_.ToString(),null,default);

        CheckEqual(s,a);

        Check.That(await a.VerifyAsync()).IsTrue();
    }

    [Test]
    public void VerifyUpload()
    {
        Byte[] b = new Byte[483900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlUpload? _u = DataControlUtility.MakeDataControlUpload(_);

        Check.That(_u.Verify()).IsTrue();
    }

    [Test]
    public async Task VerifyUploadAsync()
    {
        Byte[] b = new Byte[483900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlUpload? _u = DataControlUtility.MakeDataControlUpload(_);

        Check.That(_u.Verify()).IsTrue();

        Check.That(await _u.VerifyAsync()).IsTrue();
    }

    [Test]
    public void VerifyDownload()
    {
        Byte[] b = new Byte[483900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlDownload? _d = DataControlUtility.MakeDataControlDownload(_.ToString());

        Check.That(_d.Verify()).IsTrue();
    }

    [Test]
    public async Task VerifyDownloadAsync()
    {
        Byte[] b = new Byte[483900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlDownload? _d = DataControlUtility.MakeDataControlDownload(_.ToString());

        Check.That(_d.Verify()).IsTrue();

        Check.That(await _d.VerifyAsync()).IsTrue();
    }

    private static void CheckEqual(DataControlDownload? a , DataControlDownload? b)
    {
        Check.That(a is not null && b is not null).IsTrue();

        if(a is null || b is null) { return; }

        Check.That(a.Object).IsEqualTo(b.Object);

        Check.That(a.ObjectSHA512.AsSpan().SequenceEqual(b.ObjectSHA512.AsSpan())).IsTrue();

        Check.That(a.StreamSHA512.AsSpan().SequenceEqual(b.StreamSHA512.AsSpan())).IsTrue();
    }

    private static void CheckEqual(DataControlUpload? a , DataControlUpload? b)
    {
        Check.That(a is not null && b is not null).IsTrue();

        if(a is null || b is null) { return; }

        Check.That(a.Object).IsEqualTo(b.Object);

        Check.That(a.ObjectSHA512.AsSpan().SequenceEqual(b.ObjectSHA512.AsSpan())).IsTrue();

        Check.That(a.StreamSHA512.AsSpan().SequenceEqual(b.StreamSHA512.AsSpan())).IsTrue();
    }
}