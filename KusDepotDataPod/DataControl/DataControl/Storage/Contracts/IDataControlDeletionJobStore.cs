namespace DataPodServices.DataControl;

/**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/main/*'/>*/
public interface IDataControlDeletionJobStore
{
    /**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/method[@name="AdvanceDeletionAsync"]/*'/>*/
    Task<Boolean> AdvanceDeletionAsync(String catalogname , Guid itemid , Guid itemInstanceToken , DataControlDeletionProgress expectedprogress , DataControlDeletionProgress progress , CancellationToken cancel = default);

    /**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/method[@name="EnsureDeletionJobAsync"]/*'/>*/
    Task<DataControlDeletionJobState> EnsureDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default);

    /**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/method[@name="LoadDeletionJobAsync"]/*'/>*/
    Task<DataControlDeletionJobState?> LoadDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default);

    /**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/method[@name="LoadDueDeletionJobsAsync"]/*'/>*/
    Task<IReadOnlyList<DataControlDeletionJobState>> LoadDueDeletionJobsAsync(DateTimeOffset now , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/method[@name="RecordDeletionFailureAsync"]/*'/>*/
    Task<Boolean> RecordDeletionFailureAsync(String catalogname , Guid itemid , Guid itemInstanceToken , String error , DateTimeOffset nextattempt , CancellationToken cancel = default);

    /**<include file='IDataControlDeletionJobStore.xml' path='IDataControlDeletionJobStore/interface[@name="IDataControlDeletionJobStore"]/method[@name="DeleteDeletionJobAsync"]/*'/>*/
    Task<Boolean> DeleteDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default);
}
