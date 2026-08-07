namespace KusDepot.Data.Clients;

/**<include file='DataControlClientWorkingDirectoryOptions.xml' path='DataControlClientWorkingDirectoryOptions/record[@name="DataControlClientWorkingDirectoryOptions"]/main/*'/>*/
public sealed record DataControlClientWorkingDirectoryOptions
{
    /**<include file='DataControlClientWorkingDirectoryOptions.xml' path='DataControlClientWorkingDirectoryOptions/record[@name="DataControlClientWorkingDirectoryOptions"]/property[@name="RootPath"]/*'/>*/
    public String RootPath { get; init; } = Path.Combine(Path.GetTempPath(),"KusDepot","DataControlClient");
}
