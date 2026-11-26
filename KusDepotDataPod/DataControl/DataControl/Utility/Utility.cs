namespace DataPodServices.DataControl;

public sealed partial class DataControl
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

    private static String GetToken(HttpContext context) => context.Request.Headers.Authorization.ToString().StartsWith("Bearer ",Ordinal) is false ? String.Empty : context.Request.Headers.Authorization.ToString()[7..];

    private static IResult InternalError(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status500InternalServerError,"Internal Server Error",detail));

    private static IResult Unprocessable(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status422UnprocessableEntity,"Unprocessable Entity",detail));

    private static IResult BadRequest(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status400BadRequest,"Bad Request",detail));

    private static IResult Conflict(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status409Conflict,"Conflict",detail));

    private static IResult PreconditionFailed(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status412PreconditionFailed,"Precondition Failed",detail));

    private static IResult NotFound(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status404NotFound,"Not Found",detail));

    private static IResult Unauthorized() => Results.Problem(BuildProblem(StatusCodes.Status401Unauthorized,"Unauthorized"));

    private static IResult BlockLimitExceeded(PublicationBlockLimitException error , Guid sessionid)
    {
        ProblemDetails problem = BuildProblem(StatusCodes.Status409Conflict,"Publication block limit exceeded",error.Message);
        problem.Type = PublicationBlockLimitProblemType;
        problem.Extensions["sessionId"] = sessionid;
        problem.Extensions["currentBlocks"] = error.CurrentBlocks;
        problem.Extensions["requestedBlocks"] = error.RequestedBlocks;
        problem.Extensions["maximumBlocks"] = TenantTransferStorageLimits.MaximumBlockCount;

        return Results.Problem(problem);
    }

    private static ProblemDetails BuildProblem(Int32 status , String title , String? detail = null)
    {
        var _ = new ProblemDetails { Status = status , Title = title };

        if(!String.IsNullOrEmpty(detail)) { _.Detail = detail; }

        return _;
    }
}