namespace KusDepot.Data;

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
            });
        }
    }

    internal static void SetupOpenApi(WebApplication server)
    {
        if(server.Environment.IsDevelopment())
        {
            server.MapOpenApi().RequireAuthorization(X509Policy);

            server.MapScalarApiReference("openapi",o => 
            {
                o.WithTitle("Catalog API");
                o.WithTheme(ScalarTheme.DeepSpace);
                o.WithDefaultHttpClient(ScalarTarget.CSharp,ScalarClient.HttpClient);
            }).RequireAuthorization(X509Policy);
        }
    }
}