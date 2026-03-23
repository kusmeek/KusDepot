namespace KusDepot;

/**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/main/*'/>*/
public interface IDataItem : IComparable<IDataItem> , IEquatable<IDataItem> , IMetaBase
{
    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="CheckDataHash"]/*'/>*/
    Task<Boolean> CheckDataHash(CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="DecryptData"]/*'/>*/
    Task<Boolean> DecryptData(ManagementKey? managementkey , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="EncryptData"]/*'/>*/
    Task<Boolean> EncryptData(ManagementKey? managementkey , Boolean sign = true , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetContentStream"]/*'/>*/
    Stream? GetContentStream();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetContentStreamed"]/*'/>*/
    Boolean GetContentStreamed();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataContent"]/*'/>*/
    Task<DataContent?> GetDataContent(ManagementKey? managementkey = null , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataEncrypted"]/*'/>*/
    Boolean GetDataEncrypted();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataEncryptInfo"]/*'/>*/
    DataEncryptInfo? GetDataEncryptInfo();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataType"]/*'/>*/
    String? GetDataType();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SetContentStreamed"]/*'/>*/
    Task<Boolean> SetContentStreamed(Boolean streamed , ManagementKey? managementkey , CancellationToken cancel = default );

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SetDataType"]/*'/>*/
    Boolean SetDataType(String? type);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SignData"]/*'/>*/
    Task<String?> SignData(String? field , ManagementKey? managementkey , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ToFile"]/*'/>*/
    Boolean ToFile(String path);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="VerifyData"]/*'/>*/
    Task<Boolean> VerifyData(String? field , ManagementKey? managementkey , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="WipeData"]/*'/>*/
    Task<Boolean> WipeData(ManagementKey? managementkey = null , CancellationToken cancel = default);
}