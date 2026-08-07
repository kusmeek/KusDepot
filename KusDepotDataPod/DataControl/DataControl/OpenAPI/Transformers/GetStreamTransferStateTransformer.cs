namespace DataPodServices.DataControl;

internal sealed class GetStreamTransferStateTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"GetStreamTransferState",OrdinalIgnoreCase))
        {
            operation.Responses ??= new();
            operation.Responses["400"] = new OpenApiResponse { Description = "Bad Request", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["404"] = new OpenApiResponse { Description = "Not Found", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["500"] = new OpenApiResponse { Description = "Internal Server Error", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
        }

        return Task.CompletedTask;
    }
}
