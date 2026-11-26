namespace KusDepot.Data.Transfer;

/**<include file='AsyncByteRateLimiter.xml' path='AsyncByteRateLimiter/class[@name="AsyncByteRateLimiter"]/main/*'/>*/
public sealed class AsyncByteRateLimiter
{
    /**<include file='AsyncByteRateLimiter.xml' path='AsyncByteRateLimiter/class[@name="AsyncByteRateLimiter"]/field[@name="NextAvailableTimestamp"]/*'/>*/
    private Int64 NextAvailableTimestamp;

    /**<include file='AsyncByteRateLimiter.xml' path='AsyncByteRateLimiter/class[@name="AsyncByteRateLimiter"]/field[@name="BytesPerSecond"]/*'/>*/
    private readonly Int64 BytesPerSecond;

    /**<include file='AsyncByteRateLimiter.xml' path='AsyncByteRateLimiter/class[@name="AsyncByteRateLimiter"]/field[@name="Gate"]/*'/>*/
    private readonly SemaphoreSlim Gate = new(1,1);

    /**<include file='AsyncByteRateLimiter.xml' path='AsyncByteRateLimiter/class[@name="AsyncByteRateLimiter"]/constructor[@name="Constructor"]/*'/>*/
    public AsyncByteRateLimiter(Int64 bytespersecond)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bytespersecond);

        this.BytesPerSecond = bytespersecond;

        this.NextAvailableTimestamp = Stopwatch.GetTimestamp();
    }

    /**<include file='AsyncByteRateLimiter.xml' path='AsyncByteRateLimiter/class[@name="AsyncByteRateLimiter"]/method[@name="WaitAsync"]/*'/>*/
    public ValueTask WaitAsync(Int32 bytecount , CancellationToken cancel = default) => this.WaitAsync(bytecount,Stopwatch.GetTimestamp(),cancel);

    internal async ValueTask WaitAsync(Int32 bytecount , Int64 operationstartedtimestamp , CancellationToken cancel)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytecount);

        if(bytecount == 0) { return; }

        Int64 deadline;

        await this.Gate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            Int64 start = Math.Max(operationstartedtimestamp,this.NextAvailableTimestamp);

            Int64 duration = (Int64)Math.Ceiling(bytecount * (Double)Stopwatch.Frequency / this.BytesPerSecond);

            deadline = checked(start + duration); this.NextAvailableTimestamp = deadline;
        }
        finally { this.Gate.Release(); }

        while(true)
        {
            Int64 now = Stopwatch.GetTimestamp();

            if(now >= deadline) { return; }

            Double secondsremaining = (deadline - now) / (Double)Stopwatch.Frequency;

            await Task.Delay(TimeSpan.FromSeconds(Math.Min(secondsremaining,TimeSpan.FromDays(1).TotalSeconds)),cancel).ConfigureAwait(false);
        }
    }
}
