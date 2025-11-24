import os
import sys
import signal
import uvicorn
import threading
from . import logger
from .http_server import app
from .grpc_server import serve_grpc

def main():
    http_port, grpc_port = (int(os.environ.get("HTTP_PORT", 8089)), int(os.environ.get("GRPC_PORT", 8090)))

    grpc_thread = threading.Thread(target=serve_grpc, args=(grpc_port,), name="gRPCServer", daemon=True)
    grpc_thread.start()

    logger.info(f"[ReactP-HttpServer] Listening on {http_port}")

    uvicorn.run(app, host="0.0.0.0", port=http_port, log_level="error")

if __name__ == "__main__":
    main()