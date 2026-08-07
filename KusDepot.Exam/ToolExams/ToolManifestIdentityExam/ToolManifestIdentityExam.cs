namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolManifestIdentityExam
{
    [Test]
    public void ToolManifestIdentityCreate_Projects_Schema_Fields()
    {
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/Manifest","Manifest-Realm","1.0","KusDepot.Tools.ManifestTool",
        [
            new ToolOperationDescriptor { Index = 1 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." }
        ]).ComputeManifestHash();

        Type identityType = typeof(ToolManifest).Assembly.GetType("KusDepot.ToolManifestIdentity",throwOnError: true)!;
        MethodInfo create = identityType.GetMethod("Create",BindingFlags.Public | BindingFlags.Static,null,[typeof(ToolManifest)],null)!;

        Object? identity = create.Invoke(null,[manifest]);

        Check.That(identity).IsNotNull();
        Check.That(identityType.GetProperty("AccessKeyRealmID")!.GetValue(identity) as String).IsEqualTo("Manifest-Realm");
        Check.That(identityType.GetProperty("ToolSchemaID")!.GetValue(identity) as String).IsEqualTo(manifest.ToolSchemaID);
        Check.That(identityType.GetProperty("ManifestHash")!.GetValue(identity) as String).IsEqualTo(manifest.ManifestHash);
    }

    [Test]
    public void ToolManifestIdentityCreate_NullManifest_Uses_Empty_Strings()
    {
        Type identityType = typeof(ToolManifest).Assembly.GetType("KusDepot.ToolManifestIdentity",throwOnError: true)!;
        MethodInfo create = identityType.GetMethod("Create",BindingFlags.Public | BindingFlags.Static,null,[typeof(ToolManifest)],null)!;

        Object? identity = create.Invoke(null,[null]);

        Check.That(identity).IsNotNull();
        Check.That(identityType.GetProperty("AccessKeyRealmID")!.GetValue(identity) as String).IsEqualTo(String.Empty);
        Check.That(identityType.GetProperty("ToolSchemaID")!.GetValue(identity) as String).IsEqualTo(String.Empty);
        Check.That(identityType.GetProperty("ManifestHash")!.GetValue(identity) as String).IsEqualTo(String.Empty);
    }

    [Test]
    public void ToolManifestIdentity_Clone_Copies_Current_Values()
    {
        ToolManifestIdentity identity = ToolManifestIdentity.Create(null);
        identity.AccessKeyRealmID = "Realm-A";
        identity.ToolSchemaID = "Schema-A";
        identity.ManifestHash = "Hash-A";

        ToolManifestIdentity clone = identity.Clone();
        identity.ToolSchemaID = "Changed-Schema";

        Check.That(clone.AccessKeyRealmID).IsEqualTo("Realm-A");
        Check.That(clone.ToolSchemaID).IsEqualTo("Schema-A");
        Check.That(clone.ManifestHash).IsEqualTo("Hash-A");
    }
}
