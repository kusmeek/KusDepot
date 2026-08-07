namespace KusDepot;

public abstract partial class DataItem
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Clone"]/*'/>*/
    public virtual DataItem? Clone()
    {
        Boolean lk = false;
        try
        {
            this.AcquireLocks(); lk = true;

            return OrleansUtility.Deserialize<DataItem>(OrleansUtility.Serialize<DataItem>(this));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CloneFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(lk) { this.ReleaseLocks(); } }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Clone_NoSync"]/*'/>*/
    protected DataItem? Clone_NoSync()
    {
        try
        {
            return OrleansUtility.Deserialize<DataItem>(OrleansUtility.Serialize<DataItem>(this));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CloneFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="CloneOpen"]/*'/>*/
    public TResult? Clone<TResult>() where TResult : DataItem
    {
        Boolean lk = false;
        try
        {
            this.AcquireLocks(); lk = true;

            return Deserialize<TResult>(OrleansUtility.Serialize<DataItem>(this));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CloneFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(lk) { this.ReleaseLocks(); } }
    }

    ///<inheritdoc/>
    Object ICloneable.Clone() { return this.Clone()!; }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="FromFile"]/*'/>*/
    public static TResult? FromFile<TResult>(String path) where TResult : DataItem
    {
        try
        {
            if(path is null) { return default; }

            if(!File.Exists(path)) { return default; }

            return OrleansUtility.FromFile<TResult>(path);
        }
        catch ( Exception _ )
        {
            KusDepotLog.Error(_,FromFileFail); if(NoExceptions) { return default; } throw;
        }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="ParseAny"]/*'/>*/
    public static DataItem? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; }

            return OrleansUtility.ParseBase64<DataItem>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DeserializeAny"]/*'/>*/
    public static DataItem? Deserialize(Byte[] input , IFormatProvider? format = null)
    {
        try
        {
            if(input is null || input.Length == 0) { return null; }

            return OrleansUtility.Deserialize<DataItem>(input);
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,DeserializeFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : DataItem
    {
        return OrleansUtility.ParseBase64<TResult>(input);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Deserialize"]/*'/>*/
    public static TResult? Deserialize<TResult>(Byte[] input , IFormatProvider? format = null) where TResult : DataItem
    {
        return OrleansUtility.Deserialize<TResult>(input);
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="DeserializeMemoryStream"]/*'/>*/
    protected static TResult? Deserialize<TResult>(MemoryStream input) where TResult : DataItem
    {
        if(input.TryGetBuffer(out ArraySegment<Byte> buffer) && buffer.Array is not null)
        {
            return OrleansUtility.Deserialize<TResult>(new ReadOnlyMemory<Byte>(buffer.Array,buffer.Offset,buffer.Count));
        }

        return OrleansUtility.Deserialize<TResult>(input.ToArray());
    }

    ///<inheritdoc/>
    public virtual Boolean ToFile(String path)
    {
        Boolean lk = false;
        try
        {
            if(File.Exists(path)) { return false; }

            this.AcquireLocks(); lk = true;

            if(!Locked) { this.ObjectFILE = path; }

            return OrleansUtility.ToFile(path,this);
        }
        finally { if(lk) { this.ReleaseLocks(); } }
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        Boolean lk = false;
        try
        {
            this.AcquireLocks(); lk = true;

            return OrleansUtility.ToBase64String<DataItem>(this);
        }
        finally { if(lk) { this.ReleaseLocks(); } }
    }

    ///<inheritdoc/>
    public Byte[] Serialize() { return OrleansUtility.Serialize<DataItem>(this); }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : DataItem
    {
        result = null; if(input is null) { return false; }

        try
        {
            return OrleansUtility.TryParseBase64(input,out result);
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="TryDeserialize"]/*'/>*/
    public static Boolean TryDeserialize<TResult>(Byte[]? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : DataItem
    {
        result = null; if(input is null || input.Length == 0) { return false; }

        try
        {
            result = OrleansUtility.Deserialize<TResult>(input); return result is not null;
        }
        catch ( SerializationException ) { return false; }

        catch ( Exception _ )
        {
            if(NoExceptions) { return false; } KusDepotLog.Error(_,TryDeserializeFail); throw;
        }
    }
}