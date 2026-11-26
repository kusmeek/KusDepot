namespace DataPodServices.DataControl;

/**<include file='ITransferMutationScope.xml' path='ITransferMutationScope/interface[@name="ITransferMutationScope"]/main/*'/>*/
public interface ITransferMutationScope : IAsyncDisposable
{
    /**<include file='ITransferMutationScope.xml' path='ITransferMutationScope/interface[@name="ITransferMutationScope"]/property[@name="ItemID"]/*'/>*/
    Guid ItemID { get; }

    /**<include file='ITransferMutationScope.xml' path='ITransferMutationScope/interface[@name="ITransferMutationScope"]/property[@name="SessionID"]/*'/>*/
    Guid SessionID { get; }
}
