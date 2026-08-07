namespace KusDepot.Security;

public static partial class ToolManifestRegistry
{
    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryEntry"]/main/*'/>*/
    private sealed record class RegistryEntry
    {
        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryEntry"]/property[@name="Manifest"]/*'/>*/
        public ToolManifest Manifest { get; }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryEntry"]/property[@name="MethodIndexes"]/*'/>*/
        public ImmutableDictionary<String,Int32> MethodIndexes { get; }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryEntry"]/constructor[@name="Constructor"]/*'/>*/
        public RegistryEntry(ToolManifest manifest , ImmutableDictionary<String,Int32> methodindexes)
        {
            this.Manifest = manifest; this.MethodIndexes = methodindexes;
        }
    }

    /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/main/*'/>*/
    private sealed record class RegistryState
    {
        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/property[@name="BySchemaID"]/*'/>*/
        public ImmutableDictionary<String,RegistryEntry> BySchemaID { get; init; }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/property[@name="Locked"]/*'/>*/
        public Boolean Locked { get; init; }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/property[@name="SchemaIDByType"]/*'/>*/
        public ImmutableDictionary<Type,String> SchemaIDByType { get; init; }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/property[@name="UnlockCode"]/*'/>*/
        public Byte[]? UnlockCode { get; init; }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/constructor[@name="Constructor"]/*'/>*/
        public RegistryState(ImmutableDictionary<String,RegistryEntry> byschemaid , ImmutableDictionary<Type,String> schemaidbytype , Boolean locked = false , Byte[]? unlockcode = null)
        {
            this.BySchemaID = byschemaid; this.SchemaIDByType = schemaidbytype; this.Locked = locked; this.UnlockCode = unlockcode?.CloneByteArray();
        }

        /**<include file='ToolManifestRegistry.xml' path='ToolManifestRegistry/record[@name="RegistryState"]/property[@name="Empty"]/*'/>*/
        public static RegistryState Empty { get; } = new(ImmutableDictionary.Create<String,RegistryEntry>(StringComparer.Ordinal),ImmutableDictionary<Type,String>.Empty);
    }
}