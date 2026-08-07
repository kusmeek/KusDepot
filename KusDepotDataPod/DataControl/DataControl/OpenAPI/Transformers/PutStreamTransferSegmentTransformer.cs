namespace DataPodServices.DataControl;

internal sealed class PutStreamTransferSegmentTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        if(String.Equals(operation.OperationId,"PutStreamTransferSegment",OrdinalIgnoreCase))
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<String,OpenApiMediaType>
                {
                    [TransferEnvelope.MediaType] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "binary",
                            Description = "Stream transfer envelope stream containing serialized PutStreamTransferSegmentRequest metadata followed by the segment payload bytes."
                        }
                    }
                }
            };

            operation.Responses ??= new();
            operation.Responses["400"] = new OpenApiResponse { Description = "Bad Request", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["404"] = new OpenApiResponse { Description = "Not Found", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["409"] = new OpenApiResponse { Description = "Conflict", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["412"] = new OpenApiResponse { Description = "Precondition Failed", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["422"] = new OpenApiResponse { Description = "Unprocessable Entity", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
            operation.Responses["500"] = new OpenApiResponse { Description = "Internal Server Error", Content = new Dictionary<String,OpenApiMediaType> { ["application/problem+json"] = StreamTransferOpenApiSchemas.ProblemMediaType() } };
        }

        return Task.CompletedTask;
    }
}
