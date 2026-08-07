namespace KusDepot.Exams;

public partial class MetaBaseExam
{
    [Test]
    public void DataIsolationEnabled_MetadataAccessors_IsolateValues()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            MetaBaseTest item = new();
            String application = CloneString("Application");
            Version applicationversion = new Version("1.2.3.4");
            HashSet<String> notes = new(){CloneString("NoteA"),CloneString("NoteB")};
            HashSet<String> tags = new(){CloneString("TagA"),CloneString("TagB")};
            Dictionary<String,Object?> extension = new(){[CloneString("ExtKey")] = new Object()};
            GuidReferenceItem link = new(Guid.NewGuid());
            Dictionary<String,GuidReferenceItem> links = new(){[CloneString("LinkKey")] = link};

            Check.That(item.SetApplication(application)).IsTrue();
            Check.That(item.SetApplicationVersion(applicationversion)).IsTrue();
            Check.That(item.SetNotes(notes)).IsTrue();
            Check.That(item.SetTags(tags)).IsTrue();
            Check.That(item.SetExtension(extension)).IsTrue();
            Check.That(item.SetLinks(links)).IsTrue();

            String? gotapplication = item.GetApplication();
            Version? gotapplicationversion = item.GetApplicationVersion();
            HashSet<String>? gotnotes = item.GetNotes();
            HashSet<String>? gottags = item.GetTags();
            Dictionary<String,Object?>? gotextension = item.GetExtension();
            Dictionary<String,GuidReferenceItem>? gotlinks = item.GetLinks();
            String notestring = gotnotes!.First(_ => _.Equals("NoteA"));
            String tagstring = gottags!.First(_ => _.Equals("TagA"));

            Check.That(gotapplication).IsEqualTo(application);
            Check.That(ReferenceEquals(gotapplication,application)).IsFalse();
            Check.That(gotapplicationversion).IsEqualTo(applicationversion);
            Check.That(ReferenceEquals(gotapplicationversion,applicationversion)).IsFalse();
            Check.That(gotnotes).Contains(notes);
            Check.That(ReferenceEquals(gotnotes,notes)).IsFalse();
            Check.That(ReferenceEquals(notestring,notes.First(_ => _.Equals("NoteA")))).IsFalse();
            Check.That(gottags).Contains(tags);
            Check.That(ReferenceEquals(gottags,tags)).IsFalse();
            Check.That(ReferenceEquals(tagstring,tags.First(_ => _.Equals("TagA")))).IsFalse();
            Check.That(gotextension).ContainsKey("ExtKey");
            Check.That(ReferenceEquals(gotextension,extension)).IsFalse();
            Check.That(gotlinks).ContainsKey("LinkKey");
            Check.That(ReferenceEquals(gotlinks,links)).IsFalse();
            Check.That(ReferenceEquals(gotlinks!["LinkKey"],link)).IsFalse();
        }
    }

    [Test]
    public void DataIsolationDisabled_MetadataAccessors_ShareValues()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            MetaBaseTest item = new();
            String application = CloneString("Application");
            Version applicationversion = new Version("1.2.3.4");
            HashSet<String> notes = new(){CloneString("NoteA"),CloneString("NoteB")};
            HashSet<String> tags = new(){CloneString("TagA"),CloneString("TagB")};
            Dictionary<String,Object?> extension = new(){[CloneString("ExtKey")] = new Object()};
            GuidReferenceItem link = new(Guid.NewGuid());
            Dictionary<String,GuidReferenceItem> links = new(){[CloneString("LinkKey")] = link};

            Check.That(item.SetApplication(application)).IsTrue();
            Check.That(item.SetApplicationVersion(applicationversion)).IsTrue();
            Check.That(item.SetNotes(notes)).IsTrue();
            Check.That(item.SetTags(tags)).IsTrue();
            Check.That(item.SetExtension(extension)).IsTrue();
            Check.That(item.SetLinks(links)).IsTrue();

            Check.That(ReferenceEquals(item.GetApplication(),application)).IsTrue();
            Check.That(ReferenceEquals(item.GetApplicationVersion(),applicationversion)).IsTrue();
            Check.That(ReferenceEquals(item.GetNotes(),notes)).IsTrue();
            Check.That(ReferenceEquals(item.GetTags(),tags)).IsTrue();
            Check.That(ReferenceEquals(item.GetExtension(),extension)).IsTrue();
            Check.That(ReferenceEquals(item.GetLinks(),links)).IsTrue();
            Check.That(ReferenceEquals(item.GetLinks()!["LinkKey"],link)).IsTrue();
        }
    }

    [Test]
    public void DataIsolationEnabled_MetadataMutators_IsolateIncomingCollections()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            MetaBaseTest item = new();
            HashSet<String> addnotes = new(){CloneString("AddNote")};
            HashSet<String> addtags = new(){CloneString("AddTag")};
            HashSet<String> setnotes = new(){CloneString("SetNote")};
            HashSet<String> settags = new(){CloneString("SetTag")};

            Check.That(item.AddNotes(addnotes)).IsTrue();
            Check.That(item.AddTags(addtags)).IsTrue();
            Check.That(item.SetNotes(setnotes)).IsTrue();
            Check.That(item.SetTags(settags)).IsTrue();

            HashSet<String>? gotnotes = item.GetNotes();
            HashSet<String>? gottags = item.GetTags();
            String setnotestring = gotnotes!.First(_ => _.Equals("SetNote"));
            String settagstring = gottags!.First(_ => _.Equals("SetTag"));

            Check.That(ReferenceEquals(gotnotes,setnotes)).IsFalse();
            Check.That(ReferenceEquals(setnotestring,setnotes.First(_ => _.Equals("SetNote")))).IsFalse();
            Check.That(ReferenceEquals(gottags,settags)).IsFalse();
            Check.That(ReferenceEquals(settagstring,settags.First(_ => _.Equals("SetTag")))).IsFalse();
        }
    }

    [Test]
    public void GetDescriptor_RespectsDataIsolationMode()
    {
        String application = CloneString("DescriptorApplication");
        String name = CloneString("DescriptorName");
        HashSet<String> notes = new(){CloneString("NoteA")};
        HashSet<String> tags = new(){CloneString("TagA")};

        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            MetaBaseTest item = new();
            Check.That(item.SetApplication(application)).IsTrue();
            Check.That(item.SetName(name)).IsTrue();
            Check.That(item.SetNotes(notes)).IsTrue();
            Check.That(item.SetTags(tags)).IsTrue();

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Application).IsEqualTo(application);
            Check.That(ReferenceEquals(descriptor.Application,application)).IsFalse();
            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsFalse();
            Check.That(ReferenceEquals(descriptor.Notes,notes)).IsFalse();
            Check.That(ReferenceEquals(descriptor.Tags,tags)).IsFalse();
        }

        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            MetaBaseTest item = new();
            Check.That(item.SetApplication(application)).IsTrue();
            Check.That(item.SetName(name)).IsTrue();
            Check.That(item.SetNotes(notes)).IsTrue();
            Check.That(item.SetTags(tags)).IsTrue();

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(ReferenceEquals(descriptor.Application,application)).IsTrue();
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
            Check.That(ReferenceEquals(descriptor.Notes,notes)).IsTrue();
            Check.That(ReferenceEquals(descriptor.Tags,tags)).IsTrue();
        }
    }

    private static String CloneString(String value) => new(value);
}
