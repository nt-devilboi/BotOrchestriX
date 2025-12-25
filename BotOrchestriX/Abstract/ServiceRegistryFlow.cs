using Stateless;

namespace BotOrchestriX.Abstract;

public interface IServiceRegistryFlow
{
    void AddFlow(List<StateEvent> stateEvents);

    IStateMachine Wraps(StateMachine<string, Trigger> stateMachine);
}

public class ServiceRegistryFlow : IServiceRegistryFlow //todo: make a internal class
{
    private readonly Dictionary<string, List<StateEvent>> Flows = new();

    public void AddFlow(List<StateEvent> stateEvents)
    {
        foreach (var stateEvent in stateEvents)
        {
            Flows.Add(stateEvent.Source, stateEvents);
        }
    }

    public IStateMachine Wraps(StateMachine<string, Trigger> stateMachine)
    {
        var approveTrigger = stateMachine.SetTriggerParameters<string>(Trigger.UserGoToSubTask);
        foreach (var stateEvent in Flows[stateMachine.State])
        {
            var stateConfiguration = stateMachine.Configure(stateEvent.Source);

            if (stateEvent.Trigger == Trigger.UserGoToSubTask)
            {
                stateConfiguration.PermitIf(approveTrigger, stateEvent.Dest,
                    x => stateEvent.CanGo == x); // какая-то бесполезная логика.
                continue;
            }

            stateConfiguration.Permit(stateEvent.Trigger, stateEvent.Dest);
        }


        return new StateMachine(stateMachine);
    }
}

public record StateEvent(Trigger Trigger, string Source, string Dest, string? CanGo = null);