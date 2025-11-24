namespace KusDepot.FabricExams.Data;

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
    public void MakeDataControlDownload()
    {
        Byte[] b = new Byte[660900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlDownload? _d = DataControlUtility.MakeDataControlDownload(_.ToString());

        Check.That(_d.Verify()).IsTrue();
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
    public void VerifyDownload()
    {
        Byte[] b = new Byte[483900];

        RandomNumberGenerator.Create().GetBytes(b);

        var _ = new DataSetItem(content:new HashSet<DataItem>(){new BinaryItem(b)});

        DataControlDownload? _d = DataControlUtility.MakeDataControlDownload(_.ToString());

        Check.That(_d.Verify()).IsTrue();
    }
}