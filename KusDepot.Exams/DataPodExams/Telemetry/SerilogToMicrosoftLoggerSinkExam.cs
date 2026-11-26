using DataPodServices;
using System.Collections.Generic;
using LoggingLevel = Microsoft.Extensions.Logging.LogLevel;

namespace KusDepot.DataServices;

[TestFixture]
public sealed class SerilogToMicrosoftLoggerSinkExam
{
    [Test]
    public void Emit_PreservesNamedPropertiesMessageTemplateAndException()
    {
        CaptureLogger target = new();
        InvalidOperationException exception = new("expected failure");
        Serilog.Core.Logger logger = new Serilog.LoggerConfiguration()
            .WriteTo.Sink(new SerilogToMicrosoftLoggerSink(target))
            .CreateLogger();

        logger.Error(exception,"Transfer {TransferId} failed for {@Owner}",20,new { Name = "Ada", Region = "north" });

        Assert.That(target.Calls,Is.EqualTo(1));
        Assert.That(target.Exception,Is.SameAs(exception));
        Assert.That(target.EventId,Is.EqualTo(new Microsoft.Extensions.Logging.EventId(0,"Transfer {TransferId} failed for {@Owner}")));
        Assert.That(target.LogLevel,Is.EqualTo(Microsoft.Extensions.Logging.LogLevel.Error));
        Assert.That(target.Message,Is.EqualTo("Transfer 20 failed for { Name: \"Ada\", Region: \"north\" }"));
        Assert.That(target.State["TransferId"],Is.EqualTo(20));
        Assert.That(target.State["Owner"],Is.TypeOf<Dictionary<String,Object?>>());
        Assert.That(target.State["{OriginalFormat}"],Is.EqualTo("Transfer {TransferId} failed for {@Owner}"));

        Dictionary<String,Object?> owner = (Dictionary<String,Object?>)target.State["Owner"]!;
        Assert.That(owner["Name"],Is.EqualTo("Ada"));
        Assert.That(owner["Region"],Is.EqualTo("north"));
    }

    private sealed class CaptureLogger : Microsoft.Extensions.Logging.ILogger
    {
        public Int32 Calls { get; private set; }
        public LoggingLevel LogLevel { get; private set; }
        public Microsoft.Extensions.Logging.EventId EventId { get; private set; }
        public Exception? Exception { get; private set; }
        public String? Message { get; private set; }
        public Dictionary<String,Object?> State { get; private set; } = new(StringComparer.Ordinal);

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public Boolean IsEnabled(LoggingLevel logLevel) => true;

        public void Log<TState>(LoggingLevel logLevel,Microsoft.Extensions.Logging.EventId eventId,TState state,Exception? exception,Func<TState,Exception?,String> formatter)
        {
            this.Calls++;
            this.LogLevel = logLevel;
            this.EventId = eventId;
            this.Exception = exception;
            this.Message = formatter(state,exception);
            this.State = state is IEnumerable<KeyValuePair<String,Object?>> properties
                ? properties.ToDictionary(property => property.Key,property => property.Value,StringComparer.Ordinal)
                : throw new InvalidOperationException("Expected structured key-value log state.");
        }
    }
}
