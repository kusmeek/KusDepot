namespace KusDepot.Security;

/**<include file='SelfKey.xml' path='SelfKey/class[@name="SelfKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.SelfKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "SelfKey" , Namespace = "KusDepot.Security")]

public sealed record class SelfKey : AccessKey
{
    /**<include file='SelfKey.xml' path='SelfKey/class[@name="SelfKey"]/constructor[@name="Constructor"]/*'/>*/
    public SelfKey() {}
}