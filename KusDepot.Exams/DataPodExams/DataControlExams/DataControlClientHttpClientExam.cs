namespace KusDepot.DataPodExams;

[TestFixture]
public sealed class DataControlClientHttpClientExam
{
    [Test]
    public async Task InjectedHttpClientUsesEndpointAndRequestScopedTokens()
    {
        RecordingHttpMessageHandler handler = new();
        using HttpClient httpClient = new(handler);
        Guid requestItemID = Guid.NewGuid();
        Guid defaultItemID = Guid.NewGuid();
        using DataControlClient client = new("https://data.example.test/service/", "client-token", new DataControlClientOptions
        {
            HttpClient = httpClient,
        });

        _ = await client.Delete(requestItemID, "request-token");
        _ = await client.Delete(defaultItemID);

        Assert.That(httpClient.BaseAddress,Is.Null);
        Assert.That(httpClient.DefaultRequestHeaders.Authorization,Is.Null);
        Assert.That(handler.RequestUris,Is.EqualTo(new[]
        {
            new Uri($"https://data.example.test/Delete/{requestItemID}"),
            new Uri($"https://data.example.test/Delete/{defaultItemID}"),
        }));
        Assert.That(handler.AuthorizationHeaders,Is.EqualTo(new[] { "Bearer request-token", "Bearer client-token" }));
    }

    [Test]
    public void InjectedHttpClientRejectsBaseAddress()
    {
        using HttpClient httpClient = new(new RecordingHttpMessageHandler())
        {
            BaseAddress = new Uri("https://data.example.test/"),
        };

        Assert.Throws<ArgumentException>(() => new DataControlClient("https://data.example.test/",options:new DataControlClientOptions
        {
            HttpClient = httpClient,
        }));
    }

    [Test]
    public void InjectedHttpClientRejectsDefaultAuthorizationHeader()
    {
        using HttpClient httpClient = new(new RecordingHttpMessageHandler());
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer","global-token");

        Assert.Throws<ArgumentException>(() => new DataControlClient("https://data.example.test/",options:new DataControlClientOptions
        {
            HttpClient = httpClient,
        }));
    }

    [Test]
    public void DataControlClientDisposesOwnedInjectedHttpClient()
    {
        RecordingHttpMessageHandler handler = new();
        HttpClient httpClient = new(handler);
        DataControlClient client = new("https://data.example.test/",options:new DataControlClientOptions
        {
            HttpClient = httpClient,
            HttpClientOwned = true,
        });

        client.Dispose();

        Assert.That(handler.IsDisposed,Is.True);
    }

    [Test]
    public async Task DataControlClientLeavesUnownedInjectedHttpClientAliveOnAsyncDispose()
    {
        RecordingHttpMessageHandler handler = new();
        using HttpClient httpClient = new(handler);
        DataControlClient client = new("https://data.example.test/",options:new DataControlClientOptions
        {
            HttpClient = httpClient,
        });

        await client.DisposeAsync();

        Assert.That(handler.IsDisposed,Is.False);
    }

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        public List<Uri> RequestUris { get; } = new();

        public List<String?> AuthorizationHeaders { get; } = new();

        public Boolean IsDisposed { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            this.RequestUris.Add(request.RequestUri!);
            this.AuthorizationHeaders.Add(request.Headers.Authorization?.ToString());

            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(String.Empty),
            });
        }

        protected override void Dispose(Boolean disposing)
        {
            this.IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
