namespace KusDepot;

/**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/main/*'/>*/
public static class DataItemHelpers
{
    /**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/method[@name="TryDistinctByID"]/*'/>*/
    public static Boolean TryDistinctByID(this IEnumerable<DataItem>? items , [MaybeNullWhen(false)] out HashSet<DataItem> result)
    {
        result = null; if(items is null) { return false; }

        try { result = items.GroupBy(_ => _.GetID()).Select(_ => _.First()).ToHashSet(); return true; }

        catch { return false; }
    }

    /**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/method[@name="TryCloneDistinctByID"]/*'/>*/
    public static Boolean TryCloneDistinctByID(this IEnumerable<DataItem>? items , [MaybeNullWhen(false)] out HashSet<DataItem> result)
    {
        result = null; if(items is null) { return false; }

        try
        {
            var clones = items.Select(i => i?.Clone()).Where(c => c is not null).Cast<DataItem>();

            result = clones.GroupBy(_ => _.GetID()).Select(_ => _.First()).ToHashSet(); return true;
        }
        catch { return false; }
    }

    /**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/method[@name="TryAddByID"]/*'/>*/
    public static Boolean TryAddByID(this HashSet<DataItem>? target , IEnumerable<DataItem>? items , out Int32 added)
    {
        added = 0; if(target is null || items is null) { return false; }

        try
        {
            var existing = target.Select(d => d.GetID()).ToHashSet();

            foreach(var i in items)
            {
                var id = i?.GetID(); if(existing.Add(id))
                {
                    var c = i?.Clone(); if(c is not null) { target.Add(c); added++; }
                }
            }

            return true;
        }
        catch { return false; }
    }
}