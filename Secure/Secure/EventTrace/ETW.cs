namespace KusDepot.Data;

[EventSource(Name = "KusDepot.Data.Secure")]
internal sealed class ETW : EventSource
{
    public static ETW Log { get; } = new();

    public ETW() : base("KusDepot.Data.Secure",EventSourceSettings.EtwSelfDescribingEventFormat) {}

    [Event(1 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void IsAdminStart(String? username)                                         { WriteEvent(1,username); }

    [Event(2 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void IsAdminError(String? message , String? username)                       { WriteEvent(2,message,username); }

    [Event(3 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void IsAdminFinish(String? username)                                        { WriteEvent(3,username); }

    [Event(4 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void SetAdminStart(String? username)                                        { WriteEvent(4,username); }

    [Event(5 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void SetAdminError(String? message , String? username)                      { WriteEvent(5,message,username); }

    [Event(6 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void SetAdminSuccess(String? username)                                      { WriteEvent(6,username); }

    [Event(7 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void ValidateVerifyStart(String? username , String? role)                   { WriteEvent(7,username,role); }

    [Event(8 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void ValidateVerifyError(String? message , String? username , String? role) { WriteEvent(8,message,username,role); }

    [Event(9 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void ValidateVerifyFinish(String? username , String? role)                  { WriteEvent(9,username,role); }

    [Event(10 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnActivateStart()                                                      { WriteEvent(10); }

    [Event(11 , Level = EventLevel.Error , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateError(String? message)                                       { WriteEvent(11,message); }

    [Event(12 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Message = "{0}" , Opcode = EventOpcode.Stop)]
    public void OnActivateSuccess()                                                    { WriteEvent(12); }

    [Event(13 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Start)]
    public void OnDeactivateStart()                                                    { WriteEvent(13); }

    [Event(14 , ActivityOptions = EventActivityOptions.Detachable | EventActivityOptions.Recursive , Opcode = EventOpcode.Stop)]
    public void OnDeactivateSuccess()                                                  { WriteEvent(14); }
}