namespace KusDepot.Exams;

public partial class JsonUtilityExam
{
    [Test]
    public void Parse_And_ToJsonString_Descriptor_Roundtrip()
    {
        var original = new Descriptor()
        {
            Application = "KusDepot",
            ApplicationVersion = "1.0.0",
            Artist = "Tester",
            BornOn = "2026-03-11T00:00:00+00:00",
            Commands = new HashSet<CommandDescriptor>()
            {
                new() { Type = "Create" , Specifications = "Create String" , RegisteredHandles = new List<String>() { "System.String" } }
            },
            ContentStreamed = false,
            DataType = "System.String",
            DistinguishedName = "CN=KusDepot",
            FILE = "descriptor.json",
            ID = Guid.NewGuid(),
            LiveStream = false,
            Modified = "2026-03-11T00:00:00+00:00",
            Name = "DescriptorName",
            Notes = new HashSet<String>() { "note-a" , "note-b" },
            ObjectType = typeof(Descriptor).FullName,
            Services = new HashSet<ToolDescriptor>()
            {
                new() { Type = "Service" , Specifications = "Service Description" , ExtendedData = new List<String>() { "System.String" } }
            },
            ServiceVersion = "1.0.1",
            Size = "256",
            Tags = new HashSet<String>() { "tag-a" , "tag-b" },
            Title = "DescriptorTitle",
            Version = "2.0.0",
            Year = "2026"
        };

        var json = JsonUtility.ToJsonString(original);
        var roundtrip = JsonUtility.Parse<Descriptor>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Application).IsEqualTo(original.Application);
        Check.That(roundtrip.ApplicationVersion).IsEqualTo(original.ApplicationVersion);
        Check.That(roundtrip.Artist).IsEqualTo(original.Artist);
        Check.That(roundtrip.BornOn).IsEqualTo(original.BornOn);
        Check.That(roundtrip.ContentStreamed).IsEqualTo(original.ContentStreamed);
        Check.That(roundtrip.DataType).IsEqualTo(original.DataType);
        Check.That(roundtrip.DistinguishedName).IsEqualTo(original.DistinguishedName);
        Check.That(roundtrip.FILE).IsEqualTo(original.FILE);
        Check.That(roundtrip.ID).IsEqualTo(original.ID);
        Check.That(roundtrip.LiveStream).IsEqualTo(original.LiveStream);
        Check.That(roundtrip.Modified).IsEqualTo(original.Modified);
        Check.That(roundtrip.Name).IsEqualTo(original.Name);
        Check.That(roundtrip.ObjectType).IsEqualTo(original.ObjectType);
        Check.That(roundtrip.ServiceVersion).IsEqualTo(original.ServiceVersion);
        Check.That(roundtrip.Size).IsEqualTo(original.Size);
        Check.That(roundtrip.Title).IsEqualTo(original.Title);
        Check.That(roundtrip.Version).IsEqualTo(original.Version);
        Check.That(roundtrip.Year).IsEqualTo(original.Year);
        Check.That(roundtrip.Commands).IsNotNull();
        Check.That(roundtrip.Commands!.Select(_ => $"{_.Type}|{_.Specifications}|{String.Join(',',_.RegisteredHandles ?? [])}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet())
            .IsEqualTo(original.Commands!.Select(_ => $"{_.Type}|{_.Specifications}|{String.Join(',',_.RegisteredHandles ?? [])}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet());
        Check.That(roundtrip.Services).IsNotNull();
        Check.That(roundtrip.Services!.Select(_ => $"{_.ID}|{_.Type}|{_.Specifications}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet())
            .IsEqualTo(original.Services!.Select(_ => $"{_.ID}|{_.Type}|{_.Specifications}|{String.Join(',',_.ExtendedData ?? [])}").ToHashSet());
        Check.That(roundtrip.Notes).IsNotNull();
        Check.That(roundtrip.Notes!.Count).IsEqualTo(original.Notes!.Count);
        Check.That(roundtrip.Tags).IsNotNull();
        Check.That(roundtrip.Tags!.Count).IsEqualTo(original.Tags!.Count);
    }

    [Test]
    public void Serialize_And_Deserialize_Descriptor_Roundtrip()
    {
        var original = new Descriptor()
        {
            Application = "KusDepot",
            ApplicationVersion = "3.0.0",
            ID = Guid.NewGuid(),
            Name = "SerializeDescriptor",
            ObjectType = typeof(Descriptor).FullName,
            Title = "Serialized Descriptor"
        };

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<Descriptor>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Application).IsEqualTo(original.Application);
        Check.That(roundtrip.ApplicationVersion).IsEqualTo(original.ApplicationVersion);
        Check.That(roundtrip.ID).IsEqualTo(original.ID);
        Check.That(roundtrip.Name).IsEqualTo(original.Name);
        Check.That(roundtrip.ObjectType).IsEqualTo(original.ObjectType);
        Check.That(roundtrip.Title).IsEqualTo(original.Title);
    }

    [Test]
    public void Parse_And_ToJsonString_CommandDescriptor_Roundtrip()
    {
        var original = new CommandDescriptor()
        {
            Type = typeof(String).FullName,
            Specifications = "Command specification",
            ExtendedData = new List<String>() { "ext-a" , "ext-b" },
            RegisteredHandles = new List<String>() { "HandleA" , "HandleB" }
        };

        var json = JsonUtility.ToJsonString(original);
        var roundtrip = JsonUtility.Parse<CommandDescriptor>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Type).IsEqualTo(original.Type);
        Check.That(roundtrip.Specifications).IsEqualTo(original.Specifications);
        Check.That(roundtrip.ExtendedData).IsNotNull();
        Check.That(roundtrip.ExtendedData!).ContainsExactly(original.ExtendedData!);
        Check.That(roundtrip.RegisteredHandles).IsNotNull();
        Check.That(roundtrip.RegisteredHandles!).ContainsExactly(original.RegisteredHandles!);
    }

    [Test]
    public void Serialize_And_Deserialize_CommandDescriptor_Roundtrip()
    {
        var original = new CommandDescriptor()
        {
            Type = typeof(Int32).FullName,
            Specifications = "Binary op",
            ExtendedData = new List<String>() { "x" },
            RegisteredHandles = new List<String>() { "Add" }
        };

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<CommandDescriptor>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Type).IsEqualTo(original.Type);
        Check.That(roundtrip.Specifications).IsEqualTo(original.Specifications);
        Check.That(roundtrip.ExtendedData).IsNotNull();
        Check.That(roundtrip.ExtendedData!).ContainsExactly(original.ExtendedData!);
        Check.That(roundtrip.RegisteredHandles).IsNotNull();
        Check.That(roundtrip.RegisteredHandles!).ContainsExactly(original.RegisteredHandles!);
    }

    [Test]
    public void Parse_And_ToJsonString_ToolDescriptor_Roundtrip()
    {
        var original = new ToolDescriptor()
        {
            ID = Guid.NewGuid(),
            Type = typeof(StringBuilder).FullName,
            Specifications = "Tool specification",
            ExtendedData = new List<String>() { "tool-a" , "tool-b" }
        };

        var json = JsonUtility.ToJsonString(original);
        var roundtrip = JsonUtility.Parse<ToolDescriptor>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.ID).IsEqualTo(original.ID);
        Check.That(roundtrip.Type).IsEqualTo(original.Type);
        Check.That(roundtrip.Specifications).IsEqualTo(original.Specifications);
        Check.That(roundtrip.ExtendedData).IsNotNull();
        Check.That(roundtrip.ExtendedData!).ContainsExactly(original.ExtendedData!);
    }

    [Test]
    public void Serialize_And_Deserialize_ToolDescriptor_Roundtrip()
    {
        var original = new ToolDescriptor()
        {
            ID = Guid.NewGuid(),
            Type = typeof(Guid).FullName,
            Specifications = "Guid tool",
            ExtendedData = new List<String>() { "guid" }
        };

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<ToolDescriptor>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.ID).IsEqualTo(original.ID);
        Check.That(roundtrip.Type).IsEqualTo(original.Type);
        Check.That(roundtrip.Specifications).IsEqualTo(original.Specifications);
        Check.That(roundtrip.ExtendedData).IsNotNull();
        Check.That(roundtrip.ExtendedData!).ContainsExactly(original.ExtendedData!);
    }
}
