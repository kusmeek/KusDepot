namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public static void ConfigureOpenApi(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi("Catalog",o =>
            {
                o.AddOperationTransformer<ServicesTransformer>();
                o.AddOperationTransformer<CommandsTransformer>();
                o.AddOperationTransformer<ElementsTransformer>();
                o.AddOperationTransformer<MediaTransformer>();
                o.AddOperationTransformer<NotesTransformer>();
                o.AddOperationTransformer<TagsTransformer>();
                o.AddOperationTransformer<CatalogPagingTransformer>();
            });
        }
    }

    internal static void SetupOpenApi(WebApplication server)
    {
        if(server.Environment.IsDevelopment())
        {
            server.MapOpenApi();

            server.MapScalarApiReference("openapi",o => 
            {
                o.WithTitle("Catalog API");
                o.WithTheme(ScalarTheme.DeepSpace);
                o.WithDefaultHttpClient(ScalarTarget.CSharp,ScalarClient.HttpClient);
            });
        }
    }
}