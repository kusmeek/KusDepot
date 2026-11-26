namespace DataPodServices.DataControl;

internal static class StreamTransferOpenApiSchemas
{
    public static OpenApiMediaType ProblemMediaType() => new()
    {
        Schema = new OpenApiSchema { Type = JsonSchemaType.Object }
    };
}
