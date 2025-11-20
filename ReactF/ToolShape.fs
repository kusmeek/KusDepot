namespace ReactF

module ToolShape =
    open System

    [<CLIMutable>]
    type ToolShape = {
        id: Guid option
        x: float option
        y: float option
        circle: bool option
        opacity: float option
        rgb: string option
        rotation: float option
        scale: float option
        sides: int option
        star: bool option
    }