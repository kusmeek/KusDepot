namespace KusDepot;

public abstract partial class DataItem
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="AcquireLocks"]/*'/>*/
    protected virtual void AcquireLocks()
    {
        if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

        if(!this.Sync.Data.Wait(SyncTime)) { this.Sync.Meta.Release(); throw SyncException; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="AcquireLocksAsync"]/*'/>*/
    protected virtual async Task AcquireLocksAsync(CancellationToken cancel = default)
    {
        if(!await this.Sync.Meta.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; }

        if(!await this.Sync.Data.WaitAsync(SyncTime,cancel).ConfigureAwait(false))
        {
            this.Sync.Meta.Release(); throw SyncException;
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetSyncLockOrder"]/*'/>*/
    protected Int64 GetSyncLockOrder()
    {
        if(this.SyncLockOrder == 0)
        {
            Int64 n = Interlocked.Increment(ref SyncLockCount);

            Interlocked.CompareExchange(ref this.SyncLockOrder,n,0);
        }

        return this.SyncLockOrder;
    }

    ///<inheritdoc/>
    protected override SyncNode GetSyncNode() { this.Sync ??= new(); return this.Sync; }

    ///<inheritdoc/>
    protected override void RefreshSyncNodes()
    {
        try { this.Sync = (this.GetSyncNode() as DataSync)!; base.RefreshSyncNodes(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,RefreshSyncNodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ReleaseLocks"]/*'/>*/
    protected virtual void ReleaseLocks()
    {
        this.Sync.Data.Release(); this.Sync.Meta.Release();
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ReleaseSyncLocks"]/*'/>*/
    protected virtual void ReleaseSyncLocks(LockState state)
    {
        if(state.SecondData) { state.Second!.Sync.Data.Release(); }
        if(state.SecondMeta) { state.Second!.Sync.Meta.Release(); }
        if(state.FirstData)  { state.First!.Sync.Data.Release(); }
        if(state.FirstMeta)  { state.First!.Sync.Meta.Release(); }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryAcquireMetaLocks"]/*'/>*/
    protected virtual Boolean TryAcquireMetaLocks(DataItem? other , out LockState state)
    {
        state = default;

        if(other is null) { return false; }

        DataItem f = this; DataItem s = other;

        if(f.GetSyncLockOrder() > s.GetSyncLockOrder()) { f = other; s = this; }

        Boolean fm = false; Boolean sm = false; Boolean ok = false;

        try
        {
            if(!f.Sync.Meta.Wait(SyncTime)) { return false; } fm = true;
            if(!s.Sync.Meta.Wait(SyncTime)) { return false; } sm = true;

            state = new(f,s,fm,false,sm,false); ok = true; return true;
        }
        finally
        {
            if(ok) { }

            else
            {
                if(sm) { s.Sync.Meta.Release(); }
                if(fm) { f.Sync.Meta.Release(); }
            }
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryAcquireSyncLocks"]/*'/>*/
    protected virtual Boolean TryAcquireSyncLocks(DataItem? other , out LockState state)
    {
        state = default;

        if(other is null) { return false; }

        DataItem f = this; DataItem s = other;

        if(f.GetSyncLockOrder() > s.GetSyncLockOrder()) { f = other; s = this; }

        Boolean fm = false; Boolean fd = false; Boolean sm = false; Boolean sd = false; Boolean ok = false;

        try
        {
            if(!f.Sync.Meta.Wait(SyncTime)) { return false; } fm = true;
            if(!f.Sync.Data.Wait(SyncTime)) { return false; } fd = true;
            if(!s.Sync.Meta.Wait(SyncTime)) { return false; } sm = true;
            if(!s.Sync.Data.Wait(SyncTime)) { return false; } sd = true;

            state = new(f,s,fm,fd,sm,sd); ok = true; return true;
        }
        finally
        {
            if(ok) { }

            else
            {
                if(sd) { s.Sync.Data.Release(); }
                if(sm) { s.Sync.Meta.Release(); }
                if(fd) { f.Sync.Data.Release(); }
                if(fm) { f.Sync.Meta.Release(); }
            }
        }
    }
}