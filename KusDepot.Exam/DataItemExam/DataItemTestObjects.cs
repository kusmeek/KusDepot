using Orleans;

namespace KusDepot.Exams;

[GenerateSerializer] [Alias("KusDepot.DataItemTest")]
public sealed class DataItemTest : DataItem
{
    public DataItemTest() {this.Initialize();}

    public override DataItemTest? Clone()
    {
        return Parse<DataItemTest>(this.ToString(),null);
    }
}