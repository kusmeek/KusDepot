namespace ReactF

module ToolShapeInterop = 
    open System
    open ToolShape

    let toCSharp (fsShape: ToolShape.ToolShape) : Shapeapi.ToolShape =
        let csShape = Shapeapi.ToolShape()
        match fsShape.id with Some v -> csShape.Id <- v.ToString() | None -> ()
        match fsShape.x with Some v -> csShape.X <- v | None -> ()
        match fsShape.y with Some v -> csShape.Y <- v | None -> ()
        match fsShape.circle with Some v -> csShape.Circle <- v | None -> ()
        match fsShape.opacity with Some v -> csShape.Opacity <- v | None -> ()
        match fsShape.rgb with Some v -> csShape.Rgb <- v | None -> ()
        match fsShape.rotation with Some v -> csShape.Rotation <- v | None -> ()
        match fsShape.scale with Some v -> csShape.Scale <- v | None -> ()
        match fsShape.sides with Some v -> csShape.Sides <- v | None -> ()
        match fsShape.star with Some v -> csShape.Star <- v | None -> ()
        csShape

    let fromCSharp (csShape: Shapeapi.ToolShape) : ToolShape.ToolShape =
        {
            id = if csShape.HasId then Some(Guid.Parse(csShape.Id)) else None
            x = if csShape.HasX then Some(csShape.X) else None
            y = if csShape.HasY then Some(csShape.Y) else None
            circle = if csShape.HasCircle then Some(csShape.Circle) else None
            opacity = if csShape.HasOpacity then Some(csShape.Opacity) else None
            rgb = if csShape.HasRgb then Some(csShape.Rgb) else None
            rotation = if csShape.HasRotation then Some(csShape.Rotation) else None
            scale = if csShape.HasScale then Some(csShape.Scale) else None
            sides = if csShape.HasSides then Some(csShape.Sides) else None
            star = if csShape.HasStar then Some(csShape.Star) else None
        }