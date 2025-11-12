using Stateless;

namespace BotOrchestriX.Abstract;

public interface IServiceRegistryFlow
{
    void AddFlow<TState>(List<StateEvent> stateEvents);

    IStateMachine<TState> Wraps<TState>(StateMachine<TState, Trigger> stateMachine) where TState : struct, Enum;
}

public class ServiceRegistryFlow : IServiceRegistryFlow //todo: make a internal class
{
    private readonly Dictionary<Type, List<StateEvent>> Flows = new();

    public void AddFlow<TState>(List<StateEvent> stateEvents)
    {
        Flows.Add(typeof(TState), stateEvents);
    }

    public IStateMachine<TState> Wraps<TState>(StateMachine<TState, Trigger> stateMachine)
        where TState : struct, Enum

    {
        var approveTrigger = stateMachine.SetTriggerParameters<string>(Trigger.UserGoToSubTask);
        foreach (var stateEvent in Flows[typeof(TState)])
        {
            var stateConfiguration = stateMachine.Configure((TState)stateEvent.Source);
            
            if (stateEvent.Trigger == Trigger.UserGoToSubTask)
            {
                stateConfiguration.PermitIf(approveTrigger, (TState)stateEvent.Dest, x => stateEvent.NameHandler == x);
                continue;
            }

            stateConfiguration.Permit(stateEvent.Trigger, (TState)stateEvent.Dest);
        }


        return new StateMachine<TState>(stateMachine);
    }
}

public record StateEvent(Trigger Trigger, Enum Source, Enum Dest, string? NameHandler = null);