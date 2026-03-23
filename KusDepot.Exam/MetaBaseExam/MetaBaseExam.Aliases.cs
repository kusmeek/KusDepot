namespace KusDepot.Exams;

public partial class MetaBaseExam
{
    [Test]
    public void SetNotes()
    {
        MetaBaseTest _0 = new();
        String[] _1 = ["note-a","note-b"];

        Check.That(_0.SetNotes(null)).IsFalse();
        Check.That(_0.SetNotes(_1)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.MyNotes).Contains(_1);
        Check.That(_0.SetNotes(Array.Empty<String>())).IsTrue();
        Check.That(_0.GetNotes()).IsNull();
    }

    [Test]
    public void SetTags()
    {
        MetaBaseTest _0 = new();
        String[] _1 = ["tag-a","tag-b"];

        Check.That(_0.SetTags(null)).IsFalse();
        Check.That(_0.SetTags(_1)).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
        Check.That(_0.MyTags).Contains(_1);
        Check.That(_0.SetTags(Array.Empty<String>())).IsTrue();
        Check.That(_0.GetTags()).IsNull();
    }

    [Test]
    public void MetaAliases()
    {
        MetaBaseTest _0 = new();
        Guid _1 = Guid.NewGuid();

        _0.MyID = _1;
        _0.MyName = "AliasName";
        _0.MyNotes = ["alias-note"];
        _0.MyTags = ["alias-tag"];

        Check.That(_0.GetID()).IsEqualTo(_1);
        Check.That(_0.GetName()).IsEqualTo("AliasName");
        Check.That(_0.GetNotes()).Contains("alias-note");
        Check.That(_0.GetTags()).Contains("alias-tag");
    }
}