namespace KusDepot.Security.Assertions;

/**<include file='IAccessKeyAssertionGenerator.xml' path='IAccessKeyAssertionGenerator/interface[@name="IAccessKeyAssertionGenerator"]/main/*'/>*/
public interface IAccessKeyAssertionGenerator : IAccessKeyAssertionGeneratorMode
{
    /**<include file='IAccessKeyAssertionGenerator.xml' path='IAccessKeyAssertionGenerator/interface[@name="IAccessKeyAssertionGenerator"]/method[@name="Generate"]/*'/>*/
    ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context);
}