namespace KusDepot.Data;

public sealed partial class DataControl
{
    public void ConfigureTLS(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(o =>
        {
            o.ListenAnyIP(GetPort(),o =>
            {
                o.UseHttps((_,_,_,_) =>
                {
                    return ValueTask.FromResult(new SslServerAuthenticationOptions()
                    {
                        CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                        EncryptionPolicy = EncryptionPolicy.RequireEncryption,
                        ServerCertificate = GetServerCertificate(),
                        ClientCertificateRequired = true
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