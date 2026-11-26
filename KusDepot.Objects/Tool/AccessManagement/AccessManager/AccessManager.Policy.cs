namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    ///<inheritdoc/>
    public virtual Boolean AccessCheck(AccessKey? key , Int32 operation)
    {
        try
        {
            if(operation < 0 || operation >= AccessKeySecret.MaxOperations) { return false; }

            if(key is SelfKey selfkey)
            {
                if(!CreateSelfAccessPolicyContext(selfkey,operation,out AccessPolicyContext selfcontext)) { return false; }

                return EvaluatePolicy(this.PolicyEngine,in selfcontext).Allowed;
            }

            if(TryGetPolicyMembership(key,out ToolManifestIdentity? manifestidentity,out MembershipToken membership,out DateTimeOffset evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive,out Boolean issueraccessmanagermatched,out Boolean issuertoolmatched) is false) { return false; }

            Byte[]? secret = key?.GetKey();

            Boolean operationallowed = membership.Allows(operation);

            if(UsesDefaultPolicyEngine && !IsSignedAccessKeySecret(secret) && !membership.IssuerAccessOnly) { return manifestmatched && !expired && membershipactive && operationallowed; }

            AccessKeyClaims claims = membership.ToClaims(evaluatedat);

            if(!TryGetSignatureValidation(manifestidentity,secret,out SignatureValidation signaturevalidation)) { return false; }

            AccessPolicyContext context = CreateAccessPolicyContext(manifestidentity,key,claims,evaluatedat,manifestmatched,expired,membershipactive,signaturevalidation,operation,operationallowed,membership.Subject,Tool,Logger);

            context = context with { IssuerAccessManagerMatched = issueraccessmanagermatched , IssuerToolMatched = issuertoolmatched };

            AccessPolicyDecision decision = EvaluatePolicy(this.PolicyEngine,in context);

            return decision.Allowed && IssuerAccessAllowed(membership,issueraccessmanagermatched,issuertoolmatched);
        }
        catch ( Exception _ ) { Logger?.Error(_,AccessCheckFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean AccessCheck(AccessKey? key = null , [CallerMemberName] String? operationname = null)
    {
        try
        {
            if(key is not SelfKey && !HasMemberships()) { return false; }

            if(key is null || String.IsNullOrEmpty(operationname)) { return false; }

            Int32 op = ProtectedOperationResolver.ResolveIndex(operationname);

            if(op < 0 && Tool is not null) { _ = ToolManifestRegistry.TryResolveOperation(Tool.GetType(),operationname,out op); }

            return op < 0 ? false : this.AccessCheck(key,op);
        }
        catch ( Exception _ ) { Logger?.Error(_,AccessCheckFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="Authorize"]/*'/>*/
    public virtual Boolean Authorize(AccessKey? key = null)
    {
        try
        {
            if(TryGetPolicyMembership(key,out ToolManifestIdentity? manifestidentity,out MembershipToken membership,out DateTimeOffset evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive,out Boolean issueraccessmanagermatched,out Boolean issuertoolmatched) is false) { return false; }

            Byte[]? secret = key?.GetKey();

            if(UsesDefaultPolicyEngine && !IsSignedAccessKeySecret(secret) && !membership.IssuerAccessOnly) { return manifestmatched && !expired && membershipactive; }

            if(!TryGetSignatureValidation(manifestidentity,secret,out SignatureValidation signaturevalidation)) { return false; }

            AccessPolicyContext context = CreateAuthorizationPolicyContext(manifestidentity,key,membership.ToClaims(evaluatedat),evaluatedat,manifestmatched,expired,membershipactive,signaturevalidation,membership.Subject,Tool,Logger);

            context = context with { IssuerAccessManagerMatched = issueraccessmanagermatched , IssuerToolMatched = issuertoolmatched };

            return EvaluatePolicy(this.PolicyEngine,in context).Allowed && IssuerAccessAllowed(membership,issueraccessmanagermatched,issuertoolmatched);
        }
        catch ( Exception _ ) { Logger?.Error(_,AuthorizeFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateAccessPolicyContext"]/*'/>*/
    protected virtual AccessPolicyContext CreateAccessPolicyContext(
        ToolManifestIdentity manifestidentity , AccessKey? key , AccessKeyClaims claims , DateTimeOffset evaluatedat , Boolean manifestmatched,
        Boolean expired , Boolean membershipactive , SignatureValidation signaturevalidation , Int32 operation , Boolean operationallowed , String subject , ITool? tool , ILogger? logger)
    {
        return new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            AssertionSummaries = claims.AssertionSummaries,
            Claims = claims,
            Expired = expired,
            EvaluatedAt = evaluatedat,
            Key = key,
            Logger = logger,
            ManifestHash = manifestidentity.ManifestHash,
            ManifestMatched = manifestmatched,
            MembershipActive = membershipactive,
            Mode = AccessPolicyMode.Access,
            Operation = operation,
            OperationAllowed = operationallowed,
            SignatureValidation = signaturevalidation,
            Subject = subject,
            Tool = tool,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateAuthorizationPolicyContext"]/*'/>*/
    protected virtual AccessPolicyContext CreateAuthorizationPolicyContext(
        ToolManifestIdentity manifestidentity , AccessKey? key , AccessKeyClaims claims , DateTimeOffset evaluatedat,
        Boolean manifestmatched , Boolean expired , Boolean membershipactive , SignatureValidation signaturevalidation , String subject , ITool? tool , ILogger? logger)
    {
        return new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            AssertionSummaries = claims.AssertionSummaries,
            Claims = claims,
            Expired = expired,
            EvaluatedAt = evaluatedat,
            Key = key,
            Logger = logger,
            ManifestHash = manifestidentity.ManifestHash,
            ManifestMatched = manifestmatched,
            MembershipActive = membershipactive,
            Mode = AccessPolicyMode.Authorize,
            SignatureValidation = signaturevalidation,
            Subject = subject,
            Tool = tool,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateRequestPolicyContext"]/*'/>*/
    protected virtual AccessPolicyRequestContext CreateRequestPolicyContext(AccessRequest request , ITool tool)
    {
        ArgumentNullException.ThrowIfNull(request); ArgumentNullException.ThrowIfNull(tool);

        TimeSpan? lifetime = request.Lifetime;
        Boolean lifetimevalid = lifetime is null || lifetime > TimeSpan.Zero;
        Boolean toollocked = tool.GetLocked();

        AccessPolicyRequestContext CreateContext(
            Boolean requestvalidated , String requestedkeytype , IEnumerable<Int32>? operations = null , Boolean externalrequest = false,
            Boolean hostmatched = false , Boolean managementcredentialvalid = false , Boolean servicematched = false)
        {
            return new()
            {
                Audiences = request.Audiences,
                Credential = request.Credential,
                ExternalRequest = externalrequest,
                HostMatched = hostmatched,
                IssuerAccessOnly = request.IssuerAccessOnly || request is ManagementRequest,
                Lifetime = lifetime,
                ManagementCredentialValid = managementcredentialvalid,
                Operations = operations?.ToImmutableArray() ?? request.Operations,
                Properties = request.Properties,
                Request = request,
                RequestedAssertions = request.RequestedAssertions,
                RequestedKeyType = requestedkeytype,
                RequestValidated = requestvalidated && lifetimevalid,
                Scopes = request.Scopes,
                ServiceMatched = servicematched,
                Subject = request.Subject?.Trim() ?? String.Empty
            };
        }

        switch(request)
        {
            case ManagementRequest managementrequest:
            {
                Boolean credentialvalid = false;

                switch(managementrequest.Credential)
                {
                    case ManagementKeyCredential managementkeycredential when String.IsNullOrWhiteSpace(managementkeycredential.Key) is false:
                    {
                        ManagementKey? managementkey = ManagementKey.Parse(managementkeycredential.Key,null);

                        credentialvalid = tool.CheckManager(managementkey) || tool.CheckOwner(managementkey);

                        break;
                    }

                    case CertificateCredential certificatecredential when certificatecredential.TryGetCertificate(out X509Certificate2? certificate):
                    {
                        using(certificate)
                        {
                            credentialvalid = tool.CheckOwner(new OwnerKey(certificate)) || tool.CheckManager(new ManagerKey(certificate));
                        }

                        break;
                    }
                }

                return CreateContext(credentialvalid,NormalizeRequestedKeyType(request.RequestedKeyType) ?? String.Empty,managementrequest.Operations,managementcredentialvalid: credentialvalid);
            }

            case HostRequest hostrequest:
            {
                Boolean hostmatched = !hostrequest.External && hostrequest.Host?.IsHosting(tool,tool) is true;

                return CreateContext(!toollocked && (hostrequest.External || hostmatched),NormalizeRequestedKeyType(request.RequestedKeyType) ?? String.Empty,hostrequest.Operations,externalrequest: hostrequest.External,hostmatched: hostmatched);
            }

            case ServiceRequest servicerequest:
            {
                Boolean servicetoolvalid = servicerequest.Tool?.GetID() is Guid id && id != Guid.Empty;

                Boolean servicematched = ReferenceEquals(tool,servicerequest.Tool);

                return CreateContext(!toollocked && servicetoolvalid,NormalizeRequestedKeyType(request.RequestedKeyType) ?? NormalizeRequestedKeyType(nameof(ServiceKey))!,
                    servicerequest.Operations ?? ProtectedOperationSets.Service,servicematched: servicematched);
            }

            case StandardRequest:
            {
                return CreateContext(!toollocked,NormalizeRequestedKeyType(request.RequestedKeyType) ?? String.Empty,request.Operations);
            }

            default:
            {
                return CreateContext(false,String.Empty);
            }
        }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateRequestEvaluationContext"]/*'/>*/
    protected virtual AccessPolicyContext CreateRequestEvaluationContext(
        ToolManifestIdentity manifestidentity , AccessRequest request , in AccessPolicyRequestContext requestcontext , ITool tool , ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(request); ArgumentNullException.ThrowIfNull(tool);

        return new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            EvaluatedAt = DateTimeOffset.UtcNow,
            Logger = logger,
            ManifestHash = manifestidentity.ManifestHash,
            ManifestMatched = true,
            Mode = AccessPolicyMode.Request,
            RequestContext = requestcontext,
            Subject = requestcontext.Subject,
            Tool = tool,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateRevocationPolicyContext"]/*'/>*/
    protected virtual AccessPolicyContext CreateRevocationPolicyContext(
        ToolManifestIdentity manifestidentity , AccessKey? key , AccessKeyClaims claims , DateTimeOffset evaluatedat,
        Boolean manifestmatched , Boolean expired , Boolean membershipactive , SignatureValidation signaturevalidation,
        String subject , ITool? tool , ILogger? logger)
    {
        return new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            AssertionSummaries = claims.AssertionSummaries,
            Claims = claims,
            Expired = expired,
            EvaluatedAt = evaluatedat,
            Key = key,
            Logger = logger,
            ManifestHash = manifestidentity.ManifestHash,
            ManifestMatched = manifestmatched,
            MembershipActive = membershipactive,
            Mode = AccessPolicyMode.Revoke,
            SignatureValidation = signaturevalidation,
            Subject = subject,
            Tool = tool,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateSelfAccessPolicyContext"]/*'/>*/
    private Boolean CreateSelfAccessPolicyContext(SelfKey selfkey , Int32 operation , out AccessPolicyContext context)
    {
        ToolManifestIdentity? manifestidentity = ManifestIdentity;

        if(operation < 0 || operation >= AccessKeySecret.MaxOperations || !ReferenceEquals(ToolSelfKey,selfkey) || Tool is null || manifestidentity is null)
        {
            context = default;

            return false;
        }

        context = new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            EvaluatedAt = DateTimeOffset.UtcNow,
            Key = selfkey,
            Logger = Logger,
            ManifestHash = manifestidentity.ManifestHash,
            Mode = AccessPolicyMode.SelfAccess,
            Operation = operation,
            OperationAllowed = false,
            Subject = String.Empty,
            Tool = Tool,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };

        return true;
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="EvaluatePolicyStatic"]/*'/>*/
    private static AccessPolicyDecision EvaluatePolicy(IAccessPolicyEngineMode policyengine , in AccessPolicyContext context)
    {
        ArgumentNullException.ThrowIfNull(policyengine);

        if(policyengine is IAccessPolicyEngine synchronous) { return synchronous.Evaluate(in context); }

        if(policyengine is IAsyncAccessPolicyEngine)
        {
            throw new InvalidOperationException(SynchronousAccessManagerRequiresSynchronousPolicyEngine);
        }

        throw new InvalidOperationException(ConfiguredPolicyEngineModeInvalid);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="EvaluatePolicyAsyncStatic"]/*'/>*/
    private static async Task<AccessPolicyDecision> EvaluatePolicyAsync(IAccessPolicyEngineMode policyengine , AccessPolicyContext context , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(policyengine);

        cancel.ThrowIfCancellationRequested();

        return policyengine switch
        {
            IAsyncAccessPolicyEngine asynchronous => await asynchronous.EvaluateAsync(context,cancel).ConfigureAwait(false),

            IAccessPolicyEngine synchronous => synchronous.Evaluate(in context),

            _ => throw new InvalidOperationException(ConfiguredPolicyEngineModeInvalid)
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IsSignedAccessKeySecret"]/*'/>*/
    private static Boolean IsSignedAccessKeySecret(Byte[]? secret)
    {
        return secret is not null && secret.Length > 0 && AccessKeySecret.IsSigned(secret);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssuerAccessAllowed"]/*'/>*/
    private static Boolean IssuerAccessAllowed(MembershipToken membership , Boolean issueraccessmanagermatched , Boolean issuertoolmatched)
    {
        return !membership.IssuerAccessOnly || issueraccessmanagermatched && issuertoolmatched;
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="ResolvePolicyEngine"]/*'/>*/
    private void ResolvePolicyEngine()
    {
        this.PolicyEngine = this.Options?.PolicyEngine ?? DefaultAccessPolicyEngine.Instance;

        ValidatePolicyEngineMode(this.PolicyEngine);

        this.UsesDefaultPolicyEngine = ReferenceEquals(this.PolicyEngine,DefaultAccessPolicyEngine.Instance);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="ValidatePolicyEngineMode"]/*'/>*/
    private static void ValidatePolicyEngineMode(IAccessPolicyEngineMode? policyengine)
    {
        if(policyengine is null) { return; }

        if(policyengine is IAccessPolicyEngine or IAsyncAccessPolicyEngine) { return; }

        throw new InvalidOperationException(ConfiguredPolicyEngineModeInvalid);
    }
}