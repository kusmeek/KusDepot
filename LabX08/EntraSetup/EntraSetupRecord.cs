namespace LabX08;

internal sealed record EntraSetupRecord
{
    public String TenantID { get; init; } = String.Empty;

    public String ClientID { get; init; } = String.Empty;

    public String Scope { get; init; } = String.Empty;

    public String AdminUserName { get; init; } = String.Empty;

    public String AdminUserPass { get; init; } = String.Empty;

    public String ReadUserName { get; init; } = String.Empty;

    public String ReadUserPass { get; init; } = String.Empty;

    public String WriteUserName { get; init; } = String.Empty;

    public String WriteUserPass { get; init; } = String.Empty;

    public String ConnectionString { get; init; } = String.Empty;

    public String Authority => String.Concat("https://login.microsoftonline.com/",this.TenantID);
}
