import grpc
from . import logger
from concurrent import futures
from .toolshape import ToolShape
from .shapeapi import generate_shape
from .shapeapi_pb2 import ToolShape as ProtoToolShape
from .shapeapi_pb2_grpc import ShapeAPIServicer, add_ShapeAPIServicer_to_server

class ShapeAPIService(ShapeAPIServicer):
    def GenerateShape(self, request, context):
        try:
            input_shape = ToolShape.from_proto(request)
            shape = generate_shape(input_shape)
            return shape.to_proto()
        except Exception as e:
            logger.error("gRPC GenerateShape failed", exc_info=True)
            return ProtoToolShape()

def serve_grpc(port=8090):
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    add_ShapeAPIServicer_to_server(ShapeAPIService(), server)
    server.add_insecure_port(f'[::]:{port}')
    logger.info(f"[ReactP-GrpcServer] Listening on {port}")
    server.start()
    server.wait_for_termination()