using Serilog;

namespace KusDepot.ToolActor;

internal static class ToolActorStartUp
{
    private static async Task Main()
    {
        Serilog.Core.Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File($"{LogDirectory}{"ToolActor"}-{ProcessId}.log",formatProvider:InvariantCulture,shared:true).CreateLogger();

            await ActorRuntime.RegisterActorAsync<ToolActor>( (context,type) => new ActorService(context,type,settings:ServiceSettings) );

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }
}