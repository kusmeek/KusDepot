namespace KusDepotDataPod.Build;

[TaskName("BuildImages")]
public sealed class BuildImagesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var registry = context.Arguments.GetArguments("registry");

        var commits = context.Arguments.GetArguments("commit");

        if(registry.Count > 1 || commits.Count > 1)
        {
            throw new CakeException("BuildImages accepts at most one --registry and one --commit argument.");
        }

        if(registry.Count == 1 != (commits.Count == 1))
        {
            throw new CakeException("Specify both --registry and --commit for a registry image build, or neither for a local build.");
        }

        if(commits.Count == 1)
        {
            DataPodImages.ValidateRegistrySource(context,commits.Single());
        }

        var references = DataPodImages.GetReferences(context);

        var dockerfiles = DataPodImages.GetDockerfiles(context);

        for(var index = 0; index < references.Length; index++)
        {
            var settings = new Cake.Core.IO.ProcessSettings
            {
                Arguments = new Cake.Core.IO.ProcessArgumentBuilder()
                    .Append("build")
                    .Append("--file")
                    .AppendQuoted(dockerfiles[index])
                    .Append("--tag")
                    .AppendQuoted(references[index])
                    .AppendQuoted(context.RepositoryRoot),

                WorkingDirectory = context.RepositoryRoot,
            };

            var exitCode = context.StartProcess("docker",settings);

            if(exitCode != 0)
            {
                throw new CakeException($"Docker build failed for {references[index]} with exit code {exitCode}.");
            }
        }
    }
}
