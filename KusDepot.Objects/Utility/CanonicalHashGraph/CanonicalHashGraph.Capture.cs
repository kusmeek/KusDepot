namespace KusDepot.Utilities;

internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="Compute"]/*'/>*/
    public Byte[] Compute(Object? input)
    {
        List<Exception>? operationErrors = null;

        try
        {
            if(input is MetaBase && this.profile is DeterministicHashingProfile.MetaBaseIntegrity) { this.rootMetaBase = input; }

            if(this.rootDataItem is not null) { this.contentSnapshotOverrides[this.rootDataItem] = this.rootContentSnapshot!; }

            HashGraphValue root = this.GetValue(this.rootDataItem ?? input,input is MetaBase && this.profile is DeterministicHashingProfile.MetaBaseIntegrity);

            Int32[] individualized = new Int32[this.nodes.Count];

            return SearchCanonicalEncoding(this.nodes,root,individualized,this.profile,this.cancel,ref this.searchStates,0);
        }
        catch ( Exception error )
        {
            operationErrors = new List<Exception> { error };

            throw;
        }
        finally
        {
            #pragma warning disable CA2219

            HashSet<ContentIntegritySnapshot> snapshots = new(ReferenceEqualityComparer.Instance);

            List<Exception>? disposalErrors = null;

            if(input is ContentIntegritySnapshot rootSnapshot) { snapshots.Add(rootSnapshot); }

            foreach(ContentIntegritySnapshot snapshot in this.contentSnapshotOverrides.Values) { snapshots.Add(snapshot); }

            foreach(ContentIntegritySnapshot snapshot in snapshots)
            {
                try { snapshot.Dispose(); }

                catch ( Exception error ) { (disposalErrors ??= new List<Exception>()).Add(error); }
            }

            if(disposalErrors is not null)
            {
                if(operationErrors is not null) { operationErrors.AddRange(disposalErrors); }

                else { throw new AggregateException("Canonical graph computation succeeded, but one or more snapshots could not be disposed.",disposalErrors); }
            }

            if(operationErrors is { Count: > 1 }) { throw new AggregateException("Canonical graph computation and cleanup both failed.",operationErrors); }

            #pragma warning restore CA2219
        }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GetValue"]/*'/>*/
    private HashGraphValue GetValue(Object? input , Boolean rootMetaBaseView = false)
    {
        this.cancel.ThrowIfCancellationRequested();

        if(input is null) { return HashGraphValue.FromAtom(EncodeAtom(HashGraphKind.NullAtom,ReadOnlySpan<Byte>.Empty)); }

        if(input is ContentIntegrityValue content) { return HashGraphValue.FromAtom(EncodeAtom(HashGraphKind.ByteArrayAtom,content.ComputeDigest(this.cancel))); }

        if(input is not Byte[] && TryEncodeScalar(input,out Byte[]? scalar)) { return HashGraphValue.FromAtom(scalar!); }

        Boolean contentView = input is DataItem && !rootMetaBaseView;

        if(contentView)
        {
            if(this.contentNodeIndices.TryGetValue((DataItem)input,out Int32 existingContent)) { return HashGraphValue.FromNode(existingContent); }
        }
        else if(this.nodeIndices.TryGetValue(input,out Int32 existing)) { return HashGraphValue.FromNode(existing); }

        if(this.nodes.Count >= MaximumGraphNodes) { throw new InvalidOperationException($"Deterministic hashing exceeded the {MaximumGraphNodes} node limit."); }

        if(++this.graphDepth > MaximumGraphDepth) { this.graphDepth--; throw new InvalidOperationException($"Deterministic hashing exceeded the {MaximumGraphDepth} graph-depth limit."); }

        Int32 nodeIndex = this.nodes.Count;

        Object graphNodeType = ReferenceEquals(input,this.rootDataItem) ? this.rootContentSnapshot! : input;

        HashGraphNode node = new(CreateNodeLabel(input is Byte[] ? HashGraphKind.ByteArrayNode : HashGraphKind.ObjectNode,GetCanonicalTypeName(graphNodeType.GetType()),ReadOnlySpan<Byte>.Empty));

        this.nodes.Add(node);

        if(contentView) { this.contentNodeIndices.Add((DataItem)input,nodeIndex); }

        else if(!input.GetType().IsValueType) { this.nodeIndices.Add(input,nodeIndex); }

        try
        {
            if(input is MetaBase metaBase && this.profile is DeterministicHashingProfile.MetaBaseIntegrity && (input is not IDataItem || rootMetaBaseView))
            {
                this.snapshotOverrides[input] = metaBase.CreateIntegrityHashSnapshot(this.snapshotOverrides);
            }
            else if(contentView && input is DataItem dataItem && !this.contentSnapshotOverrides.ContainsKey(dataItem))
            {
                this.contentSnapshotOverrides[dataItem] = dataItem.CaptureContentIntegritySnapshotForHashing();
            }

            Object nodeSnapshot = contentView
                ? this.contentSnapshotOverrides[(DataItem)input]
                : this.snapshotOverrides.TryGetValue(input,out Object? snapshot) ? snapshot : input;

            this.PopulateNode(input,node,nodeSnapshot,rootMetaBaseView && ReferenceEquals(input,this.rootMetaBase));
        }
        finally { this.graphDepth--; }

        return HashGraphValue.FromNode(nodeIndex);
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="PopulateNode"]/*'/>*/
    private void PopulateNode(Object input , HashGraphNode node , Object snapshot , Boolean rootMetaBaseView)
    {
        if(input is MetaBase metaBase && this.profile is DeterministicHashingProfile.MetaBaseIntegrity && (input is not IDataItem || rootMetaBaseView))
        {
            Dictionary<String,Object?> fields = this.snapshotOverrides.TryGetValue(input,out Object? integritySnapshot) ? (Dictionary<String,Object?>)integritySnapshot : metaBase.CreateIntegrityHashSnapshot(this.snapshotOverrides);

            foreach(KeyValuePair<String,Object?> field in fields.OrderBy(pair => pair.Key,StringComparer.Ordinal))
            {
                if(field.Key == "ID" && ReferenceEquals(input,this.rootMetaBase)) { continue; }

                node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.PropertyEdge,StrictUtf8.GetBytes(field.Key)),this.GetValue(field.Value)));
            }
            return;
        }

        if(snapshot is ContentIntegritySnapshot contentSnapshot)
        {
            if(input is DataItem contentDataItem && !ReferenceEquals(input,this.rootDataItem))
            {
                Byte[] identity = EncodeDataItemIdentity(input.GetType(),contentDataItem.GetID());

                node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.PropertyEdge,StrictUtf8.GetBytes("Identity")),HashGraphValue.FromAtom(identity)));
            }

            this.PopulateContentIntegritySnapshot(contentSnapshot,node);

            return;
        }

        if(input is DataItem projectedDataItem)
        {
            Byte[] identity = EncodeDataItemIdentity(input.GetType(),projectedDataItem.GetID());

            node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.PropertyEdge,StrictUtf8.GetBytes("Identity")),HashGraphValue.FromAtom(identity)));

            ContentIntegritySnapshot content = (ContentIntegritySnapshot)this.snapshotOverrides[input];

            foreach(KeyValuePair<String,Object?> field in content.Fields)
            {
                this.cancel.ThrowIfCancellationRequested();

                node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.PropertyEdge,StrictUtf8.GetBytes(field.Key)),this.GetValue(field.Value)));
            }
            return;
        }

        if(snapshot is IDictionary dictionary)
        {
            this.PopulateDictionary(dictionary,node);

            return;
        }

        if(snapshot is Array array)
        {
            if(array is Byte[] bytes)
            {
                node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.ByteArrayPayloadEdge,ReadOnlySpan<Byte>.Empty),HashGraphValue.FromAtom(EncodeAtom(HashGraphKind.ByteArrayAtom,bytes))));
            }
            else { this.PopulateArray(array,node); }

            return;
        }

        if(snapshot is IEnumerable enumerable)
        {
            this.PopulateEnumerable(enumerable,node,IsGraphUnorderedCollection(input.GetType()) || IsGraphUnorderedDictionary(input.GetType()));

            return;
        }

        this.PopulateObject(input,node);
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="PopulateContentIntegritySnapshot"]/*'/>*/
    private void PopulateContentIntegritySnapshot(ContentIntegritySnapshot snapshot , HashGraphNode node)
    {
        foreach(KeyValuePair<String,Object?> field in snapshot.Fields)
        {
            this.cancel.ThrowIfCancellationRequested();

            node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.PropertyEdge,StrictUtf8.GetBytes(field.Key)),this.GetValue(field.Value)));
        }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="PopulateDictionary"]/*'/>*/
    private void PopulateDictionary(IDictionary dictionary , HashGraphNode node)
    {
        foreach(DictionaryEntry entry in dictionary)
        {
            this.cancel.ThrowIfCancellationRequested();

            if(this.nodes.Count >= MaximumGraphNodes) { throw new InvalidOperationException($"Deterministic hashing exceeded the {MaximumGraphNodes} node limit."); }

            HashGraphNode entryNode = new(CreateNodeLabel(HashGraphKind.DictionaryEntryNode,"DictionaryEntry",ReadOnlySpan<Byte>.Empty));

            Int32 entryIndex = this.nodes.Count;

            this.nodes.Add(entryNode);

            entryNode.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.DictionaryFieldEdge,StrictUtf8.GetBytes("Key")),this.GetValue(entry.Key)));

            entryNode.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.DictionaryFieldEdge,StrictUtf8.GetBytes("Value")),this.GetValue(entry.Value)));

            node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.DictionaryEntryEdge,ReadOnlySpan<Byte>.Empty),HashGraphValue.FromNode(entryIndex)));
        }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="PopulateArray"]/*'/>*/
    private void PopulateArray(Array array , HashGraphNode node)
    {
        ArrayBufferWriter<Byte> shape = new();

        WriteInt32(shape,array.Rank);

        for(Int32 dimension = 0; dimension < array.Rank; dimension++)
        {
            WriteInt32(shape,array.GetLength(dimension));

            WriteInt32(shape,array.GetLowerBound(dimension));
        }
        node.Label = CreateNodeLabel(HashGraphKind.ArrayNode,GetCanonicalTypeName(array.GetType()),shape.WrittenSpan);

        Int32 index = 0;

        foreach(Object? element in array)
        {
            this.cancel.ThrowIfCancellationRequested();

            node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.OrderedElementEdge,EncodeInt32(index++)),this.GetValue(element)));
        }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="PopulateEnumerable"]/*'/>*/
    private void PopulateEnumerable(IEnumerable enumerable , HashGraphNode node , Boolean unordered)
    {
        Int32 index = 0;

        foreach(Object? element in enumerable)
        {
            this.cancel.ThrowIfCancellationRequested();

            Byte[] label = unordered ? CreateEdgeLabel(HashGraphKind.UnorderedElementEdge,ReadOnlySpan<Byte>.Empty) : CreateEdgeLabel(HashGraphKind.OrderedElementEdge,EncodeInt32(index));

            node.Edges.Add(new HashGraphEdge(label,this.GetValue(element)));

            index++;
        }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="PopulateObject"]/*'/>*/
    private void PopulateObject(Object input , HashGraphNode node)
    {
        PropertyInfo[] properties = input.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0 && property.GetMethod is { IsPublic:true, IsStatic:false })
            .OrderBy(property => property.Name,StringComparer.Ordinal)
            .ThenBy(property => GetCanonicalTypeName(property.DeclaringType ?? input.GetType()),StringComparer.Ordinal)
            .ToArray();

        foreach(PropertyInfo property in properties)
        {
            this.cancel.ThrowIfCancellationRequested();

            ArrayBufferWriter<Byte> label = new();

            WriteLengthPrefixed(label,StrictUtf8.GetBytes(property.Name));

            WriteLengthPrefixed(label,StrictUtf8.GetBytes(GetCanonicalTypeName(property.DeclaringType ?? input.GetType())));

            WriteLengthPrefixed(label,StrictUtf8.GetBytes(GetCanonicalTypeName(property.PropertyType)));

            node.Edges.Add(new HashGraphEdge(CreateEdgeLabel(HashGraphKind.PropertyEdge,label.WrittenSpan),this.GetValue(property.GetValue(input))));
        }
    }
}
