namespace KusDepot.Test;

public sealed class EchoService : Tool
{
    private readonly List<String> Store = new();

    public EchoService() : base() {}

    public String[] Messages => Store.ToArray();

    public Int32 MessageCount => Store.Count;

    public String Post(String message) { Store.Add(message); return message; }

    public void Clear() { Store.Clear(); }

    public override ToolDescriptor? GetToolDescriptor(AccessKey? key = null)
    {
        if(this.AccessCheck(key) is false) { return null; }

        return new ToolDescriptor
        {
            ID = GetID(),
            Type = GetType().FullName,
            Specifications = "Message store service. Post appends messages, Messages returns the array, Clear empties the store.",
            ExtendedData = ["Role: Service", "State: Messages (String[]), MessageCount (Int32)", "Methods: Post, Clear"]
        };
    }
}
