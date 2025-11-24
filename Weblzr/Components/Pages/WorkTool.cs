namespace Weblzr.Components.Pages;

public partial class WorkTool : ComponentBase , IDisposable
{
    private ITool? Tool;

    private Guid? ToolID;

    private Boolean Disposed;

    [Inject]
    public IToolConnect? Connect {get;set;}

    private void Work()
    {
        this.ToolID = this.Tool?.GetID();
    }

    protected override void OnInitialized()
    {
        Tool = Connect?.Tool;
    }

    protected virtual void Dispose(Boolean disposing)
    {
        if(Disposed is false) { if(disposing) { } Disposed = true; }
    }

    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
}