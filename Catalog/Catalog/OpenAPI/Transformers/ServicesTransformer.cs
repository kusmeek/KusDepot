namespace KusDepot.Data;

internal sealed class ServicesTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"SearchServices",OrdinalIgnoreCase))
        {
            operation.Responses?.TryAdd("400", new OpenApiResponse
            {
                Description = "Bad Request"
            });

            operation.Responses?.TryAdd("401", new OpenApiResponse
            {
                Description = "Unauthorized"
            });

            operation.Responses?.TryAdd("403", new OpenApiResponse
            {
                Description = "Forbidden"
            });

            operation.Responses?.TryAdd("500", new OpenApiResponse
            {
                Description = "Internal Server Error",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",Nullable = true
                        }
                    }
                }
            });
        }

        return Task.CompletedTask;
    }
}