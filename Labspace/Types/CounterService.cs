namespace KusDepot.Test;

public sealed class CounterService : Tool
{
    public CounterService() : base() {}

    public CounterService(Int32 initial) : base() { Count = initial; }

    public Int32 Count { get; private set; }

    public Int32 Increment() { return ++Count; }

    public Int32 Decrement() { return --Count; }

    public void Reset() { Count = 0; }

    public override ToolDescriptor? GetToolDescriptor(AccessKey? key = null)
    {
        if(this.AccessCheck(key) is false) { return null; }

        return new ToolDescriptor
        {
            ID = GetID(),
            Type = GetType().FullName,
            Specifications = "Stateful counter service with Increment, Decrement, and Reset operations. Count property reflects current value.",
            ExtendedData = ["Role: Service", "State: Count (Int32)", "Methods: Increment, Decrement, Reset"]
        };
    }
}
