import * as React from "react";

interface InfoState
{
    scale: number;
}

interface InfoProps
{
    forwardedRef?: React.RefObject<HTMLFormElement | null>;
    showChat: boolean;
    showCursors: boolean;
    shimmerEnabled: boolean;
    soundMuted: boolean;
    trailsEnabled: boolean;
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
            const rows = [
                { key: "I", action: "Info", status: "On" },
                { key: "`", action: "Chat", status: this.props.showChat ? "On" : "Off" },
                { key: "M", action: "Sound", status: this.props.soundMuted ? "Muted" : "On" },
                { key: "T", action: "Shape trails", status: this.props.trailsEnabled ? "On" : "Off" },
                { key: "C", action: "User cursors", status: this.props.showCursors ? "On" : "Off" },
                { key: "S", action: "Shape shimmer", status: this.props.shimmerEnabled ? "On" : "Off" },
                { key: "0-9", action: "Reactor speed", status: "Active" },
                { key: "Space", action: "Shape selector", status: "Active" },
                { key: "Escape", action: "Clear selection", status: "Active" },
                { key: "Delete", action: "Purge shapes", status: "Active" },
                { key: "Shift+Click", action: "Group move", status: "Active" }
            ];

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
                        <div style={{ display: "flex", flexDirection: "column", gap: 8, width: "100%", marginBottom: 12 }}>
                            {rows.map(row => (
                                <div key={row.key} style={{ display: "grid", gridTemplateColumns: "120px 1fr auto", alignItems: "center", columnGap: 20, width: "100%", fontSize: 16 }}>
                                    <span style={{ color: "#8fd3ff", fontFamily: "monospace" }}>{row.key}</span>
                                    <span>{row.action}</span>
                                    <span style={{ color: "#aaa", fontFamily: "monospace" }}>{row.status}</span>
                                </div>
                            ))}
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