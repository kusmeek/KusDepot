import fastify from "fastify";
import ShapeAPI from "./ShapeAPI";
import ToolShape from "./ToolShape";

const server = fastify();

server.post<{ Body: ToolShape }>("/generateshape", async (request, reply) =>
{
    try
    {
        const input = request.body; const shape = ShapeAPI.generateShape(input);

        reply.send(shape);
    }
    catch (error)
    {
        console.error("[ReactN-HttpServer] /generateshape failed:",error);

        reply.status(500).send({ error: "Failed to generate shape" });
    }
});

export async function startHttpServer(port: number)
{
    try
    {
        await server.listen({ port, host: "0.0.0.0" });

        console.log(`[ReactN-HttpServer] Listening on port ${port}`);
    }
    catch (error)
    {
        console.error(`[ReactN-HttpServer] Failed to start server on port ${port}:`,error);
    }
}