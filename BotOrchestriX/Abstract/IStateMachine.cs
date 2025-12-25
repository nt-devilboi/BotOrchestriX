using Stateless;

namespace BotOrchestriX.Abstract;

public interface IStateMachine
{
    void Continue();
    void GoTo(string state);
}

internal class StateMachine(StateMachine<string, Trigger> stateMachine) : IStateMachine
{
    private readonly StateMachine<string, Trigger>.TriggerWithParameters<string> goToSubTask =
        new(Trigger.UserGoToSubTask);

    public void Continue()
    {
        stateMachine.Fire(Trigger.UserWantToContinue);
    }


    public void GoTo(string state)
    {
        stateMachine.Fire(goToSubTask, state);
    }

    public static implicit operator StateMachine(StateMachine<string, Trigger> stateMachine) =>
        new(stateMachine);
}