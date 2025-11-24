import React from "react";
import { ShapeBase } from "./ShapeBase";
import { ShapeState } from "./ShapeState";

export default class Circle extends React.Component<ShapeBase,ShapeState>
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
            console.error(`[Circle] (id: ${this.props.id}) Error in componentDidMount:`,error);
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
            console.error(`[Circle] (id: ${this.props.id}) Error in componentDidUpdate:`,error);
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
            console.error(`[Circle] (id: ${this.props.id}) Error in componentWillUnmount:`,error);
        }
    }

    render()
    {
        try
        {
            const { id, rgb, x, y, rotation, scale, onMouseDown, onTouchStart, onTouchEnd } = this.props;
            const containerStyle: React.CSSProperties = {
                position: "absolute",
                left: x ?? 0,
                top: y ?? 0,
                width: 100,
                height: 100,
                transform: `translate(-50%, -50%) rotate(${rotation ?? 0}deg) scale(${scale ?? 1})`,
                clipPath: "circle(50% at 50% 50%)"
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
                            borderRadius: "50%",
                            opacity: this.state.animatedOpacity,
                            transition: "opacity .5s"
                        }}
                    />
                </div>
            );
        }
        catch (error)
        {
            console.error(`[Circle] (id: ${this.props.id}) Error in render:`,error);
            return null;
        }
    }
}