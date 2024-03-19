namespace KusDepot.Data;

/**<include file='Note.xml' path='Note/record[@name="NoteRequest"]/main/*'/>*/
public record NoteRequest
{
    /**<include file='Note.xml' path='Note/record[@name="NoteRequest"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes {get;init;}
}

/**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/main/*'/>*/
public record NoteResponse
{
    /**<include file='Note.xml' path='Note/record[@name="NoteResponse"]/property[@name="IDs"]/*'/>*/
    public HashSet<Guid> IDs {get;init;} = new HashSet<Guid>();
}