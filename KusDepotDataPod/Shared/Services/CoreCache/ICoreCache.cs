namespace DataPodServices.CoreCache;

[Alias("DataPodServices.ICoreCache")]
public interface ICoreCache : IGrainWithStringKey
{
    [Alias("Delete")]
    Task<Boolean> Delete([Immutable] String? id , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("Exists")]
    Task<Boolean?> Exists([Immutable] String? id , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("Get")]
    Task<String?> Get([Immutable] String? id , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("Store")]
    Task<Boolean> Store([Immutable] String? id , [Immutable] String? it , [Immutable] String? traceid , [Immutable] String? spanid);
}