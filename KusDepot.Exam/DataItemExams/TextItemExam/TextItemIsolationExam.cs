namespace KusDepot.Exams.DataItems;

public partial class TextItemExam
{
    [Test]
    public void GetContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("HelloText");
            TextItem item = new(content);

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
            String content = new("HelloText");
            TextItem item = new(content);

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
            String content = new("SetText");
            TextItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent(),content)).IsFalse();
        }
    }

    [Test]
    public void SetContent_DataIsolationDisabled_SharesIncomingString()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("SetText");
            TextItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent(),content)).IsTrue();
        }
    }

    [Test]
    public void GetLanguage_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String language = new("en-US");
            TextItem item = new(language:language);

            String? got = item.GetLanguage();

            Check.That(got).IsEqualTo(language);
            Check.That(ReferenceEquals(got,language)).IsFalse();
        }
    }

    [Test]
    public void GetLanguage_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String language = new("en-US");
            TextItem item = new(language:language);

            String? got = item.GetLanguage();

            Check.That(got).IsEqualTo(language);
            Check.That(ReferenceEquals(got,language)).IsTrue();
        }
    }

    [Test]
    public void SetLanguage_DataIsolationEnabled_IsolatesIncomingString()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String language = new("fr-FR");
            TextItem item = new();

            Check.That(item.SetLanguage(language)).IsTrue();

            Check.That(ReferenceEquals(item.GetLanguage(),language)).IsFalse();
        }
    }

    [Test]
    public void SetLanguage_DataIsolationDisabled_SharesIncomingString()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String language = new("fr-FR");
            TextItem item = new();

            Check.That(item.SetLanguage(language)).IsTrue();

            Check.That(ReferenceEquals(item.GetLanguage(),language)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String content = new("DataContentText");
            TextItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Text).IsEqualTo(content);
            Check.That(ReferenceEquals(dc.Text,content)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String content = new("DataContentText");
            TextItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Text).IsEqualTo(content);
            Check.That(ReferenceEquals(dc.Text,content)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            TextItem item = new(name:name);

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
            TextItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
