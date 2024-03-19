namespace KusDepot.Data;

internal sealed partial class Catalog
{
    private static void MapSearchNotes(WebApplication application)
    {
        application.MapPost("Catalog/Notes",
                   ([FromBody] NoteRequest? search,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return SearchNotes(search,af,dc,hc);})
                   .Produces<NoteResponse>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<NoteResponse>(StatusCodes.Status404NotFound)
                   .Produces<String?>(StatusCodes.Status500InternalServerError)
                   .WithName("SearchNotes")
                   .WithOpenApi(o=>{o.OperationId = "SearchNotes";return o;});
    }

    private static IResult SearchNotes(NoteRequest? search , IArkKeeperFactory af , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"SearchNotes");

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(SNUnAuth); SetErr(_); return Results.Unauthorized(); }

            if(search is null || search.Notes is null || Equals(search.Notes.Length,0)) { Log.Error(BadArg); SetErr(_); return Results.BadRequest(); }

            StorageSilo? s = dc.GetAuthorizedReadSilo(t,dt,ds).Result; if(s is null) { Log.Error(SNUnAuth); SetErr(_); return Results.Unauthorized(); }

            IArkKeeper k = af.Create(s.CatalogName); Ark? a = Ark.Parse(k.GetArk(dt,ds).Result); if(a is null) { Log.Error(SNArkNull); SetErr(_); return Results.Problem(); }

            DataTable _t = a.Tables[Ark.NotesTableName]!;

            HashSet<Guid> n = new HashSet<Guid>(
                from DataRow _r in _t.Rows
                where search.Notes.All(note => _t.Columns.Cast<DataColumn>().Any(_c => _c.ColumnName != "ID" && !(_r[_c] is DBNull) && ((String)_r[_c]).Contains(note,StringComparison.OrdinalIgnoreCase)))
                select (Guid)_r["ID"]
            );

            if(Equals(n.Count,0)) { SetOk(_); return Results.NotFound(new NoteResponse()); }

            SetOk(_); return Results.Ok(new NoteResponse(){ IDs = n });
        }
        catch ( Exception _ ) { Log.Error(_,SNFail); return Results.Problem(); }
    }

    private static List<String>? GetNotes(Ark? ark , Guid? id)
    {
        try
        {
            if(id is null || ark is null || ark.Tables[Ark.NotesTableName]!.Rows.Count == 0) { return null; }

            DataRow? _r = ark.Tables[Ark.NotesTableName]!.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],id));

            if(_r is null) { return null; } List<String> _l = new List<String>();

            for(Int32 _ = 1; _ < _r.ItemArray.Length; _++) { if(_r[_] is DBNull) { break; } _l.Add((String)_r[_]); }

            if(Equals(_l.Count,0)) { return null; } return _l;
        }
        catch ( Exception _ ) { Log.Error(_,GNFail); return null; }
    }
}