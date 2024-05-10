public abstract class State
{
    protected FiniteStateMachine stateMachine;
    protected State nextState;

    public void Initialization(FiniteStateMachine stateMachine, State nextState)
    {
        this.stateMachine = stateMachine;
        this.nextState = nextState;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate()
    {
        DoCheck();
    }

    public virtual void DoCheck() { }

}

