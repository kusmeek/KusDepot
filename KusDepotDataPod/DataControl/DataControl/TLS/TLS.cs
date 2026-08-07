namespace DataPodServices.DataControl;

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
        try
        {
            if(OperatingSystem.IsWindows()) { return GetWindowsServerCertificate(); }

            if(OperatingSystem.IsLinux()) { return GetLinuxServerCertificate(); }

            throw new PlatformNotSupportedException("TLS server certificate loading is not supported on this platform.");
        }
        catch ( Exception _ )
        {
            Log.Fatal(_,X509NotFound);

            Process.GetCurrentProcess().Kill(); return null!;
        }
    }

    private const String ServerCertificatePasswordEnvironmentVariable = "KUSDEPOT_DATAPOD_DATACONTROL_CERTIFICATE_PASSWORD";

    private static X509Certificate2 GetLinuxServerCertificate()
    {
        if(File.Exists(DataControlServerCertificateFilePath) is false)
        {
            throw new FileNotFoundException("TLS server certificate file was not found.",DataControlServerCertificateFilePath);
        }

        return X509CertificateLoader.LoadPkcs12FromFile(DataControlServerCertificateFilePath,
            GetEnvironmentVariable(ServerCertificatePasswordEnvironmentVariable),X509KeyStorageFlags.EphemeralKeySet);
    }

    private static X509Certificate2 GetWindowsServerCertificate()
    {
        using X509Store s = new(StoreName.My,StoreLocation.LocalMachine); s.Open(OpenFlags.ReadOnly); X509Certificate2? c;

        c = s.Certificates.Find(X509FindType.FindBySubjectName,X509SubjectName,true).FirstOrDefault();

        return c ?? throw new InvalidOperationException(X509NotFound);
    }
}