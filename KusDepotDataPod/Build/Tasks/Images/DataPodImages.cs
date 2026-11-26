namespace KusDepotDataPod.Build;

public static class DataPodImages
{
    private static readonly String[] ImageNames =
    [
        "datapod-catalog",
        "datapod-control",
        "datapod-silo",
    ];

    public static String[] GetReferences(BuildContext context)
    {
        var registry = GetOptionalArgument(context,"registry");

        var commit = GetOptionalArgument(context,"commit");

        if(String.IsNullOrWhiteSpace(registry) && String.IsNullOrWhiteSpace(commit))
        {
            var localReferences = new String[ImageNames.Length];

            for(var index = 0; index < ImageNames.Length; index++)
            {
                localReferences[index] = $"{ImageNames[index]}:local-dev";
            }

            return localReferences;
        }

        if(String.IsNullOrWhiteSpace(registry) || String.IsNullOrWhiteSpace(commit))
        {
            throw new CakeException("Specify both --registry and --commit, or neither.");
        }

        registry = registry.Trim().TrimEnd('/');

        commit = commit.Trim();

        if(registry.Contains('/') || registry.Contains("://") || String.IsNullOrWhiteSpace(registry))
        {
            throw new CakeException("--registry must be a registry host such as example.azurecr.io, without a scheme or repository path.");
        }

        if(commit.Length != 40)
        {
            throw new CakeException("--commit must be a full 40-character Git commit SHA.");
        }

        foreach(var character in commit)
        {
            if(!Uri.IsHexDigit(character))
            {
                throw new CakeException("--commit must be a full 40-character Git commit SHA.");
            }
        }

        var references = new String[ImageNames.Length];

        for(var index = 0; index < ImageNames.Length; index++)
        {
            references[index] = $"{registry}/{ImageNames[index]}:{commit.ToLowerInvariant()}";
        }

        return references;
    }

    public static String[] GetDockerfiles(BuildContext context) =>
    [
        context.ProjectPath("KusDepotDataPod/Catalog/Dockerfile"),
        context.ProjectPath("KusDepotDataPod/DataControl/Dockerfile"),
        context.ProjectPath("KusDepotDataPod/Services/Dockerfile"),
    ];

    public static void ValidateRegistrySource(BuildContext context , String commit)
    {
        var head = RunGit(context,"rev-parse HEAD").Trim();

        commit = commit.Trim();

        if(!String.Equals(head,commit,StringComparison.OrdinalIgnoreCase))
        {
            throw new CakeException($"--commit {commit} does not match the current HEAD {head}.");
        }

        var status = RunGit(context,"status --porcelain --untracked-files=all");

        if(!String.IsNullOrWhiteSpace(status))
        {
            throw new CakeException("Registry image builds and pushes require a clean working tree.");
        }

    }

    private static String? GetOptionalArgument(BuildContext context , String name)
    {
        var values = context.Arguments.GetArguments(name);

        if(values.Count > 1)
        {
            throw new CakeException($"Specify --{name} only once.");
        }

        foreach(var value in values)
        {
            return value;
        }

        return null;
    }

    private static String RunGit(BuildContext context , String arguments)
    {
        var settings = new Cake.Core.IO.ProcessSettings
        {
            Arguments = new Cake.Core.IO.ProcessArgumentBuilder().Append(arguments),
            WorkingDirectory = context.RepositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        var process = context.ProcessRunner.Start("git",settings); process.WaitForExit();

        if(process.GetExitCode() != 0)
        {
            throw new CakeException($"git {arguments} failed: {String.Join(Environment.NewLine,process.GetStandardError())}");
        }

        return String.Join(Environment.NewLine,process.GetStandardOutput());
    }
}
