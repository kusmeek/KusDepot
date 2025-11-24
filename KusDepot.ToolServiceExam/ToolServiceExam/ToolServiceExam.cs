using ToolServiceExam;

namespace KusDepot.ToolServiceExam;

[TestFixture] [SingleThreaded] [Parallelizable(ParallelScope.All)]
public class ToolServiceExam
{
    private static CustomBinding Binding = new CustomBinding(new BindingElement[]{
                    SecurityBindingElement.CreateCertificateOverTransportBindingElement(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10),
                    new BinaryMessageEncodingBindingElement(){CompressionFormat = CompressionFormat.GZip,MessageVersion = MessageVersion.Soap12WSAddressing10 , ReaderQuotas = XmlDictionaryReaderQuotas.Max},
                    new HttpsTransportBindingElement(){MaxBufferPoolSize = Int32.MaxValue , MaxReceivedMessageSize = Int32.MaxValue , RequireClientCertificate = true}})
                    {Name = "ToolService" , Namespace = "KusDepot"};

    private static String URL = "https://localhost:4430/ToolService";

    private static ToolServiceClient _0 = new ToolServiceClient(Binding,new EndpointAddress(URL));

    private ClientKey? KeyC; private Guid? Oid;

    [OneTimeSetUp]
    public void Calibrate()
    {
        _0.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,"ToolServiceClient");

        Process.Start(new ProcessStartInfo(){ CreateNoWindow = false , FileName = "cmd.exe" , Arguments = "/C " +

        Environment.GetEnvironmentVariable("KusDepotSolution") + @"\KusDepot.ToolService\bin\x64\Release\net10.0\KusDepot.ToolService.exe" , UseShellExecute = true , WindowStyle = ProcessWindowStyle.Maximized});

        Thread.Sleep(60000);
    }

    [OneTimeTearDown]
    public void Complete() { Thread.Sleep(8000); Process.GetProcessesByName("KusDepot.ToolService").FirstOrDefault()?.Kill(); }

    [Test] [Order(1)]
    public void RequestAccess()
    {
        var r = new StandardRequest(); r.Data = "ToolServiceExam";

        KeyC = _0.RequestAccess(r) as ClientKey; Check.That(KeyC).IsNotNull();
    }

    [Test] [Order(2)]
    public void ExecuteCommand()
    {
        CommandDetails _1 = new CommandDetails() { Handle = "CommandTest" };

        Oid = _0.ExecuteCommand(_1,KeyC); Check.That(Oid).IsNotNull();
    }

    [Test] [Order(3)]
    public void GetOutput()
    {
        Check.That(_0.GetOutput(Oid,KeyC) as DataSetItem).IsNotNull();
    }

    [Test] [Order(4)]
    public void RevokeAccess()
    {
        Check.That(_0.RevokeAccess(KeyC)).IsTrue();
    }
}