import { startHttpServer } from "./HttpServer";
import { startGrpcServer } from "./GrpcServer";

class StartUp
{
    static async Main()
    {
        try
        {
            const httpPort = process.env.HTTP_PORT ? parseInt(process.env.HTTP_PORT) : 8087;
            const grpcPort = process.env.GRPC_PORT ? parseInt(process.env.GRPC_PORT) : 8088;

            await startHttpServer(httpPort); startGrpcServer(grpcPort);
        }
        catch (error)
        {
            console.error('[ReactN] Failed to initialize servers:', error);
        }
    }
}

StartUp.Main();