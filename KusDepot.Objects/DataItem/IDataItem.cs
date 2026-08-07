namespace KusDepot;

/**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/main/*'/>*/
public interface IDataItem : IComparable<IDataItem> , IEquatable<IDataItem> , IMetaBase
{
    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="CheckDataHash"]/*'/>*/
    Task<Boolean> CheckDataHash(CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="DecryptData"]/*'/>*/
    Task<Boolean> DecryptData(DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="EncryptData"]/*'/>*/
    Task<Boolean> EncryptData(DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetContentStream"]/*'/>*/
    Stream? GetContentStream();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetContentStreamed"]/*'/>*/
    Boolean GetContentStreamed();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataContent"]/*'/>*/
    Task<DataContent?> GetDataContent(DataItemSecurityContext? security = null , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataEncrypted"]/*'/>*/
    Boolean GetDataEncrypted();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataProtectionInfo"]/*'/>*/
    DataProtectionInfo? GetDataProtectionInfo();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataType"]/*'/>*/
    String? GetDataType();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="IsMetadataOnly"]/*'/>*/
    Boolean IsMetadataOnly();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ProtectData"]/*'/>*/
    Task<Boolean> ProtectData(DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SetContentStreamed"]/*'/>*/
    Task<Boolean> SetContentStreamed(Boolean streamed , DataItemSecurityContext? security , CancellationToken cancel = default );

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="Serialize"]/*'/>*/
    Byte[] Serialize();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SetDataType"]/*'/>*/
    Boolean SetDataType(String? type);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SignData"]/*'/>*/
    Task<String?> SignData(String? field , DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ToFile"]/*'/>*/
    Boolean ToFile(String path);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="UnProtectData"]/*'/>*/
    Task<Boolean> UnProtectData(DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ValidateData"]/*'/>*/
    Task<Boolean> ValidateData(DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="VerifyData"]/*'/>*/
    Task<Boolean> VerifyData(String? field , DataItemSecurityContext? security , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="WipeData"]/*'/>*/
    Task<Boolean> WipeData(DataItemSecurityContext? security = null , CancellationToken cancel = default);
}