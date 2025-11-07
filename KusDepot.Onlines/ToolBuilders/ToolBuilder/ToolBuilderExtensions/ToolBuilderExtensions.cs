namespace KusDepot;

/**<include file='ToolBuilderExtensions.xml' path='ToolBuilderExtensions/class[@name="ToolBuilderExtensions"]/main/*'/>*/
public static class ToolBuilderExtensions
{
    /**<include file='ToolBuilderExtensions.xml' path='ToolBuilderExtensions/class[@name="ToolBuilderExtensions"]/method[@name="ToArgument"]/*'/>*/
    public static IEnumerable<Meta<ICommand>>? ToArgument(this Dictionary<String,ICommand>? commands)
    {
        if(commands is null) { yield break; }

        foreach(KeyValuePair<String,ICommand> _ in commands)
        {
            yield return new Meta<ICommand>(_.Value,new Dictionary<String,Object?>(){{"Handle",_.Key}});
        }
    }
}