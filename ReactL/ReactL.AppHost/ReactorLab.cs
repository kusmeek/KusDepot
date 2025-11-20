internal class Laboperator
{
    private static async Task Main(String[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:InvariantCulture).CreateLogger();

        var ab = ToolBuilderFactory.CreateAspireHostBuilder(); ab.Builder.Configuration.AddJsonFile("appsettings.json");

        ab.ConfigureContainer((x,b) => b.RegisterInstance<IHostedService>(ToolBuilderFactory.CreateHostBuilder().BuildHost()));

        var kds = Environment.GetEnvironmentVariable("KusDepotSolution"); var config = ab.Builder.Configuration;;

        var o = new DistributedApplicationOptions() { AllowUnsecuredTransport = true , Args = args , DisableDashboard = true };

        ab.UseBuilderOptions(o).UseConsoleLifetime().UseLogger(Log.Logger).Seal().AutoStart(); ab.Builder.Services.AddSerilog(Log.Logger);

        var F_HttpPort = GetPort(config,REACT_F,HTTP_PORT,DEFAULT_F_HTTP_PORT);
        var F_GrpcPort = GetPort(config,REACT_F,GRPC_PORT,DEFAULT_F_GRPC_PORT);
        var reactF = ab.Builder.AddProject<ReactF>(REACT_F)
            .WithEnvironment(HTTP_PORT,F_HttpPort.ToString())
            .WithEnvironment(GRPC_PORT,F_GrpcPort.ToString())
            .WithHttpEndpoint(port:F_HttpPort,targetPort:F_HttpPort,isProxied:false,name:F_HTTP)
            .WithHttpEndpoint(port:F_GrpcPort,targetPort:F_GrpcPort,isProxied:false,name:F_GRPC);

        var G_HttpPort = GetPort(config,REACT_G,HTTP_PORT,DEFAULT_G_HTTP_PORT);
        var G_GrpcPort = GetPort(config,REACT_G,GRPC_PORT,DEFAULT_G_GRPC_PORT);
        var reactG = ab.Builder.AddProject<ReactG>(REACT_G)
            .WithEnvironment(HTTP_PORT,G_HttpPort.ToString())
            .WithEnvironment(GRPC_PORT,G_GrpcPort.ToString())
            .WithHttpEndpoint(port:G_HttpPort,targetPort:G_HttpPort,isProxied:false,name:G_HTTP)
            .WithHttpEndpoint(port:G_GrpcPort,targetPort:G_GrpcPort,isProxied:false,name:G_GRPC);

        var J_HttpPort = GetPort(config,REACT_J,HTTP_PORT,DEFAULT_J_HTTP_PORT);
        var J_GrpcPort = GetPort(config,REACT_J,GRPC_PORT,DEFAULT_J_GRPC_PORT);
        var reactJ = ab.Builder.AddProject<ReactJ>(REACT_J)
            .WithEnvironment(HTTP_PORT,J_HttpPort.ToString())
            .WithEnvironment(GRPC_PORT,J_GrpcPort.ToString())
            .WithHttpEndpoint(port:J_HttpPort,targetPort:J_HttpPort,isProxied:false,name:J_HTTP)
            .WithHttpEndpoint(port:J_GrpcPort,targetPort:J_GrpcPort,isProxied:false,name:J_GRPC);

        var N_HttpPort = GetPort(config,REACT_N,HTTP_PORT,DEFAULT_N_HTTP_PORT);
        var N_GrpcPort = GetPort(config,REACT_N,GRPC_PORT,DEFAULT_N_GRPC_PORT);
        var reactN = ab.Builder.AddProject<ReactN>(REACT_N)
            .WithEnvironment(HTTP_PORT,N_HttpPort.ToString())
            .WithEnvironment(GRPC_PORT,N_GrpcPort.ToString())
            .WithHttpEndpoint(port:N_HttpPort,targetPort:N_HttpPort,isProxied:false,name:N_HTTP)
            .WithHttpEndpoint(port:N_GrpcPort,targetPort:N_GrpcPort,isProxied:false,name:N_GRPC);

        var P_HttpPort = GetPort(config,REACT_P,HTTP_PORT,DEFAULT_P_HTTP_PORT);
        var P_GrpcPort = GetPort(config,REACT_P,GRPC_PORT,DEFAULT_P_GRPC_PORT);
        var reactP = ab.Builder.AddProject<ReactP>(REACT_P)
            .WithEnvironment(HTTP_PORT,P_HttpPort.ToString())
            .WithEnvironment(GRPC_PORT,P_GrpcPort.ToString())
            .WithHttpEndpoint(port:P_HttpPort,targetPort:P_HttpPort,isProxied:false,name:P_HTTP)
            .WithHttpEndpoint(port:P_GrpcPort,targetPort:P_GrpcPort,isProxied:false,name:P_GRPC);

        var R_HttpPort = GetPort(config,REACT_R,HTTP_PORT,DEFAULT_R_HTTP_PORT);
        var R_GrpcPort = GetPort(config,REACT_R,GRPC_PORT,DEFAULT_R_GRPC_PORT);
        var reactR = ab.Builder.AddProject<ReactR>(REACT_R)
            .WithEnvironment(HTTP_PORT,R_HttpPort.ToString())
            .WithEnvironment(GRPC_PORT,R_GrpcPort.ToString())
            .WithHttpEndpoint(port:R_HttpPort,targetPort:R_HttpPort,isProxied:false,name:R_HTTP)
            .WithHttpEndpoint(port:R_GrpcPort,targetPort:R_GrpcPort,isProxied:false,name:R_GRPC);

        var S_HttpPort = GetPort(config,REACT_S,HTTP_PORT,DEFAULT_S_HTTP_PORT);
        var reactS = ab.Builder.AddProject<Reacts>(REACT_S)
            .WithEnvironment(HTTP_PORT,S_HttpPort.ToString())
            .WithReference(reactF.GetEndpoint(name:F_HTTP))
            .WithReference(reactF.GetEndpoint(name:F_GRPC))
            .WithReference(reactG.GetEndpoint(name:G_HTTP))
            .WithReference(reactG.GetEndpoint(name:G_GRPC))
            .WithReference(reactJ.GetEndpoint(name:J_HTTP))
            .WithReference(reactJ.GetEndpoint(name:J_GRPC))
            .WithReference(reactN.GetEndpoint(name:N_HTTP))
            .WithReference(reactN.GetEndpoint(name:N_GRPC))
            .WithReference(reactP.GetEndpoint(name:P_HTTP))
            .WithReference(reactP.GetEndpoint(name:P_GRPC))
            .WithReference(reactR.GetEndpoint(name:R_HTTP))
            .WithReference(reactR.GetEndpoint(name:R_GRPC));

        var aspirehost = ab.BuildAspireHost();

        await Task.Delay(Timeout.Infinite);
    }

    private static Int32 GetPort(ConfigurationManager config , String svcName , String key , Int32 defaultPort)
    {
        return Int32.TryParse(config[$"Executables:{svcName}:Environment:{key}"], out var port) ? port : defaultPort;
    }

    private const Int32 DEFAULT_F_HTTP_PORT = 8081;
    private const Int32 DEFAULT_F_GRPC_PORT = 8082;
    private const Int32 DEFAULT_G_HTTP_PORT = 8083;
    private const Int32 DEFAULT_G_GRPC_PORT = 8084;
    private const Int32 DEFAULT_J_HTTP_PORT = 8085;
    private const Int32 DEFAULT_J_GRPC_PORT = 8086;
    private const Int32 DEFAULT_N_HTTP_PORT = 8087;
    private const Int32 DEFAULT_N_GRPC_PORT = 8088;
    private const Int32 DEFAULT_P_HTTP_PORT = 8089;
    private const Int32 DEFAULT_P_GRPC_PORT = 8090;
    private const Int32 DEFAULT_R_HTTP_PORT = 8091;
    private const Int32 DEFAULT_R_GRPC_PORT = 8092;
    private const Int32 DEFAULT_S_HTTP_PORT = 8080;
    private const String HTTP_PORT = "HTTP_PORT";
    private const String GRPC_PORT = "GRPC_PORT";
    private const String REACT_F = "ReactF";
    private const String REACT_G = "ReactG";
    private const String REACT_J = "ReactJ";
    private const String REACT_N = "ReactN";
    private const String REACT_P = "ReactP";
    private const String REACT_R = "ReactR";
    private const String REACT_S = "ReactS";
    private const String F_HTTP = "F-HTTP";
    private const String F_GRPC = "F-GRPC";
    private const String G_HTTP = "G-HTTP";
    private const String G_GRPC = "G-GRPC";
    private const String J_HTTP = "J-HTTP";
    private const String J_GRPC = "J-GRPC";
    private const String N_HTTP = "N-HTTP";
    private const String N_GRPC = "N-GRPC";
    private const String P_HTTP = "P-HTTP";
    private const String P_GRPC = "P-GRPC";
    private const String R_HTTP = "R-HTTP";
    private const String R_GRPC = "R-GRPC";
    private const String S_HTTP = "S-HTTP";
}