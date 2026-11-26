namespace KusDepotDataPod.Build;

[TaskName("PushImages")]
public sealed class PushImagesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var registry = context.Arguments.GetArguments("registry");

        var commit = context.Arguments.GetArguments("commit");

        if(registry.Count != 1 || commit.Count != 1)
        {
            throw new CakeException("PushImages requires exactly one --registry and one --commit argument.");
        }

        var references = DataPodImages.GetReferences(context);

        DataPodImages.ValidateRegistrySource(context,commit.Single());

        foreach(var reference in references)
        {
            var settings = new Cake.Core.IO.ProcessSettings
            {
                Arguments = new Cake.Core.IO.ProcessArgumentBuilder()
                    .Append("push")
                    .AppendQuoted(reference),

                WorkingDirectory = context.RepositoryRoot,
            };

            var exitCode = context.StartProcess("docker",settings);

            if(exitCode != 0)
            {
                throw new CakeException($"Docker push failed for {reference} with exit code {exitCode}.");
            }
        }
    }
}
