namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    ///<inheritdoc/>
    public async Task<RestResponse<AbortTransferResponse>> AbortTransfer(AbortTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/AbortTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<AbortTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AbortTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<CommitUploadTransferResponse>> CommitUploadTransfer(CommitUploadTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/CommitUploadTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<CommitUploadTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CommitUploadTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<SegmentDownloadInfo?> GetTransferSegment(GetTransferSegmentRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h);

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await hc.PostAsJsonAsync(new Uri(this.EndPoint+"/GetTransferSegment"),request,cancel).ConfigureAwait(false);

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
    public async Task<RestResponse<GetTransferStateResponse>> GetTransferState(DataItemTransferIdentity identity , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/GetTransferState",Method.Post).AddJsonBody(identity); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<GetTransferStateResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTransferStateFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<OpenGetTransferResponse>> OpenGetTransfer(OpenGetTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/OpenGetTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<OpenGetTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenGetTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<OpenUploadTransferResponse>> OpenUploadTransfer(OpenUploadTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/OpenUploadTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<OpenUploadTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenUploadTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<PutTransferSegmentClientResponse?> PutTransferSegment(PutTransferSegmentRequest request , Stream payload , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h);

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpContent c = new SegmentedTransferEnvelopeContent(JsonUtility.Serialize(request),payload,request.Length);

            using HttpResponseMessage response = await hc.PutAsync(new Uri(this.EndPoint+"/PutTransferSegment"),c,cancel).ConfigureAwait(false);

            String? content = response.Content is null ? null : await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);

            PutTransferSegmentResponse? parsed = response.IsSuccessStatusCode && !String.IsNullOrWhiteSpace(content)
                ? JsonUtility.Parse<PutTransferSegmentResponse>(content)
                : null;

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
    public async Task<RestResponse<RemoveTransferResponse>> RemoveTransfer(RemoveTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/RemoveTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<RemoveTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<ReOpenGetTransferResponse>> ReOpenGetTransfer(ReOpenGetTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/ReOpenGetTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<ReOpenGetTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenGetTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<ReOpenUploadTransferResponse>> ReOpenUploadTransfer(ReOpenUploadTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/ReOpenUploadTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<ReOpenUploadTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenUploadTransferFail); if(NoExceptions) { return null!; } throw; }
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

            await using (payloadStream.ConfigureAwait(false))
            {
                HashingWriteStream hashing = new(destination,leaveopen:true);

                await using (hashing.ConfigureAwait(false))
                {
                    await payloadStream.CopyToAsync(hashing,cancel).ConfigureAwait(false);

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

            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h);

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await hc.PostAsJsonAsync(new Uri(this.EndPoint+"/GetTransferSegment"),request,cancel).ConfigureAwait(false);

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

        /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="SegmentedTransferEnvelopeContent"]/constructor[@name="Constructor"]/*'/>*/ 
        public SegmentedTransferEnvelopeContent(Byte[] metadatabytes , Stream payload , Int64 payloadlength)
        {
            this.MetadataBytes = metadatabytes; this.Payload = payload; this.PayloadLength = payloadlength;

            this.Headers.ContentType = new MediaTypeHeaderValue(TransferEnvelope.MediaType);
        }

        ///<inheritdoc/>
        protected override async Task SerializeToStreamAsync(Stream stream , TransportContext? context)
        {
            if(this.Payload.CanSeek) { this.Payload.Position = 0; }

            await TransferEnvelope.WriteAsync(stream,this.MetadataBytes,this.Payload,this.PayloadLength).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        protected override Boolean TryComputeLength(out Int64 length)
        {
            length = TransferEnvelope.ComputeEnvelopeLength(this.MetadataBytes.Length,this.PayloadLength);

            return true;
        }
    }
}