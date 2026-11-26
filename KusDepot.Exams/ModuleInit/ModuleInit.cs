namespace KusDepot.Exams;

public static class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("System.Net.Security.NoRevocationCheckByDefault",true);

        ImportAzureEnvironmentVariables();

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

    private static void ImportAzureEnvironmentVariables()
    {
        if(!String.Equals(Environment.GetEnvironmentVariable("KUSDEPOT_DEPLOYMENT_PROFILE"),"AzureContainer",StringComparison.OrdinalIgnoreCase)) { return; }

        String[] variableNames =
        [
            "KUSDEPOT_APPCONFIGURATION_ENDPOINT",
            "KUSDEPOT_DATACONTROL_ENDPOINT",
            "KUSDEPOT_CATALOG_ENDPOINT",
            "AZURE_TENANT_ID",
            "AZURE_CLIENT_ID",
            "AZURE_CLIENT_SECRET"
        ];

        foreach(String variableName in variableNames)
        {
            if(!String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(variableName,EnvironmentVariableTarget.Process))) { continue; }

            String? userValue = Environment.GetEnvironmentVariable(variableName,EnvironmentVariableTarget.User);

            if(!String.IsNullOrWhiteSpace(userValue)) { Environment.SetEnvironmentVariable(variableName,userValue,EnvironmentVariableTarget.Process); }
        }
    }
}