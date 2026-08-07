namespace KusDepot.Data.Transfer;

public sealed partial record DataItemTransferManifest
{
    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="AreRangesWithinStreamLengthCore"]/*'/>*/
    private static Boolean AreRangesWithinStreamLengthCore(DataItemTransferRange[] realized , Int64 streamlength)
    {
        for(Int32 i = 0 ; i < realized.Length ; i++)
        {
            if(realized[i].EndOffsetExclusive > streamlength) { return false; }
        }

        return true;
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="CanReadRange"]/*'/>*/
    public Boolean CanReadRange(DataItemTransferRange? range)
    {
        if(range is null || range.Length <= 0 || !range.Validate()) { return false; }

        DataItemTransferRange[] realized = NormalizeRangesCore(this.RealizedRanges);

        for(Int32 i = 0 ; i < realized.Length ; i++)
        {
            if(realized[i].Contains(range)) { return true; }
        }

        return false;
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="CanWriteRange"]/*'/>*/
    public Boolean CanWriteRange(DataItemTransferRange? range)
    {
        return range is not null && range.Validate() && range.EndOffsetExclusive <= this.StreamLength;
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="EnumerateRangesWithOneMore"]/*'/>*/
    private static IEnumerable<DataItemTransferRange> EnumerateRangesWithOneMore(IEnumerable<DataItemTransferRange>? ranges , DataItemTransferRange range)
    {
        if(ranges is not null)
        {
            foreach(DataItemTransferRange current in ranges) { yield return current; }
        }

        yield return range;
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="GetMissingRanges"]/*'/>*/
    public DataItemTransferRange[] GetMissingRanges() { return GetMissingRangesCore(NormalizeRangesCore(this.RealizedRanges),this.StreamLength); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="GetMissingRangesCore"]/*'/>*/
    private static DataItemTransferRange[] GetMissingRangesCore(DataItemTransferRange[] realized , Int64 streamlength)
    {
        if(streamlength <= 0) { return Array.Empty<DataItemTransferRange>(); }

        List<DataItemTransferRange> missing = new(); Int64 cursor = 0;

        foreach(DataItemTransferRange range in realized)
        {
            if(range.Offset > cursor) { missing.Add(new(){ Offset = cursor , Length = range.Offset - cursor }); }

            cursor = Math.Max(cursor,range.EndOffsetExclusive);
        }

        if(cursor < streamlength) { missing.Add(new(){ Offset = cursor , Length = streamlength - cursor }); }

        return missing.ToArray();
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="GetRealizedBytes"]/*'/>*/
    public Int64 GetRealizedBytes() { return GetRealizedBytesCore(NormalizeRangesCore(this.RealizedRanges)); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="GetRealizedBytesCore"]/*'/>*/
    private static Int64 GetRealizedBytesCore(DataItemTransferRange[] realized)
    {
        Int64 total = 0;

        for(Int32 i = 0 ; i < realized.Length ; i++) { total += realized[i].Length; }

        return total;
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="HasFullCoverage"]/*'/>*/
    public Boolean HasFullCoverage() { return GetMissingRanges().Length == 0; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="NormalizeRanges"]/*'/>*/
    public DataItemTransferManifest NormalizeRanges() { return this with { RealizedRanges = NormalizeRangesCore(this.RealizedRanges) }; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="NormalizeRangesCore"]/*'/>*/
    private static DataItemTransferRange[] NormalizeRangesCore(IEnumerable<DataItemTransferRange>? ranges)
    {
        if(ranges is null) { return Array.Empty<DataItemTransferRange>(); }

        List<DataItemTransferRange> ordered = new();

        foreach(DataItemTransferRange? range in ranges)
        {
            if(range is not null && range.Validate() && range.Length > 0) { ordered.Add(range); }
        }

        if(ordered.Count == 0) { return Array.Empty<DataItemTransferRange>(); }

        ordered.Sort(static (left,right) => left.Offset.CompareTo(right.Offset));

        List<DataItemTransferRange> merged = new(){ ordered[0] };

        for(Int32 i = 1 ; i < ordered.Count ; i++)
        {
            DataItemTransferRange current = merged[^1]; DataItemTransferRange next = ordered[i];

            if(current.CanMerge(next)) { merged[^1] = current.Merge(next); }

            else { merged.Add(next); }
        }

        return merged.ToArray();
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="ToState"]/*'/>*/
    public DataItemTransferState ToState()
    {
        DataItemTransferRange[] realized = NormalizeRangesCore(this.RealizedRanges); DataItemTransferRange[] missing = GetMissingRangesCore(realized,this.StreamLength);

        Int64 realizedBytes = GetRealizedBytesCore(realized); Int64 remainingBytes = Math.Max(0,this.StreamLength - realizedBytes);

        return new()
        {
            SessionID = this.SessionID,
            ItemID = this.ItemID,
            SourceSessionID = this.SourceSessionID,
            ObjectSHA512 = this.ObjectSHA512,
            StreamSHA512 = this.StreamSHA512,
            StreamLength = this.StreamLength,
            Status = this.Status,
            Mode = this.Mode,
            StateVersion = this.StateVersion,
            Created = this.Created,
            Updated = this.Updated,
            RealizedBytes = realizedBytes,
            RemainingBytes = remainingBytes,
            RealizedRanges = realized,
            MissingRanges = missing,
            SegmentSizePolicy = this.SegmentSizePolicy,
            ObjectInfo = this.ObjectInfo,
            FaultMessage = this.FaultMessage,
        };
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Touch"]/*'/>*/
    public DataItemTransferManifest Touch(Boolean clearfault = false)
    {
        return this with { Updated = DateTimeOffset.UtcNow , StateVersion = this.StateVersion + 1 , FaultMessage = clearfault ? null : this.FaultMessage };
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="WithAddedRange"]/*'/>*/
    public DataItemTransferManifest WithAddedRange(DataItemTransferRange range)
    {
        if(!CanWriteRange(range)) { return this; }

        DataItemTransferRange[] realized = NormalizeRangesCore(EnumerateRangesWithOneMore(this.RealizedRanges,range));

        return this with
        {
            RealizedRanges = realized,
            Updated = DateTimeOffset.UtcNow,
            StateVersion = this.StateVersion + 1,
            Status = GetMissingRangesCore(realized,this.StreamLength).Length == 0 ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,
            FaultMessage = null,
        };
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="WithStatus"]/*'/>*/
    public DataItemTransferManifest WithStatus(DataItemTransferStatus status , String? faultmessage = null)
    {
        return this with { Status = status , Updated = DateTimeOffset.UtcNow , StateVersion = this.StateVersion + 1 , FaultMessage = faultmessage };
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize() { return JsonUtility.Serialize(this); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Save"]/*'/>*/
    public Boolean Save(String path)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(path)) { return false; }

            String fullPath = Path.GetFullPath(path); String? directory = Path.GetDirectoryName(fullPath);

            if(String.IsNullOrWhiteSpace(directory) is false && Directory.Exists(directory) is false) { Directory.CreateDirectory(directory); }

            File.WriteAllBytes(fullPath,Serialize());

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SaveFileFail,typeof(DataItemTransferManifest).FullName); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Parse"]/*'/>*/
    public static DataItemTransferManifest? Parse(String? input) { return JsonUtility.Parse<DataItemTransferManifest>(input); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Deserialize"]/*'/>*/
    public static DataItemTransferManifest? Deserialize(Byte[]? input) { return JsonUtility.Deserialize<DataItemTransferManifest>(input); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , [MaybeNullWhen(false)] out DataItemTransferManifest manifest)
    {
        manifest = Parse(input);

        return manifest is not null;
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Load"]/*'/>*/
    public static DataItemTransferManifest? Load(String path) { return JsonUtility.FromFile<DataItemTransferManifest>(path); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="ToString"]/*'/>*/
    public override String ToString() { return JsonUtility.ToJsonString(this); }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="Validate"]/*'/>*/
    public Boolean Validate()
    {
        DataItemTransferRange[] realized = NormalizeRangesCore(this.RealizedRanges);

        return this.SessionID != Guid.Empty &&
               this.ItemID != Guid.Empty &&
               this.StreamLength >= 0 &&
               ValidateObjectHash(this.ObjectSHA512) &&
               ValidateStreamHash(this.StreamSHA512,this.StreamLength) &&
               this.SegmentSizePolicy.Validate() &&
               AreRangesWithinStreamLengthCore(realized,this.StreamLength);
    }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="ValidateObjectHash"]/*'/>*/
    private static Boolean ValidateObjectHash(Byte[]? hash) { return hash is { Length: 64 }; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/method[@name="ValidateStreamHash"]/*'/>*/
    private static Boolean ValidateStreamHash(Byte[]? hash , Int64 streamlength)
    {
        if(hash is null) { return false; }

        return streamlength == 0 ? hash.Length == 0 : hash.Length == 64;
    }
}