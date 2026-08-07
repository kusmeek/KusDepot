namespace KusDepot.Exams.DataItems;

public partial class GenericItemExam
{
    [Test]
    public async Task GetDataContent_DataIsolationEnabled_IsolatesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            List<Object> content = new(){"A","B"};
            GenericItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Generic).IsNotNull();
            Check.That(ReferenceEquals(dc.Generic,item.GetContent())).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            List<Object> content = new(){"A","B"};
            GenericItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.Generic).IsNotNull();

            DataContent? dc2 = await item.GetDataContent(security:null);

            Check.That(ReferenceEquals(dc.Generic,dc2!.Generic)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            GenericItem item = new(name:name);

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
            GenericItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
