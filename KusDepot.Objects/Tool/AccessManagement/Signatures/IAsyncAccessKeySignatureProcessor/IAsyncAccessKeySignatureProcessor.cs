namespace KusDepot.Security.Signatures;

/**<include file='IAsyncAccessKeySignatureProcessor.xml' path='IAsyncAccessKeySignatureProcessor/interface[@name="IAsyncAccessKeySignatureProcessor"]/main/*'/>*/
public interface IAsyncAccessKeySignatureProcessor : IAccessKeySignatureProcessorMode
{
    /**<include file='IAsyncAccessKeySignatureProcessor.xml' path='IAsyncAccessKeySignatureProcessor/interface[@name="IAsyncAccessKeySignatureProcessor"]/method[@name="VerifyAsync"]/*'/>*/
    ValueTask<SignatureValidation> VerifyAsync(AccessKeySignatureValidationContext context , CancellationToken cancel = default);
}