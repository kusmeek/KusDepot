namespace KusDepot.ToolManifests.Analyzers;

public sealed partial class ToolManifestGenerator
{
    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="ValidateCompanionIdentities"]/*'/>*/
    private static Boolean ValidateCompanionIdentities(Compilation compilation , AssemblyManifestMetadata assemblyMetadata , List<ToolModel> models , SourceProductionContext context)
    {
        HashSet<ToolModel> invalidModels = [];

        String assemblyNamespace = String.IsNullOrWhiteSpace(assemblyMetadata.CompanionNamespace) ? "KusDepot.ToolManifests.Generated" : assemblyMetadata.CompanionNamespace!;

        String assemblyTypeName = String.IsNullOrWhiteSpace(assemblyMetadata.CompanionTypeName)
            ? $"{SanitizeIdentifier(compilation.AssemblyName)}ToolManifests"
            : SanitizeIdentifier(assemblyMetadata.CompanionTypeName);

        String assemblyIdentity = GetGeneratedTypeIdentity(assemblyNamespace,assemblyTypeName);

        foreach(IGrouping<String,ToolModel> group in models.GroupBy(_ => GetGeneratedTypeIdentity(_.Namespace,_.CompanionName),StringComparer.Ordinal))
        {
            ToolModel[] conflicts = group.ToArray();

            if(conflicts.Length < 2) { continue; }

            foreach(ToolModel model in conflicts)
            {
                ToolModel other = conflicts.First(_ => ReferenceEquals(_,model) is false);

                ReportCompanionTypeCollision(context,model,GetGeneratedTypeIdentity(model.Namespace,model.CompanionName),other.ToolTypeQualifiedName);

                _ = invalidModels.Add(model);
            }
        }

        foreach(ToolModel model in models)
        {
            String identity = GetGeneratedTypeIdentity(model.Namespace,model.CompanionName);

            if(String.Equals(identity,assemblyIdentity,StringComparison.Ordinal))
            {
                ReportCompanionTypeCollision(context,model,identity,"the assembly bootstrap");

                _ = invalidModels.Add(model);
            }

            if(invalidModels.Contains(model)) { continue; }

            String[] generatedMembers = ["CreateManifest","Register","Bind","RegisterAndBind"];

            if(HasCompatiblePartialType(compilation,model.Namespace,model.CompanionName,generatedMembers) is false)
            {
                ReportCompanionTypeCollision(context,model,identity,"an existing source type");

                _ = invalidModels.Add(model);
            }
        }

        Boolean emitAssemblyBootstrap = true;
        String[] bootstrapMembers = assemblyMetadata.RegisterModuleInitializer
            ? ["RegisterAll","BindAll","RegisterAndBindAll","InitializeModule"]
            : ["RegisterAll","BindAll","RegisterAndBindAll"];

        if(HasCompatiblePartialType(compilation,assemblyNamespace,assemblyTypeName,bootstrapMembers) is false)
        {
            AttributeData? assemblyAttribute = compilation.Assembly.GetAttributes()
                .FirstOrDefault(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.ToolManifestAssemblyAttributeFullName);

            context.ReportDiagnostic(Diagnostic.Create(
                ToolManifestDiagnostics.CompanionTypeCollisionRule,
                assemblyAttribute?.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                "the assembly bootstrap",
                assemblyIdentity,
                "an incompatible existing source type"));
            emitAssemblyBootstrap = false;
        }

        models.RemoveAll(invalidModels.Contains);

        return emitAssemblyBootstrap;
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="ResolveNestedCompanionNameCollisions"]/*'/>*/
    private static void ResolveNestedCompanionNameCollisions(List<ToolModel> models)
    {
        foreach(ToolModel[] collision in models
            .GroupBy(_ => GetGeneratedTypeIdentity(_.Namespace,_.CompanionName),StringComparer.Ordinal)
            .Where(static _ => _.Count() > 1)
            .Select(static _ => _.ToArray()))
        {
            if(collision.Any(static _ => _.IsCompanionNameExplicit || _.ContainingTypeQualifiedName is null)) { continue; }

            ToolModel[] rewriteableModels = collision
                .Where(static _ => _.IsCompanionNameExplicit is false && _.ContainingTypeQualifiedName is not null)
                .ToArray();

            if(rewriteableModels.Length == 0) { continue; }

            HashSet<String> occupiedNames = new(models.Select(_ => GetGeneratedTypeIdentity(_.Namespace,_.CompanionName)),StringComparer.Ordinal);

            foreach(ToolModel model in rewriteableModels)
            {
                String readableName = String.Join("_",model.ContainingTypeQualifiedName!.Split('.').Concat([model.ToolTypeName]).Select(static _ => SanitizeIdentifier(_)));

                String hash = ComputeStableNameHash($"{model.ContainingTypeQualifiedName}.{model.ToolTypeName}");

                String companionName = $"{readableName}_{hash}ToolManifest";

                String qualifiedName = GetGeneratedTypeIdentity(model.Namespace,companionName);

                Int32 suffix = 2;

                while(occupiedNames.Contains(qualifiedName))
                {
                    companionName = $"{readableName}_{hash}_{suffix++}ToolManifest";

                    qualifiedName = GetGeneratedTypeIdentity(model.Namespace,companionName);
                }

                _ = occupiedNames.Add(qualifiedName);

                ReplaceToolModel(models,model,companionName,qualifiedName);
            }
        }
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="ReplaceToolModel"]/*'/>*/
    private static void ReplaceToolModel(List<ToolModel> models , ToolModel model , String companionName , String companionQualifiedName)
    {
        Int32 index = models.IndexOf(model);
        models[index] = new(
            model.ToolTypeName,
            model.Namespace,
            companionName,
            companionQualifiedName,
            model.ToolTypeQualifiedName,
            model.ToolLocation,
            model.ToolSchemaID,
            model.AccessKeyRealmID,
            model.ToolSchemaVersion,
            model.Operations,
            model.IsCompanionNameExplicit,
            model.ContainingTypeQualifiedName);
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="ComputeStableNameHash"]/*'/>*/
    private static String ComputeStableNameHash(String value)
    {
        Byte[] hash;

        using(SHA256 sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        return String.Concat(hash.Take(4).Select(static _ => _.ToString("x2",CultureInfo.InvariantCulture))).ToUpperInvariant();
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="HasCompatiblePartialType"]/*'/>*/
    private static Boolean HasCompatiblePartialType(Compilation compilation , String @namespace , String typeName , String[] generatedMembers)
    {
        INamedTypeSymbol[] existingTypes = compilation.GetSymbolsWithName(typeName,SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .Where(static _ => _.Locations.Any(static location => location.IsInSource))
            .Where(static _ => _.ContainingType is null)
            .Where(static _ => _.Arity == 0)
            .Where(_ => String.Equals(_.ContainingNamespace.ToDisplayString(),@namespace,StringComparison.Ordinal))
            .ToArray();

        if(existingTypes.Length == 0) { return true; }

        if(existingTypes.Length != 1) { return false; }

        INamedTypeSymbol existingType = existingTypes[0];

        if(existingType.TypeKind != TypeKind.Class || existingType.IsStatic is false || existingType.Arity != 0 || existingType.DeclaredAccessibility is not Accessibility.Internal) { return false; }

        if(existingType.DeclaringSyntaxReferences.Length == 0 || existingType.DeclaringSyntaxReferences
            .Select(static _ => _.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .Count() != existingType.DeclaringSyntaxReferences.Length)
        {
            return false;
        }

        foreach(ClassDeclarationSyntax declaration in existingType.DeclaringSyntaxReferences.Select(static _ => _.GetSyntax()).OfType<ClassDeclarationSyntax>())
        {
            if(declaration.Modifiers.Any(SyntaxKind.PartialKeyword) is false || declaration.Modifiers.Any(SyntaxKind.StaticKeyword) is false) { return false; }
        }

        return generatedMembers.All(memberName => existingType.GetMembers(memberName).Length == 0);
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="GetGeneratedTypeIdentity"]/*'/>*/
    private static String GetGeneratedTypeIdentity(String @namespace , String typeName)
        => String.IsNullOrEmpty(@namespace) ? typeName : $"global::{@namespace}.{typeName}";

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="ReportCompanionTypeCollision"]/*'/>*/
    private static void ReportCompanionTypeCollision(SourceProductionContext context , ToolModel model , String identity , String conflictingIdentity)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            ToolManifestDiagnostics.CompanionTypeCollisionRule,
            model.ToolLocation,
            model.ToolTypeQualifiedName,
            identity,
            conflictingIdentity));
    }
}
