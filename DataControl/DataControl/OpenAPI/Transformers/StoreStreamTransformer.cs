namespace KusDepot.Data;

internal sealed class StoreStreamTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"StoreStream",OrdinalIgnoreCase))
        {
            operation.Parameters.Clear(); operation.Parameters.Add(new OpenApiParameter
            {
                In = ParameterLocation.Header,

                Name = "DataControlUpload",

                Schema = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = "DataControlUpload",Type = ReferenceType.Schema
                    }
                }
            });

            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/octet-stream"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "Content Stream",Format = "binary",Type = "string"
                        }
                    }
                }
            };

            operation.Responses?.TryAdd("200", new OpenApiResponse
            {
                Description = "OK",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
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

            operation.Responses?.TryAdd("409", new OpenApiResponse
            {
                Description = "Conflict",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "Conflicting ID.",Type = "string"
                        }
                    }
                }
            });

            operation.Responses?.TryAdd("422", new OpenApiResponse
            {
                Description = "Unprocessable Entity",

                Content = new Dictionary<String,OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Description = "Verification failed. DataControlUpload ID.",Type = "string"
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