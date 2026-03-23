namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodSetWorkingDirectory")]
    [Description(Descriptions.SetWorkingDirectory)]
    public static ToolPodResult SetWorkingDirectory(
        [Description(SetWorkingDirectoryPath)] String path)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(path)) { return ErrorResult(ErrorStrings.PathRequired); }

            String fullPath = Path.GetFullPath(path); String? root = Path.GetPathRoot(fullPath);

            String systemRoot = Path.GetPathRoot(Environment.SystemDirectory) ?? String.Empty;

            if(!String.IsNullOrWhiteSpace(root) && String.Equals(root,systemRoot,StringComparison.OrdinalIgnoreCase))
            {
                return ErrorResult(ErrorStrings.SystemDriveNotAllowed);
            }

            lock(Sync)
            {
                if(!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                activeworkingdirectory = fullPath;
            }

            LogDiagnostic($"Set working directory to {fullPath}");

            return ValueResult(JsonSerializer.Serialize(new { WorkingDirectory = fullPath }),typeof(String).FullName);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.SetWorkingDirectoryFailed); return ErrorResult(_.Message); }
    }
}