using ToolServiceExam;

namespace KusDepot.ToolService;

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

        return new(){ Content = _.ToList() };
    }

    private static DataItem CreateRandomItem(Type itemType)
    {
        return itemType switch
        {
            Type t when t == typeof(DataSetItem)       => CreateDataSet(2),
            Type t when t == typeof(GuidReferenceItem) => new GuidReferenceItem(){Content = Guid.NewGuid()},
            Type t when t == typeof(TextItem)          => new TextItem() {Content = "Random Text " + Random.Next()},
            Type t when t == typeof(CodeItem)          => new CodeItem() { Content = "Random Code " + Random.Next() },
            Type t when t == typeof(MSBuildItem)       => new MSBuildItem() { Content = "MSBuild Item " + Random.Next() },
            Type t when t == typeof(BinaryItem)        => new BinaryItem() {Content = new Byte[] { (Byte)Random.Next(256),(Byte)Random.Next(256) } },
            Type t when t == typeof(MultiMediaItem)    => new MultiMediaItem() { Content = new Byte[] { (Byte)Random.Next(256),(Byte)Random.Next(256) } },
            Type t when t == typeof(GenericItem)       => new GenericItem() { Content = new List<Object> { Random.Next(),Random.NextDouble(),"Random Generic " + Random.Next() } },
            _ => throw new InvalidOperationException(),
        };
    }
}