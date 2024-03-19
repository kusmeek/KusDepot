namespace KusDepot.Data;

/**<include file='IDataControl.xml' path='IDataControl/interface[@name="IDataControl"]/main/*'/>*/
internal interface IDataControl
{
    /**<include file='IDataControl.xml' path='IDataControl/interface[@name="IDataControl"]/method[@name="Delete"]/*'/>*/
    internal String Delete(String? id);

    /**<include file='IDataControl.xml' path='IDataControl/interface[@name="IDataControl"]/method[@name="Get"]/*'/>*/
    internal String Get(String? id);

    /**<include file='IDataControl.xml' path='IDataControl/interface[@name="IDataControl"]/method[@name="Store"]/*'/>*/
    internal String Store(DataControlUpload? dcu);
}