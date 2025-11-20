namespace ReactF

module StartUp =
    open System
    open Serilog
    open System.Threading.Tasks
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.AspNetCore.Server.Kestrel.Core

    [<EntryPoint>]
    let main args =
        try
            let httpPort = Utility.getPortFromEnv "HTTP_PORT" 8081
            let grpcPort = Utility.getPortFromEnv "GRPC_PORT" 8082

            let builder = WebApplication.CreateBuilder(args)
            builder.WebHost.ConfigureKestrel(fun options ->
                options.ListenAnyIP(httpPort)
                options.ListenAnyIP(grpcPort, fun o -> o.Protocols <- HttpProtocols.Http2)
            ) |> ignore
            builder.Services.AddGrpc() |> ignore
            Utility.setupLog(builder)
            let app = builder.Build()

            app.MapPost("/generateshape", Func<HttpContext, Task<IResult>>(fun ctx ->
                task {
                    try
                        let! shape = ctx.Request.ReadFromJsonAsync<ToolShape.ToolShape>()
                        match ToolShapeGenerator.generateShape (Some shape) with
                        | Some result -> return Results.Json(result)
                        | None -> return Results.Problem("Shape generation failed")
                    with ex ->
                        Log.Error(ex, "StartUp: /generateshape failed")
                        return Results.Problem("Internal server error")
                }
            )) |> ignore

            app.MapGrpcService<GrpcService.ShapeApiService>() |> ignore

            Log.Information("[ReactF-HttpServer] Listening on {HttpPort}", httpPort)
            Log.Information("[ReactF-GrpcServer] Listening on {GrpcPort}", grpcPort)

            app.Run()
            0
        with ex ->
            Log.Error(ex, "StartUp: main failed")
            -1