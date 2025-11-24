package shapeapi

import (
	"github.com/google/uuid"
)

type ToolShape struct {
	ID       *uuid.UUID `json:"id,omitempty"`
	X        *float64   `json:"x,omitempty"`
	Y        *float64   `json:"y,omitempty"`
	Circle   *bool      `json:"circle,omitempty"`
	Opacity  *float64   `json:"opacity,omitempty"`
	RGB      *string    `json:"rgb,omitempty"`
	Rotation *float64   `json:"rotation,omitempty"`
	Scale    *float64   `json:"scale,omitempty"`
	Sides    *int       `json:"sides,omitempty"`
	Star     *bool      `json:"star,omitempty"`
}