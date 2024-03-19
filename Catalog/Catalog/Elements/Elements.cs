namespace KusDepot.Data;

internal sealed partial class Catalog
{
    private static void MapSearchElements(WebApplication application)
    {
        application.MapPost("Catalog/Elements",
                   ([FromBody] ElementRequest? search,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return SearchElements(search,af,dc,hc);})
                   .Produces<ElementResponse>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<ElementResponse>(StatusCodes.Status404NotFound)
                   .Produces<String?>(StatusCodes.Status500InternalServerError)
                   .WithName("SearchElements")
                   .WithOpenApi(o=>{o.OperationId = "SearchElements";return o;});
    }

    private static IResult SearchElements(ElementRequest? search , IArkKeeperFactory af , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"SearchElements")?.AddTag("id",search?.ID);;

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SELUnAuth); SetErr(_); return Results.Unauthorized(); }

            StorageSilo? s = dc.GetAuthorizedReadSilo(t,dt,ds).Result; if(s is null) { Log.Error(SELUnAuth); SetErr(_); return Results.Unauthorized(); }

            IArkKeeper k = af.Create(s.CatalogName); Ark? a = Ark.Parse(k.GetArk(dt,ds).Result); if(a is null) { Log.Error(SELArkNull); SetErr(_); return Results.Problem(); }

            List<Element> f = new List<Element>(); String g = GenerateElementsQuery(search); DataRow[] q = a.Tables[Ark.ElementsTableName]!.Select(g);

            foreach(DataRow r in q)
            {
                Element e = new Element
                {
                    Application        = r["Application"]        is DBNull ? null : (String)r["Application"],
                    ApplicationVersion = r["ApplicationVersion"] is DBNull ? null : (String)r["ApplicationVersion"],
                    BornOn             = r["BornOn"]             is DBNull ? null : (String)r["BornOn"],
                    DistinguishedName  = r["DistinguishedName"]  is DBNull ? null : (String)r["DistinguishedName"],
                    ID                 = r["ID"]                 is DBNull ? null : (Guid?) r["ID"],
                    Modified           = r["Modified"]           is DBNull ? null : (String)r["Modified"],
                    Name               = r["Name"]               is DBNull ? null : (String)r["Name"],
                    Notes              = r["ID"]                 is DBNull ? null : GetNotes(a,(Guid?)r["ID"])?.ToArray(),
                    ObjectType         = r["ObjectType"]         is DBNull ? null : (String)r["ObjectType"],
                    ServiceVersion     = r["ServiceVersion"]     is DBNull ? null : (String)r["ServiceVersion"],
                    Tags               = r["ID"]                 is DBNull ? null : GetTags(a,(Guid?)r["ID"])?.ToArray(),
                    Type               = r["Type"]               is DBNull ? null : (String)r["Type"],
                    Version            = r["Version"]            is DBNull ? null : (String)r["Version"]
                };
                f.Add(e);
            }

            f.RemoveAll(e => 
                search?.Notes != null && (e.Notes == null || !search.Notes.All(note => e.Notes.Any(_note => _note?.Contains(note,StringComparison.OrdinalIgnoreCase) == true))) ||
                search?.Tags != null && (e.Tags == null || !search.Tags.All(tag => e.Tags.Any(_tag => _tag?.Contains(tag,StringComparison.OrdinalIgnoreCase) == true))));

            if(Equals(f.Count,0)) { SetOk(_); return Results.NotFound(new ElementResponse()); }

            SetOk(_); return Results.Ok(new ElementResponse(){ Elements = f.ToArray() });
        }
        catch ( Exception _ ) { Log.Error(_,SELFail); return Results.Problem(); }
    }

    private static String GenerateElementsQuery(ElementRequest? search)
    {
        try
        {
            if(search is null) { return String.Empty; } List<String> w = new();

            if(!String.IsNullOrEmpty(search.Application))        { w.Add($"Application LIKE '%{search.Application}%'"); }
            if(!String.IsNullOrEmpty(search.ApplicationVersion)) { w.Add($"ApplicationVersion LIKE '%{search.ApplicationVersion}%'"); }
            if(!String.IsNullOrEmpty(search.BornOn))             { w.Add($"BornOn LIKE '%{search.BornOn}%'"); }
            if(!String.IsNullOrEmpty(search.DistinguishedName))  { w.Add($"DistinguishedName LIKE '%{search.DistinguishedName}%'"); }
            if(!String.IsNullOrEmpty(search.ID?.ToString()))     { w.Add($"ID='{search.ID}'"); }
            if(!String.IsNullOrEmpty(search.Modified))           { w.Add($"Modified LIKE '%{search.Modified}%'"); }
            if(!String.IsNullOrEmpty(search.Name))               { w.Add($"Name LIKE '%{search.Name}%'"); }
            if(!String.IsNullOrEmpty(search.ObjectType))         { w.Add($"ObjectType LIKE '%{search.ObjectType}%'"); }
            if(!String.IsNullOrEmpty(search.ServiceVersion))     { w.Add($"ServiceVersion LIKE '%{search.ServiceVersion}%'"); }
            if(!String.IsNullOrEmpty(search.Type))               { w.Add($"Type LIKE '%{search.Type}%'"); }
            if(!String.IsNullOrEmpty(search.Version))            { w.Add($"Version LIKE '%{search.Version}%'"); }

            return String.Join(" AND ",w);
        }
        catch ( Exception _ ) { Log.Error(_,SELQueryFail); return String.Empty; }
    }
}