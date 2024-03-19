namespace KusDepot.Data;

internal sealed partial class Catalog : StatelessService
{
    public Catalog(StatelessServiceContext context) : base(context) {this.URL = $"http://{this.Context.NodeContext.IPAddressOrFQDN}:{this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port}";}

    private void RunApp(Object? server) {Log.Information(CatalogStartedURL,this.URL); ((WebApplication?)server)!.Run();}

    private readonly String URL;

    protected override async Task RunAsync(CancellationToken token)
    {
        try
        {
            WebApplicationBuilder _0 = WebApplication.CreateBuilder(new WebApplicationOptions(){ApplicationName = "Catalog" , Args = Array.Empty<String>() , ContentRootPath = CurrentDirectory , WebRootPath = CurrentDirectory});
            {
                _0.Services.ConfigureHttpJsonOptions(o=>{o.SerializerOptions.PropertyNamingPolicy = null; o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip; o.SerializerOptions.WriteIndented = true;});

                FabricClient _ = new FabricClient(); _.PropertyManager.PutPropertyAsync(this.Context.ServiceName,"EndpointLocator",this.URL).Wait(token); _.Dispose();

                _0.Services.AddSwaggerGen(o=>{o.SwaggerDoc("CatalogAPI",new OpenApiInfo(){Title = "Catalog",Version = "1"});});

                _0.Configuration.AddJsonFile(ConfigFilePath,true,true);

                _0.Services.ConfigureCatalogServices();

                _0.Services.AddEndpointsApiExplorer();

                _0.Services.AddHealthChecks();

                _0.WebHost.UseUrls(this.URL);
            }

            WebApplication _1 = _0.Build();
            {
                _1.MapHealthChecks("Catalog/Health");

                _1.MapSwagger("/swagger/{documentName}/swagger.json");

                _1.UseSwaggerUI(o=>{o.SwaggerEndpoint("/swagger/CatalogAPI/swagger.json","MetaData");});

                MapSearchActiveServices(_1); MapSearchElements(_1); MapSearchMedia(_1); MapSearchNotes(_1); MapSearchTags(_1); SetupDiagnostics();
            }

            new Thread(this.RunApp).Start(_1);

            while(true) {if(token.IsCancellationRequested) {Log.Information(CatalogStopped); await _1.StopAsync(CancellationToken.None).ConfigureAwait(true); } await Task.Delay(30000,token).ConfigureAwait(true);}
        }
        catch ( Exception _ ) {Log.Fatal(_,CatalogFail);}
    }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() { return Array.Empty<ServiceInstanceListener>(); }

    protected override Task OnCloseAsync(CancellationToken token) { Log.Information(CatalogClosing); Log.CloseAndFlush(); return base.OnCloseAsync(token); }

    private static String GetToken(HttpContext context) { return context.Request.Headers["Authorization"].ToString().Replace("Bearer ",String.Empty,StringComparison.Ordinal); }
}