use uuid::Uuid;
use std::f64::NAN;
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ToolShape {
    pub id: Option<Uuid>,
    pub x: Option<f64>,
    pub y: Option<f64>,
    pub circle: Option<bool>,
    pub opacity: Option<f64>,
    pub rgb: Option<String>,
    pub rotation: Option<f64>,
    pub scale: Option<f64>,
    pub sides: Option<i32>,
    pub star: Option<bool>,
}

#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct ToolShapeInterOp {
    pub id: [u8; 16],
    pub x: f64,
    pub y: f64,
    pub circle: bool,
    pub opacity: f64,
    pub rgb: [u8; 8],
    pub rotation: f64,
    pub scale: f64,
    pub sides: i32,
    pub star: bool,
}

impl ToolShapeInterOp {
    pub fn from_toolshape(shape: &ToolShape) -> Self {
        let mut id = [0u8; 16];
        if let Some(uuid) = shape.id {
            id.copy_from_slice(uuid.as_bytes());
        }
        let x = shape.x.unwrap_or(NAN);
        let y = shape.y.unwrap_or(NAN);
        let circle = shape.circle.unwrap_or(false);
        let opacity = shape.opacity.unwrap_or(NAN);
        let rotation = shape.rotation.unwrap_or(NAN);
        let mut rgb = [0u8; 8];
        if let Some(ref s) = shape.rgb {
            let bytes = s.as_bytes();
            let len = bytes.len().min(7);
            rgb[..len].copy_from_slice(&bytes[..len]);
            if len < 8 { rgb[len] = 0; }
        }
        let scale = shape.scale.unwrap_or(NAN);
        let sides = shape.sides.unwrap_or(0);
        let star = shape.star.unwrap_or(false);
        ToolShapeInterOp {
            id,
            x,
            y,
            circle,
            opacity,
            rgb,
            rotation,
            scale,
            sides,
            star,
        }
    }

    pub fn to_toolshape(&self) -> ToolShape {
        let id = if self.id.iter().any(|&b| b != 0) {
            Some(Uuid::from_bytes(self.id))
        } else {
            None
        };
        let x = if self.x.is_nan() { None } else { Some(self.x) };
        let y = if self.y.is_nan() { None } else { Some(self.y) };
        let circle = if self.circle { Some(true) } else { None };
        let opacity = if self.opacity.is_nan() { None } else { Some(self.opacity) };
        let rgb = if self.rgb.iter().any(|&b| b != 0) {
            let len = self.rgb.iter().position(|&b| b == 0).unwrap_or(8);
            match std::str::from_utf8(&self.rgb[..len]) {
                Ok(s) => Some(s.to_string()),
                Err(_) => None,
            }
        } else {
            None
        };
        let rotation = if self.rotation.is_nan() { None } else { Some(self.rotation) };
        let scale = if self.scale.is_nan() { None } else { Some(self.scale) };
        let sides = if self.sides == 0 { None } else { Some(self.sides) };
        let star = if self.star { Some(true) } else { None };
        ToolShape {
            id,
            x,
            y,
            circle,
            opacity,
            rgb,
            rotation,
            scale,
            sides,
            star,
        }
    }
}