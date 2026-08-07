import React from "react";

import starfield from "./Image/starfield.webp";
import ChatControl, { ChatItem } from "./ChatControl/ChatControl";
import CursorOverlay from "./CursorOverlay/CursorOverlay";
import Info from "./Info/Info";
import ReactorControl from "./ReactorControl/ReactorControl";
import ShapeMorph from "./ShapeMorph/ShapeMorph";
import ShapeSelector from "./ShapeSelector/ShapeSelector";
import ShapeShimmer from "./ShapeShimmer/ShapeShimmer";
import Circle from "./Shapes/Circle";
import "./Shapes/fadeout.css";
import Polygon from "./Shapes/Polygon";
import Star from "./Shapes/Star";
import SoundEngine from "./SoundEngine/SoundEngine";
import CursorPosition from "./CursorPosition";
import ToolMessage from "./ToolMessage";
import ToolShape from "./ToolShape";
import ToolShapePosition from "./ToolShapePosition";
import UpdateThrottle from "./UpdateThrottle";

type PointerEventLike = MouseEvent | TouchEvent | React.MouseEvent | React.TouchEvent;
type ShapeTemplateKey = "circle" | "opacity" | "rgb" | "rotation" | "scale" | "sides" | "star";
const shapeTemplateKeys: readonly ShapeTemplateKey[] = ["circle", "opacity", "rgb", "rotation", "scale", "sides", "star"];

interface TrailPosition
{
    x: number;
    y: number;
}

interface ToolGUIState
{
    chatItems: ChatItem[];
    chatName: string;
    cursors: Map<string, CursorPosition>;
    dragOffset: { x: number; y: number; } | null;
    draggingShapeId: string | null;
    groupDragOffsets: Map<string, { x: number; y: number }> | null;
    remoteTrailPositions: Map<string, TrailPosition[]>;
    selectedShapeIds: Set<string>;
    shapeSelectorValue: ToolShape | null;
    shapes: ToolShape[];
    shimmerEnabled: boolean;
    shimmerOffsets: Map<string, number>;
    soundMuted: boolean;
    showChat: boolean;
    showCursors: boolean;
    showInfo: boolean;
    showShapeSelector: boolean;
    trailPositions: TrailPosition[];
    trailsEnabled: boolean;
    userShapeTemplate: ToolShape | null;
}

const REMOTE_TRAIL_EXPIRE_MS = 150;
const REMOTE_TRAIL_SAMPLE_INTERVAL = 2;
const TRAIL_BASE_OPACITY = 0.35;
const TRAIL_LENGTH = 5;
const TRAIL_SAMPLE_INTERVAL = 3;

export default class ToolGUI extends React.Component<{}, ToolGUIState>
{
    private reactorControl: ReactorControl;
    private soundEngine: SoundEngine = new SoundEngine();
    private shapeMorph: ShapeMorph = new ShapeMorph();
    private shapeShimmer: ShapeShimmer = new ShapeShimmer();

    private chatRef: React.RefObject<HTMLDivElement | null>;
    private modalRef: React.RefObject<HTMLFormElement | null>;

    private dragInitiated: boolean = false;
    private dragStartTimer: ReturnType<typeof setTimeout> | null = null;
    private longPressTimer: ReturnType<typeof setTimeout> | null = null;
    private mouseDownPosition: { x: number; y: number; } | null = null;

    private throttledUpdateCursorPosition: (cursor: CursorPosition) => void;
    private throttledUpdateShapePosition: (position: ToolShapePosition) => void;
    private throttledUpdateShapePositions: (positions: ToolShapePosition[]) => void;

    private remoteTrailBuffers: Map<string, TrailPosition[]> = new Map();
    private remoteTrailSampleCounters: Map<string, number> = new Map();
    private remoteTrailTimers: Map<string, ReturnType<typeof setTimeout>> = new Map();
    private trailBuffer: TrailPosition[] = [];
    private trailSampleCounter: number = 0;

    constructor(props: {})
    {
        super(props);

        this.state = {
            shapes: [],
            chatName: "",
            chatItems: [],
            showInfo: true,
            showChat: false,
            dragOffset: null,
            draggingShapeId: null,
            userShapeTemplate: null,
            shapeSelectorValue: null,
            showShapeSelector: false,
            trailsEnabled: true,
            trailPositions: [],
            remoteTrailPositions: new Map(),
            shimmerEnabled: true,
            soundMuted: false,
            showCursors: true,
            cursors: new Map(),
            shimmerOffsets: new Map(),
            selectedShapeIds: new Set(),
            groupDragOffsets: null,
        };

        this.reactorControl = new ReactorControl();

        this.throttledUpdateShapePosition = UpdateThrottle((position: ToolShapePosition) =>
        {
            if (position.id) { this.reactorControl.updateShapePosition(position); }
        }, 10);

        this.throttledUpdateShapePositions = UpdateThrottle((positions: ToolShapePosition[]) =>
        {
            if (positions.length > 0) { this.reactorControl.updateShapePositions(positions); }
        }, 10);

        this.throttledUpdateCursorPosition = UpdateThrottle((cursor: CursorPosition) =>
        {
            this.reactorControl.updateCursorPosition(cursor);
        }, 50);

        this.modalRef = React.createRef<HTMLFormElement>();
        this.chatRef = React.createRef<HTMLDivElement>();
    }

