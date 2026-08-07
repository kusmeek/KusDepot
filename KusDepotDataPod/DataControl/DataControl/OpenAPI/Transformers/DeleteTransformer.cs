namespace DataPodServices.DataControl;

internal sealed class DeleteTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"Delete",OrdinalIgnoreCase))
        {
            operation.Responses ??= new();
            operation.Responses["200"] = new OpenApiResponse
            {
                Description = "OK",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "Deleted ID",Type = JsonSchemaType.String
                        }
                    }
                }
            };

            operation.Responses["400"] = new OpenApiResponse { Description = "Bad Request", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["404"] = new OpenApiResponse { Description = "Not Found", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["500"] = new OpenApiResponse { Description = "Internal Server Error", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
        }

        return Task.CompletedTask;
    }
}