namespace KusDepot.Exams;

/** <include file = 'AttributesExam.xml' path = 'AttributesExam/class[@Name = "AttributesExam"]'/> */
[TestFixture] [Parallelizable(ParallelScope.All)]
public class AttributesExam
{
    /** <include file = 'AttributesExam.xml' path = 'AttributesExam/class[@Name = "AttributesExam"]/method[@Name = "Calibrate"]'/> */
    [OneTimeSetUp]
    public void Calibrate() { }

    /** <include file = 'AttributesExam.xml' path = 'AttributesExam/class[@Name = "AttributesExam"]/method[@Name = "DataTypeConstructor"]'/> */
    [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTypeTestCases))]
    public void DataTypeConstructor(String? type)
    {
        Check.That(new DataTypeAttribute(type)).IsInstanceOfType(typeof(DataTypeAttribute));
    }

    /** <include file = 'AttributesExam.xml' path = 'AttributesExam/class[@Name = "AttributesExam"]/method[@Name = "TagConstructor"]'/> */
    [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.TagTestCases))]
    public void TagConstructor(String? tag)
    {
        Check.That(new TagAttribute(tag)).IsInstanceOfType(typeof(TagAttribute));
    }
}