namespace KusDepot.FabricExams;

public static class DataItemGenerator
{
    private static readonly Random Random = new();

    private static readonly Type[] DataItemTypes =
    {
        typeof(DataSetItem),
        typeof(TextItem),typeof(CodeItem),typeof(BinaryItem),
        typeof(MultiMediaItem),typeof(MSBuildItem),typeof(GenericItem),typeof(GuidReferenceItem),
    };

    public static DataSetItem CreateDataSet(Int32 depth)
    {
        HashSet<DataItem> _ =  new();

        for(Int32 i = 0; i < depth; i++) { _.Add(CreateRandomItem(DataItemTypes[Random.Next(DataItemTypes.Length)])); }

        return new(content:_);
    }

    private static DataItem CreateRandomItem(Type itemType)
    {
        return itemType switch
        {
            Type t when t == typeof(DataSetItem)       => CreateDataSet(2),
            Type t when t == typeof(GuidReferenceItem) => new GuidReferenceItem(content: Guid.NewGuid()),
            Type t when t == typeof(TextItem)          => new TextItem(content: "Random Text " + Random.Next()),
            Type t when t == typeof(CodeItem)          => new CodeItem(content: "Random Code " + Random.Next()),
            Type t when t == typeof(MSBuildItem)       => new MSBuildItem(content: "MSBuild Item " + Random.Next()),
            Type t when t == typeof(BinaryItem)        => new BinaryItem(content: new Byte[] { (Byte)Random.Next(256),(Byte)Random.Next(256) }),
            Type t when t == typeof(MultiMediaItem)    => new MultiMediaItem(content: new Byte[] { (Byte)Random.Next(256),(Byte)Random.Next(256) }),
            Type t when t == typeof(GenericItem)       => new GenericItem(content: new List<Object> { Random.Next(), Random.NextDouble(), "Random Generic " + Random.Next() }),
            _ => throw new InvalidOperationException(),
        };
    }
}