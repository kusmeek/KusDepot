namespace KusDepot.Exams.DataItems;

public partial class DataSetItemExam
{
    [Test]
    public void GetContent_DataIsolationEnabled_ClonesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            HashSet<DataItem>? got1 = item.GetContent();
            HashSet<DataItem>? got2 = item.GetContent();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsFalse();
            Check.That(ReferenceEquals(got1!.First(),item1)).IsFalse();
        }
    }

    [Test]
    public void GetContent_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            HashSet<DataItem>? got1 = item.GetContent();
            HashSet<DataItem>? got2 = item.GetContent();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsTrue();
            Check.That(ReferenceEquals(got1!.First(),item1)).IsTrue();
        }
    }

    [Test]
    public void GetDataItem_DataIsolationEnabled_ClonesItem()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            DataItem? got = item.GetDataItem(item1.GetID(),security:null);

            Check.That(got).IsNotNull();
            Check.That(ReferenceEquals(got,item1)).IsFalse();
        }
    }

    [Test]
    public void GetDataItem_DataIsolationDisabled_SharesItem()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            DataItem? got = item.GetDataItem(item1.GetID(),security:null);

            Check.That(got).IsNotNull();
            Check.That(ReferenceEquals(got,item1)).IsTrue();
        }
    }

    [Test]
    public void GetDataItems_DataIsolationEnabled_ClonesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            HashSet<DataItem>? got1 = item.GetDataItems(security:null);
            HashSet<DataItem>? got2 = item.GetDataItems(security:null);

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsFalse();
            Check.That(ReferenceEquals(got1!.First(),item1)).IsFalse();
        }
    }

    [Test]
    public void GetDataItems_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            HashSet<DataItem>? got1 = item.GetDataItems(security:null);
            HashSet<DataItem>? got2 = item.GetDataItems(security:null);

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsTrue();
            Check.That(ReferenceEquals(got1!.First(),item1)).IsTrue();
        }
    }

    [Test]
    public void SetContent_DataIsolationEnabled_ClonesIncomingElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new();

            Check.That(item.SetContent(new DataItem[]{item1}, (DataItemSecurityContext?)null)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent()!.First(),item1)).IsFalse();
        }
    }

    [Test]
    public void SetContent_DataIsolationDisabled_SharesIncomingElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new();

            Check.That(item.SetContent(new DataItem[]{item1}, (DataItemSecurityContext?)null)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent()!.First(),item1)).IsTrue();
        }
    }

    [Test]
    public void AddDataItems_DataIsolationEnabled_ClonesIncomingElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new();

            Check.That(item.AddDataItems(new DataItem[]{item1}, (DataItemSecurityContext?)null)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent()!.First(),item1)).IsFalse();
        }
    }

    [Test]
    public void AddDataItems_DataIsolationDisabled_SharesIncomingElements()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new();

            Check.That(item.AddDataItems(new DataItem[]{item1}, (DataItemSecurityContext?)null)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent()!.First(),item1)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_ClonesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.DataSet).IsNotNull();
            Check.That(ReferenceEquals(dc.DataSet,item.GetContent())).IsFalse();
            Check.That(ReferenceEquals(dc.DataSet!.First(),item1)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            BinaryItem item1 = new(new Byte[]{1,2,3});
            DataSetItem item = new(new DataItem[]{item1});

            DataContent? dc1 = await item.GetDataContent(security:null);
            DataContent? dc2 = await item.GetDataContent(security:null);

            Check.That(dc1).IsNotNull();
            Check.That(ReferenceEquals(dc1!.DataSet,dc2!.DataSet)).IsTrue();
            Check.That(ReferenceEquals(dc1.DataSet!.First(),item1)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            DataSetItem item = new(name:name);

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
            DataSetItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
