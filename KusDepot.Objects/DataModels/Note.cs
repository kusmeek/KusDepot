namespace KusDepot.Data.Models;

/**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/main/*'/>*/
[DataContract(Name = "NoteQuery" , Namespace = "KusDepot")]
public sealed record NoteQuery
{
    /**<include file='Note.xml' path='Note/record[@name="NoteQuery"]/property[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Notes {get;init;}
}

/**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/main/*'/>*/
[DataContract(Name = "NoteResponse" , Namespace = "KusDepot")]
public sealed record NoteResponse
{
    /**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/property[@name="IDs"]/*'/>*/
    [DataMember(Name = "IDs" , EmitDefaultValue = true , IsRequired = true)]
    public HashSet<Guid> IDs {get;init;} = new HashSet<Guid>();
}