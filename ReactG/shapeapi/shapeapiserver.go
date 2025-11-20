package shapeapi

import (
	"log"
	"context"
	"github.com/google/uuid"
	"kusdepot/reactg/shapeapi_pb"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
)

type shapeAPIServer struct {
	shapeapi_pb.UnimplementedShapeAPIServer
}

func NewShapeAPIServer() *shapeAPIServer {
	return &shapeAPIServer{}
}

func (s *shapeAPIServer) GenerateShape(ctx context.Context, req *shapeapi_pb.ToolShape) (*shapeapi_pb.ToolShape, error) {
	defer func() {
		if r := recover(); r != nil {
			log.Printf("[ReactG-GrpcServer] GenerateShape: %v", r)
		}
	}()
	shape := pbToToolShape(req)
	if shape == nil {
		return nil, status.Error(codes.InvalidArgument, "pbToToolShape")
	}
	result := GenerateShape(shape)
	if result == nil {
		return nil, status.Error(codes.Internal, "GenerateShape")
	}
	return toolShapeToPb(result), nil
}

func pbToToolShape(p *shapeapi_pb.ToolShape) *ToolShape {
	if p == nil {
		return nil
	}
	var idPtr *uuid.UUID
	if p.GetId() != "" {
		if id, err := uuid.Parse(p.GetId()); err == nil {
			idPtr = &id
		} else {
			log.Printf("[ReactG-GrpcServer] pbToToolShape: %v", err)
		}
	}
	return &ToolShape{
		ID:       idPtr,
		X:        float64PtrOrNil(p.GetX()),
		Y:        float64PtrOrNil(p.GetY()),
		Circle:   boolPtrOrNil(p.GetCircle()),
		Opacity:  float64PtrOrNil(p.GetOpacity()),
		RGB:      stringPtrOrNil(p.GetRgb()),
		Rotation: float64PtrOrNil(p.GetRotation()),
		Scale:    float64PtrOrNil(p.GetScale()),
		Sides:    intPtrOrNil(int(p.GetSides())),
		Star:     boolPtrOrNil(p.GetStar()),
	}
}

func toolShapeToPb(i *ToolShape) *shapeapi_pb.ToolShape {
	if i == nil {
		return nil
	}
	var idPtr *string
	if i.ID != nil {
		s := i.ID.String()
		idPtr = &s
	}
	return &shapeapi_pb.ToolShape{
		Id:       idPtr,
		X:        i.X,
		Y:        i.Y,
		Circle:   i.Circle,
		Opacity:  i.Opacity,
		Rgb:      i.RGB,
		Rotation: i.Rotation,
		Scale:    i.Scale,
		Sides:    toInt32Ptr(i.Sides),
		Star:     i.Star,
	}
}

func stringPtrOrNil(s string) *string {
	if s == "" {
		return nil
	}
	return &s
}

func float64PtrOrNil(f float64) *float64 {
	if f == 0 {
		return nil
	}
	return &f
}

func boolPtrOrNil(b bool) *bool {
	if !b {
		return nil
	}
	return &b
}

func intPtrOrNil(i int) *int {
	if i == 0 {
		return nil
	}
	return &i
}

func toInt32Ptr(i *int) *int32 {
	if i == nil {
		return nil
	}
	val := int32(*i)
	return &val
}