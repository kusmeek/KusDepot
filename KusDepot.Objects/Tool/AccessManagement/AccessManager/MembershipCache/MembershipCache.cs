namespace KusDepot.Security;

/**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/main/*'/>*/
public sealed class MembershipCache
{
    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/field[@name="DefaultMaximumCount"]/*'/>*/
    public const Int32 DefaultMaximumCount = 1024;

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/field[@name="Entries"]/*'/>*/
    private readonly Dictionary<MembershipCacheKey,AccessKeyToken> Entries;

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/field[@name="MaximumCount"]/*'/>*/
    private readonly Int32 MaximumCount;

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/field[@name="Sync"]/*'/>*/
    private readonly Lock Sync;

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/constructor[@name="Constructor"]/*'/>*/
    public MembershipCache(Int32 maximumcount = DefaultMaximumCount)
    {
        this.Sync = new(); this.Entries = new(); this.MaximumCount = maximumcount <= 0 ? DefaultMaximumCount : maximumcount;
    }

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/method[@name="Clear"]/*'/>*/
    public void Clear() { lock(this.Sync) { this.Entries.Clear(); } }

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/method[@name="Remove"]/*'/>*/
    public Boolean Remove(MembershipCacheKey key)
    {
        try
        {
            lock(this.Sync) { return this.Entries.Remove(key); }
        }
        catch { return false; }
    }

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/method[@name="TryGet"]/*'/>*/
    public Boolean TryGet(MembershipCacheKey key , out AccessKeyToken token)
    {
        token = default;

        try
        {
            lock(this.Sync) { return this.Entries.TryGetValue(key,out token); }
        }
        catch { token = default; return false; }
    }

    /**<include file='MembershipCache.xml' path='MembershipCache/class[@name="MembershipCache"]/method[@name="TrySet"]/*'/>*/
    public Boolean TrySet(MembershipCacheKey key , AccessKeyToken token)
    {
        try
        {
            lock(this.Sync)
            {
                if(this.Entries.Count >= MaximumCount && this.Entries.ContainsKey(key) is false) { this.Entries.Clear(); }

                this.Entries[key] = token; return true;
            }
        }
        catch { return false; }
    }
}