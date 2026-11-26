namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetTransferSessions(WebApplication application)
    {
        application.MapPost("GetTransferSessions",
                   ([FromBody] GetTransferSessionsRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService segmentedTransfer,
                   [FromServices] IDataStreamTransferService streamTransfer,
                   HttpContext hc) => { return GetTransferSessions(request,gf,segmentedTransfer,streamTransfer,hc); })
                   .Produces<DataControlServerSessionInfo[]>(StatusCodes.Status200OK)
                   .WithName("GetTransferSessions");
    }

    private async Task<IResult> GetTransferSessions(GetTransferSessionsRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService segmentedTransfer , IDataStreamTransferService streamTransfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.SessionID?.ToString() ?? request?.ItemID?.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            TraceContext? tracecontext = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(GetTransferSessionsUnAuthID,id); SetErr(_); return Unauthorized(); }

            IDataControlDataConfigsClient dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            if((await dc.CheckAdminAsync(t,tracecontext,hc.RequestAborted)).Status is not AdminCheckStatus.Authorized) { Log.Error(GetTransferSessionsUnAuthID,id); SetErr(_); return Unauthorized(); }

            DataControlServerSessionInfo[][] responses = await Task.WhenAll(
                segmentedTransfer.GetSessions(request,hc.RequestAborted),
                streamTransfer.GetSessions(request,hc.RequestAborted));

            DataControlServerSessionInfo[] response = responses.SelectMany(static sessions => sessions).ToArray();

            Log.Information(GetTransferSessionsSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,GetTransferSessionsBadArgID,id); return BadRequest(GetTransferSessionsBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,GetTransferSessionsFailID,id); return _.FailureCode is StreamTransferFailureCode ? MapStreamTransferFailure(_) : MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,GetTransferSessionsFailID,id); return InternalError(); }
    }
}