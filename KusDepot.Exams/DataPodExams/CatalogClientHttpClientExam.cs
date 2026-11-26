using System.Net;

namespace KusDepot.DataPodExams;

[TestFixture]
public sealed class CatalogClientHttpClientExam
{
    [Test]
    public async Task CatalogClientInjectedHttpClientUsesEndpointAndRequestScopedTokens()
    {
        RecordingHttpMessageHandler handler = new();
        using HttpClient httpClient = new(handler);
        using CatalogClient client = new("https://catalog.example.test/service/", "client-token", new CatalogClientOptions
        {
            HttpClient = httpClient,
        });

        await client.SearchTags(new TagQuery(), "request-token");
        await client.SearchTags(new TagQuery());

        Assert.That(httpClient.BaseAddress,Is.Null);
        Assert.That(httpClient.DefaultRequestHeaders.Authorization,Is.Null);
        Assert.That(handler.RequestUris,Is.EqualTo(new[]
        {
            new Uri("https://catalog.example.test/Catalog/Tags"),
            new Uri("https://catalog.example.test/Catalog/Tags"),
        }));
        Assert.That(handler.AuthorizationHeaders,Is.EqualTo(new[] { "Bearer request-token", "Bearer client-token" }));
    }

    [Test]
    public async Task CatalogClientTagIteratorPreservesFiltersAndFollowsPageTokens()
    {
        Guid firstId = Guid.NewGuid();
        Guid secondId = Guid.NewGuid();
        RecordingHttpMessageHandler handler = new();
        handler.Responses.Enqueue(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent($"{{\"IDs\":[\"{firstId}\"],\"NextCatalogPageToken\":\"next-page\"}}"),
        });
        handler.Responses.Enqueue(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent($"{{\"IDs\":[\"{secondId}\"],\"NextCatalogPageToken\":null}}"),
        });

        using HttpClient httpClient = new(handler);
        using CatalogClient client = new("https://catalog.example.test/",options:new CatalogClientOptions { HttpClient = httpClient });
        TagQuery query = new() { Tags = ["blue"], CatalogPageSize = 1 };
        List<Guid> results = new();

        await foreach(Guid id in client.EnumerateTagIDsAsync(query))
        {
            results.Add(id);
        }

        Assert.That(results,Is.EqualTo(new[] { firstId,secondId }));
        Assert.That(handler.RequestBodies,Has.Count.EqualTo(2));
        TagQuery firstQuery = JsonUtility.Parse<TagQuery>(handler.RequestBodies[0])!;
        TagQuery secondQuery = JsonUtility.Parse<TagQuery>(handler.RequestBodies[1])!;
        Assert.That(firstQuery.Tags,Is.EqualTo(new[] { "blue" }));
        Assert.That(firstQuery.CatalogPageSize,Is.EqualTo(1));
        Assert.That(firstQuery.CatalogPageToken,Is.Null);
        Assert.That(secondQuery.Tags,Is.EqualTo(new[] { "blue" }));
        Assert.That(secondQuery.CatalogPageSize,Is.EqualTo(1));
        Assert.That(secondQuery.CatalogPageToken,Is.EqualTo("next-page"));
    }

    [Test]
    public async Task CatalogClientIteratorThrowsWithHttpStatusAndRawResponseBody()
    {
        const String body = "catalog error details";
        RecordingHttpMessageHandler handler = new();
        handler.Responses.Enqueue(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
        {
            Content = new StringContent(body),
        });

        using HttpClient httpClient = new(handler);
        using CatalogClient client = new("https://catalog.example.test/",options:new CatalogClientOptions { HttpClient = httpClient });

        CatalogClientHttpException exception = (await Assert.ThrowsAsync<CatalogClientHttpException>(async () =>
        {
            await foreach(Guid _ in client.EnumerateTagIDsAsync(new TagQuery())) { }
        }))!;

        Assert.That(exception.ResponseStatusCode,Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        Assert.That(exception.ResponseBody,Is.EqualTo(body));
    }

    [Test]
    public async Task CatalogClientIteratorPassesCancellationToHttpRequest()
    {
        RecordingHttpMessageHandler handler = new() { WaitForCancellation = true };
        using HttpClient httpClient = new(handler);
        using CatalogClient client = new("https://catalog.example.test/",options:new CatalogClientOptions { HttpClient = httpClient });
        using CancellationTokenSource cancellation = new();
        await using IAsyncEnumerator<Guid> enumerator = client.EnumerateTagIDsAsync(new TagQuery(),cancel:cancellation.Token).GetAsyncEnumerator();

        Task<Boolean> next = enumerator.MoveNextAsync().AsTask();
        await handler.RequestStarted.Task;
        cancellation.Cancel();

        await Assert.CatchAsync<OperationCanceledException>(async () => await next);
    }

    [Test]
    public async Task CatalogClientInjectedHttpClientTimeoutAppliesWhileReadingResponseBody()
    {
        TaskCompletionSource bodyReadStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        RecordingHttpMessageHandler handler = new()
        {
            ResponseContent = new DelayedHttpContent(bodyReadStarted),
        };
        using HttpClient httpClient = new(handler) { Timeout = TimeSpan.FromMilliseconds(100) };
        using CatalogClient client = new("https://catalog.example.test/",options:new CatalogClientOptions { HttpClient = httpClient });

        await Assert.CatchAsync<OperationCanceledException>(async () => await client.SearchTags(new TagQuery()));

        Assert.That(bodyReadStarted.Task.IsCompleted,Is.True);
    }

    [Test]
    public void CatalogClientPreservesInfiniteTimeoutOnInjectedHttpClient()
    {
        using HttpClient httpClient = new(new RecordingHttpMessageHandler()) { Timeout = Timeout.InfiniteTimeSpan };
        using CatalogClient _ = new("https://catalog.example.test/",options:new CatalogClientOptions { HttpClient = httpClient });

        Assert.That(httpClient.Timeout,Is.EqualTo(Timeout.InfiniteTimeSpan));
    }

    [Test]
    public void CatalogClientInjectedHttpClientRejectsBaseAddress()
    {
        using HttpClient httpClient = new(new RecordingHttpMessageHandler())
        {
            BaseAddress = new Uri("https://catalog.example.test/"),
        };

        Assert.Throws<ArgumentException>(() => new CatalogClient("https://catalog.example.test/",options:new CatalogClientOptions
        {
            HttpClient = httpClient,
        }));
    }

    [Test]
    public void CatalogClientInjectedHttpClientRejectsDefaultAuthorizationHeader()
    {
        using HttpClient httpClient = new(new RecordingHttpMessageHandler());
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer","global-token");

        Assert.Throws<ArgumentException>(() => new CatalogClient("https://catalog.example.test/",options:new CatalogClientOptions
        {
            HttpClient = httpClient,
        }));
    }

    [Test]
    public void CatalogClientDisposesOwnedInjectedHttpClient()
    {
        RecordingHttpMessageHandler handler = new();
        HttpClient httpClient = new(handler);
        CatalogClient client = new("https://catalog.example.test/",options:new CatalogClientOptions
        {
            HttpClient = httpClient,
            HttpClientOwned = true,
        });

        client.Dispose();

        Assert.That(handler.IsDisposed,Is.True);
    }

    [Test]
    public void CatalogClientLeavesUnownedInjectedHttpClientAlive()
    {
        RecordingHttpMessageHandler handler = new();
        using HttpClient httpClient = new(handler);
        CatalogClient client = new("https://catalog.example.test/",options:new CatalogClientOptions
        {
            HttpClient = httpClient,
        });

        client.Dispose();

        Assert.That(handler.IsDisposed,Is.False);
    }

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        public Queue<HttpResponseMessage> Responses { get; } = new();

        public List<Uri> RequestUris { get; } = new();

        public List<String> RequestBodies { get; } = new();

        public List<String?> AuthorizationHeaders { get; } = new();

        public TaskCompletionSource RequestStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Boolean WaitForCancellation { get; init; }

        public HttpContent? ResponseContent { get; init; }

        public Boolean IsDisposed { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            this.RequestUris.Add(request.RequestUri!);
            this.AuthorizationHeaders.Add(request.Headers.Authorization?.ToString());
            this.RequestBodies.Add(request.Content is null ? String.Empty : await request.Content.ReadAsStringAsync(cancellationToken));

            this.RequestStarted.TrySetResult();

            if(this.WaitForCancellation)
            {
                await Task.Delay(Timeout.Infinite,cancellationToken);
            }

            if(this.Responses.TryDequeue(out HttpResponseMessage? response))
            {
                return response;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = this.ResponseContent ?? new StringContent(String.Empty),
            };
        }

        protected override void Dispose(Boolean disposing)
        {
            this.IsDisposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class DelayedHttpContent(TaskCompletionSource bodyReadStarted) : HttpContent
    {
        protected override async Task SerializeToStreamAsync(Stream stream,System.Net.TransportContext? context)
        {
            await this.SerializeToStreamAsync(stream,context,CancellationToken.None);
        }

        protected override async Task SerializeToStreamAsync(Stream stream,System.Net.TransportContext? context,CancellationToken cancellationToken)
        {
            bodyReadStarted.TrySetResult();
            await Task.Delay(Timeout.Infinite,cancellationToken);
        }

        protected override Boolean TryComputeLength(out Int64 length)
        {
            length = 0;
            return false;
        }
    }
}
