namespace KusDepot.Data.Clients;

public sealed partial class CatalogClient
{
    ///<inheritdoc/>
    public IAsyncEnumerable<KusDepot.Data.Models.Element> EnumerateElementsAsync(ElementQuery query , String? token = null , CancellationToken cancel = default) =>
        this.EnumeratePagesAsync(query,(q,t) => q with { CatalogPageToken = t },(q,c) => this.SearchElements(q,token,c),r => r.Elements,r => r.NextCatalogPageToken,cancel);

    ///<inheritdoc/>
    public IAsyncEnumerable<KusDepot.Data.Models.Command> EnumerateCommandsAsync(CommandQuery query , String? token = null , CancellationToken cancel = default) =>
        this.EnumeratePagesAsync(query,(q,t) => q with { CatalogPageToken = t },(q,c) => this.SearchCommands(q,token,c),r => r.Commands,r => r.NextCatalogPageToken,cancel);

    ///<inheritdoc/>
    public IAsyncEnumerable<KusDepot.Data.Models.Service> EnumerateServicesAsync(ServiceQuery query , String? token = null , CancellationToken cancel = default) =>
        this.EnumeratePagesAsync(query,(q,t) => q with { CatalogPageToken = t },(q,c) => this.SearchServices(q,token,c),r => r.Services,r => r.NextCatalogPageToken,cancel);

    ///<inheritdoc/>
    public IAsyncEnumerable<KusDepot.Data.Models.Media> EnumerateMediaAsync(MediaQuery query , String? token = null , CancellationToken cancel = default) =>
        this.EnumeratePagesAsync(query,(q,t) => q with { CatalogPageToken = t },(q,c) => this.SearchMedia(q,token,c),r => r.Media,r => r.NextCatalogPageToken,cancel);

    ///<inheritdoc/>
    public IAsyncEnumerable<Guid> EnumerateNoteIDsAsync(NoteQuery query , String? token = null , CancellationToken cancel = default) =>
        this.EnumeratePagesAsync(query,(q,t) => q with { CatalogPageToken = t },(q,c) => this.SearchNotes(q,token,c),r => r.IDs,r => r.NextCatalogPageToken,cancel);

    ///<inheritdoc/>
    public IAsyncEnumerable<Guid> EnumerateTagIDsAsync(TagQuery query , String? token = null , CancellationToken cancel = default) =>
        this.EnumeratePagesAsync(query,(q,t) => q with { CatalogPageToken = t },(q,c) => this.SearchTags(q,token,c),r => r.IDs,r => r.NextCatalogPageToken,cancel);

    private async IAsyncEnumerable<TItem> EnumeratePagesAsync<TQuery,TResponse,TItem>(
        TQuery query,
        Func<TQuery,String?,TQuery> withPageToken,
        Func<TQuery,CancellationToken,Task<CatalogResponse<TResponse>>> search,
        Func<TResponse,IEnumerable<TItem>> getItems,
        Func<TResponse,String?> getNextPageToken,
        [EnumeratorCancellation] CancellationToken cancel = default)
    {
        TQuery pageQuery = query;

        while(true)
        {
            cancel.ThrowIfCancellationRequested();

            CatalogResponse<TResponse> response = await search(pageQuery,cancel).ConfigureAwait(false);

            if((Int32)response.StatusCode is < (Int32)HttpStatusCode.OK or >= (Int32)HttpStatusCode.MultipleChoices)
            {
                throw new CatalogClientHttpException(response.StatusCode,response.Content ?? String.Empty);
            }

            if(response.Data is null)
            {
                throw new OperationFailedException("Catalog returned a successful response without a deserialized response body.");
            }

            foreach(TItem item in getItems(response.Data))
            {
                cancel.ThrowIfCancellationRequested();

                yield return item;
            }

            String? nextPageToken = getNextPageToken(response.Data);

            if(nextPageToken is null)
            {
                yield break;
            }

            pageQuery = withPageToken(query,nextPageToken);
        }
    }
}
