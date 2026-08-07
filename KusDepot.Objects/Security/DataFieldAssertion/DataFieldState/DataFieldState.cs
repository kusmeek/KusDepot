namespace KusDepot.Security;

/**<include file='DataFieldState.xml' path='DataFieldState/enum[@name="DataFieldState"]/main/*'/>*/
public enum DataFieldState
{
    /**<include file='DataFieldState.xml' path='DataFieldState/enum[@name="DataFieldState"]/field[@name="ClearContent"]/*'/>*/
    ClearContent = 0,

    /**<include file='DataFieldState.xml' path='DataFieldState/enum[@name="DataFieldState"]/field[@name="EncryptedContent"]/*'/>*/
    EncryptedContent = 1,

    /**<include file='DataFieldState.xml' path='DataFieldState/enum[@name="DataFieldState"]/field[@name="HashCode"]/*'/>*/
    HashCode = 2,

    /**<include file='DataFieldState.xml' path='DataFieldState/enum[@name="DataFieldState"]/field[@name="MetaData"]/*'/>*/
    MetaData = 3
}
