namespace KusDepot.Data.Services.Blob;

/**<include file='BlobService.xml' path='BlobService/class[@name="BlobService"]/main/*'/>*/
public sealed partial class BlobService : IBlob
{
    /**<include file='BlobService.xml' path='BlobService/class[@name="BlobService"]/field[@name="id"]/*'/>*/
    private String? id;

    /**<include file='BlobService.xml' path='BlobService/class[@name="BlobService"]/field[@name="version"]/*'/>*/
    private String? version;

    /**<include file='BlobService.xml' path='BlobService/class[@name="BlobService"]/field[@name="connection"]/*'/>*/
    private String? connection;

    /**<include file='BlobService.xml' path='BlobService/class[@name="BlobService"]/field[@name="blobclient"]/*'/>*/
    private BlobClient? blobclient;

    /**<include file='BlobService.xml' path='BlobService/class[@name="BlobService"]/field[@name="containerclient"]/*'/>*/
    private BlobContainerClient? containerclient;

    ///<inheritdoc/>
    public Boolean Initialize(String? connection , String? id , String? version = null)
    {
        this.connection = connection; this.id = id; this.version = version;

        if(this.connection is not null && this.id is not null)
        {
            containerclient = new BlobContainerClient(this.connection,this.id);

            blobclient = new BlobClient(this.connection,this.id,this.id);

            if(this.version is not null) { blobclient = blobclient.WithVersion(this.version); }
        }

        return this.blobclient is not null && this.containerclient is not null;
    }
}