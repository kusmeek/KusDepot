namespace KusDepot.Data.Models;

/**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "NoteQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.NoteQuery")] [GenerateSerializer] [Immutable]

public sealed record class NoteQuery
{
    /**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/property[@name="CatalogPageSize"]/*'/>*/
    [JsonPropertyName("CatalogPageSize")] [JsonInclude]
    [DataMember(Name = "CatalogPageSize" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Int32? CatalogPageSize {get;init;}

    /**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/property[@name="CatalogPageToken"]/*'/>*/
    [JsonPropertyName("CatalogPageToken")] [JsonInclude]
    [DataMember(Name = "CatalogPageToken" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? CatalogPageToken {get;init;}

    /**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonInclude]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String[]? Notes {get;init;}
}

/**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "NoteResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.NoteResponse")] [GenerateSerializer] [Immutable]

public sealed record class NoteResponse
{
    /**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/property[@name="IDs"]/*'/>*/
    [JsonPropertyName("IDs")] [JsonInclude]
    [DataMember(Name = "IDs" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Guid[] IDs {get;init;} = Array.Empty<Guid>();

    /**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/property[@name="NextCatalogPageToken"]/*'/>*/
    [JsonPropertyName("NextCatalogPageToken")] [JsonInclude]
    [DataMember(Name = "NextCatalogPageToken" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? NextCatalogPageToken {get;init;}
}