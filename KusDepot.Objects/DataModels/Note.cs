namespace KusDepot.Data.Models;

/**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "NoteQuery" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.NoteQuery")] [GenerateSerializer] [Immutable]

public sealed record class NoteQuery
{
    /**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/property[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonRequired]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String[]? Notes {get;init;}
}

/**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "NoteResponse" , Namespace = "KusDepot.Data.Models")]
[Alias("KusDepot.Data.Models.NoteResponse")] [GenerateSerializer] [Immutable]

public sealed record class NoteResponse
{
    /**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/property[@name="IDs"]/*'/>*/
    [JsonPropertyName("IDs")] [JsonRequired]
    [DataMember(Name = "IDs" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public HashSet<Guid> IDs {get;init;} = new HashSet<Guid>();
}