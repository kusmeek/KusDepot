using KusDepot.Data;

namespace KusDepot.Exams;

[TestFixture] [Parallelizable]
public class DataControlDownloadExam
{
    [Test]
    public void ToString_And_Parse_RoundTrip()
    {
        Byte[] b = new Byte[4096]; RandomNumberGenerator.Create().GetBytes(b);

        DataItem item = new BinaryItem(b);

        String o = item.ToString();

        DataControlDownload original = new() { Object = o , ObjectSHA512 = "objecthash" , StreamSHA512 = "streamhash" };

        String json = original.ToString();

        DataControlDownload? parsed = DataControlDownload.Parse(json,null);

        CheckEqual(original,parsed);
    }

    [Test]
    public async Task ToStringAsync_And_ParseAsync_RoundTrip()
    {
        Byte[] b = new Byte[4096]; RandomNumberGenerator.Create().GetBytes(b);

        DataItem item = new BinaryItem(b);

        String o = item.ToString();

        DataControlDownload original = new() { Object = o , ObjectSHA512 = "objecthash" , StreamSHA512 = "streamhash" };

        String json = await original.ToStringAsync(default).ConfigureAwait(false);

        DataControlDownload? parsed = await DataControlDownload.ParseAsync(json,null,default).ConfigureAwait(false);

        DataControlDownload? parsed2 = DataControlDownload.Parse(json,null);

        CheckEqual(original,parsed); CheckEqual(original,parsed2);
    }

    private static void CheckEqual(DataControlDownload? a , DataControlDownload? b)
    {
        Check.That(a is not null && b is not null).IsTrue();

        if(a is null || b is null) { return; }

        Check.That(a.Object).IsEqualTo(b.Object);

        Check.That(a.ObjectSHA512.AsSpan().SequenceEqual(b.ObjectSHA512.AsSpan())).IsTrue();

        Check.That(a.StreamSHA512.AsSpan().SequenceEqual(b.StreamSHA512.AsSpan())).IsTrue();
    }
}