using Metalama.Framework.Aspects;

namespace KusDepot.Meta;

[RunTimeOrCompileTime]
public sealed class CommandDetailsTransformer : ContractAspect
{
    public override void Validate(dynamic? value) { }
}