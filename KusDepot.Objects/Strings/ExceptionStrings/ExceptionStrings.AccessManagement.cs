namespace KusDepot.Strings;

public static partial class ExceptionStrings
{
    #pragma warning disable CS1591

    public const String ConfiguredAssertionGeneratorModeInvalid = "Configured assertion generator mode is invalid.";
    public const String ConfiguredPolicyEngineModeInvalid = "Configured policy engine mode is invalid.";
    public const String ConfiguredPolicyStageModeInvalid = "Configured policy stage mode is invalid.";
    public const String ConfiguredSignatureProcessorModeInvalid = "Configured signature processor mode is invalid.";
    public const String SynchronousAccessManagerRequiresSynchronousAssertionGenerator = "Synchronous access-manager calls require a synchronous assertion generator.";
    public const String SynchronousAccessManagerRequiresSynchronousPolicyEngine = "Synchronous access-manager calls require a synchronous policy engine.";
    public const String SynchronousAccessManagerRequiresSynchronousSignatureProcessor = "Synchronous access-manager calls require a synchronous signature processor.";
    public const String SynchronousAssertionGeneratorRequiresSynchronousChildGenerators = "Synchronous assertion-generator calls require synchronous child generators.";
    public const String SynchronousPolicyEngineRequiresSynchronousPolicyStages = "Synchronous policy-engine calls require synchronous policy stages.";
}