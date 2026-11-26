namespace DataPodServices.DataControl;

internal static class DataControlTransferSnapshot
{
    public static String SanitizeManifest(String json)
    {
        return Sanitize(json,"ObjectPayload");
    }

    public static String SanitizeJournal(String json)
    {
        return Sanitize(json,"FinalObjectPayload");
    }

    private static String Sanitize(String json , String payloadProperty)
    {
        if(String.IsNullOrWhiteSpace(json)) { throw new ArgumentException("A transfer snapshot is required.",nameof(json)); }

        JsonNode? parsed = JsonNode.Parse(json);

        if(parsed is not JsonObject snapshot) { throw new JsonException("A transfer snapshot must contain a JSON object."); }

        snapshot.Remove(payloadProperty);

        return snapshot.ToJsonString();
    }
}