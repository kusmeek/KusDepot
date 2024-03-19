namespace KusDepot.Data;

internal sealed partial class Catalog
{
    private static void MapSearchActiveServices(WebApplication application)
    {
        application.MapPost("Catalog/ActiveServices",
                   ([FromBody] ActiveServiceRequest? search,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return SearchActiveServices(search,af,dc,hc);})
                   .Produces<ActiveServiceResponse>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<ActiveServiceResponse>(StatusCodes.Status404NotFound)
                   .Produces<String?>(StatusCodes.Status500InternalServerError)
                   .WithName("SearchActiveServices")
                   .WithOpenApi(o=>{o.OperationId = "SearchActiveServices"; return o;});
    }

    private static IResult SearchActiveServices(ActiveServiceRequest? search , IArkKeeperFactory af ,IDataConfigs dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"SearchActiveServices")?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SASUnAuth); SetErr(_); return Results.Unauthorized(); }

            StorageSilo? si = dc.GetAuthorizedReadSilo(t,dt,ds).Result; if(si is null) { Log.Error(SASUnAuth); SetErr(_); return Results.Unauthorized(); }

            IArkKeeper k = af.Create(si.CatalogName); Ark? a = Ark.Parse(k.GetArk(dt,ds).Result); if(a is null) { Log.Error(SASArkNull); SetErr(_); return Results.Problem(); }

            List<ActiveService> f = new List<ActiveService>(); String g = GenerateActiveServicesQuery(search); DataRow[] q = a.Tables[Ark.ActiveServicesTableName]!.Select(g);

            foreach(DataRow r in q)
            {
                ActiveService s = new ActiveService
                {
                    ActorID            = r["ActorID"]            is DBNull ? null : (Guid?) r["ActorID"],
                    ActorNameID        = r["ActorNameID"]        is DBNull ? null : (String)r["ActorNameID"],
                    Application        = r["Application"]        is DBNull ? null : (String)r["Application"],
                    ApplicationVersion = r["ApplicationVersion"] is DBNull ? null : (String)r["ApplicationVersion"],
                    BornOn             = r["BornOn"]             is DBNull ? null : (String)r["BornOn"],
                    DistinguishedName  = r["DistinguishedName"]  is DBNull ? null : (String)r["DistinguishedName"],
                    ID                 = r["ID"]                 is DBNull ? null : (Guid?) r["ID"],
                    Interfaces         = r["Interfaces"]         is DBNull ? null : (String)r["Interfaces"],
                    Modified           = r["Modified"]           is DBNull ? null : (String)r["Modified"],
                    Name               = r["Name"]               is DBNull ? null : (String)r["Name"],
                    Notes              = r["ID"]                 is DBNull ? null : GetNotes(a,(Guid?)r["ID"])?.ToArray(),
                    Purpose            = r["Purpose"]            is DBNull ? null : (String)r["Purpose"],
                    Registered         = r["Registered"]         is DBNull ? null : (String)r["Registered"],
                    ServiceVersion     = r["ServiceVersion"]     is DBNull ? null : (String)r["ServiceVersion"],
                    Tags               = r["ID"]                 is DBNull ? null : GetTags(a,(Guid?)r["ID"])?.ToArray(),
                    Url                = r["Url"]                is DBNull ? null : (String)r["Url"],
                    Version            = r["Version"]            is DBNull ? null : (String)r["Version"]
                };
                f.Add(s);
            }

            f.RemoveAll(s => 
                search?.Notes != null && (s.Notes == null || !search.Notes.All(note => s.Notes.Any(_note => _note?.Contains(note,StringComparison.OrdinalIgnoreCase) == true))) ||
                search?.Tags != null && (s.Tags == null || !search.Tags.All(tag => s.Tags.Any(_tag => _tag?.Contains(tag,StringComparison.OrdinalIgnoreCase) == true))));

            if(Equals(f.Count,0)) { SetOk(_); return Results.NotFound(new ActiveServiceResponse()); }

            SetOk(_); return Results.Ok(new ActiveServiceResponse(){ ActiveServices = f.ToArray() });
        }
        catch ( Exception _ ) { Log.Error(_,SASFail); return Results.Problem(); }
    }

    private static String GenerateActiveServicesQuery(ActiveServiceRequest? search)
    {
        try
        {
            if(search is null) { return String.Empty; } List<String> w = new();

            if(!String.IsNullOrEmpty(search.ActorID?.ToString())) { w.Add($"ActorID='{search.ActorID}'"); }
            if(!String.IsNullOrEmpty(search.ActorNameID))         { w.Add($"ActorNameID LIKE '%{search.ActorNameID}%'"); }
            if(!String.IsNullOrEmpty(search.Application))         { w.Add($"Application LIKE '%{search.Application}%'"); }
            if(!String.IsNullOrEmpty(search.ApplicationVersion))  { w.Add($"ApplicationVersion LIKE '%{search.ApplicationVersion}%'"); }
            if(!String.IsNullOrEmpty(search.BornOn))              { w.Add($"BornOn LIKE '%{search.BornOn}%'"); }
            if(!String.IsNullOrEmpty(search.DistinguishedName))   { w.Add($"DistinguishedName LIKE '%{search.DistinguishedName}%'"); }
            if(!String.IsNullOrEmpty(search.ID?.ToString()))      { w.Add($"ID='{search.ID}'"); }
            if(!String.IsNullOrEmpty(search.Interfaces))          { w.Add($"Interfaces LIKE '%{search.Interfaces}%'"); }
            if(!String.IsNullOrEmpty(search.Modified))            { w.Add($"Modified LIKE '%{search.Modified}%'"); }
            if(!String.IsNullOrEmpty(search.Name))                { w.Add($"Name LIKE '%{search.Name}%'"); }
            if(!String.IsNullOrEmpty(search.Purpose))             { w.Add($"Purpose LIKE '%{search.Purpose}%'"); }
            if(!String.IsNullOrEmpty(search.Registered))          { w.Add($"Registered LIKE '%{search.Registered}%'"); }
            if(!String.IsNullOrEmpty(search.ServiceVersion))      { w.Add($"ServiceVersion LIKE '%{search.ServiceVersion}%'"); }
            if(!String.IsNullOrEmpty(search.Url))                 { w.Add($"Url LIKE '%{search.Url}%'"); }
            if(!String.IsNullOrEmpty(search.Version))             { w.Add($"Version LIKE '%{search.Version}%'"); }

            return String.Join(" AND ",w);
        }
        catch ( Exception _ ) { Log.Error(_,SASQueryFail); return String.Empty; }
    }
}