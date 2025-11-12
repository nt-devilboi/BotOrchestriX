using Stateless;

namespace BotOrchestriX.Abstract;

public interface IStateMachine<in TState>
{
    void Continue();
    void GoTo(TState state);
}

internal class StateMachine<TState>(StateMachine<TState, Trigger> stateMachine) : IStateMachine<TState> where TState : Enum 
{
    private readonly StateMachine<TState, Trigger>.TriggerWithParameters<string> goToSubTask =
        new(Trigger.UserGoToSubTask);

    public void Continue()
    {
        stateMachine.Fire(Trigger.UserWantToContinue);
    }

    public void GoTo(TState state)
    {
        stateMachine.Fire(goToSubTask, state.ToString());
    }

    public static implicit operator StateMachine<TState>(StateMachine<TState, Trigger> stateMachine) =>
        new(stateMachine);
}