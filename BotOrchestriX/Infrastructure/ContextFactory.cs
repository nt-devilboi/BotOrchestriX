using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;

namespace BotOrchestriX;

internal class ContextFactory(IServiceRegistryFlow flows) : IContextFactory
{
    public DetailContext<TPayload, TState> Create<TPayload, TState>(ChatContext context)
        where TPayload : BasePayload where TState : struct, Enum
    {
        return new DetailContext<TPayload, TState>(context, flows);
    }
}