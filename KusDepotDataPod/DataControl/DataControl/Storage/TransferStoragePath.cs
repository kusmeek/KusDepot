namespace DataPodServices.DataControl;

/**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/main/*'/>*/
internal sealed record TransferStoragePath
{
    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="SessionID"]/*'/>*/
    public Guid SessionID { get; init; }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="ItemInstanceToken"]/*'/>*/
    public Guid ItemInstanceToken { get; init; }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="Prefix"]/*'/>*/
    public String Prefix => CreatePrefix(this.ItemInstanceToken,this.SessionID);

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="InstancePrefix"]/*'/>*/
    public String InstancePrefix => ForInstance(this.ItemInstanceToken).Prefix;

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="PublishedObjectName"]/*'/>*/
    public String PublishedObjectName => ForInstance(this.ItemInstanceToken).PublishedObjectName;

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="PublishedStreamName"]/*'/>*/
    public String PublishedStreamName => ForInstance(this.ItemInstanceToken).PublishedStreamName;

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="PublishedIntegrityName"]/*'/>*/
    public String PublishedIntegrityName => ForInstance(this.ItemInstanceToken).PublishedIntegrityName;

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/property[@name="ObjectName"]/*'/>*/
    public String ObjectName => String.Concat(this.Prefix,"/object");

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="ForInstance"]/*'/>*/
    public static TransferInstanceStoragePath ForInstance(Guid itemInstanceToken)
    {
        return TransferInstanceStoragePath.Create(itemInstanceToken);
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="Create"]/*'/>*/
    public static TransferStoragePath Create(Guid itemInstanceToken , Guid sessionid)
    {
        ValidateIdentity(itemInstanceToken,sessionid);

        return new() { ItemInstanceToken = itemInstanceToken , SessionID = sessionid };
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="CreateRangeName"]/*'/>*/
    public String CreateRangeName(DataItemTransferRange range)
    {
        ArgumentNullException.ThrowIfNull(range);

        if(!range.Validate()) { throw new ArgumentException("A valid transfer range is required.",nameof(range)); }

        return String.Concat(this.Prefix,"/ranges/",range.Offset.ToString(CultureInfo.InvariantCulture),"-",range.Length.ToString(CultureInfo.InvariantCulture));
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="CreateChunkName"]/*'/>*/
    public String CreateChunkName(Guid operationid)
    {
        ValidateOperationID(operationid);

        return String.Concat(this.Prefix,"/chunks/",operationid.ToString("N",CultureInfo.InvariantCulture));
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="CreatePublicationObjectName"]/*'/>*/
    public String CreatePublicationObjectName(Guid operationid)
    {
        ValidateOperationID(operationid);

        return String.Concat(this.Prefix,"/publications/",operationid.ToString("N",CultureInfo.InvariantCulture),"/object");
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="CreatePrefix"]/*'/>*/
    private static String CreatePrefix(Guid itemInstanceToken , Guid sessionid)
    {
        ValidateIdentity(itemInstanceToken,sessionid);

        return String.Concat("instances/",itemInstanceToken.ToString("N",CultureInfo.InvariantCulture),"/transfers/",sessionid.ToString("N",CultureInfo.InvariantCulture));
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="ValidateIdentity"]/*'/>*/
    private static void ValidateIdentity(Guid itemInstanceToken , Guid sessionid)
    {
        if(itemInstanceToken == Guid.Empty) { throw new ArgumentException("An item instance token is required.",nameof(itemInstanceToken)); }

        if(sessionid == Guid.Empty) { throw new ArgumentException("A transfer session identifier is required.",nameof(sessionid)); }
    }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferStoragePath"]/method[@name="ValidateOperationID"]/*'/>*/
    private static void ValidateOperationID(Guid operationid)
    {
        if(operationid == Guid.Empty) { throw new ArgumentException("A transfer operation identifier is required.",nameof(operationid)); }
    }
}

/**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/main/*'/>*/
internal sealed record TransferInstanceStoragePath
{
    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/property[@name="ItemInstanceToken"]/*'/>*/
    public Guid ItemInstanceToken { get; init; }

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/property[@name="Prefix"]/*'/>*/
    public String Prefix => String.Concat("instances/",this.ItemInstanceToken.ToString("N",CultureInfo.InvariantCulture),"/");

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/property[@name="PublishedObjectName"]/*'/>*/
    public String PublishedObjectName => String.Concat(this.Prefix,"published/object");

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/property[@name="PublishedStreamName"]/*'/>*/
    public String PublishedStreamName => String.Concat(this.Prefix,"published/stream");

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/property[@name="PublishedIntegrityName"]/*'/>*/
    public String PublishedIntegrityName => String.Concat(this.Prefix,"published/integrity");

    /**<include file='TransferStoragePath.xml' path='TransferStoragePath/record[@name="TransferInstanceStoragePath"]/method[@name="Create"]/*'/>*/
    internal static TransferInstanceStoragePath Create(Guid itemInstanceToken)
    {
        if(itemInstanceToken == Guid.Empty) { throw new ArgumentException("An item instance token is required.",nameof(itemInstanceToken)); }

        return new() { ItemInstanceToken = itemInstanceToken };
    }
}
