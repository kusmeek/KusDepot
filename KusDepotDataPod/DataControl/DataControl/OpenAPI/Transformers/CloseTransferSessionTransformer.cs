namespace DataPodServices.DataControl;

internal sealed class CloseTransferSessionTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"CloseTransferSession",OrdinalIgnoreCase))
        {
            operation.Responses ??= new();
            operation.Responses["400"] = new OpenApiResponse { Description = "Bad Request", Content = new Dictionary<String,IOpenApiMediaType> { ["application/problem+json"] = SegmentedTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized", Content = new Dictionary<String,IOpenApiMediaType> { ["application/problem+json"] = SegmentedTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["409"] = new OpenApiResponse { Description = "Conflict" };
            operation.Responses["500"] = new OpenApiResponse { Description = "Internal Server Error", Content = new Dictionary<String,IOpenApiMediaType> { ["application/problem+json"] = SegmentedTransferOpenApiSchemas.ProblemMediaType() } };
        }

        return Task.CompletedTask;
    }
}
