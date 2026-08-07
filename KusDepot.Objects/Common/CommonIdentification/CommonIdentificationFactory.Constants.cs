namespace KusDepot;

/**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/main/*'/>*/
public static partial class CommonIdentificationFactory
{
    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="DataVersion"]/*'/>*/
    public const Byte DataVersion = 0x01;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="EnvelopeVersion"]/*'/>*/
    internal const Byte EnvelopeVersion = 0x01;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="SignatureAlgorithmRsaSha512Pss"]/*'/>*/
    internal const Byte SignatureAlgorithmRsaSha512Pss = 0x01;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="ByteSize"]/*'/>*/
    private const Int32 ByteSize = 1;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="GuidSize"]/*'/>*/
    private const Int32 GuidSize = 16;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="Int64Size"]/*'/>*/
    private const Int32 Int64Size = 8;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="UInt16Size"]/*'/>*/
    private const Int32 UInt16Size = 2;

    /**<include file='CommonIdentificationFactory.xml' path='CommonIdentificationFactory/class[@name="CommonIdentificationFactory"]/field[@name="UInt32Size"]/*'/>*/
    private const Int32 UInt32Size = 4;
}