using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BotOrchestriX.Abstract;

internal sealed record RouterTriggerDescriptor(string Trigger);

internal sealed class TriggerProvider
{
    private readonly RouterTriggerDescriptor[] _map;

    public TriggerProvider(IEnumerable<RouterTriggerDescriptor> descriptors)
    {
        _map = descriptors.ToArray();
    }
    
    public ReadOnlySpan<RouterTriggerDescriptor> GetAll()
    {
        return _map;
    }
}