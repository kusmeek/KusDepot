import React from "react";
import { ShapeBase } from "./ShapeBase";
import { ShapeState } from "./ShapeState";

export default class Star extends React.Component<ShapeBase, ShapeState>
{
    private fadeTimeout: any = null;

    constructor(props: ShapeBase)
    {
        super(props);
        this.state = { animatedOpacity: 0 };
    }

    componentDidMount()
    {
        try
        {
            this.fadeTimeout = setTimeout(() =>
            {
                this.setState({ animatedOpacity: this.props.opacity ?? 1 });
            }, 10);
        }
        catch (error)
        {
            console.error(`[Star] (id: ${this.props.id}) Error in componentDidMount:`, error);
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
            console.error(`[Star] (id: ${this.props.id}) Error in componentDidUpdate:`, error);
        }
    }

    componentWillUnmount()
    {
        try
        {
            if (this.fadeTimeout) { clearTimeout(this.fadeTimeout); }
        }
        catch (error)
        {
            console.error(`[Star] (id: ${this.props.id}) Error in componentWillUnmount:`, error);
        }
    }

    private computeInnerRatio(points: number): number
    {
        try
        {
            if (points < 3) { return 0.5; }
            let geometricRatio = Math.cos((2 * Math.PI) / points) / Math.cos(Math.PI / points);
            if (!isFinite(geometricRatio) || geometricRatio <= 0.25)
            {
                geometricRatio = 0.25;
            }
            return geometricRatio;
        }
        catch
        {
            return 0.5;
        }
    }

    private buildClipPath(points: number): string
    {
        try
        {
            const p = Math.max(2, points);
            const innerR = this.computeInnerRatio(p);
            const verts: string[] = [];
            const outerRadius = 50;
            const innerRadius = outerRadius * innerR;
            const startAngle = -Math.PI / 2;
            const angleIncrement = Math.PI / p;
            const vertexCount = p * 2;
            for (let i = 0; i < vertexCount; i++)
            {
                const angle = startAngle + angleIncrement * i;
                const r = (i % 2 === 0) ? outerRadius : innerRadius;
                const x = 50 + r * Math.cos(angle);
                const y = 50 + r * Math.sin(angle);
                verts.push(`${x}% ${y}%`);
            }
            return `polygon(${verts.join(", ")})`;
        }
        catch (error)
        {
            console.error(`[Star] (id: ${this.props.id}) Error in buildClipPath:`, error);
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
                clipPath: this.buildClipPath(sides)
            };

            return (
                <div
                    id={`shape-${id}`}
                    style={containerStyle}
                    onMouseDown={onMouseDown}
                    onTouchStart={onTouchStart}
                    onTouchEnd={onTouchEnd}
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
            console.error(`[Star] (id: ${this.props.id}) Error in render:`, error);
            return null;
        }
    }
}