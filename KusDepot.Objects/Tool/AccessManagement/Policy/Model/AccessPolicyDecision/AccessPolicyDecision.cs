namespace KusDepot.Security.Policy;

/**<include file='AccessPolicyDecision.xml' path='AccessPolicyDecision/struct[@name="AccessPolicyDecision"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Policy.AccessPolicyDecision")]

public readonly struct AccessPolicyDecision
{
    /**<include file='AccessPolicyDecision.xml' path='AccessPolicyDecision/struct[@name="AccessPolicyDecision"]/property[@name="Allowed"]/*'/>*/
    [JsonPropertyName("Allowed")] [JsonRequired] [Id(0)]
    public Boolean Allowed { get; init; }

    /**<include file='AccessPolicyDecision.xml' path='AccessPolicyDecision/struct[@name="AccessPolicyDecision"]/property[@name="Reason"]/*'/>*/
    [JsonPropertyName("Reason")] [JsonRequired] [Id(1)]
    public String Reason { get; init; } = String.Empty;

    /**<include file='AccessPolicyDecision.xml' path='AccessPolicyDecision/struct[@name="AccessPolicyDecision"]/constructor[@name="Constructor"]/*'/>*/
    public AccessPolicyDecision() {}

    /**<include file='AccessPolicyDecision.xml' path='AccessPolicyDecision/struct[@name="AccessPolicyDecision"]/method[@name="Allow"]/*'/>*/
    public static AccessPolicyDecision Allow()
    {
        return new() { Allowed = true };
    }

    /**<include file='AccessPolicyDecision.xml' path='AccessPolicyDecision/struct[@name="AccessPolicyDecision"]/method[@name="Deny"]/*'/>*/
    public static AccessPolicyDecision Deny(String? reason = null)
    {
        return new() { Allowed = false , Reason = reason ?? String.Empty };
    }
}