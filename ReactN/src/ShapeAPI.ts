import { v4 as uuidv4 } from "uuid";
import ToolShape from "./ToolShape";

export default class ShapeAPI
{
    static generateShape(input?: ToolShape): ToolShape
    {
        try
        {
            return {
                id: input?.id ?? uuidv4(),
                x: input?.x ?? ShapeAPI.randomCoordinate(7648),
                y: input?.y ?? ShapeAPI.randomCoordinate(4304),
                circle: input?.circle ?? ShapeAPI.randomCircle(),
                opacity: input?.opacity ?? ShapeAPI.randomOpacity(0.0, 1.0),
                rgb: input?.rgb ?? ShapeAPI.randomColor(),
                rotation: input?.rotation ?? ShapeAPI.randomRotation(),
                scale: input?.scale ?? ShapeAPI.randomScale(0.1, 2.0),
                sides: input?.sides ?? ShapeAPI.randomSides(3, 8),
                star: input?.star ?? ShapeAPI.randomStar()
            };
        }
        catch (error)
        {
            console.error("[ShapeAPI] generateShape failed:",error); return {};
        }
    }

    private static randomCircle(): boolean
    {
        try
        {
            return Math.floor(Math.random() * 256) < 25;
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomCircle failed:",error); return false;
        }
    }

    private static randomColor(): string
    {
        try
        {
            const bytes = new Uint8Array(3); crypto.getRandomValues(bytes);

            return `#${bytes[0].toString(16).padStart(2, '0')}${bytes[1].toString(16).padStart(2, '0')}${bytes[2].toString(16).padStart(2, '0')}`;
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomColor failed:",error); return "";
        }
    }

    private static randomCoordinate(max: number): number
    {
        try
        {
            if (max <= 0) { return 0; }

            return Math.random() * max;
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomCoordinate failed:",error); return 0;
        }
    }

    private static randomOpacity(min: number, max: number): number
    {
        try
        {
            return min + (max - min) * Math.random();
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomOpacity failed:",error); return 1.0;
        }
    }

    private static randomRotation(): number
    {
        try
        {
            return Math.random() * 360.0;
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomRotation failed:",error); return 0;
        }
    }

    private static randomScale(min: number, max: number): number
    {
        try
        {
            return min + (max - min) * Math.random();
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomScale failed:",error); return 1.0;
        }
    }

    private static randomSides(min: number, max: number): number
    {
        try
        {
            if (min > max) { return 12; }

            return Math.floor(Math.random() * (max - min + 1)) + min;
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomSides failed:",error); return 12;
        }
    }

    private static randomStar(): boolean
    {
        try
        {
            return Math.floor(Math.random() * 256) < 50;
        }
        catch (error)
        {
            console.error("[ShapeAPI] randomStar failed:",error); return false;
        }
    }
}