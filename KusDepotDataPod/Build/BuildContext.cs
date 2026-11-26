namespace KusDepotDataPod.Build;

public sealed class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context) : base(context)
    {
        DirectoryInfo? directory = new(System.Environment.CurrentDirectory);

        while(directory is not null && !File.Exists(Path.Combine(directory.FullName,"KusDepot.slnx")))
        {
            directory = directory.Parent;
        }

        if(directory is null)
        {
            throw new CakeException("Could not locate the repository root containing KusDepot.slnx.");
        }

        RepositoryRoot = directory.FullName;

        BuildConfiguration = "Release";
    }

    public String RepositoryRoot { get; }

    public String BuildConfiguration { get; }

    public String ProjectPath(String relativePath) => Path.Combine(RepositoryRoot,relativePath);
}