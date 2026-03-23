namespace KusDepot.Data.Models;

/**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "TagQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.TagQuery")] [GenerateSerializer] [Immutable]

public sealed record class TagQuery
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/property[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonRequired]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String[]? Tags {get;init;}
}

/**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "TagResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.TagResponse")] [GenerateSerializer] [Immutable]

public sealed record class TagResponse
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/property[@name="IDs"]/*'/>*/
    [JsonPropertyName("IDs")] [JsonRequired]
    [DataMember(Name = "IDs" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public HashSet<Guid> IDs {get;init;} = new HashSet<Guid>();
}