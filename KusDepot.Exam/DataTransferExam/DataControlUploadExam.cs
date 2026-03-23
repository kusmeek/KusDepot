using KusDepot.Data;

namespace KusDepot.Exams;

[TestFixture] [Parallelizable]
public class DataControlUploadExam
{
    [Test]
    public void ToString_And_Parse_RoundTrip()
    {
        Byte[] b = new Byte[4096]; RandomNumberGenerator.Create().GetBytes(b);

        DataItem item = new BinaryItem(b);

        String o = item.ToString();

        Descriptor d = item.GetDescriptor()!;

        DataControlUpload original = new() { Descriptor = d , Object = o , ObjectSHA512 = "objecthash" , StreamSHA512 = "streamhash" };

        String json = original.ToString();

        DataControlUpload? parsed = DataControlUpload.Parse(json,null);

        CheckEqual(original,parsed);
    }

    [Test]
    public async Task ToStringAsync_And_ParseAsync_RoundTrip()
    {
        Byte[] b = new Byte[4096]; RandomNumberGenerator.Create().GetBytes(b);

        DataItem item = new BinaryItem(b);

        String o = item.ToString();

        Descriptor d = item.GetDescriptor()!;

        DataControlUpload original = new() { Descriptor = d , Object = o , ObjectSHA512 = "objecthash" , StreamSHA512 = "streamhash" };

        String json = await original.ToStringAsync(default).ConfigureAwait(false);

        DataControlUpload? parsed = await DataControlUpload.ParseAsync(json,null,default).ConfigureAwait(false);

        DataControlUpload? parsed2 = DataControlUpload.Parse(json,null);

        CheckEqual(original,parsed); CheckEqual(original,parsed2);
    }

    [Test]
    public void TryParse_Success_And_Failure()
    {
        Byte[] b = new Byte[4096]; RandomNumberGenerator.Create().GetBytes(b);

        DataItem item = new BinaryItem(b);

        String o = item.ToString();

        Descriptor d = item.GetDescriptor()!;

        DataControlUpload original = new() { Descriptor = d , Object = o , ObjectSHA512 = "objecthash" , StreamSHA512 = "streamhash" };

        String json = original.ToString();

        Boolean r1 = DataControlUpload.TryParse(json,null,out DataControlUpload? parsed);

        Check.That(r1).IsTrue(); CheckEqual(original,parsed);

        Boolean r2 = DataControlUpload.TryParse(null,null,out DataControlUpload? _);

        Check.That(r2).IsFalse();

        Boolean r3 = DataControlUpload.TryParse("not json",null,out DataControlUpload? _invalid);

        Check.That(r3).IsFalse();
    }

    private static void CheckEqual(DataControlUpload? a , DataControlUpload? b)
    {
        Check.That(a is not null && b is not null).IsTrue();

        if(a is null || b is null) { return; }

        Check.That(a.Object).IsEqualTo(b.Object);

        Check.That(a.ObjectSHA512.AsSpan().SequenceEqual(b.ObjectSHA512.AsSpan())).IsTrue();

        Check.That(a.StreamSHA512.AsSpan().SequenceEqual(b.StreamSHA512.AsSpan())).IsTrue();

        Check.That(a.Descriptor.ID).IsEqualTo(b.Descriptor.ID);
    }
}