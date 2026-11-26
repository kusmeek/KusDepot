namespace KusDepot.Security;

/**<include file='AccessManagerSnapshot.xml' path='AccessManagerSnapshot/class[@name="AccessManagerSnapshot"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessManagerSnapshot")] [Immutable]

public sealed class AccessManagerSnapshot
{
    /**<include file='AccessManagerSnapshot.xml' path='AccessManagerSnapshot/class[@name="AccessManagerSnapshot"]/property[@name="AccessKeys"]/*'/>*/
    [JsonPropertyName("AccessKeys")] [JsonRequired] [Id(0)]
    public Dictionary<String,ImmutableArray<AccessManagerSnapshotEntry>>? AccessKeys
    {
        get
        {
            try
            {
                if(accesskeys is null) { return null; }

                return accesskeys.ToDictionary(_ => new String(_.Key),_ => _.Value.Select(c => c.Copy()).ToImmutableArray());
            }
            catch ( Exception ) { if(NoExceptions) { return null; } throw; }
        }
        set => accesskeys ??= value?.ToDictionary(_ => new String(_.Key),_ => _.Value.Select(c => c.Copy()).ToImmutableArray());
    }

    /**<include file='AccessManagerSnapshot.xml' path='AccessManagerSnapshot/class[@name="AccessManagerSnapshot"]/field[@name="accesskeys"]/*'/>*/
    [NonSerialized]
    private Dictionary<String,ImmutableArray<AccessManagerSnapshotEntry>>? accesskeys;

    /**<include file='AccessManagerSnapshot.xml' path='AccessManagerSnapshot/class[@name="AccessManagerSnapshot"]/property[@name="ToolManifestIdentity"]/*'/>*/
    [JsonPropertyName("ToolManifestIdentity")] [JsonRequired] [Id(1)]
    public ToolManifestIdentity? ToolManifestIdentity
    {
        get => toolmanifestidentity?.Clone();

        set => toolmanifestidentity ??= value?.Clone();
    }

    /**<include file='AccessManagerSnapshot.xml' path='AccessManagerSnapshot/class[@name="AccessManagerSnapshot"]/field[@name="toolmanifestidentity"]/*'/>*/
    [NonSerialized]
    private ToolManifestIdentity? toolmanifestidentity;
}