namespace KusDepot.Data;

internal sealed partial class Blob
{
    public async Task<Boolean> Store(String? connection , String? id , Byte[]? it , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.StoreStart(id); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null || it is null || connection is null) { Log.Error(BadArg); SetErr(_); ETW.Log.StoreError(BadArg,id); return false; }

            if(await this.Exists(connection,id) is true) { Log.Error(StoreConflictID,id); SetErr(_); ETW.Log.StoreError(StoreConflict,id); return false; }

            BlobContainerClient _0 = new BlobContainerClient(connection,id); await _0.CreateIfNotExistsAsync(); BlobClient _1 = _0.GetBlobClient(id);

            BlobUploadOptions _2 = new BlobUploadOptions(){TransferValidation = new(){ChecksumAlgorithm = StorageChecksumAlgorithm.StorageCrc64}};

            await _1.UploadAsync(BinaryData.FromBytes(it),_2); Log.Information(StoreSuccessID,id); SetOk(_); ETW.Log.StoreSuccess(id); return true;
        }
        catch ( Exception _ ) { Log.Error(_,StoreFailID,id); ETW.Log.StoreError(_.Message,id); return false; }
    }
}