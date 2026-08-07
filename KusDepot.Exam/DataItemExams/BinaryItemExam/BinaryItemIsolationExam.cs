namespace KusDepot.Exams.DataItems;

public partial class BinaryItemExam
{
    [Test]
    public void GetContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{1,2,3,4};
            BinaryItem item = new(content);

            Byte[]? got = item.GetContent();

            Check.That(got).ContainsExactly(content);
            Check.That(ReferenceEquals(got,content)).IsFalse();
        }
    }

    [Test]
    public void GetContent_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Byte[] content = new Byte[]{1,2,3,4};
            BinaryItem item = new(content);

            Byte[]? got = item.GetContent();

            Check.That(got).ContainsExactly(content);
            Check.That(ReferenceEquals(got,content)).IsTrue();
        }
    }

    [Test]
    public void SetContent_DataIsolationEnabled_IsolatesIncomingArray()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{10,20,30};
            BinaryItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Byte[]? got = item.GetContent();

            Check.That(got).ContainsExactly(content);
            Check.That(ReferenceEquals(got,content)).IsFalse();
        }
    }

    [Test]
    public void SetContent_DataIsolationDisabled_SharesIncomingArray()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Byte[] content = new Byte[]{10,20,30};
            BinaryItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Byte[]? got = item.GetContent();

            Check.That(got).ContainsExactly(content);
            Check.That(ReferenceEquals(got,content)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{5,6,7,8};
            BinaryItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Binary).ContainsExactly(content);
            Check.That(ReferenceEquals(dc.Binary,content)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Byte[] content = new Byte[]{5,6,7,8};
            BinaryItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Binary).ContainsExactly(content);
            Check.That(ReferenceEquals(dc.Binary,content)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{1,2,3};
            String name = new("IsolatedName");
            BinaryItem item = new(content,name:name);

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
            Byte[] content = new Byte[]{1,2,3};
            String name = new("SharedName");
            BinaryItem item = new(content,name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
