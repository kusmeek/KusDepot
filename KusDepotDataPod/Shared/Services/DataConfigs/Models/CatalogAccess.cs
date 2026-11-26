namespace DataPodServices.DataConfigs;

[GenerateSerializer]
[Alias("DataPodServices.DataConfigs.CatalogAccess")]
public enum CatalogAccess : Byte
{
    Read = 0,

    Write = 1
}
