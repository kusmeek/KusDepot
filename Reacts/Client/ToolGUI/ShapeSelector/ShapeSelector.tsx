import * as React from "react";
import ToolShape from "../ToolShape";

interface ShapeSelectorState
{
    form: ToolShape;
    scale: number;
}

interface ShapeSelectorProps
{
    onRandomize: () => void;
    toolShape: ToolShape | null;
    onDone: (toolShape: ToolShape) => void;
    forwardedRef?: React.RefObject<HTMLFormElement>;
}

export default class ShapeSelector extends React.Component<ShapeSelectorProps,ShapeSelectorState>
{
    constructor(props: ShapeSelectorProps)
    {
        super(props);

        this.state = {
            form: props.toolShape || {},
            scale: 4 / window.devicePixelRatio
        };

        this.updateScale = this.updateScale.bind(this);
    }

    componentDidMount()
    {
        try
        {
            this.updateScale();
            window.addEventListener('resize',this.updateScale);
            window.addEventListener('orientationchange',this.updateScale);
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in componentDidMount:`,error);
        }
    }

    componentWillUnmount()
    {
        try
        {
            window.removeEventListener('resize',this.updateScale);
            window.removeEventListener('orientationchange',this.updateScale);
            this.props.onDone(this.state.form);
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in componentWillUnmount:`,error);
        }
    }

    componentDidUpdate(prevProps: ShapeSelectorProps)
    {
        try
        {
            if (prevProps.toolShape !== this.props.toolShape)
            {
                this.setState({ form: this.props.toolShape || {} });
            }
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in componentDidUpdate:`,error);
        }
    }

    private updateScale()
    {
        try
        {
            this.setState({ scale: 1 / window.devicePixelRatio });
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in updateScale:`,error);
        }
    }

    private handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    {
        try
        {
            const target = e.target;
            const name = target.name;
            const type = target.type;
            let val: any = (type === "checkbox") ? (target as HTMLInputElement).checked : target.value;

            if (["rotation", "opacity", "scale", "sides"].indexOf(name) !== -1)
            {
                val = val === "" ? null : Number(val);
            }

            this.setState(prevState =>
            {
                const updated = { ...prevState.form } as any;

                if (name === "star")
                {
                    updated.star = val ? true : false;
                    if (val)
                    {
                        updated.circle = false;
                    }
                }
                else if (name === "circle")
                {
                    updated.circle = val ? true : false;
                    if (val)
                    {
                        updated.star = false; updated.sides = null;
                    }
                }
                else
                {
                    updated[name] = val;
                }

                return { form: updated };
            });
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in handleChange:`,error);
        }
    };

    private handleResetField = (field: string) =>
    {
        try
        {
            this.setState(prevState => {
                let newValue: any = null;
                return { form: { ...prevState.form, [field]: newValue } };
            });
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in handleResetField for ${field}:`,error);
        }
    };

    private handleRandomize = () =>
    {
        try
        {
            this.props.onRandomize();
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in handleRandomize:`,error);
        }
    };

    render()
    {
        try
        {
            const { form, scale } = this.state;
            return (
                <div style={{
                    position: "fixed", top: 0, left: 0, width: "100vw", height: "100vh", zIndex: 1000,
                    background: "rgba(0,0,0,0)", display: "flex", alignItems: "center", justifyContent: "center"
                }}>
                    <form ref={this.props.forwardedRef} style={{
                        background: "#111", padding: 24, borderRadius: 32, minWidth: 480, color: "#fff", boxShadow: "0 0 24px #000",
                        transform: `scale(${scale})`, transformOrigin: "center"
                    }}>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 18 }}>
                            <h3 style={{ margin: 0 }}>Shape Selector</h3>
                        </div>
                        {!form.circle && (
                            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 12, width: "100%" }}>
                                <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("star")}>Star:</span>
                                <input name="star" type="checkbox" checked={!!form.star} onChange={this.handleChange} style={{ marginLeft: 8 }} />
                            </div>
                        )}
                        {!form.star && (
                            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 12, width: "100%" }}>
                                <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("circle")}>Circle:</span>
                                <input name="circle" type="checkbox" checked={!!form.circle} onChange={this.handleChange} style={{ marginLeft: 8 }} />
                            </div>
                        )}
                        <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 12, width: "100%" }}>
                            <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("rgb")}>Color:</span>
                            <input name="rgb" type="color" value={form.rgb || "#ffffff"} onChange={this.handleChange} style={{ marginLeft: 8 }} />
                        </div>
                        <div style={{ display: "flex", alignItems: "center", marginBottom: 12, width: "100%" }}>
                            <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("opacity")}>Opacity:</span>
                            <input name="opacity" type="range" min="0" max="1" step="0.01" value={form.opacity ?? 0} onChange={this.handleChange} style={{ flex: 1, margin: "0 8px" }} />
                            <span style={{ minWidth: 32, textAlign: "right" }}>{form.opacity ?? 0}</span>
                        </div>
                        <div style={{ display: "flex", alignItems: "center", marginBottom: 12, width: "100%" }}>
                            <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("rotation")}>Rotation:</span>
                            <input name="rotation" type="range" min="0" max="360" step=".01" value={form.rotation ?? 0} onChange={this.handleChange} style={{ flex: 1, margin: "0 8px" }} />
                            <span style={{ minWidth: 32, textAlign: "right" }}>{form.rotation ?? 0}</span>
                        </div>
                        <div style={{ display: "flex", alignItems: "center", marginBottom: 12, width: "100%" }}>
                            <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("scale")}>Scale:</span>
                            <input name="scale" type="range" min="0.1" max="10" step="0.01" value={form.scale ?? 0} onChange={this.handleChange} style={{ flex: 1, margin: "0 8px" }} />
                            <span style={{ minWidth: 32, textAlign: "right" }}>{form.scale ?? 0}</span>
                        </div>
                        {!form.circle && (
                            <div style={{ display: "flex", alignItems: "center", marginBottom: 12, width: "100%" }}>
                                <span style={{ minWidth: 70, cursor: "pointer" }} onClick={() => this.handleResetField("sides")}>{form.star ? 'Points:' : 'Sides:'}</span>
                                <input name="sides" type="range" min="3" max="100" step="1" value={form.sides ?? 0} onChange={this.handleChange} style={{ flex: 1, margin: "0 8px" }} />
                                <span style={{ minWidth: 32, textAlign: "right" }}>{form.sides ?? 0}</span>
                            </div>
                        )}
                        <div style={{ display: "flex", justifyContent: "center", gap: 12, marginTop: 18 }}>
                            <button type="button" onClick={this.handleRandomize}>Randomize</button>
                        </div>
                    </form>
                </div>
            );
        }
        catch (error)
        {
            console.error(`[ShapeSelector] Error in render:`,error);
            return null;
        }
    }
}