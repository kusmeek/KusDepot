namespace KusDepot.Data;

internal sealed partial class Catalog
{
    private static void MapSearchTags(WebApplication application)
    {
        application.MapPost("Catalog/Tags",
                   ([FromBody] TagRequest? search,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return SearchTags(search,af,dc,hc);})
                   .Produces<TagResponse>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<TagResponse>(StatusCodes.Status404NotFound)
                   .Produces<String?>(StatusCodes.Status500InternalServerError)
                   .WithName("SearchTags")
                   .WithOpenApi(o=>{o.OperationId = "SearchTags";return o;});
    }

    private static IResult SearchTags(TagRequest? search , IArkKeeperFactory af , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"SearchTags");

            String tk = GetToken(hc); _?.AddTag("enduser.id",GetUPN(tk));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(tk)) { Log.Error(STUnAuth); SetErr(_); return Results.Unauthorized(); }

            if(search is null || search.Tags is null || Equals(search.Tags.Length,0)) { Log.Error(BadArg); SetErr(_); return Results.BadRequest(); }

            StorageSilo? s = dc.GetAuthorizedReadSilo(tk,dt,ds).Result; if(s is null) { Log.Error(STUnAuth); SetErr(_); return Results.Unauthorized(); }

            IArkKeeper k = af.Create(s.CatalogName); Ark? a = Ark.Parse(k.GetArk(dt,ds).Result); if(a is null) { Log.Error(STArkNull); SetErr(_); return Results.Problem(); }

            DataTable _t = a.Tables[Ark.TagsTableName]!;

            HashSet<Guid> t = new HashSet<Guid>(
                from DataRow _r in _t.Rows
                where search.Tags.All(tag => _t.Columns.Cast<DataColumn>().Any(_c => _c.ColumnName != "ID" && !(_r[_c] is DBNull) && ((String)_r[_c]).Contains(tag,StringComparison.OrdinalIgnoreCase)))
                select (Guid)_r["ID"]
            );

            if(Equals(t.Count,0)) { SetOk(_); return Results.NotFound(new TagResponse()); }

            SetOk(_); return Results.Ok(new TagResponse(){ IDs = t });
        }
        catch ( Exception _ ) { Log.Error(_,STFail); return Results.Problem(); }
    }

    private static List<String>? GetTags(Ark? ark , Guid? id)
    {
        try
        {
            if(id is null || ark is null || ark.Tables[Ark.TagsTableName]!.Rows.Count == 0) { return null; }

            DataRow? _r = ark.Tables[Ark.TagsTableName]!.Rows.Cast<DataRow>().FirstOrDefault(_=>Guid.Equals(_["ID"],id));

            if(_r is null) { return null; } List<String> _l = new List<String>();

            for(Int32 _ = 1; _ < _r.ItemArray.Length; _++) { if(_r[_] is DBNull) { break; } _l.Add((String)_r[_]);}

            if(Equals(_l.Count,0)) { return null; } return _l;
        }
        catch ( Exception _ ) { Log.Error(_,GTFail); return null; }
    }
}