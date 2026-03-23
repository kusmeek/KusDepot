namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public partial class JsonUtilityExam
{
    private static Byte[] Bytes(Byte seed)
    {
        var data = new Byte[32];
        for(Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return data;
    }

    [Test]
    public void Parse_And_ToJsonString_AccessRequest_Roundtrip()
    {
        var original = new ManagementRequest(Guid.NewGuid().ToString());

        var json = JsonUtility.ToJsonString((AccessRequest)original, typeof(AccessRequest));
        var polymorphic = AccessRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ManagementRequest\"");
        Check.That(polymorphic).IsInstanceOf<ManagementRequest>();
        Check.That(((ManagementRequest)polymorphic!).Data).IsEqualTo(original.Data);
    }

    [Test]
    public void Parse_And_ToJsonString_StandardRequest_Roundtrip()
    {
        var original = new StandardRequest("standard-request");

        var json = JsonUtility.ToJsonString((AccessRequest)original, typeof(AccessRequest));
        var polymorphic = AccessRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"StandardRequest\"");
        Check.That(polymorphic).IsInstanceOf<StandardRequest>();
        Check.That(((StandardRequest)polymorphic!).Data).IsEqualTo(original.Data);
    }

    [Test]
    public void Parse_And_ToJsonString_ServiceRequest_Roundtrip()
    {
        var original = new ServiceRequest(null,"service-request");

        var json = JsonUtility.ToJsonString((AccessRequest)original, typeof(AccessRequest));
        var polymorphic = AccessRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ServiceRequest\"");
        Check.That(polymorphic).IsInstanceOf<ServiceRequest>();
        Check.That(((ServiceRequest)polymorphic!).Data).IsEqualTo(original.Data);
        Check.That(((ServiceRequest)polymorphic!).Tool).IsNull();
    }

    [Test]
    public void Serialize_And_Deserialize_ManagementRequest_Roundtrip()
    {
        var original = new ManagementRequest("management-request");

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<ManagementRequest>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Data).IsEqualTo(original.Data);
    }

    [Test]
    public void Parse_And_ToJsonString_SecurityKey_Roundtrip()
    {
        var original = new ServiceKey(Bytes(4), Guid.NewGuid());

        var json = JsonUtility.ToJsonString(original);
        var roundtrip = JsonUtility.Parse<ServiceKey>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void Serialize_And_Deserialize_SecurityKey_Roundtrip()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var original = new ManagerKey(cert!, Guid.NewGuid());

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<ManagerKey>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Thumbprint).IsEqualTo(original.Thumbprint);
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void Runtime_Type_ToJsonString_Preserves_Derived_Key_Members()
    {
        var original = new TokenKey(new Byte[] { 1, 2, 3 }, Guid.NewGuid(), TokenKeyType.Spnego);

        var json = JsonUtility.ToJsonString((Object)original, typeof(TokenKey));
        var roundtrip = TokenKey.Parse(json);

        Check.That(json).Contains("\"TokenType\":10");
        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.TokenType).IsEqualTo(TokenKeyType.Spnego);
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void Runtime_Type_Serialize_Preserves_Derived_Key_Members()
    {
        var original = new TokenKey(new Byte[] { 9, 4, 9 }, Guid.NewGuid(), TokenKeyType.Jwt);

        var bytes = JsonUtility.Serialize((Object)original, typeof(TokenKey));
        var roundtrip = TokenKey.Parse(Encoding.UTF8.GetString(bytes));

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.TokenType).IsEqualTo(TokenKeyType.Jwt);
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void ToFile_And_FromFile_AccessRequest_Roundtrip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"JsonUtilityExam-Request-{Guid.NewGuid():N}.json");
        var original = new StandardRequest("json-utility-file");

        try
        {
            var written = JsonUtility.ToFile(path, (AccessRequest)original, typeof(AccessRequest));
            var polymorphic = AccessRequest.Parse(File.ReadAllText(path));

            Check.That(written).IsTrue();
            Check.That(polymorphic).IsInstanceOf<StandardRequest>();
            Check.That(((StandardRequest)polymorphic!).Data).IsEqualTo(original.Data);
        }
        finally
        {
            if(File.Exists(path)) { File.Delete(path); }
        }
    }

    [Test]
    public void ToFile_And_FromFile_SecurityKey_Roundtrip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"JsonUtilityExam-Key-{Guid.NewGuid():N}.json");
        var original = new HostKey(Bytes(7), Guid.NewGuid());

        try
        {
            var written = JsonUtility.ToFile(path, (Object)original, typeof(HostKey));
            var roundtrip = JsonUtility.FromFile<HostKey>(path);

            Check.That(written).IsTrue();
            Check.That(roundtrip).IsNotNull();
            Check.That(roundtrip).IsEqualTo(original);
        }
        finally
        {
            if(File.Exists(path)) { File.Delete(path); }
        }
    }

    [Test]
    public void Parse_And_ToJsonString_KusDepotCab_Roundtrip()
    {
        var original = new KusDepotCab
        {
            Type = typeof(StandardRequest).AssemblyQualifiedName,
            Data = JsonUtility.ToJsonString(new StandardRequest("cab-request"),typeof(StandardRequest)),
            Manifest = new Dictionary<String,String>
            {
                ["Type"] = nameof(StandardRequest)
            },
            Cargo = new Dictionary<String,KusDepotCab>
            {
                ["child"] = new KusDepotCab
                {
                    Type = typeof(ServiceRequest).AssemblyQualifiedName,
                    Data = JsonUtility.ToJsonString(new ServiceRequest(null,"child-request"),typeof(ServiceRequest))
                }
            }
        };

        var json = JsonUtility.ToJsonString(original,typeof(KusDepotCab));
        var roundtrip = KusDepotCab.Parse(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Type).IsEqualTo(original.Type);
        Check.That(roundtrip.Data).IsEqualTo(original.Data);
        Check.That(roundtrip.Manifest).IsNotNull();
        Check.That(roundtrip.Manifest!["Type"]).IsEqualTo(nameof(StandardRequest));
        Check.That(roundtrip.Cargo).IsNotNull();
        Check.That(roundtrip.Cargo!.ContainsKey("child")).IsTrue();
        Check.That(roundtrip.Cargo["child"].Type).IsEqualTo(typeof(ServiceRequest).AssemblyQualifiedName);
    }
}