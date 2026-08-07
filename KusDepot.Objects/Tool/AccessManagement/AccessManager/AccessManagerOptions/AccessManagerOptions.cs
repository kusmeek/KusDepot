namespace KusDepot.Security;

/**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/main/*'/>*/
public sealed class AccessManagerOptions
{
    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="ID"]/*'/>*/
    public Guid? ID { get; set; }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="InitialMembershipCatalog"]/*'/>*/
    public AccessManagerMembershipCatalog? InitialMembershipCatalog
    {
        get => initialmembershipcatalog?.Clone();

        set => initialmembershipcatalog = value?.Clone();
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/field[@name="initialmembershipcatalog"]/*'/>*/
    private AccessManagerMembershipCatalog? initialmembershipcatalog;

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="KeyMaker"]/*'/>*/
    public KeyMaker? KeyMaker
    {
        get => keymaker;

        set => keymaker = value;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/field[@name="keymaker"]/*'/>*/
    private KeyMaker? keymaker;

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="KeyAssertionGenerator"]/*'/>*/
    public IAccessKeyAssertionGeneratorMode? KeyAssertionGenerator
    {
        get => keyassertiongenerator;

        set => keyassertiongenerator = value;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/field[@name="keyassertiongenerator"]/*'/>*/
    private IAccessKeyAssertionGeneratorMode? keyassertiongenerator;

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="Manifest"]/*'/>*/
    public ToolManifest? Manifest
    {
        get => manifest is null ? null : (manifest with { });

        set => manifest = value is null ? null : (value with { });
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/field[@name="manifest"]/*'/>*/
    private ToolManifest? manifest;

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="MembershipCacheSize"]/*'/>*/
    public Int32? MembershipCacheSize
    {
        get => membershipcachesize;

        set => membershipcachesize = value <= 0 ? MembershipCache.DefaultMaximumCount : value;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/field[@name="membershipcachesize"]/*'/>*/
    private Int32? membershipcachesize;

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="PolicyEngine"]/*'/>*/
    public IAccessPolicyEngineMode? PolicyEngine { get; set; }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="SignatureProcessor"]/*'/>*/
    public IAccessKeySignatureProcessorMode? SignatureProcessor { get; set; }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/property[@name="RealmKey"]/*'/>*/
    public Byte[]? RealmKey
    {
        get => realmkey?.CloneByteArray();

        set => realmkey = value?.CloneByteArray();
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/field[@name="realmkey"]/*'/>*/
    private Byte[]? realmkey;

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/constructor[@name="Constructor"]/*'/>*/
    public AccessManagerOptions(Guid? id = null , Byte[]? realmkey = null , ToolManifest? manifest = null,
        IAccessPolicyEngineMode? policyengine = null , Int32? membershipcachesize = MembershipCache.DefaultMaximumCount,
        AccessManagerMembershipCatalog? initialmembershipcatalog = null , KeyMaker? keymaker = null , IAccessKeyAssertionGeneratorMode? keyassertiongenerator = null,
        IAccessKeySignatureProcessorMode? signatureprocessor = null)
    {
        this.ID = id;
        this.KeyAssertionGenerator = keyassertiongenerator;
        this.KeyMaker = keymaker;
        this.Manifest = manifest;
        this.MembershipCacheSize = membershipcachesize <= 0 ? MembershipCache.DefaultMaximumCount : membershipcachesize;
        this.InitialMembershipCatalog = initialmembershipcatalog;
        this.PolicyEngine = policyengine;
        this.RealmKey = realmkey;
        this.SignatureProcessor = signatureprocessor;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="Create"]/*'/>*/
    public static AccessManagerOptions Create(Guid? id = null , Byte[]? realmkey = null , ToolManifest? manifest = null,
        IAccessPolicyEngineMode? policyengine = null , Int32? membershipcachesize = MembershipCache.DefaultMaximumCount,
        AccessManagerMembershipCatalog? initialmembershipcatalog = null , KeyMaker? keymaker = null , IAccessKeyAssertionGeneratorMode? keyassertiongenerator = null,
        IAccessKeySignatureProcessorMode? signatureprocessor = null)
    {
        return new(id,realmkey,manifest,policyengine,membershipcachesize,initialmembershipcatalog,keymaker,keyassertiongenerator,signatureprocessor);
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetID"]/*'/>*/
    public AccessManagerOptions SetID(Guid? id)
    {
        this.ID = id; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetManifest"]/*'/>*/
    public AccessManagerOptions SetManifest(ToolManifest? manifest)
    {
        this.Manifest = manifest; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetKeyAssertionGenerator"]/*'/>*/
    public AccessManagerOptions SetKeyAssertionGenerator(IAccessKeyAssertionGeneratorMode? keyassertiongenerator = null)
    {
        this.KeyAssertionGenerator = keyassertiongenerator; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetMembershipCacheSize"]/*'/>*/
    public AccessManagerOptions SetMembershipCacheSize(Int32? membershipcachesize = MembershipCache.DefaultMaximumCount)
    {
        this.MembershipCacheSize = membershipcachesize; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetInitialMembershipCatalog"]/*'/>*/
    public AccessManagerOptions SetInitialMembershipCatalog(AccessManagerMembershipCatalog? initialmembershipcatalog = null)
    {
        this.InitialMembershipCatalog = initialmembershipcatalog; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetPolicyEngine"]/*'/>*/
    public AccessManagerOptions SetPolicyEngine(IAccessPolicyEngineMode? policyengine = null)
    {
        this.PolicyEngine = policyengine; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetRealmKey"]/*'/>*/
    public AccessManagerOptions SetRealmKey(Byte[]? realmkey = null)
    {
        this.RealmKey = realmkey; return this;
    }

    /**<include file='AccessManagerOptions.xml' path='AccessManagerOptions/class[@name="AccessManagerOptions"]/method[@name="SetSignatureProcessor"]/*'/>*/
    public AccessManagerOptions SetSignatureProcessor(IAccessKeySignatureProcessorMode? signatureprocessor = null)
    {
        this.SignatureProcessor = signatureprocessor; return this;
    }
}