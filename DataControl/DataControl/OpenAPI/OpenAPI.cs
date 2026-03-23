namespace KusDepot.Data;

public sealed partial class DataControl
{
    public static void ConfigureOpenApi(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi("DataControl",o =>
            {
                o.AddOperationTransformer<DeleteTransformer>();
                o.AddOperationTransformer<GetTransformer>();
                o.AddOperationTransformer<GetStreamTransformer>();
                o.AddOperationTransformer<StoreTransformer>();
                o.AddOperationTransformer<StoreStreamTransformer>();
            });
        }
    }

    private static void SetupOpenApi(WebApplication server)
    {
        if(server.Environment.IsDevelopment())
        {
            server.MapOpenApi().RequireAuthorization(X509Policy);

            server.MapScalarApiReference("openapi",o => 
            {
                o.WithTitle("DataControl API");
                o.WithTheme(ScalarTheme.DeepSpace);
                o.WithDefaultHttpClient(ScalarTarget.CSharp,ScalarClient.HttpClient);
            }).RequireAuthorization(X509Policy);
        }
    }
}