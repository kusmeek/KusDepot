namespace KusDepot.Exams;

[TestFixture]
public partial class UtilityExam
{
    private interface IReflectionTestA {}
    private interface IReflectionTestB<T> {}
    private class ReflectionTestA : IReflectionTestA {}
    private class ReflectionTestB<T> : ReflectionTestA, IReflectionTestB<T> {}
    private class ReflectionTestC<T,U> : ReflectionTestB<T>, IReflectionTestA {}

    [Test]
    public void GetInheritanceList_Tests()
    {
        Check.That(GetInheritanceList(null)).IsNull();

        var testObjectB = new ReflectionTestB<Int32>();
        Check.That(GetInheritanceList(testObjectB)).IsEqualTo("System.Object -> KusDepot.Exams.UtilityExam.ReflectionTestA -> KusDepot.Exams.UtilityExam.ReflectionTestB<System.Int32>");

        var testObjectC = new ReflectionTestC<String, Double>();
        Check.That(GetInheritanceList(testObjectC)).IsEqualTo("System.Object -> KusDepot.Exams.UtilityExam.ReflectionTestA -> KusDepot.Exams.UtilityExam.ReflectionTestB<System.String> -> KusDepot.Exams.UtilityExam.ReflectionTestC<System.String,System.Double>");
    }

    [Test]
    public void GetInterfaceList_Tests()
    {
        Check.That(GetInterfaceList(null)).IsNull();

        var testObjectA = new ReflectionTestA();
        Check.That(GetInterfaceList(testObjectA)).IsEqualTo("KusDepot.Exams.UtilityExam.IReflectionTestA");

        var testObjectB = new ReflectionTestB<Boolean>();
        Check.That(GetInterfaceList(testObjectB)).Contains("KusDepot.Exams.UtilityExam.IReflectionTestA").And.Contains("KusDepot.Exams.UtilityExam.IReflectionTestB<System.Boolean>");

        var testObjectC = new ReflectionTestC<Guid, DateTime>();
        Check.That(GetInterfaceList(testObjectC))
             .Contains("KusDepot.Exams.UtilityExam.IReflectionTestB<System.Guid>")
             .And.Contains("KusDepot.Exams.UtilityExam.IReflectionTestA");
    }

    [Test]
    public void GetInterfaceList_Type_Tests()
    {
        Check.That(GetInterfaceList((Type)null!)).IsNull();

        Check.That(GetInterfaceList(typeof(ReflectionTestA))).IsEqualTo("KusDepot.Exams.UtilityExam.IReflectionTestA");

        Check.That(GetInterfaceList(typeof(ReflectionTestB<Version>))).Contains("KusDepot.Exams.UtilityExam.IReflectionTestA").And.Contains("KusDepot.Exams.UtilityExam.IReflectionTestB<System.Version>");
    }
}
