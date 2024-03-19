namespace KusDepot;

internal static class ToolService
{
    private static void Main()
    {
        WebApplicationBuilder _0 = WebApplication.CreateBuilder();
        {
            _0.WebHost.UseUrls("http://localhost:80");

            _0.Services.AddServiceModelServices(); _0.Services.AddServiceModelMetadata();
        }
        WebApplication _1 = _0.Build();
        {
            _1.UseServiceModel( servicebuilder =>
            {
                servicebuilder.AddService<Tool>();

                servicebuilder.AddServiceEndpoint<Tool,ITool>(new CustomBinding(new BindingElement[]{
                    new BinaryMessageEncodingBindingElement(){CompressionFormat = CompressionFormat.GZip,
                    MessageVersion = MessageVersion.Soap12WSAddressing10 , ReaderQuotas = XmlDictionaryReaderQuotas.Max},
                    new HttpTransportBindingElement(){MaxBufferPoolSize = Int32.MaxValue , MaxReceivedMessageSize = Int32.MaxValue}})
                    {Name = "ToolService" , Namespace = "KusDepot"},"/ToolService");

                _1.Services.GetRequiredService<ServiceMetadataBehavior>().HttpGetEnabled = true;
            });
        }
        _1.Run();
    }
}