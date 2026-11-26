namespace KusDepot.Security.Assertions;

/**<include file='IAsyncAccessKeyAssertionGenerator.xml' path='IAsyncAccessKeyAssertionGenerator/interface[@name="IAsyncAccessKeyAssertionGenerator"]/main/*'/>*/
public interface IAsyncAccessKeyAssertionGenerator : IAccessKeyAssertionGeneratorMode
{
    /**<include file='IAsyncAccessKeyAssertionGenerator.xml' path='IAsyncAccessKeyAssertionGenerator/interface[@name="IAsyncAccessKeyAssertionGenerator"]/method[@name="GenerateAsync"]/*'/>*/
    ValueTask<ImmutableArray<AccessKeyAssertion>> GenerateAsync(AccessKeyAssertionGeneratorContext context , CancellationToken cancel = default);
}