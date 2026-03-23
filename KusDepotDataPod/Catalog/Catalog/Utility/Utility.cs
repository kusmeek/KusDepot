namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public static Int32 GetGatewayPort()
    {
        var c = new ConfigurationBuilder()
            .AddJsonFile(ConfigFilePath,true).Build();

        return Int32.TryParse(c["GatewayPort"],out Int32 p) ? p : 0;
    }

    private static Int32 GetPort()
    {
        var c = new ConfigurationBuilder()
            .AddJsonFile(ConfigFilePath,true).Build();

        return Int32.TryParse(c["Port"],out Int32 p) ? p : 0;
    }

    private String GetURL() => this.URL;

    private void SetURL() { this.URL = $"https://localhost:{GetPort()}"; }

    private static String GetToken(HttpContext context) => context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", Ordinal) is false ? String.Empty : context.Request.Headers.Authorization.ToString()[7..];

    private static IResult InternalError(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status500InternalServerError, "Internal Server Error", detail));

    private static IResult BadRequest(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status400BadRequest, "Bad Request", detail));

    private static IResult Unauthorized() => Results.Problem(BuildProblem(StatusCodes.Status401Unauthorized, "Unauthorized"));

    private static ProblemDetails BuildProblem(Int32 status , String title , String? detail = null)
    {
        var _ = new ProblemDetails { Status = status, Title = title };

        if(!String.IsNullOrEmpty(detail)) { _.Detail = detail; }

        return _;
    }
}