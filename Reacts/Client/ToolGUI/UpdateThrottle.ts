export default function UpdateThrottle<T extends (...args: any[]) => void>(func: T, limit: number): T
{
    let lastCall = 0;
    let timeout: any = null;
    let lastArgs: any;
    const throttled = function(this: any, ...args: any[])
    {
        const now = Date.now();
        lastArgs = args;
        if (now - lastCall >= limit)
        {
            lastCall = now;
            func.apply(this, args);
        }
        else
        {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                lastCall = Date.now();
                func.apply(this, lastArgs);
            }, limit - (now - lastCall));
        }
    } as T;
    return throttled;
}