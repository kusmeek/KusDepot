namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class AttributesExam
{
    [OneTimeSetUp]
    public void Calibrate() { }

    [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTagTestCases))]
    public void DataTagConstructor(String? tag)
    {
        Check.That(new DataTagAttribute(tag)).IsInstanceOfType(typeof(DataTagAttribute));
    }

    [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTypeTestCases))]
    public void DataTypeConstructor(String? type)
    {
        Check.That(new DataTypeAttribute(type)).IsInstanceOfType(typeof(DataTypeAttribute));
    }
}