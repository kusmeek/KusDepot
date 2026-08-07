const SHIMMER_CHANCE = 0.5;
const SHIMMER_MIN_MS = 400;
const SHIMMER_MAX_MS = 2000;
const SHIMMER_INTENSITY = 0.28;

interface ShimmerState
{
    shapeId: string;
    startTime: number;
    duration: number;
}

export default class ShapeShimmer
{
    private active: ShimmerState | null = null;
    private animationFrameId: number | null = null;
    private running: boolean = false;

    private onShimmer: ((id: string, opacityOffset: number) => void) | null = null;
    private onShimmerEnd: ((id: string) => void) | null = null;
    private getEligibleIds: (() => string[]) | null = null;

    start(
        getEligibleIds: () => string[],
        onShimmer: (id: string, opacityOffset: number) => void,
        onShimmerEnd: (id: string) => void
    ): void
    {
        try
        {
            this.getEligibleIds = getEligibleIds;
            this.onShimmer = onShimmer;
            this.onShimmerEnd = onShimmerEnd;
            this.running = true;
            this.ensureLoop();
        }
        catch (error)
        {
            console.error('[ShapeShimmer] Error in start:', error);
        }
    }

    stop(): void
    {
        try
        {
            this.running = false;
            if (this.animationFrameId !== null)
            {
                cancelAnimationFrame(this.animationFrameId);
                this.animationFrameId = null;
            }
            if (this.active && this.onShimmerEnd)
            {
                this.onShimmerEnd(this.active.shapeId);
            }
            this.active = null;
            this.onShimmer = null;
            this.onShimmerEnd = null;
            this.getEligibleIds = null;
        }
        catch (error)
        {
            console.error('[ShapeShimmer] Error in stop:', error);
        }
    }

    private ensureLoop(): void
    {
        try
        {
            if (this.animationFrameId !== null || !this.running) { return; }
            this.animationFrameId = requestAnimationFrame(this.tick);
        }
        catch (error)
        {
            console.error('[ShapeShimmer] Error in ensureLoop:', error);
        }
    }

    private tick = (): void =>
    {
        try
        {
            this.animationFrameId = null;
            if (!this.running) { return; }

            const now = performance.now();

            if (this.active)
            {
                const elapsed = now - this.active.startTime;
                if (elapsed >= this.active.duration)
                {
                    if (this.onShimmerEnd) { this.onShimmerEnd(this.active.shapeId); }
                    this.active = null;
                }
                else
                {
                    const t = elapsed / this.active.duration;
                    const offset = Math.sin(t * Math.PI) * SHIMMER_INTENSITY;
                    if (this.onShimmer) { this.onShimmer(this.active.shapeId, offset); }
                }
            }
            else
            {
                if (Math.random() < SHIMMER_CHANCE && this.getEligibleIds)
                {
                    const ids = this.getEligibleIds();
                    if (ids.length > 0)
                    {
                        const shapeId = ids[Math.floor(Math.random() * ids.length)];
                        const duration = SHIMMER_MIN_MS + Math.random() * (SHIMMER_MAX_MS - SHIMMER_MIN_MS);
                        this.active = { shapeId, startTime: now, duration };
                    }
                }
            }

            this.animationFrameId = requestAnimationFrame(this.tick);
        }
        catch (error)
        {
            console.error('[ShapeShimmer] Error in tick:', error);
        }
    };
}
