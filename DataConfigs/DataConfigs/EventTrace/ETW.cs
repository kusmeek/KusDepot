namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.Configuration")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.Configuration",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetReadStart()                     { WriteEvent(1); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetReadError(String? message)      { WriteEvent(2,message); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetReadSuccess(String? message)    { WriteEvent(3,message); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetWriteStart()                    { WriteEvent(4); }

    [Event(5 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetWriteError(String? message)     { WriteEvent(5,message); }

    [Event(6 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetWriteSuccess(String? message)   { WriteEvent(6,message); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void GetSilosStart()                    { WriteEvent(7); }

    [Event(8 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetSilosError(String? message)     { WriteEvent(8,message); }

    [Event(9 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void GetSilosSuccess(String? message)   { WriteEvent(9,message); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SetSilosStart()                    { WriteEvent(10); }

    [Event(11 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SetSilosError(String? message)     { WriteEvent(11,message); }

    [Event(12 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SetSilosSuccess(String? message)   { WriteEvent(12,message); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnActivateStart()                  { WriteEvent(13); }

    [Event(14 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateError(String? message)   { WriteEvent(14,message); }

    [Event(15 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateSuccess(String? message) { WriteEvent(15,message); }

    [Event(16 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnDeactivateStart()                { WriteEvent(16); }

    [Event(17 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnDeactivateSuccess()              { WriteEvent(17); }
}