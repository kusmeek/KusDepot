namespace DataPodServices;

public sealed record OrleansConfig
{
    public String ClusterId { get; init; } = "default";

    public String ServiceId { get; init; } = "default";

    public String? AzureTableEndpoint { get; init; }

    public Boolean Validate(Boolean requireAzureTableEndpoint)
    {
        if(String.IsNullOrWhiteSpace(ClusterId) || String.IsNullOrWhiteSpace(ServiceId)) { return false; }

        return !requireAzureTableEndpoint ||
            Uri.TryCreate(AzureTableEndpoint,UriKind.Absolute,out Uri? endpoint) &&
            String.Equals(endpoint.Scheme,Uri.UriSchemeHttps,StringComparison.OrdinalIgnoreCase);
    }

    public static OrleansConfig? FromConfiguration(IConfiguration configuration , Boolean requireAzureTableEndpoint)
    {
        OrleansConfig orleans = configuration.GetSection("Orleans").Get<OrleansConfig>() ?? new OrleansConfig();

        return orleans.Validate(requireAzureTableEndpoint) ? orleans : null;
    }
}