namespace KusDepot;

/**<include file='KusDepotRegistry.xml' path='KusDepotRegistry/class[@name="KusDepotRegistry"]/main/*'/>*/
public static class KusDepotRegistry
{
    /**<include file='KusDepotRegistry.xml' path='KusDepotRegistry/class[@name="KusDepotRegistry"]/method[@name="UpdateCommandRegistry"]/*'/>*/
    public static void UpdateCommandRegistry()
    {
        Commands = Tool.GetInstanceIDs()
            ?.Select(id => Tool.GetInstance(id))
            ?.SelectMany(t => t?.GetCommandTypes()
            ?? new Dictionary<String,String>(),(t,c) => new { Handle = c.Key , Tool = t })
            ?.GroupBy(_ =>_.Handle)?.ToDictionary(g => g.Key,g => g.ToList().Select((g) => g.Tool!).ToHashSet());}

    /**<include file='KusDepotRegistry.xml' path='KusDepotRegistry/class[@name="KusDepotRegistry"]/property[@name="Commands"]/*'/>*/
    public static Dictionary<String,HashSet<ITool>>? Commands { get; private set; }
}