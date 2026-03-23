namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchNotes(WebApplication application)
    {
        application.MapPost("Catalog/Notes",
                   ([FromBody] NoteQuery? search,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return SearchNotes(search,gf,hc);})
                   .Produces<NoteResponse>(StatusCodes.Status200OK)
                   .Produces<NoteResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchNotes").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchNotes(NoteQuery? search , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            ETW.Log.SNStart();

            using DiagnosticActivity? _ = StartDiagnostic(hc); String t = GetToken(hc);

            _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(SNUnAuth); SetErr(_); ETW.Log.SNError(SNUnAuth); return Unauthorized(); }

            if(search is null || search.Notes is null || Equals(search.Notes.Length,0)) { Log.Error(BadArg); SetErr(_); ETW.Log.SNError(BadArg); return BadRequest(BadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToStringInvariant()!);

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(SNUnAuth); SetErr(_); ETW.Log.SNError(SNUnAuth); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            var f = await c.SearchNotes(search,dt,ds);

            if(Equals(f.IDs.Count,0)) { SetOk(_); ETW.Log.SNSuccess(); return Results.NotFound(new NoteResponse()); }

            SetOk(_); ETW.Log.SNSuccess(); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SNFail); ETW.Log.SNError(_.Message); return InternalError(); }
    }
}