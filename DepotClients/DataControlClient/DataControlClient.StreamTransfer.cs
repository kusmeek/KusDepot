namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    ///<inheritdoc/>
    public async Task<RestResponse<CompleteStreamTransferResponse>> CompleteStreamTransfer(CompleteStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/CompleteStreamTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<CompleteStreamTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompleteStreamTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<StreamSegmentDownloadInfo?> GetStreamTransferSegment(GetStreamTransferSegmentRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h);

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await hc.PostAsJsonAsync(new Uri(this.EndPoint+"/GetStreamTransferSegment"),request,cancel).ConfigureAwait(false);

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
    public async Task<RestResponse<GetStreamTransferStateResponse>> GetStreamTransferState(DataItemTransferIdentity identity , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/GetStreamTransferState",Method.Post).AddJsonBody(identity); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<GetStreamTransferStateResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetStreamTransferStateFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<OpenFollowStreamTransferResponse>> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/OpenFollowStreamTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<OpenFollowStreamTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenFollowStreamTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<OpenStreamTransferResponse>> OpenStreamTransfer(OpenStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/OpenStreamTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<OpenStreamTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenStreamTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<PutStreamTransferSegmentClientResponse?> PutStreamTransferSegment(PutStreamTransferSegmentRequest request , Stream payload , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            token = this.ResolveToken(token);

            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h);

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpContent c = new SegmentedTransferEnvelopeContent(JsonUtility.Serialize(request),payload,request.Length);

            using HttpResponseMessage response = await hc.PutAsync(new Uri(this.EndPoint+"/PutStreamTransferSegment"),c,cancel).ConfigureAwait(false);

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
    public async Task<RestResponse<ReOpenFollowStreamTransferResponse>> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/ReOpenFollowStreamTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<ReOpenFollowStreamTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenFollowStreamTransferFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<ReOpenStreamTransferResponse>> ReOpenStreamTransfer(ReOpenStreamTransferRequest request , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/ReOpenStreamTransfer",Method.Post).AddJsonBody(request); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<ReOpenStreamTransferResponse>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ReOpenStreamTransferFail); if(NoExceptions) { return null!; } throw; }
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

            await using (payloadStream.ConfigureAwait(false))
            {
                HashingWriteStream hashing = new(destination,leaveopen:true);

                await using (hashing.ConfigureAwait(false))
                {
                    await payloadStream.CopyToAsync(hashing,cancel).ConfigureAwait(false);

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

            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h);

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            using HttpResponseMessage response = await hc.PostAsJsonAsync(new Uri(this.EndPoint+"/GetStreamTransferSegment"),request,cancel).ConfigureAwait(false);

            return await this.DownloadStreamTransferSegmentToStream(response,destination,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DownloadStreamTransferSegmentToStreamFail); if(NoExceptions) { return null; } throw; }
    }
}