namespace KusDepotDataPod.Build;

public abstract class PulumiTask : FrostingTask<BuildContext>
{
    protected static void RunPulumi(BuildContext context, String command)
    {
        var stacks = context.Arguments.GetArguments("stack");

        if(stacks.Count != 1)
        {
            throw new CakeException($"{command} requires exactly one non-empty --stack argument.");
        }

        var stackValue = stacks.Single();

        if(String.IsNullOrWhiteSpace(stackValue))
        {
            throw new CakeException($"{command} requires exactly one non-empty --stack argument.");
        }

        var stack = stackValue.Trim();

        var arguments = new Cake.Core.IO.ProcessArgumentBuilder()
            .Append(command)
            .Append("--stack")
            .AppendQuoted(stack);

        if(command == "up")
        {
            arguments.Append("--yes");
        }

        var settings = new Cake.Core.IO.ProcessSettings
        {
            Arguments = arguments,
            WorkingDirectory = context.ProjectPath("KusDepotDataPod/Infrastructure"),
        };

        var exitCode = context.StartProcess("pulumi",settings);

        if(exitCode != 0)
        {
            throw new CakeException($"Pulumi {command} failed for stack '{stack}' with exit code {exitCode}.");
        }
    }
}
