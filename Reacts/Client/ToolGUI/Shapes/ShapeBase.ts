export interface ShapeBase
{
    id?: string | null;
    rgb?: string | null;
    x?: number | null;
    y?: number | null;
    circle?: boolean | null;
    rotation?: number | null;
    opacity?: number | null;
    scale?: number | null;
    sides?: number | null;
    star?: boolean | null;
    onRemove?: () => void;
    onMouseDown?: (e: React.MouseEvent) => void;
    onTouchStart?: (e: React.TouchEvent) => void;
    onTouchEnd?: (e: React.TouchEvent) => void;
}