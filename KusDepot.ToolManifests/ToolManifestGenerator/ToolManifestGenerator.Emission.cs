namespace KusDepot.ToolManifests.Analyzers;

public sealed partial class ToolManifestGenerator
{
    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="GenerateAssemblySource"]/*'/>*/
    private static String GenerateAssemblySource(Compilation compilation , IEnumerable<ToolModel> models , AssemblyManifestMetadata assemblyMetadata)
    {
        String assemblyNamespace = String.IsNullOrWhiteSpace(assemblyMetadata.CompanionNamespace) ? "KusDepot.ToolManifests.Generated" : assemblyMetadata.CompanionNamespace!;
        String assemblyTypeName = String.IsNullOrWhiteSpace(assemblyMetadata.CompanionTypeName)
            ? $"{SanitizeIdentifier(compilation.AssemblyName)}ToolManifests"
            : SanitizeIdentifier(assemblyMetadata.CompanionTypeName);
        Boolean registerModuleInitializer = assemblyMetadata.RegisterModuleInitializer;
        StringBuilder builder = new();

        builder.AppendLine($"namespace {assemblyNamespace};");
        builder.AppendLine();
        builder.AppendLine($"internal static partial class {assemblyTypeName}");
        builder.AppendLine("{");
        builder.AppendLine("    public static void RegisterAll()");
        builder.AppendLine("    {");

        foreach(ToolModel model in models.OrderBy(_ => _.ToolTypeName,StringComparer.Ordinal))
        {
            builder.AppendLine($"        _ = {model.CompanionQualifiedName}.Register();");
        }

        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static void BindAll()");
        builder.AppendLine("    {");

        foreach(ToolModel model in models.OrderBy(_ => _.ToolTypeName,StringComparer.Ordinal))
        {
            builder.AppendLine($"        _ = {model.CompanionQualifiedName}.Bind();");
        }

        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static void RegisterAndBindAll()");
        builder.AppendLine("    {");

        foreach(ToolModel model in models.OrderBy(_ => _.ToolTypeName,StringComparer.Ordinal))
        {
            builder.AppendLine($"        _ = {model.CompanionQualifiedName}.RegisterAndBind();");
        }

        builder.AppendLine("    }");

        if(registerModuleInitializer)
        {
            builder.AppendLine();
            builder.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
            builder.AppendLine("    internal static void InitializeModule()");
            builder.AppendLine("    {");
            builder.AppendLine("        RegisterAndBindAll();");
            builder.AppendLine("    }");
        }

        builder.AppendLine("}");

        return builder.ToString();
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="GenerateToolSource"]/*'/>*/
    private static String GenerateToolSource(ToolModel model)
    {
        StringBuilder builder = new();

        if(String.IsNullOrEmpty(model.Namespace) is false)
        {
            builder.AppendLine($"namespace {model.Namespace};");
            builder.AppendLine();
        }

        builder.AppendLine($"internal static partial class {model.CompanionName}");
        builder.AppendLine("{");
        builder.AppendLine("    public static global::KusDepot.ToolManifest CreateManifest()");
        builder.AppendLine("    {");
        builder.AppendLine("        return global::KusDepot.ToolManifest.Create(");
        builder.AppendLine($"            toolschemaid: \"{EscapeStringLiteral(model.ToolSchemaID)}\",");
        builder.AppendLine(model.AccessKeyRealmID is null
            ? "            accesskeyrealmid: null,"
            : $"            accesskeyrealmid: \"{EscapeStringLiteral(model.AccessKeyRealmID)}\",");
        builder.AppendLine($"            toolschemaversion: \"{EscapeStringLiteral(model.ToolSchemaVersion)}\",");
        builder.AppendLine($"            tooltype: typeof({model.ToolTypeQualifiedName}).FullName,");
        builder.AppendLine("            operations:");
        builder.AppendLine("            [");

        foreach(OperationModel operation in model.Operations)
        {
            builder.AppendLine("                new global::KusDepot.ToolOperationDescriptor");
            builder.AppendLine("                {");
            builder.AppendLine($"                    Index = {operation.Index},");
            builder.AppendLine($"                    MethodName = nameof({model.ToolTypeQualifiedName}.{operation.MethodName}),");
            builder.AppendLine($"                    Description = \"{EscapeStringLiteral(operation.Description)}\"");
            builder.AppendLine("                },");
        }

        builder.AppendLine("            ]).ComputeManifestHash();");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static bool Register()");
        builder.AppendLine("    {");
        builder.AppendLine("        return global::KusDepot.Security.ToolManifestRegistry.TryRegister(CreateManifest());");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static bool Bind()");
        builder.AppendLine("    {");
        builder.AppendLine($"        return global::KusDepot.Security.ToolManifestRegistry.TryBind<{model.ToolTypeQualifiedName}>(CreateManifest());");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static bool RegisterAndBind()");
        builder.AppendLine("    {");
        builder.AppendLine($"        return global::KusDepot.Security.ToolManifestRegistry.TryRegisterAndBind<{model.ToolTypeQualifiedName}>(CreateManifest());");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="EscapeStringLiteral"]/*'/>*/
    private static String EscapeStringLiteral(String value)
    {
        return value.Replace("\\","\\\\").Replace("\"","\\\"");
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="SanitizeIdentifier"]/*'/>*/
    private static String SanitizeIdentifier(String? value)
    {
        if(String.IsNullOrWhiteSpace(value)) { return "Generated"; }

        String identifier = value!;

        StringBuilder builder = new();

        foreach(Char c in identifier)
        {
            if(Char.IsLetterOrDigit(c) || c == '_') { builder.Append(c); }
        }

        if(builder.Length == 0)
        {
            builder.Append('_');
        }

        if(!SyntaxFacts.IsIdentifierStartCharacter(builder[0])) { builder.Insert(0,'_'); }

        return builder.ToString();
    }
}
