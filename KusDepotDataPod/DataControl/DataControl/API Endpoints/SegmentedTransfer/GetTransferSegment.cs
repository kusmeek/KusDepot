namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetTransferSegment(WebApplication application)
    {
        application.MapPost("GetTransferSegment",
                   ([FromBody] GetTransferSegmentRequest? request,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return GetTransferSegment(request,transfer,hc); })
                   .WithMetadata(new PublishedTransferRequestContextMetadata())
                   .WithName("GetTransferSegment");
    }

    private async Task<IResult> GetTransferSegment(GetTransferSegmentRequest? request , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(GetTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(GetTransferSegmentBadArgID,id); SetErr(_); return BadRequest(GetTransferSegmentBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Read,tc,hc.RequestAborted);

            if(s is null) { Log.Error(GetTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            using SegmentedTransferReadResult response = await transfer.GetTransferSegment(request,hc.RequestAborted);

            TransferPayloadRateLimiters rateLimiters = hc.RequestServices.GetRequiredService<TransferPayloadRateLimiters>();

            TransferSegmentFooter footerTemplate = response.Footer with { SegmentSHA512 = new Byte[64] };

            Byte[] footerTemplateBytes = JsonUtility.Serialize(footerTemplate);

            hc.Response.ContentType = TransferEnvelope.MediaType;

            using RateLimitedReadStream? limitedPayload = rateLimiters.Download is AsyncByteRateLimiter limiter
                ? new RateLimitedReadStream(response.Payload,limiter,leaveopen:true)
                : null;

            await TransferEnvelope.WriteAsync(hc.Response.Body,ReadOnlyMemory<Byte>.Empty,limitedPayload ?? response.Payload,response.Footer.ReturnedLength,footerTemplateBytes.Length,hc.RequestAborted);

            Byte[] footerBytes = JsonUtility.Serialize(response.FinalizeFooter());

            if(footerBytes.Length != footerTemplateBytes.Length) { throw new InvalidDataException(); }

            if(footerBytes.Length > 0) { await hc.Response.Body.WriteAsync(footerBytes,hc.RequestAborted); }

            Log.Information(GetTransferSegmentSuccessID,id); SetOk(_); return Results.Empty;
        }
        catch ( ArgumentException _ )
        {
            Log.Error(_,GetTransferSegmentBadArgID,id);

            if(hc.Response.HasStarted) { throw; }

            return BadRequest(GetTransferSegmentBadArg);
        }

        catch ( OperationFailedException _ )
        {
            Log.Error(_,GetTransferSegmentFailID,id);

            if(hc.Response.HasStarted) { throw; }

            return MapSegmentedFailure(_);
        }

        catch ( Exception _ )
        {
            Log.Error(_,GetTransferSegmentFailID,id);

            if(hc.Response.HasStarted) { throw; }

            return InternalError();
        }
    }
}
