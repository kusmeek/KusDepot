namespace KusDepot.DataPodExams;

[TestFixture]
public sealed class CatalogPagingExam
{
    [Test]
    public void PageSizeUsesDefaultWhenOmitted()
    {
        Assert.That(DataPodServices.CatalogDB.CatalogPageSize.Resolve(null), Is.EqualTo(50));
    }

    [TestCase(1)]
    [TestCase(100)]
    public void PageSizeAcceptsInclusiveBounds(Int32 pageSize)
    {
        Assert.That(DataPodServices.CatalogDB.CatalogPageSize.Resolve(pageSize), Is.EqualTo(pageSize));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(101)]
    public void PageSizeRejectsOutOfRangeValues(Int32 pageSize)
    {
        Assert.Throws<DataPodServices.CatalogDB.CatalogPageRequestException>(() =>
            DataPodServices.CatalogDB.CatalogPageSize.Resolve(pageSize));
    }

    [Test]
    public void PageTokenRoundTripsGuidKey()
    {
        Guid key = Guid.NewGuid();

        String token = DataPodServices.CatalogDB.CatalogPageTokenCodec.Encode(key);

        Assert.That(token, Has.Length.EqualTo(23));
        Assert.That(token, Does.Not.Contain("="));
        Assert.That(DataPodServices.CatalogDB.CatalogPageTokenCodec.Decode(token), Is.EqualTo(key));
    }

    [TestCase("")]
    [TestCase("invalid")]
    [TestCase("A")]
    [TestCase("!!!!!!!!!!!!!!!!!!!!!!!")]
    public void PageTokenRejectsMalformedValues(String token)
    {
        Assert.Throws<DataPodServices.CatalogDB.CatalogPageRequestException>(() =>
            DataPodServices.CatalogDB.CatalogPageTokenCodec.Decode(token));
    }

    [Test]
    public void PageTokenRejectsUnsupportedVersion()
    {
        Byte[] payload = new Byte[17];
        payload[0] = 2;
        Guid key = Guid.NewGuid();
        key.TryWriteBytes(payload.AsSpan(1), bigEndian: true, out _);
        String token = Convert.ToBase64String(payload).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        Assert.Throws<DataPodServices.CatalogDB.CatalogPageRequestException>(() =>
            DataPodServices.CatalogDB.CatalogPageTokenCodec.Decode(token));
    }
}
