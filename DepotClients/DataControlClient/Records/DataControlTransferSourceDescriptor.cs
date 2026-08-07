namespace KusDepot.Data.Clients;

/**<include file='DataControlTransferSourceDescriptor.xml' path='DataControlTransferSourceDescriptor/record[@name="DataControlTransferSourceDescriptor"]/main/*'/>*/
public sealed record DataControlTransferSourceDescriptor
{
    /**<include file='DataControlTransferSourceDescriptor.xml' path='DataControlTransferSourceDescriptor/record[@name="DataControlTransferSourceDescriptor"]/property[@name="Kind"]/*'/>*/
    public DataControlTransferSourceKind Kind { get; init; }

    /**<include file='DataControlTransferSourceDescriptor.xml' path='DataControlTransferSourceDescriptor/record[@name="DataControlTransferSourceDescriptor"]/property[@name="Length"]/*'/>*/
    public Int64? Length { get; init; }

    /**<include file='DataControlTransferSourceDescriptor.xml' path='DataControlTransferSourceDescriptor/record[@name="DataControlTransferSourceDescriptor"]/property[@name="Path"]/*'/>*/
    public String? Path { get; init; }
}
