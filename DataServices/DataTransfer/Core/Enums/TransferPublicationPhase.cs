namespace KusDepot.Data.Transfer;

/**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/main/*'/>*/
public enum TransferPublicationPhase
{
    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="Prepared"]/*'/>*/
    Prepared = 0,

    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="Publishing"]/*'/>*/
    Publishing = 1,

    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="Published"]/*'/>*/
    Published = 2,

    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="CatalogUpdated"]/*'/>*/
    CatalogUpdated = 3,

    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="Finalized"]/*'/>*/
    Finalized = 4,

    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="OperatorActionRequired"]/*'/>*/
    OperatorActionRequired = 5,

    /**<include file='TransferPublicationPhase.xml' path='TransferPublicationPhase/enum[@name="TransferPublicationPhase"]/value[@name="Failed"]/*'/>*/
    Failed = 6
}