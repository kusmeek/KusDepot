namespace KusDepot.Reacts;

public partial class ReactorCore : Hub
{
    private static Int32 Speed;
    private static Boolean Active;
    private static IShapeApi[]? _apiclients;
    private static IHubCallerClients? _clients;
    private static ShapeApiClient? _fhclient;
    private static ShapeApiGRPClient? _fgclient;
    private static ShapeApiClient? _ghclient;
    private static ShapeApiGRPClient? _ggclient;
    private static ShapeApiClient? _jhclient;
    private static ShapeApiGRPClient? _jgclient;
    private static ShapeApiClient? _nhclient;
    private static ShapeApiGRPClient? _ngclient;
    private static ShapeApiClient? _phclient;
    private static ShapeApiGRPClient? _pgclient;
    private static ShapeApiClient? _rhclient;
    private static ShapeApiGRPClient? _rgclient;
    private static Boolean _stats = true;

    static ReactorCore()
    {
        try
        {
            CreateShapeApiClients(); 

            Task.Run(() => StatusCheck());
        }
        catch ( Exception _ ) { Log.Error(_,ReactorCoreFail); }
    }

    private async Task Activate()
    {
        try
        {
            if(Active is not false) { return; } Active = true; _clients = Clients;

            Task.Run(async () =>
            {
                try { await RunCore(); }

                catch ( Exception _ ) { Log.Error(_,RunCoreFail); }
            });

            Log.Information(Activated);
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); await Task.CompletedTask; }
    }

    private static async Task Deactivate()
    {
        try
        {
            if(Active is not true) { return; } Active = false; Speed = 0;

            Log.Information(Deactivated);
        }
        catch ( Exception _ ) { Log.Error(_,DeactivateFail); await Task.CompletedTask; }
    }

    private static async Task RunCore()
    {
        try
        {
            while(Active)
            {
                if(Speed <= 0 || Speed > 9) { Speed = 1; }

                await Task.Delay(TimeSpan.FromMilliseconds(1000.0 / Speed));

                await (_clients?.All.SendAsync(ShapeAdded,GenerateShape()) ?? Task.CompletedTask);
            }
        }
        catch ( Exception _ ) { Log.Error(_,RunCoreFail); Active = false; }
    }

    public async Task AddShape(ToolShape shape)
    {
        try
        {
            await Clients.All.SendAsync(ShapeAdded,GenerateShape(shape));
        }
        catch ( Exception _ ) { Log.Error(_,AddShapeFail); await Task.CompletedTask; }
    }

    public async Task RemoveShape(Guid id)
    {
        try
        {
            if(Equals(Guid.Empty,id)) { return; }

            await Clients.All.SendAsync(ShapeRemoved,id);
        }
        catch ( Exception _ ) { Log.Error(_,RemoveShapeFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShape(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            await Clients.All.SendAsync(ShapeUpdated,GenerateShape(_));
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeFH(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _fhclient is not null && _fhclient.Online ? (await _fhclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeFHFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeFG(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _fgclient is not null && _fgclient.Online ? (await _fgclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeFGFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeGH(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _ghclient is not null && _ghclient.Online ? (await _ghclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeGHFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeGG(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _ggclient is not null && _ggclient.Online ? (await _ggclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeGGFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeJH(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _jhclient is not null && _jhclient.Online ? (await _jhclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeJHFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeJG(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _jgclient is not null && _jgclient.Online ? (await _jgclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeJGFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeNH(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _nhclient is not null && _nhclient.Online ? (await _nhclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeNHFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeNG(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _ngclient is not null && _ngclient.Online ? (await _ngclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeNGFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapePH(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _phclient is not null && _phclient.Online ? (await _phclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapePHFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapePG(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _pgclient is not null && _pgclient.Online ? (await _pgclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapePGFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeRH(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _rhclient is not null && _rhclient.Online ? (await _rhclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeRHFail); await Task.CompletedTask; }
    }

    public async Task RandomizeShapeRG(ToolShape shape)
    {
        try
        {
            if(shape is null || Equals(Guid.Empty,shape.id)) { return; }

            var _ = new ToolShape { id = shape.id , x = shape.x , y = shape.y };

            _ = _rgclient is not null && _rgclient.Online ? (await _rgclient.GenerateShape(_))?.AsValid() ?? GenerateShape(_) : GenerateShape(_);

            await Clients.All.SendAsync(ShapeUpdated,_);
        }
        catch ( Exception _ ) { Log.Error(_,RandomizeShapeRGFail); await Task.CompletedTask; }
    }

    public async Task ReMakeShape(ToolShape shape)
    {
        try
        {
            if(shape is null || shape.x is null || shape.y is null || Equals(Guid.Empty,shape.id)) { return; }

            var o = _apiclients?.Where(_ => _.Online).ToArray(); if(o is null || Equals(o.Length,0))

                { await Clients.All.SendAsync(ShapeUpdated,GenerateShape(shape)); }

            await Clients.All.SendAsync(ShapeUpdated,(await o![RandomNumberGenerator.GetInt32(o.Length)].GenerateShape(shape))?.AsValid() ?? GenerateShape(shape));
        }
        catch ( Exception _ ) { Log.Error(_,ReMakeShapeFail); await Task.CompletedTask; }
    }

    public async Task UpdateShape(ToolShape shape)
    {
        try
        {
            if(shape is null) { return; }

            await Clients.All.SendAsync(ShapeUpdated,shape.AsValid());
        }
        catch ( Exception _ ) { Log.Error(_,UpdateShapeFail); await Task.CompletedTask; }
    }

    public async Task UpdateShapePosition(ToolShapePosition position)
    {
        try
        {
            if(position is null || Equals(Guid.Empty,position.id)) { return; }

            await Clients.All.SendAsync(ShapePositionUpdated,position);
        }
        catch ( Exception _ ) { Log.Error(_,UpdateShapePositionFail); await Task.CompletedTask; }
    }

    public async Task SendMessage(ToolMessage message)
    {
        try
        {
            if(String.IsNullOrEmpty(Sanitize(message.sender)) ||
               String.IsNullOrEmpty(Sanitize(message.message))) { return; }

            KusDepotRegistry.Commands?.TryGetValue("SendMessage",out var tools);

            message = message with { id = Guid.NewGuid() }; await Clients.All.SendAsync(MessageReceived,message);
        }
        catch ( Exception _ ) { Log.Error(_,SendMessageFail); await Task.CompletedTask; }
    }

    public async Task SetCoreSpeed(Int32 speed)
    {
        try
        {
            if(speed < 0 || speed > 9) { return; } if(speed == 0) { await Deactivate(); return; }

            Speed = speed; Log.Information(CoreSpeedSet,Speed); if(Active is false) { await Activate(); return; }
        }
        catch ( Exception _ ) { Log.Error(_,SetCoreSpeedFail); await Task.CompletedTask; }
    }

    public async Task PurgeCore()
    {
        try { await Clients.All.SendAsync(CorePurge); }

        catch ( Exception _ ) { Log.Error(_,PurgeCoreFail); await Task.CompletedTask; }
    }

    [GeneratedRegex(@"^services__(?:[a-zA-Z0-9_-]+)__([a-zA-Z0-9_-]+)__\d+$")]
    private static partial Regex ServiceEndpointRegex();

    private static void CreateShapeApiClients()
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json",true,false)
                .Build();

            var endpoints = new Dictionary<String,String>();

            foreach(var _ in config.GetSection("ShapeApiEndpoints").GetChildren())
            {
                endpoints[_.Key] = _.Value ?? String.Empty;
            }

            foreach(DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                var key = entry.Key as String;
                var value = entry.Value as String;
                if(key is null || value is null) { continue; }
                var match = ServiceEndpointRegex().Match(key);
                if(match.Success) { endpoints[match.Groups[1].Value] = value; }
            }

            _fhclient = endpoints.TryGetValue("F-HTTP", out var fHttp) ? new ShapeApiClient(fHttp,"F-HTTP") : null;
            _fgclient = endpoints.TryGetValue("F-GRPC", out var fGrpc) ? new ShapeApiGRPClient(fGrpc,"F-GRPC") : null;
            _ghclient = endpoints.TryGetValue("G-HTTP", out var gHttp) ? new ShapeApiClient(gHttp,"G-HTTP") : null;
            _ggclient = endpoints.TryGetValue("G-GRPC", out var gGrpc) ? new ShapeApiGRPClient(gGrpc,"G-GRPC") : null;
            _jhclient = endpoints.TryGetValue("J-HTTP", out var jHttp) ? new ShapeApiClient(jHttp,"J-HTTP") : null;
            _jgclient = endpoints.TryGetValue("J-GRPC", out var jGrpc) ? new ShapeApiGRPClient(jGrpc,"J-GRPC") : null;
            _nhclient = endpoints.TryGetValue("N-HTTP", out var nHttp) ? new ShapeApiClient(nHttp,"N-HTTP") : null;
            _ngclient = endpoints.TryGetValue("N-GRPC", out var nGrpc) ? new ShapeApiGRPClient(nGrpc,"N-GRPC") : null;
            _phclient = endpoints.TryGetValue("P-HTTP", out var pHttp) ? new ShapeApiClient(pHttp,"P-HTTP") : null;
            _pgclient = endpoints.TryGetValue("P-GRPC", out var pGrpc) ? new ShapeApiGRPClient(pGrpc,"P-GRPC") : null;
            _rhclient = endpoints.TryGetValue("R-HTTP", out var rHttp) ? new ShapeApiClient(rHttp,"R-HTTP") : null;
            _rgclient = endpoints.TryGetValue("R-GRPC", out var rGrpc) ? new ShapeApiGRPClient(rGrpc,"R-GRPC") : null;

            _apiclients = new IShapeApi[]
            {
                _fhclient!, _fgclient!,
                _ghclient!, _ggclient!,
                _jhclient!, _jgclient!,
                _nhclient!, _ngclient!,
                _phclient!, _pgclient!,
                _rhclient!, _rgclient!
            }.Where(_ => _ is not null).ToArray();
        }
        catch ( Exception _ ) { Log.Error(_,CreateShapeApiClientsFail); }
    }

    private static async Task StatusCheck()
    {
        try
        {
            Int32 interval = Int32.TryParse(Environment.GetEnvironmentVariable("STATUS_INTERVAL"),out Int32 p) is true && p > 0 ? p : 10;

            while(_stats)
            {
                var tasks = new List<Task>();
                if(_fhclient is not null) tasks.Add(_fhclient.IsOnline());
                if(_fgclient is not null) tasks.Add(_fgclient.IsOnline());
                if(_ghclient is not null) tasks.Add(_ghclient.IsOnline());
                if(_ggclient is not null) tasks.Add(_ggclient.IsOnline());
                if(_jhclient is not null) tasks.Add(_jhclient.IsOnline());
                if(_jgclient is not null) tasks.Add(_jgclient.IsOnline());
                if(_nhclient is not null) tasks.Add(_nhclient.IsOnline());
                if(_ngclient is not null) tasks.Add(_ngclient.IsOnline());
                if(_phclient is not null) tasks.Add(_phclient.IsOnline());
                if(_pgclient is not null) tasks.Add(_pgclient.IsOnline());
                if(_rhclient is not null) tasks.Add(_rhclient.IsOnline());
                if(_rgclient is not null) tasks.Add(_rgclient.IsOnline());
                try { await Task.WhenAll(tasks); } catch {}
                await Task.Delay(TimeSpan.FromSeconds(interval));
            }
        }
        catch ( Exception _ ) { Log.Error(_,StatusCheckFail); }
    }
}