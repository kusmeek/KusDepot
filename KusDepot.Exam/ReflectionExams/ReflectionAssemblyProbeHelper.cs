namespace KusDepot.Exams;

internal static class ReflectionAssemblyProbeHelper
{
    public const String TestAssemblySimpleName = "Labspace";

    public const String TestTypeFullName = "KusDepot.Test.MathCommand";

    private const String TestAssemblyFileName = "Labspace.dll";

    private const String TestProjectFileName = "Labspace.csproj";

    public static String TestAssemblyQualifiedTypeName => $"{TestTypeFullName}, {TestAssemblySimpleName}";

    public static Boolean TryGetTestAssemblyPath(out String path)
    {
        path = String.Empty;

        try
        {
            String root = GetSolutionRoot();

            ReadOnlySpan<String> candidates =
            [
                Path.Combine(root,"Labspace","bin","x64","Debug","net10.0",TestAssemblyFileName),
                Path.Combine(root,"Labspace","bin","x64","Release","net10.0",TestAssemblyFileName),
                Path.Combine(root,"Labspace","bin","Debug","net10.0",TestAssemblyFileName),
                Path.Combine(root,"Labspace","bin","Release","net10.0",TestAssemblyFileName)
            ];

            foreach(String candidate in candidates)
            {
                if(File.Exists(candidate)) { path = candidate; return true; }
            }

            return false;
        }
        catch { return false; }
    }

    private static String GetSolutionRoot()
    {
        DirectoryInfo? d = new(AppContext.BaseDirectory);

        for(Int32 i = 0; i < 8 && d is not null; i++)
        {
            if(DirectoryContainsLabspaceProject(d.FullName)) { return d.FullName; }

            d = d.Parent;
        }

        throw new DirectoryNotFoundException($"Could not locate a repository root containing Labspace{Path.DirectorySeparatorChar}{TestProjectFileName} from test base directory.");
    }

    private static Boolean DirectoryContainsLabspaceProject(String directory) => File.Exists(Path.Combine(directory,"Labspace",TestProjectFileName));
}