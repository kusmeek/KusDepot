namespace ReactF

module Utility =
    open System
    open Serilog
    open Microsoft.AspNetCore.Builder
    open Microsoft.Extensions.Logging

    let getPortFromEnv (envVar: string) (defaultPort: int) =
        try
            match Environment.GetEnvironmentVariable(envVar) with
            | null -> defaultPort
            | value -> match Int32.TryParse(value) with | true, v -> v | _ -> defaultPort
        with ex ->
            Log.Error(ex, "Utility.getPortFromEnv failed")
            defaultPort

    let setupLog (builder: WebApplicationBuilder) =
        builder.Logging.ClearProviders() |> ignore
        Log.Logger <- LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger()        