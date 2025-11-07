namespace KusDepot.ToolFabric;

public partial class ToolFabric
{
    public void ConfigureToolFabricServer(WebApplication server)
    {
        AppContext.SetSwitch("System.Net.Security.NoRevocationCheckByDefault",true);

        server.UseServiceModel(b =>
        {
            b.AddService<ToolFabric>();

            if(server.Environment.IsDevelopment())
            {
                server.Services.GetRequiredService<ServiceMetadataBehavior>().HttpGetEnabled = true;

                b.AddServiceEndpoint<ToolFabric,IToolFabric>(new CustomBinding(new BindingElement[]{
                    new BinaryMessageEncodingBindingElement(){CompressionFormat = CompressionFormat.GZip , MessageVersion = MessageVersion.Soap12WSAddressing10 , ReaderQuotas = XmlDictionaryReaderQuotas.Max},
                    new HttpTransportBindingElement(){MaxBufferPoolSize = Int32.MaxValue , MaxReceivedMessageSize = Int32.MaxValue}}) {Name = "ToolFabric" , Namespace = "KusDepot"},"/ToolFabric");
            }
            else
            {
                b.ConfigureServiceHostBase<ToolFabric>(c =>
                {
                    c.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                    c.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new X509PolicyValidator();
                    c.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                    c.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,X509SubjectName);
                });

                b.AddServiceEndpoint<ToolFabric,IToolFabric>(new CustomBinding(new BindingElement[]{
                    SecurityBindingElement.CreateCertificateOverTransportBindingElement(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10),
                    new BinaryMessageEncodingBindingElement(){CompressionFormat = CompressionFormat.GZip , MessageVersion = MessageVersion.Soap12WSAddressing10 , ReaderQuotas = XmlDictionaryReaderQuotas.Max},
                    new HttpsTransportBindingElement(){MaxBufferPoolSize = Int32.MaxValue , MaxReceivedMessageSize = Int32.MaxValue , RequireClientCertificate = true}}) {Name = "ToolFabric" , Namespace = "KusDepot"},"/ToolFabric");
            }
        });
    }

    public static WebApplicationOptions GetToolFabricOptions()
    {
        try
        {
            String? _ = JsonDocument.Parse(File.ReadAllText(ToolFabric.ConfigFilePath)).RootElement.GetProperty("Environment").GetString();

            if(new[]{Environments.Development,Environments.Production,Environments.Staging}.Contains(_))
            {
                return new(){ ApplicationName = ServiceName , EnvironmentName = _ };
            }

            return new(){ ApplicationName = ServiceName };
        }
        catch { return new(){ ApplicationName = ServiceName }; }
    }

    private static String configFilePath => AppContext.BaseDirectory + @"\appsettings.json";

    public static String ConfigFilePath => configFilePath;
}