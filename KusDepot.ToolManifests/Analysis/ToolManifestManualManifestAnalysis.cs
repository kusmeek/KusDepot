namespace KusDepot.ToolManifests.Analysis;

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/main/*'/>*/
internal static class ToolManifestManualManifestAnalysis
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="GetMethodDeclarationSyntax"]/*'/>*/
    private static MethodDeclarationSyntax? GetMethodDeclarationSyntax(IMethodSymbol method)
    {
        return method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="GetReturnExpression"]/*'/>*/
    private static ExpressionSyntax? GetReturnExpression(MethodDeclarationSyntax methodDeclaration)
    {
        if(methodDeclaration.ExpressionBody is not null) { return methodDeclaration.ExpressionBody.Expression; }
        if(methodDeclaration.Body is null || methodDeclaration.Body.Statements.Count != 1) { return null; }

        return methodDeclaration.Body.Statements[0] is ReturnStatementSyntax returnStatement ? returnStatement.Expression : null;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="TryGetInvocationArgumentExpression"]/*'/>*/
    private static Boolean TryGetInvocationArgumentExpression(InvocationExpressionSyntax invocation , IMethodSymbol method , String parameterName , out ExpressionSyntax expression)
    {
        expression = null!;

        for(Int32 index = 0; index < invocation.ArgumentList.Arguments.Count; index++)
        {
            ArgumentSyntax argument = invocation.ArgumentList.Arguments[index];
            String? argumentName = argument.NameColon?.Name.Identifier.ValueText;

            if(StringComparer.Ordinal.Equals(argumentName,parameterName))
            {
                expression = argument.Expression;

                return true;
            }

            if(argumentName is not null) { continue; }
            if(index >= method.Parameters.Length) { continue; }
            if(StringComparer.Ordinal.Equals(method.Parameters[index].Name,parameterName) is false) { continue; }

            expression = argument.Expression;

            return true;
        }

        return false;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="TryGetToolManifestCreateInvocation"]/*'/>*/
    private static Boolean TryGetToolManifestCreateInvocation(ExpressionSyntax expression , SemanticModel semanticModel , out InvocationExpressionSyntax createInvocation , out IMethodSymbol createMethod)
    {
        createInvocation = null!;
        createMethod = null!;

        if(expression is InvocationExpressionSyntax invocation
            && semanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol invocationMethod)
        {
            if(StringComparer.Ordinal.Equals(invocationMethod.Name,"Create")
                && StringComparer.Ordinal.Equals(invocationMethod.ContainingType.ToDisplayString(),"KusDepot.ToolManifest")
                && invocationMethod.Parameters.Length == 5)
            {
                createInvocation = invocation;
                createMethod = invocationMethod;

                return true;
            }

            if(StringComparer.Ordinal.Equals(invocationMethod.Name,"ComputeManifestHash")
                && invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                return TryGetToolManifestCreateInvocation(memberAccess.Expression,semanticModel,out createInvocation,out createMethod);
            }
        }

        return false;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="TryExtractHandwrittenManifestDescriptor"]/*'/>*/
    private static Boolean TryExtractHandwrittenManifestDescriptor(ExpressionSyntax expression , SemanticModel semanticModel , out HandwrittenManifestDescriptor descriptor)
    {
        descriptor = null!;

        InitializerExpressionSyntax? initializer = expression switch
        {
            ObjectCreationExpressionSyntax objectCreation => objectCreation.Initializer,
            ImplicitObjectCreationExpressionSyntax implicitObjectCreation => implicitObjectCreation.Initializer,
            _ => null
        };

        if(initializer is null) { return false; }

        String? methodName = null;
        Int32? index = null;
        Location? methodNameLocation = null;
        Location? indexLocation = null;

        foreach(ExpressionSyntax initializerExpression in initializer.Expressions)
        {
            if(initializerExpression is not AssignmentExpressionSyntax assignment) { continue; }
            if(assignment.Left is not IdentifierNameSyntax identifierName) { continue; }

            if(StringComparer.Ordinal.Equals(identifierName.Identifier.ValueText,"MethodName")
                && semanticModel.GetConstantValue(assignment.Right).Value is String configuredMethodName)
            {
                methodName = configuredMethodName;
                methodNameLocation = assignment.Right.GetLocation();
            }
            else if(StringComparer.Ordinal.Equals(identifierName.Identifier.ValueText,"Index")
                && semanticModel.GetConstantValue(assignment.Right).Value is Int32 configuredIndex)
            {
                index = configuredIndex;
                indexLocation = assignment.Right.GetLocation();
            }
        }

        if(String.IsNullOrWhiteSpace(methodName) || index is null) { return false; }

        descriptor = new(methodName!,index.Value,methodNameLocation,indexLocation);

        return true;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="TryExtractHandwrittenManifestDescriptorsFromCreateInvocation"]/*'/>*/
    private static Boolean TryExtractHandwrittenManifestDescriptorsFromCreateInvocation(InvocationExpressionSyntax createInvocation , IMethodSymbol createMethod , SemanticModel semanticModel , out ImmutableArray<HandwrittenManifestDescriptor> descriptors)
    {
        descriptors = [];

        if(TryGetInvocationArgumentExpression(createInvocation,createMethod,"operations",out ExpressionSyntax operationsExpression) is false) { return false; }
        if(operationsExpression is not CollectionExpressionSyntax collectionExpression) { return false; }

        ImmutableArray<HandwrittenManifestDescriptor>.Builder builder = ImmutableArray.CreateBuilder<HandwrittenManifestDescriptor>();

        foreach(CollectionElementSyntax element in collectionExpression.Elements)
        {
            if(element is not ExpressionElementSyntax expressionElement) { return false; }

            if(TryExtractHandwrittenManifestDescriptor(expressionElement.Expression,semanticModel,out HandwrittenManifestDescriptor descriptor) is false) { return false; }

            builder.Add(descriptor);
        }

        descriptors = builder.ToImmutable();

        return true;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="TryExtractHandwrittenManifestDescriptors"]/*'/>*/
    private static Boolean TryExtractHandwrittenManifestDescriptors(Compilation compilation , IMethodSymbol manifestMethod , ISet<IMethodSymbol> visitedMethods , out ImmutableArray<HandwrittenManifestDescriptor> descriptors)
    {
        descriptors = [];

        if(visitedMethods.Add(manifestMethod) is false) { return false; }

        MethodDeclarationSyntax? methodDeclaration = GetMethodDeclarationSyntax(manifestMethod);
        if(methodDeclaration is null) { return false; }

        SemanticModel semanticModel = compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
        ExpressionSyntax? returnExpression = GetReturnExpression(methodDeclaration);

        if(returnExpression is null) { return false; }

        if(TryGetToolManifestCreateInvocation(returnExpression,semanticModel,out InvocationExpressionSyntax createInvocation,out IMethodSymbol createMethod))
        {
            return TryExtractHandwrittenManifestDescriptorsFromCreateInvocation(createInvocation,createMethod,semanticModel,out descriptors);
        }

        if(returnExpression is not InvocationExpressionSyntax helperInvocation) { return false; }
        if(semanticModel.GetSymbolInfo(helperInvocation).Symbol is not IMethodSymbol helperMethod) { return false; }
        if(SymbolEqualityComparer.Default.Equals(helperMethod.ContainingAssembly,manifestMethod.ContainingAssembly) is false) { return false; }

        return TryExtractHandwrittenManifestDescriptors(compilation,helperMethod,visitedMethods,out descriptors);
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestManualManifestAnalysis"]/method[@name="ValidateHandwrittenManifestDescriptors"]/*'/>*/
    internal static void ValidateHandwrittenManifestDescriptors(Compilation compilation , INamedTypeSymbol toolType , IReadOnlyDictionary<String,MethodMetadata> methodsByName , ICollection<ToolManifestAnalysisDiagnostic> diagnostics)
    {
        IMethodSymbol? manifestMethod = toolType.GetMembers("GetToolManifest")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(static _ => _.IsStatic is false && _.IsOverride);

        if(manifestMethod is null) { return; }

        if(TryExtractHandwrittenManifestDescriptors(compilation,manifestMethod,new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default),out ImmutableArray<HandwrittenManifestDescriptor> descriptors) is false) { return; }

        foreach(HandwrittenManifestDescriptor descriptor in descriptors)
        {
            if(methodsByName.TryGetValue(descriptor.MethodName,out MethodMetadata? methodMetadata) is false)
            {
                diagnostics.Add(new(
                    ToolManifestDiagnostics.HandwrittenManifestMethodNameMismatchRule,
                    descriptor.MethodNameLocation ?? manifestMethod.Locations.FirstOrDefault(),
                    toolType.ToDisplayString(),
                    descriptor.MethodName));
                continue;
            }

            if(methodMetadata.Index == descriptor.Index) { continue; }

            diagnostics.Add(new(
                ToolManifestDiagnostics.HandwrittenManifestIndexMismatchRule,
                descriptor.IndexLocation ?? descriptor.MethodNameLocation ?? manifestMethod.Locations.FirstOrDefault(),
                toolType.ToDisplayString(),
                descriptor.MethodName,
                descriptor.Index,
                methodMetadata.Index));
        }
    }
}
