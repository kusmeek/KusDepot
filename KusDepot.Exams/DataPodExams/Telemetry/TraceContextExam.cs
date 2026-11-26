namespace KusDepot.DataPodExams;

[TestFixture]
public sealed class TraceContextExam
{
    [Test]
    public void FromActivityContextReturnsNullForNoContext()
    {
        Check.That(TraceContext.FromActivityContext(null)).IsNull();
    }

    [TestCase(ActivityTraceFlags.Recorded,"vendor=value")]
    [TestCase(ActivityTraceFlags.None,null)]
    public void ActivityContextRoundTripPreservesFlagsAndTraceState(ActivityTraceFlags traceFlags , String? traceState)
    {
        ActivityContext original = new(
            ActivityTraceId.CreateFromString("0123456789abcdef0123456789abcdef"),
            ActivitySpanId.CreateFromString("0123456789abcdef"),
            traceFlags,
            traceState);

        TraceContext traceContext = TraceContext.FromActivityContext(original)!;

        Check.That(traceContext.TryToActivityContext(out ActivityContext restored)).IsTrue();
        Check.That(restored.TraceId).IsEqualTo(original.TraceId);
        Check.That(restored.SpanId).IsEqualTo(original.SpanId);
        Check.That(restored.TraceFlags).IsEqualTo(original.TraceFlags);
        Check.That(restored.TraceState).IsEqualTo(original.TraceState);
    }

    [TestCase("invalid", "0123456789abcdef")]
    [TestCase("00000000000000000000000000000000", "0123456789abcdef")]
    [TestCase("0123456789abcdef0123456789abcdef", "0000000000000000")]
    public void TryToActivityContextRejectsInvalidIds(String traceId , String spanId)
    {
        TraceContext traceContext = new(traceId,spanId,ActivityTraceFlags.None,null);

        Check.That(traceContext.TryToActivityContext(out ActivityContext context)).IsFalse();
        Check.That(context).IsEqualTo(default(ActivityContext));
    }
}
