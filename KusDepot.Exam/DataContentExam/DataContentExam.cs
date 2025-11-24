namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable]
public class DataContentExam
{
    [Test]
    public void Parse()
    {
        var rng = RandomNumberGenerator.Create();
        var guid = Guid.NewGuid();
        var guidRef = Guid.NewGuid();
        var binary = new Byte[4]; rng.GetBytes(binary);
        var multiMediaBytes = new Byte[4]; rng.GetBytes(multiMediaBytes);
        var code = $"code sample {rng.GetHashCode()}";
        var msbuildContent = $"msbuild content {rng.GetHashCode()}";
        var text = $"text content {rng.GetHashCode()}";
        var generic = new List<Object> { $"ParseExam{rng.GetHashCode()}", rng.GetHashCode() };
        var dataItemContent = $"item content {rng.GetHashCode()}";
        var dataItem = DataItemGenerator.CreateDataSet(3);
        var dataSet = new HashSet<DataItem> { dataItem };

        var requirements = new HashSet<DataItem> { new TextItem("req1"), new TextItem("req2") };
        var sequence = new SortedList<Int32, MSBuildItem> { { 0, new MSBuildItem("seq1" ) }, { 1, new MSBuildItem("seq2") } };
        var msbuildDict = new Dictionary<String, Object?>
        {
            { "Content", msbuildContent },
            { "Requirements", requirements },
            { "Sequence", sequence }
        };

        var nextGuidRef = new GuidReferenceItem(Guid.NewGuid());
        var prevGuidRef = new GuidReferenceItem(Guid.NewGuid());
        var guidReferenceDict = new Dictionary<String, Object?>
        {
            { "Content", guidRef },
            { "Next", nextGuidRef },
            { "Previous", prevGuidRef }
        };

        var multiMediaDict = new Dictionary<String, Object?>
        {
            { "Content", multiMediaBytes }
        };

        var original = new DataContent
        {
            Binary = binary,
            Code = code,
            DataSet = dataSet,
            Generic = generic,
            GuidReference = guidReferenceDict,
            MSBuild = msbuildDict,
            MultiMedia = multiMediaDict,
            Text = text
        };

        var serialized = original.ToString(); var parsed = DataContent.Parse(serialized);

        Check.That(parsed).IsNotNull();
        Check.That(parsed!.Binary).IsEqualTo(original.Binary);
        Check.That(parsed.Code).IsEqualTo(original.Code);
        Check.That(parsed.DataSet).IsEqualTo(original.DataSet);
        Check.That(parsed.Generic).IsNotNull();
        Check.That(parsed.Generic!.Count).IsEqualTo(original.Generic!.Count);
        Check.That(parsed.GuidReference).IsNotNull();
        Check.That(parsed.GuidReference!.Count).IsEqualTo(original.GuidReference!.Count);
        Check.That(parsed.GuidReference["Content"]).IsEqualTo(guidReferenceDict["Content"]);
        Check.That(parsed.GuidReference["Next"]).IsInstanceOfType(typeof(GuidReferenceItem));
        Check.That(parsed.GuidReference["Previous"]).IsInstanceOfType(typeof(GuidReferenceItem));
        Check.That(parsed.MSBuild).IsNotNull();
        Check.That(parsed.MSBuild!.Count).IsEqualTo(original.MSBuild!.Count);
        Check.That(parsed.MSBuild["Content"]).IsEqualTo(msbuildDict["Content"]);
        Check.That(parsed.MSBuild["Requirements"]).IsInstanceOfType(typeof(HashSet<DataItem>));
        Check.That(parsed.MSBuild["Sequence"]).IsInstanceOfType(typeof(SortedList<Int32, MSBuildItem>));
        Check.That(parsed.MultiMedia).IsNotNull();
        Check.That(parsed.MultiMedia!.Count).IsEqualTo(original.MultiMedia!.Count);
        Check.That(parsed.MultiMedia["Content"]).IsEqualTo(multiMediaDict["Content"]);
        Check.That(parsed.Text).IsEqualTo(original.Text);
    }

    [Test]
    public void TryParse_Valid_Roundtrips()
    {
        var rng = RandomNumberGenerator.Create();
        var binary = new Byte[8]; rng.GetBytes(binary);
        var multiMediaBytes = new Byte[8]; rng.GetBytes(multiMediaBytes);
        var generic = new List<Object> { $"TryParseExam{rng.GetHashCode()}", rng.GetHashCode() };
        var requirements = new HashSet<DataItem> { new TextItem("reqA"), new TextItem("reqB") };
        var sequence = new SortedList<Int32, MSBuildItem> { { 5, new MSBuildItem("s5") }, { 7, new MSBuildItem("s7") } };
        var msbuildDict = new Dictionary<String, Object?>
        {
            { "Content", $"msbuild content {rng.GetHashCode()}" },
            { "Requirements", requirements },
            { "Sequence", sequence }
        };
        var guidReferenceDict = new Dictionary<String, Object?>
        {
            { "Content", Guid.NewGuid() },
            { "Next", new GuidReferenceItem(Guid.NewGuid()) },
            { "Previous", new GuidReferenceItem(Guid.NewGuid()) }
        };
        var multiMediaDict = new Dictionary<String, Object?> { { "Content", multiMediaBytes } };

        var original = new DataContent
        {
            Binary = binary,
            Code = $"code {rng.GetHashCode()}",
            DataSet = new HashSet<DataItem> { DataItemGenerator.CreateDataSet(2) },
            Generic = generic,
            GuidReference = guidReferenceDict,
            MSBuild = msbuildDict,
            MultiMedia = multiMediaDict,
            Text = $"text {rng.GetHashCode()}"
        };

        var serialized = original.ToString();
        var ok = DataContent.TryParse(serialized, null, out var parsed);

        Check.That(ok).IsTrue();
        Check.That(parsed).IsNotNull();
        Check.That(parsed!.Binary).IsEqualTo(original.Binary);
        Check.That(parsed.Code).IsEqualTo(original.Code);
        Check.That(parsed.DataSet).IsNotNull();
        Check.That(parsed.DataSet!.Count).IsEqualTo(original.DataSet!.Count);
        Check.That(parsed.Generic).IsNotNull();
        Check.That(parsed.Generic!.Count).IsEqualTo(original.Generic!.Count);
        Check.That(parsed.GuidReference).IsNotNull();
        Check.That(parsed.GuidReference!.Count).IsEqualTo(original.GuidReference!.Count);
        Check.That(parsed.MSBuild).IsNotNull();
        Check.That(parsed.MSBuild!.Count).IsEqualTo(original.MSBuild!.Count);
        Check.That(parsed.MultiMedia).IsNotNull();
        Check.That(parsed.MultiMedia!.Count).IsEqualTo(original.MultiMedia!.Count);
        Check.That(parsed.Text).IsEqualTo(original.Text);
    }
}