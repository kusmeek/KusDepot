namespace LabX08;

internal static class LabDataControlClientFactory
{
    public static DataControlClient Create(LabDemoOptions options , String token)
    {
        return new(options.Endpoint,token,new DataControlClientOptions()
        {
            WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions()
            {
                RootPath = options.WorkingDirectory,
            }
        });
    }
}
