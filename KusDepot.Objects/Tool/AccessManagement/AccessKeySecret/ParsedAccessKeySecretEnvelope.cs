namespace KusDepot.Security;

/**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/main/*'/>*/
internal readonly struct ParsedAccessKeySecretEnvelope
{
    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="Algorithm"]/*'/>*/
    public required AccessKeySignatureAlgorithm Algorithm { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="CiphertextLength"]/*'/>*/
    public required Int32 CiphertextLength { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="CiphertextMemory"]/*'/>*/
    public required ReadOnlyMemory<Byte> CiphertextMemory { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="Flags"]/*'/>*/
    public required Byte Flags { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="NonceMemory"]/*'/>*/
    public required ReadOnlyMemory<Byte> NonceMemory { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignatureLength"]/*'/>*/
    public required Int32 SignatureLength { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignatureMemory"]/*'/>*/
    public required ReadOnlyMemory<Byte> SignatureMemory { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignerKeyIDFormat"]/*'/>*/
    public required AccessKeySignerKeyIdFormat SignerKeyIDFormat { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignerKeyIDMemory"]/*'/>*/
    public required ReadOnlyMemory<Byte> SignerKeyIDMemory { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignerAccessManagerID"]/*'/>*/
    public required Guid SignerAccessManagerID { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignerToolID"]/*'/>*/
    public required Guid SignerToolID { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="SignedRegionMemory"]/*'/>*/
    public required ReadOnlyMemory<Byte> SignedRegionMemory { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="TagMemory"]/*'/>*/
    public required ReadOnlyMemory<Byte> TagMemory { get; init; }

    /**<include file='ParsedAccessKeySecretEnvelope.xml' path='ParsedAccessKeySecretEnvelope/struct[@name="ParsedAccessKeySecretEnvelope"]/property[@name="Signed"]/*'/>*/
    public Boolean Signed => (Flags & AccessKeySecret.SignedFlag) == AccessKeySecret.SignedFlag;
}
