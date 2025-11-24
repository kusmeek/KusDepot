namespace KusDepot;

/**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/main/*'/>*/
public sealed class LifeCycleStateMachine
{
    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/property[@name="ToolID"]/*'/>*/
    private Guid? ToolID { get; set; }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/property[@name="State"]/*'/>*/
    public LifeCycleState State { get; private set; } = UnInitialized;

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/constructor[@name="Constructor"]/*'/>*/
    public LifeCycleStateMachine(Guid? toolid = null) { this.ToolID = toolid; }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ActivateOK"]/*'/>*/
    public Boolean ActivateOK()
    {
        switch(this.State)
        {
            case InActive:
            case Initialized:
            case Starting:
            case UnInitialized:
            { return true; }

            case Active:
            case Error:
            case Stopping:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="DeactivateOK"]/*'/>*/
    public Boolean DeactivateOK()
    {
        switch(this.State)
        {
            case Active:
            case Stopping:
            { return true; }

            case Error:
            case InActive:
            case Initialized:
            case Starting:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="InitializeOK"]/*'/>*/
    public Boolean InitializeOK()
    {
        switch(this.State)
        {
            case InActive:
            case Initialized:
            case UnInitialized:
            { return true; }

            case Active:
            case Error:
            case Starting:
            case Stopping:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StartingOK"]/*'/>*/
    public Boolean StartingOK()
    {
        switch(this.State)
        {
            case InActive:
            case Initialized:
            case UnInitialized:
            { return true; }

            case Active:
            case Error:
            case Starting:
            case Stopping:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StartOK"]/*'/>*/
    public Boolean StartOK()
    {
        switch(this.State)
        {
            case InActive:
            case Initialized:
            case Starting:
            case UnInitialized:
            { return true; }

            case Active:
            case Error:
            case Stopping:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StartedOK"]/*'/>*/
    public Boolean StartedOK()
    {
        switch(this.State)
        {
            case Active:
            { return true; }

            case Error:
            case InActive:
            case Initialized:
            case Starting:
            case Stopping:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StoppingOK"]/*'/>*/
    public Boolean StoppingOK()
    {
        switch(this.State)
        {
            case Active:
            { return true; }

            case Error:
            case InActive:
            case Initialized:
            case Starting:
            case Stopping:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StopOK"]/*'/>*/
    public Boolean StopOK()
    {
        switch(this.State)
        {
            case Active:
            case Stopping:
            { return true; }

            case Error:
            case InActive:
            case Initialized:
            case Starting:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StoppedOK"]/*'/>*/
    public Boolean StoppedOK()
    {
        switch(this.State)
        {
            case InActive:
            { return true; }

            case Active:
            case Error:
            case Initialized:
            case Starting:
            case Stopping:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StartHostOK"]/*'/>*/
    public Boolean StartHostOK()
    {
        switch(this.State)
        {
            case InActive:
            case Initialized:
            case UnInitialized:
            { return true; }

            case Active:
            case Error:
            case Starting:
            case Stopping:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="StopHostOK"]/*'/>*/
    public Boolean StopHostOK()
    {
        switch(this.State)
        {
            case Active:
            { return true; }

            case Error:
            case InActive:
            case Initialized:
            case Starting:
            case Stopping:
            case UnInitialized:
            { return false; }

            default: { return false; }
        }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToActive"]/*'/>*/
    public Boolean ToActive()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = Active; return Equals(this.State,Active) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToActiveFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToInActive"]/*'/>*/
    public Boolean ToInActive()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = InActive; return Equals(this.State,InActive) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToInActiveFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToInActive"]/*'/>*/
    public Boolean ToInitialized()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = Initialized; return Equals(this.State,Initialized) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToInitializedFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToError"]/*'/>*/
    public Boolean ToError()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = Error; return Equals(this.State,Error) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToErrorFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToStarting"]/*'/>*/
    public Boolean ToStarting()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = Starting; return Equals(this.State,Starting) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStartingFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToStopping"]/*'/>*/
    public Boolean ToStopping()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = Stopping; return Equals(this.State,Stopping) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStoppingFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }

    /**<include file='LifeCycleStateMachine.xml' path='LifeCycleStateMachine/class[@name="LifeCycleStateMachine"]/method[@name="ToUnInitialized"]/*'/>*/
    public Boolean ToUnInitialized()
    {
        try
        {
            if(!TryEnter(this.State,SyncTime)) { throw SyncException; }

            this.State = UnInitialized; return Equals(this.State,UnInitialized) ? true : throw new OperationFailedException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToUnInitializedFail,ToolID); throw; }

        finally { if(IsEntered(this.State)) { Exit(this.State); } }
    }
}