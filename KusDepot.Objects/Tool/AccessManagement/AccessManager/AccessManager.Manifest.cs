namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="InitializeManifestIdentity"]/*'/>*/
    private Boolean InitializeManifestIdentity()
    {
        try
        {
            ToolManifest? manifest = this.Options?.Manifest;

            if(manifest is null)
            {
                if(Tool is null) { this.ManifestIdentity = null; return false; }

                manifest = ToolManifestRegistry.TryGetCurrentManifest(Tool.GetType(),out ToolManifest registered)
                    ? registered
                    : ToolManifest.Create(Tool);
            }

            manifest = manifest.ComputeManifestHash();

            ToolManifestIdentity identity = ToolManifestIdentity.Create(manifest);

            if(String.IsNullOrWhiteSpace(identity.ToolSchemaID) || String.IsNullOrWhiteSpace(identity.AccessKeyRealmID) || String.IsNullOrWhiteSpace(identity.ManifestHash))
            {
                this.ManifestIdentity = null; return false;
            }

            this.ManifestIdentity = identity; return true;
        }
        catch { this.ManifestIdentity = null; return false; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetManifestIdentity"]/*'/>*/
    private Boolean TryGetManifestIdentity(out ToolManifestIdentity identity)
    {
        identity = this.ManifestIdentity!;

        try
        {
            if(identity is not null) { return true; }

            if(InitializeManifestIdentity() is false || this.ManifestIdentity is null) { identity = null!; return false; }

            identity = this.ManifestIdentity; return true;
        }
        catch { identity = null!; return false; }
    }
}