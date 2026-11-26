namespace KusDepot.Exams;

public static class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        try
        {
            String tempRoot = @"R:\temp";
            String tempPath = Path.Combine(tempRoot,"kusdepotexams");

            if(Directory.Exists(tempPath) is false && Directory.Exists(tempRoot))
            {
                Directory.CreateDirectory(tempPath);
            }

            if(Directory.Exists(tempPath))
            {
                Environment.SetEnvironmentVariable("TMP",tempPath);
                Environment.SetEnvironmentVariable("TEMP",tempPath);
            }
        }
        catch { }
    }
}