namespace LabX08;

internal static class LabDataControlClientFactory
{
    public static DataControlClient Create(LabDemoOptions options , String token)
    {
        X509Certificate2 certificate = LoadCertificate(options.CertificateSubjectName);

        return new(options.Endpoint,certificate,token)
        {
            WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions()
            {
                RootPath = options.WorkingDirectory,
            }
        };
    }

    private static X509Certificate2 LoadCertificate(String subjectName)
    {
        using X509Store store = new(StoreName.My,StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        X509Certificate2? certificate = store.Certificates
            .Find(X509FindType.FindBySubjectName,subjectName,validOnly:true)
            .OfType<X509Certificate2>()
            .FirstOrDefault();

        return certificate ?? throw new InvalidOperationException($"No certificate was found for subject '{subjectName}'.");
    }
}
