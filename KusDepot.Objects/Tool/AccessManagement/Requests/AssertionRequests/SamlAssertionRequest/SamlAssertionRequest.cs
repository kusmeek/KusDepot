namespace KusDepot.Security.Requests;

/**<include file='SamlAssertionRequest.xml' path='SamlAssertionRequest/class[@name="SamlAssertionRequest"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.SamlAssertionRequest")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public record class SamlAssertionRequest : AccessAssertionRequest
{
    /**<include file='SamlAssertionRequest.xml' path='SamlAssertionRequest/class[@name="SamlAssertionRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public SamlAssertionRequest() { AssertionType = "saml2"; }
}