import React from "react";
import CursorPosition from "../CursorPosition";

interface CursorOverlayProps
{
    cursors: Map<string, CursorPosition>;
}

const DOT_SIZE = 8;
const LABEL_OFFSET_X = 12;
const LABEL_OFFSET_Y = -4;

export default class CursorOverlay extends React.Component<CursorOverlayProps>
{
    render()
    {
        try
        {
            const nodes: React.ReactNode[] = [];

            this.props.cursors.forEach((cursor, connectionId) =>
            {
                if (cursor.x == null || cursor.y == null) { return; }

                const color = cursor.rgb || "#888";
                const name = cursor.name || "";

                nodes.push(
                    <div
                        key={connectionId}
                        style={{
                            position: "absolute",
                            left: cursor.x,
                            top: cursor.y,
                            pointerEvents: "none",
                            zIndex: 9999,
                            transform: "translate(-50%, -50%)",
                        }}
                    >
                        <div
                            style={{
                                width: DOT_SIZE,
                                height: DOT_SIZE,
                                borderRadius: "50%",
                                backgroundColor: color,
                                boxShadow: `0 0 6px ${color}`,
                                opacity: 0.85,
                            }}
                        />
                        {name && (
                            <div
                                style={{
                                    position: "absolute",
                                    left: LABEL_OFFSET_X,
                                    top: LABEL_OFFSET_Y,
                                    color: color,
                                    fontSize: "11px",
                                    fontFamily: "monospace",
                                    whiteSpace: "nowrap",
                                    opacity: 0.7,
                                    textShadow: "0 0 4px rgba(0,0,0,0.8)",
                                    userSelect: "none",
                                }}
                            >
                                {name}
                            </div>
                        )}
                    </div>
                );
            });

            return <>{nodes}</>;
        }
        catch (error)
        {
            console.error('[CursorOverlay] Error in render:', error);
            return null;
        }
    }
}
