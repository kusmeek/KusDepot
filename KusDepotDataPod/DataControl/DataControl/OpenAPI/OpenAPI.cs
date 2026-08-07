namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    public static void ConfigureOpenApi(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi("DataControl",o =>
            {
                o.AddOperationTransformer<AbortTransferTransformer>();
                o.AddOperationTransformer<CommitUploadTransferTransformer>();
                o.AddOperationTransformer<CompleteStreamTransferTransformer>();
                o.AddOperationTransformer<DeleteTransformer>();
                o.AddOperationTransformer<GetTransferSegmentTransformer>();
                o.AddOperationTransformer<GetTransferSessionsTransformer>();
                o.AddOperationTransformer<GetTransferStateTransformer>();
                o.AddOperationTransformer<GetStreamTransferSegmentTransformer>();
                o.AddOperationTransformer<GetStreamTransferStateTransformer>();
                o.AddOperationTransformer<OpenFollowStreamTransferTransformer>();
                o.AddOperationTransformer<OpenGetTransferTransformer>();
                o.AddOperationTransformer<OpenUploadTransferTransformer>();
                o.AddOperationTransformer<OpenStreamTransferTransformer>();
                o.AddOperationTransformer<PutTransferSegmentTransformer>();
                o.AddOperationTransformer<PutStreamTransferSegmentTransformer>();
                o.AddOperationTransformer<RemoveTransferTransformer>();
                o.AddOperationTransformer<ReOpenFollowStreamTransferTransformer>();
                o.AddOperationTransformer<ReOpenGetTransferTransformer>();
                o.AddOperationTransformer<ReOpenUploadTransferTransformer>();
                o.AddOperationTransformer<ReOpenStreamTransferTransformer>();
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