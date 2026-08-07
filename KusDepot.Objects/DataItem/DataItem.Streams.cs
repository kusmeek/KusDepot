namespace KusDepot;

public abstract partial class DataItem
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CreateBuffer"]/*'/>*/
    protected static MemoryStream CreateBuffer(Int32 capacity = 0)
    {
        return capacity > 0 ? new MemoryStream(capacity) : new MemoryStream();
    }

    ///<inheritdoc/>
    public virtual Stream? GetContentStream()
    {
        try
        {
            if( this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE) ) { return null; }

            if(!this.Sync.Data.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(File.Exists(this.FILE))
                {
                    try { return OpenSequentialRead(this.FILE); }
                    catch {}
                }
            }
            finally { this.Sync.Data.Release(); }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetContentStream_NoSync"]/*'/>*/
    protected virtual Stream? GetContentStream_NoSync()
    {
        try
        {
            if( this.ContentStreamed is false || String.IsNullOrEmpty(this.FILE) ) { return null; }

            if(File.Exists(this.FILE)) { try { return OpenSequentialRead(this.FILE); } catch {} }

            return null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetContentStreamFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetMemoryStreamBuffer"]/*'/>*/
    protected static Byte[] GetMemoryStreamBuffer(MemoryStream data)
    {
        if(data.TryGetBuffer(out ArraySegment<Byte> buffer) && buffer.Offset == 0 && buffer.Array is not null && buffer.Array.Length == buffer.Count) { return buffer.Array; }

        return data.ToArray();
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetUtf16StringMemoryStream"]/*'/>*/
    protected static String GetUtf16String(MemoryStream? data)
    {
        if(data is null) { return String.Empty; }

        if(data.TryGetBuffer(out ArraySegment<Byte> buffer) && buffer.Array is not null) { return Encoding.Unicode.GetString(buffer.Array,buffer.Offset,buffer.Count); }

        return Encoding.Unicode.GetString(data.ToArray());
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="OpenExclusiveWrite"]/*'/>*/
    protected static Stream OpenExclusiveWrite(String path)
    {
        return new FileStream(path,new FileStreamOptions{ Mode = FileMode.Open , Access = FileAccess.Write , Share = FileShare.None , Options = FileOptions.Asynchronous });
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="OpenSequentialRead"]/*'/>*/
    protected static Stream OpenSequentialRead(String path)
    {
        return new FileStream(path,new FileStreamOptions{ Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.Read , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });
    }
}