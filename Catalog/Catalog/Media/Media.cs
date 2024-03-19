namespace KusDepot.Data;

internal sealed partial class Catalog
{
    private static void MapSearchMedia(WebApplication application)
    {
        application.MapPost("Catalog/Media",
                   ([FromBody] MediaRequest? search,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return SearchMedia(search,af,dc,hc);})
                   .Produces<MediaResponse>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<MediaResponse>(StatusCodes.Status404NotFound)
                   .Produces<String?>(StatusCodes.Status500InternalServerError)
                   .WithName("SearchMedia")
                   .WithOpenApi(o=>{o.OperationId = "SearchMedia";return o;});
    }

    private static IResult SearchMedia(MediaRequest? search , IArkKeeperFactory af , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"SearchMedia")?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SMUnAuth); SetErr(_); return Results.Unauthorized(); }

            StorageSilo? s = dc.GetAuthorizedReadSilo(t,dt,ds).Result; if(s is null) { Log.Error(SMUnAuth); SetErr(_); return Results.Unauthorized(); }

            IArkKeeper k = af.Create(s.CatalogName); Ark? a = Ark.Parse(k.GetArk(dt,ds).Result); if(a is null) { Log.Error(SMArkNull); SetErr(_); return Results.Problem(); }

            List<Media> f = new List<Media>(); String g = GenerateMediaQuery(search); DataRow[] q = a.Tables[Ark.MediaLibraryTableName]!.Select(g);

            foreach(DataRow r in q)
            {
                Media m = new Media
                {
                    Album              = r["Album"]             is DBNull ? null : (String)r["Album"],
                    Application        = r["Application"]       is DBNull ? null : (String)r["Application"],
                    Artist             = r["Artist"]            is DBNull ? null : (String)r["Artist"],
                    BornOn             = r["BornOn"]            is DBNull ? null : (String)r["BornOn"],
                    DistinguishedName  = r["DistinguishedName"] is DBNull ? null : (String)r["DistinguishedName"],
                    ID                 = r["ID"]                is DBNull ? null : (Guid?) r["ID"],
                    Modified           = r["Modified"]          is DBNull ? null : (String)r["Modified"],
                    Name               = r["Name"]              is DBNull ? null : (String)r["Name"],
                    Notes              = r["ID"]                is DBNull ? null : GetNotes(a,(Guid?)r["ID"])?.ToArray(),
                    Tags               = r["ID"]                is DBNull ? null : GetTags(a,(Guid?)r["ID"])?.ToArray(),
                    Title              = r["Title"]             is DBNull ? null : (String)r["Title"],
                    Type               = r["Type"]              is DBNull ? null : (String)r["Type"],
                    Year               = r["Year"]              is DBNull ? null : (String)r["Year"],
                };
                f.Add(m);
            }

            f.RemoveAll(m => 
                search?.Notes != null && (m.Notes == null || !search.Notes.All(note => m.Notes.Any(_note => _note?.Contains(note,StringComparison.OrdinalIgnoreCase) == true))) ||
                search?.Tags != null && (m.Tags == null || !search.Tags.All(tag => m.Tags.Any(_tag => _tag?.Contains(tag,StringComparison.OrdinalIgnoreCase) == true))));

            if(Equals(f.Count,0)) { SetOk(_); return Results.NotFound(new MediaResponse()); }

            SetOk(_); return Results.Ok(new MediaResponse(){ Media = f.ToArray() });
        }
        catch ( Exception _ ) { Log.Error(_,SMFail); return Results.Problem(); }
    }

    private static String GenerateMediaQuery(MediaRequest? search)
    {
        try
        {
            if(search is null) { return String.Empty; } List<String> w = new();

            if(!String.IsNullOrEmpty(search.Album))             { w.Add($"Album LIKE '%{search.Album}%'"); }
            if(!String.IsNullOrEmpty(search.Application))       { w.Add($"Application LIKE '%{search.Application}%'"); }
            if(!String.IsNullOrEmpty(search.Artist))            { w.Add($"Artist LIKE '%{search.Artist}%'"); }
            if(!String.IsNullOrEmpty(search.BornOn))            { w.Add($"BornOn LIKE '%{search.BornOn}%'"); }
            if(!String.IsNullOrEmpty(search.DistinguishedName)) { w.Add($"DistinguishedName LIKE '%{search.DistinguishedName}%'"); }
            if(!String.IsNullOrEmpty(search.ID?.ToString()))    { w.Add($"ID='{search.ID}'"); }
            if(!String.IsNullOrEmpty(search.Modified))          { w.Add($"Modified LIKE '%{search.Modified}%'"); }
            if(!String.IsNullOrEmpty(search.Name))              { w.Add($"Name LIKE '%{search.Name}%'"); }
            if(!String.IsNullOrEmpty(search.Title))             { w.Add($"Title LIKE '%{search.Title}%'"); }
            if(!String.IsNullOrEmpty(search.Type))              { w.Add($"Type LIKE '%{search.Type}%'"); }
            if(!String.IsNullOrEmpty(search.Year))              { w.Add($"Year LIKE '%{search.Year}%'"); }

            return String.Join(" AND ",w);
        }
        catch ( Exception _ ) { Log.Error(_,SMQueryFail); return String.Empty; }
    }
}