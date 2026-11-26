namespace KusDepot.Data.Clients;

/**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/main/*'/>*/
public sealed class TransferBlockLimitExceededException : HttpRequestException
{
    /**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/constructor[@name="Constructor"]/*'/>*/
    public TransferBlockLimitExceededException(Guid sessionId , Int64 currentBlocks , Int64 requestedBlocks , Int64 maximumBlocks , String? detail)
        : base(detail ?? "The transfer exceeds the publication block limit.",null,HttpStatusCode.Conflict)
    {
        this.SessionId = sessionId;
        this.CurrentBlocks = currentBlocks;
        this.RequestedBlocks = requestedBlocks;
        this.MaximumBlocks = maximumBlocks;
    }

    /**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/property[@name="SessionId"]/*'/>*/
    public Guid SessionId { get; }

    /**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/property[@name="CurrentBlocks"]/*'/>*/
    public Int64 CurrentBlocks { get; }

    /**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/property[@name="RequestedBlocks"]/*'/>*/
    public Int64 RequestedBlocks { get; }

    /**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/property[@name="MaximumBlocks"]/*'/>*/
    public Int64 MaximumBlocks { get; }

    /**<include file='TransferBlockLimitExceededException.xml' path='TransferBlockLimitExceededException/class[@name="TransferBlockLimitExceededException"]/method[@name="FromResponse"]/*'/>*/
    internal static TransferBlockLimitExceededException? FromResponse(HttpStatusCode status , String? content)
    {
        if(status is not HttpStatusCode.Conflict || String.IsNullOrWhiteSpace(content)) { return null; }

        JsonDocument document;

        try { document = JsonDocument.Parse(content); } catch ( JsonException ) { return null; }

        using(document)
        {
            JsonElement root = document.RootElement;

            if(root.ValueKind is not JsonValueKind.Object || !root.TryGetProperty("type",out JsonElement type) || type.ValueKind is not JsonValueKind.String ||
                type.GetString() != PublicationBlockLimitProblemType ||
                !root.TryGetProperty("sessionId",out JsonElement session) || session.ValueKind is not JsonValueKind.String || !session.TryGetGuid(out Guid sessionid) ||
                !root.TryGetProperty("currentBlocks",out JsonElement current) || current.ValueKind is not JsonValueKind.Number || !current.TryGetInt64(out Int64 currentBlocks) ||
                !root.TryGetProperty("requestedBlocks",out JsonElement requested) || requested.ValueKind is not JsonValueKind.Number || !requested.TryGetInt64(out Int64 requestedBlocks) ||
                !root.TryGetProperty("maximumBlocks",out JsonElement maximum) || maximum.ValueKind is not JsonValueKind.Number || !maximum.TryGetInt64(out Int64 maximumBlocks)) { return null; }

            String? detail = root.TryGetProperty("detail",out JsonElement value) && value.ValueKind is JsonValueKind.String ? value.GetString() : null;

            return new TransferBlockLimitExceededException(sessionid,currentBlocks,requestedBlocks,maximumBlocks,detail);
        }
    }
}
