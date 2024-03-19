namespace KusDepot.Data;

internal sealed partial class DataControl : StatelessService
{
    public DataControl(StatelessServiceContext context) : base(context) {this.URL = $"http://{this.Context.NodeContext.IPAddressOrFQDN}:{this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port}";}

    private void RunApp(Object? server) {Log.Information(DataControlStartedURL,this.URL); ((WebApplication?)server)!.Run();}

    private readonly String URL;

    protected override async Task RunAsync(CancellationToken token)
    {
        try
        {
            WebApplicationBuilder _0 = WebApplication.CreateBuilder(new WebApplicationOptions(){ApplicationName = "DataControl" , Args = Array.Empty<String>() , ContentRootPath = CurrentDirectory , WebRootPath = CurrentDirectory});
            {
                _0.Services.ConfigureHttpJsonOptions(o=>{o.SerializerOptions.PropertyNameCaseInsensitive = true;o.SerializerOptions.PropertyNamingPolicy = null;o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;});

                FabricClient _ = new FabricClient(); _.PropertyManager.PutPropertyAsync(this.Context.ServiceName,"EndpointLocator",this.URL).Wait(token); _.Dispose();

                _0.Services.AddSwaggerGen(o=>{o.SwaggerDoc("DataControlAPI",new OpenApiInfo(){Title = "DataControl",Version = "1"});});

                _0.Configuration.AddJsonFile(ConfigFilePath,true,true);

                _0.Services.ConfigureDataControlServices();

                _0.Services.AddEndpointsApiExplorer();

                _0.Services.AddHealthChecks();

                _0.WebHost.UseUrls(this.URL);
            }

            WebApplication _1 = _0.Build();
            {
                SetupDiagnostics();

                MapDelete(_1); MapGet(_1); MapStore(_1);

                _1.MapHealthChecks("DataControl/Health");

                _1.MapSwagger("/swagger/{documentName}/swagger.json");

                _1.UseSwaggerUI(o=>{o.SwaggerEndpoint("/swagger/DataControlAPI/swagger.json","MetaData");});
            }

            new Thread(this.RunApp).Start(_1);

            while(true) {if(token.IsCancellationRequested) {Log.Information(DataControlStopped); await _1.StopAsync(CancellationToken.None).ConfigureAwait(true);} await Task.Delay(30000,token).ConfigureAwait(true);}
        }
        catch ( Exception _ ) {Log.Fatal(_,DataControlFail);}
    }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() { return Array.Empty<ServiceInstanceListener>(); }

    protected override Task OnCloseAsync(CancellationToken token) { Log.Information(DataControlClosing); Log.CloseAndFlush(); return base.OnCloseAsync(token); }

    private static String GetToken(HttpContext context) { return context.Request.Headers["Authorization"].ToString().Replace("Bearer ",String.Empty,StringComparison.Ordinal); }
}