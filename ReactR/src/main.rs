use std::env;
pub mod shapesvcs;
use log::LevelFilter;
use log::{info, error};
use env_logger::Builder;
use axum::{Router, routing::post, Json};
use crate::shapesvcs::shapeapi::ShapeAPI;
use crate::shapesvcs::toolshape::ToolShape;
use crate::shapesvcs::grpcservice::GrpcService;
use crate::shapesvcs::shape_api_server::ShapeApiServer;
use tonic::transport::Server;

#[tokio::main]
async fn main() {
    Builder::new().filter_level(LevelFilter::Info).init();

    let http_port = env::var("HTTP_PORT").ok().and_then(|p| p.parse::<u16>().ok()).unwrap_or(8091);
    let grpc_port = env::var("GRPC_PORT").ok().and_then(|p| p.parse::<u16>().ok()).unwrap_or(8092);

    let http_addr_str = format!("0.0.0.0:{}", http_port);
    let grpc_addr_str = format!("0.0.0.0:{}", grpc_port);

    let app = Router::new().route("/generateshape", post(generate_shape_handler));
    let http_listener = tokio::net::TcpListener::bind(&http_addr_str).await.unwrap();
    info!("[ReactR-HttpServer] Listening on {}", http_port);
    let http_server = axum::serve(http_listener, app);

    let grpc_addr = grpc_addr_str.parse().unwrap();
    let grpc_service = GrpcService;
    let grpc_server = Server::builder()
        .add_service(ShapeApiServer::new(grpc_service))
        .serve(grpc_addr);
    info!("[ReactR-GrpcServer] Listening on {}", grpc_port);

    let http_handle = tokio::spawn(async move {
        if let Err(e) = http_server.await {
            error!("HTTP server error: {}", e);
        }
    });
    let grpc_handle = tokio::spawn(async move {
        if let Err(e) = grpc_server.await {
            error!("gRPC server error: {}", e);
        }
    });

    let _ = tokio::try_join!(http_handle, grpc_handle);

    info!("ShapeAPI servers stopped.");
}

async fn generate_shape_handler(Json(input): Json<ToolShape>) -> Result<Json<ToolShape>, axum::http::StatusCode> {
    let api = ShapeAPI;
    match api.generate_shape(Some(input)) {
        Ok(shape) => Ok(Json(shape)),
        Err(e) => {
            error!("Failed to generate ToolShape: {:?}", e);
            Err(axum::http::StatusCode::INTERNAL_SERVER_ERROR)
        }
    }
}