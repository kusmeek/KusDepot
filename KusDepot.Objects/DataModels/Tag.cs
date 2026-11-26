namespace KusDepot.Data.Models;

/**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "TagQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.TagQuery")] [GenerateSerializer] [Immutable]

public sealed record class TagQuery
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/property[@name="CatalogPageSize"]/*'/>*/
    [JsonPropertyName("CatalogPageSize")] [JsonInclude]
    [DataMember(Name = "CatalogPageSize" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Int32? CatalogPageSize {get;init;}

    /**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/property[@name="CatalogPageToken"]/*'/>*/
    [JsonPropertyName("CatalogPageToken")] [JsonInclude]
    [DataMember(Name = "CatalogPageToken" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? CatalogPageToken {get;init;}

    /**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonInclude]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String[]? Tags {get;init;}
}

/**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "TagResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.TagResponse")] [GenerateSerializer] [Immutable]

public sealed record class TagResponse
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/property[@name="IDs"]/*'/>*/
    [JsonPropertyName("IDs")] [JsonInclude]
    [DataMember(Name = "IDs" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Guid[] IDs {get;init;} = Array.Empty<Guid>();

    /**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/property[@name="NextCatalogPageToken"]/*'/>*/
    [JsonPropertyName("NextCatalogPageToken")] [JsonInclude]
    [DataMember(Name = "NextCatalogPageToken" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? NextCatalogPageToken {get;init;}
}