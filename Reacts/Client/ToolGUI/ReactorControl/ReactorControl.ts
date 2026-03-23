import ToolShape from "../ToolShape";
import ToolMessage from "../ToolMessage";
import ToolShapePosition from "../ToolShapePosition";
import StringSanitizer from "../StringSanitizer/StringSanitizer";
import { HubConnection, HubConnectionBuilder, LogLevel, HttpTransportType } from "@microsoft/signalr";

declare const REACTOR_PATH: string;

export default class ReactorControl
{
    private connection: HubConnection;
    private corePurgedHandler: (() => void) | null = null;
    private shapeRemovedHandler: ((id: string) => void) | null = null;
    private shapeAddedHandler: ((shape: ToolShape) => void) | null = null;
    private shapeUpdatedHandler: ((shape: ToolShape) => void) | null = null;
    private messageReceivedHandler: ((message: ToolMessage) => void) | null = null;
    private shapePositionUpdatedHandler: ((position: ToolShapePosition) => void) | null = null;

    constructor(url?: string)
    {
        if (!url)
        {
            url = (typeof REACTOR_PATH !== 'undefined' ? REACTOR_PATH : '/reactor');
        }
        this.connection = new HubConnectionBuilder()
            .withUrl(url, { transport: HttpTransportType.WebSockets })
            .configureLogging(LogLevel.Error)
            .build();

        this.connection.on("ShapeAdded", (shape: ToolShape) =>
        {
            if (this.shapeAddedHandler && shape != null) this.shapeAddedHandler(shape);
        });

        this.connection.on("ShapeUpdated", (shape: ToolShape) =>
        {
            if (this.shapeUpdatedHandler && shape != null) this.shapeUpdatedHandler(shape);
        });

        this.connection.on("ShapeRemoved", (id: string) =>
        {
            if (this.shapeRemovedHandler && id != null) this.shapeRemovedHandler(id);
        });

        this.connection.on("ShapePositionUpdated", (position: ToolShapePosition) => {
            if (this.shapePositionUpdatedHandler && position != null) this.shapePositionUpdatedHandler(position);
        });

        this.connection.on("CorePurge", () => {
            if (this.corePurgedHandler) this.corePurgedHandler();
        });

        this.connection.on("MessageReceived", (message: ToolMessage) => {
            if (this.messageReceivedHandler && message != null) this.messageReceivedHandler(message);
        });
    }

    async start()
    {
        if (this.connection.state !== "Disconnected") return;
        try
        {
            await this.connection.start();
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to start connection:",error);
        }
    }

    async stop()
    {
        try
        {
            await this.connection.stop();
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to stop connection:",error);
        }
    }

    async addShape(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("AddShape",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to add shape:",error);
        }
    }

    async updateShape(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("UpdateShape",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to update shape:",error);
        }
    }

    async updateShapePosition(position: ToolShapePosition): Promise<void>
    {
        try
        {
            await this.connection.send("UpdateShapePosition",position);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to update shape position:",error);
        }
    }

    async removeShape(id: string): Promise<void>
    {
        try
        {
            await this.connection.send("RemoveShape",id);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to remove shape:",error);
        }
    }

    async purgeCore(): Promise<void>
    {
        try
        {
            await this.connection.send("PurgeCore");
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to purge core:",error);
        }
    }

    async setCoreSpeed(speed: number): Promise<void>
    {
        try
        {
            await this.connection.send("SetCoreSpeed",speed);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to set core speed:",error);
        }
    }

    async randomizeShape(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShape",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape:",error);
        }
    }

    async randomizeShapeFH(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeFH",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (FH):",error);
        }
    }

    async randomizeShapeFG(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeFG",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (FG):",error);
        }
    }

    async randomizeShapeGH(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeGH",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (GH):",error);
        }
    }

    async randomizeShapeGG(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeGG",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (GG):",error);
        }
    }

    async randomizeShapeJH(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeJH",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (JH):",error);
        }
    }

    async randomizeShapeJG(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeJG",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (JG):",error);
        }
    }

    async randomizeShapeNH(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeNH",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (NH):",error);
        }
    }

    async randomizeShapeNG(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeNG",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (NG):",error);
        }
    }

    async randomizeShapePH(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapePH",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (PH):",error);
        }
    }

    async randomizeShapePG(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapePG",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (PG):",error);
        }
    }

    async randomizeShapeRH(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeRH",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (RH):",error);
        }
    }

    async randomizeShapeRG(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("RandomizeShapeRG",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to randomize shape (RG):",error);
        }
    }

    async reMakeShape(shape: ToolShape): Promise<void>
    {
        try
        {
            await this.connection.send("ReMakeShape",shape);
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to ReMake shape:",error);
        }
    }

    async sendMessage(message: ToolMessage): Promise<void>
    {
        try
        {
            const sanitizedSender = StringSanitizer.sanitize(message.sender);
            const sanitizedMessage = StringSanitizer.sanitize(message.message);
            await this.connection.send("SendMessage", { sender: sanitizedSender, message: sanitizedMessage });
        }
        catch (error)
        {
            console.error("[ReactorControl] Failed to send message:",error);
        }
    }

    onShapeAdded(handler: (shape: ToolShape) => void): void
    {
        this.shapeAddedHandler = handler;
    }

    onShapeUpdated(handler: (shape: ToolShape) => void): void
    {
        this.shapeUpdatedHandler = handler;
    }

    onShapeRemoved(handler: (id: string) => void): void
    {
        this.shapeRemovedHandler = handler;
    }

    onShapePositionUpdated(handler: (position: ToolShapePosition) => void): void
    {
        this.shapePositionUpdatedHandler = handler;
    }

    onCorePurged(handler: () => void): void
    {
        this.corePurgedHandler = handler;
    }

    onMessageReceived(handler: (message: ToolMessage) => void): void
    {
        this.messageReceivedHandler = handler;
    }
}