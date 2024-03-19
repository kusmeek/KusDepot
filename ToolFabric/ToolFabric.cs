namespace ToolFabric;

internal sealed class ToolFabric : StatefulService
{
    private readonly String URL;

    public ToolFabric(StatefulServiceContext context) : base(context) {this.URL = "http://" + this.Context.NodeContext.IPAddressOrFQDN + ":" + this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port;}

    private void RunApp(Object? server) {((WebApplication?)server)?.Run();}

    protected override async Task RunAsync(CancellationToken token)
    {
        WebApplicationBuilder _0 = WebApplication.CreateBuilder();
        {
            _0.WebHost.UseUrls(this.URL);

            _0.Services.AddServiceModelServices(); _0.Services.AddServiceModelMetadata();

            FabricClient _ = new FabricClient(); _.PropertyManager.PutPropertyAsync(this.Context.ServiceName,"EndpointLocator",this.URL).Wait(token); _.Dispose();
        }

        WebApplication _1 = _0.Build();
        {
            ((IApplicationBuilder)_1).UseServiceModel( servicebuilder =>
            {
                servicebuilder.AddService<Tool>();

                servicebuilder.AddServiceEndpoint<Tool,ITool>(new CustomBinding(new BindingElement[]{
                    new BinaryMessageEncodingBindingElement(){CompressionFormat = CompressionFormat.GZip,
                    MessageVersion = MessageVersion.Soap12WSAddressing10 , ReaderQuotas = XmlDictionaryReaderQuotas.Max},
                    new HttpTransportBindingElement(){MaxBufferPoolSize = Int32.MaxValue , MaxReceivedMessageSize = Int32.MaxValue}})
                    {Name = "ToolFabric" , Namespace = "KusDepot"},"/ToolFabric");

                _1.Services.GetRequiredService<ServiceMetadataBehavior>().HttpGetEnabled = true;
            });
        }

        new Thread(this.RunApp).Start(_1);

        while(true) {if(token.IsCancellationRequested) {await _1.StopAsync(CancellationToken.None).ConfigureAwait(true);} await Task.Delay(30000,token).ConfigureAwait(true);}
    }

    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners() { return Array.Empty<ServiceReplicaListener>(); }
}