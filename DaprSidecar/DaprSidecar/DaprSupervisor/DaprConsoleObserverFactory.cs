using CliWrap.EventStream;
using Serilog;

namespace KusDepot.Dapr;

internal static class DaprConsoleObserverFactory
{
    internal static ConsoleCommandObserver Create(Func<StartedCommandEvent,CancellationToken,Task>? onstarted = null)
    {
        return new ConsoleCommandObserver
        {
            OnStarted = (e,c) => { Log.Logger.Information("Process started. PID = {Pid}",e.ProcessId); return onstarted?.Invoke(e,c) ?? Task.CompletedTask; },

            OnStdOut  = (e,_) => { Log.Logger.Information("{Line}",e.Text);                            return Task.CompletedTask; },

            OnStdErr  = (e,_) => { Log.Logger.Error("{Line}",e.Text);                                  return Task.CompletedTask; },

            OnExited  = (e,_) => { Log.Logger.Information("Process exited. Code = {Code}",e.ExitCode); return Task.CompletedTask; }
        };
    }
}