    componentDidMount()
    {
        try
        {
            this.reactorControl.start();
            this.shapeMorph.start(this.handleMorphUpdate, this.handleMorphComplete);
            this.shapeShimmer.start(this.getShimmerEligibleIds, this.handleShimmer, this.handleShimmerEnd);
            window.addEventListener('keydown', this.handleKeyDown);
            this.reactorControl.onCorePurged(this.handleCorePurge);
            this.reactorControl.onShapeAdded(this.handleShapeAdded);
            this.reactorControl.onShapeRemoved(this.handleShapeRemoved);
            this.reactorControl.onShapeUpdated(this.handleShapeUpdated);
            this.reactorControl.onMessageReceived(this.handleMessageReceived);
            this.reactorControl.onShapePositionUpdated(this.handleShapePositionUpdated);
            this.reactorControl.onCursorPositionUpdated(this.handleCursorPositionUpdated);
            this.reactorControl.onCursorRemoved(this.handleCursorRemoved);
            document.addEventListener('mousemove', this.handleGlobalMouseMove);

            if (this.state.showShapeSelector || this.state.showInfo)
            {
                document.addEventListener('mousedown',this.handleDocumentMouseDown, true);
                document.addEventListener('keydown',this.handleModalKeyDown, true);
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in componentDidMount:',error);
        }
    }

    componentWillUnmount()
    {
        try
        {
            this.reactorControl.stop();
            this.shapeMorph.stop();
            this.shapeShimmer.stop();
            window.removeEventListener('keydown', this.handleKeyDown);
            document.removeEventListener('mousemove', this.handleGlobalMouseMove);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in componentWillUnmount:',error);
        }
    }

    componentDidUpdate(prevProps: {}, prevState: ToolGUIState)
    {
        try
        {
            const modalNowOpen = (this.state.showShapeSelector || this.state.showInfo);
            const modalWasOpen = (prevState.showShapeSelector || prevState.showInfo);
            if (modalNowOpen && !modalWasOpen)
            {
                document.addEventListener('mousedown',this.handleDocumentMouseDown,true);
                document.addEventListener('keydown',this.handleModalKeyDown,true);
            }
            else if (!modalNowOpen && modalWasOpen)
            {
                document.removeEventListener('mousedown',this.handleDocumentMouseDown,true);
                document.removeEventListener('keydown',this.handleModalKeyDown,true);
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in componentDidUpdate:',error);
        }
    }

    // === Shape Management === //
    private addShape(shape: ToolShape): void
    {
        try
        {
            this.reactorControl.addShape(shape);
        }
        catch (error)
        {
            console.error('[ToolGUI] Failed to add shape:',error);
        }
    }

    private handleShapeAdded = (shape: ToolShape) =>
    {
        try
        {
            this.setState(prevState => ({
                shapes: [...prevState.shapes, shape]
            }));
            this.soundEngine.playCreation(shape.star, shape.circle);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeAdded:',error);
        }
    };

    private removeShape(id: string | undefined | null): void
    {
        try
        {
            if (id && id === this.state.draggingShapeId)
            {
                this.cancelDrag();
            }
            if (id)
            {
                this.reactorControl.removeShape(id);
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Failed to remove shape:',error);
        }
    }

    private handleShapeRemoved = (id: string) =>
    {
        try
        {
            this.clearRemoteTrail(id);
            this.shapeMorph.cancelMorph(id);
            this.soundEngine.playRemoval();
            this.setState(prevState =>
            {
                const next = new Set(prevState.selectedShapeIds);
                next.delete(id);
                return { selectedShapeIds: next };
            });
            const el = document.getElementById(`shape-${id}`);
            if (el) { el.classList.add('fade-out'); }
            setTimeout(() =>
            {
                this.setState(prevState => ({
                    shapes: prevState.shapes.filter(shape => shape.id !== id)
                }));
            }, 2000);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeRemoved:',error);
        }
    };

    private handleShapeUpdated = (updatedShape: ToolShape) =>
    {
        try
        {
            this.setState(prevState =>
            {
                const currentShape = prevState.shapes.find(shape => shape.id === updatedShape.id);
                if (currentShape)
                {
                    this.shapeMorph.beginMorph(currentShape, updatedShape);
                    this.soundEngine.playRandomize();
                    return null;
                }
                else
                {
                    return {
                        shapes: [...prevState.shapes, updatedShape]
                    };
                }
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeUpdated:',error);
        }
    };

    private handleShapePositionUpdated = (position: ToolShapePosition) =>
    {
        try
        {
            if (!position.id) return;
            this.setState(prevState => ({
                shapes: prevState.shapes.map(shape =>
                    shape.id === position.id ? { ...shape, x: position.x, y: position.y } : shape
                )
            }));

            if (position.id !== this.state.draggingShapeId)
            {
                this.sampleRemoteTrailPosition(position.id, position.x ?? 0, position.y ?? 0);
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapePositionUpdated:',error);
        }
    };

    private handleShapeSelectorDone = (toolShape: ToolShape) =>
    {
        try
        {
            this.setState({
                showShapeSelector: false,
                userShapeTemplate: { ...toolShape },
                shapeSelectorValue: { ...toolShape }
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeSelectorDone:', error);
        }
    };

    private handleShapeSelectorRandomize = () =>
    {
        try
        {
            this.setState({
                userShapeTemplate: null,
                shapeSelectorValue: {}
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeSelectorRandomize:', error);
        }
    };

    private applyShapeTemplate(baseShape: ToolShape, template: ToolShape | null): ToolShape
    {
        const nextShape: ToolShape = { ...baseShape };

        if (!template) { return nextShape; }

        const nextShapeValues = nextShape as Record<ShapeTemplateKey, ToolShape[ShapeTemplateKey]>;
        const templateValues = template as Record<ShapeTemplateKey, ToolShape[ShapeTemplateKey]>;

        for (const key of shapeTemplateKeys)
        {
            if (templateValues[key] !== undefined)
            {
                nextShapeValues[key] = templateValues[key];
            }
        }

        return nextShape;
    }

    private handleShapeReMake = (shape: ToolShape) =>
    {
        try
        {
            const { id, x, y } = shape;
            const newShape = this.applyShapeTemplate({ id, x, y }, this.state.shapeSelectorValue);

            this.reactorControl.reMakeShape(newShape);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeReMake:', error);
        }
    };

    private handleCorePurge = () =>
    {
        try
        {
            this.clearAllRemoteTrails();
            this.shapeMorph.cancelAll();
            this.soundEngine.playPurge();
            this.setState({ selectedShapeIds: new Set() });
            const idsToPurge = new Set(this.state.shapes.map(shape => shape.id).filter((id): id is string => id != null));
            idsToPurge.forEach(id =>
            {
                const el = document.getElementById(`shape-${id}`);
                if (el) el.classList.add('fade-out');
            });
            setTimeout(() =>
            {
                this.setState(prevState => ({
                    shapes: prevState.shapes.filter(shape => !shape.id || !idsToPurge.has(shape.id))
                }));
            }, 2000);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleCorePurge:',error);
        }
    };

    // === Morph Management === //
    private handleMorphUpdate = (id: string, interpolated: ToolShape): void =>
    {
        try
        {
            this.setState(prevState => ({
                shapes: prevState.shapes.map(shape =>
                    shape.id === id ? { ...shape, ...interpolated } : shape
                )
            }));
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMorphUpdate:', error);
        }
    };

    private handleMorphComplete = (id: string, finalShape: ToolShape): void =>
    {
        try
        {
            this.setState(prevState => ({
                shapes: prevState.shapes.map(shape =>
                    shape.id === id ? { ...shape, ...finalShape } : shape
                )
            }));
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMorphComplete:', error);
        }
    };

    // === Trail Management === //
    private sampleTrailPosition(x: number, y: number): void
    {
        try
        {
            if (!this.state.trailsEnabled) { return; }

            this.trailSampleCounter++;
            if (this.trailSampleCounter % TRAIL_SAMPLE_INTERVAL !== 0) { return; }

            this.trailBuffer.push({ x, y });
            if (this.trailBuffer.length > TRAIL_LENGTH)
            {
                this.trailBuffer.splice(0, this.trailBuffer.length - TRAIL_LENGTH);
            }

            this.setState({ trailPositions: [...this.trailBuffer] });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in sampleTrailPosition:', error);
        }
    }

    private clearTrail(): void
    {
        try
        {
            this.trailBuffer = [];
            this.trailSampleCounter = 0;
            this.setState({ trailPositions: [] });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in clearTrail:', error);
        }
    }

    private sampleRemoteTrailPosition(shapeId: string, x: number, y: number): void
    {
        try
        {
            if (!this.state.trailsEnabled) { return; }

            const counter = (this.remoteTrailSampleCounters.get(shapeId) ?? 0) + 1;
            this.remoteTrailSampleCounters.set(shapeId, counter);
            if (counter % REMOTE_TRAIL_SAMPLE_INTERVAL !== 0) { return; }

            let buffer = this.remoteTrailBuffers.get(shapeId);
            if (!buffer)
            {
                buffer = [];
                this.remoteTrailBuffers.set(shapeId, buffer);
            }

            buffer.push({ x, y });
            if (buffer.length > TRAIL_LENGTH)
            {
                buffer.splice(0, buffer.length - TRAIL_LENGTH);
            }

            const existingTimer = this.remoteTrailTimers.get(shapeId);
            if (existingTimer) { clearTimeout(existingTimer); }

            this.remoteTrailTimers.set(shapeId, setTimeout(() =>
            {
                this.clearRemoteTrail(shapeId);
            }, REMOTE_TRAIL_EXPIRE_MS));

            this.setState(prevState =>
            {
                const next = new Map(prevState.remoteTrailPositions);
                next.set(shapeId, [...buffer!]);
                return { remoteTrailPositions: next };
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in sampleRemoteTrailPosition:', error);
        }
    }

    private clearRemoteTrail(shapeId: string): void
    {
        try
        {
            this.remoteTrailBuffers.delete(shapeId);
            this.remoteTrailTimers.delete(shapeId);
            this.remoteTrailSampleCounters.delete(shapeId);
            this.setState(prevState =>
            {
                const next = new Map(prevState.remoteTrailPositions);
                next.delete(shapeId);
                return { remoteTrailPositions: next };
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in clearRemoteTrail:', error);
        }
    }

    private clearAllRemoteTrails(): void
    {
        try
        {
            this.remoteTrailTimers.forEach(timer => clearTimeout(timer));
            this.remoteTrailTimers.clear();
            this.remoteTrailBuffers.clear();
            this.remoteTrailSampleCounters.clear();
            this.setState({ remoteTrailPositions: new Map() });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in clearAllRemoteTrails:', error);
        }
    }

    // === Interaction === //
    private startDrag = (shape: ToolShape, x: number, y: number) =>
    {
        try
        {
            this.dragInitiated = true;
            const { selectedShapeIds, shapes } = this.state;
            const useGroup = shape.id != null && selectedShapeIds.size > 1 && selectedShapeIds.has(shape.id);
            let groupOffsets: Map<string, { x: number; y: number }> | null = null;
            if (useGroup)
            {
                groupOffsets = new Map();
                for (const s of shapes)
                {
                    if (s.id && selectedShapeIds.has(s.id))
                    {
                        groupOffsets.set(s.id, { x: x - (s.x ?? 0), y: y - (s.y ?? 0) });
                    }
                }
            }
            this.setState({
                draggingShapeId: shape.id ?? null,
                dragOffset: { x: x - (shape.x ?? 0), y: y - (shape.y ?? 0) },
                groupDragOffsets: groupOffsets
            });
            document.addEventListener("mousemove", this.handleMouseMove);
            document.addEventListener("mouseup", this.handleMouseUp);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in startDrag:',error);
        }
    };

    private cancelDrag()
    {
        try
        {
            this.clearTrail();
            this.setState({ draggingShapeId: null, dragOffset: null, groupDragOffsets: null });
            document.removeEventListener("mousemove",this.handleMouseMove);
            document.removeEventListener("mouseup",this.handleMouseUp);
            document.removeEventListener("touchmove",this.handleTouchMove);
            document.removeEventListener("touchend",this.handleTouchEnd);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in cancelDrag:',error);
        }
    }

    private getPointerPosition = (event: PointerEventLike): { x: number; y: number; } =>
    {
        try
        {
            if ('touches' in event && event.touches.length > 0)
            {
                return { x: event.touches[0].clientX, y: event.touches[0].clientY };
            }
            if ('clientX' in event && 'clientY' in event)
            {
                return { x: event.clientX, y: event.clientY };
            }

            return { x: 0, y: 0 };
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in getPointerPosition:',error);
            return { x: 0, y: 0 };
        }
    };

    private handleShapeMouseDown = (event: React.MouseEvent, shape: ToolShape) =>
    {
        try
        {
            if (event.button === 2)
            {
                event.preventDefault();
                event.stopPropagation();
                const { selectedShapeIds } = this.state;
                if (shape.id && selectedShapeIds.size > 1 && selectedShapeIds.has(shape.id))
                {
                    const idsToRemove = [...selectedShapeIds];
                    idsToRemove.forEach(id => this.reactorControl.removeShape(id));
                    this.setState({ selectedShapeIds: new Set() });
                }
                else
                {
                    this.removeShape(shape.id);
                    if (shape.id)
                    {
                        this.setState(prevState =>
                        {
                            const next = new Set(prevState.selectedShapeIds);
                            next.delete(shape.id!);
                            return { selectedShapeIds: next };
                        });
                    }
                }
                return;
            }
            if (event.button === 1)
            {
                event.preventDefault();
                event.stopPropagation();
                if (shape.id)
                {
                    const { selectedShapeIds, shapes } = this.state;
                    const useGroup = selectedShapeIds.size > 1 && selectedShapeIds.has(shape.id);
                    const targets = useGroup
                        ? shapes.filter(s => s.id != null && selectedShapeIds.has(s.id!))
                        : [shape];
                    const caps = event.getModifierState('CapsLock');
                    for (const target of targets)
                    {
                        if (event.ctrlKey && event.shiftKey && event.altKey)
                        {
                            this.handleShapeReMake(target);
                        }
                        else if (event.ctrlKey && event.shiftKey && !caps)
                        {
                            this.reactorControl.randomizeShapeFG(target);
                        }
                        else if (event.ctrlKey && event.shiftKey && caps)
                        {
                            this.reactorControl.randomizeShapeFH(target);
                        }
                        else if (event.altKey && event.ctrlKey && !caps)
                        {
                            this.reactorControl.randomizeShapeNG(target);
                        }
                        else if (event.altKey && event.ctrlKey && caps)
                        {
                            this.reactorControl.randomizeShapeNH(target);
                        }
                        else if (event.altKey && event.shiftKey && !caps)
                        {
                            this.reactorControl.randomizeShapePG(target);
                        }
                        else if (event.altKey && event.shiftKey && caps)
                        {
                            this.reactorControl.randomizeShapePH(target);
                        }
                        else if (event.shiftKey && !caps)
                        {
                            this.reactorControl.randomizeShapeGG(target);
                        }
                        else if (event.shiftKey && caps)
                        {
                            this.reactorControl.randomizeShapeGH(target);
                        }
                        else if (event.altKey && !caps)
                        {
                            this.reactorControl.randomizeShapeJG(target);
                        }
                        else if (event.altKey && caps)
                        {
                            this.reactorControl.randomizeShapeJH(target);
                        }
                        else if (event.ctrlKey && !caps)
                        {
                            this.reactorControl.randomizeShapeRG(target);
                        }
                        else if (event.ctrlKey && caps)
                        {
                            this.reactorControl.randomizeShapeRH(target);
                        }
                        else
                        {
                            this.reactorControl.randomizeShape(target);
                        }
                    }
                }
                return;
            }
            if (event.button === 0)
            {
                event.preventDefault();
                event.stopPropagation();
                if (event.shiftKey && shape.id)
                {
                    this.setState(prevState =>
                    {
                        const next = new Set(prevState.selectedShapeIds);
                        if (next.has(shape.id!)) { next.delete(shape.id!); }
                        else { next.add(shape.id!); }
                        return { selectedShapeIds: next };
                    });
                    return;
                }
                const { x, y } = this.getPointerPosition(event);
                this.mouseDownPosition = { x, y };
                this.dragInitiated = false;
                this.dragStartTimer = setTimeout(() =>
                {
                    this.startDrag(shape, x, y);
                }, 10);
                document.addEventListener("mousemove",this.handleMouseMoveForDragStart);
                document.addEventListener("mouseup",this.handleMouseUpForDragStart);
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeMouseDown:',error);
        }
    };

    private handleMouseMoveForDragStart = (event: MouseEvent) =>
    {
        try
        {
            if (!this.mouseDownPosition) return;
            const { x, y } = this.getPointerPosition(event);
            const dx = x - this.mouseDownPosition.x;
            const dy = y - this.mouseDownPosition.y;
            if (Math.sqrt(dx * dx + dy * dy) > 5 && !this.dragInitiated)
            {
                if (this.dragStartTimer) { clearTimeout(this.dragStartTimer); }
                this.dragStartTimer = null;
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMouseMoveForDragStart:',error);
        }
    };

    private handleMouseUpForDragStart = () =>
    {
        try
        {
            if (this.dragStartTimer) { clearTimeout(this.dragStartTimer); }
            this.dragStartTimer = null;
            this.mouseDownPosition = null;
            document.removeEventListener("mousemove",this.handleMouseMoveForDragStart);
            document.removeEventListener("mouseup",this.handleMouseUpForDragStart);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMouseUpForDragStart:',error);
        }
    };

    private handleShapeTouchStart = (event: React.TouchEvent, shape: ToolShape) =>
    {
        try
        {
            event.stopPropagation();
            this.longPressTimer = setTimeout(() =>
            {
                const { x, y } = this.getPointerPosition(event);
                this.setState({
                    draggingShapeId: shape.id ?? null,
                    dragOffset: { x: x - (shape.x ?? 0), y: y - (shape.y ?? 0) }
                });
                document.addEventListener("touchmove",this.handleTouchMove, { passive: false });
                document.addEventListener("touchend",this.handleTouchEnd);
            }, 10);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeTouchStart:',error);
        }
    };

    private handleShapeTouchEnd = (event: React.TouchEvent) =>
    {
        try
        {
            event.stopPropagation();
            if (this.longPressTimer) { clearTimeout(this.longPressTimer); }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShapeTouchEnd:',error);
        }
    };

    private handleMouseMove = (event: MouseEvent) =>
    {
        try
        {
            const { draggingShapeId, dragOffset, shapes, groupDragOffsets } = this.state;
            if (!draggingShapeId || !dragOffset) return;
            const { x, y } = this.getPointerPosition(event);

            if (groupDragOffsets && groupDragOffsets.size > 1)
            {
                const positions: ToolShapePosition[] = [];
                const updatedShapes = shapes.map(shape =>
                {
                    if (!shape.id) return shape;
                    const offset = groupDragOffsets.get(shape.id);
                    if (!offset) return shape;
                    const nx = x - offset.x;
                    const ny = y - offset.y;
                    positions.push({ id: shape.id, x: nx, y: ny });
                    return { ...shape, x: nx, y: ny };
                });
                this.setState({ shapes: updatedShapes });
                this.throttledUpdateShapePositions(positions);
            }
            else
            {
                const newX = x - dragOffset.x;
                const newY = y - dragOffset.y;
                const updatedShapes = shapes.map(shape =>
                    shape.id === draggingShapeId
                        ? { ...shape, x: newX, y: newY }
                        : shape
                );
                this.setState({ shapes: updatedShapes });
                this.throttledUpdateShapePosition({ id: draggingShapeId, x: newX, y: newY });
            }

            const primaryX = x - dragOffset.x;
            const primaryY = y - dragOffset.y;
            this.sampleTrailPosition(primaryX, primaryY);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMouseMove:',error);
        }
    };

    private handleMouseUp = () =>
    {
        try
        {
            const { draggingShapeId, shapes, groupDragOffsets } = this.state;
            if (draggingShapeId)
            {
                if (groupDragOffsets && groupDragOffsets.size > 1)
                {
                    for (const id of groupDragOffsets.keys())
                    {
                        const shape = shapes.find(s => s.id === id);
                        if (shape) { this.reactorControl.updateShape(shape); }
                    }
                }
                else
                {
                    const shape = shapes.find(s => s.id === draggingShapeId);
                    if (shape) { this.reactorControl.updateShape(shape); }
                }
            }
            this.clearTrail();
            this.setState({ draggingShapeId: null, dragOffset: null, groupDragOffsets: null });
            document.removeEventListener("mousemove", this.handleMouseMove);
            document.removeEventListener("mouseup", this.handleMouseUp);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMouseUp:',error);
        }
    };

    private handleTouchMove = (event: TouchEvent) =>
    {
        try
        {
            event.preventDefault();
            const { draggingShapeId, dragOffset, shapes, groupDragOffsets } = this.state;
            if (!draggingShapeId || !dragOffset) return;
            const { x, y } = this.getPointerPosition(event);

            if (groupDragOffsets && groupDragOffsets.size > 1)
            {
                const positions: ToolShapePosition[] = [];
                const updatedShapes = shapes.map(shape =>
                {
                    if (!shape.id) return shape;
                    const offset = groupDragOffsets.get(shape.id);
                    if (!offset) return shape;
                    const nx = x - offset.x;
                    const ny = y - offset.y;
                    positions.push({ id: shape.id, x: nx, y: ny });
                    return { ...shape, x: nx, y: ny };
                });
                this.setState({ shapes: updatedShapes });
                this.throttledUpdateShapePositions(positions);
            }
            else
            {
                const newX = x - dragOffset.x;
                const newY = y - dragOffset.y;
                const updatedShapes = shapes.map(shape =>
                    shape.id === draggingShapeId
                        ? { ...shape, x: newX, y: newY }
                        : shape
                );
                this.setState({ shapes: updatedShapes });
                this.throttledUpdateShapePosition({ id: draggingShapeId, x: newX, y: newY });
            }

            const primaryX = x - dragOffset.x;
            const primaryY = y - dragOffset.y;
            this.sampleTrailPosition(primaryX, primaryY);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleTouchMove:',error);
        }
    };

    private handleTouchEnd = () =>
    {
        try
        {
            const { draggingShapeId, shapes, groupDragOffsets } = this.state;
            if (draggingShapeId)
            {
                if (groupDragOffsets && groupDragOffsets.size > 1)
                {
                    for (const id of groupDragOffsets.keys())
                    {
                        const shape = shapes.find(s => s.id === id);
                        if (shape) { this.reactorControl.updateShape(shape); }
                    }
                }
                else
                {
                    const shape = shapes.find(s => s.id === draggingShapeId);
                    if (shape) { this.reactorControl.updateShape(shape); }
                }
            }
            this.clearTrail();
            this.setState({ draggingShapeId: null, dragOffset: null, groupDragOffsets: null });
            document.removeEventListener("touchmove",this.handleTouchMove);
            document.removeEventListener("touchend",this.handleTouchEnd);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleTouchEnd:',error);
        }
    };

    private handleContainerMouseDown = (event: React.MouseEvent<HTMLDivElement>) =>
    {
        try
        {
            if (event.button !== 0) return;
            const { x, y } = this.getPointerPosition(event);
            this.mouseDownPosition = { x, y };
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleContainerMouseDown:',error);
        }
    };

    private handleContainerMouseUp = (event: React.MouseEvent<HTMLDivElement>) =>
    {
        try
        {
            if (event.button !== 0) return;
            if (!this.mouseDownPosition) return;
            if (this.state.draggingShapeId) return;
            if (this.modalRef.current) return;
            if (this.chatRef.current && this.chatRef.current.contains(event.target as Node)) return;
            const { x, y } = this.getPointerPosition(event);
            const dx = x - this.mouseDownPosition.x;
            const dy = y - this.mouseDownPosition.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            this.mouseDownPosition = null;
            if (distance > 250) return;
            if (!event.shiftKey && this.state.selectedShapeIds.size > 0)
            {
                this.setState({ selectedShapeIds: new Set() });
            }
            const rect = (event.currentTarget as HTMLDivElement).getBoundingClientRect();
            const relX = x - rect.left;
            const relY = y - rect.top;
            const base = this.state.userShapeTemplate || {};
            const newShape: ToolShape = { ...base, x: relX, y: relY };
            this.addShape(newShape);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleContainerMouseUp:',error);
        }
    };

    private handleContainerTouchStart = (event: React.TouchEvent<HTMLDivElement>) =>
    {
        try
        {
            const { x, y } = this.getPointerPosition(event);
            this.mouseDownPosition = { x, y };
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleContainerTouchStart:',error);
        }
    };

    private handleContainerTouchEnd = (event: React.TouchEvent<HTMLDivElement>) =>
    {
        try
        {
            if (!this.mouseDownPosition) return;
            if (this.state.draggingShapeId) return;
            if (this.modalRef.current) return;
            if (this.chatRef.current && this.chatRef.current.contains(event.target as Node)) return;
            let x = 0, y = 0;
            if (event.changedTouches && event.changedTouches.length > 0)
            {
                x = event.changedTouches[0].clientX;
                y = event.changedTouches[0].clientY;
            }
            else
            {
                const pos = this.getPointerPosition(event);
                x = pos.x;
                y = pos.y;
            }
            const dx = x - this.mouseDownPosition.x;
            const dy = y - this.mouseDownPosition.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            this.mouseDownPosition = null;
            if (distance > 250) return;
            const rect = (event.currentTarget as HTMLDivElement).getBoundingClientRect();
            const relX = x - rect.left;
            const relY = y - rect.top;
            const base = this.state.userShapeTemplate || {};
            const newShape: ToolShape = { ...base, x: relX, y: relY };
            this.addShape(newShape);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleContainerTouchEnd:',error);
        }
    };

    // === Keyboard === //
    private handleKeyDown = (event: KeyboardEvent) =>
    {
        try
        {
            if (event.key === 'Escape')
            {
                if (!this.state.showShapeSelector && !this.state.showInfo && this.state.selectedShapeIds.size > 0)
                {
                    this.setState({ selectedShapeIds: new Set() });
                    event.preventDefault();
                }
            }
            else if (event.key === 'Backspace' || event.key === 'Delete')
            {
                this.reactorControl.purgeCore();
                event.preventDefault();
            }
            else if (/^[0-9]$/.test(event.key))
            {
                const speed = parseInt(event.key,10);
                this.reactorControl.setCoreSpeed(speed);
                event.preventDefault();
            }
            else if (event.key === ' ' || event.key === 'Space')
            {
                if (!this.state.showShapeSelector && !this.state.showInfo)
                {
                    this.setState({
                        showShapeSelector: true,
                        shapeSelectorValue: this.state.userShapeTemplate ? { ...this.state.userShapeTemplate } : {}
                    });
                }
                else if (this.state.showShapeSelector)
                {
                    this.setState({
                        showShapeSelector: false,
                        userShapeTemplate: this.state.shapeSelectorValue ? { ...this.state.shapeSelectorValue } : null
                    });
                }
                event.preventDefault();
            }
            else if (event.key.toLowerCase() === 'i')
            {
                if (!this.state.showShapeSelector)
                {
                    this.setState(prev => ({ showInfo: !prev.showInfo }));
                }
                event.preventDefault();
            }
            else if (event.key === '`')
            {
                if (!this.state.showShapeSelector && !this.state.showInfo)
                {
                    this.setState(prev => ({ showChat: !prev.showChat }));
                }
                event.preventDefault();
            }
            else if (event.key.toLowerCase() === 't')
            {
                this.setState(prev =>
                {
                    if (prev.trailsEnabled)
                    {
                        this.clearTrail();
                        this.clearAllRemoteTrails();
                    }
                        return { trailsEnabled: !prev.trailsEnabled };
                        });
                        event.preventDefault();
            }
            else if (event.key.toLowerCase() === 'm')
            {
                this.setState({ soundMuted: this.soundEngine.toggleMute() });
                event.preventDefault();
            }
            else if (event.key.toLowerCase() === 's')
            {
                this.setState(prev =>
                {
                    const shimmerEnabled = !prev.shimmerEnabled;

                    if (shimmerEnabled)
                    {
                        this.shapeShimmer.start(this.getShimmerEligibleIds, this.handleShimmer, this.handleShimmerEnd);
                    }
                    else
                    {
                        this.shapeShimmer.stop();
                    }

                    return {
                        shimmerEnabled,
                        shimmerOffsets: shimmerEnabled ? prev.shimmerOffsets : new Map()
                    };
                });
                event.preventDefault();
            }
            else if (event.key.toLowerCase() === 'c')
            {
                this.setState(prev => ({ showCursors: !prev.showCursors }));
                event.preventDefault();
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleKeyDown:',error);
        }
    };

    // === Modal Management === //
    private handleModalDismissal = () =>
    {
        try
        {
            if (this.state.showShapeSelector)
            {
                this.setState({ showShapeSelector: false });
            }
            else if (this.state.showInfo)
            {
                this.setState({ showInfo: false });
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleModalDismissal:',error);
        }
    };

    private handleModalKeyDown = (event: KeyboardEvent) =>
    {
        try
        {
            if (event.key === 'Escape')
            {
                this.handleModalDismissal();
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleModalKeyDown:',error);
        }
    };

    private handleDocumentMouseDown = (event: MouseEvent) =>
    {
        try
        {
            if (this.modalRef.current && !this.modalRef.current.contains(event.target as Node))
            {
                this.handleModalDismissal();
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleDocumentMouseDown:',error);
        }
    };

    // === Chat Management === //
    private handleMessageReceived = (message: ToolMessage) =>
    {
        try
        {
            if (!message || !message.id || !message.sender || !message.message) return;
            this.setState(prevState => ({
                chatItems: [...prevState.chatItems, {
                    id: String(message.id),
                    sender: String(message.sender),
                    message: String(message.message)
                }]
            }));
            this.soundEngine.playMessage();
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleMessageReceived:',error);
        }
    };

    private handleChatDismiss = () =>
    {
        try
        {
            this.setState({ showChat: false });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleChatDismiss:',error);
        }
    };

    private handleSendMessage = (sender: string, message: string) =>
    {
        try
        {
            this.reactorControl.sendMessage({ sender, message });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleSendMessage:',error);
        }
    };

    private handleSetChatName = (name: string) =>
    {
        try
        {
            this.setState({ chatName: name });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleSetChatName:',error);
        }
    };

    // === Cursor Management === //
    private handleGlobalMouseMove = (event: MouseEvent): void =>
    {
        try
        {
            this.throttledUpdateCursorPosition({
                name: this.state.chatName || undefined,
                rgb: this.state.shapeSelectorValue?.rgb || undefined,
                x: event.clientX,
                y: event.clientY
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleGlobalMouseMove:', error);
        }
    };

    private handleCursorPositionUpdated = (cursor: CursorPosition): void =>
    {
        try
        {
            if (!cursor.connectionId) { return; }
            this.setState(prevState =>
            {
                const next = new Map(prevState.cursors);
                next.set(cursor.connectionId!, cursor);
                return { cursors: next };
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleCursorPositionUpdated:', error);
        }
    };

    private handleCursorRemoved = (connectionId: string): void =>
    {
        try
        {
            this.setState(prevState =>
            {
                const next = new Map(prevState.cursors);
                next.delete(connectionId);
                return { cursors: next };
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleCursorRemoved:', error);
        }
    };

    // === Shimmer Management === //
    private getShimmerEligibleIds = (): string[] =>
    {
        try
        {
            const { shapes, draggingShapeId, shimmerEnabled } = this.state;
            if (!shimmerEnabled) { return []; }
            return shapes
                .filter(s => s.id != null && s.id !== draggingShapeId && !this.shapeMorph.hasMorph(s.id!))
                .map(s => s.id!);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in getShimmerEligibleIds:', error);
            return [];
        }
    };

    private handleShimmer = (id: string, opacityOffset: number): void =>
    {
        try
        {
            this.setState(prevState =>
            {
                const next = new Map(prevState.shimmerOffsets);
                next.set(id, opacityOffset);
                return { shimmerOffsets: next };
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShimmer:', error);
        }
    };

    private handleShimmerEnd = (id: string): void =>
    {
        try
        {
            this.setState(prevState =>
            {
                const next = new Map(prevState.shimmerOffsets);
                next.delete(id);
                return { shimmerOffsets: next };
            });
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleShimmerEnd:', error);
        }
    };

    // === Rendering === //
    private renderGhostShape(shape: ToolShape, trailIndex: number, trailLength: number): React.ReactNode
    {
        try
        {
            const ghostOpacity = TRAIL_BASE_OPACITY * ((trailIndex + 1) / trailLength);
            const ghostKey = `ghost-${shape.id}-${trailIndex}`;

            if (shape.star)
            {
                return (
                    <Star
                        key={ghostKey}
                        id={shape.id ?? ""}
                        rgb={shape.rgb ?? "#000"}
                        x={shape.x ?? 0}
                        y={shape.y ?? 0}
                        rotation={shape.rotation ?? 0}
                        opacity={shape.opacity ?? 1}
                        scale={shape.scale ?? 1}
                        sides={shape.sides ?? 5}
                        ghost={true}
                        ghostOpacity={ghostOpacity}
                    />
                );
            }
            else if (shape.circle)
            {
                return (
                    <Circle
                        key={ghostKey}
                        id={shape.id ?? ""}
                        rgb={shape.rgb ?? "#000"}
                        x={shape.x ?? 0}
                        y={shape.y ?? 0}
                        rotation={shape.rotation ?? 0}
                        opacity={shape.opacity ?? 1}
                        scale={shape.scale ?? 1}
                        ghost={true}
                        ghostOpacity={ghostOpacity}
                    />
                );
            }
            else
            {
                return (
                    <Polygon
                        key={ghostKey}
                        id={shape.id ?? ""}
                        rgb={shape.rgb ?? "#000"}
                        x={shape.x ?? 0}
                        y={shape.y ?? 0}
                        rotation={shape.rotation ?? 0}
                        opacity={shape.opacity ?? 1}
                        scale={shape.scale ?? 1}
                        sides={shape.sides ?? 12}
                        ghost={true}
                        ghostOpacity={ghostOpacity}
                    />
                );
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in renderGhostShape:', error);
            return null;
        }
    }

    private renderTrail(): React.ReactNode[]
    {
        try
        {
            const { draggingShapeId, shapes, trailPositions, remoteTrailPositions } = this.state;
            const nodes: React.ReactNode[] = [];

            if (draggingShapeId && trailPositions.length > 0)
            {
                const draggedShape = shapes.find(s => s.id === draggingShapeId);
                if (draggedShape)
                {
                    trailPositions.forEach((pos, index) =>
                    {
                        nodes.push(this.renderGhostShape(
                            { ...draggedShape, x: pos.x, y: pos.y },
                            index,
                            trailPositions.length
                        ));
                    });
                }
            }

            remoteTrailPositions.forEach((positions, shapeId) =>
            {
                const shape = shapes.find(s => s.id === shapeId);
                if (!shape || positions.length === 0) { return; }

                positions.forEach((pos, index) =>
                {
                    nodes.push(this.renderGhostShape(
                        { ...shape, x: pos.x, y: pos.y },
                        index,
                        positions.length
                    ));
                });
            });

            return nodes;
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in renderTrail:', error);
            return [];
        }
    }

    renderShape(shape: ToolShape)
    {
        try
        {
            const onRemove = () => this.removeShape(shape.id);
            const onMouseDown = (e: React.MouseEvent) => this.handleShapeMouseDown(e,shape);
            const onTouchStart = (e: React.TouchEvent) => this.handleShapeTouchStart(e,shape);
            const onTouchEnd = (e: React.TouchEvent) => this.handleShapeTouchEnd(e);
            const shimmerOffset = shape.id ? (this.state.shimmerOffsets.get(shape.id) ?? 0) : 0;
            const effectiveOpacity = Math.min(1, Math.max(0, (shape.opacity ?? 1) + shimmerOffset));
            const isSelected = shape.id != null && this.state.selectedShapeIds.has(shape.id);

            if (shape.star)
            {
                return (
                    <Star
                        key={shape.id ?? ""}
                        id={shape.id ?? ""}
                        rgb={shape.rgb ?? "#000"}
                        x={shape.x ?? 0}
                        y={shape.y ?? 0}
                        rotation={shape.rotation ?? 0}
                        opacity={effectiveOpacity}
                        scale={shape.scale ?? 1}
                        sides={shape.sides ?? 5}
                        selected={isSelected}
                        onRemove={onRemove}
                        onMouseDown={onMouseDown}
                        onTouchStart={onTouchStart}
                        onTouchEnd={onTouchEnd}
                    />
                );
            }
            else if (shape.circle)
            {
                return (
                    <Circle
                        key={shape.id ?? ""}
                        id={shape.id ?? ""}
                        rgb={shape.rgb ?? "#000"}
                        x={shape.x ?? 0}
                        y={shape.y ?? 0}
                        rotation={shape.rotation ?? 0}
                        opacity={effectiveOpacity}
                        scale={shape.scale ?? 1}
                        selected={isSelected}
                        onRemove={onRemove}
                        onMouseDown={onMouseDown}
                        onTouchStart={onTouchStart}
                        onTouchEnd={onTouchEnd}
                    />
                );
            }
            else
            {
                return (
                    <Polygon
                        key={shape.id ?? ""}
                        id={shape.id ?? ""}
                        rgb={shape.rgb ?? "#000"}
                        x={shape.x ?? 0}
                        y={shape.y ?? 0}
                        rotation={shape.rotation ?? 0}
                        opacity={effectiveOpacity}
                        scale={shape.scale ?? 1}
                        sides={shape.sides ?? 12}
                        selected={isSelected}
                        onRemove={onRemove}
                        onMouseDown={onMouseDown}
                        onTouchStart={onTouchStart}
                        onTouchEnd={onTouchEnd}
                    />
                );
            }
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in renderShape:',error);
            return null;
        }
    }

    render()
    {
        try
        {
            const containerStyle: React.CSSProperties = {
                position: "relative",
                width: "100vw",
                height: "100vh",
                overflow: "hidden",
                background: `#000000 url(${starfield}) center/cover no-repeat`
            };
            return (
                <div
                    style={containerStyle}
                    onContextMenu={e => e.preventDefault()}
                    onMouseDown={this.handleContainerMouseDown}
                    onMouseUp={this.handleContainerMouseUp}
                    onTouchStart={this.handleContainerTouchStart}
                    onTouchEnd={this.handleContainerTouchEnd}
                >
                    {this.renderTrail()}
                    {this.state.shapes.map((shape) => this.renderShape(shape))}
                    {this.state.showShapeSelector && (
                        <ShapeSelector
                            onDone={this.handleShapeSelectorDone}
                            toolShape={this.state.shapeSelectorValue}
                            onRandomize={this.handleShapeSelectorRandomize}
                            forwardedRef={this.modalRef}
                        />
                    )}
                    {this.state.showChat && (
                        <ChatControl
                            messages={this.state.chatItems}
                            onSendMessage={this.handleSendMessage}
                            setName={this.handleSetChatName}
                            name={this.state.chatName}
                            forwardedRef={this.chatRef}
                            onDismiss={this.handleChatDismiss}
                        />
                    )}
                    {this.state.showInfo && (
                        <Info
                            forwardedRef={this.modalRef}
                            showChat={this.state.showChat}
                            showCursors={this.state.showCursors}
                            shimmerEnabled={this.state.shimmerEnabled}
                            soundMuted={this.state.soundMuted}
                            trailsEnabled={this.state.trailsEnabled}
                        />
                    )}
                    {this.state.showCursors && this.state.cursors.size > 0 && (
                        <CursorOverlay cursors={this.state.cursors} />
                    )}
                </div>
            );
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in render:',error);
            return null;
        }
    }
}