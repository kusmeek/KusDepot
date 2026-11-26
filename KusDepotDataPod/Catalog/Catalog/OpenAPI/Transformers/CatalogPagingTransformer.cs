namespace DataPodServices.Catalog;

internal sealed class CatalogPagingTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation , OpenApiOperationTransformerContext context , CancellationToken cancel)
    {
        String? schemaPrefix = operation.OperationId?.ToString() switch
        {
            "SearchElements" => "Element",
            "SearchServices" => "Service",
            "SearchCommands" => "Command",
            "SearchMedia" => "Media",
            "SearchNotes" => "Note",
            "SearchTags" => "Tag",
            _ => null
        };

        if(schemaPrefix is null)
        {
            return Task.CompletedTask;
        }

        operation.Description = "Returns one page of matching catalog items in ID order. Set CatalogPageSize from 1 to 100 (default 50). Valid searches with no matches return 200 with an empty collection.";

        SetPropertyDescription(context.Document,schemaPrefix + "Query","CatalogPageSize","Maximum records to return on this page. Defaults to 50; valid values are 1 through 100.",true);
        SetPropertyDescription(context.Document,schemaPrefix + "Query","CatalogPageToken","Opaque continuation token from the previous page. Reuse it with the same filters.");
        SetPropertyDescription(context.Document,schemaPrefix + "Response","NextCatalogPageToken","Opaque token to request the next page, or null when no page remains.");

        return Task.CompletedTask;
    }

    private static void SetPropertyDescription(OpenApiDocument? document , String schemaName , String propertyName , String description , Boolean setPageSizeConstraints = false)
    {
        if(document?.Components?.Schemas?.TryGetValue(schemaName,out IOpenApiSchema? component) is not true ||
            component is not OpenApiSchema schema || schema.Properties?.TryGetValue(propertyName,out IOpenApiSchema? property) is not true ||
            property is not OpenApiSchema propertySchema)
        {
            return;
        }

        propertySchema.Description = description;

        if(setPageSizeConstraints)
        {
            propertySchema.Minimum = "1";
            propertySchema.Maximum = "100";
            propertySchema.Default = JsonNode.Parse("50");
        }
    }
}
