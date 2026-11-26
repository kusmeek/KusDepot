using System.Diagnostics;

namespace DataPodServices;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.TraceContext")]
public sealed record class TraceContext
{
    [Id(0)]
    public String TraceId { get; init; }

    [Id(1)]
    public String SpanId { get; init; }

    [Id(2)]
    public ActivityTraceFlags TraceFlags { get; init; }

    [Id(3)]
    public String? TraceState { get; init; }

    public TraceContext(String traceId , String spanId , ActivityTraceFlags traceFlags , String? traceState)
    {
        this.TraceId = traceId; this.SpanId = spanId; this.TraceFlags = traceFlags; this.TraceState = traceState;
    }

    public static TraceContext? FromActivityContext(ActivityContext? activityContext)
    {
        if(activityContext is null) { return null; }

        ActivityContext context = activityContext.Value;

        String traceId = context.TraceId.ToHexString();

        String spanId = context.SpanId.ToHexString();

        if(IsValidId(traceId,32) is false || IsValidId(spanId,16) is false) { return null; }

        return new TraceContext(traceId,spanId,context.TraceFlags,context.TraceState);
    }

    public Boolean TryToActivityContext(out ActivityContext activityContext)
    {
        activityContext = default;

        if(IsValidId(this.TraceId,32) is false || IsValidId(this.SpanId,16) is false) { return false; }

        try
        {
            ActivityTraceId traceId = ActivityTraceId.CreateFromString(this.TraceId);

            ActivitySpanId spanId = ActivitySpanId.CreateFromString(this.SpanId);

            activityContext = new ActivityContext(traceId,spanId,this.TraceFlags,this.TraceState);

            return true;
        }
        catch ( Exception exception ) when(exception is ArgumentException or FormatException)
        {
            activityContext = default;

            return false;
        }
    }

    private static Boolean IsValidId(String? value , Int32 length)
    {
        if(value is null || value.Length != length) { return false; }

        Boolean nonZero = false;

        foreach(Char character in value)
        {
            if(Uri.IsHexDigit(character) is false) { return false; }

            if(character is not '0') { nonZero = true; }
        }

        return nonZero;
    }
}
