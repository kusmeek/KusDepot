namespace KusDepot;

/**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/main/*'/>*/
public interface IDataItem : IComparable<IDataItem> , IEquatable<IDataItem> , IMetaBase
{
    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="CheckDataHash"]/*'/>*/
    Task<Boolean> CheckDataHash(CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ClearEncryptedData"]/*'/>*/
    Boolean ClearEncryptedData(ManagementKey? managementkey);

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

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetDataMasked"]/*'/>*/
    Boolean GetDataMasked();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="GetType"]/*'/>*/
    String? GetType();

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="MaskData"]/*'/>*/
    Boolean MaskData(Boolean mask , ManagementKey? managementkey);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SetContentStreamed"]/*'/>*/
    Boolean SetContentStreamed(Boolean streamed , ManagementKey? managementkey);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SetType"]/*'/>*/
    Boolean SetType(String? type);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="SignData"]/*'/>*/
    String? SignData(String? field , ManagementKey? managementkey);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ToFile"]/*'/>*/
    Boolean ToFile(String path);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="VerifyData"]/*'/>*/
    Task<Boolean> VerifyData(String? field , ManagementKey? managementkey , CancellationToken cancel = default);

    /**<include file='IDataItem.xml' path='IDataItem/interface[@name="IDataItem"]/method[@name="ZeroData"]/*'/>*/
    Task<Boolean> ZeroData(ManagementKey? managementkey = null , CancellationToken cancel = default);
}