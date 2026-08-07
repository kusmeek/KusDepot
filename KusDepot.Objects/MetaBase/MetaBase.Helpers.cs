namespace KusDepot;

public abstract partial class MetaBase
{
    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="CreateIsolatedSet"]/*'/>*/
    private static HashSet<String> CreateIsolatedSet(IEnumerable<String> values)
    {
        HashSet<String> _ = values is ICollection<String> c ? new(c.Count) : new();

        foreach(var v in values) { _.Add(new(v)); }

        return _;
    }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="GetCachedInheritanceList"]/*'/>*/
    protected String GetCachedInheritanceList()
    {
        return InheritanceCache.GetOrAdd(this.GetType(),static (_,state) => GetInheritanceList(state)!,this);
    }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="ClearCachedFileSize_NoSync"]/*'/>*/
    protected void ClearCachedFileSize_NoSync()
    {
        this.FileSizeCached = null;
    }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="ResolveCachedFileSize_NoSync"]/*'/>*/
    protected Int64? ResolveCachedFileSize_NoSync()
    {
        if(String.IsNullOrEmpty(this.FILE)) { return null; }

        if(this.FileSizeCached is not null) { return this.FileSizeCached; }

        this.FileSizeCached = GetFileSize(this.FILE); return this.FileSizeCached;
    }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="SetCachedFileSize_NoSync"]/*'/>*/
    protected void SetCachedFileSize_NoSync(Int64? size)
    {
        this.FileSizeCached = size;
    }
}