namespace KusDepot.Utilities;

internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="FormatVersion"]/*'/>*/
    private const Byte FormatVersion = 0x01;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="MaximumGraphNodes"]/*'/>*/
    private const Int32 MaximumGraphNodes = 4096;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="MaximumSearchStates"]/*'/>*/
    private const Int32 MaximumSearchStates = 100000;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="MaximumGraphDepth"]/*'/>*/
    private const Int32 MaximumGraphDepth = 256;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="DomainSeparator"]/*'/>*/
    private static readonly Byte[] DomainSeparator = Encoding.ASCII.GetBytes("KusDepot.DeterministicGraphHash\0");

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="StrictUtf8"]/*'/>*/
    private static readonly UTF8Encoding StrictUtf8 = new(false,true);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="profile"]/*'/>*/
    private readonly DeterministicHashingProfile profile;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="cancel"]/*'/>*/
    private readonly CancellationToken cancel;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="rootDataItem"]/*'/>*/
    private readonly DataItem? rootDataItem;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="rootContentSnapshot"]/*'/>*/
    private readonly ContentIntegritySnapshot? rootContentSnapshot;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="nodes"]/*'/>*/
    private readonly List<HashGraphNode> nodes = new();

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="nodeIndices"]/*'/>*/
    private readonly Dictionary<Object,Int32> nodeIndices = new(ReferenceEqualityComparer.Instance);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="contentNodeIndices"]/*'/>*/
    private readonly Dictionary<DataItem,Int32> contentNodeIndices = new(ReferenceEqualityComparer.Instance);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="snapshotOverrides"]/*'/>*/
    private readonly Dictionary<Object,Object> snapshotOverrides = new(ReferenceEqualityComparer.Instance);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="contentSnapshotOverrides"]/*'/>*/
    private readonly Dictionary<DataItem,ContentIntegritySnapshot> contentSnapshotOverrides = new(ReferenceEqualityComparer.Instance);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="rootMetaBase"]/*'/>*/
    private Object? rootMetaBase;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="searchStates"]/*'/>*/
    private Int32 searchStates;

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/field[@name="graphDepth"]/*'/>*/
    private Int32 graphDepth;
}