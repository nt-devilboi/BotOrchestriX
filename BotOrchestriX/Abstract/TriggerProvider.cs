using System.Collections.Generic;
using System.Linq;

namespace BotOrchestriX.Abstract;

public interface IRouterTriggerDescriptor
{
    Type StateType { get; }
    string Trigger { get; }
}

internal sealed record RouterTriggerDescriptor(Type StateType, string Trigger) : IRouterTriggerDescriptor;

public interface ITriggerProvider
{
    string GetTrigger<TState>() where TState : struct, Enum;
    bool TryGetTrigger(Type stateType, out string trigger);
    IReadOnlyDictionary<Type, string> GetAll();
}

internal sealed class TriggerProvider : ITriggerProvider
{
    private readonly IReadOnlyDictionary<Type, string> _map;

    public TriggerProvider(IEnumerable<IRouterTriggerDescriptor> descriptors)
    {
        _map = descriptors
            .ToDictionary(x => x.StateType, g => g.Trigger);
    }

    public string GetTrigger<TState>() where TState : struct, Enum
    {
        var type = typeof(TState);
        if (!_map.TryGetValue(type, out var trigger))
            throw new KeyNotFoundException($"Trigger for state '{type.FullName}' is not registered.");
        return trigger;
    }

    public bool TryGetTrigger(Type stateType, out string trigger)
    {
        return _map.TryGetValue(stateType, out trigger);
    }

    public IReadOnlyDictionary<Type, string> GetAll()
    {
        return _map;
    }
}