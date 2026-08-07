namespace KusDepot.ToolManifests.Analysis;

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/main/*'/>*/
internal static class ToolManifestSymbolAnalysis
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="CreateMethodMetadata"]/*'/>*/
    internal static MethodMetadata? CreateMethodMetadata(IMethodSymbol method)
    {
        AttributeData? accessCheckAttribute = method.GetAttributes().FirstOrDefault(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.AccessCheckAttributeFullName);

        if(accessCheckAttribute is null || accessCheckAttribute.ConstructorArguments.Length != 1 || accessCheckAttribute.ConstructorArguments[0].Value is not Int32 index) { return null; }

        String description = method.Name;

        AttributeData? toolOperationAttribute = method.GetAttributes().FirstOrDefault(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.ToolOperationAttributeFullName);

        if(toolOperationAttribute is not null)
        {
            foreach(KeyValuePair<String,TypedConstant> argument in toolOperationAttribute.NamedArguments)
            {
                if(argument.Key == "Description" && argument.Value.Value is String configuredDescription && String.IsNullOrWhiteSpace(configuredDescription) is false)
                {
                    description = configuredDescription;

                    break;
                }
            }
        }

        return new(method.Name,index,description);
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="IsAsyncOperationMethod"]/*'/>*/
    internal static Boolean IsAsyncOperationMethod(IMethodSymbol method)
    {
        if(method.Name.EndsWith("Async",StringComparison.Ordinal) is false) { return false; }

        if(method.ReturnType is not INamedTypeSymbol returnType) { return false; }

        return returnType.ContainingNamespace?.ToDisplayString() == "System.Threading.Tasks"
            && (returnType.Name == "Task" || returnType.Name == "ValueTask");
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="CreateToolManifestMetadata"]/*'/>*/
    internal static ToolManifestMetadata? CreateToolManifestMetadata(INamedTypeSymbol toolType , ICollection<ToolManifestAnalysisDiagnostic> diagnostics)
    {
        AttributeData? toolManifestAttribute = toolType.GetAttributes().FirstOrDefault(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.ToolManifestAttributeFullName);

        if(toolManifestAttribute is null) { return null; }

        String? toolSchemaID = null;
        String? accessKeyRealmID = null;
        String toolSchemaVersion = String.Empty;
        String? companionNamespace = null;
        String? companionTypeName = null;

        foreach(KeyValuePair<String,TypedConstant> argument in toolManifestAttribute.NamedArguments)
        {
            if(argument.Key == "ToolSchemaID") { toolSchemaID = argument.Value.Value as String; }
            else if(argument.Key == "AccessKeyRealmID") { accessKeyRealmID = argument.Value.Value as String; }
            else if(argument.Key == "ToolSchemaVersion") { toolSchemaVersion = argument.Value.Value as String ?? String.Empty; }
            else if(argument.Key == "CompanionNamespace") { companionNamespace = argument.Value.Value as String; }
            else if(argument.Key == "CompanionTypeName") { companionTypeName = argument.Value.Value as String; }
        }

        if(String.IsNullOrWhiteSpace(toolSchemaID))
        {
            diagnostics.Add(new(ToolManifestDiagnostics.MissingToolSchemaIDRule,toolType.Locations.FirstOrDefault(),toolType.ToDisplayString()));

            return null;
        }

        if(companionNamespace is not null && IsValidNamespace(companionNamespace) is false)
        {
            diagnostics.Add(new(ToolManifestDiagnostics.InvalidCompanionNamespaceRule,toolType.Locations.FirstOrDefault(),toolType.ToDisplayString(),companionNamespace));

            return null;
        }

        if(companionTypeName is not null && IsValidIdentifier(companionTypeName) is false)
        {
            diagnostics.Add(new(ToolManifestDiagnostics.InvalidCompanionTypeNameRule,toolType.Locations.FirstOrDefault(),toolType.ToDisplayString(),companionTypeName));

            return null;
        }

        return new(toolSchemaID!,accessKeyRealmID,toolSchemaVersion,companionNamespace,companionTypeName);
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="CreateAssemblyManifestMetadata"]/*'/>*/
    internal static AssemblyManifestMetadata CreateAssemblyManifestMetadata(IAssemblySymbol assembly , ICollection<ToolManifestAnalysisDiagnostic> diagnostics)
    {
        AttributeData? attribute = assembly.GetAttributes().FirstOrDefault(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.ToolManifestAssemblyAttributeFullName);

        if(attribute is null) { return new(false,null,null); }

        Boolean registerModuleInitializer = false;
        String? companionNamespace = null;
        String? companionTypeName = null;

        foreach(KeyValuePair<String,TypedConstant> argument in attribute.NamedArguments)
        {
            if(argument.Key == "RegisterModuleInitializer" && argument.Value.Value is Boolean configuredRegisterModuleInitializer)
            {
                registerModuleInitializer = configuredRegisterModuleInitializer;
            }
            else if(argument.Key == "CompanionNamespace")
            {
                companionNamespace = argument.Value.Value as String;
            }
            else if(argument.Key == "CompanionTypeName")
            {
                companionTypeName = argument.Value.Value as String;
            }
        }

        if(companionNamespace is not null && IsValidNamespace(companionNamespace) is false)
        {
            diagnostics.Add(new(ToolManifestDiagnostics.InvalidAssemblyCompanionNamespaceRule, attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(), companionNamespace));
        }

        return new(registerModuleInitializer,companionNamespace,companionTypeName);
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="IsValidNamespace"]/*'/>*/
    private static Boolean IsValidNamespace(String value)
    {
        if(String.IsNullOrWhiteSpace(value)) { return false; }

        foreach(String segment in value.Split('.'))
        {
            if(String.IsNullOrWhiteSpace(segment)) { return false; }

            if(SyntaxFacts.IsIdentifierStartCharacter(segment[0]) is false) { return false; }

            for(Int32 index = 1; index < segment.Length; index++)
            {
                if(SyntaxFacts.IsIdentifierPartCharacter(segment[index]) is false) { return false; }
            }
        }

        return true;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="IsValidIdentifier"]/*'/>*/
    private static Boolean IsValidIdentifier(String value)
    {
        if(String.IsNullOrWhiteSpace(value)) { return false; }

        if(SyntaxFacts.IsIdentifierStartCharacter(value[0]) is false) { return false; }

        for(Int32 index = 1; index < value.Length; index++)
        {
            if(SyntaxFacts.IsIdentifierPartCharacter(value[index]) is false) { return false; }
        }

        return true;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="GetToolTypeForGeneration"]/*'/>*/
    internal static INamedTypeSymbol? GetToolTypeForGeneration(GeneratorSyntaxContext context)
    {
        if(context.Node is not ClassDeclarationSyntax classDeclaration) { return null; }

        if(context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol toolType) { return null; }

        return toolType.GetAttributes().Any(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.ToolManifestAttributeFullName) ? toolType : null;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="GetToolOperationMethods"]/*'/>*/
    internal static IEnumerable<IMethodSymbol> GetToolOperationMethods(INamedTypeSymbol toolType)
    {
        for(INamedTypeSymbol? current = toolType; current is not null && current.ToDisplayString() != ToolManifestConstants.ToolFullName; current = current.BaseType)
        {
            if(!SymbolEqualityComparer.Default.Equals(current.ContainingAssembly,toolType.ContainingAssembly)) { break; }

            foreach(IMethodSymbol method in current.GetMembers().OfType<IMethodSymbol>())
            {
                if(method.MethodKind is not MethodKind.Ordinary || method.IsStatic || method.DeclaredAccessibility is Accessibility.Private) { continue; }

                if(method.GetAttributes().Any(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.AccessCheckAttributeFullName) is false) { continue; }

                yield return method;
            }
        }
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="ShouldRegisterModuleInitializer"]/*'/>*/
    internal static Boolean ShouldRegisterModuleInitializer(IAssemblySymbol assembly)
    {
        return CreateAssemblyManifestMetadata(assembly,[]).RegisterModuleInitializer;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestSymbolAnalysis"]/method[@name="InheritsFromTool"]/*'/>*/
    internal static Boolean InheritsFromTool(INamedTypeSymbol toolType)
    {
        if(SymbolEqualityComparer.Default.Equals(toolType.BaseType,toolType)) { return false; }

        for(INamedTypeSymbol? current = toolType.BaseType; current is not null; current = current.BaseType)
        {
            if(current.ToDisplayString() == ToolManifestConstants.ToolFullName) { return true; }
        }

        return false; 
    }
}
