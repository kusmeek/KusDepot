import path from "path";
import ShapeAPI from "./ShapeAPI";
import ToolShape from "./ToolShape";
import * as grpc from "@grpc/grpc-js";
import * as protoLoader from "@grpc/proto-loader";

const PROTO_PATH = path.join(__dirname, "../proto/shapeapi.proto");

const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
    defaults: false,
    keepCase: true,
    longs: String,
    enums: String
});

const shapeapiProto = grpc.loadPackageDefinition(packageDefinition).shapeapi as any;

function generateShape(call: any, callback: any)
{
    try
    {
        const input: ToolShape = call.request;

        const shape = ShapeAPI.generateShape(input);

        callback(null, shape);
    }
    catch (error)
    {
        console.error("[ReactN-GrpcServer] GenerateShape failed:", error);

        callback({ code: grpc.status.INTERNAL, message: "Failed to generate shape" });
    }
}

export function startGrpcServer(port: number)
{
    try
    {
        const server = new grpc.Server();

        server.addService(shapeapiProto.ShapeAPI.service, { generateShape });

        server.bindAsync(`0.0.0.0:${port}`, grpc.ServerCredentials.createInsecure(), (err: Error | null, bindPort: number) => {
            if (err)
            {
                console.error(`[ReactN-GrpcServer] Failed to bind server on port ${port}:`, err); return;
            }
            console.log(`[ReactN-GrpcServer] Listening on port ${bindPort}`);
        });
    }
    catch (error)
    {
        console.error(`[ReactN-GrpcServer] Failed to start server on port ${port}:`, error);
    }
}