namespace KusDepot.Utilities;

internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="SerializeCanonicalGraph"]/*'/>*/
    private static Byte[] SerializeCanonicalGraph(List<HashGraphNode> nodes , HashGraphValue root , Int32[] colors , DeterministicHashingProfile profile , CancellationToken cancel)
    {
        Int32[] order = Enumerable.Range(0,nodes.Count).OrderBy(index => colors[index]).ToArray();

        Int32[] canonicalIDs = new Int32[nodes.Count];

        for(Int32 index = 0; index < order.Length; index++) { canonicalIDs[order[index]] = index; }

        ArrayBufferWriter<Byte> output = new();

        WriteRaw(output,DomainSeparator);
        WriteByte(output,FormatVersion);
        WriteByte(output,(Byte)profile);
        WriteLengthPrefixed(output,SerializeValue(root,canonicalIDs));
        WriteInt32(output,nodes.Count);

        foreach(Int32 nodeIndex in order)
        {
            cancel.ThrowIfCancellationRequested();

            HashGraphNode node = nodes[nodeIndex];

            WriteLengthPrefixed(output,node.Label);

            List<Byte[]> edges = node.Edges.Select(edge => SerializeEdge(edge,canonicalIDs)).ToList();

            edges.Sort(Comparer<Byte[]>.Create(CompareBytes));

            WriteInt32(output,edges.Count);

            foreach(Byte[] edge in edges) { WriteLengthPrefixed(output,edge); }
        }
        return output.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="SerializeEdge"]/*'/>*/
    private static Byte[] SerializeEdge(HashGraphEdge edge , Int32[] canonicalIDs)
    {
        ArrayBufferWriter<Byte> output = new();

        WriteLengthPrefixed(output,edge.Label);

        WriteLengthPrefixed(output,SerializeValue(edge.Value,canonicalIDs));

        return output.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="SerializeValue"]/*'/>*/
    private static Byte[] SerializeValue(HashGraphValue value , Int32[] canonicalIDs)
    {
        if(!value.IsNode) { return CreateFramedValue(HashGraphKind.ValueAtom,value.Atom!); }

        return CreateFramedValue(HashGraphKind.ValueNodeReference,EncodeInt32(canonicalIDs[value.NodeIndex]));
    }
}
