namespace KusDepot.Security.Assertions;

/**<include file='DefaultAccessKeyAssertionGenerator.xml' path='DefaultAccessKeyAssertionGenerator/class[@name="DefaultAccessKeyAssertionGenerator"]/main/*'/>*/
public sealed class DefaultAccessKeyAssertionGenerator : IAccessKeyAssertionGenerator , IAsyncAccessKeyAssertionGenerator
{
    /**<include file='DefaultAccessKeyAssertionGenerator.xml' path='DefaultAccessKeyAssertionGenerator/class[@name="DefaultAccessKeyAssertionGenerator"]/field[@name="Generators"]/*'/>*/
    private readonly ImmutableArray<IAccessKeyAssertionGeneratorMode> Generators;

    /**<include file='DefaultAccessKeyAssertionGenerator.xml' path='DefaultAccessKeyAssertionGenerator/class[@name="DefaultAccessKeyAssertionGenerator"]/constructor[@name="Constructor"]/*'/>*/
    public DefaultAccessKeyAssertionGenerator(IEnumerable<IAccessKeyAssertionGeneratorMode>? generators = null)
    {
        this.Generators = generators?.Where(_ => _ is not null && !ReferenceEquals(_,this)).ToImmutableArray() ?? [];
    }

    /**<include file='DefaultAccessKeyAssertionGenerator.xml' path='DefaultAccessKeyAssertionGenerator/class[@name="DefaultAccessKeyAssertionGenerator"]/method[@name="Generate"]/*'/>*/
    public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
    {
        if(this.Generators.IsDefaultOrEmpty) { return []; }

        var assertions = ImmutableArray.CreateBuilder<AccessKeyAssertion>();

        foreach(IAccessKeyAssertionGeneratorMode generator in this.Generators)
        {
            ArgumentNullException.ThrowIfNull(generator);

            ImmutableArray<AccessKeyAssertion> generated = generator switch
            {
                IAccessKeyAssertionGenerator synchronous => synchronous.Generate(in context),

                IAsyncAccessKeyAssertionGenerator => throw new InvalidOperationException(SynchronousAssertionGeneratorRequiresSynchronousChildGenerators),

                _ => throw new InvalidOperationException(ConfiguredAssertionGeneratorModeInvalid)
            };

            if(generated.IsDefaultOrEmpty) { continue; }

            foreach(AccessKeyAssertion? assertion in generated)
            {
                if(assertion is not null) { assertions.Add(assertion); }
            }
        }

        return assertions.ToImmutable();
    }

    /**<include file='DefaultAccessKeyAssertionGenerator.xml' path='DefaultAccessKeyAssertionGenerator/class[@name="DefaultAccessKeyAssertionGenerator"]/method[@name="GenerateAsync"]/*'/>*/
    public async ValueTask<ImmutableArray<AccessKeyAssertion>> GenerateAsync(AccessKeyAssertionGeneratorContext context , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        if(this.Generators.IsDefaultOrEmpty) { return []; }

        var assertions = ImmutableArray.CreateBuilder<AccessKeyAssertion>();

        foreach(IAccessKeyAssertionGeneratorMode generator in this.Generators)
        {
            ArgumentNullException.ThrowIfNull(generator);

            ImmutableArray<AccessKeyAssertion> generated = generator switch
            {
                IAsyncAccessKeyAssertionGenerator asynchronous => await asynchronous.GenerateAsync(context,cancel).ConfigureAwait(false),

                IAccessKeyAssertionGenerator synchronous => synchronous.Generate(in context),

                _ => throw new InvalidOperationException(ConfiguredAssertionGeneratorModeInvalid)
            };

            if(generated.IsDefaultOrEmpty) { continue; }

            foreach(AccessKeyAssertion? assertion in generated)
            {
                cancel.ThrowIfCancellationRequested();

                if(assertion is not null) { assertions.Add(assertion); }
            }
        }

        return assertions.ToImmutable();
    }
}