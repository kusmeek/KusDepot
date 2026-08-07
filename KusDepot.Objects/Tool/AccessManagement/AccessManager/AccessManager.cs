namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager : IAccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/constructor[@name="Constructor"]/*'/>*/
    public AccessManager(ITool? tool = null , ILogger? logger = null) : this(null,tool,logger) {}

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/constructor[@name="ConstructorOptions"]/*'/>*/
    public AccessManager(AccessManagerOptions? options , ITool? tool = null , ILogger? logger = null) : this(options?.MembershipCacheSize)
    {
        try
        {
            this.Options = options; Byte[]? realmkey = this.Options?.RealmKey; this.KeyMaker = this.Options?.KeyMaker;

            this.KeyAssertionGenerator = this.Options?.KeyAssertionGenerator; this.SignatureProcessor = this.Options?.SignatureProcessor;

            ValidateAssertionGeneratorMode(this.KeyAssertionGenerator); ValidateSignatureProcessorMode(this.SignatureProcessor);

            if(realmkey is not null && realmkey.Length != AccessKeySecret.SymmetricKeySize) { throw new ArgumentException(KeySizeInvalid,nameof(options)); }

            this.RealmKey = realmkey?.CloneByteArray();

            this.ID = this.Options?.ID is Guid id && id != Guid.Empty ? id : options is null ? null : Guid.NewGuid();

            this.Initialize(tool,logger);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,this.GetType().Name,this.ID?.ToString()); if(MyNoExceptions) { return; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/constructor[@name="ConstructorPrivate"]/*'/>*/
    private AccessManager(Int32? membershipcachesize = MembershipCache.DefaultMaximumCount)
    {
        this.AccessKeys = new(); this.MembershipsByToken = new(); this.MembershipCache = new(membershipcachesize.GetValueOrDefault());
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.DestroySecrets)]
    public virtual Boolean DestroySecrets(AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return false; }

            if(!TryEnter(AccessKeys,SyncTime)) { throw SyncException; }

            foreach(HashSet<AccessKeyToken> s in AccessKeys.Values) { foreach(AccessKeyToken t in s) { t.Clear(); } s.Clear(); }

            AccessKeys.Clear();

            if(!TryEnter(MembershipsByToken,SyncTime)) { throw SyncException; }

            MembershipsByToken.Clear(); this.MembershipCache.Clear(); if(RealmKey is not null) { ZeroMemory(RealmKey); RealmKey = null; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,DestroySecretsFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally
        {
            if(IsEntered(MembershipsByToken)) { Exit(MembershipsByToken); }

            if(IsEntered(AccessKeys)) { Exit(AccessKeys); }
        }
    }

    ///<inheritdoc/>
    public virtual Boolean Initialize(ITool? tool = null , ILogger? logger = null)
    {
        try
        {
            if(this.Initialized || tool is null) { return false; }

            this.Tool = tool; this.Logger = logger; this.ID ??= Guid.NewGuid(); this.RealmKey ??= GenerateRealmKey();

            ResolvePolicyEngine();

            this.Initialized = this.RealmKey is not null && InitializeManifestIdentity();

            if(this.Initialized && this.Options?.InitialMembershipCatalog is not null)
            {
                AccessManagerMembershipCatalog initialmembershipcatalog = this.Options.InitialMembershipCatalog;

                this.Initialized = UpdateMembershipCatalog(initialmembershipcatalog,MembershipUpdateOperation.Add);

                if(this.Initialized) { this.Options.InitialMembershipCatalog = null; }
            }

            return this.Initialized;
        }
        catch ( Exception _ ) { Logger?.Error(_,InitializeFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="NormalizeRequestedKeyType"]/*'/>*/
    protected virtual String? NormalizeRequestedKeyType(String? requestedkeytype)
    {
        if(String.IsNullOrWhiteSpace(requestedkeytype)) { return null; }

        String normalized = requestedkeytype.Trim();

        return normalized switch
        {
            nameof(ClientKey)    or "KusDepot.Security.ClientKey"    => nameof(ClientKey),
            nameof(CommandKey)   or "KusDepot.Security.CommandKey"   => nameof(CommandKey),
            nameof(ExecutiveKey) or "KusDepot.Security.ExecutiveKey" => nameof(ExecutiveKey),
            nameof(HostKey)      or "KusDepot.Security.HostKey"      => nameof(HostKey),
            nameof(MyHostKey)    or "KusDepot.Security.MyHostKey"    => nameof(MyHostKey),
            nameof(ServiceKey)   or "KusDepot.Security.ServiceKey"   => nameof(ServiceKey),
            nameof(TokenKey)     or "KusDepot.Security.TokenKey"     => nameof(TokenKey),
            _                                                        => normalized
        };
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.QuerySnapshot)]
    public virtual AccessManagerSnapshot? QuerySnapshot(AccessManagerQuery? query = null , AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return null; }

            AccessManagerSnapshot snapshot = new();

            if(TryGetManifestIdentity(out ToolManifestIdentity? manifestidentity) is false) { return snapshot; }

            DateTimeOffset evaluatedat = DateTimeOffset.UtcNow;

            Dictionary<String,List<AccessManagerSnapshotEntry>> groupedclaims = new();

            foreach(MembershipToken membership in GetMembershipMetadataSnapshot())
            {
                if(!String.Equals(membership.ToolSchemaID,manifestidentity.ToolSchemaID,Ordinal) ||
                   !String.Equals(membership.AccessKeyRealmID,manifestidentity.AccessKeyRealmID,Ordinal) ||
                   !String.Equals(membership.ManifestHash,manifestidentity.ManifestHash,Ordinal))
                {
                    continue;
                }

                if(!IsActiveMember(membership.Subject,membership.Token)) { continue; }

                AccessKeyClaims claims = membership.ToClaims(evaluatedat);

                if(!MatchesAccessManagerQuery(claims,query)) { continue; }

                if(!groupedclaims.TryGetValue(membership.Subject,out List<AccessManagerSnapshotEntry>? subjectclaims))
                {
                    subjectclaims = new(); groupedclaims.Add(membership.Subject,subjectclaims);
                }

                subjectclaims.Add(AccessManagerSnapshotEntry.Create(claims));
            }

            snapshot.ToolManifestIdentity = manifestidentity;

            snapshot.AccessKeys = groupedclaims.ToDictionary(_ => new String(_.Key),_ => _.Value.ToImmutableArray());

            return snapshot;
        }
        catch ( Exception _ ) { Logger?.Error(_,QuerySnapshotFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual AccessKey? RequestAccess(AccessRequest? request = null)
    {
        try
        {
            if(request is null || Tool is null) { return null; }

            if(TryGetManifestIdentity(out ToolManifestIdentity? manifestidentity) is false) { return null; }

            AccessPolicyRequestContext requestcontext = CreateRequestPolicyContext(request,Tool);

            AccessPolicyContext context = CreateRequestEvaluationContext(manifestidentity,request,requestcontext,Tool,Logger);

            AccessPolicyDecision decision = EvaluatePolicy(this.PolicyEngine,in context);

            if(!decision.Allowed) { return null; }

            return IssueRequestedAccessKey(in requestcontext);
        }
        catch ( Exception _ ) { Logger?.Error(_,RequestAccessFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean RevokeAccess(AccessKey? key)
    {
        try
        {
            if(TryGetPolicyMembership(key,out ToolManifestIdentity? manifestidentity,out MembershipToken membershiptoken,out DateTimeOffset evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive) is false) { return false; }

            Byte[]? secret = key?.GetKey();

            if(UsesDefaultPolicyEngine && !IsSignedAccessKeySecret(secret))
            {
                if(!manifestmatched) { return false; }
            }

            else
            {
                if(!TryGetSignatureValidation(manifestidentity,key?.GetKey(),out SignatureValidation signaturevalidation)) { return false; }

                AccessPolicyContext context = CreateRevocationPolicyContext(manifestidentity,key,membershiptoken.ToClaims(evaluatedat),evaluatedat,manifestmatched,expired,membershipactive,signaturevalidation,membershiptoken.Subject,Tool,Logger);

                AccessPolicyDecision revocation = EvaluatePolicy(this.PolicyEngine,in context);

                if(!revocation.Allowed) { return false; }
            }

            Boolean removed = RemoveMembership(membershiptoken.Subject,membershiptoken.Token);

            if(removed && secret is not null && MembershipCacheKey.TryCreate(secret,out MembershipCacheKey cachekey))
            {
                _ = RemoveMembershipMetadata(membershiptoken.Token);

                _ = this.MembershipCache.Remove(cachekey);
            }

            else if(removed)
            {
                _ = RemoveMembershipMetadata(membershiptoken.Token);
            }

            return removed;
        }
        catch ( Exception _ ) { Logger?.Error(_,RevokeAccessFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }
}