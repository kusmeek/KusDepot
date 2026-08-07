namespace KusDepot.Security.Assertions;

public abstract partial record class AccessKeyAssertion
{
    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/field[@name="EnvelopeVersion"]/*'/>*/
    private const Byte EnvelopeVersion = 0x01;

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/field[@name="PayloadLengthSize"]/*'/>*/
    private const Int32 PayloadLengthSize = 4;

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/field[@name="SerializationIdentifierLengthSize"]/*'/>*/
    private const Int32 SerializationIdentifierLengthSize = 2;
}