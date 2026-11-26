namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    private async Task<Int32> SendMalformedTransferEnvelopeAsync(String path , Object request , Byte[] payload , Int64 declaredPayloadLength , String? token)
    {
        ArgumentNullException.ThrowIfNull(request); ArgumentNullException.ThrowIfNull(payload);

        using HttpClient client = new() { BaseAddress = new Uri(this.EndpointLocator!,UriKind.Absolute) };

        await using MemoryStream envelope = new();

        await TransferEnvelope.WriteAsync(envelope,JsonUtility.Serialize(request),new MemoryStream(payload,false),declaredPayloadLength);

        using ByteArrayContent content = new(envelope.ToArray());

        content.Headers.ContentType = new MediaTypeHeaderValue(TransferEnvelope.MediaType);

        using HttpRequestMessage message = new(HttpMethod.Put,new Uri(path,UriKind.Relative)) { Content = content };

        if(String.IsNullOrWhiteSpace(token) is false) { message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token); }

        using HttpResponseMessage response = await client.SendAsync(message);

        return (Int32)response.StatusCode;
    }
}
