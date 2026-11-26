namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetTransferIntegrityEvidence(WebApplication application)
    {
        application.MapPost("GetTransferIntegrityEvidence",
                   ([FromBody] GetTransferIntegrityEvidenceRequest? request,
                    [FromServices] IGrainFactory gf,
                    [FromServices] IDataItemSegmentedTransferService segmented,
                    [FromServices] IDataStreamTransferService stream,
                    HttpContext hc) => GetTransferIntegrityEvidence(request,gf,segmented,stream,hc))
                   .WithMetadata(new PublishedTransferRequestContextMetadata())
                   .Produces<GetTransferIntegrityEvidenceResponse>(StatusCodes.Status200OK)
                   .WithName("GetTransferIntegrityEvidence");
    }

    private async Task<IResult> GetTransferIntegrityEvidence(GetTransferIntegrityEvidenceRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService segmented , IDataStreamTransferService stream , HttpContext hc)
    {
        String? id = request?.ItemID.ToString();

        try
        {
            using DiagnosticActivity? activity = StartDiagnostic(hc)?.AddTag("id",id);

            String token = GetToken(hc); activity?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { SetErr(activity); return Unauthorized(); }

            if(request is null || request.ItemID == Guid.Empty || request.SessionID == Guid.Empty || request.PrefixLength < 0 || !Enum.IsDefined(request.TransferFamily)) { SetErr(activity); return BadRequest(); }

            TraceContext? tracecontext = GetTraceContext(hc,activity);

            IDataControlDataConfigsClient configs = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? silo = await configs.ResolveStorageAsync(token,CatalogAccess.Read,tracecontext,hc.RequestAborted); if(silo is null) { SetErr(activity); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(silo);

            GetTransferIntegrityEvidenceResponse response = request.TransferFamily switch
            {
                DataControlTransferFamily.Segmented => await segmented.GetTransferIntegrityEvidence(request,hc.RequestAborted),

                DataControlTransferFamily.Stream => await stream.GetTransferIntegrityEvidence(request,hc.RequestAborted),

                _ => throw new ArgumentException("Transfer family is invalid.",nameof(request)),
            };

            SetOk(activity);

            return Results.Ok(response);
        }
        catch ( ArgumentException ) { SetErr(null); return BadRequest(); }

        catch ( OperationFailedException failure )
        {
            Log.Error(failure,GetTransferIntegrityEvidenceFailID,id); SetErr(null);

            return request?.TransferFamily is DataControlTransferFamily.Stream ? MapStreamTransferFailure(failure) : MapSegmentedFailure(failure);
        }
        catch ( Exception error ) { Log.Error(error,GetTransferIntegrityEvidenceFailID,id); SetErr(null); return InternalError(); }
    }
}
