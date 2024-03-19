namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class ToolActorExam
{
    public IToolActor? Proxy;

    [Test]
    public void Activate() { Check.That(this.Proxy!.Activate().Result).IsTrue(); }

    [Test]
    public void AddDataItems()
    {
        HashSet<DataItem> _0  = new HashSet<DataItem>();
        GuidReferenceItem _1  = new GuidReferenceItem(); _1.SetID(Guid.NewGuid());
        GenericItem _2        = new GenericItem(); _2.SetID(Guid.NewGuid());
        TextItem _3           = new TextItem(); _3.SetID(Guid.NewGuid());
        MSBuildItem _4        = new MSBuildItem(); _4.SetID(Guid.NewGuid());
        BinaryItem _5         = new BinaryItem(); _5.SetID(Guid.NewGuid());
        Byte[] _15            = new Byte[1048576]; RandomNumberGenerator.Create().GetBytes(_15); _5.SetContent(_15);
        Dictionary<String,Byte[]> _5sh = new Dictionary<String,Byte[]>();
        _5sh.Add("Content",SHA512.HashData(_15)); _5.SetSecureHashes(_5sh);
        MultiMediaItem _6     = new MultiMediaItem(); _5.SetID(Guid.NewGuid());
        List<Guid?> _8        = new List<Guid?>();
        List<Guid?> _10       = new List<Guid?>();
        HashSet<DataItem> _11 = new HashSet<DataItem>();
        List<Guid?> _12       = new List<Guid?>();
        List<Guid?> _13       = new List<Guid?>();

        _0.Add(_1); _8.Add(_1.GetID()); _12.Add(_1.GetID());
        _0.Add(_2); _8.Add(_2.GetID()); _12.Add(_2.GetID());
        _0.Add(_3); _8.Add(_3.GetID()); _12.Add(_3.GetID());
        _0.Add(_4); _8.Add(_4.GetID()); _12.Add(_4.GetID());
        _0.Add(_5); _8.Add(_5.GetID()); _12.Add(_5.GetID());
        Check.That(this.Proxy!.AddDataItems(_0).Result).IsTrue();
        HashSet<DataItem>? _9   = this.Proxy!.GetDataItems().Result!.ToHashSet();
        if( _9 is not null )
        {
            foreach(DataItem item in _9)
            {
                _10!.Add(item!.GetID());
            }
        }
        _11.Add(_6);
        _12.Add(_6.GetID());
        Check.That(this.Proxy!.AddDataItems(_11).Result).IsTrue();
        HashSet<DataItem>? _14   = this.Proxy!.GetDataItems().Result!.ToHashSet();
        if( _14 is not null )
        {
            foreach(DataItem item in _14)
            {
                _13!.Add(item!.GetID());
            }
        }

        Check.That(this.Proxy!.AddDataItems(null).Result).IsFalse();
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_13).ContainsExactly(_12.ToArray());
    }

    [Test]
    public void AddInput()
    {
        String _1 = "AddInputExam";
        String _2 = "Pass";

        Check.That(this.Proxy!.GetInputs().Result).IsEmpty();
        Check.That(this.Proxy.AddInput(_1).Result).IsTrue();
        Check.That(this.Proxy.GetInputs().Result?.Count).Equals(1);
        Check.That(this.Proxy.AddInput(_2).Result).IsTrue();
        Check.That(this.Proxy.GetInputs().Result?.Count).Equals(2);
        Check.That(this.Proxy.GetInput().Result).Equals(_1);
        Check.That(this.Proxy.GetInputs().Result?.Count).Equals(1);
        Check.That(this.Proxy.GetInput().Result).Equals(_2);
        Check.That(this.Proxy.GetInputs().Result).IsEmpty();
    }

    [Test]
    public void AddNotes()
    {
        List<String> _1 = new List<String>(){"AddNotesExam"};
        List<String> _2 = new List<String>(){"Pass"};
        List<String> _3 = new List<String>();
        _3.AddRange(_1);
        _3.AddRange(_2);

        Check.That(this.Proxy!.AddNotes(null).Result).IsFalse();
        Check.That(this.Proxy!.GetNotes().Result).IsEmpty();
        Check.That(this.Proxy.AddNotes(_1.ToHashSet()).Result).IsTrue();
        Check.That(this.Proxy.GetNotes().Result).ContainsExactly(_1);
        Check.That(this.Proxy.AddNotes(_2.ToHashSet()).Result).IsTrue();
        Check.That(this.Proxy.GetNotes().Result).ContainsExactly(_3);
    }

    [Test]
    public void AddOutput()
    {
        String _1 = "AddOutputExam"; Guid _11 = Guid.NewGuid();
        String _2 = "Pass";          Guid _12 = Guid.NewGuid();

        Check.That(this.Proxy!.AddOutput(Guid.NewGuid(),null).Result).IsFalse();
        Check.That(this.Proxy!.GetOutputIDs().Result).IsEmpty();
        Check.That(this.Proxy.AddOutput(_11,_1).Result).IsTrue();
        Check.That(this.Proxy.GetOutputIDs().Result).Contains(_11);
        Check.That(this.Proxy.AddOutput(_12,_2).Result).IsTrue();
        Check.That(this.Proxy.GetOutputIDs().Result).Contains(_11,_12);
        Check.That(this.Proxy.GetOutput(_12).Result).Equals(_2);
    }

    [Test]
    public void AddTags()
    {
        String _1 = "AddTagsExam";
        String _2 = "Pass";
        List<String> _3 = new List<String>();
        _3.Add(_1);
        List<String> _4 = new List<String>();
        _4.Add(_2);

        Check.That(this.Proxy!.AddTags(null).Result).IsFalse();
        Check.That(this.Proxy!.GetTags().Result).IsEmpty();
        Check.That(this.Proxy.AddTags(_3.ToHashSet()).Result).IsTrue();
        Check.That(this.Proxy.GetTags().Result).ContainsExactly(_1);
        Check.That(this.Proxy.AddTags(_4.ToHashSet()).Result).IsTrue();
        Check.That(this.Proxy.GetTags().Result).ContainsExactly(_1 , _2);
    }

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Proxy = ActorProxy.Create<IToolActor>(new ActorId(Guid.NewGuid()),ServiceLocators.ToolActorService);
    }

    [Test]
    public void ClearEventLogs()
    {
        String _1 = "ClearEventLogs";
        String _2 = "Pass";

        Check.That(this.Proxy!.LogEvent(_1).Result).IsTrue();
        Check.That(this.Proxy.LogEvent(_2).Result).IsTrue();
        Check.That(this.Proxy.GetEventLogs().Result!.Keys.Count).Equals(2);
        Check.That(this.Proxy.GetEvent(0).Result).Equals(_1);
        Check.That(this.Proxy.GetEvent(1).Result).Equals(_2);
        Check.That(this.Proxy.ClearEventLogs().Result).IsTrue();
        Check.That(this.Proxy.GetEventLogs().Result).IsNull();
    }

    [OneTimeTearDown]
    public void Complete() { }

    [Test]
    public void Deactivate() { Check.That(this.Proxy!.Deactivate().Result).IsTrue(); }

    [Test]
    public void ExecuteCommand()
    {
        Object[] _1 = new Object[4]{"ExecuteCommand","P0","P1","P2"};

        Check.That(this.Proxy!.ExecuteCommand(_1).Result).IsNull();
    }

    [Test]
    public void GetAppDomainID()
    {

        Check.That(this.Proxy!.GetAppDomainID().Result).HasAValue();
    }

    [Test]
    public void GetAppDomainUID()
    {

        Check.That(this.Proxy!.GetAppDomainUID().Result).HasAValue();
    }

    [Test]
    public void GetApplication()
    {
        Check.That(this.Proxy!.GetApplication().Result).IsEmpty();
    }

    [Test]
    public void GetApplicationVersion()
    {
        Check.That(this.Proxy!.GetApplicationVersion().Result).IsNotNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        Check.That(this.Proxy!.GetAssemblyVersion().Result).IsNotNull();;
    }

    [Test]
    public void GetBornOn()
    {
        Check.That(this.Proxy!.GetBornOn().Result).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        Check.That(this.Proxy!.GetCertificates().Result).IsEmpty();
    }

    [Test]
    public void GetCPUID()
    {
        Check.That(this.Proxy!.GetAppDomainUID().Result).HasAValue();
    }

    [Test]
    public void GetDataItems()
    {
        DataItem[] _0 = this.Proxy!.GetDataItems().Result!.ToArray();

        Check.That(_0.Length).IsEqualTo(6);

        foreach(DataItem i in _0)
        {
            if(((Object)i).GetType() == typeof(BinaryItem))
            {
                BinaryItem d = (BinaryItem)i;

                Check.That(d.GetContent()!.Length).IsEqualTo(1048576);

                Check.That(d.GetSecureHashes()!["Content"]).IsEqualTo(SHA512.HashData(d.GetContent()!));
            }
        }
    }

    [Test]
    public void GetDistinguishedName()
    {
        String _1 = "CN=ServiceN,DC=Journey International,DC=ORG";

        Check.That(this.Proxy!.SetDistinguishedName(_1).Result).IsTrue();
        Check.That(this.Proxy.GetDistinguishedName().Result).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        Check.That(this.Proxy!.GetDomainID().Result).IsEmpty();
    }

    [Test]
    public void GetEvent()
    {
        String _1 = "GetEventExam";
        String _2 = "Pass";

        Check.That(this.Proxy!.LogEvent(_1).Result).IsTrue();
        Check.That(this.Proxy.LogEvent(_2).Result).IsTrue();
        Check.That(this.Proxy.GetEventLogs().Result!.Keys.Count).Equals(2);
        Check.That(this.Proxy.GetEvent(0).Result).Equals(_1);
        Check.That(this.Proxy.GetEvent(1).Result).Equals(_2);
        Check.That(this.Proxy.ClearEventLogs().Result).IsTrue();
    }

    [Test]
    public void GetEventLogs()
    {
        Check.That(this.Proxy!.GetEventLogs().Result).IsNull();
    }

    [Test]
    public void GetExtension()
    {
        Check.That(this.Proxy!.GetExtension().Result).IsEmpty();
    }

    [Test]
    public void GetGPS()
    {
        Check.That(this.Proxy!.GetGPS().Result).IsEmpty();
    }

    [Test]
    public void GetID()
    {
        Check.That(this.Proxy!.GetID().Result).HasAValue();
    }

    [Test]
    public void GetInput()
    {
        String _1 = "GetInputExam";
        String _2 = "Pass";

        Check.That(this.Proxy!.GetInputs().Result).IsEmpty();
        Check.That(this.Proxy.AddInput(_1).Result).IsTrue();
        Check.That(this.Proxy.GetInputs().Result?.Count).Equals(1);
        Check.That(this.Proxy.AddInput(_2).Result).IsTrue();
        Check.That(this.Proxy.GetInputs().Result?.Count).Equals(2);
        Check.That(this.Proxy.GetInput().Result).Equals(_1);
        Check.That(this.Proxy.GetInputs().Result?.Count).Equals(1);
        Check.That(this.Proxy.GetInput().Result).Equals(_2);
        Check.That(this.Proxy.GetInputs().Result).IsEmpty();
    }

    [Test]
    public void GetInputs()
    {
        Check.That(this.Proxy!.GetInputs().Result).IsEmpty();
    }

    [Test]
    public void GetLocator()
    {
        Check.That(this.Proxy!.GetLocator().Result).IsNotNull();
    }

    [Test]
    public void GetMachineID()
    {
        Check.That(this.Proxy!.GetMachineID().Result).IsNotNull();
    }

    [Test]
    public void GetModified()
    {
        Check.That(this.Proxy!.GetModified().Result).HasAValue();
    }

    [Test]
    public void GetName()
    {
        Check.That(this.Proxy!.GetName().Result).IsEmpty();
    }

    [Test]
    public void GetNotes()
    {
        Check.That(this.Proxy!.GetNotes().Result).HasSize(2);
    }

    [Test]
    public void GetObjectives()
    {
        String _1 = "GetObjectivesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(this.Proxy!.SetObjectives(_2).Result).IsTrue();
        Check.That(this.Proxy.GetObjectives().Result).IsEquivalentTo(_2);
        Check.That(this.Proxy.GetObjectives().Result!.ToArray().First()).IsEqualTo(_1);
    }

    [Test]
    public void GetOutput()
    {
        String _1 = "GetOutputExam";
        Guid _2 = Guid.NewGuid();

        Check.That(this.Proxy!.GetOutput(Guid.Empty).Result).IsNull();
        Check.That(this.Proxy.AddOutput(_2,_1).Result).IsTrue();
        Check.That(this.Proxy.Lock("Key").Result).IsTrue();
        Check.That(this.Proxy.GetOutput(_2).Result).Equals(_1);
        Check.That(this.Proxy.UnLock("Key").Result).IsTrue();
    }

    [Test]
    public void GetOutputIDs()
    {
        Check.That(this.Proxy!.GetOutputIDs().Result!.Count).Equals(3);
    }

    [Test]
    public void GetPolicies()
    {
        String _1 = "GetPoliciesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(this.Proxy!.SetPolicies(_2).Result).IsTrue();
        Check.That(this.Proxy!.GetPolicies().Result).IsEquivalentTo(_2);
        Check.That(this.Proxy!.GetPolicies().Result!.ToArray().First()).IsEqualTo(_1);
    }

    [Test]
    public void GetProcessID()
    {
        Check.That(this.Proxy!.GetProcessID().Result).HasAValue();
    }

    [Test]
    public void GetPurpose()
    {
        Check.That(this.Proxy!.GetPurpose().Result).IsEmpty();
    }

    [Test]
    public void GetSecurityDescriptor()
    {
        String _1 = "GetSecurityDescriptorExam";

        Check.That(this.Proxy!.SetSecurityDescriptor(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetSecurityDescriptor().Result).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(this.Proxy!.SetServiceVersion(_2).Result).IsTrue();
        Check.That(this.Proxy!.GetServiceVersion().Result!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetStatus()
    {
        Check.That(this.Proxy!.GetStatus().Result).IsEmpty();
    }

    [Test]
    public void GetStringID()
    {
        Check.That(this.Proxy!.GetStringID().Result).IsNull();
    }

    [Test]
    public void GetTags()
    {
        Check.That(this.Proxy!.GetTags().Result!.Count).IsEqualTo(2);
    }

    [Test]
    public void GetThreadID()
    {
        Check.That(this.Proxy!.GetThreadID().Result).HasAValue();
    }

    [Test]
    public void GetVersion()
    {
        Check.That(this.Proxy!.GetVersion().Result).HasSameValueAs(new Version());
    }

    [Test]
    public void LogEvent()
    {
        String _1 = "LogEventExam";
        String _2 = "Pass";

        Check.That(this.Proxy!.LogEvent(_1).Result).IsTrue();
        Check.That(this.Proxy.LogEvent(_2).Result).IsTrue();
        Check.That(this.Proxy.GetEventLogs().Result!.Keys.Count).Equals(2);
        Check.That(this.Proxy.GetEvent(0).Result).Equals(_1);
        Check.That(this.Proxy.GetEvent(1).Result).Equals(_2);
        Check.That(this.Proxy.ClearEventLogs().Result).IsTrue();
    }

    [Test]
    public void RemoveDataItem()
    {
        HashSet<DataItem>? _0 = this.Proxy!.GetDataItems().Result;

        Check.That(_0).HasSize(6);
        Check.That(this.Proxy.RemoveDataItem(null).Result).IsFalse();
        Check.That(this.Proxy.RemoveDataItem(_0!.First().GetID()).Result).IsTrue();
        Check.That(this.Proxy.GetDataItems().Result).HasSize(5);

        foreach(DataItem _1 in this.Proxy.GetDataItems().Result!)
        {
            Check.That(this.Proxy.RemoveDataItem(_1.GetID()).Result).IsTrue();
        }

        Check.That(this.Proxy.GetDataItems().Result).IsNull();
    }

    [Test]
    public void RemoveNote()
    {
        Check.That(this.Proxy!.RemoveNote(null).Result).IsFalse();
        Check.That(this.Proxy.GetNotes().Result!.Count).IsEqualTo(2);
        Check.That(this.Proxy.Lock("key").Result).IsTrue();
        Check.That(this.Proxy.RemoveNote(this.Proxy.GetNotes().Result!.First()).Result).IsFalse();
        Check.That(this.Proxy.UnLock("key").Result).IsTrue();
        Check.That(this.Proxy.RemoveNote(this.Proxy.GetNotes().Result!.First()).Result).IsTrue();
        Check.That(this.Proxy.GetNotes().Result!.Count).IsEqualTo(1);
    }

    [Test]
    public void RemoveOutput()
    {
        Check.That(this.Proxy!.GetOutputIDs().Result!.All(_=>this.Proxy.RemoveOutput(_).Result)).IsTrue();
    }

    [Test]
    public void RemoveStatus()
    {
        String _1 = "S_OK";
        String _2 = "E_FAIL";
        String _3 = "OP_23";
        String _4 = "OP_97";

        Check.That(this.Proxy!.GetStatus().Result).IsEmpty();
        Check.That(this.Proxy.SetStatus(_3,_2).Result).IsTrue();
        Check.That(this.Proxy.SetStatus(_4,_1).Result).IsTrue();
        Check.That(this.Proxy.GetStatus().Result?.Count).Equals(2);
        Check.That(this.Proxy.GetStatus().Result?[_3]).Equals(_2);
        Check.That(this.Proxy.RemoveStatus(_3).Result).IsTrue();
        Check.That(this.Proxy.GetStatus().Result?[_4]).Equals(_1);
        Check.That(this.Proxy.RemoveStatus(_4).Result).IsTrue();
        Check.That(this.Proxy.GetStatus().Result).IsNull();
    }

    [Test]
    public void RemoveTag()
    {
        Check.That(this.Proxy!.RemoveTag(null).Result).IsFalse();
        Check.That(this.Proxy.GetTags().Result!.Count).IsEqualTo(2);
        Check.That(this.Proxy.RemoveTag(this.Proxy.GetTags().Result!.First()).Result).IsTrue();
        Check.That(this.Proxy.GetTags().Result!.Count).IsEqualTo(1);
    }

    [Test]
    public void SetApplication()
    {
        String _1 = "SetApplicationExam";

        Check.That(this.Proxy!.SetApplication(null).Result).IsFalse();
        Check.That(this.Proxy!.SetApplication(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetApplication().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(this.Proxy!.SetApplicationVersion(_2).Result).IsTrue();
        Check.That(this.Proxy!.GetApplicationVersion().Result).IsEqualTo(_2);
    }

    [Test]
    public void SetCertificates()
    {

        Dictionary<String,String> _1 = new Dictionary<String,String>();
        _1.Add("Content 0","6131f9c300030000000f");
        _1.Add("Content 1","611e23c200030000000e");
        _1.Add("Extension ExtKey InnerKey1 DeeperKeyN 8","473d29a500030000000f");

        Check.That(this.Proxy!.SetCertificates(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetCertificates().Result).ContainsExactly(_1);
        Check.That(this.Proxy!.SetCertificates(new Dictionary<String,String>()).Result).IsTrue();
        Check.That(this.Proxy!.GetCertificates().Result).IsNull();
    }


    [Test]
    public void SetDistinguishedName()
    {
        String _1 = "CN=ServiceN,OU=BoardofDirectors,DC=Corporate,DC=BIZ";

        Check.That(this.Proxy!.SetDistinguishedName(null).Result).IsFalse();
        Check.That(this.Proxy!.SetDistinguishedName(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetDistinguishedName().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        String _1 = "SetDomainID";

        Check.That(this.Proxy!.SetDomainID(null).Result).IsFalse();
        Check.That(this.Proxy!.SetDomainID(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetDomainID().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetExtension()
    {
        String _1 = "SetExtensionExam";
        Dictionary<String,Object?> _2 = new();
        _2.Add("1",_1);

        Check.That(this.Proxy!.SetExtension(null).Result).IsFalse();
        Check.That(this.Proxy!.SetExtension(_2).Result).IsTrue();
        Check.That(this.Proxy!.GetExtension().Result!["1"]).IsEqualTo(_1);
    }

    [Test]
    public void SetGPS()
    {
        String _1 = "SetGPS";

        Check.That(this.Proxy!.SetGPS(null).Result).IsFalse();
        Check.That(this.Proxy!.SetGPS(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetGPS().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        Guid _1 = Guid.NewGuid();

        Check.That(this.Proxy!.SetID(null).Result).IsFalse();
        Check.That(this.Proxy!.SetID(_1).Result).IsTrue();
        Check.That(this.Proxy!.GetID().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetLocator()
    {
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(this.Proxy!.SetLocator(null).Result).IsFalse();
        Check.That(this.Proxy!.SetLocator(_2).Result).IsTrue();
        Check.That(this.Proxy!.GetLocator().Result!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void SetModified()
    {
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(this.Proxy!.SetModified(null).Result).IsFalse();
        Check.That(this.Proxy.SetModified(_1).Result).IsTrue();
        Check.That(this.Proxy.GetModified().Result).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        String _1 = "SetNameExam";

        Check.That(this.Proxy!.SetName(null).Result).IsFalse();
        Check.That(this.Proxy!.SetName(_1).Result).IsTrue();
        Check.That(this.Proxy.GetName().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetObjectives()
    {
        String _1 = "SetObjectivesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(this.Proxy!.SetObjectives(null).Result).IsFalse();
        Check.That(this.Proxy!.SetObjectives(_2).Result).IsTrue();
        Check.That(this.Proxy.GetObjectives().Result).Contains(_2);
        Check.That(this.Proxy.GetObjectives().Result!.ToArray().First()).IsEqualTo(_1);
    }

    [Test]
    public void SetPolicies()
    {
        String _1 = "SetPoliciesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(this.Proxy!.SetPolicies(null).Result).IsFalse();
        Check.That(this.Proxy!.SetPolicies(_2).Result).IsTrue();
        Check.That(this.Proxy.GetPolicies().Result).Contains(_2);
        Check.That(this.Proxy.GetPolicies().Result!.ToArray().First()).IsEqualTo(_1);
    }

    [Test]
    public void SetPurpose()
    {
        String _1 = "SetPurposeExam";

        Check.That(this.Proxy!.SetPurpose(null).Result).IsFalse();
        Check.That(this.Proxy!.SetPurpose(_1).Result).IsTrue();
        Check.That(this.Proxy.GetPurpose().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        String _1 = "SetSecurityDescriptorExam";

        Check.That(this.Proxy!.SetSecurityDescriptor(null).Result).IsFalse();
        Check.That(this.Proxy!.SetSecurityDescriptor(_1).Result).IsTrue();
        Check.That(this.Proxy.GetSecurityDescriptor().Result).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(this.Proxy!.SetServiceVersion(_2).Result).IsTrue();
        Check.That(this.Proxy.GetServiceVersion().Result!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetStatus()
    {
        String _1 = "S_OK";
        String _2 = "E_FAIL";
        String _3 = "OP_23";
        String _4 = "OP_97";

        Check.That(this.Proxy!.GetStatus().Result).IsNull();
        Check.That(this.Proxy.SetStatus(_3,_2).Result).IsTrue();
        Check.That(this.Proxy.SetStatus(_4,_1).Result).IsTrue();
        Check.That(this.Proxy.GetStatus().Result?.Count).Equals(2);
        Check.That(this.Proxy.GetStatus().Result?[_3]).Equals(_2);
        Check.That(this.Proxy.RemoveStatus(_3).Result).IsTrue();
        Check.That(this.Proxy.GetStatus().Result?[_4]).Equals(_1);
        Check.That(this.Proxy.RemoveStatus(_4).Result).IsTrue();
        Check.That(this.Proxy.GetStatus().Result).IsNull();
    }

    [Test]
    public void SetVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(this.Proxy!.SetVersion(_2).Result).IsTrue();
        Check.That(this.Proxy!.GetVersion().Result!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void UpdateInputs()
    {
        String _1 = "UpdateInputsExam"; String _3 = "Pass";
        Queue<Object> _2 = new Queue<Object>(); Queue<Object> _4 = new Queue<Object>();
        Queue<Object>? _5;

        _2.Enqueue(_1);

        Check.That(this.Proxy!.UpdateInputs(_2).Result).IsTrue();
        Check.That(this.Proxy.UpdateInputs(null).Result!).IsFalse();
        Check.That(this.Proxy.GetInputs().Result!).Contains(_2.ToArray());
        Check.That(this.Proxy.GetInputs().Result!.Peek()).IsEqualTo(_1);

        _4.Enqueue(_1); _4.Enqueue(_3);

        Check.That(this.Proxy.UpdateInputs(_4).Result).IsTrue();

        _5 = this.Proxy.GetInputs().Result;

        Check.That(_5!.Dequeue()).IsEqualTo(_1);
        Check.That(_5.Dequeue()).IsEqualTo(_3);
    }

}