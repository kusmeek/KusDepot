use rand::Rng;
use uuid::Uuid;

use anyhow::{Result, anyhow};
use crate::shapesvcs::toolshape::ToolShape;

pub struct ShapeAPI;

impl ShapeAPI {
    pub fn generate_shape(&self, input: Option<ToolShape>) -> Result<ToolShape> {
        let result = std::panic::catch_unwind(|| {
            let base = input.unwrap_or(ToolShape {
                id: None,
                x: None,
                y: None,
                circle: None,
                opacity: None,
                rgb: None,
                rotation: None,
                scale: None,
                sides: None,
                star: None,
            });
            let shape = ToolShape {
                id: base.id.or(Some(Uuid::new_v4())),
                x: base.x.or(Some(Self::random_coordinate(7648.0))),
                y: base.y.or(Some(Self::random_coordinate(4304.0))),
                circle: base.circle.or(Some(Self::random_circle())),
                opacity: base.opacity.or(Some(Self::random_opacity())),
                rgb: base.rgb.or(Some(Self::random_color())),
                rotation: base.rotation.or(Some(Self::random_rotation())),
                scale: base.scale.or(Some(Self::random_scale(2.0))),
                sides: base.sides.or(Some(Self::random_sides(3, 8))),
                star: base.star.or(Some(Self::random_star())),
            };
            shape
        });
        match result {
            Ok(shape) => Ok(shape),
            Err(_) => Err(anyhow!("generate_shape failed")),
        }
    }

    fn random_circle() -> bool {
        let mut rng = rand::rng();
        rng.random_bool(25.0/256.0)
    }

    fn random_color() -> String {
        let mut rng = rand::rng();
        let r: u8 = rng.random();
        let g: u8 = rng.random();
        let b: u8 = rng.random();
        format!("#{:02X}{:02X}{:02X}", r, g, b)
    }

    fn random_coordinate(max: f64) -> f64 {
        let mut rng = rand::rng();
        rng.random_range(0.0..max)
    }

    fn random_opacity() -> f64 {
        let mut rng = rand::rng();
        rng.random_range(0.0..=1.0)
    }

    fn random_rotation() -> f64 {
        let mut rng = rand::rng();
        rng.random_range(0.0..360.0)
    }

    fn random_scale(max: f64) -> f64 {
        let mut rng = rand::rng();
        rng.random_range(0.1..=max)
    }

    fn random_sides(min: i32, max: i32) -> i32 {
        let mut rng = rand::rng();
        rng.random_range(min..=max)
    }

    fn random_star() -> bool {
        let mut rng = rand::rng();
        rng.random_bool(50.0/256.0)
    }
}