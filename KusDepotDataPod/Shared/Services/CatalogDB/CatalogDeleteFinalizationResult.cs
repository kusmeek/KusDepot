namespace DataPodServices.CatalogDB;

/**<include file='CatalogDeleteFinalizationResult.xml' path='CatalogDeleteFinalizationResult/enum[@name="CatalogDeleteFinalizationResult"]/main/*'/>*/
[GenerateSerializer]
[Alias("DataPodServices.CatalogDB.CatalogDeleteFinalizationResult")]
public enum CatalogDeleteFinalizationResult : Byte
{
    /**<include file='CatalogDeleteFinalizationResult.xml' path='CatalogDeleteFinalizationResult/enum[@name="CatalogDeleteFinalizationResult"]/field[@name="Removed"]/*'/>*/
    Removed = 0,
    /**<include file='CatalogDeleteFinalizationResult.xml' path='CatalogDeleteFinalizationResult/enum[@name="CatalogDeleteFinalizationResult"]/field[@name="AlreadyAbsent"]/*'/>*/
    AlreadyAbsent = 1,
    /**<include file='CatalogDeleteFinalizationResult.xml' path='CatalogDeleteFinalizationResult/enum[@name="CatalogDeleteFinalizationResult"]/field[@name="TokenMismatch"]/*'/>*/
    TokenMismatch = 2,
    /**<include file='CatalogDeleteFinalizationResult.xml' path='CatalogDeleteFinalizationResult/enum[@name="CatalogDeleteFinalizationResult"]/field[@name="NotPending"]/*'/>*/
    NotPending = 3
}
