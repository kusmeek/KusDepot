namespace KusDepot.Data.Models;

/**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/main/*'/>*/
[DataContract(Name = "TagQuery" , Namespace = "KusDepot")]
public sealed record TagQuery
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagQuery"]/property[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    public String[]? Tags {get;init;}
}

/**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/main/*'/>*/
[DataContract(Name = "TagResponse" , Namespace = "KusDepot")]
public sealed record TagResponse
{
    /**<include file='Tag.xml' path='Tag/record[@name="TagResponse"]/property[@name="IDs"]/*'/>*/
    [DataMember(Name = "IDs" , EmitDefaultValue = true , IsRequired = true)]
    public HashSet<Guid> IDs {get;init;} = new HashSet<Guid>();
}