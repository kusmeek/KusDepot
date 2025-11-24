namespace KusDepot.Exams;

public sealed class DataItemTest : DataItem
{
    public DataItemTest() {this.Initialize();}

    public override DataItemTest? Clone()
    {
        return Parse<DataItemTest>(this.ToString(),null);
    }
}