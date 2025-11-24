namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="FormatName"]/*'/>*/
    private static String FormatName(Type type)
    {
        if(type is null) { return String.Empty; }

        if(type.IsGenericType is false) { return type.FullName!.Replace('+' , '.'); }

        String name;

        if(type.IsNested)
        {
            name = $"{type.DeclaringType!.FullName!.Replace('+','.')}.{type.Name[..type.Name.IndexOf('`',StringComparison.InvariantCulture)]}";
        }
        else
        {
            name = $"{type.Namespace}.{type.Name[..type.Name.IndexOf('`',StringComparison.InvariantCulture)]}";
        }

        return $"{name}<{String.Join(",",type.GetGenericArguments().Select(FormatName))}>";
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetAllDataTypes"]/*'/>*/
    public static Dictionary<String,String> GetAllDataTypes()
    {
        try
        {
            return typeof(DataType).GetFields(BindingFlags.Public|BindingFlags.Static).ToDictionary(_ => _.Name,_ => (String)_.GetValue(null)!);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetAllDataTypesFail); if(NoExceptions) { return new(); } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetAllTags"]/*'/>*/
    public static List<String> GetAllTags(Type type)
    {
        if(type is null) { return new(); }

        try
        {
            if(new Type[] { typeof(ArchitectureType),
                            typeof(BrowserType),
                            typeof(DatabaseType),
                            typeof(DeploymentType),
                            typeof(FrameworkVersionType),
                            typeof(HardwareType),
                            typeof(HostType),
                            typeof(Language),
                            typeof(OperatingSystemType),
                            typeof(PlatformType),
                            typeof(ServiceReference),
                            typeof(UsageType)
                          }.Contains(type) is false)
            {
                return new();
            }
            else
            {
                return type.GetFields(BindingFlags.Public|BindingFlags.Static).Select(_ => (String)_.GetValue(null)!).ToList();
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetAllTagsFail); if(NoExceptions) { return new(); } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetCommandSpecifications"]/*'/>*/
    public static Dictionary<String,String>? GetCommandSpecifications(Type? type)
    {
        if(type is null || !type.IsAssignableTo(typeof(Command))) { return null; }

        try
        {
            return type.Assembly.GetCustomAttributesData()

                .Where(d => String.Equals(d.AttributeType.FullName,typeof(CommandContainerAttribute).FullName,StringComparison.Ordinal))

                .Select(d => new 
                {
                    Type = d.ConstructorArguments.ElementAt(0).Value as Type,
                    Handle = d.ConstructorArguments.ElementAt(1).Value as String,
                    Specifications = d.ConstructorArguments.ElementAt(2).Value as String
                })

                .Where(d => d.Type is not null && d.Handle is not null && d.Specifications is not null && String.Equals(d.Type.FullName,type.FullName,StringComparison.Ordinal))

                .ToDictionary(d => d.Handle!,d => d.Specifications!);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetCommandSpecificationsFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetAssemblyCommands"]/*'/>*/
    public static HashSet<Tuple<String,String,String>>? GetAssemblyCommands(Byte[]? assembly = null , IEnumerable<String>? dllpaths = null)
    {
        if(assembly is null || Equals(assembly.Length,0)) { return null; }

        try
        {
            List<String> dlls = GetNetDlls(); dlls.AddRange(dllpaths?.ToList() ?? new());

            dlls.AddRange(Directory.GetFiles(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Common))!.Location)!,"*.dll"));

            using MetadataLoadContext c = new MetadataLoadContext(new PathAssemblyResolver(dlls),typeof(Object).Assembly.FullName);

            return c.LoadFromByteArray(assembly).GetCustomAttributesData()

                .Where(d => String.Equals(d.AttributeType.FullName,typeof(CommandContainerAttribute).FullName,StringComparison.Ordinal))

                .Select(d => new 
                {
                    Type = d.ConstructorArguments.ElementAt(0).Value as Type,
                    Handle = d.ConstructorArguments.ElementAt(1).Value as String,
                    Specifications = d.ConstructorArguments.ElementAt(2).Value as String
                })

                .Where(d => d.Type is not null && d.Handle is not null && d.Specifications is not null)

                .Select(d => new Tuple<String,String,String>(d.Type!.FullName!,d.Handle!,d.Specifications!))

                .ToHashSet();
        }
        catch ( BadImageFormatException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetAssemblyCommandsFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetAssemblyServices"]/*'/>*/
    public static HashSet<Tuple<String,String>>? GetAssemblyServices(Byte[]? assembly = null , IEnumerable<String>? dllpaths = null)
    {
        if(assembly is null || Equals(assembly.Length,0)) { return null; }

        try
        {
            List<String> dlls = GetNetDlls(); dlls.AddRange(dllpaths?.ToList() ?? new());

            dlls.AddRange(Directory.GetFiles(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Common))!.Location)!,"*.dll"));

            using MetadataLoadContext c = new MetadataLoadContext(new PathAssemblyResolver(dlls),typeof(Object).Assembly.FullName);

            return c.LoadFromByteArray(assembly).GetCustomAttributesData()

                .Where(d => String.Equals(d.AttributeType.FullName,typeof(ServiceContainerAttribute).FullName,StringComparison.Ordinal))

                .Select(d => new 
                {
                    Type = d.ConstructorArguments.ElementAt(0).Value as Type
                })

                .Where(d => d.Type is not null)

                .Select(d => new Tuple<String,String>(d.Type!.FullName!,GetInterfaceList(d.Type)!))

                .ToHashSet();
        }
        catch ( BadImageFormatException ) { return null; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetAssemblyServicesFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetInheritanceList"]/*'/>*/
    public static String? GetInheritanceList(Object? input)
    {
        if(input is null) { return null; }

        List<String> i = new(); Type? t = input.GetType();

        while(t is not null) { i.Add(FormatName(t)); t = t.BaseType; }

        i.Reverse(); return String.Join(" -> ",i);
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetInterfaceList"]/*'/>*/
    public static String? GetInterfaceList(Object? input)
    {
        return input is null ? null : String.Join(" + ", input.GetType().GetInterfaces().Select(FormatName));
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GetInterfaceListType"]/*'/>*/
    public static String? GetInterfaceList(Type? type)
    {
        return type is null ? null : String.Join(" + ", type.GetInterfaces().Select(FormatName));
    }

    private static List<String> GetNetDlls()
    {
        try
        {
            List<String> dlls = new();

            var rtd = RuntimeEnvironment.GetRuntimeDirectory(); dlls.AddRange(Directory.GetFiles(rtd,"*.dll"));

            var ver = Path.GetFileName(rtd.TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar));

            var shr = Path.GetDirectoryName(Path.GetDirectoryName(rtd.TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar))!);

            var asp = Path.Combine(shr!,"Microsoft.AspNetCore.App",ver); if(Directory.Exists(asp)) dlls.AddRange(Directory.GetFiles(asp,"*.dll"));

            return dlls;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetNetDllsFail); if(NoExceptions) { return []; } throw; }
    }
}