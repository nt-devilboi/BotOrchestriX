using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;

namespace BotOrchestriX;

internal class ContextFactory(IServiceRegistryFlow flows) : IContextFactory
{
    public DetailContext<TPayload> Create<TPayload>(ChatContext context)
        where TPayload : BasePayload

    {
        return new DetailContext<TPayload>(context, flows);
    }
}