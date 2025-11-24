namespace KusDepot.Data;

internal sealed class DataControlGlobalHandler : IExceptionHandler
{
    public async ValueTask<Boolean> TryHandleAsync(HttpContext context , Exception exception , CancellationToken cancel)
    {
        try
        {
            Log.Error(exception,DataControlFail);

            var d = new ProblemDetails { Status = StatusCodes.Status500InternalServerError , Title = "Internal Server Error" };

            context.Response.StatusCode = d.Status!.Value; await context.Response.WriteAsJsonAsync(d,cancel);

            return true;
        }
        catch ( Exception _ ) { Log.Fatal(_,DataControlFail); return false; }
    }
}