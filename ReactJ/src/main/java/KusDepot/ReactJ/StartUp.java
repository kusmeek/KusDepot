package KusDepot.ReactJ;

import io.grpc.Server;
import org.slf4j.Logger;
import io.javalin.Javalin;
import io.grpc.ServerBuilder;
import org.slf4j.LoggerFactory;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.Executors;
import java.util.concurrent.ExecutorService;

public class StartUp
{
    private static final Logger logger = LoggerFactory.getLogger(StartUp.class);
    private static Server server = null; private static Javalin app = null;

    public static void main(String[] args)
    {
        final int httpPort = Utility.getPortFromEnv("HTTP_PORT", 8085);
        final int grpcPort = Utility.getPortFromEnv("GRPC_PORT", 8086);

        ExecutorService executor = Executors.newFixedThreadPool(2);

        Runnable httpTask = () ->
        {
            try
            {
                app = Javalin.create();
                app.post("/generateshape", ctx ->
                {
                    try
                    {
                        ToolShape input = ctx.bodyAsClass(ToolShape.class);
                        ToolShape shape = ShapeAPI.GenerateShape(input);
                        ctx.json(shape);
                    }
                    catch (Exception e)
                    {
                        logger.error("GenerateShape Failed", e);
                        ctx.status(500).json(java.util.Map.of("error", "GenerateShape Failed"));
                    }
                });
                logger.info("[ReactJ-HttpServer] Listening on {}", httpPort);
                app.start(httpPort);
            }
            catch (Exception e)
            {
                logger.error("ReactJ ShapeAPI HTTP Failed", e);
                System.exit(1);
            }
        };

        Runnable grpcTask = () ->
        {
            try
            {
                server = ServerBuilder.forPort(grpcPort)
                        .addService(new ShapeGRPC()).build().start();
                logger.info("[ReactJ-GrpcServer] Listening on {}", grpcPort);
                server.awaitTermination();
            }
            catch (Exception e)
            {
                logger.error("ReactJ ShapeAPI gRPC Failed", e);
                if (server != null) server.shutdownNow();
                System.exit(1);
            }
        };

        executor.submit(httpTask); executor.submit(grpcTask);

        Runtime.getRuntime().addShutdownHook(new Thread(() ->
        {
            logger.info("Shutdown initiated.");
            try
            {
                if (app != null) { app.stop(); }

                if (server != null) { server.shutdownNow(); }
            }
            catch (Exception e)
            {
                logger.error("Error stopping servers", e);
            }
            executor.shutdownNow(); logger.info("Servers stopped.");
        }));
    }
}