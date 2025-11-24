import uuid
from typing import Optional
from reactp import shapeapi_pb2
from dataclasses import dataclass

@dataclass
class ToolShape:
    id: Optional[uuid.UUID] = None
    x: Optional[float] = None
    y: Optional[float] = None
    circle: Optional[bool] = None
    opacity: Optional[float] = None
    rgb: Optional[str] = None
    rotation: Optional[float] = None
    scale: Optional[float] = None
    sides: Optional[int] = None
    star: Optional[bool] = None

    def to_dict(self):
        return {
            "id": str(self.id) if self.id else None,
            "x": self.x,
            "y": self.y,
            "circle": self.circle,
            "opacity": self.opacity,
            "rgb": self.rgb,
            "rotation": self.rotation,
            "scale": self.scale,
            "sides": self.sides,
            "star": self.star,
        }

    @staticmethod
    def from_dict(data):
        return ToolShape(
            id=uuid.UUID(data["id"]) if data.get("id") else None,
            x=data.get("x"),
            y=data.get("y"),
            circle=data.get("circle"),
            opacity=data.get("opacity"),
            rgb=data.get("rgb"),
            rotation=data.get("rotation"),
            scale=data.get("scale"),
            sides=data.get("sides"),
            star=data.get("star"),
        )

    @staticmethod
    def from_proto(proto: shapeapi_pb2.ToolShape) -> 'ToolShape':
        return ToolShape(
            id=uuid.UUID(proto.id) if proto.HasField('id') else None,
            x=proto.x if proto.HasField('x') else None,
            y=proto.y if proto.HasField('y') else None,
            circle=proto.circle if proto.HasField('circle') else None,
            opacity=proto.opacity if proto.HasField('opacity') else None,
            rgb=proto.rgb if proto.HasField('rgb') else None,
            rotation=proto.rotation if proto.HasField('rotation') else None,
            scale=proto.scale if proto.HasField('scale') else None,
            sides=proto.sides if proto.HasField('sides') else None,
            star=proto.star if proto.HasField('star') else None,
        )

    def to_proto(self) -> shapeapi_pb2.ToolShape:
        proto = shapeapi_pb2.ToolShape()
        if self.id is not None:
            proto.id = str(self.id)
        if self.x is not None:
            proto.x = self.x
        if self.y is not None:
            proto.y = self.y
        if self.circle is not None:
            proto.circle = self.circle
        if self.opacity is not None:
            proto.opacity = self.opacity
        if self.rgb is not None:
            proto.rgb = self.rgb
        if self.rotation is not None:
            proto.rotation = self.rotation
        if self.scale is not None:
            proto.scale = self.scale
        if self.sides is not None:
            proto.sides = self.sides
        if self.star is not None:
            proto.star = self.star
        return proto