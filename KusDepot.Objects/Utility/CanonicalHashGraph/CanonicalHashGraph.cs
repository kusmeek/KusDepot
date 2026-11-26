namespace KusDepot.Utilities;

/**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/main/*'/>*/
internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/constructor[@name="CanonicalHashGraph"]/*'/>*/
    public CanonicalHashGraph(DeterministicHashingProfile profile , CancellationToken cancel , DataItem? rootDataItem = null , ContentIntegritySnapshot? rootContentSnapshot = null)
    {
        if(!Enum.IsDefined(profile)) { throw new ArgumentOutOfRangeException(nameof(profile)); }

        if((rootDataItem is null) != (rootContentSnapshot is null)) { throw new ArgumentException("Root data item and content snapshot must be provided together."); }

        this.profile = profile; this.cancel = cancel; this.rootDataItem = rootDataItem; this.rootContentSnapshot = rootContentSnapshot;
    }
}