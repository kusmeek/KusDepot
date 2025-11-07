namespace KusDepot.Data;

internal sealed class DeleteTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"Delete",OrdinalIgnoreCase))
        {
            operation.Responses?.TryAdd("200", new OpenApiResponse
            {
                Description = "OK",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "Deleted ID",Type = "string"
                        }
                    }
                }
            });

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
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "ID",Type = "string",Nullable = true
                        }
                    }
                }
            });
        }

        return Task.CompletedTask;
    }
}