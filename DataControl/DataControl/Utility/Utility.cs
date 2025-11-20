namespace KusDepot.Data;

public sealed partial class DataControl
{
    private static void DeleteBlobActor(ActorId id)
    {
        new Thread( () =>
        {
            try { ActorServiceProxy.Create(ServiceLocators.BlobService,id).DeleteActorAsync(id,CancellationToken.None).GetAwaiter().GetResult(); } catch {}
        })
        .Start();
    }

    private String GetURL() { return $"https://{this.Context.NodeContext.IPAddressOrFQDN}:{this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port}"; }

    private Int32 GetPort() { if(Int32.TryParse(this.URL.Split(':').ElementAt(2),out Int32 p)) { return p; } return this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port; }

    private static String GetToken(HttpContext context) => context.Request.Headers.Authorization.ToString().StartsWith("Bearer ",Ordinal) is false ? String.Empty : context.Request.Headers.Authorization.ToString()[7..];

    private static IResult InternalError(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status500InternalServerError,"Internal Server Error",detail));

    private static IResult Unprocessable(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status422UnprocessableEntity,"Unprocessable Entity",detail));

    private static IResult BadRequest(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status400BadRequest,"Bad Request",detail));

    private static IResult Conflict(String? detail = null) => Results.Problem(BuildProblem(StatusCodes.Status409Conflict,"Conflict",detail));

    private static IResult Unauthorized() => Results.Problem(BuildProblem(StatusCodes.Status401Unauthorized,"Unauthorized"));

    private static ProblemDetails BuildProblem(Int32 status , String title , String? detail = null)
    {
        var _ = new ProblemDetails { Status = status , Title = title };

        if(!String.IsNullOrEmpty(detail)) { _.Detail = detail; }

        return _;
    }
}