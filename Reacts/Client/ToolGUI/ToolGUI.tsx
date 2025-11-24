import React from "react";
import "./Shapes/fadeout.css";
import Info from "./Info/Info";
import Star from "./Shapes/Star";
import ToolShape from "./ToolShape";
import Circle from "./Shapes/Circle";
import Polygon from "./Shapes/Polygon";
import ToolMessage from "./ToolMessage";
import UpdateThrottle from "./UpdateThrottle";
import starfield from "./Image/starfield.webp";
import ToolShapePosition from "./ToolShapePosition";
import ShapeSelector from "./ShapeSelector/ShapeSelector";
import ReactorControl from "./ReactorControl/ReactorControl";
import ChatControl, { ChatItem } from "./ChatControl/ChatControl";

interface ToolGUIState
{
    chatName: string;
    showInfo: boolean;
    showChat: boolean;
    shapes: ToolShape[];
    chatItems: ChatItem[];
    showShapeSelector: boolean;
    draggingShapeId: string | null;
    userShapeTemplate: ToolShape | null;
    shapeSelectorValue: ToolShape | null;
    dragOffset: { x: number; y: number; } | null;
}

export default class ToolGUI extends React.Component<{}, ToolGUIState>
{
    private longPressTimer: any = null;
    private dragStartTimer: any = null;
    private dragInitiated: boolean = false;
    private reactorControl: ReactorControl;
    private chatRef: React.RefObject<HTMLDivElement>;
    private modalRef: React.RefObject<HTMLFormElement>;
    private mouseDownPosition: { x: number; y: number; } | null = null;
    private throttledUpdateShapePosition: (position: ToolShapePosition) => void;

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
        };

        this.reactorControl = new ReactorControl();

        this.throttledUpdateShapePosition = UpdateThrottle((position: ToolShapePosition) =>
        {
            if (position.id) { this.reactorControl.updateShapePosition(position); }
        }, 10);

        this.modalRef = React.createRef<HTMLFormElement>();
        this.chatRef = React.createRef<HTMLDivElement>();
    }

    componentDidMount()
    {
        try
        {
            this.reactorControl.start();
            window.addEventListener('keydown', this.handleKeyDown);
            this.reactorControl.onCorePurged(this.handleCorePurge);
            this.reactorControl.onShapeAdded(this.handleShapeAdded);
            this.reactorControl.onShapeRemoved(this.handleShapeRemoved);
            this.reactorControl.onShapeUpdated(this.handleShapeUpdated);
            this.reactorControl.onMessageReceived(this.handleMessageReceived);
            this.reactorControl.onShapePositionUpdated(this.handleShapePositionUpdated);

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
            window.removeEventListener('keydown', this.handleKeyDown);
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
                const exists = prevState.shapes.some(shape => shape.id === updatedShape.id);
                if (exists)
                {
                    return {
                        shapes: prevState.shapes.map(shape =>
                            shape.id === updatedShape.id ? { ...shape, ...updatedShape } : shape
                        )
                    };
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

    private handleShapeReMake = (shape: ToolShape) =>
    {
        try
        {
            const { id, x, y } = shape; let newShape: ToolShape = { id, x, y };

            const selector = this.state.shapeSelectorValue || {};

            for (const key in selector)
            {
                if (['id', 'x', 'y'].includes(key)) continue;
                newShape[key] = selector[key];
            }

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
            const idsToPurge = new Set(this.state.shapes.map(shape => shape.id).filter(id => id != null));
            idsToPurge.forEach(id =>
            {
                const el = document.getElementById(`shape-${id}`);
                if (el) el.classList.add('fade-out');
            });
            setTimeout(() =>
            {
                this.setState(prevState => ({
                    shapes: prevState.shapes.filter(shape => !idsToPurge.has(shape.id))
                }));
            }, 2000);
        }
        catch (error)
        {
            console.error('[ToolGUI] Error in handleCorePurge:',error);
        }
    };

    // === Interaction === //
    private startDrag = (shape: ToolShape, x: number, y: number) =>
    {
        try
        {
            this.dragInitiated = true;
            this.setState({
                draggingShapeId: shape.id ?? null,
                dragOffset: { x: x - (shape.x ?? 0), y: y - (shape.y ?? 0) }
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
            this.setState({ draggingShapeId: null, dragOffset: null });
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

    private getPointerPosition = (event: any) =>
    {
        try
        {
            if (event.touches && event.touches.length > 0)
            {
                return { x: event.touches[0].clientX, y: event.touches[0].clientY };
            }
            return { x: event.clientX, y: event.clientY };
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
                this.removeShape(shape.id);
                return;
            }
            if (event.button === 1)
            {
                event.preventDefault();
                event.stopPropagation();
                if (shape.id)
                {
                    const caps = event.getModifierState('CapsLock');
                    if (event.ctrlKey && event.shiftKey && event.altKey)
                    {
                        this.handleShapeReMake(shape);
                    }
                    else if (event.ctrlKey && event.shiftKey && !caps)
                    {
                        this.reactorControl.randomizeShapeFG(shape);
                    }
                    else if (event.ctrlKey && event.shiftKey && caps)
                    {
                        this.reactorControl.randomizeShapeFH(shape);
                    }
                    else if (event.altKey && event.ctrlKey && !caps)
                    {
                        this.reactorControl.randomizeShapeNG(shape);
                    }
                    else if (event.altKey && event.ctrlKey && caps)
                    {
                        this.reactorControl.randomizeShapeNH(shape);
                    }
                    else if (event.altKey && event.shiftKey && !caps)
                    {
                        this.reactorControl.randomizeShapePG(shape);
                    }
                    else if (event.altKey && event.shiftKey && caps)
                    {
                        this.reactorControl.randomizeShapePH(shape);
                    }
                    else if (event.shiftKey && !caps)
                    {
                        this.reactorControl.randomizeShapeGG(shape);
                    }
                    else if (event.shiftKey && caps)
                    {
                        this.reactorControl.randomizeShapeGH(shape);
                    }
                    else if (event.altKey && !caps)
                    {
                        this.reactorControl.randomizeShapeJG(shape);
                    }
                    else if (event.altKey && caps)
                    {
                        this.reactorControl.randomizeShapeJH(shape);
                    }
                    else if (event.ctrlKey && !caps)
                    {
                        this.reactorControl.randomizeShapeRG(shape);
                    }
                    else if (event.ctrlKey && caps)
                    {
                        this.reactorControl.randomizeShapeRH(shape);
                    }
                    else
                    {
                        this.reactorControl.randomizeShape(shape);
                    }
                }
                return;
            }
            if (event.button === 0)
            {
                event.preventDefault();
                event.stopPropagation();
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
                clearTimeout(this.dragStartTimer);
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
            clearTimeout(this.dragStartTimer);
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
            clearTimeout(this.longPressTimer);
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
            const { draggingShapeId, dragOffset, shapes } = this.state;
            if (!draggingShapeId || !dragOffset) return;
            const { x, y } = this.getPointerPosition(event);
            const updatedShapes = shapes.map(shape =>
                shape.id === draggingShapeId
                    ? { ...shape, x: x - dragOffset.x, y: y - dragOffset.y }
                    : shape
            );
            this.setState({ shapes: updatedShapes });

            this.throttledUpdateShapePosition({ id: draggingShapeId, x: x - dragOffset.x, y: y - dragOffset.y });
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
            const { draggingShapeId, shapes } = this.state;
            if (draggingShapeId)
            {
                const shape = shapes.find(s => s.id === draggingShapeId);
                if (shape)
                {
                    this.reactorControl.updateShape(shape);
                }
            }
            this.setState({ draggingShapeId: null, dragOffset: null });
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
            const { draggingShapeId, dragOffset, shapes } = this.state;
            if (!draggingShapeId || !dragOffset) return;
            const { x, y } = this.getPointerPosition(event);
            const updatedShapes = shapes.map(shape =>
                shape.id === draggingShapeId
                    ? { ...shape, x: x - dragOffset.x, y: y - dragOffset.y }
                    : shape
            );
            this.setState({ shapes: updatedShapes });

            this.throttledUpdateShapePosition({ id: draggingShapeId, x: x - dragOffset.x, y: y - dragOffset.y });
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
            const { draggingShapeId, shapes } = this.state;
            if (draggingShapeId)
            {
                const shape = shapes.find(s => s.id === draggingShapeId);
                if (shape)
                {
                    this.reactorControl.updateShape(shape);
                }
            }
            this.setState({ draggingShapeId: null, dragOffset: null });
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
            if (event.key === 'Backspace' || event.key === 'Delete')
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

    // === Rendering === //
    renderShape(shape: ToolShape)
    {
        try
        {
            const onRemove = () => this.removeShape(shape.id);
            const onMouseDown = (e: React.MouseEvent) => this.handleShapeMouseDown(e,shape);
            const onTouchStart = (e: React.TouchEvent) => this.handleShapeTouchStart(e,shape);
            const onTouchEnd = (e: React.TouchEvent) => this.handleShapeTouchEnd(e);

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
                        opacity={shape.opacity ?? 1}
                        scale={shape.scale ?? 1}
                        sides={shape.sides ?? 5}
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
                        opacity={shape.opacity ?? 1}
                        scale={shape.scale ?? 1}
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
                        opacity={shape.opacity ?? 1}
                        scale={shape.scale ?? 1}
                        sides={shape.sides ?? 12}
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
                        <Info forwardedRef={this.modalRef} />
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