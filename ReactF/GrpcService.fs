namespace ReactF

module GrpcService =
    open Serilog
    open Shapeapi
    open Grpc.Core
    open System.Threading.Tasks

    type ShapeApiService() =
        inherit ShapeAPI.ShapeAPIBase()

        override _.GenerateShape(request: ToolShape, context: ServerCallContext) : Task<ToolShape> =
            try
                let input = ToolShapeInterop.fromCSharp request
                match ToolShapeGenerator.generateShape (Some input) with
                | Some result -> Task.FromResult(ToolShapeInterop.toCSharp result)
                | None -> null
            with ex ->
                Log.Error(ex, "GrpcService.GenerateShape failed")
                null