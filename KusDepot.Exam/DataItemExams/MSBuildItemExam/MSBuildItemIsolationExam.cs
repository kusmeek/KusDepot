namespace KusDepot.Exams.DataItems;

public partial class MSBuildItemExam
{
    [Test]
    public void GetContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("BuildScript");
            MSBuildItem item = new(content);

            String? got = item.GetContent();

            Check.That(got).IsEqualTo(content);
            Check.That(ReferenceEquals(got,content)).IsFalse();
        }
    }

    [Test]
    public void GetContent_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("BuildScript");
            MSBuildItem item = new(content);

            String? got = item.GetContent();

            Check.That(got).IsEqualTo(content);
            Check.That(ReferenceEquals(got,content)).IsTrue();
        }
    }

    [Test]
    public void SetContent_DataIsolationEnabled_IsolatesIncomingString()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("SetScript");
            MSBuildItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent(),content)).IsFalse();
        }
    }

    [Test]
    public void SetContent_DataIsolationDisabled_SharesIncomingString()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("SetScript");
            MSBuildItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent(),content)).IsTrue();
        }
    }

    [Test]
    public void GetRequirements_DataIsolationEnabled_ClonesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem req = new(new Byte[]{1,2,3});
            MSBuildItem item = new();

            Check.That(item.SetRequirements(new DataItem[]{req})).IsTrue();

            HashSet<DataItem>? got1 = item.GetRequirements();
            HashSet<DataItem>? got2 = item.GetRequirements();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsFalse();
            Check.That(ReferenceEquals(got1!.First(),req)).IsFalse();
        }
    }

    [Test]
    public void GetRequirements_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem req = new(new Byte[]{1,2,3});
            MSBuildItem item = new();

            Check.That(item.SetRequirements(new DataItem[]{req})).IsTrue();

            HashSet<DataItem>? got1 = item.GetRequirements();
            HashSet<DataItem>? got2 = item.GetRequirements();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsTrue();
            Check.That(ReferenceEquals(got1!.First(),req)).IsTrue();
        }
    }

    [Test]
    public void GetSequence_DataIsolationEnabled_ClonesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            MSBuildItem step = new("Step1");
            MSBuildItem item = new();

            Check.That(item.SetSequence(new Dictionary<Int32,MSBuildItem>{{0,step}})).IsTrue();

            SortedList<Int32,MSBuildItem>? got1 = item.GetSequence();
            SortedList<Int32,MSBuildItem>? got2 = item.GetSequence();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsFalse();
            Check.That(ReferenceEquals(got1![0],step)).IsFalse();
        }
    }

    [Test]
    public void GetSequence_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            MSBuildItem step = new("Step1");
            MSBuildItem item = new();

            Check.That(item.SetSequence(new Dictionary<Int32,MSBuildItem>{{0,step}})).IsTrue();

            SortedList<Int32,MSBuildItem>? got1 = item.GetSequence();
            SortedList<Int32,MSBuildItem>? got2 = item.GetSequence();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsTrue();
            Check.That(ReferenceEquals(got1![0],step)).IsTrue();
        }
    }

    [Test]
    public void SetRequirements_DataIsolationEnabled_ClonesIncomingElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem req = new(new Byte[]{1,2,3});
            MSBuildItem item = new();

            Check.That(item.SetRequirements(new DataItem[]{req})).IsTrue();

            Check.That(ReferenceEquals(item.GetRequirements()!.First(),req)).IsFalse();
        }
    }

    [Test]
    public void SetRequirements_DataIsolationDisabled_SharesIncomingElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem req = new(new Byte[]{1,2,3});
            MSBuildItem item = new();

            Check.That(item.SetRequirements(new DataItem[]{req})).IsTrue();

            Check.That(ReferenceEquals(item.GetRequirements()!.First(),req)).IsTrue();
        }
    }

    [Test]
    public void SetSequence_DataIsolationEnabled_ClonesIncomingValues()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            MSBuildItem step = new("Step1");
            MSBuildItem item = new();

            Check.That(item.SetSequence(new Dictionary<Int32,MSBuildItem>{{0,step}})).IsTrue();

            Check.That(ReferenceEquals(item.GetSequence()![0],step)).IsFalse();
        }
    }

    [Test]
    public void SetSequence_DataIsolationDisabled_SharesIncomingValues()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            MSBuildItem step = new("Step1");
            MSBuildItem item = new();

            Check.That(item.SetSequence(new Dictionary<Int32,MSBuildItem>{{0,step}})).IsTrue();

            Check.That(ReferenceEquals(item.GetSequence()![0],step)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_IsolatesData()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("ContentData");
            MSBuildItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.MSBuild).ContainsKey("Content");
            Check.That(ReferenceEquals(dc.MSBuild!["Content"],content)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesData()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("ContentData");
            MSBuildItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.MSBuild).ContainsKey("Content");
            Check.That(ReferenceEquals(dc.MSBuild!["Content"],content)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            MSBuildItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsFalse();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationDisabled_SharesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String name = new("SharedName");
            MSBuildItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
