namespace KusDepot.Utilities;

internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="SearchCanonicalEncoding"]/*'/>*/
    private static Byte[] SearchCanonicalEncoding(List<HashGraphNode> nodes , HashGraphValue root , Int32[] individualized , DeterministicHashingProfile profile , CancellationToken cancel , ref Int32 searchStates , Int32 depth)
    {
        cancel.ThrowIfCancellationRequested();

        if(++searchStates > MaximumSearchStates) { throw new InvalidOperationException($"Deterministic hashing exceeded the {MaximumSearchStates} canonical-labeling search-state limit."); }

        Int32[] colors = RefineColors(nodes,root,individualized,cancel);

        Int32[]? ambiguousClass = FindAmbiguousClass(colors);

        if(ambiguousClass is null) { return SerializeCanonicalGraph(nodes,root,colors,profile,cancel); }

        Byte[]? best = null;

        foreach(Int32 candidate in ambiguousClass)
        {
            cancel.ThrowIfCancellationRequested();

            Int32[] branch = (Int32[])individualized.Clone();

            branch[candidate] = depth + 1;

            Byte[] encoding = SearchCanonicalEncoding(nodes,root,branch,profile,cancel,ref searchStates,depth+1);

            if(best is null || CompareBytes(encoding,best) < 0) { best = encoding; }
        }
        return best!;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="RefineColors"]/*'/>*/
    private static Int32[] RefineColors(List<HashGraphNode> nodes , HashGraphValue root , Int32[] individualized , CancellationToken cancel)
    {
        List<(Int32 Source,Byte[] Label)>[] incoming = Enumerable.Range(0,nodes.Count).Select(_ => new List<(Int32,Byte[])>()).ToArray();

        for(Int32 source = 0; source < nodes.Count; source++)
        {
            foreach(HashGraphEdge edge in nodes[source].Edges)
            {
                if(edge.Value.IsNode) { incoming[edge.Value.NodeIndex].Add((source,edge.Label)); }
            }
        }

        Int32[] colors = AssignColors(Enumerable.Range(0,nodes.Count).Select(index => BuildNodeSignature(nodes,root,index,individualized,null,null,incoming,cancel)).ToArray());

        Int32 colorCount = colors.Distinct().Count();

        while(nodes.Count > 0)
        {
            cancel.ThrowIfCancellationRequested();

            Int32[] refined = AssignColors(Enumerable.Range(0,nodes.Count).Select(index => BuildNodeSignature(nodes,root,index,individualized,colors,colors,incoming,cancel)).ToArray());

            Int32 refinedCount = refined.Distinct().Count();

            if(refinedCount == colorCount) { return refined; }

            colors = refined;

            colorCount = refinedCount;
        }
        return colors;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="BuildNodeSignature"]/*'/>*/
    private static Byte[] BuildNodeSignature(List<HashGraphNode> nodes , HashGraphValue root , Int32 index , Int32[] individualized , Int32[]? outgoingColors , Int32[]? incomingColors , List<(Int32 Source,Byte[] Label)>[] incoming , CancellationToken cancel)
    {
        HashGraphNode node = nodes[index];

        ArrayBufferWriter<Byte> signature = new();

        WriteLengthPrefixed(signature,node.Label);

        WriteInt32(signature,individualized[index]);

        WriteByte(signature,root.IsNode && root.NodeIndex == index ? (Byte)1 : (Byte)0);

        if(outgoingColors is not null) { WriteInt32(signature,outgoingColors[index]); }

        List<Byte[]> outgoing = new(node.Edges.Count);

        foreach(HashGraphEdge edge in node.Edges)
        {
            cancel.ThrowIfCancellationRequested();

            outgoing.Add(BuildIncidenceSignature(edge.Label,edge.Value,outgoingColors));
        }

        outgoing.Sort(Comparer<Byte[]>.Create(CompareBytes));

        WriteInt32(signature,outgoing.Count);

        foreach(Byte[] incidence in outgoing) { WriteLengthPrefixed(signature,incidence); }

        List<Byte[]> incomingSignatures = new(incoming[index].Count);

        foreach((Int32 source,Byte[] label) in incoming[index])
        {
            ArrayBufferWriter<Byte> incidence = new();

            WriteLengthPrefixed(incidence,label);

            WriteInt32(incidence,incomingColors is null ? -1 : incomingColors[source]);

            incomingSignatures.Add(incidence.WrittenSpan.ToArray());

        }

        incomingSignatures.Sort(Comparer<Byte[]>.Create(CompareBytes));

        WriteInt32(signature,incomingSignatures.Count);

        foreach(Byte[] incidence in incomingSignatures) { WriteLengthPrefixed(signature,incidence); }

        return signature.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="BuildIncidenceSignature"]/*'/>*/
    private static Byte[] BuildIncidenceSignature(Byte[] label , HashGraphValue value , Int32[]? colors)
    {
        ArrayBufferWriter<Byte> incidence = new();

        WriteLengthPrefixed(incidence,label);

        if(value.IsNode)
        {
            WriteByte(incidence,HashGraphKind.IncidenceNodeReference);

            WriteInt32(incidence,colors is null ? -1 : colors[value.NodeIndex]);
        }
        else
        {
            WriteByte(incidence,HashGraphKind.IncidenceAtom);

            WriteLengthPrefixed(incidence,value.Atom!);
        }
        return incidence.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="AssignColors"]/*'/>*/
    private static Int32[] AssignColors(Byte[][] signatures)
    {
        Int32[] order = Enumerable.Range(0,signatures.Length).ToArray();

        Array.Sort(order,(left,right) => CompareBytes(signatures[left],signatures[right]));

        Int32[] colors = new Int32[signatures.Length]; Int32 color = -1; Byte[]? previous = null;

        foreach(Int32 index in order)
        {
            if(previous is null || CompareBytes(previous,signatures[index]) != 0) { color++; previous = signatures[index]; }

            colors[index] = color;
        }
        return colors;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="FindAmbiguousClass"]/*'/>*/
    private static Int32[]? FindAmbiguousClass(Int32[] colors)
    {
        foreach(IGrouping<Int32,Int32> group in Enumerable.Range(0,colors.Length).GroupBy(index => colors[index]).OrderBy(group => group.Key))
        {
            Int32[] members = group.ToArray();

            if(members.Length > 1) { return members; }
        }
        return null;
    }
}
