namespace KusDepot.Security;

/**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/main/*'/>*/
public readonly struct MembershipToken
{
    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="AccessKeyRealmID"]/*'/>*/
    public String AccessKeyRealmID { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="AssertionSummaries"]/*'/>*/
    public ImmutableArray<AccessKeyAssertionSummary> AssertionSummaries { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="Audiences"]/*'/>*/
    public ImmutableArray<String> Audiences { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="IssuedAt"]/*'/>*/
    public DateTimeOffset IssuedAt { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    public Guid IssuerAccessManagerID { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="IssuerToolID"]/*'/>*/
    public Guid IssuerToolID { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="ManifestHash"]/*'/>*/
    public String ManifestHash { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="NotAfter"]/*'/>*/
    public DateTimeOffset? NotAfter { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/field[@name="Permissions"]/*'/>*/
    private readonly Byte[] Permissions;

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="Scopes"]/*'/>*/
    public ImmutableArray<String> Scopes { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="Subject"]/*'/>*/
    public String Subject { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="Token"]/*'/>*/
    public AccessKeyToken Token { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/property[@name="ToolSchemaID"]/*'/>*/
    public String ToolSchemaID { get; }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/constructor[@name="Constructor"]/*'/>*/
    public MembershipToken(String subject , AccessKeyToken token , Byte[]? permissions = null , DateTimeOffset? notafter = null , String? toolschemaid = null , String? manifesthash = null ,
        String? accesskeyrealmid = null , Guid issuertoolid = default , Guid issueraccessmanagerid = default , ImmutableArray<String> audiences = default , ImmutableArray<String> scopes = default ,
        ImmutableArray<AccessKeyAssertionSummary> assertionsummaries = default)
        : this(subject,token,permissions,default,notafter,toolschemaid,manifesthash,accesskeyrealmid,issuertoolid,issueraccessmanagerid,audiences,scopes,assertionsummaries)
    {}

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/constructor[@name="ConstructorIssuedAt"]/*'/>*/
    public MembershipToken(String subject , AccessKeyToken token , Byte[]? permissions , DateTimeOffset issuedat , DateTimeOffset? notafter , String? toolschemaid , String? manifesthash ,
        String? accesskeyrealmid , Guid issuertoolid , Guid issueraccessmanagerid , ImmutableArray<String> audiences , ImmutableArray<String> scopes , ImmutableArray<AccessKeyAssertionSummary> assertionsummaries = default)
    {
        Subject = subject ?? String.Empty; Token = token; IssuedAt = issuedat; NotAfter = notafter;

        ToolSchemaID = toolschemaid ?? String.Empty; ManifestHash = manifesthash ?? String.Empty;

        AccessKeyRealmID = accesskeyrealmid ?? String.Empty; IssuerToolID = issuertoolid; IssuerAccessManagerID = issueraccessmanagerid;

        Audiences = audiences.IsDefault ? [] : audiences; AssertionSummaries = assertionsummaries.IsDefault ? [] : assertionsummaries; Scopes = scopes.IsDefault ? [] : scopes;

        this.Permissions = permissions?.CloneByteArray() ?? Array.Empty<Byte>();
    }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/method[@name="ToClaims"]/*'/>*/
    internal AccessKeyClaims ToClaims(DateTimeOffset evaluatedat)
    {
        return new()
        {
            AccessKeyRealmID = AccessKeyRealmID,
            AssertionSummaries = AssertionSummaries,
            Audiences = Audiences,
            Expired = NotAfter is not null && evaluatedat > NotAfter.Value,
            IssuedAt = IssuedAt,
            IssuerAccessManagerID = IssuerAccessManagerID,
            IssuerToolID = IssuerToolID,
            ManifestHash = ManifestHash,
            NotAfter = NotAfter,
            Permissions = this.Permissions.CloneByteArray(),
            Scopes = Scopes,
            Subject = Subject,
            Token = Token,
            ToolSchemaID = ToolSchemaID
        };
    }

    /**<include file='MembershipToken.xml' path='MembershipToken/struct[@name="MembershipToken"]/method[@name="Allows"]/*'/>*/
    public Boolean Allows(Int32 operation)
    {
        try
        {
            if(operation < 0) { return false; }

            Int32 bit = operation % AccessKeySecret.BitsPerBlock;

            Int32 blockindex = operation / AccessKeySecret.BitsPerBlock;

            Int32 blockoffset = blockindex * AccessKeySecret.BytesPerBlock;

            if(blockoffset + AccessKeySecret.BytesPerBlock > this.Permissions.Length) { return false; }

            UInt128 block = ReadUInt128BigEndian(this.Permissions.AsSpan(blockoffset,AccessKeySecret.BytesPerBlock));

            return ((block >> bit) & UInt128.One) == UInt128.One;
        }
        catch { return false; }
    }
}