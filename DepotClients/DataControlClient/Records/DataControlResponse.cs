namespace KusDepot.Data.Clients;

/**<include file='DataControlResponse.xml' path='DataControlResponse/record[@name="DataControlResponse"]/main/*'/>*/
public sealed record DataControlResponse<T>
{
    /**<include file='DataControlResponse.xml' path='DataControlResponse/record[@name="DataControlResponse"]/property[@name="Content"]/*'/>*/
    public String? Content { get; init; }

    /**<include file='DataControlResponse.xml' path='DataControlResponse/record[@name="DataControlResponse"]/property[@name="Data"]/*'/>*/
    public T? Data { get; init; }

    /**<include file='DataControlResponse.xml' path='DataControlResponse/record[@name="DataControlResponse"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode StatusCode { get; init; }
}
