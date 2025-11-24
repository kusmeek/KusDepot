namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class IDEqualityExam
{
    [Test]
    public void EqualsAndGetHashCode()
    {
        Guid _ = Guid.NewGuid();
        String __ = "EqualsExam";

        GuidReferenceItem _0 = new(id:_);
        TextItem _1 = new(id:_);
        CodeItem _2 = new(id:_);
        BinaryItem _3 = new(id:_);
        MultiMediaItem _4 = new(id:_);
        GenericItem _5 = new(id:_);
        MSBuildItem _6 = new(id:_);

        TextItem _7 = new(__);
        TextItem _8 = new(__);

        Check.That(Equals(_7,_8)).IsTrue();
        Check.That(_7.GetHashCode()).Not.Equals(_8.GetHashCode());

        TextItem _9 = new(content:__,id:_);

        Check.That(Equals(_7,_9)).IsTrue();
        Check.That(_7.GetHashCode()).Not.Equals(_9.GetHashCode());

        TextItem _10 = new(content:__.ToUpperInvariant(),id:_);

        Check.That(Equals(_7,_10)).IsFalse();
        Check.That(_7.GetHashCode()).IsNotEqualTo(_10.GetHashCode());

        HashSet<DataItem> ___ = new(new IDEquality());

        Check.That(___.Add(_0)).IsTrue();
        Check.That(___.Add(_1)).IsFalse();
        Check.That(___.Add(_2)).IsFalse();
        Check.That(___.Add(_3)).IsFalse();
        Check.That(___.Add(_4)).IsFalse();
        Check.That(___.Add(_5)).IsFalse();
        Check.That(___.Add(_6)).IsFalse();
        Check.That(___.Add(_7)).IsTrue();
        Check.That(___.Add(_8)).IsTrue();
        Check.That(___.Add(_9)).IsFalse();
        Check.That(___.Add(_10)).IsFalse();
    }
}