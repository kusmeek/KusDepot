namespace KusDepot.Data;

internal sealed class GetTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"Get",OrdinalIgnoreCase))
        {
            operation.Responses?.TryAdd("400", new OpenApiResponse
            {
                Description = "Bad Request",
                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/problem+json"] = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object", Nullable = false } }
                }
            });

            operation.Responses?.TryAdd("401", new OpenApiResponse
            {
                Description = "Unauthorized",
                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/problem+json"] = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object", Nullable = false } }
                }
            });

            operation.Responses?.TryAdd("404", new OpenApiResponse
            {
                Description = "Not Found",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "ID",Type = "string"
                        }
                    }
                }
            });

            operation.Responses?.TryAdd("500", new OpenApiResponse
            {
                Description = "Internal Server Error",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/problem+json"] = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object", Nullable = false } }
                }
            });
        }

        return Task.CompletedTask;
    }
}