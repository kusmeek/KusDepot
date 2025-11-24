use uuid::Uuid;
use tonic::{Request, Response, Status};
use crate::shapesvcs::shapeapi::ShapeAPI;
use crate::shapesvcs::shape_api_server::ShapeApi;
use crate::shapesvcs::ToolShape as ProtoToolShape;
use crate::shapesvcs::toolshape::ToolShape as ModelToolShape;

pub struct GrpcService;

impl ProtoToolShape {
    pub fn to_model(&self) -> ModelToolShape {
        ModelToolShape {
            id: self.id.as_ref().and_then(|id| Uuid::parse_str(id).ok()),
            x: self.x,
            y: self.y,
            circle: self.circle,
            opacity: self.opacity,
            rgb: self.rgb.clone(),
            rotation: self.rotation,
            scale: self.scale,
            sides: self.sides,
            star: self.star,
        }
    }
}

impl ModelToolShape {
    pub fn to_proto(&self) -> ProtoToolShape {
        ProtoToolShape {
            id: self.id.as_ref().map(|u| u.to_string()),
            x: self.x,
            y: self.y,
            circle: self.circle,
            opacity: self.opacity,
            rgb: self.rgb.clone(),
            rotation: self.rotation,
            scale: self.scale,
            sides: self.sides,
            star: self.star,
        }
    }
}

#[tonic::async_trait]
impl ShapeApi for GrpcService {
    async fn generate_shape(
        &self,
        request: Request<ProtoToolShape>,
    ) -> Result<Response<ProtoToolShape>, Status> {
        let input = request.get_ref().to_model();
        let api = ShapeAPI;
        match api.generate_shape(Some(input.clone())) {
            Ok(shape) => {
                let proto = shape.to_proto();
                Ok(Response::new(proto))
            },
            Err(e) => {
                Err(Status::internal(format!("[ReactR-GrpcService] generate_shape failed: {:?}", e)))
            },
        }
    }
}