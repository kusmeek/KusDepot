namespace DataPodServices.DataControl;

internal static class SegmentedTransferOpenApiSchemas
{
    public static OpenApiMediaType ProblemMediaType() => new()
    {
        Schema = new OpenApiSchema { Type = JsonSchemaType.Object }
    };
}
