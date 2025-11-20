namespace ReactF

module ToolShapeGenerator =
    open System
    open Serilog
    open ToolShape
    open System.Security.Cryptography

    let private rng = RandomNumberGenerator.Create()

    let private randomBytes count =
        try
            let bytes = Array.zeroCreate<byte> count
            rng.GetBytes(bytes)
            bytes
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomBytes failed")
            Array.zeroCreate<byte> count

    let private randomCircle () =
        try
            let bytes = randomBytes 1
            bytes.[0] < 25uy
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomCircle failed")
            false

    let private randomColor () =
        try
            let bytes = randomBytes 3
            sprintf "#%02X%02X%02X" bytes.[0] bytes.[1] bytes.[2]
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomColor failed")
            "#000000"

    let private randomCoordinate max =
        try
            if max <= 0 then 0.0
            else
                let bytes = randomBytes 4
                let rand = BitConverter.ToUInt32(bytes, 0)
                (float rand / float UInt32.MaxValue) * float max
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomCoordinate failed")
            0.0

    let private randomOpacity (min: float) (max: float) =
        try
            let bytes = randomBytes 4
            let rand = BitConverter.ToUInt32(bytes, 0)
            (float rand / float UInt32.MaxValue) * (max - min) + min
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomOpacity failed")
            min

    let private randomRotation () =
        try
            let bytes = randomBytes 8
            let rand = BitConverter.ToUInt64(bytes, 0)
            (float rand / float UInt64.MaxValue) * 360.0
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomRotation failed")
            0.0

    let private randomScale (min: float) (max: float) =
        try
            let bytes = randomBytes 4
            let rand = BitConverter.ToUInt32(bytes, 0)
            (float rand / float UInt32.MaxValue) * (max - min) + min
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomScale failed")
            min

    let private randomSides (min: int) (max: int) =
        try
            if min > max then 12
            else
                let bytes = randomBytes 4
                let rand = BitConverter.ToUInt32(bytes, 0)
                int (rand % uint32 (max - min + 1)) + min
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomSides failed")
            12

    let private randomStar () =
        try
            let bytes = randomBytes 1
            bytes.[0] < 50uy
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.randomStar failed")
            false

    let generateShape (input: ToolShape option) : ToolShape option =
        try
            let i = input |> Option.bind (fun s -> s.id) |> Option.defaultValue (Guid.NewGuid())
            let x = input |> Option.bind (fun s -> s.x) |> Option.defaultValue (randomCoordinate 7648)
            let y = input |> Option.bind (fun s -> s.y) |> Option.defaultValue (randomCoordinate 4304)
            let circle = input |> Option.bind (fun s -> s.circle) |> Option.defaultValue (randomCircle ())
            let opacity = input |> Option.bind (fun s -> s.opacity) |> Option.defaultValue (randomOpacity 0.0 1.0)
            let rgb = input |> Option.bind (fun s -> s.rgb) |> Option.defaultValue (randomColor ())
            let rotation = input |> Option.bind (fun s -> s.rotation) |> Option.defaultValue (randomRotation ())
            let scale = input |> Option.bind (fun s -> s.scale) |> Option.defaultValue (randomScale 0.1 2.0)
            let sides = input |> Option.bind (fun s -> s.sides) |> Option.defaultValue (randomSides 3 8)
            let star = input |> Option.bind (fun s -> s.star) |> Option.defaultValue (randomStar ())
            Some {
                id = Some i
                x = Some x
                y = Some y
                circle = Some circle
                opacity = Some opacity
                rgb = Some rgb
                rotation = Some rotation
                scale = Some scale
                sides = Some sides
                star = Some star
            }
        with ex ->
            Log.Error(ex, "ToolShapeGenerator.generateShape failed")
            None