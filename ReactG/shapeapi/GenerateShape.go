package shapeapi

import (
	"log"
	"math"
	"crypto/rand"
	"encoding/hex"
	"github.com/google/uuid"
)

func GenerateShape(input *ToolShape) *ToolShape {
	random, err := randomToolShape()
	if err != nil {
		log.Println("GenerateShapeFail", err)
		return nil
	}

	result := &ToolShape{
		ID:       chooseUUID(input.ID, random.ID),
		X:        chooseFloat64(input.X, random.X),
		Y:        chooseFloat64(input.Y, random.Y),
		Circle:   chooseBool(input.Circle, random.Circle),
		Opacity:  chooseFloat64(input.Opacity, random.Opacity),
		RGB:      chooseString(input.RGB, random.RGB),
		Rotation: chooseFloat64(input.Rotation, random.Rotation),
		Scale:    chooseFloat64(input.Scale, random.Scale),
		Sides:    chooseInt(input.Sides, random.Sides),
		Star:     chooseBool(input.Star, random.Star),
	}

	return result
}

func randomCircle() (*bool, error) {
	bytes := make([]byte, 1)
	if _, err := rand.Read(bytes); err != nil {
		return nil, err
	}
	val := bytes[0] < 25
	return &val, nil
}

func randomColor() (*string, error) {
	bytes := make([]byte, 3)
	if _, err := rand.Read(bytes); err != nil {
		return nil, err
	}
	color := "#" + hex.EncodeToString(bytes)
	return &color, nil
}

func randomCoordinate(max int) (*float64, error) {
	if max <= 0 {
		zero := 0.0
		return &zero, nil
	}
	val, err := randomFloat64()
	if err != nil {
		return nil, err
	}
	result := val * float64(max)
	return &result, nil
}

func randomFloat64() (float64, error) {
	bytes := make([]byte, 8)
	if _, err := rand.Read(bytes); err != nil {
		return 0, err
	}
	u := uint64(0)
	for i := 0; i < 8; i++ {
		u = (u << 8) | uint64(bytes[i])
	}
	return float64(u) / float64(math.MaxUint64), nil
}

func randomOpacity(min, max float64) (*float64, error) {
	val, err := randomFloat64()
	if err != nil {
		return nil, err
	}
	result := min + (max-min)*val
	return &result, nil
}

func randomRotation() (*float64, error) {
	val, err := randomFloat64()
	if err != nil {
		return nil, err
	}
	result := val * 360.0
	return &result, nil
}

func randomScale(min, max float64) (*float64, error) {
	val, err := randomFloat64()
	if err != nil {
		return nil, err
	}
	result := min + (max-min)*val
	return &result, nil
}

func randomSides(min, max int) (*int, error) {
	if min > max {
		twelve := 12
		return &twelve, nil
	}
	bytes := make([]byte, 4)
	if _, err := rand.Read(bytes); err != nil {
		return nil, err
	}
	val := int(bytes[0])
	result := (val % (max - min + 1)) + min
	return &result, nil
}

func randomStar() (*bool, error) {
	bytes := make([]byte, 1)
	if _, err := rand.Read(bytes); err != nil {
		return nil, err
	}
	val := bytes[0] < 50
	return &val, nil
}

func randomToolShape() (*ToolShape, error) {
	id, err := uuid.NewRandom()
	if err != nil {
		log.Println("NewRandomIdFail", err)
		return nil, err
	}
	x, err := randomCoordinate(7648)
	if err != nil {
		log.Println("RandomCoordinateFail", err)
		return nil, err
	}
	y, err := randomCoordinate(4304)
	if err != nil {
		log.Println("RandomCoordinateFail", err)
		return nil, err
	}
	circle, err := randomCircle()
	if err != nil {
		log.Println("RandomCircleFail", err)
		return nil, err
	}
	opacity, err := randomOpacity(0.0, 1.0)
	if err != nil {
		log.Println("RandomOpacityFail", err)
		return nil, err
	}
	rgb, err := randomColor()
	if err != nil {
		log.Println("RandomColorFail", err)
		return nil, err
	}
	rotation, err := randomRotation()
	if err != nil {
		log.Println("RandomRotationFail", err)
		return nil, err
	}
	scale, err := randomScale(0.1, 2.0)
	if err != nil {
		log.Println("RandomScaleFail", err)
		return nil, err
	}
	sides, err := randomSides(3, 8)
	if err != nil {
		log.Println("RandomSidesFail", err)
		return nil, err
	}
	star, err := randomStar()
	if err != nil {
		log.Println("RandomStarFail", err)
		return nil, err
	}
	return &ToolShape{
		ID:       &id,
		X:        x,
		Y:        y,
		Circle:   circle,
		Opacity:  opacity,
		RGB:      rgb,
		Rotation: rotation,
		Scale:    scale,
		Sides:    sides,
		Star:     star,
	}, nil
}

func chooseUUID(a, b *uuid.UUID) *uuid.UUID {
	if a != nil {
		return a
	}
	return b
}

func chooseString(a, b *string) *string {
	if a != nil {
		return a
	}
	return b
}

func chooseFloat64(a, b *float64) *float64 {
	if a != nil {
		return a
	}
	return b
}

func chooseInt(a, b *int) *int {
	if a != nil {
		return a
	}
	return b
}

func chooseBool(a, b *bool) *bool {
	if a != nil {
		return a
	}
	return b
}