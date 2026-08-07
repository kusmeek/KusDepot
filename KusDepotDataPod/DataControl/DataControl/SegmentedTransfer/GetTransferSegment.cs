namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetTransferSegment(WebApplication application)
    {
        application.MapPost("GetTransferSegment",
                   ([FromBody] GetTransferSegmentRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return GetTransferSegment(request,gf,transfer,hc); })
                   .WithName("GetTransferSegment").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> GetTransferSegment(GetTransferSegmentRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(GetTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(GetTransferSegmentBadArgID,id); SetErr(_); return BadRequest(GetTransferSegmentBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(GetTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            using SegmentedTransferReadResult response = await transfer.GetTransferSegment(request,hc.RequestAborted);

            TransferSegmentFooter footerTemplate = response.Footer with { SegmentSHA512 = new Byte[64] };

            Byte[] footerTemplateBytes = JsonUtility.Serialize(footerTemplate);

            hc.Response.ContentType = TransferEnvelope.MediaType;

            await TransferEnvelope.WriteAsync(hc.Response.Body,ReadOnlyMemory<Byte>.Empty,response.Payload,response.Footer.ReturnedLength,footerTemplateBytes.Length,hc.RequestAborted);

            Byte[] footerBytes = JsonUtility.Serialize(response.FinalizeFooter());

            if(footerBytes.Length != footerTemplateBytes.Length) { throw new InvalidDataException(); }

            if(footerBytes.Length > 0) { await hc.Response.Body.WriteAsync(footerBytes,hc.RequestAborted); }

            Log.Information(GetTransferSegmentSuccessID,id); SetOk(_); return Results.Empty;
        }
        catch ( ArgumentException _ ) { Log.Error(_,GetTransferSegmentBadArgID,id); return BadRequest(GetTransferSegmentBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,GetTransferSegmentFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,GetTransferSegmentFailID,id); return InternalError(); }
    }
}
