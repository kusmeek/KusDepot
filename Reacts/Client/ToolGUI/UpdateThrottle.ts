export default function UpdateThrottle<TArgs extends unknown[]>(func: (...args: TArgs) => void, limit: number): (...args: TArgs) => void
{
    let lastCall = 0;
    let timeout: ReturnType<typeof setTimeout> | null = null;
    let lastArgs: TArgs;

    const throttled = (...args: TArgs) =>
    {
        const now = Date.now();
        lastArgs = args;
        if (now - lastCall >= limit)
        {
            lastCall = now;
            func(...args);
        }
        else
        {
            if (timeout) { clearTimeout(timeout); }
            timeout = setTimeout(() => {
                lastCall = Date.now();
                func(...lastArgs);
            }, limit - (now - lastCall));
        }
    };

    return throttled;
}