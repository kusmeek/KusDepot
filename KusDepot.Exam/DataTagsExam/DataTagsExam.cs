namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class DataTagsExam
{
    [Test]
    public void Architecture_AllTags()
    {
        var tags = DataTags.Architecture.AllTags;
        Check.That(tags.Count).IsEqualTo(5);
    }

    [Test]
    public void Browser_AllTags()
    {
        var tags = DataTags.Browser.AllTags;
        Check.That(tags.Count).IsEqualTo(7);
    }

    [Test]
    public void Database_AllTags()
    {
        var tags = DataTags.Database.AllTags;
        Check.That(tags.Count).IsEqualTo(11);
    }

    [Test]
    public void Deployment_AllTags()
    {
        var tags = DataTags.Deployment.AllTags;
        Check.That(tags.Count).IsEqualTo(13);
    }

    [Test]
    public void FrameworkVersion_AllTags()
    {
        var tags = DataTags.FrameworkVersion.AllTags;
        Check.That(tags.Count).IsEqualTo(11);
    }

    [Test]
    public void Hardware_AllTags()
    {
        var tags = DataTags.Hardware.AllTags;
        Check.That(tags.Count).IsEqualTo(9);
    }

    [Test]
    public void Host_AllTags()
    {
        var tags = DataTags.Host.AllTags;
        Check.That(tags.Count).IsEqualTo(8);
    }

    [Test]
    public void Language_AllTags()
    {
        var tags = DataTags.Language.AllTags;
        Check.That(tags.Count).IsEqualTo(47);
    }

    [Test]
    public void OperatingSystem_AllTags()
    {
        var tags = DataTags.OperatingSystem.AllTags;
        Check.That(tags.Count).IsEqualTo(8);
    }

    [Test]
    public void Platform_AllTags()
    {
        var tags = DataTags.Platform.AllTags;
        Check.That(tags.Count).IsEqualTo(6);
    }

    [Test]
    public void Usage_AllTags()
    {
        var tags = DataTags.Usage.AllTags;
        Check.That(tags.Count).IsEqualTo(82);
    }
}