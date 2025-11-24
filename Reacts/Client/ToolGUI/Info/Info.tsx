import * as React from "react";

interface InfoState
{
    scale: number;
}

interface InfoProps
{
    forwardedRef?: React.RefObject<HTMLFormElement>;
}

export default class Info extends React.Component<InfoProps,InfoState>
{
    constructor(props: InfoProps)
    {
        super(props);

        this.state = {
            scale: 1 / window.devicePixelRatio
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
            console.error(`[Info] Error in componentDidMount:`,error);
        }
    }

    componentWillUnmount()
    {
        try
        {
            window.removeEventListener('resize',this.updateScale);
            window.removeEventListener('orientationchange',this.updateScale);
        }
        catch (error)
        {
            console.error(`[Info] Error in componentWillUnmount:`,error);
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
            console.error(`[Info] Error in updateScale:`,error);
        }
    }

    render()
    {
        try
        {
            const { scale } = this.state;
            return (
                <div style={{
                    position: "fixed", top: 0, left: 0, width: "100vw", height: "100vh", zIndex: 1001,
                    background: "rgba(0,0,0,0)", display: "flex", alignItems: "center", justifyContent: "center"
                }}>
                    <form ref={this.props.forwardedRef} style={{
                        background: "#111", padding: 24, borderRadius: 32, minWidth: 480, color: "#fff", boxShadow: "0 0 24px #000",
                        transform: `scale(${scale})`, transformOrigin: "center"
                    }}>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 18 }}>
                            <h3 style={{ margin: 0 }}>
                                <a href="https://github.com/kusmeek/KusDepot" target="_blank" rel="noopener noreferrer" style={{ color: "#fff", textDecoration: "none" }}>
                                    KusDepot
                                </a>
                            </h3>
                        </div>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 8 }}>
                            <span style={{ fontSize: 16 }}>~Tilde to Talk~</span>
                        </div>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 8 }}>
                            <span style={{ fontSize: 16 }}>Zoom Out (Ctrl +/-)</span>
                        </div>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 8 }}>
                            <span style={{ fontSize: 16 }}>Reactor Control 0-9, Delete</span>
                        </div>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 8 }}>
                            <span style={{ fontSize: 16 }}>Spacebar to Select your Shape</span>
                        </div>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginBottom: 8 }}>
                            <span style={{ fontSize: 16 }}>Move/Randomize/Remove with Mouse</span>
                        </div>
                        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", width: "100%", marginTop: 16 }}>
                            <a href="https://www.linkedin.com/in/kusmeek/" target="_blank" rel="noopener noreferrer" style={{ color: "#fff", fontWeight: 500, fontSize: 18, textDecoration: "none" }}>
                                React/TS with Live SignalR.NET by Mike Abrahams
                            </a>
                        </div>
                    </form>
                </div>
            );
        }
        catch (error)
        {
            console.error(`[Info] Error in render:`,error);
            return null;
        }
    }
}