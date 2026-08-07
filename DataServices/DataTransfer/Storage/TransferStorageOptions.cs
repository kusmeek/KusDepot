namespace KusDepot.Data.Services.DataTransfer;

/**<include file='TransferStorageOptions.xml' path='TransferStorageOptions/class[@name="TransferStorageOptions"]/main/*'/>*/
public sealed class TransferStorageOptions
{
    /**<include file='TransferStorageOptions.xml' path='TransferStorageOptions/class[@name="TransferStorageOptions"]/property[@name="RootPath"]/*'/>*/
    public String RootPath { get; set; } = Path.Combine(Path.GetTempPath(),"KusDepot","DataTransfer");
}
