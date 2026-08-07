namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignerKeyIdFormat.xml' path='AccessKeySignerKeyIdFormat/enum[@name="AccessKeySignerKeyIdFormat"]/main/*'/>*/
public enum AccessKeySignerKeyIdFormat : Byte
{
    /**<include file='AccessKeySignerKeyIdFormat.xml' path='AccessKeySignerKeyIdFormat/enum[@name="AccessKeySignerKeyIdFormat"]/value[@name="None"]/*'/>*/
    None = 0,

    /**<include file='AccessKeySignerKeyIdFormat.xml' path='AccessKeySignerKeyIdFormat/enum[@name="AccessKeySignerKeyIdFormat"]/value[@name="Thumbprint"]/*'/>*/
    Thumbprint = 1
}