namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public static Int32 GetGatewayPort(IConfiguration configuration)
    {
        return configuration.GetValue<Int32>("GatewayPort");
    }

    private static Int32 GetPort(IConfiguration configuration)
    {
        return configuration.GetValue<Int32>("Port");
    }

    private String GetURL() => this.URL;

    internal void SetURL(IConfiguration configuration) { this.URL = $"http://localhost:{GetPort(configuration)}"; }

    private static String GetToken(HttpContext context) => context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", Ordinal) is false ? String.Empty : context.Request.Headers.Authorization.ToString()[7..];

    private static IResult InternalError(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status500InternalServerError, "Internal Server Error", detail));

    private static IResult BadRequest(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status400BadRequest, "Bad Request", detail));

    private static IResult? ValidateCatalogPaging(Int32? pageSize , String? pageToken)
    {
        try
        {
            CatalogPageSize.Resolve(pageSize);

            CatalogPageTokenCodec.Decode(pageToken);

            return null;
        }
        catch ( CatalogPageRequestException exception )
        {
            return BadRequest(exception.Message);
        }
    }

    private static IResult Unauthorized() => Results.Problem(BuildProblem(StatusCodes.Status401Unauthorized, "Unauthorized"));

    private static ProblemDetails BuildProblem(Int32 status , String title , String? detail = null)
    {
        var _ = new ProblemDetails { Status = status, Title = title };

        if(!String.IsNullOrEmpty(detail)) { _.Detail = detail; }

        return _;
    }
}