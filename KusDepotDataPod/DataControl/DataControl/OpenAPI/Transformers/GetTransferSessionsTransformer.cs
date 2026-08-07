namespace DataPodServices.DataControl;

internal sealed class GetTransferSessionsTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"GetTransferSessions",OrdinalIgnoreCase))
        {
            operation.Responses ??= new();
            operation.Responses["400"] = new OpenApiResponse { Description = "Bad Request", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = SegmentedTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = SegmentedTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["500"] = new OpenApiResponse { Description = "Internal Server Error", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = SegmentedTransferOpenApiSchemas.ProblemMediaType() } };
        }

        return Task.CompletedTask;
    }
}