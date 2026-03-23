namespace KusDepot;

/**<include file='KusDepotRegistry.xml' path='KusDepotRegistry/class[@name="KusDepotRegistry"]/main/*'/>*/
public static class KusDepotRegistry
{
    /**<include file='KusDepotRegistry.xml' path='KusDepotRegistry/class[@name="KusDepotRegistry"]/method[@name="UpdateCommandRegistry"]/*'/>*/
    public static void UpdateCommandRegistry()
    {
        Dictionary<String,HashSet<Guid>> r = new();

        var ids = Tool.GetInstanceIDs(); if(ids is null) { Commands = r; return; }

        foreach(var id in ids)
        {
            var t = Tool.GetInstance(id); if(t is null) { continue; }

            var d = t.GetCommandDescriptors(); if(d is null) { continue; }

            foreach(var c in d)
            {
                if(c.RegisteredHandles is null) { continue; }

                foreach(var h in c.RegisteredHandles)
                {
                    if(String.IsNullOrEmpty(h)) { continue; }

                    if(r.TryGetValue(h,out HashSet<Guid>? tools) is false)
                    {
                        tools = new(); r[h] = tools;
                    }

                    tools.Add(id);
                }
            }
        }

        Commands = r;
    }

    /**<include file='KusDepotRegistry.xml' path='KusDepotRegistry/class[@name="KusDepotRegistry"]/property[@name="Commands"]/*'/>*/
    public static Dictionary<String,HashSet<Guid>>? Commands { get; private set; }
}