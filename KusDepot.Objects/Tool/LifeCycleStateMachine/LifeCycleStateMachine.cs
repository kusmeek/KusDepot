namespace KusDepot;

/**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/main/*'/>*/
public sealed class LifeCycleStateMachine
{
    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/property[@name="ToolID"]/*'/>*/
    private Guid? ToolID { get; set; }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/field[@name="startedadmitted"]/*'/>*/
    private Boolean startedadmitted;

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/field[@name="stoppedadmitted"]/*'/>*/
    private Boolean stoppedadmitted;

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/field[@name="StateSync"]/*'/>*/
    private readonly Object StateSync = new();

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/field[@name="state"]/*'/>*/
    private LifeCycleState state = UnInitialized;

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/property[@name="State"]/*'/>*/
    public LifeCycleState State { get { lock(this.StateSync) { return this.state; } } }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/constructor[@name="Constructor"]/*'/>*/
    public LifeCycleStateMachine(Guid? toolid = null) { this.ToolID = toolid; }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="TryAdmitOperation"]/*'/>*/
    public LifeCycleOperationResult TryAdmitOperation(LifeCycleOperation operation , out LifeCycleState previous)
    {
        Boolean entered = false;

        try
        {
            if(!TryEnter(this.StateSync,SyncTime)) { throw SyncException; } entered = true;

            previous = this.state;

            return operation switch
            {
                LifeCycleOperation.Starting => SetStarting(),

                LifeCycleOperation.Start => SetStart(),

                LifeCycleOperation.Started => SetStarted(),

                LifeCycleOperation.Stopping => SetStopping(),

                LifeCycleOperation.Stop => SetStop(),

                LifeCycleOperation.Stopped => SetStopped(),

                _ => LifeCycleOperationResult.Rejected
            };
        }
        finally { if(entered) { Exit(this.StateSync); } }

        LifeCycleOperationResult SetStarting()
        {
            if(this.state is Starting or Active) { return LifeCycleOperationResult.AlreadySatisfied; }

            return SetState(Starting) ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected;
        }

        LifeCycleOperationResult SetStart()
        {
            if(this.state is Active) { return LifeCycleOperationResult.AlreadySatisfied; }

            if(this.state is Starting) { return LifeCycleOperationResult.Entered; }

            return SetState(Starting) ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected;
        }

        LifeCycleOperationResult SetStarted()
        {
            if(this.state is Active)
            {
                if(this.startedadmitted) { return LifeCycleOperationResult.AlreadySatisfied; }

                this.startedadmitted = true; return LifeCycleOperationResult.Entered;
            }

            return LifeCycleOperationResult.Rejected;
        }

        LifeCycleOperationResult SetStopping()
        {
            if(this.state is Stopping or Initialized or InActive) { return LifeCycleOperationResult.AlreadySatisfied; }

            return SetState(Stopping) ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected;
        }

        LifeCycleOperationResult SetStop()
        {
            if(this.state is Initialized or InActive) { return LifeCycleOperationResult.AlreadySatisfied; }

            if(this.state is Stopping) { return LifeCycleOperationResult.Entered; }

            return SetState(Stopping) ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected;
        }

        LifeCycleOperationResult SetStopped()
        {
            if(this.state is Initialized) { return LifeCycleOperationResult.AlreadySatisfied; }

            if(this.state is InActive)
            {
                if(this.stoppedadmitted) { return LifeCycleOperationResult.AlreadySatisfied; }

                this.stoppedadmitted = true; return LifeCycleOperationResult.Entered;
            }

            return LifeCycleOperationResult.Rejected;
        }

        Boolean SetState(LifeCycleState target)
        {
            return this.SetState(target,this.state,false);
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="TrySetState"]/*'/>*/
    private Boolean TrySetState(LifeCycleState target , out LifeCycleState previous , Boolean allowcurrent)
    {
        Boolean entered = false;

        try
        {
            if(!TryEnter(this.StateSync,SyncTime)) { throw SyncException; } entered = true;

            previous = this.state;

            if(previous == target) { return allowcurrent; }

            if(!CanEnterState(previous,target)) { return false; }

            return SetState(target,previous,allowcurrent);
        }
        finally { if(entered) { Exit(this.StateSync); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="SetState"]/*'/>*/
    private Boolean SetState(LifeCycleState target , LifeCycleState previous , Boolean allowcurrent)
    {
        if(previous == target) { return allowcurrent; }

        if(!CanEnterState(previous,target)) { return false; }

        if(target is Starting) { this.startedadmitted = false; }

        if(target is Stopping) { this.stoppedadmitted = false; }

        this.state = target; return true;
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="TryActivate"]/*'/>*/
    public Boolean TryActivate(out LifeCycleState previous)
    {
        return this.TrySetState(Active,out previous,false);
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="TryDeactivate"]/*'/>*/
    public Boolean TryDeactivate(out LifeCycleState previous)
    {
        return this.TrySetState(InActive,out previous,false);
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="InitializeOK"]/*'/>*/
    public Boolean InitializeOK()
    {
        return this.State is Initialized || CanEnterState(this.State,Initialized);
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="TryInitialize"]/*'/>*/
    public Boolean TryInitialize(out LifeCycleState previous)
    {
        return this.TrySetState(Initialized,out previous,true);
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToError"]/*'/>*/
    public Boolean ToError()
    {
        try
        {
            Boolean entered = false;

            try
            {
                if(!TryEnter(this.StateSync,SyncTime)) { throw SyncException; } entered = true;

                this.state = Error; return true;
            }
            finally { if(entered) { Exit(this.StateSync); } }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToErrorFail,ToolID); throw; }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="CanEnterState"]/*'/>*/
    private static Boolean CanEnterState(LifeCycleState previous , LifeCycleState target)
    {
        if(previous is Error) { return target is Error or UnInitialized; }

        return (previous,target) switch
        {
            (UnInitialized,Initialized) => true,

            (Initialized,Active)        => true,

            (Initialized,Starting)      => true,

            (InActive,Initialized)      => true,

            (InActive,Starting)         => true,

            (InActive,Active)           => true,

            (Starting,Active)           => true,

            (Active,Stopping)           => true,

            (Active,InActive)           => true,

            (Stopping,InActive)         => true,

            (_,Error)                   => true,

            _                           => false
        };
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="CommandChangeOK"]/*'/>*/
    public Boolean CommandChangeOK()
    {
        switch(this.State)
        {
            case Active:
            case InActive:
            case Initialized:
            { return true; }

            case Error:
            case Starting:
            case Stopping:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="HostedServicesChangeOK"]/*'/>*/
    public Boolean HostedServicesChangeOK()
    {
        switch(this.State)
        {
            case Active:
            case InActive:
            case Initialized:
            { return true; }

            case Error:
            case Starting:
            case Stopping:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }
}