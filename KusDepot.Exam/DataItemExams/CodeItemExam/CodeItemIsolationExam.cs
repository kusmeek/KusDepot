namespace KusDepot.Exams.DataItems;

public partial class CodeItemExam
{
    [Test]
    public void GetContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("HelloCode");
            CodeItem item = new(content);

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
            String content = new("HelloCode");
            CodeItem item = new(content);

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
            String content = new("SetCode");
            CodeItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            String? got = item.GetContent();

            Check.That(got).IsEqualTo(content);
            Check.That(ReferenceEquals(got,content)).IsFalse();
        }
    }

    [Test]
    public void SetContent_DataIsolationDisabled_SharesIncomingString()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("SetCode");
            CodeItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            String? got = item.GetContent();

            Check.That(got).IsEqualTo(content);
            Check.That(ReferenceEquals(got,content)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("DataContentCode");
            CodeItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Code).IsEqualTo(content);
            Check.That(ReferenceEquals(dc.Code,content)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("DataContentCode");
            CodeItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Code).IsEqualTo(content);
            Check.That(ReferenceEquals(dc.Code,content)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            CodeItem item = new(name:name);

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
            CodeItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
