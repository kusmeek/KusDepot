import React from "react";
import { ShapeBase } from "./ShapeBase";
import { ShapeState } from "./ShapeState";

export default class Polygon extends React.Component<ShapeBase,ShapeState>
{
    private fadeTimeout: any = null;

    constructor(props: ShapeBase)
    {
        super(props); this.state = { animatedOpacity: 0 };
    }

    componentDidMount()
    {
        try
        {
            this.fadeTimeout = setTimeout(() => {
                this.setState({ animatedOpacity: this.props.opacity ?? 1 });
            }, 10);
        }
        catch (error)
        {
            console.error(`[Polygon] (id: ${this.props.id}) Error in componentDidMount:`,error);
        }
    }

    componentDidUpdate(prevProps: ShapeBase)
    {
        try
        {
            if (prevProps.opacity !== this.props.opacity)
            {
                this.setState({ animatedOpacity: this.props.opacity ?? 1 });
            }
        }
        catch (error)
        {
            console.error(`[Polygon] (id: ${this.props.id}) Error in componentDidUpdate:`,error);
        }
    }

    componentWillUnmount()
    {
        try
        {
            if (this.fadeTimeout)
            {
                clearTimeout(this.fadeTimeout);
            }
        }
        catch (error)
        {
            console.error(`[Polygon] (id: ${this.props.id}) Error in componentWillUnmount:`,error);
        }
    }

    private getPolygonClipPathPoints(sides: number): string
    {
        try
        {
            const points: string[] = [];
            const angleOffset = -90;
            for (let i = 0; i < sides; i++) {
                const angle = (2 * Math.PI * i) / sides + (angleOffset * Math.PI / 180);
                const x = 50 + 50 * Math.cos(angle);
                const y = 50 + 50 * Math.sin(angle);
                points.push(`${x}% ${y}%`);
            }
            return `polygon(${points.join(", ")})`;
        }
        catch (error)
        {
            console.error(`[Polygon] (id: ${this.props.id}) Error in getPolygonClipPathPoints:`,error);
            return "none";
        }
    }

    render()
    {
        try
        {
            const { id, rgb, x, y, rotation, scale, sides, onMouseDown, onTouchStart, onTouchEnd } = this.props;
            const containerStyle: React.CSSProperties = {
                position: "absolute",
                left: x ?? 0,
                top: y ?? 0,
                width: 100,
                height: 100,
                transform: `translate(-50%, -50%) rotate(${rotation ?? 0}deg) scale(${scale ?? 1})`,
                clipPath: this.getPolygonClipPathPoints(sides)
            };
            return (
                <div
                    id={`shape-${id}`}
                    style={containerStyle}
                    onTouchEnd={onTouchEnd}
                    onMouseDown={onMouseDown}
                    onTouchStart={onTouchStart}
                    onClick={e => e.stopPropagation()}
                    draggable={false}
                >
                    <div
                        style={{
                            width: "100%",
                            height: "100%",
                            backgroundColor: rgb ?? "#000",
                            opacity: this.state.animatedOpacity,
                            transition: "opacity .5s"
                        }}
                    />
                </div>
            );
        }
        catch (error)
        {
            console.error(`[Polygon] (id: ${this.props.id}) Error in render:`,error);
            return null;
        }
    }
}