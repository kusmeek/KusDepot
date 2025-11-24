use crate::shapesvcs::shapeapi::ShapeAPI;
use crate::shapesvcs::toolshape::{ToolShapeInterOp};

#[unsafe(no_mangle)]
pub extern "C" fn generate_shape_interop(input: ToolShapeInterOp) -> ToolShapeInterOp {
    let api = ShapeAPI;
    let input_shape = input.to_toolshape();
    match api.generate_shape(Some(input_shape)) {
        Ok(shape) => ToolShapeInterOp::from_toolshape(&shape),
        Err(_) => ToolShapeInterOp {
            id: [0; 16],
            x: f64::NAN,
            y: f64::NAN,
            circle: false,
            opacity: f64::NAN,
            rgb: [0; 8],
            rotation: f64::NAN,
            scale: f64::NAN,
            sides: 0,
            star: false,
        }
    }
}