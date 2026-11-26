namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    ///<inheritdoc/>
    public async Task<DataControlResponse<PublishStreamTransferResponse>> PublishStreamTransfer(PublishStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<PublishStreamTransferRequest,PublishStreamTransferResponse>(HttpMethod.Post,"/PublishStreamTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,PublishStreamTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<StreamSegmentDownloadInfo?> GetStreamTransferSegment(GetStreamTransferSegmentRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpRequestMessage message = new(HttpMethod.Post,this.CreateRequestUri("/GetStreamTransferSegment"))
            {
                Content = JsonContent.Create(request)
            };

            if(String.IsNullOrEmpty(token) is false) { message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await this.Client.SendAsync(message,HttpCompletionOption.ResponseHeadersRead,cancel).ConfigureAwait(false);

            if(response.IsSuccessStatusCode is false)
            {
                Byte[] body = await response.Content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);

                return new()
                {
                    StatusCode = response.StatusCode,

                    Content = body.Length == 0 ? null : Encoding.UTF8.GetString(body)
                };
            }

            using MemoryStream payload = new();

            StreamSegmentDownloadInfo? streamed = await this.DownloadStreamTransferSegmentToStream(response,payload,cancel).ConfigureAwait(false); if(streamed is null) { return null; }

            return new()
            {
                StatusCode = streamed.StatusCode,

                Footer = streamed.Footer,

                Payload = payload.ToArray()
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetStreamTransferSegmentFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<GetStreamTransferStateResponse>> GetStreamTransferState(DataItemTransferIdentity identity , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<DataItemTransferIdentity,GetStreamTransferStateResponse>(HttpMethod.Post,"/GetStreamTransferState",identity,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetStreamTransferStateFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<GetTransferIntegrityEvidenceResponse>> GetTransferIntegrityEvidence(GetTransferIntegrityEvidenceRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<GetTransferIntegrityEvidenceRequest,GetTransferIntegrityEvidenceResponse>(HttpMethod.Post,"/GetTransferIntegrityEvidence",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTransferIntegrityEvidenceFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<OpenFollowStreamTransferResponse>> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<OpenFollowStreamTransferRequest,OpenFollowStreamTransferResponse>(HttpMethod.Post,"/OpenFollowStreamTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenFollowStreamTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<OpenStreamTransferResponse>> OpenStreamTransfer(OpenStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<OpenStreamTransferRequest,OpenStreamTransferResponse>(HttpMethod.Post,"/OpenStreamTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenStreamTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<PutStreamTransferSegmentClientResponse?> PutStreamTransferSegment(PutStreamTransferSegmentRequest request , Stream payload , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpContent c = new SegmentedTransferEnvelopeContent(JsonUtility.Serialize(request),payload,request.Length,this.UploadPayloadRateLimiter);

            using HttpRequestMessage message = new(HttpMethod.Put,this.CreateRequestUri("/PutStreamTransferSegment"))
            {
                Content = c
            };

            if(String.IsNullOrEmpty(token) is false) { message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await this.Client.SendAsync(message,cancel).ConfigureAwait(false);

            String? content = response.Content is null ? null : await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);

            PutStreamTransferSegmentResponse? parsed = response.IsSuccessStatusCode && !String.IsNullOrWhiteSpace(content)
                ? JsonUtility.Parse<PutStreamTransferSegmentResponse>(content)
                : null;

            return new()
            {
                StatusCode = response.StatusCode,
                Response = parsed,
                Content = content
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,PutStreamTransferSegmentFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<ReOpenFollowStreamTransferResponse>> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<ReOpenFollowStreamTransferRequest,ReOpenFollowStreamTransferResponse>(HttpMethod.Post,"/ReOpenFollowStreamTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenFollowStreamTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<ReOpenStreamTransferResponse>> ReOpenStreamTransfer(ReOpenStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<ReOpenStreamTransferRequest,ReOpenStreamTransferResponse>(HttpMethod.Post,"/ReOpenStreamTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenStreamTransferFail); throw; }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="DownloadStreamTransferSegmentToStreamResponse"]/*'/>*/
    private async Task<StreamSegmentDownloadInfo?> DownloadStreamTransferSegmentToStream(HttpResponseMessage response , Stream destination , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(response); ArgumentNullException.ThrowIfNull(destination);

        if(response.IsSuccessStatusCode is false)
        {
            Byte[] body = await response.Content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);

            return new()
            {
                StatusCode = response.StatusCode,

                Content = body.Length == 0 ? null : Encoding.UTF8.GetString(body)
            };
        }

        Stream stream = await response.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);

        await using (stream.ConfigureAwait(false))
        {
            TransferEnvelopeHeader? header = await TransferEnvelope.ReadHeaderAsync(stream,cancel).ConfigureAwait(false);

            if(!header.HasValue || header.Value.MetadataLength != 0 || header.Value.TrailerLength <= 0) { return null; }

            Stream payloadStream = TransferEnvelope.OpenPayloadStream(stream,header.Value,leaveopen:true);

            Stream? limitedPayloadStream = this.DownloadPayloadRateLimiter is AsyncByteRateLimiter limiter
                ? new RateLimitedReadStream(payloadStream,limiter,leaveopen:false)
                : null;

            Stream readPayload = limitedPayloadStream ?? payloadStream;

            await using (readPayload.ConfigureAwait(false))
            {
                HashingWriteStream hashing = new(destination,leaveopen:true);

                await using (hashing.ConfigureAwait(false))
                {
                    await readPayload.CopyToAsync(hashing,cancel).ConfigureAwait(false);

                    if(limitedPayloadStream is null) { await payloadStream.DisposeAsync().ConfigureAwait(false); }

                    await hashing.FlushAsync(cancel).ConfigureAwait(false);

                    ReadOnlyMemory<Byte> footerBytes = await TransferEnvelope.ReadTrailerBytesAsync(stream,header.Value,cancel).ConfigureAwait(false);

                    if(footerBytes.IsEmpty && header.Value.TrailerLength > 0) { return null; }

                    StreamTransferSegmentFooter? footer = JsonUtility.Deserialize<StreamTransferSegmentFooter>(footerBytes); if(footer is null) { return null; }

                    Byte[] payloadHash = hashing.GetHashAndReset();

                    if(footer.SegmentSHA512.AsSpan().SequenceEqual(payloadHash.AsSpan()) is false) { return null; }

                    return new()
                    {
                        StatusCode = response.StatusCode,

                        Footer = footer,
                    };
                }
            }
        }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="DownloadStreamTransferSegmentToStreamRequest"]/*'/>*/
    internal async Task<StreamSegmentDownloadInfo?> DownloadStreamTransferSegmentToStream(GetStreamTransferSegmentRequest request , Stream destination , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpRequestMessage message = new(HttpMethod.Post,this.CreateRequestUri("/GetStreamTransferSegment"))
            {
                Content = JsonContent.Create(request)
            };

            if(String.IsNullOrEmpty(token) is false) { message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await this.Client.SendAsync(message,HttpCompletionOption.ResponseHeadersRead,cancel).ConfigureAwait(false);

            return await this.DownloadStreamTransferSegmentToStream(response,destination,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DownloadStreamTransferSegmentToStreamFail); if(NoExceptions) { return null; } throw; }
    }
}