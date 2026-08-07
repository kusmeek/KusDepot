namespace LabX08;

internal sealed record LabDemoOptions
{
    public String Endpoint { get; init; } = String.Empty;

    public String CertificateSubjectName { get; init; } = "DataControlUser";

    public String WorkingDirectory { get; init; } = Path.Combine("R:","KusDepot","LabWorking","Consumer");

    public String Mode { get; init; } = "Read";

    public Int32 DefaultChunkBytes { get; init; } = 256;

    public Int32 MaximumChunkBytes { get; init; } = 1024;

    public Int32 DefaultPauseMilliseconds { get; init; } = 350;

    public Boolean RandomPauseEnabled { get; init; }

    public Int32 StatusRefreshMilliseconds { get; init; } = 200;
}
