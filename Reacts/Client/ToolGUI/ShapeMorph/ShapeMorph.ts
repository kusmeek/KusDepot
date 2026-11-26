import ToolShape from "../ToolShape";

interface MorphState
{
    startShape: ToolShape;
    targetShape: ToolShape;
    startTime: number;
}

const MORPH_DURATION_MS = 300;

type MorphNumericKey = "rotation" | "scale" | "opacity" | "sides";

const MORPH_NUMERIC_KEYS: readonly MorphNumericKey[] = ["rotation", "scale", "opacity", "sides"];

export default class ShapeMorph
{
    private activeMorphs: Map<string, MorphState> = new Map();
    private animationFrameId: number | null = null;
    private onUpdate: ((id: string, interpolated: ToolShape) => void) | null = null;
    private onComplete: ((id: string, finalShape: ToolShape) => void) | null = null;

    start(
        onUpdate: (id: string, interpolated: ToolShape) => void,
        onComplete: (id: string, finalShape: ToolShape) => void
    ): void
    {
        try
        {
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in start:', error);
        }
    }

    stop(): void
    {
        try
        {
            if (this.animationFrameId !== null)
            {
                cancelAnimationFrame(this.animationFrameId);
                this.animationFrameId = null;
            }
            this.activeMorphs.clear();
            this.onUpdate = null;
            this.onComplete = null;
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in stop:', error);
        }
    }

    beginMorph(currentShape: ToolShape, targetShape: ToolShape): void
    {
        try
        {
            const id = targetShape.id;
            if (!id) { return; }

            const typeChanged = (currentShape.star !== targetShape.star)
                || (currentShape.circle !== targetShape.circle);

            if (typeChanged)
            {
                this.activeMorphs.delete(id);
                if (this.onComplete) { this.onComplete(id, targetShape); }
                return;
            }

            const existing = this.activeMorphs.get(id);
            const startShape = existing
                ? this.interpolate(existing, performance.now())
                : { ...currentShape };

            this.activeMorphs.set(id, {
                startShape,
                targetShape: { ...targetShape },
                startTime: performance.now()
            });

            this.ensureLoop();
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in beginMorph:', error);
        }
    }

    cancelMorph(id: string): void
    {
        try
        {
            this.activeMorphs.delete(id);
            if (this.activeMorphs.size === 0 && this.animationFrameId !== null)
            {
                cancelAnimationFrame(this.animationFrameId);
                this.animationFrameId = null;
            }
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in cancelMorph:', error);
        }
    }

    cancelAll(): void
    {
        try
        {
            this.activeMorphs.clear();
            if (this.animationFrameId !== null)
            {
                cancelAnimationFrame(this.animationFrameId);
                this.animationFrameId = null;
            }
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in cancelAll:', error);
        }
    }

    hasMorph(id: string): boolean
    {
        return this.activeMorphs.has(id);
    }

    private ensureLoop(): void
    {
        try
        {
            if (this.animationFrameId !== null) { return; }
            this.animationFrameId = requestAnimationFrame(this.tick);
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in ensureLoop:', error);
        }
    }

    private tick = (): void =>
    {
        try
        {
            this.animationFrameId = null;
            const now = performance.now();
            const completedIds: string[] = [];

            this.activeMorphs.forEach((morph, id) =>
            {
                const elapsed = now - morph.startTime;
                if (elapsed >= MORPH_DURATION_MS)
                {
                    completedIds.push(id);
                }
                else
                {
                    const interpolated = this.interpolate(morph, now);
                    if (this.onUpdate) { this.onUpdate(id, interpolated); }
                }
            });

            for (const id of completedIds)
            {
                const morph = this.activeMorphs.get(id);
                this.activeMorphs.delete(id);
                if (morph && this.onComplete) { this.onComplete(id, morph.targetShape); }
            }

            if (this.activeMorphs.size > 0)
            {
                this.animationFrameId = requestAnimationFrame(this.tick);
            }
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in tick:', error);
        }
    };

    private interpolate(morph: MorphState, now: number): ToolShape
    {
        try
        {
            const elapsed = now - morph.startTime;
            const t = Math.min(elapsed / MORPH_DURATION_MS, 1);
            const eased = this.easeOutCubic(t);

            const result: ToolShape = { ...morph.targetShape };

            for (const key of MORPH_NUMERIC_KEYS)
            {
                const from = morph.startShape[key] as number | null | undefined;
                const to = morph.targetShape[key] as number | null | undefined;
                if (from != null && to != null)
                {
                    let value = from + (to - from) * eased;
                    if (key === "sides") { value = Math.round(value); }
                    (result as Record<string, unknown>)[key] = value;
                }
            }

            const fromRgb = this.parseHex(morph.startShape.rgb);
            const toRgb = this.parseHex(morph.targetShape.rgb);
            if (fromRgb && toRgb)
            {
                const r = Math.round(fromRgb.r + (toRgb.r - fromRgb.r) * eased);
                const g = Math.round(fromRgb.g + (toRgb.g - fromRgb.g) * eased);
                const b = Math.round(fromRgb.b + (toRgb.b - fromRgb.b) * eased);
                result.rgb = `#${this.toHex(r)}${this.toHex(g)}${this.toHex(b)}`;
            }

            return result;
        }
        catch (error)
        {
            console.error('[ShapeMorph] Error in interpolate:', error);
            return { ...morph.targetShape };
        }
    }

    private easeOutCubic(t: number): number
    {
        return 1 - Math.pow(1 - t, 3);
    }

    private parseHex(hex: string | null | undefined): { r: number; g: number; b: number } | null
    {
        try
        {
            if (!hex) { return null; }
            const clean = hex.replace("#", "");
            if (clean.length !== 6) { return null; }
            return {
                r: parseInt(clean.substring(0, 2), 16),
                g: parseInt(clean.substring(2, 4), 16),
                b: parseInt(clean.substring(4, 6), 16)
            };
        }
        catch
        {
            return null;
        }
    }

    private toHex(value: number): string
    {
        return Math.max(0, Math.min(255, value)).toString(16).padStart(2, "0");
    }
}
