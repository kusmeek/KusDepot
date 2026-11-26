namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureAlgorithm.xml' path='AccessKeySignatureAlgorithm/enum[@name="AccessKeySignatureAlgorithm"]/main/*'/>*/
public enum AccessKeySignatureAlgorithm : Byte
{
    /**<include file='AccessKeySignatureAlgorithm.xml' path='AccessKeySignatureAlgorithm/enum[@name="AccessKeySignatureAlgorithm"]/value[@name="None"]/*'/>*/
    None = 0,

    /**<include file='AccessKeySignatureAlgorithm.xml' path='AccessKeySignatureAlgorithm/enum[@name="AccessKeySignatureAlgorithm"]/value[@name="RsaPssSha512"]/*'/>*/
    RsaPssSha512 = 1
}