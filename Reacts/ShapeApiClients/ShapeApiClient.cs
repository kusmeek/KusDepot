namespace KusDepot.Reacts;

public sealed class ShapeApiClient : IShapeApi
{
    private readonly RestClient Client; public String? Endpoint { get; private set; }

    public Boolean Online { get; private set; } = false; public String? Service { get; private set; }

    public ShapeApiClient(String endpoint , String? service)
    {
        Endpoint = endpoint; Service = service; Client = new(new RestClientOptions(endpoint) { Timeout = TimeSpan.FromMilliseconds(100) });
    }

    public async Task<ToolShape?> GenerateShape(ToolShape? input = null)
    {
        try
        {
            RestRequest req = new("/generateshape",Method.Post); if(input is not null) { req.AddJsonBody(input); }

            var res = await Client.ExecuteAsync<ToolShape>(req);

            if(res.IsSuccessful is not true)
            {
                if(res.ErrorException is TaskCanceledException) { Log.Error(GenerateShapeTimeout,Endpoint,Service); }

                else { Log.Error(res.ErrorException,GenerateShapeFail,Endpoint,Service); }

                return null;
            }

            return res.Data;
        }
        catch ( Exception _ ) { Log.Error(_,GenerateShapeFail,Endpoint,Service); return null; }
    }

    public async Task<Boolean> IsOnline()
    {
        try
        {
            RestRequest rq = new("/generateshape",Method.Post); rq.AddJsonBody(new ToolShape());

            var rs = await Client.ExecuteAsync<ToolShape>(rq);

            if(rs.IsSuccessful is not true)
            {
                Log.Warning(rs.ErrorException,IsOnlineFail,Endpoint,Service);

                Online = false; return false;
            }

            Log.Verbose(IsOnlineSuccess,Endpoint,Service); Online = true; return true;
        }
        catch ( Exception _ ) { Log.Warning(_,IsOnlineFail,Endpoint,Service); Online = false; return false; }
    }

    private const String IsOnlineFail = "ShapeApiClient IsOnline Failed {@Endpoint} {@Service}";
    private const String IsOnlineSuccess = "ShapeApiClient IsOnline Success {@Endpoint} {@Service}";
    private const String GenerateShapeFail = "ShapeApiClient GenerateShape Failed {@Endpoint} {@Service}";
    private const String GenerateShapeTimeout = "ShapeApiClient GenerateShape Timeout {@Endpoint} {@Service}";
}