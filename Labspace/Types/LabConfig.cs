namespace KusDepot.Test;

public sealed class LabConfig
{
    public LabConfig() {}

    public LabConfig(String name , Int32 maxItems , Boolean verbose)
    {
        Name = name;
        MaxItems = maxItems;
        Verbose = verbose;
    }

    public String Name { get; set; } = String.Empty;

    public Int32 MaxItems { get; set; } = 100;

    public Boolean Verbose { get; set; }
}
