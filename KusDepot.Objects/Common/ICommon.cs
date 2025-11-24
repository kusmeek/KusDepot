namespace KusDepot;

/**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/main/*'/>*/
public interface ICommon : IComparable<ICommon> , IEquatable<ICommon>
{
    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="CheckManager"]/*'/>*/
    Boolean CheckManager(ManagementKey? managementkey);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="CreateManagementKey"]/*'/>*/
    ManagementKey? CreateManagementKey(String? subject);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="CreateManagementKeyCertificate"]/*'/>*/
    ManagementKey? CreateManagementKey(X509Certificate2? certificate);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="DisableMyExceptions"]/*'/>*/
    Boolean DisableMyExceptions();

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="EnableMyExceptions"]/*'/>*/
    Boolean EnableMyExceptions();

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="GetID"]/*'/>*/
    Guid? GetID();

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="GetLocked"]/*'/>*/
    Boolean GetLocked();

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="Initialize"]/*'/>*/
    Boolean Initialize();

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="Lock"]/*'/>*/
    Boolean Lock(ManagementKey? managementkey);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="MyExceptionsEnabled"]/*'/>*/
    Boolean MyExceptionsEnabled();

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="RegisterManager"]/*'/>*/
    Boolean RegisterManager(ManagementKey? managementkey);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="RegisterManagerCertificate"]/*'/>*/
    ManagementKey? RegisterManager(X509Certificate2? certificate);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="SetID"]/*'/>*/
    Boolean SetID(Guid? id);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="UnLock"]/*'/>*/
    Boolean UnLock(ManagementKey? managementkey);

    /**<include file='ICommon.xml' path='ICommon/interface[@name="ICommon"]/method[@name="UnRegisterManager"]/*'/>*/
    Boolean UnRegisterManager(ManagementKey? managementkey);
}