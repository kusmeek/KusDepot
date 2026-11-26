namespace DataPodServices;

public sealed class SerilogToMicrosoftLoggerSink : ILogEventSink
{
    private readonly ILogger logger;

    public SerilogToMicrosoftLoggerSink(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        this.logger = logger;
    }

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        LogLevel level = ToMicrosoftLevel(logEvent.Level);

        if(!this.logger.IsEnabled(level)) { return; }

        List<KeyValuePair<String,Object?>> state = new(logEvent.Properties.Count + 1);

        foreach(KeyValuePair<String,LogEventPropertyValue> property in logEvent.Properties)
        {
            state.Add(new KeyValuePair<String,Object?>(property.Key,ToObject(property.Value)));
        }

        state.Add(new KeyValuePair<String,Object?>("{OriginalFormat}",logEvent.MessageTemplate.Text));

        String renderedMessage = logEvent.RenderMessage(CultureInfo.InvariantCulture);

        this.logger.Log(
            level,
            new EventId(0,logEvent.MessageTemplate.Text),
            state,
            logEvent.Exception,
            (_,_) => renderedMessage);
    }

    private static LogLevel ToMicrosoftLevel(LogEventLevel level) => level switch
    {
        LogEventLevel.Verbose => LogLevel.Trace,
        LogEventLevel.Debug => LogLevel.Debug,
        LogEventLevel.Information => LogLevel.Information,
        LogEventLevel.Warning => LogLevel.Warning,
        LogEventLevel.Error => LogLevel.Error,
        LogEventLevel.Fatal => LogLevel.Critical,
        _ => LogLevel.None
    };

    private static Object? ToObject(LogEventPropertyValue value) => value switch
    {
        ScalarValue scalar => scalar.Value,
        SequenceValue sequence => sequence.Elements.Select(ToObject).ToArray(),
        StructureValue structure => structure.Properties.ToDictionary(property => property.Name,property => ToObject(property.Value)),
        DictionaryValue dictionary => dictionary.Elements.ToDictionary(pair => Convert.ToString(ToObject(pair.Key),CultureInfo.InvariantCulture) ?? String.Empty,pair => ToObject(pair.Value)),
        _ => value.ToString()
    };
}
