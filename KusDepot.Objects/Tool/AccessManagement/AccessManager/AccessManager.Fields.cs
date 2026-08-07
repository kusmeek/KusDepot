namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager : IAccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="AccessKeys"]/*'/>*/
    private Dictionary<String,HashSet<AccessKeyToken>> AccessKeys;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="ID"]/*'/>*/
    protected Guid? ID;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Initialized"]/*'/>*/
    private Boolean Initialized;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="KeyAssertionGenerator"]/*'/>*/
    protected IAccessKeyAssertionGeneratorMode? KeyAssertionGenerator;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="KeyMaker"]/*'/>*/
    protected KeyMaker? KeyMaker;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Logger"]/*'/>*/
    protected ILogger? Logger;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="ManifestIdentity"]/*'/>*/
    protected ToolManifestIdentity? ManifestIdentity;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="MembershipCache"]/*'/>*/
    private MembershipCache MembershipCache;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="MembershipsByToken"]/*'/>*/
    private Dictionary<AccessKeyToken,MembershipToken> MembershipsByToken;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Options"]/*'/>*/
    protected AccessManagerOptions? Options;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="PolicyEngine"]/*'/>*/
    protected IAccessPolicyEngineMode PolicyEngine = DefaultAccessPolicyEngine.Instance;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="RealmKey"]/*'/>*/
    protected Byte[]? RealmKey;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="SignatureProcessor"]/*'/>*/
    protected IAccessKeySignatureProcessorMode? SignatureProcessor;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="Tool"]/*'/>*/
    protected ITool? Tool;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/field[@name="UsesDefaultPolicyEngine"]/*'/>*/
    private Boolean UsesDefaultPolicyEngine = true;

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/property[@name="MyNoExceptions"]/*'/>*/
    private Boolean MyNoExceptions => !Tool?.MyExceptionsEnabled() ?? true;
}