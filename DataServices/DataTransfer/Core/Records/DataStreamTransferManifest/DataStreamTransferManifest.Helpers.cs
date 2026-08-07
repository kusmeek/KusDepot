namespace KusDepot.Data.Transfer;

public sealed partial record DataStreamTransferManifest
{
    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="CanReadRange"]/*'/>*/
    public Boolean CanReadRange(DataItemTransferRange? range)
    {
        return range is not null && range.Validate() && range.EndOffsetExclusive <= this.AppendedLength;
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="CanWriteRange"]/*'/>*/
    public Boolean CanWriteRange(DataItemTransferRange? range)
    {
        return range is not null && range.Validate() && range.Offset == this.AppendedLength;
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Deserialize"]/*'/>*/
    public static DataStreamTransferManifest? Deserialize(Byte[]? input) { return JsonUtility.Deserialize<DataStreamTransferManifest>(input); }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="GetRealizedBytes"]/*'/>*/
    public Int64 GetRealizedBytes() { return this.AppendedLength; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Load"]/*'/>*/
    public static DataStreamTransferManifest? Load(String path) { return JsonUtility.FromFile<DataStreamTransferManifest>(path); }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Parse"]/*'/>*/
    public static DataStreamTransferManifest? Parse(String? input) { return JsonUtility.Parse<DataStreamTransferManifest>(input); }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Save"]/*'/>*/
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
        catch ( Exception _ ) { KusDepotLog.Error(_,SaveFileFail,typeof(DataStreamTransferManifest).FullName); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize() { return JsonUtility.Serialize(this); }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="ToString"]/*'/>*/
    public override String ToString() { return JsonUtility.ToJsonString(this); }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="ToState"]/*'/>*/
    public DataStreamTransferState ToState()
    {
        return new()
        {
            AppendedLength = this.AppendedLength,
            Created = this.Created,
            FaultMessage = this.FaultMessage,
            ItemID = this.ItemID,
            Mode = this.Mode,
            ObjectInfo = this.ObjectInfo,
            ObjectSHA512 = this.ObjectSHA512,
            SegmentSizePolicy = this.SegmentSizePolicy,
            SessionID = this.SessionID,
            SourceSessionID = this.SourceSessionID,
            StateVersion = this.StateVersion,
            Status = this.Status,
            StreamSHA512 = this.StreamSHA512,
            Updated = this.Updated,
        };
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Touch"]/*'/>*/
    public DataStreamTransferManifest Touch(Boolean clearfault = false)
    {
        return this with { FaultMessage = clearfault ? null : this.FaultMessage , StateVersion = this.StateVersion + 1 , Updated = DateTimeOffset.UtcNow };
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , [MaybeNullWhen(false)] out DataStreamTransferManifest manifest)
    {
        manifest = Parse(input);

        return manifest is not null;
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="Validate"]/*'/>*/
    public Boolean Validate()
    {
        return this.AppendedLength >= 0 &&
               this.ItemID != Guid.Empty &&
               (this.ObjectPayload.Length == 0 || ValidateHash(this.ObjectSHA512)) &&
               this.SegmentSizePolicy.Validate() &&
               this.SessionID != Guid.Empty &&
               ValidateOptionalHash(this.StreamSHA512);
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="ValidateHash"]/*'/>*/
    private static Boolean ValidateHash(Byte[]? hash) { return hash is { Length: 64 }; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="ValidateOptionalHash"]/*'/>*/
    private static Boolean ValidateOptionalHash(Byte[]? hash) { return hash is null || hash.Length == 0 || ValidateHash(hash); }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="WithAppendedLength"]/*'/>*/
    public DataStreamTransferManifest WithAppendedLength(Int64 appendedlength)
    {
        if(appendedlength < this.AppendedLength) { return this; }

        return this with { AppendedLength = appendedlength , FaultMessage = null , StateVersion = this.StateVersion + 1 , Updated = DateTimeOffset.UtcNow };
    }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/method[@name="WithStatus"]/*'/>*/
    public DataStreamTransferManifest WithStatus(DataItemTransferStatus status , String? faultmessage = null)
    {
        return this with { FaultMessage = faultmessage , StateVersion = this.StateVersion + 1 , Status = status , Updated = DateTimeOffset.UtcNow };
    }
}
