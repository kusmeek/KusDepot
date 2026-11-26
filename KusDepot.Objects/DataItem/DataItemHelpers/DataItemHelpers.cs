namespace KusDepot;

/**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/main/*'/>*/
public static class DataItemHelpers
{
    /**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/method[@name="TryDistinctByID"]/*'/>*/
    public static Boolean TryDistinctByID(this IEnumerable<DataItem>? items , [MaybeNullWhen(false)] out HashSet<DataItem> result)
    {
        result = null; if(items is null) { return false; }

        try { result = new(items.GroupBy(_ => _.GetID()).Select(_ => _.First()),new IDEquality()); return true; }

        catch { return false; }
    }

    /**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/method[@name="TryCloneDistinctByID"]/*'/>*/
    public static Boolean TryCloneDistinctByID(this IEnumerable<DataItem>? items , [MaybeNullWhen(false)] out HashSet<DataItem> result)
    {
        result = null; if(items is null) { return false; }

        try
        {
            List<DataItem> distinct = [];

            HashSet<Guid?> ids = [];

            foreach(DataItem? item in items)
            {
                if(item is null) { continue; }

                if(ids.Add(item.GetID())) { distinct.Add(item); }
            }

            HashSet<DataItem> clones = new(new IDEquality());

            foreach(DataItem item in distinct)
            {
                DataItem? clone = item.Clone(); if(clone is null || clone.GetID() != item.GetID() || !clones.Add(clone)) { return false; }
            }

            result = clones; return true;
        }
        catch { return false; }
    }

    /**<include file='DataItemHelpers.xml' path='DataItemHelpers/class[@name="DataItemHelpers"]/method[@name="TryAddByID"]/*'/>*/
    public static Boolean TryAddByID(this HashSet<DataItem>? target , IEnumerable<DataItem>? items , out Int32 added)
    {
        added = 0; if(target is null || items is null) { return false; }

        List<DataItem> committed = [];

        try
        {
            HashSet<DataItem> existing = new(target,new IDEquality());

            List<DataItem> clones = [];

            foreach(DataItem? item in items)
            {
                if(item is null) { continue; }

                if(existing.Add(item) is false) { continue; }

                DataItem? clone = item.Clone(); if(clone is null || clone.GetID() != item.GetID()) { return false; }

                clones.Add(clone);
            }

            foreach(DataItem clone in clones)
            {
                if(!target.Add(clone)) { RollbackAdded(); return false; }

                committed.Add(clone);
            }

            added = committed.Count; return true;
        }
        catch { RollbackAdded(); return false; }

        void RollbackAdded()
        {
            foreach(DataItem item in committed) { try { target.Remove(item); } catch {} }

            committed.Clear();
        }
    }
}