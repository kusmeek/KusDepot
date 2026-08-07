namespace KusDepot.Security.Policy;

/**<include file='AccessPolicyDecisionStrings.xml' path='AccessPolicyDecisionStrings/class[@name="AccessPolicyDecisionStrings"]/main/*'/>*/
public static class AccessPolicyDecisionStrings
{
    #pragma warning disable CS1591

    public const String AccessKeyExpired = "Access key is expired.";

    public const String AccessKeyNotActiveMembership = "Access key is not an active membership.";

    public const String AccessRequestIsNotValid = "Access request is not valid.";

    public const String ManifestIdentityDoesNotMatchCurrentToolState = "Manifest identity does not match current tool state.";

    public const String ProtectedOperationIsNotAllowed = "Protected operation is not allowed.";

    public const String ProtectedOperationIsRequired = "Protected operation is required.";

    public const String RequestContextIsRequired = "Request context is required.";

    public const String RequestedKeyTypeIsRequired = "Requested key type is required.";

    public const String SubjectDoesNotMatchRequiredSubject = "Subject does not match required subject.";

    public const String SubjectIsRequired = "Subject is required.";

    public const String UnknownPolicyMode = "Unknown policy mode.";

    public const String ValidatedClaimsAreRequired = "Validated claims are required.";
}