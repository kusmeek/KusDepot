namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetStreamTransferSegment(WebApplication application)
    {
        application.MapPost("GetStreamTransferSegment",
                   ([FromBody] GetStreamTransferSegmentRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return GetStreamTransferSegment(request,gf,transfer,hc); })
                   .WithName("GetStreamTransferSegment").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> GetStreamTransferSegment(GetStreamTransferSegmentRequest? request , IGrainFactory gf , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(GetStreamTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(GetStreamTransferSegmentBadArgID,id); SetErr(_); return BadRequest(GetStreamTransferSegmentBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(GetStreamTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            using StreamTransferReadResult response = await transfer.GetStreamTransferSegment(request,hc.RequestAborted);

            StreamTransferSegmentFooter footerTemplate = response.Footer with { SegmentSHA512 = new Byte[64] };

            Byte[] footerTemplateBytes = JsonUtility.Serialize(footerTemplate);

            hc.Response.ContentType = TransferEnvelope.MediaType;

            await TransferEnvelope.WriteAsync(hc.Response.Body,ReadOnlyMemory<Byte>.Empty,response.Payload,response.Footer.ReturnedLength,footerTemplateBytes.Length,hc.RequestAborted);

            Byte[] footerBytes = JsonUtility.Serialize(response.FinalizeFooter());

            if(footerBytes.Length != footerTemplateBytes.Length) { throw new InvalidDataException(); }

            if(footerBytes.Length > 0) { await hc.Response.Body.WriteAsync(footerBytes,hc.RequestAborted); }

            Log.Information(GetStreamTransferSegmentSuccessID,id); SetOk(_); return Results.Empty;
        }
        catch ( ArgumentException _ ) { Log.Error(_,GetStreamTransferSegmentBadArgID,id); return BadRequest(GetStreamTransferSegmentBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,GetStreamTransferSegmentFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,GetStreamTransferSegmentFailID,id); return InternalError(); }
    }
}
