namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    ///<inheritdoc/>
    public async Task<DataControlResponse<AbortTransferResponse>> AbortTransfer(AbortTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<AbortTransferRequest,AbortTransferResponse>(HttpMethod.Post,"/AbortTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AbortTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<CloseTransferSessionResponse>> CloseTransferSession(CloseTransferSessionRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<CloseTransferSessionRequest,CloseTransferSessionResponse>(HttpMethod.Post,"/CloseTransferSession",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception error ) { KusDepotLog.Error(error,CloseTransferSessionFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<PublishSegmentedTransferResponse>> PublishSegmentedTransfer(PublishSegmentedTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<PublishSegmentedTransferRequest,PublishSegmentedTransferResponse>(HttpMethod.Post,"/PublishSegmentedTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,PublishSegmentedTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<SegmentDownloadInfo?> GetTransferSegment(GetTransferSegmentRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpRequestMessage message = new(HttpMethod.Post,this.CreateRequestUri("/GetTransferSegment"))
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

            SegmentDownloadInfo? streamed = await this.DownloadTransferSegmentToStream(response,payload,cancel).ConfigureAwait(false); if(streamed is null) { return null; }

            return new()
            {
                StatusCode = streamed.StatusCode,

                Footer = streamed.Footer,

                Payload = payload.ToArray()
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTransferSegmentFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<GetTransferStateResponse>> GetTransferState(DataItemTransferIdentity identity , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<DataItemTransferIdentity,GetTransferStateResponse>(HttpMethod.Post,"/GetTransferState",identity,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTransferStateFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<OpenGetTransferResponse>> OpenGetTransfer(OpenGetTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<OpenGetTransferRequest,OpenGetTransferResponse>(HttpMethod.Post,"/OpenGetTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenGetTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<OpenUploadTransferResponse>> OpenUploadTransfer(OpenUploadTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<OpenUploadTransferRequest,OpenUploadTransferResponse>(HttpMethod.Post,"/OpenUploadTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenUploadTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<PutTransferSegmentClientResponse?> PutTransferSegment(PutTransferSegmentRequest request , Stream payload , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpContent c = new SegmentedTransferEnvelopeContent(JsonUtility.Serialize(request),payload,request.Length,this.UploadPayloadRateLimiter);

            using HttpRequestMessage message = new(HttpMethod.Put,this.CreateRequestUri("/PutTransferSegment"))
            {
                Content = c
            };

            if(String.IsNullOrEmpty(token) is false) { message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await this.Client.SendAsync(message,cancel).ConfigureAwait(false);

            String? content = response.Content is null ? null : await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);

            PutTransferSegmentResponse? parsed = response.IsSuccessStatusCode && PutTransferSegmentResponse.TryParseWithRequiredFields(content,out PutTransferSegmentResponse? result) ? result : null;

            return new()
            {
                StatusCode = response.StatusCode,

                Response = parsed,

                Content = content
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,PutTransferSegmentFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<RemoveTransferResponse>> RemoveTransfer(RemoveTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<RemoveTransferRequest,RemoveTransferResponse>(HttpMethod.Post,"/RemoveTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<ReOpenGetTransferResponse>> ReOpenGetTransfer(ReOpenGetTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<ReOpenGetTransferRequest,ReOpenGetTransferResponse>(HttpMethod.Post,"/ReOpenGetTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenGetTransferFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<ReOpenUploadTransferResponse>> ReOpenUploadTransfer(ReOpenUploadTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<ReOpenUploadTransferRequest,ReOpenUploadTransferResponse>(HttpMethod.Post,"/ReOpenUploadTransfer",request,ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenUploadTransferFail); throw; }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="DownloadTransferSegmentToStreamResponse"]/*'/>*/
    private async Task<SegmentDownloadInfo?> DownloadTransferSegmentToStream(HttpResponseMessage response , Stream destination , CancellationToken cancel)
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

                    await hashing.FlushAsync(cancel).ConfigureAwait(false);

                    ReadOnlyMemory<Byte> footerBytes = await TransferEnvelope.ReadTrailerBytesAsync(stream,header.Value,cancel).ConfigureAwait(false);

                    if(footerBytes.IsEmpty && header.Value.TrailerLength > 0) { return null; }

                    TransferSegmentFooter? footer = JsonUtility.Deserialize<TransferSegmentFooter>(footerBytes); if(footer is null) { return null; }

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

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="DownloadTransferSegmentToStreamRequest"]/*'/>*/
    internal async Task<SegmentDownloadInfo?> DownloadTransferSegmentToStream(GetTransferSegmentRequest request , Stream destination , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpRequestMessage message = new(HttpMethod.Post,this.CreateRequestUri("/GetTransferSegment"))
            {
                Content = JsonContent.Create(request)
            };

            if(String.IsNullOrEmpty(token) is false) { message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await this.Client.SendAsync(message,HttpCompletionOption.ResponseHeadersRead,cancel).ConfigureAwait(false);

            return await this.DownloadTransferSegmentToStream(response,destination,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DownloadTransferSegmentToStreamFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/main/*'/>*/
    private sealed class SegmentedTransferEnvelopeContent : HttpContent
    {
        /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/field[@name="MetadataBytes"]/*'/>*/
        private readonly Byte[] MetadataBytes;

        /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/field[@name="Payload"]/*'/>*/
        private readonly Stream Payload;

        /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/field[@name="PayloadLength"]/*'/>*/
        private readonly Int64 PayloadLength;

        /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/field[@name="UploadPayloadRateLimiter"]/*'/>*/
        private readonly AsyncByteRateLimiter? UploadPayloadRateLimiter;

        /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/constructor[@name="Constructor"]/*'/>*/ 
        public SegmentedTransferEnvelopeContent(Byte[] metadatabytes , Stream payload , Int64 payloadlength , AsyncByteRateLimiter? uploadpayloadratelimiter)
        {
            this.MetadataBytes = metadatabytes; this.Payload = payload; this.PayloadLength = payloadlength; this.UploadPayloadRateLimiter = uploadpayloadratelimiter;

            this.Headers.ContentType = new MediaTypeHeaderValue(TransferEnvelope.MediaType);
        }

        ///<inheritdoc/>
        protected override async Task SerializeToStreamAsync(Stream stream , TransportContext? context)
        {
            if(this.Payload.CanSeek) { this.Payload.Position = 0; }

            if(this.UploadPayloadRateLimiter is AsyncByteRateLimiter limiter)
            {
                using RateLimitedReadStream payload = new(this.Payload,limiter,leaveopen:true);

                await TransferEnvelope.WriteAsync(stream,this.MetadataBytes,payload,this.PayloadLength).ConfigureAwait(false);
            }
            else
            {
                await TransferEnvelope.WriteAsync(stream,this.MetadataBytes,this.Payload,this.PayloadLength).ConfigureAwait(false);
            }
        }

        ///<inheritdoc/>
        protected override async Task SerializeToStreamAsync(Stream stream , TransportContext? context , CancellationToken cancellationToken)
        {
            if(this.Payload.CanSeek) { this.Payload.Position = 0; }

            if(this.UploadPayloadRateLimiter is AsyncByteRateLimiter limiter)
            {
                using RateLimitedReadStream payload = new(this.Payload,limiter,leaveopen:true);

                await TransferEnvelope.WriteAsync(stream,this.MetadataBytes,payload,this.PayloadLength,cancel:cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await TransferEnvelope.WriteAsync(stream,this.MetadataBytes,this.Payload,this.PayloadLength,cancel:cancellationToken).ConfigureAwait(false);
            }
        }

        ///<inheritdoc/>
        protected override Boolean TryComputeLength(out Int64 length)
        {
            length = TransferEnvelope.ComputeEnvelopeLength(this.MetadataBytes.Length,this.PayloadLength);

            return true;
        }
    }
}