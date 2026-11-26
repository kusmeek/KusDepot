namespace KusDepot.Security.Policy;

/**<include file='PipelineAccessPolicyEngine.xml' path='PipelineAccessPolicyEngine/class[@name="PipelineAccessPolicyEngine"]/main/*'/>*/
public class PipelineAccessPolicyEngine : IAccessPolicyEngine , IAsyncAccessPolicyEngine
{
    /**<include file='PipelineAccessPolicyEngine.xml' path='PipelineAccessPolicyEngine/class[@name="PipelineAccessPolicyEngine"]/field[@name="Stages"]/*'/>*/
    private readonly ImmutableArray<IAccessPolicyStageMode> Stages;

    /**<include file='PipelineAccessPolicyEngine.xml' path='PipelineAccessPolicyEngine/class[@name="PipelineAccessPolicyEngine"]/constructor[@name="Constructor"]/*'/>*/
    public PipelineAccessPolicyEngine(IEnumerable<IAccessPolicyStageMode>? stages)
    {
        this.Stages = stages?.Where(_ => _ is not null).ToImmutableArray() ?? [];
    }

    /**<include file='PipelineAccessPolicyEngine.xml' path='PipelineAccessPolicyEngine/class[@name="PipelineAccessPolicyEngine"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        foreach(IAccessPolicyStageMode stage in this.Stages)
        {
            ArgumentNullException.ThrowIfNull(stage);

            AccessPolicyDecision decision = stage switch
            {
                IAccessPolicyStage synchronous => synchronous.Evaluate(in context),

                IAsyncAccessPolicyStage => throw new InvalidOperationException(SynchronousPolicyEngineRequiresSynchronousPolicyStages),

                _ => throw new InvalidOperationException(ConfiguredPolicyStageModeInvalid)
            };

            if(!decision.Allowed) { return decision; }
        }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='PipelineAccessPolicyEngine.xml' path='PipelineAccessPolicyEngine/class[@name="PipelineAccessPolicyEngine"]/method[@name="EvaluateAsync"]/*'/>*/
    public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        foreach(IAccessPolicyStageMode stage in this.Stages)
        {
            ArgumentNullException.ThrowIfNull(stage);

            AccessPolicyDecision decision = stage switch
            {
                IAsyncAccessPolicyStage asynchronous => await asynchronous.EvaluateAsync(context,cancel).ConfigureAwait(false),

                IAccessPolicyStage synchronous => synchronous.Evaluate(in context),

                _ => throw new InvalidOperationException(ConfiguredPolicyStageModeInvalid)
            };

            if(!decision.Allowed) { return decision; }
        }

        return AccessPolicyDecision.Allow();
    }
}