namespace KusDepot.Data;

/**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/main/*'/>*/
public class DataControlUpload
{
    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="Descriptor"]/*'/>*/
    public Descriptor Descriptor {get;init;} = new();

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="Object"]/*'/>*/
    public String Object         {get;init;} = String.Empty;

    /**<include file='DataControlUpload.xml' path='DataControlUpload/class[@name="DataControlUpload"]/property[@name="ObjectSHA512"]/*'/>*/
    public String ObjectSHA512   {get;init;} = String.Empty;
}