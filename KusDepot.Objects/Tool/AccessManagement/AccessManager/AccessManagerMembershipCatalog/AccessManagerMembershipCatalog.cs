namespace KusDepot.Security;

/**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessManagerMembershipCatalog")] [Immutable]

public sealed class AccessManagerMembershipCatalog
{
    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/property[@name="Entries"]/*'/>*/
    [JsonPropertyName("Entries")] [JsonRequired] [Id(0)]
    public Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>? Entries
    {
        get
        {
            try
            {
                if(entries is null) { return null; }

                return entries.ToDictionary(_ => new String(_.Key),_ => _.Value.Select(entry => entry.Clone()).ToImmutableArray());
            }
            catch ( Exception ) { if(NoExceptions) { return null; } throw; }
        }
        set => entries ??= value?.ToDictionary(_ => new String(_.Key),_ => _.Value.Select(entry => entry.Clone()).ToImmutableArray());
    }

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/field[@name="entries"]/*'/>*/
    [NonSerialized]
    private Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>? entries;

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/property[@name="ToolManifestIdentity"]/*'/>*/
    [JsonPropertyName("ToolManifestIdentity")] [JsonRequired] [Id(1)]
    public ToolManifestIdentity? ToolManifestIdentity
    {
        get => toolmanifestidentity?.Clone();

        set => toolmanifestidentity ??= value?.Clone();
    }

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/field[@name="toolmanifestidentity"]/*'/>*/
    [NonSerialized]
    private ToolManifestIdentity? toolmanifestidentity;

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/method[@name="Clone"]/*'/>*/
    public AccessManagerMembershipCatalog Clone()
    {
        return new()
        {
            Entries = this.Entries,

            ToolManifestIdentity = this.ToolManifestIdentity
        };
    }

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/method[@name="Clear"]/*'/>*/
    public void Clear()
    {
        try
        {
            this.entries = null;

            this.toolmanifestidentity = null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ClearFail); if(NoExceptions) { return; } throw; }
    }

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/method[@name="Deserialize"]/*'/>*/
    public static AccessManagerMembershipCatalog? Deserialize(Byte[] input , IFormatProvider? format = null)
    {
        try
        {
            if(Array.Empty<Byte>().SequenceEqual(input)) { return null; }

            return JsonUtility.Deserialize<AccessManagerMembershipCatalog>(input);
        }
        catch ( SerializationException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,DeserializeFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManagerMembershipCatalog.xml' path='AccessManagerMembershipCatalog/class[@name="AccessManagerMembershipCatalog"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize()
    {
        try
        {
            return JsonUtility.Serialize(this);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeFail); if(NoExceptions) { return Array.Empty<Byte>(); } throw; }
    }
}