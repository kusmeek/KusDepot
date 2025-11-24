namespace KusDepot.Data.Clients;

/**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/main/*'/>*/
public sealed class DataControlClient : IDataControlClient , IDisposable
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Client"]/*'/>*/
    private readonly RestClient Client;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Auth"]/*'/>*/
    private readonly JwtAuthenticator Auth;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/property[@name="EndPoint"]/*'/>*/
    public String EndPoint { private get; init;}

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2 Certificate { private get; init;}

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/constructor[@name="Constructor"]/*'/>*/
    public DataControlClient(String endpoint , X509Certificate2 certificate , String? token = null)
    {
        this.Auth = new JwtAuthenticator(token ?? "null"); this.Client = new(new RestClientOptions(endpoint){ Authenticator = this.Auth , ClientCertificates = new(){certificate} }); this.EndPoint = endpoint; this.Certificate = certificate;
    }

    ///<inheritdoc/>
    public Task<RestResponse<Guid?>> Delete(Guid? id , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest($"/Delete/{id}",Method.Delete); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<Guid?>(_,cancel);
    }

    ///<inheritdoc/>
    public Task<RestResponse<DataControlDownload?>> Get(Guid? id , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest($"/Get/{id}",Method.Get); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<DataControlDownload?>(_,cancel);
    }

    ///<inheritdoc/>
    public async Task<DownloadInfo?> GetStream(Guid? id , String? filepath = null , String? itempath = null , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            if(id.HasValue is false) { return null; } if(File.Exists(filepath) || File.Exists(itempath)) { return null; } filepath ??= Path.Combine(Path.GetTempPath(),id.ToString()!);

            using FileStream f = new(filepath,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None,StreamBufferSize,FileOptions.Asynchronous|FileOptions.SequentialScan);

            RestRequest _ = new RestRequest($"/GetStream/{id}",Method.Get); if(String.IsNullOrEmpty(token) is false) { this.Auth.SetBearerToken(token); }

            RestResponse r = await this.Client.ExecuteAsync(_,cancel).ConfigureAwait(false);

            String? dh = r.Headers?.FirstOrDefault(h => Equals(h.Name,"DataControlDownload"))?.Value?.ToString(); if(String.IsNullOrEmpty(dh)) { return null; }

            DataControlDownload? d = DataControlDownload.Parse(dh,null); if(d is null) { return null; }

            if(String.Equals(d.ObjectSHA512,SHA512.HashData(d.Object.ToByteArrayFromBase64()!).ToBase64FromByteArray(),StringComparison.Ordinal) is false) { return null; }

            if(r.IsSuccessful)
            {
                using Stream? s = await this.Client.DownloadStreamAsync(_,cancel).ConfigureAwait(false); await s!.CopyToAsync(f,cancel).ConfigureAwait(false); f.Seek(0,SeekOrigin.Begin);

                String? h = (await SHA512.HashDataAsync(f,cancel).ConfigureAwait(false)).ToBase64FromByteArray(); await f.DisposeAsync().ConfigureAwait(false);

                if(String.Equals(d.StreamSHA512,h))
                {
                    if(String.IsNullOrEmpty(itempath) is false)
                    {
                        await File.WriteAllTextAsync(itempath,d.Object,cancel).ConfigureAwait(false);

                        if(File.Exists(itempath)) { return new(){ DataControlDownload = d , FilePath = filepath , ItemPath = itempath }; }
                    }

                    return new(){ DataControlDownload = d , FilePath = filepath };
                }
            }

            if(File.Exists(filepath)) { File.Delete(filepath); } if(File.Exists(itempath)) { File.Delete(itempath); } return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetStreamFailID,id); if(File.Exists(filepath)){ File.Delete(filepath); } if(File.Exists(itempath)) { File.Delete(itempath); } if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public Task<RestResponse<Guid?>> Store(DataControlUpload dcu , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Store",Method.Post).AddJsonBody(dcu); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<Guid?>(_,cancel);
    }

    ///<inheritdoc/>
    public async Task<HttpResponseMessage?>? StoreStream(DataItem? it , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            if( it is null || it?.GetContentStreamed() is not true ) { return null; }

            DataControlUpload? d = it.MakeDataControlUpload();

            using Stream? s = it.GetContentStream(); if(d is null || s is null) { return null; }

            if(d.Descriptor.LiveStream is not true) { s.Seek(0,SeekOrigin.Begin); }

            using StreamContent c = new(s,StreamBufferSize); c.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            using HttpClientHandler h = new(); h.ClientCertificates.Add(this.Certificate); h.UseDefaultCredentials = true;

            using HttpClient hc = new(h); hc.DefaultRequestHeaders.Add("DataControlUpload",d.ToString());

            if(String.IsNullOrEmpty(token) is false) { hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token); }

            return await hc.PostAsync(new Uri(this.EndPoint+"/StoreStream"),c,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,StoreStreamFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public void Dispose() { this.Auth.SetBearerToken("null"); this.Client.Dispose(); }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/property[@name="StreamBufferSize"]/*'/>*/
    public Int32 StreamBufferSize {get;set;} = 52428880;
}

/**<include file='DataControlClient.xml' path='DataControlClient/record[@name="DownloadInfo"]/main/*'/>*/
public sealed record DownloadInfo
{
    /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="DownloadInfo"]/property[@name="DataControlDownload"]/*'/>*/
    public DataControlDownload DataControlDownload {get;init;} = new();

    /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="DownloadInfo"]/property[@name="FilePath"]/*'/>*/
    public String FilePath                         {get;init;} = String.Empty;

    /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="DownloadInfo"]/property[@name="ItemPath"]/*'/>*/
    public String ItemPath                         {get;init;} = String.Empty;
}