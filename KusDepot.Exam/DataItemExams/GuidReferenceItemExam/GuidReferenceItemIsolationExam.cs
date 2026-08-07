namespace KusDepot.Exams.DataItems;

public partial class GuidReferenceItemExam
{
    [Test]
    public void GetNext_DataIsolationEnabled_ClonesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            GuidReferenceItem next = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetNext(next)).IsTrue();

            GuidReferenceItem? got = item.GetNext();

            Check.That(got).IsNotNull();
            Check.That(got!.GetContent()).IsEqualTo(next.GetContent());
            Check.That(ReferenceEquals(got,next)).IsFalse();
        }
    }

    [Test]
    public void GetNext_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            GuidReferenceItem next = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetNext(next)).IsTrue();

            GuidReferenceItem? got = item.GetNext();

            Check.That(got).IsNotNull();
            Check.That(ReferenceEquals(got,next)).IsTrue();
        }
    }

    [Test]
    public void GetPrevious_DataIsolationEnabled_ClonesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            GuidReferenceItem previous = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetPrevious(previous)).IsTrue();

            GuidReferenceItem? got = item.GetPrevious();

            Check.That(got).IsNotNull();
            Check.That(got!.GetContent()).IsEqualTo(previous.GetContent());
            Check.That(ReferenceEquals(got,previous)).IsFalse();
        }
    }

    [Test]
    public void GetPrevious_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            GuidReferenceItem previous = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetPrevious(previous)).IsTrue();

            GuidReferenceItem? got = item.GetPrevious();

            Check.That(got).IsNotNull();
            Check.That(ReferenceEquals(got,previous)).IsTrue();
        }
    }

    [Test]
    public void GetUndirectedLinks_DataIsolationEnabled_ClonesElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            GuidReferenceItem link = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetUndirectedLinks(new[]{link})).IsTrue();

            HashSet<GuidReferenceItem>? got = item.GetUndirectedLinks();

            Check.That(got).IsNotNull();
            Check.That(ReferenceEquals(got,item.GetUndirectedLinks())).IsFalse();
            Check.That(ReferenceEquals(got!.First(),link)).IsFalse();
        }
    }

    [Test]
    public void GetUndirectedLinks_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            GuidReferenceItem link = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetUndirectedLinks(new[]{link})).IsTrue();

            HashSet<GuidReferenceItem>? got1 = item.GetUndirectedLinks();
            HashSet<GuidReferenceItem>? got2 = item.GetUndirectedLinks();

            Check.That(ReferenceEquals(got1,got2)).IsTrue();
            Check.That(ReferenceEquals(got1!.First(),link)).IsTrue();
        }
    }

    [Test]
    public void SetNext_DataIsolationEnabled_IsolatesIncomingValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            GuidReferenceItem next = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetNext(next)).IsTrue();

            Check.That(ReferenceEquals(item.GetNext(),next)).IsFalse();
        }
    }

    [Test]
    public void SetNext_DataIsolationDisabled_SharesIncomingValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            GuidReferenceItem next = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetNext(next)).IsTrue();

            Check.That(ReferenceEquals(item.GetNext(),next)).IsTrue();
        }
    }

    [Test]
    public void SetUndirectedLinks_DataIsolationEnabled_IsolatesElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            GuidReferenceItem link = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetUndirectedLinks(new[]{link})).IsTrue();

            Check.That(ReferenceEquals(item.GetUndirectedLinks()!.First(),link)).IsFalse();
        }
    }

    [Test]
    public void SetUndirectedLinks_DataIsolationDisabled_SharesElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            GuidReferenceItem link = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetUndirectedLinks(new[]{link})).IsTrue();

            Check.That(ReferenceEquals(item.GetUndirectedLinks()!.First(),link)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_ClonesGraphRefs()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            GuidReferenceItem next = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetNext(next)).IsTrue();

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.GuidReference).ContainsKey("Next");
            Check.That(ReferenceEquals(dc.GuidReference!["Next"],next)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesGraphRefs()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            GuidReferenceItem next = new(Guid.NewGuid());
            GuidReferenceItem item = new(Guid.NewGuid());

            Check.That(item.SetNext(next)).IsTrue();

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.GuidReference).ContainsKey("Next");
            Check.That(ReferenceEquals(dc.GuidReference!["Next"],next)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            GuidReferenceItem item = new(Guid.NewGuid(),name:name);

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
            GuidReferenceItem item = new(Guid.NewGuid(),name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
