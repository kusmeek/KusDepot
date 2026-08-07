namespace KusDepot.Security.Requests;

/**<include file='TokenAssertionRequest.xml' path='TokenAssertionRequest/class[@name="TokenAssertionRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[Alias("KusDepot.Security.Requests.TokenAssertionRequest")] [GenerateSerializer] [Immutable]

public record class TokenAssertionRequest : AccessAssertionRequest
{
    /**<include file='TokenAssertionRequest.xml' path='TokenAssertionRequest/class[@name="TokenAssertionRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public TokenAssertionRequest() { AssertionType = "token"; }
}