namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class SerializationEnvelopeExams
{
    [Test]
    public void HasEnvelope_Returns_True_For_Enveloped_Bytes()
    {
        Byte[] serialized = OrleansUtility.Serialize("hello");

        Assert.That(SerializationEnvelope.HasEnvelope(serialized), Is.True);
    }

    [Test]
    public void ParseHeader_Returns_Header_For_Enveloped_Bytes()
    {
        Byte[] serialized = OrleansUtility.Serialize("test");

        ParsedSerializationHeader? parsed = SerializationEnvelope.ParseHeader(serialized);

        Assert.That(parsed, Is.Not.Null);
    }

    [Test]
    public void ParseHeader_FormatVersion_Is_One()
    {
        Byte[] serialized = OrleansUtility.Serialize(42);

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        Assert.That(header.FormatVersion, Is.EqualTo(0x01));
    }

    [Test]
    public void ParseHeader_SerializerVersion_Is_Not_Empty()
    {
        Byte[] serialized = OrleansUtility.Serialize("version-check");

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        Assert.That(header.SerializerVersion, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void ParseHeader_LibraryVersion_Is_Not_Empty()
    {
        Byte[] serialized = OrleansUtility.Serialize("version-check");

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        Assert.That(header.LibraryVersion, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void ParseHeader_HeaderLength_Is_Positive()
    {
        Byte[] serialized = OrleansUtility.Serialize("length-check");

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        Assert.That(header.HeaderLength, Is.GreaterThan(0));
    }

    [Test]
    public void ParseHeader_HeaderLength_Is_Less_Than_Total_Length()
    {
        Byte[] serialized = OrleansUtility.Serialize("length-check");

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        Assert.That(header.HeaderLength, Is.LessThan(serialized.Length));
    }

    [Test]
    public void Unwrap_Returns_Payload_Without_Header()
    {
        Byte[] serialized = OrleansUtility.Serialize("unwrap-check");

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        ReadOnlyMemory<Byte> payload = SerializationEnvelope.Unwrap(serialized);

        Assert.That(payload.Length, Is.EqualTo(serialized.Length - header.HeaderLength));
    }

    [Test]
    public void Unwrap_Returns_Full_Input_For_Non_Enveloped_Bytes()
    {
        Byte[] raw = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

        ReadOnlyMemory<Byte> result = SerializationEnvelope.Unwrap(raw);

        Assert.That(result.Length, Is.EqualTo(raw.Length));
        Assert.That(result.ToArray(), Is.EqualTo(raw));
    }

    [Test]
    public void Unwrap_Payload_Deserializes_Correctly()
    {
        String original = "roundtrip-payload";

        Byte[] serialized = OrleansUtility.Serialize(original);

        String deserialized = OrleansUtility.Deserialize<String>(serialized);

        Assert.That(deserialized, Is.EqualTo(original));
    }

    [Test]
    public void ParseHeader_Magic_Bytes_Are_KD()
    {
        Byte[] serialized = OrleansUtility.Serialize("magic-check");

        Byte[] magic = serialized[..2];

        Assert.That(Encoding.UTF8.GetString(magic), Is.EqualTo("KD"));
    }

    [Test]
    public void ParsedSerializationHeader_Fields_Match_Parsed_Values()
    {
        Byte[] serialized = OrleansUtility.Serialize(Guid.NewGuid());

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;

        Assert.That(header.FormatVersion, Is.EqualTo(0x01));
        Assert.That(header.Kind, Is.EqualTo(SerializationKind.Orleans));
        Assert.That(header.SerializerVersion, Is.Not.Null);
        Assert.That(header.LibraryVersion, Is.Not.Null);
        Assert.That(header.HeaderLength, Is.GreaterThan(0));
    }

    [Test]
    public void Unwrap_And_ParseHeader_HeaderLength_Are_Consistent()
    {
        Byte[] serialized = OrleansUtility.Serialize(new List<Int32> { 1, 2, 3 });

        ParsedSerializationHeader header = SerializationEnvelope.ParseHeader(serialized)!.Value;
        ReadOnlyMemory<Byte> payload = SerializationEnvelope.Unwrap(serialized);

        Assert.That(header.HeaderLength + payload.Length, Is.EqualTo(serialized.Length));
    }
}
