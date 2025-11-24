namespace KusDepot.Data;

public static class DataControlFactory
{
    public static DataControl Create(StatelessServiceContext context)
    {
        return new DataControl(context);
    }
}