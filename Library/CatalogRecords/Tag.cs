namespace KusDepot.Data;

/**<include file='Tag.xml' path='Tag/record[@name="TagRequest"]/main/*'/>*/
public record TagRequest
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagRequest"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags {get;init;}
}

/**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/main/*'/>*/
public record TagResponse
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/property[@name="IDs"]/*'/>*/
    public HashSet<Guid> IDs {get;init;} = new HashSet<Guid>();
}