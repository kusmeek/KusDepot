namespace KusDepot.Exams.Tools;

internal sealed class FactoryTestTool : Tool
{
    public FactoryTestTool() : base() {}

    public FactoryTestTool(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null,
           Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,logger) {}
}

internal sealed class FactoryTestToolHost : ToolHost
{
    public FactoryTestToolHost() : base() {}

    public FactoryTestToolHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null,
           IConfiguration? configuration = null , Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger) {}
}

internal sealed class FactoryTestToolGenericHost : ToolGenericHost
{
    public FactoryTestToolGenericHost() : base() {}

    public FactoryTestToolGenericHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           IHost? host = null , Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null)
        : base(accessmanager,data,id,services,commands,configuration,host,lifeid,lifetime,logger) {}
}

internal sealed class FactoryTestToolWebHost : ToolWebHost
{
    public FactoryTestToolWebHost() : base() {}

    public FactoryTestToolWebHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null , ICollection<String>? urls = null , IHost? webapplication = null)
        : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger,urls,webapplication) {}
}

internal sealed class FactoryTestToolAspireHost : ToolAspireHost
{
    public FactoryTestToolAspireHost() : base() {}

    public FactoryTestToolAspireHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null , IHost? aspireapplication = null)
        : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger,aspireapplication) {}
}
