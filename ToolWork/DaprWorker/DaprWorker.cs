namespace ToolWork;

public static class DaprWorker
{
    public static IToolWebHostBuilder ConfigureWorkflows(IToolWebHostBuilder builder)
    {
        DaprWorkerConfig? cfg = builder.Builder.Configuration.GetSection("DaprWorkerConfig").Get<DaprWorkerConfig>();

        if(cfg?.Validate() is not true) { throw new InvalidOperationException("Invalid DaprWorkerConfig"); }

        SetEnvironmentVariable("DAPR_GRPC_PORT",cfg?.GrpcPort.ToString());

        builder.Builder.Services.AddDaprWorkflow(o =>
        {
            o.RegisterWorkflow<ToolWorkflow>();

            o.RegisterActivity<ToolActivity>();

            o.RegisterWorkflow<ToolDataWorkflow>();

            o.RegisterActivity<ToolDataActivity>();
        });

        return builder;
    }
}