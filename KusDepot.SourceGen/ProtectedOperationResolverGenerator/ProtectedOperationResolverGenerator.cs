namespace KusDepot;

/**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/main/*'/>*/
[Generator(LanguageNames.CSharp)]
public class ProtectedOperationResolverGenerator : IIncrementalGenerator
{
    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/method[@name="Execute"]/*'/>*/
    private static void Execute(Compilation compilation , ImmutableArray<MethodDeclarationSyntax?> methods , SourceProductionContext context)
    {
        if(methods.IsDefaultOrEmpty) { return; }

        var distinctMethods = methods.Distinct();

        var methodInfos = GetMethodInfos(compilation,distinctMethods,context.CancellationToken);

        String source = GenerateResolver(methodInfos); context.AddSource("ProtectedOperationResolver.g.cs",SourceText.From(source,Encoding.UTF8));
    }

    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/method[@name="GenerateResolver"]/*'/>*/
    private static String GenerateResolver(List<(String Name , Int32 Index)> methodInfos)
    {
        var builder = new StringBuilder();

        builder.AppendLine("namespace KusDepot;");
        builder.AppendLine();
        builder.AppendLine("internal static partial class ProtectedOperationResolver");
        builder.AppendLine("{");
        builder.AppendLine("    public static partial Int32 ResolveIndex(ReadOnlySpan<Char> operationname) => operationname switch");
        builder.AppendLine("    {");

        foreach(var (name,index) in methodInfos.OrderBy(i => i.Name))
        {
            builder.AppendLine($"        \"{name}\" => {index},");
        }

        builder.AppendLine("        _ => -1");
        builder.AppendLine("    };");
        builder.AppendLine("}");

        return builder.ToString();
    }

    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/method[@name="GetMethodInfos"]/*'/>*/
    private static List<(String,Int32)> GetMethodInfos(Compilation compilation , IEnumerable<MethodDeclarationSyntax?> methods , CancellationToken cancel)
    {
        var methodInfos = new List<(String,Int32)>();

        foreach(var method in methods)
        {
            if(method is null) { continue; }

            cancel.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);

            if(semanticModel.GetDeclaredSymbol(method,cancellationToken:cancel) is IMethodSymbol methodSymbol)
            {
                var attribute = methodSymbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == AttributeFullName);

                if(attribute is not null && attribute.ConstructorArguments.Length == 1)
                {
                    var index = (Int32)attribute.ConstructorArguments[0].Value!;

                    methodInfos.Add((methodSymbol.Name,index));
                }
            }
        }

        return methodInfos;
    }

    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/property[@name="AttributeFullName"]/*'/>*/
    private const String AttributeFullName = "KusDepot.AccessCheckAttribute";

    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/method[@name="GetSemanticTargetForGeneration"]/*'/>*/
    private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

        foreach(var attributeListSyntax in methodDeclarationSyntax.AttributeLists)
        {
            foreach(var attributeSyntax in attributeListSyntax.Attributes)
            {
                if(context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is IMethodSymbol attributeSymbol)
                {
                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;

                    String fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if(fullName == AttributeFullName)
                    {
                        return methodDeclarationSyntax;
                    }
                }
            }
        }

        return null;
    }

    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/method[@name="Initialize"]/*'/>*/
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s,_) => IsSyntaxForGeneration(s),
                transform: static (ctx,_) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndMethods = context.CompilationProvider.Combine(methodDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndMethods,static (spc,source) => Execute(source.Left,source.Right,spc));
    }

    /**<include file='ProtectedOperationResolverGenerator.xml' path='ProtectedOperationResolverGenerator/class[@name="ProtectedOperationResolverGenerator"]/method[@name="IsSyntaxForGeneration"]/*'/>*/
    private static Boolean IsSyntaxForGeneration(SyntaxNode node)
    {
        return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };
    }
}