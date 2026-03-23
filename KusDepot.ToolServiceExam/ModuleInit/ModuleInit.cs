namespace ToolServiceExam;

public static class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("System.Net.Security.NoRevocationCheckByDefault",true);
    }
}