namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateSignatureValidationAsync"]/*'/>*/
    private async Task<SignatureValidation?> CreateSignatureValidationAsync(ToolManifestIdentity manifestidentity , Byte[]? secret , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(manifestidentity);

        if(secret is null || secret.Length == 0) { return null; }

        if(!AccessKeySecret.TryReadSignature(secret,out AccessKeySignatureDescriptor signature)) { return null; }

        SignatureValidation defaultvalidation = signature.Signed ? CreateSignedUnverifiedSignatureValidation(signature) : CreateUnsignedSignatureValidation();

        if(!signature.Signed) { return defaultvalidation; }

        if(!AccessKeySecret.TryReadSignedRegion(secret,out ReadOnlyMemory<Byte> signedregion) || !AccessKeySecret.TryReadSignatureBytes(secret,out ReadOnlyMemory<Byte> signaturebytes))
        {
            return null;
        }

        cancel.ThrowIfCancellationRequested();

        AccessKeySignatureValidationContext context = CreateSignatureValidationContext(manifestidentity,signature,signedregion,signaturebytes,Logger);

        return this.SignatureProcessor switch
        {
            IAsyncAccessKeySignatureProcessor asynchronous => await asynchronous.VerifyAsync(context,cancel).ConfigureAwait(false),

            IAccessKeySignatureProcessor synchronous => synchronous.Verify(context),

            IAccessKeySigner => defaultvalidation,

            null => defaultvalidation,

            _ => throw new InvalidOperationException(ConfiguredSignatureProcessorModeInvalid)
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateSignedUnverifiedSignatureValidation"]/*'/>*/
    private static SignatureValidation CreateSignedUnverifiedSignatureValidation(AccessKeySignatureDescriptor signature)
    {
        return new()
        {
            Algorithm = signature.Algorithm,
            FailureReason = "Signature verification was not attempted.",
            Signed = true,
            SignerAccessManagerID = signature.SignerAccessManagerID,
            SignerKeyID = signature.SignerKeyID,
            SignerKeyIDFormat = signature.SignerKeyIDFormat,
            SignerToolID = signature.SignerToolID,
            VerificationAttempted = false
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateUnsignedSignatureValidation"]/*'/>*/
    private static SignatureValidation CreateUnsignedSignatureValidation()
    {
        return new()
        {
            Signed = false,
            VerificationAttempted = false
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateSignatureValidationContext"]/*'/>*/
    private static AccessKeySignatureValidationContext CreateSignatureValidationContext(ToolManifestIdentity manifestidentity , AccessKeySignatureDescriptor signature , ReadOnlyMemory<Byte> signedregion , ReadOnlyMemory<Byte> signaturebytes , ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(manifestidentity);

        return new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            Logger = logger ?? NullLogger.Instance,
            ManifestHash = manifestidentity.ManifestHash,
            Signature = signature,
            SignatureBytes = signaturebytes,
            SignedRegion = signedregion,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GetSynchronousSignatureProcessor"]/*'/>*/
    private static IAccessKeySignatureProcessor? GetSynchronousSignatureProcessor(IAccessKeySignatureProcessorMode? processor)
    {
        if(processor is null) { return null; }

        if(processor is IAccessKeySignatureProcessor synchronous) { return synchronous; }

        if(processor is IAsyncAccessKeySignatureProcessor)
        {
            throw new InvalidOperationException(SynchronousAccessManagerRequiresSynchronousSignatureProcessor);
        }

        if(processor is IAccessKeySigner) { return null; }

        throw new InvalidOperationException(ConfiguredSignatureProcessorModeInvalid);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetSignatureSigner"]/*'/>*/
    private static IAccessKeySigner? TryGetSignatureSigner(IAccessKeySignatureProcessorMode? processor)
    {
        if(processor is null) { return null; }

        if(processor is IAccessKeySigner signer) { return signer; }

        if(processor is IAsyncAccessKeySignatureProcessor) { return null; }

        throw new InvalidOperationException(ConfiguredSignatureProcessorModeInvalid);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetSignatureValidation"]/*'/>*/
    private Boolean TryGetSignatureValidation(ToolManifestIdentity manifestidentity , Byte[]? secret , out SignatureValidation signaturevalidation)
    {
        signaturevalidation = default;

        ArgumentNullException.ThrowIfNull(manifestidentity);

        if(secret is null || secret.Length == 0) { return false; }

        if(!AccessKeySecret.TryReadSignature(secret,out AccessKeySignatureDescriptor signature)) { return false; }

        signaturevalidation = signature.Signed ? CreateSignedUnverifiedSignatureValidation(signature) : CreateUnsignedSignatureValidation();

        if(!signature.Signed) { return true; }

        IAccessKeySignatureProcessor? processor = GetSynchronousSignatureProcessor(this.SignatureProcessor); if(processor is null) { return true; }

        if(!AccessKeySecret.TryReadSignedRegion(secret,out ReadOnlyMemory<Byte> signedregion) || !AccessKeySecret.TryReadSignatureBytes(secret,out ReadOnlyMemory<Byte> signaturebytes))
        {
            return false;
        }

        AccessKeySignatureValidationContext context = CreateSignatureValidationContext(manifestidentity,signature,signedregion,signaturebytes,Logger);

        signaturevalidation = processor.Verify(context);

        return true;
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="ValidateSignatureProcessorMode"]/*'/>*/
    private static void ValidateSignatureProcessorMode(IAccessKeySignatureProcessorMode? processor)
    {
        if(processor is null) { return; }

        if(processor is IAccessKeySigner or IAccessKeySignatureProcessor or IAsyncAccessKeySignatureProcessor) { return; }

        throw new InvalidOperationException(ConfiguredSignatureProcessorModeInvalid);
    }
}