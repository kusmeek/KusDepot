namespace KusDepot.Exams.DataItems;

public partial class KeySetExam
{
    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            KeySet item = new();

            Check.That(item.SetName(name)).IsTrue();

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
            KeySet item = new();

            Check.That(item.SetName(name)).IsTrue();

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
