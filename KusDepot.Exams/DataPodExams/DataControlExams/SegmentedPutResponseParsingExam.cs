using System.Text.Json.Nodes;

namespace KusDepot.DataPodExams;

[TestFixture]
internal sealed class SegmentedPutResponseParsingExam
{
    [Test]
    public void CompleteResponseParsesAndPreservesFields()
    {
        PutTransferSegmentResponse expected = CreateResponse();
        Boolean parsed = PutTransferSegmentResponse.TryParseWithRequiredFields(JsonUtility.ToJsonString(expected),out PutTransferSegmentResponse? actual);

        Check.That(parsed).IsTrue();
        Check.That(actual).IsNotNull();
        Check.That(actual).IsEqualTo(expected);
    }

    [TestCase("Accepted")]
    [TestCase("CompletedCoverage")]
    [TestCase("ItemID")]
    [TestCase("Range")]
    [TestCase("Replayed")]
    [TestCase("SessionID")]
    [TestCase("StateVersion")]
    [TestCase("Updated")]
    public void MissingResponseFieldIsRejected(String field)
    {
        JsonObject json = CreateJson();
        json.Remove(field);

        AssertRejected(json.ToJsonString());
    }

    [TestCase("Offset")]
    [TestCase("Length")]
    public void MissingRangeFieldIsRejected(String field)
    {
        JsonObject json = CreateJson();
        json["Range"]!.AsObject().Remove(field);

        AssertRejected(json.ToJsonString());
    }

    [Test]
    public void NullTimestampIsPreserved()
    {
        PutTransferSegmentResponse response = CreateResponse() with { Updated = null };

        Check.That(PutTransferSegmentResponse.TryParseWithRequiredFields(JsonUtility.ToJsonString(response),out PutTransferSegmentResponse? parsed)).IsTrue();
        Check.That(parsed).IsEqualTo(response);
    }

    [TestCase(null)]
    [TestCase("null")]
    [TestCase("")]
    [TestCase("{invalid")]
    [TestCase("[]")]
    public void InvalidContentIsRejected(String? content)
    {
        AssertRejected(content);
    }

    [TestCase("null-range")]
    [TestCase("wrong-type")]
    [TestCase("unknown-response")]
    [TestCase("unknown-range")]
    [TestCase("duplicate-response")]
    [TestCase("duplicate-range")]
    public void InvalidResponseIsRejected(String scenario)
    {
        JsonObject json = CreateJson();
        switch(scenario)
        {
            case "null-range": json["Range"] = null; break;
            case "wrong-type": json["Accepted"] = "true"; break;
            case "unknown-response": json["Unexpected"] = true; break;
            case "unknown-range": json["Range"]!.AsObject()["Unexpected"] = true; break;
        }
        String content = json.ToJsonString();
        if(scenario == "duplicate-response") { content = content.Insert(1,"\"Accepted\":true,"); }
        if(scenario == "duplicate-range") { content = content.Replace("\"Offset\":0","\"Offset\":0,\"Offset\":0",StringComparison.Ordinal); }

        AssertRejected(content);
    }

    private static void AssertRejected(String? content)
    {
        Check.That(PutTransferSegmentResponse.TryParseWithRequiredFields(content,out PutTransferSegmentResponse? response)).IsFalse();
        Check.That(response).IsNull();
    }

    [Test]
    public void GeneralRangeParsingDoesNotRequireOffset()
    {
        DataItemTransferRange? range = JsonUtility.Parse<DataItemTransferRange>("{\"Length\":8}");

        Check.That(range).IsNotNull();
        Check.That(range!.Offset).IsEqualTo(0L);
        Check.That(range.Length).IsEqualTo(8L);
    }

    private static PutTransferSegmentResponse CreateResponse()
    {
        return new()
        {
            Accepted = true,
            CompletedCoverage = false,
            ItemID = Guid.NewGuid(),
            Range = new DataItemTransferRange { Offset = 0 , Length = 8 },
            Replayed = false,
            SessionID = Guid.NewGuid(),
            StateVersion = 1,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    private static JsonObject CreateJson() { return JsonNode.Parse(JsonUtility.ToJsonString(CreateResponse()))!.AsObject(); }
}
