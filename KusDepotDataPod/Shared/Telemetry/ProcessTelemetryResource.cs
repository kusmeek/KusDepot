namespace DataPodServices;

public sealed class ProcessTelemetryResource
{
    private readonly IReadOnlyDictionary<String,Object> attributes;

    public ProcessTelemetryResource(String servicename)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(servicename);

        Dictionary<String,Object> values = ParseResourceAttributes(GetEnvironmentVariable("OTEL_RESOURCE_ATTRIBUTES"));

        values["service.name"] = String.IsNullOrWhiteSpace(GetEnvironmentVariable("OTEL_SERVICE_NAME"))
            ? servicename
            : GetEnvironmentVariable("OTEL_SERVICE_NAME")!;
        values["process.pid"] = ProcessId;
        values["service.version"] = 1;

        this.InstanceID = values.TryGetValue("service.instance.id",out Object? instance) && instance is String id && !String.IsNullOrWhiteSpace(id)
            ? id
            : Guid.NewGuid().ToString("N").ToUpperInvariant();

        values["service.instance.id"] = this.InstanceID;

        this.attributes = new ReadOnlyDictionary<String,Object>(values);
    }

    public String InstanceID { get; }

    public IReadOnlyDictionary<String,Object> Attributes => this.attributes;

    private static Dictionary<String,Object> ParseResourceAttributes(String? input)
    {
        Dictionary<String,Object> values = new(StringComparer.Ordinal);

        if(String.IsNullOrWhiteSpace(input)) { return values; }

        foreach(String attribute in SplitResourceAttributes(input))
        {
            Int32 separator = attribute.IndexOf('=');

            if(separator <= 0) { continue; }

            String key = attribute[..separator].Trim(); String value = attribute[(separator + 1)..].Trim();

            if(String.IsNullOrWhiteSpace(key)) { continue; }

            values[key] = value;
        }

        return values;
    }

    private static IEnumerable<String> SplitResourceAttributes(String input)
    {
        Boolean escaped = false; Int32 start = 0;

        for(Int32 i = 0;i < input.Length;i++)
        {
            if(escaped) { escaped = false; continue; }

            if(input[i] == '\\') { escaped = true; continue; }

            if(input[i] == ',')
            {
                yield return input[start..i]; start = i + 1;
            }
        }

        yield return input[start..];
    }
}
