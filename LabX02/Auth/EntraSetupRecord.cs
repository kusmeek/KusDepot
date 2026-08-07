namespace LabX02;

internal sealed record EntraSetupRecord
{
    public String Authority { get; init; } = String.Empty;

    public String ClientID { get; init; } = String.Empty;

    public String Scope { get; init; } = String.Empty;

    public String AdminUserName { get; init; } = String.Empty;

    public String AdminUserPass { get; init; } = String.Empty;

    public String ReadUserName { get; init; } = String.Empty;

    public String ReadUserPass { get; init; } = String.Empty;

    public String WriteUserName { get; init; } = String.Empty;

    public String WriteUserPass { get; init; } = String.Empty;
}
