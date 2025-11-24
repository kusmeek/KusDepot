namespace KusDepot.ToolService;

public sealed partial class ToolService
{
    public static void ConfigureTLS(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.WebHost.ConfigureKestrel(o => { o.ListenAnyIP(GetPort()); });

            return;
        }

        builder.WebHost.ConfigureKestrel(o =>
        {
            o.ListenAnyIP(GetPort(),o =>
            {
                o.UseHttps((_,_,_,_) =>
                {
                    return ValueTask.FromResult(new SslServerAuthenticationOptions()
                    {
                        EncryptionPolicy = EncryptionPolicy.RequireEncryption,
                        ServerCertificate = GetServerCertificate(),
                        ClientCertificateRequired = false
                    });

                },String.Empty);
            });
        });
    }

    private static X509Certificate2 GetServerCertificate()
    {
        using X509Store s = new(StoreName.My,StoreLocation.LocalMachine); s.Open(OpenFlags.ReadOnly); X509Certificate2? c;

        c = s.Certificates.Find(X509FindType.FindBySubjectName,X509SubjectName,true).FirstOrDefault();

        if(c is null) { Log.Fatal(X509NotFound); Process.GetCurrentProcess().Kill(); } return c!;
    }
}